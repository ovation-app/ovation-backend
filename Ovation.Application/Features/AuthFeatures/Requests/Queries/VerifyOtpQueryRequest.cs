using MediatR;
using Ovation.Application.DTOs;

namespace Ovation.Application.Features.AuthFeatures.Requests.Queries
{
    public sealed record VerifyOtpQueryRequest(Guid UserId, int Otp) : IRequest<ResponseData>;
}
