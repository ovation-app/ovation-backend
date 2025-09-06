using Ovation.Application.DTOs;

namespace Ovation.Application.Repositories
{
    public interface INewsletterRepository
    {
        Task<ResponseData> PostSubscriberAsync(NewsletterDto newsletter);

        Task<int> getTotalSubscribers();
    }
}
