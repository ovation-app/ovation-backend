using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserStatQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserStatQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserStatQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserStatsAsync(request.UserId);
        }
    }
}
