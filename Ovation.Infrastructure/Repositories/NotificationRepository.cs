using Microsoft.EntityFrameworkCore;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Data;

namespace Ovation.Persistence.Repositories
{
    internal class NotificationRepository(OvationDbContext context, IUnitOfWork unitOfWork)
        : BaseRepository<UserNotification>(context, unitOfWork), INotificationRepository
    {
        public async Task<ResponseData> GetUserNotificationsAsync(Guid userId, int page)
        {
            var responseDate = new ResponseData();

            try
            {
                responseDate.Data = await _context.UserNotifications
                    .Where(_ => _.ReceiverId == userId.ToByteArray())
                    .Include(_ => _.Initiator)
                    .OrderByDescending(p => p.CreatedDate)
                    .AsNoTracking()
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        Id = new Guid(x.Id),
                        x.Reference,
                        x.ReferenceId,
                        x.Title,
                        x.Message,
                        IsFollowing = x.Receiver.UserFollowerFollowers.Where(_ => _.FollowerId == userId.ToByteArray()
                        && _.UserId == x.InitiatorId).FirstOrDefault() != null ? true : false,
                        Initiator = x.Initiator != null ? new
                        {
                            InitiatorId = new Guid(x.InitiatorId!),
                            x.Initiator.Username,
                            x.Initiator.UserProfile!.DisplayName,
                            x.Initiator.UserProfile.ProfileImage,
                            x.Initiator.UserProfile.Bio,
                            x.Initiator.UserStat.Following,
                            x.Initiator.UserStat.Followers,
                            isVerified = x.Initiator.VerifiedUsers != null ? true : false,

                        } : null
                    })
                    .ToListAsync();

                responseDate.Status = true;

                return responseDate;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserPendingNotificationAsync(Guid userId)
        {
            var responseDate = new ResponseData();

            try
            {
                responseDate.Data = await _context.UserNotifications
                    .Where(_ => _.ReceiverId == userId.ToByteArray() && _.Viewed == 0)
                    .CountAsync();

                responseDate.Status = true;

                return responseDate;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> ReadAllNotificationsAsync(Guid userId)
        {
            //var entities = await _context.UserNotifications
            //    .Where(_ => _.ReceiverId == userId.ToByteArray() && _.Viewed == 0)
            //    .ToListAsync();

            //if (entities == null || entities.Count < 1) return new ResponseData { Status = true, Message = "All Notifications Read!" };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _context.UserNotifications
                .Where(item => item.ReceiverId == userId.ToByteArray())
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(item => item.Viewed, 1));

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, Message = "All Notifications Read" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> ReadNotificationAsync(Guid notificationId, Guid userId)
        {
            var entity = await _context.UserNotifications
                .FirstOrDefaultAsync(_ => _.ReceiverId == userId.ToByteArray() && _.Id == notificationId.ToByteArray());

            if (entity == null) return new ResponseData { Status = false, Message = "Notification not found" };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (entity.Viewed == 1) return new ResponseData { Status = true, Message = "Notification viewed" };

                entity.Viewed = 1;

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, Message = "Notification viewed" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }
    }
}
