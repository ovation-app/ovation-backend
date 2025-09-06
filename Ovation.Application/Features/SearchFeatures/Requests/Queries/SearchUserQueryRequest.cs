using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.SearchFeatures.Requests.Queries
{
    public sealed record SearchUserQueryRequest(string Query, int Page, Guid UserId) : IRequest<ResponseData>;
}
