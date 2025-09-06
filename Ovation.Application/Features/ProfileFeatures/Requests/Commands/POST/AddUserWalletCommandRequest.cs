using MediatR;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.POST
{
    public sealed record AddUserWalletCommandRequest(WalletAcct WalletDto, Guid UserId) : IRequest<ResponseData>;
}
