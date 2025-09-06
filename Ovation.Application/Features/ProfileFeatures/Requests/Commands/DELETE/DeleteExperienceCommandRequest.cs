using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.DELETE
{
    public sealed record DeleteExperienceCommandRequest(Guid ExperienceId, Guid UserId) : IRequest<ResponseData>;
}
