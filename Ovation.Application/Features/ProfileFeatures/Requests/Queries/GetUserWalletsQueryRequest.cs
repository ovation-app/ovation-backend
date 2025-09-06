using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Queries
{
    public sealed record GetUserWalletsQueryRequest(Guid UserId) : IRequest<ResponseData>;
    public sealed record GetWalletsQueryRequest(Guid UserId) : IRequest<ResponseData>;
}
