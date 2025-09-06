using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH
{
    public sealed record ChangePasswordCommandRequest(PasswordModDto Password, Guid UserId) : IRequest<ResponseData>;
}
