using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;

namespace Ovation.Persistence.Repositories
{
    internal class FollowRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<UserFollower>(serviceScopeFactory), IFollowRepository
    {
        public async Task<ResponseData> FollowUserAsync(Guid userId, Guid followerId)
        {
            if (followerId == Guid.Empty) return new ResponseData { Message = "Invalid userId" };

            if (await IsUserFollowedAsync(userId, followerId)) return new ResponseData { Message = "User already followed" };

            var userFollow = new UserFollower
            {
                FollowId = Guid.NewGuid().ToByteArray(),
                FollowerId = followerId.ToByteArray(),
                UserId = userId.ToByteArray()
            };
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _context.UserFollowers.AddAsync(userFollow);

                if (!await UpdateUsersStatAsync(userId, followerId)) return new();

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, Message = "Follow Successful!" };

            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();

                return new();
            }
            finally
            {
                var follower = await _context.Users
                    .Include(_ => _.UserProfile)
                    .Where(_ => _.UserId == followerId.ToByteArray())
                    .Select(x => new
                    {
                        x.Username,
                        x.UserProfile.DisplayName
                    }).FirstOrDefaultAsync();

                var notification = new List<NotificationDto>
                {
                    new NotificationDto
                    {
                        InitiatorId = followerId,
                        ReceiverId = userId,
                        Message = $"@{follower.Username}",
                        Title = $"{follower.DisplayName} follows you",
                        Reference = NotificationReference.Follow.ToString(),
                        ReferenceId = followerId.ToString()
                    }
                };

                await SaveNotificationAsync(notification);
            }
        }

        public async Task<ResponseData> GetUserFollowersAsync(Guid userId, int page, Guid authUser)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await _context.UserFollowers
                    .Where(x => x.UserId == userId.ToByteArray())
                    .Include(x => x.User)
                    .Include(x => x.Follower)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.Follower.UserId),
                        x.Follower.Username,
                        x.Follower.UserProfile.DisplayName,
                        x.Follower.UserProfile.ProfileImage,
                        x.Follower.UserProfile.Bio,
                        x.Follower.UserStat.Followers,
                        x.Follower.UserStat.Following,
                        IsFollowing = x.Follower.UserFollowerUsers.Where(_ => _.FollowerId == authUser.ToByteArray()).FirstOrDefault() != null ? true : false,
                        isVerified = x.Follower.VerifiedUsers != null ? true : false,
                    }).ToListAsync();

                response.Status = true;
                response.Message = "Data fetched!";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserFollowersAsync(string username, int page, Guid authUser)
        {
            var userId = await GetUserIdAsync(username);

            if (userId == Guid.Empty) return new ResponseData { Message = "User not found" };

            return await GetUserFollowersAsync(userId, page, authUser);
        }

        public async Task<ResponseData> GetUserFollowingAsync(Guid userId, int page, Guid authUser)
        {
            try
            {
                var response = new ResponseData();

                response.Data = await _context.UserFollowers
                    .Where(x => x.FollowerId == userId.ToByteArray())
                    .Include(x => x.User)
                    .Include(x => x.Follower)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        UserId = new Guid(x.User.UserId),
                        x.User.Username,
                        x.User.UserProfile.DisplayName,
                        x.User.UserProfile.ProfileImage,
                        x.User.UserProfile.Bio,
                        x.User.UserStat.Followers,
                        x.User.UserStat.Following,
                        IsFollowing = x.User.UserFollowerUsers.Where(_ => _.FollowerId == authUser.ToByteArray()).FirstOrDefault() != null ? true : false,
                        isVerified = x.User.VerifiedUsers != null ? true : false,
                    }).ToListAsync();

                response.Status = true;
                response.Message = "Data fetched!";

                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetUserFollowingsAsync(string username, int page, Guid authUser)
        {
            var userId = await GetUserIdAsync(username);

            if (userId == Guid.Empty) return new ResponseData { Message = "User not found" };

            return await GetUserFollowingAsync(userId, page, authUser);
        }

        public async Task<ResponseData> UnfollowUserAsync(Guid userId, Guid followerId)
        {
            if (followerId == Guid.Empty) return new ResponseData { Message = "Invalid userId" };

            var entity = await _context.UserFollowers
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.FollowerId == followerId.ToByteArray());

            if (entity == null) return new ResponseData { Message = "Users don't follow each other" };

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _context.UserFollowers.Remove(entity);
                if (!await UpdateUsersStatAsync(userId, followerId, -1)) return new();

                await RemoveFollowNotificationAsync(userId, followerId);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { Status = true, Message = "Unfollow Successful!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        internal async Task<Guid> GetUserIdAsync(string username)
        {
            var user = await _context.Users
                .Where(_ => _.Username == username)
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Select(_ => _.UserId)
                .FirstOrDefaultAsync();

            if (user == null) return Guid.Empty;

            return new Guid(user);
        }


        private async Task<bool> IsUserFollowedAsync(Guid userId, Guid followerId)
        {
            var entity = await _context.UserFollowers
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.FollowerId == followerId.ToByteArray());

            if (entity == null) return false;

            return true;
        }

        private async Task<bool> UpdateUsersStatAsync(Guid userId, Guid followerId, int value = 1)
        {
            var follower = await _context.UserStats
                .FirstOrDefaultAsync(_ => _.UserId == followerId.ToByteArray());

            var user = await _context.UserStats
                .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray());

            try
            {
                if (follower == null && user == null) return false;

                follower.Following += value;
                user.Followers += value;

                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException _)
            {
                SentrySdk.CaptureException(_);
                return false;
            }
        }

        private async Task RemoveFollowNotificationAsync(Guid userId, Guid followerId)
        {
            try
            {
                var entity = await _context.UserNotifications
                    .FirstOrDefaultAsync(_ => _.InitiatorId == followerId.ToByteArray()
                    && _.ReceiverId == userId.ToByteArray()
                    && _.Reference == NotificationReference.Follow.ToString());

                if (entity != null)
                {
                    _context.UserNotifications.Remove(entity);

                    if (Constant._offlineNotification.TryGetValue(userId, out List<NotificationDto>? notifications))
                    {
                        int index = 0;
                        foreach (var message in Constant._offlineNotification[userId])
                        {
                            if (message.InitiatorId == followerId && message.ReceiverId == userId && message.Reference == NotificationReference.Follow.ToString())
                            {
                                notifications.RemoveAt(index);
                                break;
                            }
                            index += 1;
                        }
                    }
                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }

        }
    }
}
