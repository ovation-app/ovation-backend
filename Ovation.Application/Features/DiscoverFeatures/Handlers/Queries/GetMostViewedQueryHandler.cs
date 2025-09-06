using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.DiscoverFeatures.Requests.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.DiscoverFeatures.Handlers.Queries
{
    internal class GetMostViewedQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetMostViewedQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetMostViewedQueryRequest request, CancellationToken cancellationToken)
        {
            return await _discoverRepository.GetMostViewedAsync(request.Page, request.UserId);
        }
    }
}
