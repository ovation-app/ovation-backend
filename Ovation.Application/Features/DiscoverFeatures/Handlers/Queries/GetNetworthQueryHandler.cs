using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetNetworthQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetNetworthQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetNetworthQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetHighestNetWorth(request.Page, request.UserPath, request.UserId);
        }
    }
}
