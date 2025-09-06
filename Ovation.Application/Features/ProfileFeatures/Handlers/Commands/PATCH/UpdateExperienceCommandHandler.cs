using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.PATCH
{
    internal class UpdateExperienceCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UpdateExperienceCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UpdateExperienceCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.UpdateExperienceAsync(request.ExperienceDto, request.Id, request.UserId);
        }
    }
}
