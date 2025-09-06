using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.FollowFeatures.Requests.Queries
{
    public sealed record GetUserFollowersQueryRequest(Guid UserId, int Page, Guid AuthUser) : IRequest<ResponseData>;
}
