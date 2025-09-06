using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetMostFollowedQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetMostFollowedQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetMostFollowedQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetMostFollowedAsync(request.Page, request.UserId);
        }
    }
}
