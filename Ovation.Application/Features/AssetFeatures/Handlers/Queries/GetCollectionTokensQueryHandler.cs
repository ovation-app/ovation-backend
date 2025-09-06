using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AssetFeatures.Requests.Queries;

namespace Ovation.Application.Features.AssetFeatures.Handlers.Queries
{
    internal class GetCollectionTokensQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetCollectionTokensQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetCollectionTokensQueryRequest request, CancellationToken cancellationToken)
        {
            return await _assetRepository.GetCollectionTokensAsync(request.CollectionId, request.Parameters);
        }
    }
}
