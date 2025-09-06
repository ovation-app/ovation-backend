using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Ovation.Application.DTOs;
using Ovation.Domain.Entities;

namespace Ovation.Persistence.Data;

public partial class OvationDbContext : DbContext
{
    public OvationDbContext(DbContextOptions<OvationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArchwayCollection> ArchwayCollections { get; set; }

    public virtual DbSet<Badge> Badges { get; set; }

    public virtual DbSet<BlueChip> BlueChips { get; set; }

    public virtual DbSet<ChainRate> ChainRates { get; set; }

    public virtual DbSet<DappRadarCollectionDatum> DappRadarCollectionData { get; set; }

    public virtual DbSet<DeveloperToken> DeveloperTokens { get; set; }

    public virtual DbSet<DumpDatum> DumpData { get; set; }

    public virtual DbSet<Milestone> Milestones { get; set; }

    public virtual DbSet<MilestoneTask> MilestoneTasks { get; set; }

    public virtual DbSet<Newsletter> Newsletters { get; set; }

    public virtual DbSet<NftCollectionsDatum> NftCollectionsData { get; set; }

    public virtual DbSet<NftsDatum> NftsData { get; set; }

    public virtual DbSet<PathType> PathTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAffilation> UserAffilations { get; set; }

    public virtual DbSet<UserBadge> UserBadges { get; set; }

    public virtual DbSet<UserBlueChip> UserBlueChips { get; set; }

    public virtual DbSet<UserExperience> UserExperiences { get; set; }

    public virtual DbSet<UserFavoriteNft> UserFavoriteNfts { get; set; }

    public virtual DbSet<UserFeaturedItem> UserFeaturedItems { get; set; }

    public virtual DbSet<Domain.Entities.UserFeedback> UserFeedbacks { get; set; }

    public virtual DbSet<UserFollower> UserFollowers { get; set; }

    public virtual DbSet<UserHighestNft> UserHighestNfts { get; set; }

    public virtual DbSet<UserNft> UserNfts { get; set; }

    public virtual DbSet<UserNftActivity> UserNftActivities { get; set; }

    public virtual DbSet<UserNftCollection> UserNftCollections { get; set; }

    public virtual DbSet<UserNftCollectionDatum> UserNftCollectionData { get; set; }

    public virtual DbSet<UserNftDatum> UserNftData { get; set; }

    public virtual DbSet<UserNftRecord> UserNftRecords { get; set; }

    public virtual DbSet<UserNftSale> UserNftSales { get; set; }

    public virtual DbSet<UserNftTransaction> UserNftTransactions { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    public virtual DbSet<UserPath> UserPaths { get; set; }

    public virtual DbSet<UserPathRecord> UserPathRecords { get; set; }

    public virtual DbSet<UserPortfolioRecord> UserPortfolioRecords { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserProfileView> UserProfileViews { get; set; }

    public virtual DbSet<UserReferral> UserReferrals { get; set; }

    public virtual DbSet<UserSocial> UserSocials { get; set; }

    public virtual DbSet<UserStat> UserStats { get; set; }

    public virtual DbSet<UserTask> UserTasks { get; set; }

    public virtual DbSet<UserWallet> UserWallets { get; set; }

    public virtual DbSet<UserWalletGroup> UserWalletGroups { get; set; }

    public virtual DbSet<UserWalletSalesRecord> UserWalletSalesRecords { get; set; }

    public virtual DbSet<UserWalletValue> UserWalletValues { get; set; }

    public virtual DbSet<VerifiedUser> VerifiedUsers { get; set; }

    public virtual DbSet<VerifyUser> VerifyUsers { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<XTargetAccount> XTargetAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<RankingDto>().HasNoKey();
        modelBuilder.Entity<NftDataDto>().HasNoKey();

        modelBuilder.Entity<ArchwayCollection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("archway_collections");

            entity.Property(e => e.CollectionName).HasMaxLength(45);
            entity.Property(e => e.ContractAddress).HasMaxLength(75);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.FloorPrice).HasPrecision(12, 5);
            entity.Property(e => e.Image).HasColumnType("text");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.BadgeId).HasName("PRIMARY");

            entity.ToTable("badges");

            entity.HasIndex(e => e.BadgeName, "badge_name").IsUnique();

            entity.Property(e => e.BadgeId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Active).HasDefaultValueSql("'1'");
            entity.Property(e => e.BadgeName).HasMaxLength(35);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ImageUrl).HasColumnType("text");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<BlueChip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("blue_chip");

            entity.HasIndex(e => e.CollectionName, "contractNa_idx");

            entity.HasIndex(e => e.ContractAddress, "contract_idx");

            entity.Property(e => e.CollectionName).HasMaxLength(65);
            entity.Property(e => e.ContractAddress).HasMaxLength(65);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.ImageUrl).HasColumnType("mediumtext");
            entity.Property(e => e.NftCount).HasMaxLength(20);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<ChainRate>(entity =>
        {
            entity.HasKey(e => e.Symbol).HasName("PRIMARY");

            entity.ToTable("chain_rates");

            entity.Property(e => e.Symbol)
                .HasMaxLength(15)
                .HasColumnName("symbol");
            entity.Property(e => e.UsdRate)
                .HasPrecision(18, 8)
                .HasColumnName("usd_rate");
        });

