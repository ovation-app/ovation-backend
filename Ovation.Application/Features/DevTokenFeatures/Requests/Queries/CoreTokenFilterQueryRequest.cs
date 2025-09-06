using MediatR;

namespace Ovation.Application.Features.DevTokenFeatures.Requests.Queries
{
    public sealed record CoreTokenFilterQueryRequest(Guid TokenId) : IRequest<bool>;
}
