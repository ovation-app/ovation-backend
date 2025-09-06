using Microsoft.EntityFrameworkCore;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Data;

namespace Ovation.Persistence.Repositories
{
    internal class DevRepository : BaseRepository<DeveloperToken>, IDevRepository
    {
        public DevRepository(OvationDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }

        public async Task<bool> VerifyCoreToken(Guid tokenId)
        {
            try
            {
                var res = await _context.DeveloperTokens
                .Where(d => d.TokenId == tokenId.ToByteArray() && d.Active == 1 && d.Role == 1)
                .Select(x => x.TokenId)
                .SingleOrDefaultAsync();

                if (res == null)
                    return false;

                return true;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return false;
            }
        }

        public async Task<bool> VerifyToken(Guid tokenId)
        {
            try
            {
                var res = await _context.DeveloperTokens
                .Where(d => d.TokenId == tokenId.ToByteArray() && d.Active == 1)
                .Select(x => x.TokenId)
                .SingleOrDefaultAsync();

                if (res == null)
                    return false;

                return true;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return false;
            }
        }
    }
}
