using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.PathFeatures.Requests.Queries;

namespace Ovation.Application.Features.PathFeatures.Handlers.Queries
{
    internal class GetPathQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetPathQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetPathQueryRequest request, CancellationToken cancellationToken)
        {
            return await _pathRepository.GetPathAsync(request.Id);
        }
    }
}
