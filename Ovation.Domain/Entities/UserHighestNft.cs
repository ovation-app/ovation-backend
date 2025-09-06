using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserHighestNft
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? Worth { get; set; }

    public decimal? Usd { get; set; }

    public string? TradeSymbol { get; set; }

    public string? Chain { get; set; }

    public long NftId { get; set; }

    public byte[] WalletId { get; set; } = null!;

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual UserNftDatum Nft { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual UserWallet Wallet { get; set; } = null!;
}
