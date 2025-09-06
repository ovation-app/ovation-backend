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
    class HandleUserTaskJob(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(HandleUserTaskJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData == null)
                return;

            var userId = jobData.GetString("UserId");
            var task = jobData.GetString("Task");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(task))
                return;

            await AddUserTaskAsync(task, new Guid(userId));
        }

        private async Task AddUserTaskAsync(string taskName, Guid userId)
        {
            if (!await IsTaskDone(taskName, userId))
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var task = new UserTask
                    {
                        UserId = userId.ToByteArray(),
                        TaskName = taskName,
                        CompletedAt = DateTime.UtcNow
                    };

                    await _context.UserTasks.AddAsync(task);

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    if (await IsProfileCompleteTasksDone(userId))
                        await ProfileCompleteBadgeEarned(userId);
                }
                catch (Exception _)
                {
                    SentrySdk.CaptureException(_);
                    await _unitOfWork.RollbackAsync();
                }
            }
        }

        private async Task<bool> IsTaskDone(string taskName, Guid userId)
        {
            var entity = await _context.UserTasks
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.TaskName == taskName);

            if (entity == null)
                return false;

            return true;
        }

        private async Task<bool> IsProfileCompleteTasksDone(Guid userId)
        {
            var entity = await _context.UserTasks
                .Where(_ => _.UserId == userId.ToByteArray())
                .ToListAsync();

            if (entity == null)
                return false;

            if (entity.Count > 6) return true;

            else return false;
        }

        private async Task ProfileCompleteBadgeEarned(Guid userId)
        {
            var notification = new List<NotificationDto>
            {
                new NotificationDto
                {
                    InitiatorId = null,
                    ReceiverId = userId,
                    Message = Constant._NotificationMessage[NotificationReference.Badge],
                    Title = Constant._NotificationTitle[NotificationReference.Badge],
                    Reference = NotificationReference.Badge.ToString(),
                    ReferenceId = BadgeStruct.ProfileComplete
                }
            };

            await SaveNotificationAsync(notification);
        }
    }
}
