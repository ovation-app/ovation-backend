using MediatR;

namespace Ovation.Application.Features.DevTokenFeatures.Requests.Queries
{
    public sealed record TokenFilterQueryRequest(Guid TokenId) : IRequest<bool>;
}
