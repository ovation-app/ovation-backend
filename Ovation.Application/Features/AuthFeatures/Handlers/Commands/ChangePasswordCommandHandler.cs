using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AuthFeatures.Requests.Commands;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Commands
{
    internal class ChangePasswordCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<ChangePasswordCommandRequest, ResponseData>
    {
        public Task<ResponseData> Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            return _userRepository.ChangePasswordAsync(request.ChangePassword);
        }
    }
}
