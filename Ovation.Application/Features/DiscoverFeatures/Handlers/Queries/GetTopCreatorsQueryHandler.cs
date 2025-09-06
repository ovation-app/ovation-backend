using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetTopCreatorsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetTopCreatorsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetTopCreatorsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetTopCreatorsAsync(request.Page, request.UserPath);
        }
    }
}
