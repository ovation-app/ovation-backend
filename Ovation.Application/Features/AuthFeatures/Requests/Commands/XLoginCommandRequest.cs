using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.AuthFeatures.Requests.Commands
{
    public sealed record XLoginCommandRequest(string XId) : IRequest<UserToken>;
}
