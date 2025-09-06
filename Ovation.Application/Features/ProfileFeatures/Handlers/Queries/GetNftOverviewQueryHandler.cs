using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetNftOverviewQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetNftOverviewQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetNftOverviewQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserNftOverviewAsync(request.UserId);
        }
    }
}
