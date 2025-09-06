using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.DELETE;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.DELETE
{
    internal class RemoveNftFromFavCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<RemoveNftFromFavCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(RemoveNftFromFavCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.RemoveFavoriteNftAsync(request.NftId, request.UserId);
        }
    }
}
