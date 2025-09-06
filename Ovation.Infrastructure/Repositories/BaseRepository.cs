using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.ApiUser;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Data;
using Ovation.Persistence.Observability.Interface;
using Ovation.Persistence.Services;
using System.Globalization;

namespace Ovation.Persistence.Repositories;

public class BaseRepository<T> : IBaseRepository<T>
{
    protected readonly OvationDbContext _context;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IHubContext<NotificationService, INotificationService> _hubContext;
    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly IDomainEvents _domainEvent;
    protected readonly ISentryService _sentryService;

    internal const int perPage = 100;
    internal const int active = 1;
    internal const int inactive = 0;

    public BaseRepository(OvationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public BaseRepository(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        var scope = serviceScopeFactory.CreateScope();

        _context = scope.ServiceProvider.GetRequiredService<OvationDbContext>();
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationService, INotificationService>>();
        _domainEvent = scope.ServiceProvider.GetRequiredService<IDomainEvents>();
        _sentryService = scope.ServiceProvider.GetRequiredService<ISentryService>();

    }

    public async Task CreateAsync(T entity)
    {
        await _context.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _context.Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Update(entity);
    }

    protected static string? CalculatePrice(Trade x)
    {
        if (x == null || (string.IsNullOrWhiteSpace(x.TradePrice) && string.IsNullOrWhiteSpace(x.FloorPrice)))
            return null;

        string? key = !string.IsNullOrEmpty(x.TradeSymbol)
            ? x.TradeSymbol.Trim().ToLower()
            : x.Chain?.Trim().ToLower();

        var amount = x.Chain == Constant.Solana || x.Chain == Constant.Tezos ? x.TradePrice : x.FloorPrice;

        if (string.IsNullOrEmpty(key) || !Constant._chainsToValue.TryGetValue(key, out decimal rate))
            return null;

        if (!double.TryParse(amount, NumberStyles.Float, CultureInfo.InvariantCulture, out double price))
            return null;

        double result = Math.Round(price * (double)rate, 2);
        return result.ToString("F2");
    }

    public async Task SaveNotificationAsync(List<NotificationDto> notifications)
    {
        try
        {
            foreach (var notification in notifications)
            {
                var notif = new UserNotification
                {
                    Id = Guid.NewGuid().ToByteArray(),
                    InitiatorId = notification.InitiatorId.HasValue ? notification.InitiatorId.Value.ToByteArray() : null,
                    ReceiverId = notification.ReceiverId.ToByteArray(),
                    Message = notification.Message,
                    Reference = notification.Reference,
                    Title = notification.Title,
                    ReferenceId = notification.ReferenceId
                };

                if (notification.Reference == NotificationReference.Badge.ToString())
                {
                    var badgeName = notification.ReferenceId.Split('-');
                    var query = $"{badgeName.First().Trim()}%";

                    await _context.Database.ExecuteSqlRawAsync(@"
                            UPDATE user_badges
                            SET Active = 0
                            WHERE BadgeName LIKE {0} AND UserId = {1}", query, notification.ReceiverId.ToByteArray());

                    await AddUserBadgeAsync(notification.ReferenceId, notification.ReceiverId);

                    await _context.UserNotifications.AddAsync(notif);
                }
                await _context.SaveChangesAsync();
                //await transaction.CommitAsync();                    
            }
            await SendNotificationAsync(notifications);
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            //await transaction.RollbackAsync();
        }
    }

    private async Task UpdateBadgeCountAsync(byte[] userId)
    {
        var badgeCount = await _context.UserBadges.Where(_ => _.UserId == userId && _.Active == 1).CountAsync();

        var stat = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId);

        if (stat != null)
        {
            stat.BadgeEarned = badgeCount;

            await _context.SaveChangesAsync();
        }
    }

    protected async Task SendNotificationAsync(List<NotificationDto> notificationDtos)
    {
        foreach (var notificationDto in notificationDtos)
        {
            if (Constant._userIdToConnectionId.TryGetValue(notificationDto.ReceiverId, out string connectionId) && _hubContext.Clients != null)
            {
                await _hubContext.Clients.Client(connectionId).ReceivedNotification(notificationDto);
            }
            else
            {
                // Queue message for later delivery
                if (!Constant._offlineNotification.ContainsKey(notificationDto.ReceiverId))
                {
                    Constant._offlineNotification[notificationDto.ReceiverId] = new List<NotificationDto>();
                }
                Constant._offlineNotification[notificationDto.ReceiverId].Add(notificationDto);
            }
        }

    }

    private async Task AddUserBadgeAsync(string badgeName, Guid receiverId)
    {
        var isBadgeEarned = await _context.UserBadges.AsNoTracking().FirstOrDefaultAsync(b => b.BadgeName == badgeName && b.UserId == receiverId.ToByteArray());

        if (isBadgeEarned == null)
        {
            var badge = new UserBadge
            {
                BadgeName = badgeName,
                EarnedAt = DateTime.UtcNow,
                Active = 1,
                UserId = receiverId.ToByteArray()
            };

            await _context.UserBadges.AddAsync(badge);

        }
        else if (isBadgeEarned.Active == 0)
        {
            isBadgeEarned.Active = 1;
            _context.UserBadges.Update(isBadgeEarned);
        }

        await UpdateBadgeCountAsync(receiverId.ToByteArray());

    }


    protected string EncodeIntegerToBase64(long number)
    {
        byte[] bytes = BitConverter.GetBytes(number);

        return Convert.ToBase64String(bytes);
    }

    protected int DecodeBase64ToInteger(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);

        return BitConverter.ToInt32(bytes, 0);
    }

