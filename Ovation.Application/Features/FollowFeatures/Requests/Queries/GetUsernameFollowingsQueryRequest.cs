using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.FollowFeatures.Requests.Queries
{
    public sealed record GetUsernameFollowingsQueryRequest(string Username, int Page, Guid AuthUser) : IRequest<ResponseData>;
}
