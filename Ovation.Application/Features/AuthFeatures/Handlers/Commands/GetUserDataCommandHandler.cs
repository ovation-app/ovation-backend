using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AuthFeatures.Requests.Commands;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Commands
{
    internal class GetUserDataCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserDataCommandRequest, UserToken>
    {
        public async Task<UserToken> Handle(GetUserDataCommandRequest request, CancellationToken cancellationToken)
        {
            var userId = await _userRepository.GetUserAsync(request.SocialId);

            if (userId == Guid.Empty)
                return null;

            var userData = await _authManager.GetUserDataAsync(userId.ToByteArray());

            if (userData == null)
                return new();

            return new UserToken { UserData = userData, Token = _authManager.CreateToken() };
        }
    }
}
