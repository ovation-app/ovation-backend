using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AuthFeatures.Requests.Queries;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Queries
{
    internal class VerifyAcctQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<VerifyAcctQueryRequest, UserToken>
    {
        public async Task<UserToken> Handle(VerifyAcctQueryRequest request, CancellationToken cancellationToken)
        {
            var res = await _userRepository.VerifyUserAsync(request.Code, request.Otp);

            if (res.StatusCode == 404)
                return new UserToken { Message = res.Message };

            var userData = await _authManager.GetUserDataAsync(res.GuidValue.ToByteArray());
            if (userData == null)
                return new UserToken { Message = "An error occurred" };

            return new UserToken { UserData = userData, Token = _authManager.CreateToken() };
        }
    }
}
