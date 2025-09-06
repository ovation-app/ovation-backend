using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Repositories
{
    public interface IBadgeRepository
    {
        Task<ResponseData> GetBadgesAsync(int page, Guid userId);

        Task<ResponseData> GetBlueChipAsync(int page);
    }
}
