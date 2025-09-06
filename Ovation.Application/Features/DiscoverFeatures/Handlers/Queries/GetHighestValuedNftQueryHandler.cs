using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetHighestValuedNftQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetHighestValuedNftQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetHighestValuedNftQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetHighestValuedNftAsync(request.Page, request.UserPath, request.UserId);
        }
    }
}
