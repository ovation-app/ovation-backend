using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.PathFeatures.Requests.Queries
{
    public sealed record GetPathsQueryRequest() : IRequest<ResponseData>;
}
