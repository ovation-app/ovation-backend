using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Persistence.Repositories
{
    internal class PathRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<PathType>(serviceScopeFactory), IPathRepository
    {
        public async Task<ResponseData> AddPathAsync(PathDto pathDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var id = Guid.NewGuid();

                var path = new PathType
                {
                    Id = id.ToByteArray(),
                    Name = pathDto.Name,
                    Description = pathDto.Description
                };

                await CreateAsync(path);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { GuidValue = id, Status = true, Message = "Path saved!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> GetPathAsync(Guid id)
        {
            var response = new ResponseData();
            try
            {
                response.Data = await _context.PathTypes
                    .Where(_ => _.Id == id.ToByteArray())
                    .Select(x => new
                    {
                        PathId = new Guid(x.Id),
                        x.Name,
                        x.Description
                    })
                    .SingleOrDefaultAsync();

                response.Status = true;
                return response;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return new();
            }
        }

        public async Task<ResponseData> GetPathsAsync()
        {
            var response = new ResponseData();
            try
            {
                response.Data = await _context.PathTypes
                    .OrderBy(x => x.CreatedDate)
                    .Select(x => new
                    {
                        PathId = new Guid(x.Id),
                        x.Name,
                        x.Description
                    })
                    .ToListAsync();

                response.Status = true;
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
