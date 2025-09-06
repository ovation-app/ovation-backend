using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.PATCH
{
    internal class UpdateUserPathCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UpdateUserPathCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UpdateUserPathCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.UpdateUserPath(request.UserPath, request.UserId);
        }
    }
}
