using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserNft
{
    public byte[] Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? TokenAddress { get; set; }

    public string? TokenId { get; set; }

    public string? MinterAddress { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? AnimationUrl { get; set; }

    public string? FloorPrice { get; set; }

    public string? MintPrice { get; set; }

    public string? LastTradePrice { get; set; }

    public string? LastTradeSymbol { get; set; }

    public sbyte? Cnft { get; set; }

    public string? MetaData { get; set; }

    public sbyte Public { get; set; }

    public sbyte Verified { get; set; }

    public byte[]? CollectionId { get; set; }

    public byte[]? UserWalletId { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual UserNftCollection? Collection { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual UserWallet? UserWallet { get; set; }
}
