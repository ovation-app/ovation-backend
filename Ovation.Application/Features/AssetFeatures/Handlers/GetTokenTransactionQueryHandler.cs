using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AssetFeatures.Requests.Queries;

namespace Ovation.Application.Features.AssetFeatures.Handlers
{
    class GetTokenTransactionQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetTokenTransactionQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetTokenTransactionQueryRequest request, CancellationToken cancellationToken)
        {
            return await _assetRepository.GetTokenTransactionActivitiesAsync(request.TokenId);
        }
    }

    class GetProfileTokenTransactionQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetProfileTokenTransactionQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetProfileTokenTransactionQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetTokenTransactionActivitiesAsync(request.TokenId);
        }
    }
}
