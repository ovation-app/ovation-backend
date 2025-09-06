using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Queries
{
    public sealed record GetUserExperiencesQueryRequest(Guid UserId, int Page) : IRequest<ResponseData>;
}
