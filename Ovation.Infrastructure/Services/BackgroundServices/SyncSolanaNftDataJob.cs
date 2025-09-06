using Microsoft.AspNetCore.SignalR;
using Ovation.Application.Common.Interfaces;
using Ovation.Persistence.Data;
using Quartz;

namespace Ovation.Persistence.Services.BackgroundServices
{
    public class SyncSolanaNftDataJob(IHttpClientFactory factory, OvationDbContext _context, IHubContext<NotificationService, INotificationService> hubContext) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var chain = string.Empty;
        }
    }
}
