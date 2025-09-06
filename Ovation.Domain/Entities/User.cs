using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class User
{
    public byte[] UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string GoogleId { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Type { get; set; } = null!;

    public sbyte Active { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual UserAffilation? UserAffilation { get; set; }

    public virtual ICollection<UserBadge> UserBadges { get; } = new List<UserBadge>();

    public virtual ICollection<UserBlueChip> UserBlueChips { get; } = new List<UserBlueChip>();

    public virtual ICollection<UserExperience> UserExperiences { get; } = new List<UserExperience>();

    public virtual UserFavoriteNft? UserFavoriteNft { get; set; }

    public virtual ICollection<UserFeaturedItem> UserFeaturedItems { get; } = new List<UserFeaturedItem>();

    public virtual ICollection<UserFeedback> UserFeedbacks { get; } = new List<UserFeedback>();

    public virtual ICollection<UserFollower> UserFollowerFollowers { get; } = new List<UserFollower>();

    public virtual ICollection<UserFollower> UserFollowerUsers { get; } = new List<UserFollower>();

    public virtual ICollection<UserHighestNft> UserHighestNfts { get; } = new List<UserHighestNft>();

    public virtual ICollection<UserNftCollectionDatum> UserNftCollectionData { get; } = new List<UserNftCollectionDatum>();

    public virtual ICollection<UserNftCollection> UserNftCollections { get; } = new List<UserNftCollection>();

    public virtual ICollection<UserNftDatum> UserNftData { get; } = new List<UserNftDatum>();

    public virtual ICollection<UserNftRecord> UserNftRecords { get; } = new List<UserNftRecord>();

    public virtual ICollection<UserNftSale> UserNftSales { get; } = new List<UserNftSale>();

    public virtual ICollection<UserNftTransaction> UserNftTransactions { get; } = new List<UserNftTransaction>();

    public virtual ICollection<UserNft> UserNfts { get; } = new List<UserNft>();

    public virtual ICollection<UserNotification> UserNotificationInitiators { get; } = new List<UserNotification>();

    public virtual ICollection<UserNotification> UserNotificationReceivers { get; } = new List<UserNotification>();

    public virtual UserPath? UserPath { get; set; }

    public virtual ICollection<UserPathRecord> UserPathRecords { get; } = new List<UserPathRecord>();

    public virtual ICollection<UserPortfolioRecord> UserPortfolioRecords { get; } = new List<UserPortfolioRecord>();

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<UserProfileView> UserProfileViewUsers { get; } = new List<UserProfileView>();

    public virtual ICollection<UserProfileView> UserProfileViewViewers { get; } = new List<UserProfileView>();

    public virtual ICollection<UserReferral> UserReferralInvitees { get; } = new List<UserReferral>();

    public virtual ICollection<UserReferral> UserReferralInviters { get; } = new List<UserReferral>();

    public virtual UserSocial? UserSocial { get; set; }

    public virtual UserStat? UserStat { get; set; }

    public virtual ICollection<UserTask> UserTasks { get; } = new List<UserTask>();

    public virtual ICollection<UserWalletGroup> UserWalletGroups { get; } = new List<UserWalletGroup>();

    public virtual ICollection<UserWalletSalesRecord> UserWalletSalesRecords { get; } = new List<UserWalletSalesRecord>();

    public virtual ICollection<UserWalletValue> UserWalletValues { get; } = new List<UserWalletValue>();

    public virtual ICollection<UserWallet> UserWallets { get; } = new List<UserWallet>();

    public virtual ICollection<VerifiedUser> VerifiedUsers { get; } = new List<VerifiedUser>();

    public virtual ICollection<VerifyUser> VerifyUsers { get; } = new List<VerifyUser>();
}
