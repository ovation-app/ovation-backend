using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserFavNftQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserFavNftQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserFavNftQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetFavNftsAsync(request.UserId);
        }
    }
}
