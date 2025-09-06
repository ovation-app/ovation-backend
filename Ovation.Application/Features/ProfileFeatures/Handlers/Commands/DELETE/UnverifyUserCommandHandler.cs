using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.DELETE;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.DELETE
{
    internal class UnverifyUserCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<UnverifyUserCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(UnverifyUserCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.UnlinkUserVerification(request.VerifyType, request.UserId);
        }
    }
}
