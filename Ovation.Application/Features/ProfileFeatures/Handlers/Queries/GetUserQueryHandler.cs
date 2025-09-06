using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Queries;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.Queries
{
    internal class GetUserQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUserQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUserQueryRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.GetUserAsync(request.Username, request.UserId);
        }
    }
}
