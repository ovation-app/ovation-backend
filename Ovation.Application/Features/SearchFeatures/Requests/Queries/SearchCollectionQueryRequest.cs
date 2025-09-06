using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.SearchFeatures.Requests.Queries
{
    public sealed record SearchCollectionQueryRequest(string Query, string? Cursor, Guid UserId) : IRequest<ResponseData>;
}
