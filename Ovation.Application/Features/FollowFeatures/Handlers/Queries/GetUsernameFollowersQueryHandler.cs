using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.FollowFeatures.Requests.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.FollowFeatures.Handlers.Queries
{
    internal class GetUsernameFollowersQueryHandler : BaseHandler, IRequestHandler<GetUsernameFollowersQueryRequest, ResponseData>
    {
        public GetUsernameFollowersQueryHandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        public async Task<ResponseData> Handle(GetUsernameFollowersQueryRequest request, CancellationToken cancellationToken)
        {
            return await _followRepository.GetUserFollowersAsync(request.Username, request.Page, request.AuthUser);
        }
    }
}
