using Ovation.Application.DTOs;

namespace Ovation.Application.Repositories
{
    public interface IAffilationRepository
    {
        Task<ResponseData> GetUserAffilationDataAsync(Guid userId);

        Task<ResponseData> GetInvitedUsersAsync(Guid userId);
    }
}
