using Ovation.Application.DTOs;

namespace Ovation.Application.Repositories
{
    public interface ISearchRepository
    {
        Task<ResponseData> FindUserAsync(string query, int page, Guid userId);
        Task<ResponseData> FindNftAsync(string query, string? page, Guid userId);
        Task<ResponseData> FindNftCollectionAsync(string query, string? page, Guid userId);
    }
}
