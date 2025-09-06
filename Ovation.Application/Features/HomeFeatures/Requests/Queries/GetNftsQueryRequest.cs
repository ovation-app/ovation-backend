using MediatR;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;

namespace Ovation.Application.Features.HomeFeatures.Requests.Queries
{
    public sealed record GetNftsQueryRequest() : IRequest<ResponseData>;

    public sealed record GetNftsFromWalletQueryRequest(WalletAcct WalletAcct) : IRequest<ResponseData>;
}
