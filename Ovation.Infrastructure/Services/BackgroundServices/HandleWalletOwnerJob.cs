using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Persistence.Data;
using Quartz;
using Ovation.Application.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace Ovation.Persistence.Services.BackgroundServices
{
    class HandleWalletOwnerJob(IServiceScopeFactory serviceScopeFactory, ISchedulerFactory schedulerFactory, IHttpClientFactory factory)
        : BaseJobService(serviceScopeFactory, schedulerFactory, factory), IJob
    {
        internal const string Name = nameof(HandleWalletOwnerJob);
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;

            if (jobData == null)
                return;

            var address = jobData.GetString("Address");

            if (string.IsNullOrEmpty(address))
                return;

            await HandleUnauthorizeWalletOwner(address);
        }

        private async Task HandleUnauthorizeWalletOwner(string address, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var results = await _context.UserWallets.Where(u => u.WalletAddress == address && u.WalletId == null).ToListAsync();

                if (results != null)
                {
                    var notifications = new List<NotificationDto>();
                    foreach (var result in results)
                    {
                        IScheduler schedular = await schedulerFactory.GetScheduler();
                        schedular = await schedulerFactory.GetScheduler();
                        var id = new Guid(result.Id).ToString();
                        var data = new JobDataMap
                        {
                            {"WalletId", id  },
                        };


                        ITrigger trigger = TriggerBuilder.Create()
                            .WithIdentity($"delete-wallet-{id}")
                            .ForJob(DeleteWalletDataJob.Name)
                            .UsingJobData(data)
                            .StartNow()
                            .Build();

                        await schedular.ScheduleJob(trigger);

                        notifications.Add(new NotificationDto
                        {
                            InitiatorId = null,
                            ReceiverId = new Guid(result.UserId),
                            Message = $"{Constant._NotificationMessage[NotificationReference.WalletOwnership]} {result.WalletAddress}",
                            Title = Constant._NotificationTitle[NotificationReference.WalletOwnership],
                            Reference = NotificationReference.WalletOwnership.ToString(),
                            ReferenceId = null
                        });
                    }
                    await SaveNotificationAsync(notifications);
                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
            }
        }
    }
}
