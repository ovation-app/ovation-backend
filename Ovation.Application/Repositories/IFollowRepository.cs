using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Repositories
{
    public interface IFollowRepository
    {
        Task<ResponseData> FollowUserAsync(Guid userId, Guid followerId);

        Task<ResponseData> UnfollowUserAsync(Guid userId, Guid followerId);

        Task<ResponseData> GetUserFollowersAsync(Guid userId, int page, Guid authUser);

        Task<ResponseData> GetUserFollowingAsync(Guid userId, int page, Guid authUser);

        Task<ResponseData> GetUserFollowingsAsync(string username, int page, Guid authUser);

        Task<ResponseData> GetUserFollowersAsync(string username, int page, Guid authUser);
    }
}
