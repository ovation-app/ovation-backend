using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.DELETE
{
    public sealed record UnverifyUserCommandRequest(string VerifyType, Guid UserId) : IRequest<ResponseData>;
}
