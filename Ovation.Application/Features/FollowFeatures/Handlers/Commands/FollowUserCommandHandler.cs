using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.FollowFeatures.Requests.Commands;

namespace Ovation.Application.Features.FollowFeatures.Handlers.Commands
{
    internal class FollowUserCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<FollowUserCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(FollowUserCommandRequest request, CancellationToken cancellationToken)
        {
            return await _followRepository.FollowUserAsync(request.UserId, request.FollowerId);
        }
    }
}
