using MediatR;

namespace Ovation.Application.Features.AuthFeatures.Requests.Queries
{
    public sealed record CheckVerifyAcctQueryRequest(Guid UserId) : IRequest<bool>;
}
