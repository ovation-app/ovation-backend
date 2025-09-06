using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.PATCH
{
    internal class ChangePasswordCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<ChangePasswordCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            return await _userRepository.ChangePasswordAsync(request.Password, request.UserId);
        }
    }
}
