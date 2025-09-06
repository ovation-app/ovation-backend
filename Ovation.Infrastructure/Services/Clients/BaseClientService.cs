using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.DTOs.Enums.Badges;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Data;
using Ovation.Persistence.Observability.Interface;
using Quartz;
using System.Globalization;
using System.Numerics;

namespace Ovation.Persistence.Services.Clients
{
    abstract class BaseClientService
    {
        protected readonly OvationDbContext _context;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IHubContext<NotificationService, INotificationService> _hubContext;
        protected readonly IServiceScope _serviceScope;
        protected readonly ISchedulerFactory _schedulerFactory;
        protected readonly IHttpClientFactory _factory;
        protected readonly ISentryService _sentryService;
        protected readonly ISpan? _span;

        public BaseClientService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        {
            var scope = serviceScopeFactory.CreateScope();
            _serviceScope = scope;

            _context = scope.ServiceProvider.GetRequiredService<OvationDbContext>();
            _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationService, INotificationService>>();
            _sentryService = scope.ServiceProvider.GetRequiredService<ISentryService>();
            _schedulerFactory = schedulerFactory;
            _factory = factory;

            _span = _sentryService.StartSpan("wallet.nft.fetch", "Initiated wallet nfts and transacion fetch");
        }

        protected abstract Task GetUserNftsAsync(string address, Guid userId, string? chain = default);
        protected abstract Task GetUserNftTransactionsAsync(string address, Guid userId, string? chain = default);

        internal abstract Task<int> GetNftCountAsync(string address, string? chain = default);

        public string HexToBigInteger(string? hex)
        {
            if (string.IsNullOrEmpty(hex)) return string.Empty;

            try
            {
                if (hex.StartsWith("0x") || hex.StartsWith("0X"))
                    hex = hex.Substring(2);

                return BigInteger.Parse(hex, NumberStyles.HexNumber).ToString();
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return string.Empty;
            }
        }

        protected async Task SyncCustodyDate(string address, byte[] id)
        {
            try
            {
                var transactions = await _context.UserNftTransactions
                        .Where(_ => _.UserWalletId == id)
                        .Select(x => new
                        {
                            x.Name,
                            x.TranxDate,
                            x.Chain,
                            x.ContractAddress,
                            x.ContractName,
                            x.ContractTokenId,
                            x.TokenId,
                            x.To
                        })
                        .ToListAsync();

                if (transactions != null && transactions.Count > 0)
                {
                    var nfts = await _context.UserNftData
                        .Where(_ => _.UserWalletId == id)
                        .ToListAsync();

                    if (nfts != null && nfts.Count > 0)
                    {
                        foreach (var nft in nfts)
                        {
                            try
                            {
                                var trnxDate = transactions.FirstOrDefault(_ => _.Chain == nft.Chain && _.To == address &&
                                (nft.Chain == Constant.Solana) ? _.ContractTokenId == nft.TokenAddress && _.Name == nft.Name :
                                    _.ContractAddress == nft.ContractAddress && (_.TokenId == nft.TokenId || _.ContractTokenId == nft.TokenAddress))?.TranxDate;

                                if (trnxDate != null)
                                    nft.CustodyDate = DateOnly.FromDateTime(trnxDate.Value);
                            }
                            catch (Exception _)
                            {
                                continue;
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }

        public async Task SaveNotificationAsync(Guid userId)
        {
            //var transaction = context.Database.CurrentTransaction;

            //if (transaction == null)
            //    transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var stats = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());
                var topNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

                var topValuedNft = topNft?.Usd ?? 0.00M;

                await UpdateUserNftRelatedBadge(stats, userId, topValuedNft);
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                //await transaction.RollbackAsync();
            }
        }

        private async Task UpdateUserNftRelatedBadge(UserStat? stats, Guid userId, decimal topValuedNft)
        {
            if (stats == null)
                return;

            await ResetNftRelatedBadge(userId);

            var notifications = new List<NotificationDto>();
            var referenceType = NotificationReference.Badge;
            if (stats.TotalNft >= 1)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = stats.TotalNft <= 4 ? BadgeStruct.NumberOfNft1 : stats.TotalNft <= 9 ? BadgeStruct.NumberOfNft5 : stats.TotalNft <= 24 ? BadgeStruct.NumberOfNft10 :
                    stats.TotalNft <= 49 ? BadgeStruct.NumberOfNft25 : stats.TotalNft <= 99 ? BadgeStruct.NumberOfNft50 : stats.TotalNft <= 249 ? BadgeStruct.NumberOfNft100 :
                    stats.TotalNft <= 499 ? BadgeStruct.NumberOfNft250 : BadgeStruct.NumberOfNft500
                };
                notifications.Add(notification);
            }

            if (stats.NftCreated >= 10)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = stats.NftCreated <= 49 ? BadgeStruct.Creator10 : stats.NftCreated <= 99 ?
                    BadgeStruct.Creator50 : stats.NftCreated <= 199 ? BadgeStruct.Creator100 : stats.NftCreated <= 499 ? BadgeStruct.Creator200 : BadgeStruct.Creator500
                };
                notifications.Add(notification);
            }

            if (stats.BluechipCount >= 1)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = stats.BluechipCount <= 2 ? BadgeStruct.BlueChip1 : stats.BluechipCount <= 4 ? BadgeStruct.BlueChip3 : stats.BluechipCount <= 9 ?
                    BadgeStruct.BlueChip5 : BadgeStruct.BlueChip10
                };
                notifications.Add(notification);
            }

