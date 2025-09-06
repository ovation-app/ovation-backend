using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.PATCH
{
    internal class UpdateCoverImageCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UpdateCoverImageCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UpdateCoverImageCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.UpdateCoverImageAsync(request.ImageUrl, request.UserId);
        }
    }
}
