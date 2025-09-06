using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.PATCH
{
    internal class UpdateFavNftCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UpdateFavNftCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UpdateFavNftCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.UpdateFavoriteNftAsync(request.IntId, request.UserId);
        }
    }
}
