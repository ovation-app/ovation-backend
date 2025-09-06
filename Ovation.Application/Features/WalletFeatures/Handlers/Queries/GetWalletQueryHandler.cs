using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.WalletFeatures.Requests.Queries;

namespace Ovation.Application.Features.WalletFeatures.Handlers.Queries
{
    internal class GetWalletQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetWalletQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetWalletQueryRequest request, CancellationToken cancellationToken)
        {
            return await _walletRepository.GetWalletAsync(request.Id);
        }
    }
}
