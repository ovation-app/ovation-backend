using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.PathFeatures.Requests.Queries;

namespace Ovation.Application.Features.PathFeatures.Handlers.Queries
{
    internal class GetPathsQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetPathsQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetPathsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _pathRepository.GetPathsAsync();
        }
    }
}
