using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Data;
using Quartz;
using Ovation.Application.Constants;
using Ovation.Application.DTOs.Enums.Badges;
using System.Globalization;

namespace Ovation.Persistence.Services.BackgroundServices
{
    class BaseJobService
    {
        protected readonly OvationDbContext _context;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IHubContext<NotificationService, INotificationService> _hubContext;
        protected readonly IServiceScope _serviceScope;
        protected readonly ISchedulerFactory _schedulerFactory;
        protected readonly IHttpClientFactory _factory;

        public BaseJobService(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        {
            var scope = serviceScopeFactory.CreateScope();
            _serviceScope = scope;

            _context = scope.ServiceProvider.GetRequiredService<OvationDbContext>();
            _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationService, INotificationService>>();
            _schedulerFactory = schedulerFactory;
            _factory = factory;
        }

        protected async Task SaveNotificationAsync(List<NotificationDto> notifications)
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



        protected async Task UpdateUserStatsRelatedBadge(UserStat? stats, Guid userId, decimal? topValue)
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


            if (topValue != null && topValue >= 100)
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
        }

        private async Task HandleBadgesSyncAsync(List<NotificationDto> notifications)
        {
            foreach (var notification in notifications)
            {

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
        }

        protected async Task UpdateBadgeCount(byte[] userId)
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

        protected async Task UpdateUserHighestValuedNft(byte[] userId)
        {
            var nfts = await _context.UserNftData.Where(_ => _.UserId == userId).AsNoTracking().Take(6000).ToListAsync();

            var netWorth = 0.00M;

            try
            {
                if (nfts != null && nfts.Count > 0)
                {
                    var topNft = new UserNftDatum();
                    var value = 0.00M;
                    var native = 0.00M;
                    foreach (var nft in nfts)
                    {
                        if (nft.Chain == Constant.Solana || nft.Chain == Constant.Tezos)
                        {
                            if (!string.IsNullOrEmpty(nft.LastTradePrice))
                            {
                                if (topNft == null)
                                {
                                    topNft = nft;
                                    value = Math.Round(decimal.Parse(nft.LastTradePrice ?? "0.00", NumberStyles.Float, CultureInfo.InvariantCulture) *
                                        (decimal)Constant._chainsToValue[nft.Chain!.Trim().ToLower()], 2);

                                    native = decimal.Parse(nft.LastTradePrice ?? "0.00", NumberStyles.Float, CultureInfo.InvariantCulture);

                                    netWorth += value;
                                }
                                else
                                {
                                    var newValue = Math.Round(decimal.Parse(nft.LastTradePrice ?? "0.00", NumberStyles.Float, CultureInfo.InvariantCulture) *
                                        (decimal)Constant._chainsToValue[nft.Chain!.Trim().ToLower()], 2);

                                    netWorth += newValue;

                                    if (newValue > value)
                                    {
                                        topNft = nft;
                                        value = newValue;

                                        native = decimal.Parse(nft.LastTradePrice ?? "0.00", NumberStyles.Float, CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(nft.FloorPrice))
                            {
                                if (topNft == null)
                                {
                                    topNft = nft;
                                    value = Math.Round(decimal.Parse(nft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture) *
                                        (decimal)Constant._chainsToValueFloor[nft.Chain!.Trim().ToLower()], 2);

                                    netWorth += value;

                                    native = decimal.Parse(nft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    var newValue = Math.Round(decimal.Parse(nft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture) *
                                        (decimal)Constant._chainsToValueFloor[nft.Chain!.Trim().ToLower()], 2);

                                    if (newValue > value)
                                    {
                                        topNft = nft;
                                        value = newValue;
                                        native = decimal.Parse(nft.FloorPrice, NumberStyles.Float, CultureInfo.InvariantCulture);
                                    }

                                    netWorth += newValue;
                                }
                            }
                        }
                    }

                    var stat = await _context.UserStats.FirstOrDefaultAsync(_ => _.UserId == userId);

                    if (value > 0)
                    {
                        var highNft = await _context.UserHighestNfts.FirstOrDefaultAsync(_ => _.UserId == userId);

                        if (highNft != null)
                        {

                            highNft.Worth = native;
                            highNft.WalletId = topNft.UserWalletId;
                            highNft.Chain = topNft.Chain;
                            highNft.UserId = userId;
                            highNft.Name = topNft.Name;
                            highNft.ImageUrl = topNft.ImageUrl;
                            highNft.TradeSymbol = topNft.LastTradeSymbol;
                            highNft.Usd = value;
                            highNft.NftId = topNft.Id;
                        }
                        else
                        {
                            var userHighNft = new UserHighestNft
                            {
                                Worth = native,
                                WalletId = topNft.UserWalletId,
                                Chain = topNft.Chain,
                                UserId = userId,
                                Name = topNft.Name,
                                ImageUrl = topNft.ImageUrl,
                                TradeSymbol = topNft.LastTradeSymbol,
                                Usd = value,
                                NftId = topNft.Id
                            };

                            await _context.UserHighestNfts.AddAsync(userHighNft);
                        }
                    }

                    if (stat != null)
                    {
                        stat.Networth = netWorth;
                        stat.TotalNft = nfts.Count();
                    }

                    await _context.SaveChangesAsync();

                    await UpdateUserStatsRelatedBadge(stat, new Guid(userId), value);

                    await UpdateBadgeCount(userId);
                }

            }
            catch (Exception _)
            {
            }
        }
    }
}
