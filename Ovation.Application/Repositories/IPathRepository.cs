using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Repositories
{
    public interface IPathRepository
    {
        Task<ResponseData> AddPathAsync(PathDto pathDto);

        Task<ResponseData> GetPathsAsync();

        Task<ResponseData> GetPathAsync(Guid id);
    }
}
