using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Persistence.Services.BackgroundServices;
using Ovation.Persistence.Services.Clients;
using Ovation.Persistence.Services.Clients.NFTScan;
using Quartz;

namespace Ovation.Persistence
{
    public static class BackgroudJobsServiceExtension
    {
        public static void ConfigureBackgroundServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(configure =>
            {
                //routine tasks

                //configure
                //.AddJob<SyncEvmNftDataJob>(c => c
                //.StoreDurably()
                //.WithIdentity(SyncEvmNftDataJob.Name))
                //.AddTrigger(trigger => trigger.ForJob(SyncEvmNftDataJob.Name).WithSimpleSchedule(
                //    schedule => schedule.WithIntervalInHours(24).RepeatForever()));

                //configure
                //.AddJob<SyncXFollowersDataJob>(c => c
                //.StoreDurably()
                //.WithIdentity(SyncXFollowersDataJob.Name))
                //.AddTrigger(trigger => trigger.ForJob(SyncXFollowersDataJob.Name).WithSimpleSchedule(
                //    schedule => schedule.WithIntervalInHours(24).RepeatForever()));

                //configure
                //.AddJob<GetUserNftCustodyDateJob>(c => c
                //.StoreDurably()
                //.WithIdentity(GetUserNftCustodyDateJob.Name))
                //.AddTrigger(trigger => trigger.ForJob(GetUserNftCustodyDateJob.Name).WithSimpleSchedule(
                //    schedule => schedule.WithIntervalInHours(24).RepeatForever()));

                //configure
                //.AddJob<SyncUserNftIdJob>(c => c
                //.StoreDurably()
                //.WithIdentity(SyncUserNftIdJob.Name))
                //.AddTrigger(trigger => trigger.ForJob(SyncUserNftIdJob.Name).WithSimpleSchedule(
                //    schedule => schedule.WithIntervalInHours(24).RepeatForever()));

                //Post action jobs

                configure
                .AddJob<GetEvmsCollectionDataJob>(c => c
                .StoreDurably()
                .WithIdentity(GetEvmsCollectionDataJob.Name));

                configure
                .AddJob<GetSolanaCollectionDataJob>(c => c
                .StoreDurably()
                .WithIdentity(GetSolanaCollectionDataJob.Name));

                configure
                .AddJob<GetTonCollectionDataJob>(c => c
                .StoreDurably()
                .WithIdentity(GetTonCollectionDataJob.Name));

                configure
                .AddJob<GetTezosCollectionDataJob>(c => c
                .StoreDurably()
                .WithIdentity(GetTezosCollectionDataJob.Name));

                configure
                .AddJob<GetStargazeCollectionDataJob>(c => c
                .StoreDurably()
                .WithIdentity(GetStargazeCollectionDataJob.Name));


                //Getting nft data after wallet connection
                configure
                .AddJob<StargazeService>(c => c
                .StoreDurably()
                .WithIdentity(StargazeService.Name));

                configure
                .AddJob<ArchwayService>(c => c
                .StoreDurably()
                .WithIdentity(ArchwayService.Name));

                configure
                .AddJob<SolanaService>(c => c
                .StoreDurably()
                .WithIdentity(SolanaService.Name));

                configure
                .AddJob<TezosService>(c => c
                .StoreDurably()
                .WithIdentity(TezosService.Name));

                configure
                .AddJob<TonService>(c => c
                .StoreDurably()
                .WithIdentity(TonService.Name));

                configure
                .AddJob<EvmsService>(c => c
                .StoreDurably()
                .WithIdentity(EvmsService.Name));
                
                configure
                .AddJob<AbstractService>(c => c
                .StoreDurably()
                .WithIdentity(AbstractService.Name));

                configure
                .AddJob<DeleteWalletDataJob>(c => c
                .StoreDurably()
                .WithIdentity(DeleteWalletDataJob.Name));

                configure
                .AddJob<HandleReferralJob>(c => c
                .StoreDurably()
                .WithIdentity(HandleReferralJob.Name));

                configure
                .AddJob<HandleUserTaskJob>(c => c
                .StoreDurably()
                .WithIdentity(HandleUserTaskJob.Name));

                configure
                .AddJob<HandleWalletOwnerJob>(c => c
                .StoreDurably()
                .WithIdentity(HandleWalletOwnerJob.Name));

                configure
                .AddJob<GetUserXMetricJob>(c => c
                .StoreDurably()
                .WithIdentity(GetUserXMetricJob.Name));

                configure
                .AddJob<NftPrivacyChangedDataJob>(c => c
                .StoreDurably()
                .WithIdentity(NftPrivacyChangedDataJob.Name));


                configure.UsePersistentStore(persistentOptions =>
                {
                    persistentOptions.UseMySql(cfg =>
                    {
                        cfg.ConnectionString = Environment.GetEnvironmentVariable("SCHEDULER_DB");
                    });
                    //dataSourceName: "scheduler");

                    persistentOptions.UseProperties = true;
                    persistentOptions.UseNewtonsoftJsonSerializer();
                });

                configure
                .UseDedicatedThreadPool(5);
            });

            services.AddQuartzHostedService(opt =>
            {
                opt.WaitForJobsToComplete = true;
            });
        }
    }
}
