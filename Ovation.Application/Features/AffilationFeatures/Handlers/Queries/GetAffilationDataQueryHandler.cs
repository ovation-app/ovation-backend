using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AffilationFeatures.Requests.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.AffilationFeatures.Handlers.Queries
{
    internal class GetAffilationDataQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetAffilationDataQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetAffilationDataQueryRequest request, CancellationToken cancellationToken)
        {
            return await _affilationRepository.GetUserAffilationDataAsync(request.UserId);
        }
    }
}
