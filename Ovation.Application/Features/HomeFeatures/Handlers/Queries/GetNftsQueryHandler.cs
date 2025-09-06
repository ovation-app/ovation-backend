using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.HomeFeatures.Requests.Queries;

namespace Ovation.Application.Features.HomeFeatures.Handlers.Queries
{
    class GetNftsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetNftsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetNftsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _homeRepository.GetNfts();
        }
    }

    class GetNftsFromWalletQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetNftsFromWalletQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetNftsFromWalletQueryRequest request, CancellationToken cancellationToken)
        {
            return await _homeRepository.GetNftsFromWallet(request.WalletAcct);
        }
    }
}
