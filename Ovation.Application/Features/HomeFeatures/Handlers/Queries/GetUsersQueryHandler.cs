using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.HomeFeatures.Requests.Queries;

namespace Ovation.Application.Features.HomeFeatures.Handlers.Queries
{
    class GetUsersQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetUsersQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetUsersQueryRequest request, CancellationToken cancellationToken)
        {
            return await _homeRepository.GetUsers();
        }
    }
}
