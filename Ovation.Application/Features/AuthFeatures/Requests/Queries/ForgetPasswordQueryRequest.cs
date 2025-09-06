using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.AuthFeatures.Requests.Queries
{
    public sealed record ForgetPasswordQueryRequest(string Email) : IRequest<ResponseData>;
}
