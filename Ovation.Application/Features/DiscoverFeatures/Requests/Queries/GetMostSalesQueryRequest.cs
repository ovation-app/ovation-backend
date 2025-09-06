using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.DiscoverFeatures.Requests.Queries
{
    public sealed record GetMostSalesQueryRequest(int Page, string UserPath, Guid UserId) : IRequest<ResponseData>;
}
