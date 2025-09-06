using MediatR;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;

namespace Ovation.Application.Features.AuthFeatures.Requests.Commands
{
    public sealed record RegisterUserCommandRequest(RegisterDto RegisterDto) : IRequest<UserToken>;
}
