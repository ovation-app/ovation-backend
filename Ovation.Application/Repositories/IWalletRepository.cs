using Ovation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.Repositories
{
    public interface IWalletRepository
    {
        Task<ResponseData> AddWalletAsync(WalletDto walletDto);

        Task<ResponseData> GetWalletsAsync();

        Task<ResponseData> GetWalletAsync(Guid id);
    }
}
