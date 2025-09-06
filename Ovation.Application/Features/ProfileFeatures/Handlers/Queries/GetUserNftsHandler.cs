using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserNftsHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserNftsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserNftsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserNftsAsync(request.UserId, request.AuthUser, request.Parameters);
        }
    }
    
    internal class GetUserCollectionsHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserCollectionQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserCollectionQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserCollectionsAsync(request.UserId, request.AuthUser, request.Parameters);
        }
    }

    internal class GetUserTopNftsHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserTopNftsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserTopNftsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserTopNftsAsync(request.Limit, request.UserId);
        }
    }
}
