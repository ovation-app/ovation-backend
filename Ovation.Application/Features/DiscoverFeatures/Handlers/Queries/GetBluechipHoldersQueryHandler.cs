using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetBluechipHoldersQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetBluechipHoldersQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetBluechipHoldersQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetBlueChipHoldersAsync(request.Page, request.UserPath, request.UserId);
        }
    }
}
