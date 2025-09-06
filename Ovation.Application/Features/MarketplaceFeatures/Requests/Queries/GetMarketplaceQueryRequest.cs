using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.MarketplaceFeatures.Requests.Queries
{
    public sealed record GetMarketplaceQueryRequest(
        string? Cursor = null, 
        int PageSize = 10, 
        string? SortDirection = null) : IRequest<ResponseData>;
}
