using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH
{
    public sealed record UpdateProfileCommandRequest(ProfileModDto ProfileDto, Guid userId) : IRequest<ResponseData>;
}
