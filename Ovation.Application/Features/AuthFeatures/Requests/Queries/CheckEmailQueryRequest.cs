using MediatR;

namespace Ovation.Application.Features.AuthFeatures.Requests.Queries
{
    public sealed record class CheckEmailQueryRequest(string Email) : IRequest<bool>;
}
