using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;

namespace Ovation.Application.Repositories
{
    public interface IHomeRepository
    {
        Task<ResponseData> GetUsers();

        Task<ResponseData> GetNfts();

        Task<ResponseData> GetNftsFromWallet(WalletAcct walletAcct);
    }
}