        modelBuilder.Entity<DappRadarCollectionDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("dapp_radar_collection_data");

            entity.Property(e => e.AveragePrice).HasMaxLength(45);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.FloorPrice).HasMaxLength(45);
            entity.Property(e => e.Link).HasColumnType("mediumtext");
            entity.Property(e => e.Logo).HasColumnType("mediumtext");
            entity.Property(e => e.MarketCap).HasMaxLength(45);
            entity.Property(e => e.Metadata).HasColumnType("json");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Traders).HasMaxLength(45);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.Volume).HasMaxLength(45);
        });

        modelBuilder.Entity<DeveloperToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PRIMARY");

            entity.ToTable("developer_tokens");

            entity.Property(e => e.TokenId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Active).HasDefaultValueSql("'1'");
            entity.Property(e => e.City).HasMaxLength(65);
            entity.Property(e => e.Country).HasMaxLength(45);
            entity.Property(e => e.CountryCode).HasMaxLength(5);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Email).HasMaxLength(45);
            entity.Property(e => e.FirstName).HasMaxLength(65);
            entity.Property(e => e.LastName).HasMaxLength(65);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Platform).HasMaxLength(15);
            entity.Property(e => e.Role).HasDefaultValueSql("'3'");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<DumpDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("dump_data");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Data)
                .HasColumnType("json")
                .HasColumnName("data");
            entity.Property(e => e.Type).HasMaxLength(45);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<Milestone>(entity =>
        {
            entity.HasKey(e => e.MilestoneId).HasName("PRIMARY");

            entity.ToTable("milestones");

            entity.HasIndex(e => e.BadgeName, "milestone_badge_idx");

            entity.HasIndex(e => e.MilestoneName, "milestone_name_idx").IsUnique();

            entity.Property(e => e.BadgeName).HasMaxLength(35);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.MilestoneName).HasMaxLength(35);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");

            entity.HasOne(d => d.BadgeNameNavigation).WithMany(p => p.Milestones)
                .HasPrincipalKey(p => p.BadgeName)
                .HasForeignKey(d => d.BadgeName)
                .HasConstraintName("badge_milestone");
        });

        modelBuilder.Entity<MilestoneTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PRIMARY");

            entity.ToTable("milestone_tasks");

            entity.HasIndex(e => e.MilestonesName, "milestone_task_idx");

            entity.HasIndex(e => e.TaskName, "task_name").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.MilestonesName).HasMaxLength(35);
            entity.Property(e => e.TaskName).HasMaxLength(35);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");

            entity.HasOne(d => d.MilestonesNameNavigation).WithMany(p => p.MilestoneTasks)
                .HasPrincipalKey(p => p.MilestoneName)
                .HasForeignKey(d => d.MilestonesName)
                .HasConstraintName("milestone_task");
        });

        modelBuilder.Entity<Newsletter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("newsletter");

            entity.HasIndex(e => e.SubscriberEmail, "SubscriberEmail_UNIQUE").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.SubscriberEmail).HasMaxLength(115);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<NftCollectionsDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("nft_collections_data");

            entity.HasIndex(e => e.Chain, "chain_idx");

            entity.HasIndex(e => e.ContractAddress, "contractAddr_idx");

            entity.HasIndex(e => e.ContractName, "contractName_idx");

            entity.HasIndex(e => e.ItemTotal, "itemTotal_idx");

            entity.HasIndex(e => e.Symbol, "symbol_idx");

            entity.Property(e => e.Chain).HasMaxLength(10);
            entity.Property(e => e.ContractAddress).HasMaxLength(155);
            entity.Property(e => e.ContractName).HasMaxLength(155);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.FloorPrice).HasMaxLength(50);
            entity.Property(e => e.MetaData).HasColumnType("json");
            entity.Property(e => e.Symbol).HasMaxLength(228);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<NftsDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("nfts_data");

            entity.HasIndex(e => e.Type, "chain_idx");

            entity.HasIndex(e => e.CollectionId, "collectionId_idx");

            entity.HasIndex(e => e.ContractAddress, "contractAddress_idx");

            entity.HasIndex(e => e.CreatedDate, "dateAdded_idx");

            entity.HasIndex(e => e.UpdatedDate, "dateUpdt_idx");

            entity.HasIndex(e => e.Name, "name_idx");

            entity.HasIndex(e => e.LastTradePrice, "price_idx");

            entity.HasIndex(e => e.TokenAddress, "tokenAddress_idx");

            entity.HasIndex(e => e.TokenId, "tokenId_idx");

            entity.Property(e => e.AnimationUrl).HasColumnType("text");
            entity.Property(e => e.Cnft).HasColumnName("CNft");
            entity.Property(e => e.ContractAddress).HasMaxLength(155);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.FloorPrice).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasColumnType("text");
            entity.Property(e => e.LastTradePrice).HasMaxLength(50);
            entity.Property(e => e.LastTradeSymbol).HasMaxLength(15);
            entity.Property(e => e.MetaData).HasColumnType("json");
            entity.Property(e => e.MintPrice).HasMaxLength(50);
            entity.Property(e => e.MinterAddress).HasMaxLength(155);
            entity.Property(e => e.TokenAddress).HasMaxLength(155);
            entity.Property(e => e.TokenId).HasMaxLength(155);
            entity.Property(e => e.Type).HasMaxLength(25);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");

            entity.HasOne(d => d.Collection).WithMany(p => p.NftsData)
                .HasForeignKey(d => d.CollectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("collectionId");
        });

        modelBuilder.Entity<PathType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("path_types");

            entity.HasIndex(e => e.Name, "Name_UNIQUE").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(65);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email_UNIQUE").IsUnique();

            entity.HasIndex(e => e.GoogleId, "gid_idx").IsUnique();

            entity.HasIndex(e => e.Type, "type_idx");

            entity.HasIndex(e => e.Username, "username_UNIQUE").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Email).HasMaxLength(75);
            entity.Property(e => e.GoogleId).HasMaxLength(75);
            entity.Property(e => e.Password).HasColumnType("text");
            entity.Property(e => e.Type).HasMaxLength(10);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.Username).HasMaxLength(45);
        });

        modelBuilder.Entity<UserAffilation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_affilation");

            entity.HasIndex(e => e.Code, "Code_UNIQUE").IsUnique();

            entity.HasIndex(e => e.UserId, "affilationUser_idx").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(15);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Invited)
                .HasDefaultValueSql("'0'")
                .HasColumnType("mediumint");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithOne(p => p.UserAffilation)
                .HasForeignKey<UserAffilation>(d => d.UserId)
                .HasConstraintName("affilationUser");
        });

        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_badges");

            entity.HasIndex(e => e.BadgeName, "badge_ref_idx");

            entity.HasIndex(e => e.UserId, "user_ref_idx");

            entity.Property(e => e.BadgeName).HasMaxLength(35);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.EarnedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.BadgeNameNavigation).WithMany(p => p.UserBadges)
                .HasPrincipalKey(p => p.BadgeName)
                .HasForeignKey(d => d.BadgeName)
                .HasConstraintName("badge_ref");

            entity.HasOne(d => d.User).WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_badge_ref");
        });

        modelBuilder.Entity<UserBlueChip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_blue_chips");

            entity.HasIndex(e => e.UserId, "bluchipUser_idx");

            entity.HasIndex(e => e.UserWalletId, "blueWallet_idx");

            entity.HasIndex(e => e.BluechipId, "bluechip_idx");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UserWalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.Bluechip).WithMany(p => p.UserBlueChips)
                .HasForeignKey(d => d.BluechipId)
                .HasConstraintName("bluechip");

            entity.HasOne(d => d.User).WithMany(p => p.UserBlueChips)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("bluchipUser");

            entity.HasOne(d => d.UserWallet).WithMany(p => p.UserBlueChips)
                .HasForeignKey(d => d.UserWalletId)
                .HasConstraintName("blueWallet");
        });

        modelBuilder.Entity<UserExperience>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_experiences");

            entity.HasIndex(e => e.UserId, "experience_user_idx");

            entity.Property(e => e.Id)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Company).HasMaxLength(105);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Department).HasMaxLength(75);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Role).HasMaxLength(75);
            entity.Property(e => e.Skill).HasMaxLength(75);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserExperiences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("experience_user");
        });

        modelBuilder.Entity<UserFavoriteNft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_favorite_nfts");

            entity.HasIndex(e => e.UserId, "favorite_nft_user_idx").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.FavoriteNfts).HasColumnType("json");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithOne(p => p.UserFavoriteNft)
                .HasForeignKey<UserFavoriteNft>(d => d.UserId)
                .HasConstraintName("favorite_nft_user");
        });

        modelBuilder.Entity<UserFeaturedItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_featured_items");

            entity.HasIndex(e => e.UserId, "featured_user_idx");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Featured).HasColumnType("json");
            entity.Property(e => e.ItemId).HasMaxLength(45);
            entity.Property(e => e.ItemsType).HasMaxLength(30);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserFeaturedItems)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("featured_user");
        });

        modelBuilder.Entity<Domain.Entities.UserFeedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_feedbacks");

            entity.HasIndex(e => e.UserId, "feedback_user_idx");

            entity.Property(e => e.Addition).HasColumnType("text");
            entity.Property(e => e.BiggestPain).HasColumnType("text");
            entity.Property(e => e.Confusion).HasColumnType("text");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Improvement).HasColumnType("text");
            entity.Property(e => e.LikelyRecommend).HasMaxLength(45);
            entity.Property(e => e.Satisfactory).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UsefulFeature).HasMaxLength(255);
            entity.Property(e => e.UserEmail).HasMaxLength(75);
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserFeedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("feedback_user");
        });

        modelBuilder.Entity<UserFollower>(entity =>
        {
            entity.HasKey(e => e.FollowId).HasName("PRIMARY");

            entity.ToTable("user_followers");

            entity.HasIndex(e => e.FollowerId, "follower_user_idx");

            entity.HasIndex(e => e.UserId, "user_ref_idx");

            entity.Property(e => e.FollowId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.FollowerId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.Follower).WithMany(p => p.UserFollowerFollowers)
                .HasForeignKey(d => d.FollowerId)
                .HasConstraintName("follower_user");

            entity.HasOne(d => d.User).WithMany(p => p.UserFollowerUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_ref");
        });

        modelBuilder.Entity<UserHighestNft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_highest_nfts");

            entity.HasIndex(e => e.NftId, "highNft");

            entity.HasIndex(e => e.UserId, "highUser_idx");

            entity.HasIndex(e => e.WalletId, "highWallet_idx");

            entity.Property(e => e.Chain).HasMaxLength(15);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Name).HasColumnType("text");
            entity.Property(e => e.TradeSymbol).HasMaxLength(15);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.Usd).HasPrecision(19, 2);
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.WalletId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Worth).HasPrecision(19, 9);

            entity.HasOne(d => d.Nft).WithMany(p => p.UserHighestNfts)
                .HasForeignKey(d => d.NftId)
                .HasConstraintName("highNft");

            entity.HasOne(d => d.User).WithMany(p => p.UserHighestNfts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("highUser");

            entity.HasOne(d => d.Wallet).WithMany(p => p.UserHighestNfts)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("highWallet");
        });

        modelBuilder.Entity<UserNft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_nfts");

            entity.HasIndex(e => e.Cnft, "cnft_idx");

            entity.HasIndex(e => e.FloorPrice, "floorPrice_idx");

            entity.HasIndex(e => e.MintPrice, "mintPrice_idx");

            entity.HasIndex(e => e.CollectionId, "nft_coll_idx");

            entity.HasIndex(e => e.MinterAddress, "nft_minter_addr");

            entity.HasIndex(e => e.UserId, "nft_user_idx");

            entity.HasIndex(e => e.UserWalletId, "nft_wallet_idx");

            entity.HasIndex(e => e.LastTradePrice, "tradePrice");

            entity.HasIndex(e => e.LastTradeSymbol, "tradeSym_idx");

            entity.Property(e => e.Id)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.AnimationUrl).HasColumnType("text");
            entity.Property(e => e.Cnft).HasColumnName("CNft");
            entity.Property(e => e.CollectionId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.FloorPrice).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasColumnType("text");
            entity.Property(e => e.LastTradePrice).HasMaxLength(50);
            entity.Property(e => e.LastTradeSymbol).HasMaxLength(15);
            entity.Property(e => e.MetaData).HasColumnType("json");
            entity.Property(e => e.MintPrice).HasMaxLength(50);
            entity.Property(e => e.MinterAddress).HasMaxLength(105);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Public).HasDefaultValueSql("'1'");
            entity.Property(e => e.TokenAddress).HasMaxLength(105);
            entity.Property(e => e.TokenId).HasMaxLength(105);
            entity.Property(e => e.Type).HasMaxLength(25);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UserWalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.Collection).WithMany(p => p.UserNfts)
                .HasForeignKey(d => d.CollectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("nft_coll");

            entity.HasOne(d => d.User).WithMany(p => p.UserNfts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("nft_user");

            entity.HasOne(d => d.UserWallet).WithMany(p => p.UserNfts)
                .HasForeignKey(d => d.UserWalletId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("nft_wallet");
        });

        modelBuilder.Entity<UserNftActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_nft_activities");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.EventDetails).HasColumnType("json");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<UserNftCollection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_nft_collections");

            entity.HasIndex(e => e.UserId, "collection_user_idx");

            entity.HasIndex(e => e.UserWalletId, "collection_wallet_idx");

            entity.HasIndex(e => e.ContractAddress, "contractAddr_idx");

            entity.HasIndex(e => e.ContractName, "contractName_idx");

            entity.HasIndex(e => e.FloorPrice, "floorPrice_idx");

            entity.HasIndex(e => e.ItemTotal, "itemTotal_idx");

            entity.HasIndex(e => e.OwnsTotal, "ownsTotal_idx");

            entity.HasIndex(e => e.Spam, "spam_idx");

            entity.HasIndex(e => e.Symbol, "symbol_idx");

            entity.HasIndex(e => e.Verified, "verified_idx");

            entity.Property(e => e.Id)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Chain).HasMaxLength(10);
            entity.Property(e => e.ContractAddress).HasMaxLength(65);
            entity.Property(e => e.ContractName).HasMaxLength(155);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.FloorPrice).HasMaxLength(50);
            entity.Property(e => e.LogoUrl).HasColumnType("text");
            entity.Property(e => e.Symbol).HasMaxLength(38);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UserWalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserNftCollections)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("collection_user");

            entity.HasOne(d => d.UserWallet).WithMany(p => p.UserNftCollections)
                .HasForeignKey(d => d.UserWalletId)
                .HasConstraintName("collection_wallet");
        });

        modelBuilder.Entity<UserNftCollectionDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_nft_collection_data");

            entity.HasIndex(e => e.UserId, "collection_user_idx");

            entity.HasIndex(e => e.UserWalletId, "collection_wallet_idx");

            entity.HasIndex(e => e.ContractAddress, "contractAddr_idx");

            entity.HasIndex(e => e.ContractName, "contractName_idx");

            entity.HasIndex(e => e.Created, "created_idx");

            entity.HasIndex(e => e.FloorPrice, "floorPrice_idx");

            entity.HasIndex(e => e.ItemTotal, "itemTotal_idx");

            entity.HasIndex(e => e.OwnsTotal, "ownsTotal_idx");

            entity.HasIndex(e => e.ParentCollection, "parent_collectionn_idx");

            entity.HasIndex(e => e.Spam, "spam_idx");

            entity.HasIndex(e => e.Symbol, "symbol_idx");

            entity.HasIndex(e => e.Verified, "verified_idx");

            entity.Property(e => e.Chain).HasMaxLength(10);
            entity.Property(e => e.CollectionId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.ContractAddress).HasMaxLength(155);
            entity.Property(e => e.ContractName).HasMaxLength(155);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.FloorPrice).HasMaxLength(50);
            entity.Property(e => e.Symbol).HasMaxLength(228);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UserWalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.ParentCollectionNavigation).WithMany(p => p.UserNftCollectionData)
                .HasForeignKey(d => d.ParentCollection)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("parent_collectionn");

            entity.HasOne(d => d.User).WithMany(p => p.UserNftCollectionData)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("collection_userr");

            entity.HasOne(d => d.UserWallet).WithMany(p => p.UserNftCollectionData)
                .HasForeignKey(d => d.UserWalletId)
                .HasConstraintName("collection_wallett");
        });

        modelBuilder.Entity<UserNftDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_nft_data");

            entity.HasIndex(e => e.FloorPrice, "floorPrice_idx");

            entity.HasIndex(e => e.ForSale, "idx_nft_for_sale");

            entity.HasIndex(e => new { e.UserId, e.ForSale }, "idx_nft_user_for_sale");

            entity.HasIndex(e => e.ContractAddress, "nftContrAddr_idx");

            entity.HasIndex(e => e.Created, "nftCreated");

            entity.HasIndex(e => e.Favorite, "nftFav_idx");

            entity.HasIndex(e => e.Name, "nftName_idx").HasAnnotation("MySql:FullTextIndex", true);

            entity.HasIndex(e => e.Public, "nftPublic");

            entity.HasIndex(e => e.TokenAddress, "nftTokAddr_idx");

            entity.HasIndex(e => e.TokenId, "nftToken_idx");

            entity.HasIndex(e => e.CollectionId, "nft_coll_idx");

            entity.HasIndex(e => e.NftId, "nft_nft");

            entity.HasIndex(e => e.UserId, "nft_user_idx");

            entity.HasIndex(e => e.UserWalletId, "nft_wallet_idx");

            entity.HasIndex(e => e.LastTradePrice, "tradePrice");

            entity.HasIndex(e => e.LastTradeSymbol, "tradeSym_idx");

            entity.Property(e => e.AnimationUrl).HasColumnType("text");
            entity.Property(e => e.Chain).HasMaxLength(25);
            entity.Property(e => e.ContractAddress).HasMaxLength(155);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.FloorPrice).HasMaxLength(50);
            entity.Property(e => e.ForSale).HasDefaultValueSql("'0'");
            entity.Property(e => e.LastTradePrice).HasMaxLength(50);
            entity.Property(e => e.LastTradeSymbol).HasMaxLength(15);
            entity.Property(e => e.MetaData).HasColumnType("json");
            entity.Property(e => e.Name).HasColumnType("text");
            entity.Property(e => e.Public).HasDefaultValueSql("'1'");
            entity.Property(e => e.TokenAddress).HasMaxLength(105);
            entity.Property(e => e.TokenId).HasMaxLength(105);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UserWalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.Collection).WithMany(p => p.UserNftData)
                .HasForeignKey(d => d.CollectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("nft_collection");

            entity.HasOne(d => d.Nft).WithMany(p => p.UserNftData)
                .HasForeignKey(d => d.NftId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("nft_nft");

            entity.HasOne(d => d.User).WithMany(p => p.UserNftData)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("nft_userr");

            entity.HasOne(d => d.UserWallet).WithMany(p => p.UserNftData)
                .HasForeignKey(d => d.UserWalletId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("nft_wallett");
        });

        modelBuilder.Entity<UserNftRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_nft_record");

            entity.HasIndex(e => e.UserId, "nftUserRef_idx");

            entity.Property(e => e.Address).HasMaxLength(65);
            entity.Property(e => e.Chain).HasMaxLength(15);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserNftRecords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("nftUserRef");
        });

        modelBuilder.Entity<UserNftSale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_nft_sales");

            entity.HasIndex(e => e.NftId, "idx_nft_sale");

            entity.HasIndex(e => e.UserId, "idx_user_sale");

            entity.Property(e => e.Metadata).HasColumnType("json");
            entity.Property(e => e.SaleCreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.SaleCurrency).HasMaxLength(10);
            entity.Property(e => e.SalePrice).HasPrecision(19, 9);
            entity.Property(e => e.SaleUpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.SaleUrl).HasColumnType("text");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.Nft).WithMany(p => p.UserNftSales)
                .HasForeignKey(d => d.NftId)
                .HasConstraintName("user_nft_sales_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.UserNftSales)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_nft_sales_ibfk_2");
        });

        modelBuilder.Entity<UserNftTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_nft_transactions");

            entity.HasIndex(e => e.Chain, "chain_idx");

            entity.HasIndex(e => e.ContractTokenId, "contrackToken_idx");

            entity.HasIndex(e => e.ContractAddress, "contractAddress_idx");

            entity.HasIndex(e => e.ContractName, "contractName_idx");

            entity.HasIndex(e => e.TranxDate, "date_idx");

            entity.HasIndex(e => e.EventType, "event_idx");

            entity.HasIndex(e => e.Fee, "fee_idx");

            entity.HasIndex(e => e.From, "from_idx");

            entity.HasIndex(e => e.Name, "name_idx");

            entity.HasIndex(e => e.TradeSymbol, "symb_idx");

            entity.HasIndex(e => e.To, "to_idx");

            entity.HasIndex(e => e.TokenId, "tokenId_idx");

            entity.HasIndex(e => e.TradePrice, "trade_idx");

            entity.HasIndex(e => e.UserId, "tranxUser_idx");

            entity.HasIndex(e => e.UserWalletId, "tranxWallet_idx");

            entity.HasIndex(e => e.TranxId, "tranx_idx");

            entity.Property(e => e.Chain).HasMaxLength(65);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Data).HasColumnType("json");
            entity.Property(e => e.EventType).HasColumnType("enum('Mint','Sale','Transfer','Burn')");
            entity.Property(e => e.ExchangeName).HasMaxLength(75);
            entity.Property(e => e.Fee).HasMaxLength(65);
            entity.Property(e => e.From).HasMaxLength(155);
            entity.Property(e => e.Image).HasColumnType("mediumtext");
            entity.Property(e => e.Name).HasMaxLength(155);
            entity.Property(e => e.To).HasMaxLength(155);
            entity.Property(e => e.TradePrice).HasMaxLength(50);
            entity.Property(e => e.TradeSymbol).HasMaxLength(15);
            entity.Property(e => e.TranxDate).HasColumnType("datetime");
            entity.Property(e => e.TranxId).HasMaxLength(225);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UserWalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserNftTransactions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("tranxUser");

            entity.HasOne(d => d.UserWallet).WithMany(p => p.UserNftTransactions)
                .HasForeignKey(d => d.UserWalletId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tranxWallet");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_notifications");

            entity.HasIndex(e => e.InitiatorId, "notification_initiator_idx");

            entity.HasIndex(e => e.ReceiverId, "notification_receiver_idx");

            entity.Property(e => e.Id)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.InitiatorId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.ReceiverId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Reference).HasMaxLength(45);
            entity.Property(e => e.ReferenceId).HasMaxLength(45);
            entity.Property(e => e.Title).HasMaxLength(45);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");

            entity.HasOne(d => d.Initiator).WithMany(p => p.UserNotificationInitiators)
                .HasForeignKey(d => d.InitiatorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notification_initiator");

            entity.HasOne(d => d.Receiver).WithMany(p => p.UserNotificationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("notification_receiver");
        });

        modelBuilder.Entity<UserPath>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_paths");

            entity.HasIndex(e => e.PathId, "path_ref_idx");

            entity.HasIndex(e => e.UserId, "path_user_idx").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.PathId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.Path).WithMany(p => p.UserPaths)
                .HasForeignKey(d => d.PathId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("path_ref");

            entity.HasOne(d => d.User).WithOne(p => p.UserPath)
                .HasForeignKey<UserPath>(d => d.UserId)
                .HasConstraintName("path_user");
        });

        modelBuilder.Entity<UserPathRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_path_record");

            entity.HasIndex(e => e.PathId, "recPath_idx");

            entity.HasIndex(e => e.UserId, "recUser_idx");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.PathId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.Path).WithMany(p => p.UserPathRecords)
                .HasForeignKey(d => d.PathId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("recPath");

            entity.HasOne(d => d.User).WithMany(p => p.UserPathRecords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("recUser");
        });

        modelBuilder.Entity<UserPortfolioRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_portfolio_record");

            entity.HasIndex(e => e.UserId, "userPort_idx");

            entity.Property(e => e.Address).HasMaxLength(65);
            entity.Property(e => e.Chain).HasMaxLength(15);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.MultiValue).HasColumnType("json");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UsdValue).HasPrecision(32, 2);
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Value).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.UserPortfolioRecords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userPort");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_profiles");

            entity.HasIndex(e => e.UserId, "UserId_UNIQUE").IsUnique();

            entity.Property(e => e.Bio).HasColumnType("text");
            entity.Property(e => e.CoverImage).HasColumnType("text");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.DisplayName).HasMaxLength(155);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.ProfileImage).HasColumnType("text");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .HasConstraintName("profile_user");
        });

        modelBuilder.Entity<UserProfileView>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_profile_views");

            entity.HasIndex(e => e.UserId, "user_idx");

            entity.HasIndex(e => e.ViewerId, "user_viewer_idx");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.ViewerId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserProfileViewUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user");

            entity.HasOne(d => d.Viewer).WithMany(p => p.UserProfileViewViewers)
                .HasForeignKey(d => d.ViewerId)
                .HasConstraintName("user_viewer");
        });

        modelBuilder.Entity<UserReferral>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_referrals");

            entity.HasIndex(e => e.InviteeId, "referralInvitee_idx");

            entity.HasIndex(e => e.InviterId, "referralInviter_idx");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.InviteeId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.InviterId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");

            entity.HasOne(d => d.Invitee).WithMany(p => p.UserReferralInvitees)
                .HasForeignKey(d => d.InviteeId)
                .HasConstraintName("referralInvitee");

            entity.HasOne(d => d.Inviter).WithMany(p => p.UserReferralInviters)
                .HasForeignKey(d => d.InviterId)
                .HasConstraintName("referralInviter");
        });

        modelBuilder.Entity<UserSocial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_socials");

            entity.HasIndex(e => e.UserId, "UserId_UNIQUE").IsUnique();

            entity.Property(e => e.Blur).HasColumnType("text");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Ethico).HasColumnType("text");
            entity.Property(e => e.Forcaster).HasColumnType("text");
            entity.Property(e => e.Foundation).HasColumnType("text");
            entity.Property(e => e.Lens).HasColumnType("text");
            entity.Property(e => e.LinkedIn).HasColumnType("text");
            entity.Property(e => e.Magic).HasColumnType("text");
            entity.Property(e => e.Socials).HasColumnType("json");
            entity.Property(e => e.Twitter).HasColumnType("text");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Website).HasColumnType("text");

            entity.HasOne(d => d.User).WithOne(p => p.UserSocial)
                .HasForeignKey<UserSocial>(d => d.UserId)
                .HasConstraintName("social_user");
        });

        modelBuilder.Entity<UserStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_stats");

            entity.HasIndex(e => e.UserId, "stat_user_idx").IsUnique();

            entity.HasIndex(e => e.Xfollowers, "xFollowers_idx");

            entity.HasIndex(e => e.Xfollowing, "xFollowing_idx");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Networth).HasPrecision(22, 2);
            entity.Property(e => e.SoldNftsValue).HasPrecision(19, 2);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Xfollowers).HasColumnName("XFollowers");
            entity.Property(e => e.Xfollowing).HasColumnName("XFollowing");

            entity.HasOne(d => d.User).WithOne(p => p.UserStat)
                .HasForeignKey<UserStat>(d => d.UserId)
                .HasConstraintName("stat_user");
        });

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_tasks");

            entity.HasIndex(e => e.TaskName, "task_ref_idx");

            entity.HasIndex(e => e.UserId, "task_user_ref_idx");

            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.TaskName).HasMaxLength(35);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.TaskNameNavigation).WithMany(p => p.UserTasks)
                .HasPrincipalKey(p => p.TaskName)
                .HasForeignKey(d => d.TaskName)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("task_ref");

            entity.HasOne(d => d.User).WithMany(p => p.UserTasks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("task_user_ref");
        });

        modelBuilder.Entity<UserWallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_wallets");

            entity.HasIndex(e => e.WalletAddress, "wallet_address_idx");

            entity.HasIndex(e => e.Blockchain, "wallet_blockchain_idx");

            entity.HasIndex(e => e.Chain, "wallet_chain_idx");

            entity.HasIndex(e => e.WalletGroupId, "wallet_group_idx");

            entity.HasIndex(e => e.WalletId, "wallet_ref_idx");

            entity.HasIndex(e => e.UserId, "wallet_user_idx");

            entity.Property(e => e.Id)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Blockchain).HasMaxLength(10);
            entity.Property(e => e.Chain).HasMaxLength(35);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.MetaData).HasColumnType("json");
            entity.Property(e => e.NftsValue).HasMaxLength(50);
            entity.Property(e => e.TotalSales).HasPrecision(19, 2);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.WalletAddress).HasMaxLength(75);
            entity.Property(e => e.WalletGroupId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.WalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserWallets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("wallet_user");

            entity.HasOne(d => d.WalletGroup).WithMany(p => p.UserWallets)
                .HasForeignKey(d => d.WalletGroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("wallet_group");

            entity.HasOne(d => d.Wallet).WithMany(p => p.UserWallets)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("wallet_ref");
        });

        modelBuilder.Entity<UserWalletGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_wallet_group");

            entity.HasIndex(e => e.WalletId, "groupWallet_idx");

            entity.HasIndex(e => e.UserId, "userGroup");

            entity.Property(e => e.Id)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.WalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserWalletGroups)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userGroup");

            entity.HasOne(d => d.Wallet).WithMany(p => p.UserWalletGroups)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("groupWallet");
        });

        modelBuilder.Entity<UserWalletSalesRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_wallet_sales_record");

            entity.HasIndex(e => e.UserId, "userSales");

            entity.HasIndex(e => e.WalletId, "walletSales_idx");

            entity.Property(e => e.Chain).HasMaxLength(15);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.TotalSales).HasPrecision(19, 2);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.WalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserWalletSalesRecords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userSales");

            entity.HasOne(d => d.Wallet).WithMany(p => p.UserWalletSalesRecords)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("walletSales");
        });

        modelBuilder.Entity<UserWalletValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_wallet_values");

            entity.HasIndex(e => e.UserWalletId, "valueWallet_idx");

            entity.HasIndex(e => e.UserId, "walletUserv_idx");

            entity.Property(e => e.Chain).HasMaxLength(15);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.NativeWorth).HasPrecision(19, 8);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UserWalletId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.UserWalletValues)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("valueUser");

            entity.HasOne(d => d.UserWallet).WithMany(p => p.UserWalletValues)
                .HasForeignKey(d => d.UserWalletId)
                .HasConstraintName("valueWallet");
        });

        modelBuilder.Entity<VerifiedUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("verified_users");

            entity.HasIndex(e => e.Handle, "handle_idx");

            entity.HasIndex(e => e.TypeId, "typeId_idx");

            entity.HasIndex(e => e.UserId, "verifiedUser_idx");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Handle).HasMaxLength(75);
            entity.Property(e => e.MetaData).HasColumnType("json");
            entity.Property(e => e.Type).HasColumnType("enum('X','Lens','Farcaster','ENS')");
            entity.Property(e => e.TypeId).HasMaxLength(75);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.VerifiedUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("verifiedUser");
        });

        modelBuilder.Entity<VerifyUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("verify_user");

            entity.HasIndex(e => e.UserId, "userVerify_idx");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserCode)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.VerifyUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userVerify");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("wallets");

            entity.HasIndex(e => e.Name, "Name_UNIQUE").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(16)
                .IsFixedLength();
            entity.Property(e => e.Active).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.LogoUrl).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<XTargetAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("x_target_accounts");

            entity.HasIndex(e => e.Username, "Username_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Engaged, "engaged_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.Username).HasMaxLength(65);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
