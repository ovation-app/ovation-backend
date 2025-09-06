using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;

namespace Ovation.Persistence.Repositories
{
    internal class AffilationRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<UserAffilation>(serviceScopeFactory), IAffilationRepository
    {
        public async Task<ResponseData> GetInvitedUsersAsync(Guid userId)
        {
            var response = new ResponseData();
            try
            {
                response.Data = await _context.UserReferrals
                    .Where(_ => _.InviterId == userId.ToByteArray())
                    .Select(x => new
                    {
                        UserId = new Guid(x.Invitee.UserId),
                        x.Invitee.Username,
                        x.Invitee.UserProfile.CoverImage,
                        x.Invitee.UserProfile.ProfileImage,
                        x.Invitee.UserProfile.DisplayName,
                        x.Invitee.UserProfile.Bio,
                        Joined = x.Invitee.CreatedDate,
                        x.Invitee.UserStat.Following,
                        x.Invitee.UserStat.Followers
                    })
                    .ToListAsync();

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

        public async Task<ResponseData> GetUserAffilationDataAsync(Guid userId)
        {
            var response = new ResponseData();
            try
            {
                var data = await _context.UserAffilations
                    .Where(_ => _.UserId == userId.ToByteArray())
                    .Select(x => new
                    {
                        x.Code,
                        x.Invited,
                        EarnedBadges = x.User.UserBadges.Where(_ => EF.Functions.Like(_.BadgeName, $"%UserInvites%") && _.Active == 1).Select(b => new
                        {
                            b.BadgeNameNavigation.BadgeName,
                            b.BadgeNameNavigation.ImageUrl
                        }).FirstOrDefault(),
                        Badges = _context.Badges.Where(_ => EF.Functions.Like(_.BadgeName, $"%UserInvites%") && _.Active == 1).Select(b => new
                        {
                            b.BadgeName,
                            b.ImageUrl,
                            IsEarned = b.UserBadges.Where(_ => _.UserId == userId.ToByteArray()).FirstOrDefault() != null ? true : false,
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (data != null)
                {
                    if (data.EarnedBadges == null)
                        response.Data = new { data.Code, data.Invited, data.Badges, EarnedBadges = new { BadgeName = "--", ImageUrl = "" } };
                    else response.Data = data;
                }


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

    }
}
