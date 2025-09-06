using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.WalletFeatures.Requests.Queries;

namespace Ovation.Application.Features.WalletFeatures.Handlers.Queries
{
    internal class GetWalletsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetWalletsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetWalletsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _walletRepository.GetWalletsAsync();
        }
    }
}
