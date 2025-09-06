using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.AuthFeatures.Requests.Queries
{
    public sealed record VerifyAcctQueryRequest(Ulid Code, int Otp) : IRequest<UserToken>;
}
