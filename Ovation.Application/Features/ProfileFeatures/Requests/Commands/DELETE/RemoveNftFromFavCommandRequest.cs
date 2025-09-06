using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.DELETE
{
    public sealed record RemoveNftFromFavCommandRequest(List<IntId> NftId, Guid UserId) : IRequest<ResponseData>;
}
