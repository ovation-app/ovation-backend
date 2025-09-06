using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetTopNftHolderQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetTopNftHolderQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetTopNftHolderQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetTopNftHoldersAsync(request.Page, request.UserPath, request.UserId);
        }
    }
}
