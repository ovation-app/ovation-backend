using MediatR;

namespace Ovation.Application.Features.AuthFeatures.Requests.Queries
{
    public sealed record AuthUserDataQueryRequest(Guid UserId) : IRequest<object>;
}
