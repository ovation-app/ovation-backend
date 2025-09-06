using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.WalletFeatures.Requests.Queries

{
    public sealed record GetWalletsQueryRequest : IRequest<ResponseData>;
}
