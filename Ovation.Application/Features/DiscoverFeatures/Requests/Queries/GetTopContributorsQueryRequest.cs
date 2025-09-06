using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.DiscoverFeatures.Requests.Queries
{
    public sealed record GetTopContributorsQueryRequest(int Page) : IRequest<ResponseData>;
}
