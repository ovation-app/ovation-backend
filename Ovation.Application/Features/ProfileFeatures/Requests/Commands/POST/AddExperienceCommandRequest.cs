using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.POST
{
    public sealed record AddExperienceCommandRequest(UserExperienceDto UserExperience, Guid UserId) : IRequest<ResponseData>;
}
