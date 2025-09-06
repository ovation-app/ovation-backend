using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH
{
    public sealed record UpdateNftPrivacyCommandRequest(List<NftVisibleDto> NftVisibleDto, Guid UserId) : IRequest<ResponseData>;
}
