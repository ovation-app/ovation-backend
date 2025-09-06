using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.Repositories;

namespace Ovation.Application.Features
{
    internal class BaseHandler
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserRepository _userRepository;
        protected readonly IAuthManager _authManager;
        protected readonly IProfileRepository _profileRepository;
        protected readonly IDevRepository _devRepository;
        protected readonly IFollowRepository _followRepository;
        protected readonly IAffilationRepository _affilationRepository;
        protected readonly IBadgeRepository _badgeRepository;
        protected readonly IDiscoverRepository _discoverRepository;
        protected readonly IFeedbackRepository _feedbackRepository;
        protected readonly INewsletterRepository _newsletterRepository;
        protected readonly INuclearPlayGroundRepository _nuclearPlayGroundRepository;
        protected readonly INotificationRepository _notificationRepository;
        protected readonly IPathRepository _pathRepository;
        protected readonly ISearchRepository _searchRepository;
        protected readonly IWalletRepository _walletRepository;
        protected readonly IWebhookRepository _webhookRepository;
        protected readonly IAssetRepository _assetRepository;
        protected readonly IHomeRepository _homeRepository;
        protected readonly IMarketplaceRepository _marketplaceRepository;


        protected readonly IDomainEvents _domainEvent;

        protected readonly IServiceScopeFactory _serviceScopeFactory;

        public BaseHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            var scope  = serviceScopeFactory.CreateScope();

            _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();   
            _authManager = scope.ServiceProvider.GetRequiredService<IAuthManager>();
            _profileRepository = scope.ServiceProvider.GetRequiredService<IProfileRepository>();
            _devRepository = scope.ServiceProvider.GetRequiredService<IDevRepository>();
            _followRepository = scope.ServiceProvider.GetRequiredService<IFollowRepository>();
            _affilationRepository = scope.ServiceProvider.GetRequiredService<IAffilationRepository>();
            _badgeRepository = scope.ServiceProvider.GetRequiredService<IBadgeRepository>();
            _discoverRepository = scope.ServiceProvider.GetRequiredService<IDiscoverRepository>();
            _feedbackRepository = scope.ServiceProvider.GetRequiredService<IFeedbackRepository>();
            
            _newsletterRepository = scope.ServiceProvider.GetRequiredService<INewsletterRepository>();
            _nuclearPlayGroundRepository = scope.ServiceProvider.GetRequiredService<INuclearPlayGroundRepository>();

            _notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            _pathRepository = scope.ServiceProvider.GetRequiredService<IPathRepository>();
            _searchRepository = scope.ServiceProvider.GetRequiredService<ISearchRepository>();
            _walletRepository = scope.ServiceProvider.GetRequiredService<IWalletRepository>();
            _walletRepository = scope.ServiceProvider.GetRequiredService<IWalletRepository>();
            _webhookRepository = scope.ServiceProvider.GetRequiredService<IWebhookRepository>();
            _assetRepository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
            _homeRepository = scope.ServiceProvider.GetRequiredService<IHomeRepository>();
            _marketplaceRepository = scope.ServiceProvider.GetRequiredService<IMarketplaceRepository>();

            _domainEvent = scope.ServiceProvider.GetRequiredService<IDomainEvents>();
        }
    }
}
