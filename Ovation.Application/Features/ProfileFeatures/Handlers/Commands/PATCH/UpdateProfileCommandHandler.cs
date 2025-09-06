using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.PATCH
{
    internal class UpdateProfileCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UpdateProfileCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UpdateProfileCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.UpdatePersonalInfo(request.ProfileDto, request.userId);
        }
    }
}
