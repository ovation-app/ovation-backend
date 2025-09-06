using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.FollowFeatures.Requests.Queries;

namespace Ovation.Application.Features.FollowFeatures.Handlers.Queries
{
    internal class GetUserFollowingsQueryHandler : BaseHandler, IRequestHandler<GetUserFollowingsQueryRequest, ResponseData>
    {
        public GetUserFollowingsQueryHandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory) { }

        public async Task<ResponseData> Handle(GetUserFollowingsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _followRepository.GetUserFollowingAsync(request.UserId, request.Page, request.AuthUser);
        }
    }
}
