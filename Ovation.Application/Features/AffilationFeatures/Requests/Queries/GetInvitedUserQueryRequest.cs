using MediatR;
using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.AffilationFeatures.Requests.Queries
{
    public sealed record GetInvitedUserQueryRequest(Guid UserId) : IRequest<ResponseData>;
}
