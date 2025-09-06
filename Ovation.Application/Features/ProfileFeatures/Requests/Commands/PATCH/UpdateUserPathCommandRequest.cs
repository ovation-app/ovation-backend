using MediatR;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH
{
    public sealed record UpdateUserPathCommandRequest(UserPathDto UserPath, Guid UserId) : IRequest<ResponseData>;
}
