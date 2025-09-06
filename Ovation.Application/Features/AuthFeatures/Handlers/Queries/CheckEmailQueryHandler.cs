using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Features.AuthFeatures.Requests.Queries;

namespace Ovation.Application.Features.AuthFeatures.Handlers.Queries
{
    internal class CheckEmailQueryHandler : BaseHandler, IRequestHandler<CheckEmailQueryRequest, bool>
    {
        public CheckEmailQueryHandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        public async Task<bool> Handle(CheckEmailQueryRequest request, CancellationToken cancellationToken)
        {
            return await _userRepository.FindEmailAsync(request.Email);
        }
    }
}
