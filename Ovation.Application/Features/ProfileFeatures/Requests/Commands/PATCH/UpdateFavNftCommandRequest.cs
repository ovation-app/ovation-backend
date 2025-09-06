using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH
{
    public sealed record UpdateFavNftCommandRequest(List<IntId> IntId, Guid UserId) : IRequest<ResponseData>;
}
