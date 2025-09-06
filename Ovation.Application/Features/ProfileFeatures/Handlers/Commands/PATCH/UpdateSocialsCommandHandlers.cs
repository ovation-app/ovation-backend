using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.PATCH
{
    internal class UpdateSocialsCommandHandlers(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UpdateSocialsCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UpdateSocialsCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.UpdateSocialsAsync(request.SocialsModDto, request.UserId);
        }
    }
}
