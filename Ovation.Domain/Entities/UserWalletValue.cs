using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserWalletValue
{
    public int Id { get; set; }

    public int NftCount { get; set; }

    public decimal NativeWorth { get; set; }

    public string Chain { get; set; } = null!;

    public byte[] UserWalletId { get; set; } = null!;

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual UserWallet UserWallet { get; set; } = null!;
}