    protected string? EncodeToBase64(long? longValue, decimal? decimalValue)
    {
        if (longValue == null || decimalValue == null)
            return null;
        // Convert the int value to byte array
        byte[] intBytes = BitConverter.GetBytes(longValue.Value);

        // Convert the decimal value to double and then to byte array
        byte[] decimalBytes = BitConverter.GetBytes((double)decimalValue.Value);

        // Combine the two byte arrays
        byte[] combinedBytes = new byte[intBytes.Length + decimalBytes.Length];
        Buffer.BlockCopy(intBytes, 0, combinedBytes, 0, intBytes.Length);
        Buffer.BlockCopy(decimalBytes, 0, combinedBytes, intBytes.Length, decimalBytes.Length);
        // Convert the combined byte array to a Base64 string
        return Convert.ToBase64String(combinedBytes);
    }

    // Function to decode Base64 back to int and decimal
    protected (long, decimal) DecodeFromBase64(string base64String)
    {
        // Decode the Base64 string to byte array
        byte[] decodedBytes = Convert.FromBase64String(base64String);

        // Extract the int and decimal byte arrays from the decoded byte array
        int intBytesLength = sizeof(long);
        int decimalBytesLength = sizeof(double);

        byte[] decodedIntBytes = new byte[intBytesLength];
        byte[] decodedDecimalBytes = new byte[decimalBytesLength];

        // Copy the byte arrays for int and decimal from the decoded byte array
        Buffer.BlockCopy(decodedBytes, 0, decodedIntBytes, 0, intBytesLength);
        Buffer.BlockCopy(decodedBytes, intBytesLength, decodedDecimalBytes, 0, decimalBytesLength);

        // Convert byte arrays back to original values
        int decodedIntValue = BitConverter.ToInt32(decodedIntBytes, 0);
        decimal decodedDecimalValue = (decimal)BitConverter.ToDouble(decodedDecimalBytes, 0);

        // Return both values as a tuple
        return (decodedIntValue, decodedDecimalValue);
    }

    public static string EncodeDateTimeToBase64(DateTime? dateTime)
    {
        if (dateTime == null)
            return string.Empty;

        long ticks = dateTime.Value.ToBinary(); // Includes Kind (UTC/Local)
        byte[] bytes = BitConverter.GetBytes(ticks);
        return Convert.ToBase64String(bytes);
    }

    public static DateTime? DecodeBase64ToDateTime(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
            return null;

        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            long ticks = BitConverter.ToInt64(bytes, 0);
            return DateTime.FromBinary(ticks);
        }
        catch
        {
            return null; // Return null on invalid input
        }
    }


}

//  dotnet ef dbcontext scaffold 'Server=127.0.0.1;port=3308;Database=ovation_db;Uid=root;Pwd=Pa$$w0rd@5'  Pomelo.EntityFrameworkCore.MySql --output-dir ../Ovation.Domain/Entities --context-dir Data --namespace Ovation.Domain.Entities --context-namespace Ovation.Persistence.Data --context OvationDbContext -f --no-onconfiguring