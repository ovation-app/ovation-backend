using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserFeaturedItemQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserFeaturedItemQueryRequest, List<FavNfts>>
    {
        public async Task<List<FavNfts>?> Handle(GetUserFeaturedItemQueryRequest request, CancellationToken cancellationToken)
        {
            return null; // await _profileRepository.GetFeatureItemsAsync(request.UserId);
        }
    }
}
