using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.BadgeFeatures.Requests.Queries;

namespace Ovation.Application.Features.BadgeFeatures.Handlers.Queries
{
    internal class GetBlueChipsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetBlueChipsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetBlueChipsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _badgeRepository.GetBlueChipAsync(request.Page);
        }
    }
}
