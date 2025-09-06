using MediatR;
using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Commands.PATCH
{
    public sealed record UpdateExperienceCommandRequest(UserExperienceModDto ExperienceDto, Guid Id, Guid UserId) : IRequest<ResponseData>;
}
