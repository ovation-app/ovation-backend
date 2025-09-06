using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.DiscoverFeatures.Requests.Queries
{
    public sealed record GetMostViewedQueryRequest(int Page, Guid UserId) : IRequest<ResponseData>;
}
