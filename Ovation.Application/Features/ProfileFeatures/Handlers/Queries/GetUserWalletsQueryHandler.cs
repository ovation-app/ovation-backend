using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserWalletsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserWalletsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserWalletsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserWalletsAsync(request.UserId);
        }
    }

    internal class GetWalletsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetWalletsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetWalletsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetWalletsAsync(request.UserId);
        }
    }
}
