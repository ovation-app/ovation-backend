using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserBlueChip
{
    public int Id { get; set; }

    public int BluechipId { get; set; }

    public byte[] UserWalletId { get; set; } = null!;

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual BlueChip Bluechip { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual UserWallet UserWallet { get; set; } = null!;
}
