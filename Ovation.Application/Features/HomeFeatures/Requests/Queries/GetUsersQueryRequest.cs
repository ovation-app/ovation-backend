using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.HomeFeatures.Requests.Queries
{
    public sealed record GetUsersQueryRequest() : IRequest<ResponseData>;
}
