using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AssetFeatures.Requests.Queries;

namespace Ovation.Application.Features.AssetFeatures.Handlers.Queries
{
    internal class GetOwnerDistributionQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetOwnerDistributionQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetOwnerDistributionQueryRequest request, CancellationToken cancellationToken)
        {
            return await _assetRepository.GetCollectionOwnerDistributionAsync(request.CollectionId);
        }
    }
}
