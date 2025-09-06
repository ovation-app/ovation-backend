using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.FollowFeatures.Requests.Queries;

namespace Ovation.Application.Features.FollowFeatures.Handlers.Queries
{
    internal class GetUserFollowersQueryHandlers : BaseHandler, IRequestHandler<GetUserFollowersQueryRequest, ResponseData>
    {
        public GetUserFollowersQueryHandlers(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory) { }

        public async Task<ResponseData> Handle(GetUserFollowersQueryRequest request, CancellationToken cancellationToken)
        {
            return await _followRepository.GetUserFollowersAsync(request.UserId, request.Page, request.AuthUser);
        }
    }
}
