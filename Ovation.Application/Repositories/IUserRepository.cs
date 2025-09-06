using Ovation.Application.DTOs;
using Ovation.Application.DTOs.SignUp;
using Ovation.Application.DTOs.SignUp.Claimable;
using Ovation.Domain.Entities;
namespace Ovation.Application.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email, CancellationToken cancellationToken = default);

    Task<bool> FindEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> FindEmailAsync(string email, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> FindUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<bool> FindUsernameAsync(string username, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> FindXAccountAsync(string xId, CancellationToken cancellationToken = default);

    Task<bool> FindUserWalletAsync(string address, CancellationToken cancellationToken = default);

    Task<bool> FindWalletAsync(string address, string chain, Guid userId, CancellationToken cancellationToken = default);

    bool ScreenWallet(List<UserWallet> wallets, string chain, Guid userId, bool isAuto);

    Task<ResponseData> AddUserAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);

    Task<ResponseData> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);

    Task<ResponseData> GoogleLoginAsync(string email, CancellationToken cancellationToken = default);

    Task<ResponseData> XLoginAsync(string xId, CancellationToken cancellationToken = default);

    Task<ResponseData> ForgetPasswordAsync(string email, CancellationToken cancellationToken = default);

    Task<ResponseData> ChangePasswordAsync(ChangePasswordDto changePassword, CancellationToken cancellationToken = default);

    Task<ResponseData> ChangePasswordAsync(PasswordModDto changePassword, Guid userId, CancellationToken cancellationToken = default);

    Task SendVerificationLinkAsync(Guid userId, string email, CancellationToken cancellationToken = default);

    Task<ResponseData> VerifyUserAsync(Ulid code, int otp, CancellationToken cancellationToken = default);

    Task<ResponseData> IsAccountVerified(Guid userId, CancellationToken cancellationToken = default);

    Task<Guid> GetUserAsync(string socialId, CancellationToken cancellationToken = default);

    Task<ResponseData> CreateClaimableProfileAsync(ClaimableProfile claimableProfile, CancellationToken cancellationToken = default);
}