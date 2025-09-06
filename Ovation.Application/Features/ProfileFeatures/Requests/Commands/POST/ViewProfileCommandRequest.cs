using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.POST
{
    public sealed record ViewProfileCommandRequest(Guid UserId, Guid ViewerId) : IRequest<ResponseData>;
}
