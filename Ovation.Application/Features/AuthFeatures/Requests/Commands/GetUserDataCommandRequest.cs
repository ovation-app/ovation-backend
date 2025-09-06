using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.AuthFeatures.Requests.Commands
{
    public sealed record GetUserDataCommandRequest(string SocialId) : IRequest<UserToken>;
}
