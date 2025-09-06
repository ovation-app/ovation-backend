using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.AssetFeatures.Requests.Queries
{
    public sealed record GetTokenTransactionQueryRequest(int TokenId) : IRequest<ResponseData>;

    public sealed record GetProfileTokenTransactionQueryRequest(int TokenId) : IRequest<ResponseData>;
}
