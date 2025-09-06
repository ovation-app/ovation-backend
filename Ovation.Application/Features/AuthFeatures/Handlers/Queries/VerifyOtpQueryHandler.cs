using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AuthFeatures.Requests.Queries;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Queries
{
    internal class VerifyOtpQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<VerifyOtpQueryRequest, ResponseData>
    {
        public Task<ResponseData> Handle(VerifyOtpQueryRequest request, CancellationToken cancellationToken)
        {
            if (request.Otp == 0 || request.Otp.ToString().Length < 6)
                return Task.FromResult(new ResponseData { Message = "Invalid OTP" });

            if (Constant._userOTP.TryGetValue(request.UserId, out int storedOtp))
            {
                if (storedOtp == request.Otp)
                {
                    Constant._userOTP.TryRemove(request.UserId, out _);
                    return Task.FromResult(new ResponseData { Message = "OTP Verified!", Status = true });
                }
                return Task.FromResult(new ResponseData { Message = "Invalid OTP" });
            }

            return Task.FromResult(new ResponseData { Message = "OTP not found, try requesting a new OTP" });
        }
    }
}
