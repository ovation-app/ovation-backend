using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.DELETE;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.DELETE
{
    internal class DeleteExperienceCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<DeleteExperienceCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(DeleteExperienceCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.DeleteExperienceAsync(request.ExperienceId, request.UserId);
        }
    }
}
