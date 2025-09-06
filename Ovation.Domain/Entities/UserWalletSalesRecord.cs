using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserWalletSalesRecord
{
    public int Id { get; set; }

    public int TotalSold { get; set; }

    public decimal TotalSales { get; set; }

    public string Chain { get; set; } = null!;

    public byte[] WalletId { get; set; } = null!;

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual UserWallet Wallet { get; set; } = null!;
}
