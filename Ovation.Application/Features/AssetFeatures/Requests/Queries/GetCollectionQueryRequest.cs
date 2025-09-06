using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.AssetFeatures.Requests.Queries
{
    public sealed record GetCollectionQueryRequest(int CollectionId) : IRequest<ResponseData>;
}
