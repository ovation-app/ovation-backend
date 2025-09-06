using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Features.ProfileFeatures.Requests.Commands.POST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.ProfileFeatures.Handlers.POST
{
    internal class AddExperienceCommandHandler(IServiceScopeFactory serviceScopeFactory) : BaseHandler(serviceScopeFactory), IRequestHandler<AddExperienceCommandRequest, ResponseData>
    {
        public async Task<ResponseData> Handle(AddExperienceCommandRequest request, CancellationToken cancellationToken)
        {
            return await _profileRepository.AddUserExperienceAsync(request.UserExperience, request.UserId);
        }
    }
}
