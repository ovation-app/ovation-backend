using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.PathFeatures.Requests.Queries
{
    public sealed record GetPathQueryRequest(Guid Id) : IRequest<ResponseData>;
}
