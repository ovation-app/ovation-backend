using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserWallet
{
    public byte[] Id { get; set; } = null!;

    public string WalletAddress { get; set; } = null!;

    public string Chain { get; set; } = null!;

    public string? Blockchain { get; set; }

    public string? NftsValue { get; set; }

    public int? NftCount { get; set; }

    public int? TotalSold { get; set; }

    public decimal? TotalSales { get; set; }

    public string? MetaData { get; set; }

    public sbyte Migrated { get; set; }

    public byte[]? WalletGroupId { get; set; }

    public byte[]? WalletId { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserBlueChip> UserBlueChips { get; } = new List<UserBlueChip>();

    public virtual ICollection<UserHighestNft> UserHighestNfts { get; } = new List<UserHighestNft>();

    public virtual ICollection<UserNftCollectionDatum> UserNftCollectionData { get; } = new List<UserNftCollectionDatum>();

    public virtual ICollection<UserNftCollection> UserNftCollections { get; } = new List<UserNftCollection>();

    public virtual ICollection<UserNftDatum> UserNftData { get; } = new List<UserNftDatum>();

    public virtual ICollection<UserNftTransaction> UserNftTransactions { get; } = new List<UserNftTransaction>();

    public virtual ICollection<UserNft> UserNfts { get; } = new List<UserNft>();

    public virtual ICollection<UserWalletSalesRecord> UserWalletSalesRecords { get; } = new List<UserWalletSalesRecord>();

    public virtual ICollection<UserWalletValue> UserWalletValues { get; } = new List<UserWalletValue>();

    public virtual Wallet? Wallet { get; set; }

    public virtual UserWalletGroup? WalletGroup { get; set; }
}
