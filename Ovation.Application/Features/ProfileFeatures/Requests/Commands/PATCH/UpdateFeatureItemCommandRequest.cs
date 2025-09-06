using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH
{
    public sealed record UpdateFeatureItemCommandRequest(IdsWithTypeDto FeatureItemDto, Guid UserId) : IRequest<ResponseData>;
}
