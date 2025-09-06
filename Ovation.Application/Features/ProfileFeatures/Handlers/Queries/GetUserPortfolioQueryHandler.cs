using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    class GetUserPortfolioQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserPortfolioQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserPortfolioQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserPortfolioRecordAsync(request.UserId);
        }
    }
}
