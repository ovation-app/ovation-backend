using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.DELETE
{
    public sealed record DeleteWalletCommandRequest(Guid WalletId, Guid UserId) : IRequest<ResponseData>;

    public sealed record DeleteWalletGroupCommandRequest(Guid GroupId, Guid UserId) : IRequest<ResponseData>;
}
