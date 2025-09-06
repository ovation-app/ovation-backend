using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.DTOs.Enums.Badges;
using Ovation.Domain.Entities;
using Quartz;

namespace Ovation.Persistence.Services.BackgroundServices
{
    class HandleReferralJob(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(HandleReferralJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData == null)
                return;

            var userId = jobData.GetString("UserId");
            var referral = jobData.GetString("Referral");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(referral))
                return;

            await HandleReferralAsync(referral, new Guid(userId));
        }

        private async Task HandleReferralAsync(string referral, Guid userId)
        {
            var affilation = await _context.UserAffilations
                .FirstOrDefaultAsync(_ => _.Code == referral);

            if (affilation != null)
            {
                var userReferral = new UserReferral
                {
                    InviteeId = userId.ToByteArray(),
                    InviterId = affilation.UserId
                };

                affilation.Invited += 1;
                await _context.UserReferrals.AddAsync(userReferral);

                var notification = new List<NotificationDto>();

                switch (affilation.Invited)
                {
                    case 5:
                        if (!await IsBadgeEarnedAsync(BadgeStruct.UserInvites5, affilation.UserId))
                        {
                            AddNotification(BadgeStruct.UserInvites5, new Guid(affilation.UserId), ref notification);
                        }
                        break;

                    case 10:
                        if (!await IsBadgeEarnedAsync(BadgeStruct.UserInvites10, affilation.UserId))
                        {
                            AddNotification(BadgeStruct.UserInvites10, new Guid(affilation.UserId), ref notification);
                        }
                        break;

                    case 25:
                        if (!await IsBadgeEarnedAsync(BadgeStruct.UserInvites25, affilation.UserId))
                        {
                            AddNotification(BadgeStruct.UserInvites25, new Guid(affilation.UserId), ref notification);
                        }
                        break;

                    case 50:
                        if (!await IsBadgeEarnedAsync(BadgeStruct.UserInvites50, affilation.UserId))
                        {
                            AddNotification(BadgeStruct.UserInvites50, new Guid(affilation.UserId), ref notification);
                        }
                        break;

                    default:
                        break;
                }

                await _unitOfWork.SaveChangesAsync();

                await SaveNotificationAsync(notification);
            }
        }

        private void AddNotification(string badgeName, Guid userId, ref List<NotificationDto> notification)
        {
            notification.Add(new NotificationDto
            {
                InitiatorId = null,
                ReceiverId = userId,
                Message = Constant._NotificationMessage[NotificationReference.Badge],
                Title = Constant._NotificationTitle[NotificationReference.Badge],
                Reference = NotificationReference.Badge.ToString(),
                ReferenceId = badgeName
            });
        }

        private async Task<bool> IsBadgeEarnedAsync(string badgeName, byte[] userId)
        {
            var isEarned = await _context.UserBadges
                .FirstOrDefaultAsync(_ => _.BadgeName == badgeName && _.UserId == userId);

            if (isEarned == null) return false;

            return true;
        }
    }
}
