using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.POST
{
    public sealed record VerifyUserCommandRequest(VerifiyUserDTO VerifiyUserDto, Guid UserId) : IRequest<ResponseData>;
}
