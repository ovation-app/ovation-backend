using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AuthFeatures.Requests.Commands;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Commands
{
    internal class NormalLoginCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<NormalLoginCommandRequest, UserToken>
    {
        public async Task<UserToken> Handle(NormalLoginCommandRequest request, CancellationToken cancellationToken)
        {
            var user = Guid.Empty;
            try
            {
                var verifyDetails = await _userRepository.LoginAsync(request.LoginDto);

                if (!verifyDetails.Status)
                    return new UserToken { Message = verifyDetails.Message };
                else if (verifyDetails.GuidValue == Guid.Empty)
                    return new UserToken { Message = "An error occurred" };
                var userId = verifyDetails.GuidValue;

                var userData = await _authManager.GetUserDataAsync(userId.ToByteArray());
                if (userData == null)
                    return new UserToken { Message = "An error occurred" };

                user = userId;
                return new UserToken { Message = "Success", UserData = userData, Token = _authManager.CreateToken() };
            }
            catch (Exception _)
            {
                return new UserToken { Message = "An error occurred" };
            }
            finally
            {
                if (user != Guid.Empty)
                {
                    await _profileRepository.CheckAlphaLaunchAndTesterBadge(user);

                    //await _profileService.CheckForUpdatedBadge(user);
                }

            }
        }
    }
}