            var worth = stats.Networth;
            if (worth >= 1000)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = worth <= 4999 ? BadgeStruct.PortfolioValue1k : worth <= 9999 ?
                    BadgeStruct.PortfolioValue5k : worth <= 24999 ? BadgeStruct.PortfolioValue10k : worth <= 49999
                    ? BadgeStruct.PortfolioValue25k : worth <= 99999 ? BadgeStruct.PortfolioValue50k : BadgeStruct.PortfolioValue100k
                };
                notifications.Add(notification);
            }

            var topValue = topValuedNft;
            if (topValue >= 100)
            {
                var notification = new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[referenceType],
                    Title = Constant._NotificationTitle[referenceType],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = topValue <= 499 ? BadgeStruct.TopValueNft100 : topValue <= 999 ? BadgeStruct.TopValueNft500 : topValue <= 9999 ? BadgeStruct.TopValueNft1k :
                    topValue <= 24999 ? BadgeStruct.TopValueNft10k : topValue <= 49999 ? BadgeStruct.TopValueNft25k : topValue <= 99999 ? BadgeStruct.TopValueNft50k :
                    BadgeStruct.TopValueNft100k
                };
                notifications.Add(notification);
            }

            await HandleBadgesSyncAsync(notifications);
            await UpdateBadgeCountAsync(userId.ToByteArray());
        }

        private async Task HandleBadgesSyncAsync(List<NotificationDto> notifications)
        {
            foreach (var notification in notifications)
            {
                var prefix = notification.ReferenceId?.Split('-')?.FirstOrDefault()?.Trim();

                string pattern = $"{prefix}%";
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE user_badges
                    SET Active = 0
                    WHERE BadgeName LIKE {0} AND UserId = {1}", pattern, notification.ReceiverId.ToByteArray());

                var badge = await _context.UserBadges
                    .FirstOrDefaultAsync(_ => _.UserId == notification.ReceiverId.ToByteArray() && _.BadgeName == notification.ReferenceId);

                if (badge != null)
                {
                    badge.Active = 1;
                }
                else
                {
                    var badgee = new UserBadge
                    {
                        BadgeName = notification.ReferenceId,
                        EarnedAt = DateTime.UtcNow,
                        Active = 1,
                        UserId = notification.ReceiverId.ToByteArray()
                    };

                    await _context.UserBadges.AddAsync(badgee);
                }

                await _context.SaveChangesAsync();
            }

            await SendNotificationAsync(notifications);
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

        private async Task ResetNftRelatedBadge(Guid userId)
        {
            var badges = new List<string>
            {
                BadgeStruct.BlueChip1,
                BadgeStruct.Creator10,
                BadgeStruct.NumberOfNft1,
                BadgeStruct.PortfolioValue1k,
                BadgeStruct.TopValueNft1k
            };

            foreach (var item in badges)
            {
                var prefix = item.Split('-')?.FirstOrDefault()?.Trim();

                string pattern = $"{prefix}%";
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE user_badges
                    SET Active = 0
                    WHERE BadgeName LIKE {0} AND UserId = {1}", pattern, userId.ToByteArray());
            }
        }

        private async Task SendNotificationAsync(List<NotificationDto> notificationDtos)
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
    }
}
