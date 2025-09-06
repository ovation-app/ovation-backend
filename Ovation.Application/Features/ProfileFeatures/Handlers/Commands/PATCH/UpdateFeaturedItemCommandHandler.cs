using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.PATCH
{
    internal class UpdateFeaturedItemCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UpdateFeatureItemCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UpdateFeatureItemCommandRequest request, CancellationToken cancellationToken)
        {
            return new(); // await _profileRepository.UpdateFeaturedItemsAsync(request.FeatureItemDto, request.UserId);
        }
    }
}
