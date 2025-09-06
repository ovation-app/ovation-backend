using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.SearchFeatures.Requests.Queries;

namespace Ovation.Application.Features.SearchFeatures.Handlers.Queries
{
    internal class SearchCollectionQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<SearchCollectionQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(SearchCollectionQueryRequest request, CancellationToken cancellationToken)
        {
            return await _searchRepository.FindNftCollectionAsync(request.Query, request.Cursor, request.UserId);
        }
    }
}
