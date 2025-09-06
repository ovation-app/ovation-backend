using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class NftRankingQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<NftRankingQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(NftRankingQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetNftRankingAsync(request.Page, request.UserPath);
        }
    }
}
