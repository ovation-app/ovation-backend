using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class Wallet
{
    public byte[] Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public sbyte Active { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<UserWalletGroup> UserWalletGroups { get; } = new List<UserWalletGroup>();

    public virtual ICollection<UserWallet> UserWallets { get; } = new List<UserWallet>();
}
