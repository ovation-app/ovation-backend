using MediatR;
using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Features.NotificationFeatures.Requests.Commands
{
    public sealed record ReadNotificationCommandRequest(Guid Id, Guid UserId) : IRequest<ResponseData>;
}
