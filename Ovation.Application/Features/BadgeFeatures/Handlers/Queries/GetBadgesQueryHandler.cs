using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.BadgeFeatures.Requests.Queries;

namespace Ovation.Application.Features.BadgeFeatures.Handlers.Queries
{
    internal class GetBadgesQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetBadgesQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetBadgesQueryRequest request, CancellationToken cancellationToken)
        {
            return await _badgeRepository.GetBadgesAsync(request.Page, request.UserId);
        }
    }
}
