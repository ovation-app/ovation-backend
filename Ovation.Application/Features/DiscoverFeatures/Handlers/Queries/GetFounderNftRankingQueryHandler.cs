using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetFounderNftRankingQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetFounderNftRankingQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetFounderNftRankingQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetFounderNftAsync(request.Page, request.UserPath, request.UserId);
        }
    }
}
