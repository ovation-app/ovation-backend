using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.AssetFeatures.Requests.Queries
{
    public sealed record GetTokenQueryRequest(int TokenId) : IRequest<ResponseData>;
    public sealed record GetProfileTokenQueryRequest(int AssetId, Guid UserId) : IRequest<ResponseData>;
}
