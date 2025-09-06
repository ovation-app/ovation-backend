using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.SearchFeatures.Requests.Queries
{
    public sealed record SearchNftQueryRequest(string Query, string? Cursor, Guid UserId) : IRequest<ResponseData>;
}
