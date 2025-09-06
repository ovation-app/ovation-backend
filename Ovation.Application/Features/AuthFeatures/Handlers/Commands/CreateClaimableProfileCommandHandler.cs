using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AuthFeatures.Requests.Commands;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Commands
{
    internal class CreateClaimableProfileCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<CreateClaimableProfileCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(CreateClaimableProfileCommandRequest request, CancellationToken cancellationToken)
        {
            return await _userRepository.CreateClaimableProfileAsync(request.ClaimableProfile, cancellationToken);
        }
    }
}
