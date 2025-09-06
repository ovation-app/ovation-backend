using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserSocialsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserSocialsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserSocialsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserSocialsAsync(request.UserId);
        }
    }
}
