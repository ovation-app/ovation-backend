using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserStat
{
    public int Id { get; set; }

    public long Followers { get; set; }

    public long Xfollowers { get; set; }

    public long Following { get; set; }

    public long Xfollowing { get; set; }

    public long NftCreated { get; set; }

    public long NftCollected { get; set; }

    public long NftCollections { get; set; }

    public int FounderNft { get; set; }

    public long TotalNft { get; set; }

    public int SoldNftsTotal { get; set; }

    public decimal SoldNftsValue { get; set; }

    public long BadgeEarned { get; set; }

    public decimal Networth { get; set; }

    public int BluechipCount { get; set; }

    public long Views { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
