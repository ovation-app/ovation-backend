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
    internal class GetUsernameFollowingsQueryHandler : BaseHandler, IRequestHandler<GetUsernameFollowingsQueryRequest, ResponseData>
    {
        public GetUsernameFollowingsQueryHandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        public async Task<ResponseData> Handle(GetUsernameFollowingsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _followRepository.GetUserFollowingsAsync(request.Username, request.Page, request.AuthUser);
        }
    }
}
