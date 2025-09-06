using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.DiscoverFeatures.Requests.Queries
{
    public sealed record GetMostFollowedQueryRequest(int Page, Guid UserId) : IRequest<ResponseData>;
}
