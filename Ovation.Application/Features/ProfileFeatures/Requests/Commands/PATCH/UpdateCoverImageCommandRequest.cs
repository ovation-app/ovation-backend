using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH
{
    public sealed record UpdateCoverImageCommandRequest(string ImageUrl, Guid UserId) : IRequest<ResponseData>;
}
