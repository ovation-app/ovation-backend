using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.FollowFeatures.Requests.Commands
{
    public sealed record FollowUserCommandRequest(Guid UserId, Guid FollowerId) : IRequest<ResponseData>;
}
