using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.AuthFeatures.Requests.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Queries
{
    internal class ForgetPasswordQueryHandler : BaseHandler, IRequestHandler<ForgetPasswordQueryRequest, ResponseData>
    {
        public ForgetPasswordQueryHandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        public Task<ResponseData> Handle(ForgetPasswordQueryRequest request, CancellationToken cancellationToken)
        {
            return _userRepository.ForgetPasswordAsync(request.Email);
        }
    }
}
