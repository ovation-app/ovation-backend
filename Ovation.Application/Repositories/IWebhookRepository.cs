namespace Ovation.Application.Repositories
{
    public interface IWebhookRepository
    {
        Task AddNftActivity(object data);
    }
}
