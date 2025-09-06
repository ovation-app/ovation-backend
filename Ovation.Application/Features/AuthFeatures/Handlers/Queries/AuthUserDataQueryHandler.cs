using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Features.AuthFeatures.Requests.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Queries
{
    internal class AuthUserDataQueryHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<AuthUserDataQueryRequest, object>
    {
        public Task<object?> Handle(AuthUserDataQueryRequest request, CancellationToken cancellationToken)
        {
            return _authManager.GetUserDataAsync(request.UserId.ToByteArray());
        }
    }
}
