using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.MarketplaceFeatures.Requests.Queries;

namespace Ovation.Application.Features.MarketplaceFeatures.Handlers.Queries
{
    internal class GetMarketplaceQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetMarketplaceQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetMarketplaceQueryRequest request, CancellationToken cancellationToken)
        {
            return await _marketplaceRepository.GetMarketPlaceDataAsync(
                request.Cursor, 
                request.PageSize, 
                request.SortDirection);
        }
    }
}
