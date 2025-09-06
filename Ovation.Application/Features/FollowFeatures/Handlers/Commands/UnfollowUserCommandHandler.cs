using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.FollowFeatures.Requests.Commands;

namespace Ovation.Application.Features.FollowFeatures.Handlers.Commands
{
    internal class UnfollowUserCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UnfollowUserCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UnfollowUserCommandRequest request, CancellationToken cancellationToken)
        {
            return await _followRepository.UnfollowUserAsync(request.UserId, request.FollowerId);
        }
    }
}
