using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Features.AuthFeatures.Requests.Queries;
using Ovation.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Queries
{
    internal class CheckUsernameQueryHandler : BaseHandler, IRequestHandler<CheckUsernameQueryRequest, bool>
    {
        public CheckUsernameQueryHandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        public async Task<bool> Handle(CheckUsernameQueryRequest request, CancellationToken cancellationToken)
        {
            return await _userRepository.FindUsernameAsync(request.Username);
        }
    }
}
