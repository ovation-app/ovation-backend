using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    class GetFeaturedProfileQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetFeaturedProfileQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetFeaturedProfileQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetFeaturedProfilesAsync(request.UserId);
        }
    }
}
