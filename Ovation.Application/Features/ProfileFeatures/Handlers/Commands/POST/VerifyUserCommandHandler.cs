using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.POST;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.POST
{
    internal class VerifyUserCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<VerifyUserCommandRequest, ResponseData>
    {
        public Task<ResponseData> Handle(VerifyUserCommandRequest request, CancellationToken cancellationToken)
        {
            return _profileRepository.VerifyUserAsync(request.VerifiyUserDto, request.UserId);
        }
    }
}
