using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.PathFeatures.Requests.Commands;

namespace Ovation.Application.Features.PathFeatures.Handlers.Commands
{
    internal class AddPathCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<AddPathCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(AddPathCommandRequest request, CancellationToken cancellationToken)
        {
            return await _pathRepository.AddPathAsync(request.PathDto);
        }
    }
}
