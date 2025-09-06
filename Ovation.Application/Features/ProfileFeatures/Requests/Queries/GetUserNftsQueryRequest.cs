using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Queries
{
    public sealed record GetUserNftsQueryRequest(Guid UserId, Guid AuthUser, NftQueryParametersDto Parameters) : IRequest<ResponseData>;

    public sealed record GetUserCollectionQueryRequest(Guid UserId, Guid AuthUser, NftQueryParametersDto Parameters) : IRequest<ResponseData>;

    public sealed record GetUserTopNftsQueryRequest(int Limit, Guid UserId) : IRequest<ResponseData>;
}
