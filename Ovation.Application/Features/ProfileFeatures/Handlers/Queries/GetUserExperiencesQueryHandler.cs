using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserExperiencesQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserExperiencesQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserExperiencesQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserExperienceAsync(request.UserId, request.Page);
        }
    }
}
