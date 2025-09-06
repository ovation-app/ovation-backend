using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.Repositories;
using Ovation.Persistence.Authentication;
using Ovation.Persistence.Data;
using Ovation.Persistence.Events;
using Ovation.Persistence.Observability;
using Ovation.Persistence.Observability.Interface;
using Ovation.Persistence.Repositories;
using Ovation.Persistence.Services;
using Ovation.Persistence.Services.BackgroundServices;
using Ovation.Persistence.Services.Clients;
using Ovation.Persistence.Services.Clients.NFTScan;

namespace Ovation.Persistence;

public static class ServiceExtensions
{
    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OvationDbContext>(
        options =>
        {
            options.UseMySql(Environment.GetEnvironmentVariable("OVATION_DB"),
                ServerVersion.AutoDetect(Environment.GetEnvironmentVariable("OVATION_DB")));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IAffilationRepository, AffilationRepository>();
        services.AddScoped<IBadgeRepository, BadgeRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<INuclearPlayGroundRepository, NuclearPlayGroundRepository>();
        services.AddScoped<INewsletterRepository, NewsletterRepository>();
        services.AddScoped<IPathRepository, PathRepository>();
        services.AddScoped<ISearchRepository, SearchRepository>();
        services.AddScoped<IDiscoverRepository, DiscoverRepository>();
        services.AddScoped<IDevRepository, DevRepository>();
        services.AddScoped<IAuthManager, AuthManager>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IWebhookRepository, WebhookRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IHomeRepository, HomeRepository>();
        services.AddScoped<IMarketplaceRepository, MarketplaceRepository>();

        services.AddScoped<IDomainEvents, DomainEvents>();
        services.AddScoped<ISentryService, SentryService>();

        services.AddScoped<CollectionPriceService>();
        services.AddTransient<EvmsService>();
        services.AddTransient<ArchwayService>();
        services.AddTransient<TezosService>();
        services.AddTransient<TonService>();
        services.AddTransient<SolanaService>();
        services.AddTransient<StargazeService>();
        services.AddTransient<GraphQLService>();
        services.AddTransient<AbstractService>();

        services.AddSignalRCore();
        services.AddScoped<NotificationService>();
        services.ConfigureClients();
        services.ConfigureBackgroundServices(configuration);

        services.AddScoped<BaseJobService>();
        services.AddTransient<SyncXFollowersDataJob>();
        services.AddTransient<GetUserXMetricJob>();
        services.AddTransient<HandleReferralJob>();
        services.AddTransient<HandleUserTaskJob>();
        services.AddTransient<HandleWalletOwnerJob>();
        services.AddTransient<NftPrivacyChangedDataJob>();

        services.AddTransient<XService>();
        //services.AddTransient<XOAuthHandler>();
    }

}