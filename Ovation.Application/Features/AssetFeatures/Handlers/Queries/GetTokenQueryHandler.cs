using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AssetFeatures.Requests.Queries;

namespace Ovation.Application.Features.AssetFeatures.Handlers.Queries
{
    internal class GetTokenQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetTokenQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetTokenQueryRequest request, CancellationToken cancellationToken)
        {
            return await _assetRepository.GetTokenAsync(request.TokenId);
        }
    }

    internal class GetProfileTokenQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetProfileTokenQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetProfileTokenQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetTokenAsync(request.AssetId, request.UserId);
        }
    }
}
