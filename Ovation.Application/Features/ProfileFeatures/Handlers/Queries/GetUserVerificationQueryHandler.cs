using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserVerificationQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserVerificationQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserVerificationQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserVerificationDataAsync(request.UserId);
        }
    }
}
