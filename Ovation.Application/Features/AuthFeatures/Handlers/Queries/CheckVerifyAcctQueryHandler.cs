using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Features.AuthFeatures.Requests.Queries;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Queries
{
    internal class CheckVerifyAcctQueryHandler : BaseHandler, IRequestHandler<CheckVerifyAcctQueryRequest, bool>
    {
        public CheckVerifyAcctQueryHandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        public async Task<bool> Handle(CheckVerifyAcctQueryRequest request, CancellationToken cancellationToken)
        {
            var res = await _userRepository.IsAccountVerified(request.UserId);

            return res.Status;
        }
    }
}
