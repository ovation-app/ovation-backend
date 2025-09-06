using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetMostSalesQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetMostSalesQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetMostSalesQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetMostSalesAsync(request.Page, request.UserPath, request.UserId);
        }
    }
}
