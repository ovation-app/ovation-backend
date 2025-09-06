using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ovation.Application.Common.Interfaces;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums.Badges.ProfileComplete.Milestones;
using Ovation.Application.DTOs.SignUp;
using Ovation.Application.DTOs.SignUp.Claimable;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services;
using Type = Ovation.Application.DTOs.SignUp.Type;

namespace Ovation.Persistence.Repositories;

public class UserRepository(IServiceScopeFactory services) : BaseRepository<User>(services), IUserRepository
{
    public async Task<ResponseData> AddUserAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        var validationSpan = _sentryService.StartSpan("validate.registrationDto", "Validating registrationDto data");

        // Cleaned up and optimized registration validation block
        if (!string.IsNullOrEmpty(registerDto.PersonalInfo.Email) && !HelperFunctions.IsValidEmail(registerDto.PersonalInfo.Email))
                return new ResponseData { Message = "Invalid Email Address" };

        if (registerDto.Type == Type.X.ToString())
        {
            if (string.IsNullOrEmpty(registerDto.PersonalInfo.XId))
                return new ResponseData { Message = "User X ID is missing" };

            // Only check for X account existence once
            if (await FindXAccountAsync(registerDto.PersonalInfo.XId!))
                return new ResponseData { Message = "X account already exist", StatusCode = 409 };
        }
        else
        {
            if (string.IsNullOrEmpty(registerDto.PersonalInfo.Email))
                return new ResponseData { Message = "Email address is required" };

            // Only check for email existence if email is valid and present
            if (await FindEmailAsync(registerDto.PersonalInfo.Email))
                return new ResponseData { Message = "Email address already exist", StatusCode = 409 };
        }

        // Username check (applies to all types)
        if (await FindUsernameAsync(registerDto.PersonalInfo.Username))
            return new ResponseData { Message = "Username already exist", StatusCode = 409 };

        var userId = Guid.NewGuid();
        var info = registerDto.PersonalInfo;
        var userWallet = registerDto.UserWallet;


        if (userWallet is not null && userWallet.Any(uw => uw.WalletTypeId is not null))
        {
            var walletTypeIds = userWallet
                .Where(uw => uw.WalletTypeId is not null)
                .Select(uw => uw.WalletTypeId.Value)
                .Distinct()
                .ToList();

            if (walletTypeIds.Count > 0)
            {
                foreach (var id in walletTypeIds)
                {
                    var idBytes = id.ToByteArray();
                    var walletTypeResult = await _context.Wallets.FirstOrDefaultAsync(p => p.Id == idBytes);
                    if (walletTypeResult == null)
                        return new ResponseData { Message = "Wallet Type does not exist" };
                }
            }
        }

        validationSpan?.Finish();

