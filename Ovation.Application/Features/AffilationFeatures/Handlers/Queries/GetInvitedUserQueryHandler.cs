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
    internal class GetInvitedUserQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<GetInvitedUserQueryRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(GetInvitedUserQueryRequest request, CancellationToken cancellationToken)
        {
            return await _affilationRepository.GetInvitedUsersAsync(request.UserId);
        }
    }
}
