using MediatR;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp.Claimable;

namespace Ovation.Application.Features.AuthFeatures.Requests.Commands
{
    public sealed record CreateClaimableProfileCommandRequest(ClaimableProfile ClaimableProfile) : IRequest<ResponseData>;
}

