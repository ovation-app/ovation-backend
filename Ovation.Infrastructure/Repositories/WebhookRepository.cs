using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;

namespace Ovation.Persistence.Repositories
{
    internal class WebhookRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<UserNftActivity>(serviceScopeFactory), IWebhookRepository
    {
        public async Task AddNftActivity(object data)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var userNftAct = new UserNftActivity
                {
                    EventDetails = JsonConvert.SerializeObject(data)
                };

                await _context.UserNftActivities.AddAsync(userNftAct);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception _)
            {
                await _unitOfWork.RollbackAsync();
            }
        }
    }
}
