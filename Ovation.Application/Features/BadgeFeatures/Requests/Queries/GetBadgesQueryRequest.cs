using MediatR;
using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.BadgeFeatures.Requests.Queries
{
    public sealed record GetBadgesQueryRequest(int Page, Guid UserId) : IRequest<ResponseData>;
}