        try
        {
            var registerSpan = _sentryService.StartSpan("register.user", "Registration begins");
            await _unitOfWork.BeginTransactionAsync();
            
            if (registerDto.Type != Type.Normal.ToString())
            {
                var email = registerDto.Type == Type.Google.ToString() ? info.Email : $"{info.XId}@ovation.network";
                info.Password = $"{userId.ToByteArray()}{email}";
            }
            await PrepareNewUserAsync(userId, registerDto.Type, info, userWallet);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            _sentryService.AddBreadcrumb("Registration data commited", "register.user");

            _sentryService.AddBreadcrumb("Post registration events initiated", "post.register");
            await HandlePostRegistration(registerDto, userWallet, userId);

            _sentryService.AddBreadcrumb("Post registration events triggered", "post.register");

            registerSpan?.Finish();
            return new ResponseData
            {
                Status = true,
                Message = "Success",
                GuidValue = userId
            };
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            await _unitOfWork.RollbackAsync();
            return new ResponseData();
        }
        
    }

    private async Task HandlePostRegistration(RegisterDto registerDto, List<WalletAcct>? userWallets, Guid user)
    {
        if (user != Guid.Empty)
        {

            if (userWallets != null)
            {
                await _domainEvent.TaskPerformedEvent(user, ProfileCompleteMilestones.ConnectWallet.ToString());

                foreach (var userWallet in userWallets)
                {
                    foreach (var item in userWallet.Data)
                    {
                        await _domainEvent.WalletAddedEvent(user, item.WalletAddress, item.Chain, userWallet.WalletTypeId);

                        if (userWallet.WalletTypeId != null)
                        {
                            await _domainEvent.WalletOwnershipVerifiedEvent(item.WalletAddress);
                        }
                    }
                }
            }

            await _domainEvent.TaskPerformedEvent(user, ProfileCompleteMilestones.AddUsername.ToString());

            if (!string.IsNullOrEmpty(registerDto.PersonalInfo.Email))
                await _domainEvent.TaskPerformedEvent(user, ProfileCompleteMilestones.AddEmail.ToString());


            if (!string.IsNullOrEmpty(registerDto.Referral))
            {
                await _domainEvent.UserReferredEvent(user, registerDto.Referral);
            }

            if (registerDto.Type == Type.X.ToString())
            {
                await _domainEvent.XAccountConnectedEvent(user);
            }
        }
    }

    public async Task<ResponseData> ChangePasswordAsync(ChangePasswordDto changePassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _context.Users.SingleOrDefaultAsync(u => u.UserId == changePassword.UserId.ToByteArray());

            if (response == null)
                return new();

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            response.Password = passwordHasher.HashPassword(changePassword.Password);
            var count = await _unitOfWork.SaveChangesAsync();

            return count > 0 ? new ResponseData { Status = true, Message = "Password Changed" } : new();

        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return new();
        }
    }

    public async Task<ResponseData> ChangePasswordAsync(PasswordModDto changePassword, Guid userId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var response = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId.ToByteArray());

            if (response == null)
                return new();

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            if (!passwordHasher.VerifyHashedPassword(response.Password, changePassword.OldPassword))
                return new ResponseData { Message = "Password incorrect" };

            response.Password = passwordHasher.HashPassword(changePassword.Password);

            var count = await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return count > 0 ? new ResponseData { Status = true, Message = "Password Changed" } : new();

        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            await _unitOfWork.RollbackAsync();
            return new();
        }
    }

    public async Task<bool> FindEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (result == null)
                return false;

            return true;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return false;
        }
    }

    public async Task<bool> FindEmailAsync(string email, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.Users.SingleOrDefaultAsync(u => u.Email == email && u.UserId != userId.ToByteArray());

            if (result == null)
                return false;

            return true;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return false;
        }
    }

    public async Task<bool> FindUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (result == null)
                return false;

            return true;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return false;
        }
    }

    public async Task<bool> FindUsernameAsync(string username, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.Users.SingleOrDefaultAsync(u => u.Username == username && u.UserId != userId.ToByteArray());

            if (result == null)
                return false;

            return true;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return false;
        }
    }

    public async Task<bool> FindUserWalletAsync(string address, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.UserWallets.FirstOrDefaultAsync(u => u.WalletAddress == address && u.WalletId != null);

            if (result == null)
                return false;

            return true;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return false;
        }
    }

    public async Task<bool> FindWalletAsync(string address, string chain, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.UserWallets.FirstOrDefaultAsync(u =>
            u.WalletAddress == address);

            if (result == null)
                return false;
            else if (result.WalletId != null)
                return true;            
            else if (result.Chain.Equals(chain, StringComparison.OrdinalIgnoreCase))
                return true;
            else if (result.UserId == userId.ToByteArray())
                return true;
            else if (Constant._evmChains.Contains(chain))
                return true;
            else
                return false;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return false;
        }
    }

    private bool ScreenWallet(UserWallet wallet, string chain, Guid userId, bool isAuto)
    {
        try
        {

            if (wallet == null)
                return false;
            else if (wallet.WalletId != null)
                return true;
            else if (wallet.UserId == userId.ToByteArray())
                return true;
            else if (isAuto)
                return true;
            else
                return false;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return false;
        }
    }

    public bool ScreenWallet(List<UserWallet> wallets, string chain, Guid userId, bool isAuto)
    {
        try
        {
            var isValidCount = 0;
            foreach (var item in wallets)
            {
                if (!ScreenWallet(item, chain, userId, isAuto)) isValidCount += 1;
            }

            return isValidCount == wallets.Count ? true : false;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return false;
        }
    }

    public async Task<bool> FindXAccountAsync(string xId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.Users.SingleOrDefaultAsync(u => u.GoogleId == $"{xId}@ovation.network" && u.Type == Type.X.ToString());

            if (result == null)
                return false;

            return true;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return false;
        }
    }

    public async Task<ResponseData> ForgetPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || !HelperFunctions.IsValidEmail(email))
                return new ResponseData { Message = "Invalid Email Address" };

            var isExist = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (isExist == null)
                return new ResponseData { StatusCode = 404 };

            var otp = await HelperFunctions.SendOtpResetPassword(email);

            if (otp == 0)
                return new ResponseData { Message = "OTP not sent!" };

            Constant._userOTP[new Guid(isExist.UserId)] = otp;

            return new ResponseData { Status = true, Message = "OTP Sent", GuidValue = new Guid(isExist.UserId) };
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return new();
        }
    }

    public async Task<User?> GetByEmail(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.IgnoreAutoIncludes().FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<Guid> GetUserAsync(string socialId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!socialId.Contains('@'))
                socialId = $"{socialId}@ovation.network";

            var userId = await _context.Users
                .Where(_ =>  _.GoogleId == socialId)
                .Select(_ => new Guid(_.UserId))
                .FirstOrDefaultAsync();

            return userId;
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return Guid.Empty;
        }
    }

    public async Task<ResponseData> GoogleLoginAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _context.Users.SingleOrDefaultAsync(u => u.GoogleId == email);

            if (response != null)
            {
                if (response.Type != Type.Google.ToString())
                    return new ResponseData { Status = true, GuidValue = new Guid(response.UserId) };

                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                var isPassword = passwordHasher.VerifyHashedPassword(response.Password, $"{response.UserId}{response.GoogleId}");
                if (!isPassword)
                    return new ResponseData { Message = "Authentication failed!" };

                return new ResponseData { Status = true, GuidValue = new Guid(response.UserId) };
            }

            return new ResponseData { Message = "User not found", StatusCode = 404 };
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return new ResponseData { Message = "Authentication failed!" };
        }
    }

    public async Task<ResponseData> IsAccountVerified(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
                .Where(_ => _.UserId == userId.ToByteArray())
                .Select(_ => new { _.Active, _.UserId }).FirstOrDefaultAsync();

        return new ResponseData { Status = user != null && user.Active == 1 };
    }

    public async Task<ResponseData> LoginAsync(LoginDto login, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _context.Users.SingleOrDefaultAsync(u => u.Email == login.UserId || u.Username == login.UserId);

            if (response != null)
            {
                if (response.Active == 0) return new ResponseData { Message = "Account not verified!" };

                if (response.Type != Type.Normal.ToString())
                    return new ResponseData { Message = "Authentication failed: It looks like you didn't signed up using an email and password." };

                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                if (!passwordHasher.VerifyHashedPassword(response.Password, login.Password))
                    return new ResponseData { Message = "Password incorrect" };

                return new ResponseData { Status = true, GuidValue = new Guid(response.UserId) };
            }

            return new ResponseData { Message = "User not found" };
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return new ResponseData { Message = "An error occurred" };
        }
    }

    public async Task SendVerificationLinkAsync(Guid userId, string email, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var verifyCode = Ulid.NewUlid();
            var otp = await HelperFunctions.SendOtpEmailVerification(email, verifyCode.ToString());

            var verifyUser = new VerifyUser
            {
                Otp = otp,
                UserCode = verifyCode.ToByteArray(),
                UserId = userId.ToByteArray()
            };

            await _context.VerifyUsers.AddAsync(verifyUser);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            await _unitOfWork.RollbackAsync();
        }
    }

    public async Task<ResponseData> VerifyUserAsync(Ulid code, int otp, CancellationToken cancellationToken = default)
    {
        var verifyUser = await _context.VerifyUsers
                .FirstOrDefaultAsync(_ => _.UserCode == code.ToByteArray());

        if (verifyUser == null) return new ResponseData { Message = "Invalid code", StatusCode = 404 };

        if (verifyUser.Otp != otp) return new ResponseData { Message = "Invalid OTP" };

        var user = await _context.Users
            .FirstOrDefaultAsync(_ => _.UserId == verifyUser.UserId);

        if (user == null) return new ResponseData { Message = "User not found", StatusCode = 404 };

        user.Active = 1;

        _context.VerifyUsers.Remove(verifyUser);
        await _unitOfWork.SaveChangesAsync();

        return new ResponseData { Message = "Account Verified", Status = true, GuidValue = new Guid(verifyUser.UserId) };
    }

    public async Task<ResponseData> XLoginAsync(string xId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _context.Users.SingleOrDefaultAsync(u => u.GoogleId == $"{xId}@ovation.network");

            if (response != null)
            {
                if (response.Type != Type.X.ToString())
                    return new ResponseData { Status = true, GuidValue = new Guid(response.UserId) };

                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                var isPassword = passwordHasher.VerifyHashedPassword(response.Password, $"{response.UserId}{response.GoogleId}");
                if (!isPassword)
                    return new ResponseData { Message = "Authentication failed!" };

                return new ResponseData { Status = true, GuidValue = new Guid(response.UserId) };
            }

            return new ResponseData { Message = "User not found", StatusCode = 404 };
        }
        catch (Exception _)
        {
            SentrySdk.CaptureException(_);
            return new ResponseData { Message = "Authentication failed!" };
        }
    }

    private async Task PrepareNewUserAsync(Guid userId, string type, PersonalInfo info, List<WalletAcct>? userWallets)
    {
        string gId = string.Empty;

        if (type != Type.Normal.ToString())
            gId = (type == Type.Google.ToString() ? info.Email : $"{info.XId}@ovation.network")!;
        else
            gId = info.Email;

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var user = new User
        {
            UserId = userId.ToByteArray(),
            Username = info.Username.Replace("@", "").Trim(),
            Email = !string.IsNullOrEmpty(info.Email) ? info.Email.Trim() : null,
            GoogleId = gId!,
            Password = passwordHasher.HashPassword(info.Password!),
            Type = type,
            Active = (sbyte)(type != Type.Normal.ToString() ? 1 : 0),
        };

        var profile = new UserProfile
        {
            UserId = userId.ToByteArray(),
            DisplayName = info.DisplayName.Trim(),
            ProfileImage = info.ImgUrl
        };

        //var path = new UserPath
        //{
        //    UserId = userId.ToByteArray(),
        //    PathId = userPath != null ? userPath.PathId.ToByteArray() : null
        //};

        var stats = new UserStat
        {
            UserId = userId.ToByteArray()
        };

        var socials = new UserSocial
        {
            UserId = userId.ToByteArray()
        };

        var favNft = new UserFavoriteNft
        {
            UserId = userId.ToByteArray()
        };

        var featured = new UserFeaturedItem
        {
            UserId = userId.ToByteArray()
        };

        var affilation = new UserAffilation
        {
            UserId = userId.ToByteArray(),
            Code = userId.ToString().Split('-').Last()
        };

        await _context.AddRangeAsync(user, profile, stats, socials, favNft, featured, affilation);

        //if (userPath != null)
        //{
        //    var record = new UserPathRecord { PathId = userPath.PathId.ToByteArray(), UserId = userId.ToByteArray() };

        //    await _context.UserPathRecords.AddAsync(record);
        //}

        if (type == Type.X.ToString())
        {
            var verified = new VerifiedUser
            {
                UserId = userId.ToByteArray(),
                Type = type,
                Handle = info.Username,
                TypeId = info.XId
            };

            await _context.VerifiedUsers.AddAsync(verified);
        }

        if (userWallets != null)
        {
            foreach (var userWallet in userWallets)
            {
                var id = userWallet.WalletTypeId != null ? userWallet.WalletTypeId.Value.ToByteArray() : null;

                var group = await _context.UserWalletGroups
                    .IgnoreAutoIncludes().AsNoTracking()
                    .FirstOrDefaultAsync(_ => _.UserId == userId.ToByteArray() && _.WalletId == id);

                var groupId = Guid.NewGuid().ToByteArray();

                if (group == null)
                {
                    var walletGroup = new UserWalletGroup
                    {
                        Id = groupId,
                        WalletId = id,
                        UserId = userId.ToByteArray()
                    };

                    await _context.UserWalletGroups.AddAsync(walletGroup);
                }
                else
                    groupId = group.Id;

                var addresses = userWallet.Data.Select(_ => _.WalletAddress.ToLower()).ToArray();

                var exitingWallets = await _context.UserWallets
                    .Where(u => addresses.Contains(u.WalletAddress))
                    .ToListAsync();

                var toRemove = new List<AddressData>();
                var isAuto = userWallet.WalletTypeId != null;
                foreach (var item in userWallet.Data)
                {
                    var wallets = exitingWallets.FindAll(_ => _.WalletAddress == item.WalletAddress).ToList();

                    if (ScreenWallet(wallets, item.Chain, userId, isAuto))
                    {
                        var wallet = new UserWallet
                        {
                            UserId = userId.ToByteArray(),
                            Id = Guid.NewGuid().ToByteArray(),
                            WalletAddress = userWallet != null ? item.WalletAddress.ToLower() : null,
                            WalletId = id,
                            NftCount = 0,
                            Chain = userWallet != null ? item.Chain.ToLower() : null,
                            WalletGroupId = groupId
                        };

                        await _context.UserWallets.AddAsync(wallet);
                    }
                    else
                        toRemove.Add(item);
                }

                foreach (var item in toRemove)
                {
                    userWallet?.Data.Remove(item);
                }
            }

        }
    }

    public async Task<ResponseData> CreateClaimableProfileAsync(ClaimableProfile claimableProfile, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _context.DumpData.AddAsync(new DumpDatum
            {

                Type = nameof(ClaimableProfile),
                Data = JsonConvert.SerializeObject(claimableProfile),
            });

            await _context.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return new ResponseData { Status = true, Message = "Claimable profile created successfully" };
        }
        catch (Exception _)
        {
            await _unitOfWork.RollbackAsync();
            SentrySdk.CaptureException(_);
            return new ResponseData { Status = false, Message = "An error occurred while creating claimable profile" };
        }
    }
}