using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Queries
{
    public sealed record GetUserTransactionsQueryRequest(Guid UserId, string? Cursor) : IRequest<ResponseData>;
}
