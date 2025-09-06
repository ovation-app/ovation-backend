using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.POST;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.POST
{
    internal class AddUserWalletCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<AddUserWalletCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(AddUserWalletCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.AddWalletAsync(request.WalletDto, request.UserId);
        }
    }
}
