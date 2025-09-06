using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.WalletFeatures.Requests.Queries
{
    public sealed record GetWalletQueryRequest(Guid Id) : IRequest<ResponseData>;
}
