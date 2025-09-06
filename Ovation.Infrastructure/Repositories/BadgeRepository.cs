using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums.Badges;
using Ovation.Application.DTOs.Enums.Badges.ProfileComplete.Milestones;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;

namespace Ovation.Persistence.Repositories
{
    internal class BadgeRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<Badge>(serviceScopeFactory), IBadgeRepository
    {
        public async Task<ResponseData> GetBadgesAsync(int page, Guid userId)
        {
            try
            {
                return new ResponseData
                {
                    Status = true,
                    Message = "Badges Fetched!",
                    Data = new
                    {
                        ProfileComplete = new { Description = "Awarded for fully setting up your profile on the platform.", Badges = await GetProfileCompleteBadge(userId) },

                        UserInvites = new { Description = "Earned by inviting friends and expanding the platform's community.", Badges = await GetUserInviteBadges(page, userId) },

                        BlueChip = new { Description = "Awarded to users holding well-established, high-quality NFTs known as \"blue chips\".", Badges = await GetBlueChipBadges(page, userId) },

                        HighestValuedNFT = new { Description = "Given to users holding a high-valued NFT in their portfolio.", Badges = await GetTopValueNftBadges(page, userId) },

                        NumberOfNFTs = new { Description = "Recognizes users who own multiple NFTs in their collection.", Badges = await GetNumberOfNftBadges(page, userId) },

                        //Creator = new { Description = "", Badges = await GetCreatorBadges(page, userId) },

                        //ProjectAffliation = new { Description = "", Badges = await GetProjectAffliationBadges(page, userId) },

                        PortfolioValue = new { Description = "Celebrates users with a portfolio reaching a significant overall value.", Badges = await GetPortfolioValueBadges(page, userId) },

                        PrivateLaunchTester = new { Description = "Users that were part of the Private Launch.", Badges = await GetPrivateLaunchTesterBadge(userId) },

                        AlphaLaunch = new { Description = "Users that were part of the Alpha Launch.", Badges = await GetAlphaLaunchBadge(userId) },

                    }
                };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetBlueChipAsync(int page)
        {
            var response = new ResponseData();
            try
            {
                response.Data = await _context.BlueChips
                    .OrderBy(x => x.CollectionName)
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .Select(x => new
                    {
                        x.CollectionName,
                        NftCount = x.UserBlueChips.Count(),
                        isOwned = x.UserBlueChips.FirstOrDefault() != null ? true : false,
                        x.ImageUrl
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = "Data Fetched";
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }



        private async Task<object?> GetBlueChipBadges(int page, Guid userId)
        {
            var query = $"%{BadgeStruct.BlueChip1.Split('-').First().Trim()}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                .OrderBy(_ => _.Order)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsUserEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<object?> GetNumberOfNftBadges(int page, Guid userId)
        {
            var query = $"%{BadgeStruct.NumberOfNft1.Split('-').First().Trim()}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                .OrderBy(_ => _.Order)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<object?> GetCreatorBadges(int page, Guid userId)
        {
            var query = $"%{BadgeStruct.Creator10.Split('-').First().Trim()}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                .OrderBy(_ => _.Order)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<object?> GetPortfolioValueBadges(int page, Guid userId)
        {
            var query = $"%{BadgeStruct.PortfolioValue1k.Split('-').First().Trim()}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                .OrderBy(_ => _.Order)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<object?> GetPrivateLaunchTesterBadge(Guid userId)
        {
            var query = $"%{BadgeStruct.AlphaTester}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                //.Skip(perPage * (page - 1))
                .Take(1)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<object?> GetProfileCompleteBadge(Guid userId)
        {
            var query = $"%{BadgeStruct.ProfileComplete}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                //.Skip(perPage * (page - 1))
                .Take(1)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<object?> GetAlphaLaunchBadge(Guid userId)
        {
            var query = $"%{BadgeStruct.AlphaLaunch}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                //.Skip(perPage * (page - 1))
                .Take(1)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<object?> GetProjectAffliationBadges(int page, Guid userId)
        {
            var query = $"%{BadgeStruct.ProjectAffliation1.Split('-').First().Trim()}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                .OrderBy(_ => _.Order)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<object?> GetTopValueNftBadges(int page, Guid userId)
        {
            var query = $"%{BadgeStruct.TopValueNft1k.Split('-').First().Trim()}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                .OrderBy(_ => _.Order)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<object?> GetUserInviteBadges(int page, Guid userId)
        {
            var query = $"%{BadgeStruct.UserInvites5.Split('-').First().Trim()}%";

            return await _context.Badges
                .Where(_ => EF.Functions.Like(_.BadgeName, query))
                .OrderBy(_ => _.Order)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(x => new
                {
                    x.BadgeName,
                    x.ImageUrl,
                    x.Description,
                    IsEarned = x.UserBadges.Where(_ => _.UserId == userId.ToByteArray() && _.Active == 1).FirstOrDefault() != null ? true : false,
                })
                .ToListAsync();
        }

        private async Task<bool> AddBadgesAsync()
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await AddProfileComplete();
                await AddBlueChipBadge();
                await AddTopValueNft();
                await AddNumberofNfts();
                await AddPortfolioValue();
                await AddCreator();
                await AddProjectAffliation();

                await AddAlphaTester();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        private async Task AddBlueChipBadge()
        {
            var badge1 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.BlueChip1,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge2 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.BlueChip3,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge3 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.BlueChip5,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge4 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.BlueChip10,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            await _context.Badges.AddRangeAsync(badge1, badge2, badge3, badge4);
        }

        private async Task AddTopValueNft()
        {
            var badge1 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.TopValueNft100,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge2 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.TopValueNft500,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge3 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.TopValueNft1k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge4 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.TopValueNft10k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge5 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.TopValueNft25k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge6 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.TopValueNft50k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge7 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.TopValueNft100k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            await _context.Badges.AddRangeAsync(badge1, badge2, badge3, badge4, badge5, badge6, badge7);
        }

        private async Task AddNumberofNfts()
        {
            var badge1 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.NumberOfNft1,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge2 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.NumberOfNft5,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge3 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.NumberOfNft10,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge4 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.NumberOfNft25,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            await _context.Badges.AddRangeAsync(badge1, badge2, badge3, badge4);
        }

        private async Task AddPortfolioValue()
        {
            var badge1 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.PortfolioValue1k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge2 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.PortfolioValue5k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge3 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.PortfolioValue10k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge4 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.PortfolioValue25k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge5 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.PortfolioValue50k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge6 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.PortfolioValue100k,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            await _context.Badges.AddRangeAsync(badge1, badge2, badge3, badge4, badge5, badge6);
        }

        private async Task AddCreator()
        {
            var badge1 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.Creator10,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge2 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.Creator50,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge3 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.Creator100,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge4 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.Creator200,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge5 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.Creator500,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            await _context.Badges.AddRangeAsync(badge1, badge2, badge3, badge4, badge5);
        }

        private async Task AddProjectAffliation()
        {
            var badge1 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.ProjectAffliation1,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge2 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.ProjectAffliation3,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge3 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.ProjectAffliation5,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            var badge4 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.ProjectAffliation10,
                Active = 1,
                Description = "",
                ImageUrl = null
            };

            await _context.Badges.AddRangeAsync(badge1, badge2, badge3, badge4);
        }

        private async Task AddProfileComplete()
        {
            var id = Guid.NewGuid().ToByteArray();
            var badge1 = new Badge
            {
                BadgeId = id,
                Active = 1,
                BadgeName = "ProfileComplete",
                Description = "",
                ImageUrl = null
            };

            await _context.Badges.AddAsync(badge1);

            foreach (var item in Enum.GetValues(typeof(ProfileCompleteMilestones)))
            {
                var miletones = new Milestone
                {
                    BadgeName = "ProfileComplete",
                    Description = "",
                    MilestoneName = item.ToString()!
                };

                var task = new MilestoneTask
                {
                    Description = "",
                    MilestonesName = item.ToString()!,
                    TaskName = item.ToString()!
                };

                await _context.AddRangeAsync(miletones, task);
            }
        }

        private async Task AddAlphaTester()
        {
            var badge1 = new Badge
            {
                BadgeId = Guid.NewGuid().ToByteArray(),
                BadgeName = BadgeStruct.AlphaTester,
                Active = 1,
                Description = "",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/ovation-technologies.appspot.com/o/badge-icons%2Falpha-tester%2FalphaTester.png?alt=media&token=b49f05ad-554b-4dc6-b5bb-d69c8b5774c2"
            };

            await _context.Badges.AddAsync(badge1);
        }
    }
}
