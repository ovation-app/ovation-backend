using MediatR;
using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.ProfileFeatures.Requests.Queries
{
    public sealed record GetUserQueryRequest(string Username, Guid UserId) : IRequest<ResponseData>;
}
