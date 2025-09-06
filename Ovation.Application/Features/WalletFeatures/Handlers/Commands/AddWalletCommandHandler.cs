using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.WalletFeatures.Requests.Commands;

namespace Ovation.Application.Features.WalletFeatures.Handlers.Commands
{
    internal class AddWalletCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<AddWalletCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(AddWalletCommandRequest request, CancellationToken cancellationToken)
        {
            return await _walletRepository.AddWalletAsync(request.WalletDto);
        }
    }
}
