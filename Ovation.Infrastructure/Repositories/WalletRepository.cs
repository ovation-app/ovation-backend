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
    internal class WalletRepository(IServiceScopeFactory serviceScopeFactory) : BaseRepository<Wallet>(serviceScopeFactory), IWalletRepository
    {
        public async Task<ResponseData> AddWalletAsync(WalletDto walletDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var id = Guid.NewGuid();

                var path = new Wallet
                {
                    Id = id.ToByteArray(),
                    Name = walletDto.Name,
                    LogoUrl = walletDto.LogoUrl
                };

                await CreateAsync(path);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ResponseData { GuidValue = id, Status = true, Message = "Wallet saved!" };
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                await _unitOfWork.RollbackAsync();
                return new();
            }
        }

        public async Task<ResponseData> GetWalletAsync(Guid id)
        {
            var response = new ResponseData();
            try
            {
                response.Data = await _context.Wallets
                    .Where(_ => _.Id == id.ToByteArray())
                    .Select(x => new
                    {
                        WalletId = new Guid(x.Id),
                        x.Name,
                        x.LogoUrl
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

        public async Task<ResponseData> GetWalletsAsync()
        {
            var response = new ResponseData();
            try
            {
                response.Data = await _context.Wallets
                    .Where(x => x.Active == 1)
                    .Select(x => new
                    {
                        WalletId = new Guid(x.Id),
                        x.Name,
                        x.LogoUrl
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
