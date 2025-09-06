using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.AssetFeatures.Requests.Queries
{
    public sealed record GetCollectionTokensQueryRequest(int CollectionId, TokenQueryParametersDto Parameters) : IRequest<ResponseData>;
}
