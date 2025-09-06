using Ovation.Application.Common.Interfaces;
using Ovation.Application.Constants;
using Ovation.Persistence.Services.BackgroundServices;
using Ovation.Persistence.Services.Clients;
using Ovation.Persistence.Services.Clients.NFTScan;
using Quartz;

namespace Ovation.Persistence.Events
{
    sealed class DomainEvents(ISchedulerFactory schedulerFactory) : IDomainEvents
    {
        public async Task NFTVisibilityChangedEvent(Guid userId)
        {
            try
            {
                var triggerKey = new TriggerKey($"trigger-nft-visibility-changed-{userId}", "NFTVisibility");
                var schedular = await GetSchedulerAsync();

                if (await schedular.CheckExists(triggerKey))
                    return;

                var jobData = new JobDataMap
                {
                    { "UserId", userId.ToString()! }
                };

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(NftPrivacyChangedDataJob.Name)
                    .StartNow()
                    .UsingJobData(jobData)
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }

        public async Task TaskPerformedEvent(Guid userId, string task)
        {
            try
            {
                var triggerKey = new TriggerKey($"trigger-task-{task}-user-{userId}", "UserTask");
                var schedular = await GetSchedulerAsync();

                if (await schedular.CheckExists(triggerKey))
                    return;

                var jobData = new JobDataMap
                {
                    { "UserId", userId.ToString()! },
                    { "Task", task }
                };

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(HandleUserTaskJob.Name)
                    .UsingJobData(jobData)
                    .StartNow()
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }

        public async Task UserReferredEvent(Guid userId, string refferal)
        {
            try
            {
                var triggerKey = new TriggerKey($"trigger-referral-{refferal}-user-{userId}", "UserReferral");
                var schedular = await GetSchedulerAsync();

                if (await schedular.CheckExists(triggerKey))
                    return;

                var jobData = new JobDataMap
                {
                    { "UserId", userId.ToString()! },
                    { "Referral", refferal! }
                };

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(HandleReferralJob.Name)
                    .UsingJobData(jobData)
                    .StartNow()
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }

        public async Task WalletAddedEvent(Guid userId, string address, string chain, Guid? walletTypeId)
        {
            if (string.IsNullOrEmpty(chain)) return;

            var schedular = await GetSchedulerAsync();
            string triggerKey = null;
            string jobName = null;
            JobDataMap jobData = new();

            switch (chain.ToLower())
            {
                case Constant.Solana:
                    triggerKey = $"get-solana-user-nft-data-{address}";
                    jobName = SolanaService.Name;
                    jobData.Add("Address", address);
                    jobData.Add("UserId", userId.ToString());
                    break;

                case Constant.Archway:
                    triggerKey = $"get-archway-user-nft-data-{address}";
                    jobName = ArchwayService.Name;
                    jobData.Add("Address", address);
                    jobData.Add("UserId", userId.ToString());
                    break;

                case Constant.Cosmos:
                    // No job scheduled for Cosmos
                    return;

                case Constant.Tezos:
                    triggerKey = $"get-tezos-user-nft-data-{address}";
                    jobName = TezosService.Name;
                    jobData.Add("Address", address);
                    jobData.Add("UserId", userId.ToString());
                    break;

                case Constant.Ton:
                    // No job scheduled for Ton
                    return;

                case Constant.Stargaze:
                    // No job scheduled for Stargaze
                    return;

                case Constant.Abstract:
                    triggerKey = $"get-abstract-user-nft-data-{address}";
                    jobName = AbstractService.Name;
                    jobData.Add("Address", address);
                    jobData.Add("UserId", userId.ToString());
                    break;

                default:
                    triggerKey = $"get-evm-user-nft-data-{address}";
                    jobName = EvmsService.Name;
                    jobData.Add("Address", address);
                    jobData.Add("Chain", chain);
                    jobData.Add("UserId", userId.ToString());
                    break;
            }

            if (triggerKey == null || jobName == null)
                return;

            var quartzTriggerKey = new TriggerKey(triggerKey);

            if (await schedular.CheckExists(quartzTriggerKey))
                return;

            var trigger = TriggerBuilder.Create()
                .WithIdentity(quartzTriggerKey)
                .ForJob(jobName)
                .UsingJobData(jobData)
                .StartNow()
                .Build();

            await schedular.ScheduleJob(trigger);
        }

        public async Task WalletDeletedEvent(Guid id, string action)
        {
            try
            {
                var triggerKey = new TriggerKey($"delete-wallet-{id}", "WalletDelete");
                var schedular = await GetSchedulerAsync();

                if (await schedular.CheckExists(triggerKey))
                    return;

                var data = new JobDataMap
                {
                    {"WalletId", id.ToString() },
                    {"Group", action },
                };


                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(DeleteWalletDataJob.Name)
                    .UsingJobData(data)
                    .StartNow()
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }

        public async Task WalletOwnershipVerifiedEvent(string address)
        {
            try
            {
                var triggerKey = new TriggerKey($"trigger-wallet-owner-{address}", "WalletOwner");
                var schedular = await GetSchedulerAsync();

                if (await schedular.CheckExists(triggerKey))
                    return;

                var jobData = new JobDataMap
                {
                    { "Address", address! }
                };

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(HandleWalletOwnerJob.Name)
                    .UsingJobData(jobData)
                    .StartNow()
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }

        public async Task XAccountConnectedEvent(Guid userId)
        {
            try
            {
                var triggerKey = new TriggerKey($"trigger-x-metric-{userId}", "XPublicMetric");
                var schedular = await GetSchedulerAsync();

                if (await schedular.CheckExists(triggerKey))
                    return;

                var jobData = new JobDataMap
                {
                    { "UserId", userId.ToString()! }
                };

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(GetUserXMetricJob.Name)
                    .StartNow()
                    .UsingJobData(jobData)
                    .Build();

                await schedular.ScheduleJob(trigger);
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }


        private async Task<IScheduler> GetSchedulerAsync()
        {
            IScheduler schedular = await schedulerFactory.GetScheduler();
            schedular = await schedulerFactory.GetScheduler();

            return schedular;
        }
    }
}
