using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.DiscoverFeatures.Requests.Queries
{
    public sealed record GetFeaturedProfileQueryRequest(Guid UserId) : IRequest<ResponseData>;
}
