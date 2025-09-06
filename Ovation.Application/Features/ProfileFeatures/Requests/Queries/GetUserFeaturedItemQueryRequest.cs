using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Queries
{
    public sealed record GetUserFeaturedItemQueryRequest(Guid UserId) : IRequest<List<FavNfts>?>;
}
