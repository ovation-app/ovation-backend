using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserNftDatum
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? TokenAddress { get; set; }

    public string? TokenId { get; set; }

    public string? ContractAddress { get; set; }

    public string? Chain { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? AnimationUrl { get; set; }

    public string? FloorPrice { get; set; }

    public string? LastTradePrice { get; set; }

    public string? LastTradeSymbol { get; set; }

    public string? MetaData { get; set; }

    public sbyte Public { get; set; }

    public sbyte Favorite { get; set; }

    public sbyte Created { get; set; }

    public sbyte ForSale { get; set; }

    public DateOnly? CustodyDate { get; set; }

    public long? NftId { get; set; }

    public long? CollectionId { get; set; }

    public byte[]? UserWalletId { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual UserNftCollectionDatum? Collection { get; set; }

    public virtual NftsDatum? Nft { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserHighestNft> UserHighestNfts { get; } = new List<UserHighestNft>();

    public virtual ICollection<UserNftSale> UserNftSales { get; } = new List<UserNftSale>();

    public virtual UserWallet? UserWallet { get; set; }
}
