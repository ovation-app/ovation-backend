using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetAuthUserQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetAuthUserQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetAuthUserQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetAuthUserAsync(request.UserId);
        }
    }
}
