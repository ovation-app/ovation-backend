using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserBadgesQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserBadgesQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserBadgesQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserBadgesAsync(request.UserId, request.Page);
        }
    }
}
