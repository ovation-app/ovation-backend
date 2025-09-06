using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserWalletGroup
{
    public byte[] Id { get; set; } = null!;

    public byte[] UserId { get; set; } = null!;

    public byte[]? WalletId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserWallet> UserWallets { get; } = new List<UserWallet>();

    public virtual Wallet? Wallet { get; set; }
}
