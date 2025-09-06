using MediatR;
using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Queries
{
    public sealed record GetUserStatQueryRequest(Guid UserId) : IRequest<ResponseData>;
}
