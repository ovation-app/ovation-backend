using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetTopContributorsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetTopContributorsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetTopContributorsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetTopContributorsAsync(request.Page);
        }
    }
}
