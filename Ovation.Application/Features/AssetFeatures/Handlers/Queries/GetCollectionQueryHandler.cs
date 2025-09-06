using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AssetFeatures.Requests.Queries;

namespace Ovation.Application.Features.AssetFeatures.Handlers.Queries
{
    internal class GetCollectionQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetCollectionQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetCollectionQueryRequest request, CancellationToken cancellationToken)
        {
            return await _assetRepository.GetCollectionAsync(request.CollectionId);
        }
    }
}
