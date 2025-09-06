using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.PATCH
{
    internal class UpdateNftPrivacyCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UpdateNftPrivacyCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UpdateNftPrivacyCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.UpdateNftVisibilityAsync(request.NftVisibleDto, request.UserId);
        }
    }
}
