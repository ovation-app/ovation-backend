using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH
{
    public sealed record UpdateSocialsCommandRequest(UserSocialsModDto SocialsModDto, Guid UserId) : IRequest<ResponseData>;
}
