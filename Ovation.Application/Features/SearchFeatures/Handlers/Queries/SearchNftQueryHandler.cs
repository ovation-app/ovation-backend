using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.SearchFeatures.Requests.Queries;

namespace Ovation.Application.Features.SearchFeatures.Handlers.Queries
{
    internal class SearchNftQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<SearchNftQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(SearchNftQueryRequest request, CancellationToken cancellationToken)
        {
            return await _searchRepository.FindNftAsync(request.Query, request.Cursor, request.UserId);
        }
    }
}
