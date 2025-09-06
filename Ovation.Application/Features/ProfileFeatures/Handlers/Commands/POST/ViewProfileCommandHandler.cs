using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.POST;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.POST
{
    internal class ViewProfileCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<ViewProfileCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(ViewProfileCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.ViewProfileAsync(request.UserId, request.ViewerId);
        }
    }
}
