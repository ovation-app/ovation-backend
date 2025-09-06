using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetMostSoldQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetMostSoldQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetMostSoldQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetMostSoldAsync(request.Page, request.UserPath, request.UserId);
        }
    }
}
