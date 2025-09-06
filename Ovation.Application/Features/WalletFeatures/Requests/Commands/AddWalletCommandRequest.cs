using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.WalletFeatures.Requests.Commands
{
    public sealed record AddWalletCommandRequest(WalletDto WalletDto) : IRequest<ResponseData>;
}
