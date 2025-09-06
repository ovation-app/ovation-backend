using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AuthFeatures.Requests.Commands;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Commands
{
    internal class RegisterUserCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<RegisterUserCommandRequest, UserToken>
    {
        public async Task<UserToken> Handle(RegisterUserCommandRequest request, CancellationToken cancellationToken)
        {
            var setupUser = await _userRepository.AddUserAsync(request.RegisterDto);

            if (!setupUser.Status)
                return new UserToken { Message = setupUser.Message };
            else if (setupUser.GuidValue == Guid.Empty)
                return new UserToken { Message = "An error occurred" };

            var userId = setupUser.GuidValue;

            if (request.RegisterDto.Type != DTOs.SignUp.Type.Normal.ToString())
            {
                var userData = await _authManager.GetUserDataAsync(userId.ToByteArray());
                if (userData == null)
                    return new UserToken { Message = "An error occurred" };

                return new UserToken { Message = "Account created", UserData = userData, Token = _authManager.CreateToken() };
            }

            await _userRepository.SendVerificationLinkAsync(userId, request.RegisterDto.PersonalInfo.Email.Trim());

            return new UserToken { Message = "Account created. Verification link sent to users email for verification", UserId = userId };
        }
    }
}
