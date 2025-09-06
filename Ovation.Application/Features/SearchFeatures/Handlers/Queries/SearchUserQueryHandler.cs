using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.SearchFeatures.Requests.Queries;

namespace Ovation.Application.Features.SearchFeatures.Handlers.Queries
{
    internal class SearchUserQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<SearchUserQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(SearchUserQueryRequest request, CancellationToken cancellationToken)
        {
            return await _searchRepository.FindUserAsync(request.Query, request.Page, request.UserId);
        }
    }
}
