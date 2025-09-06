using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.DiscoverFeatures.Requests.Queries
{
    public sealed record GetTopCreatorsQueryRequest(int Page, string UserPath) : IRequest<ResponseData>;
}
