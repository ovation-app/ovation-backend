using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserNftSale
{
    public long Id { get; set; }

    public decimal SalePrice { get; set; }

    public string SaleCurrency { get; set; } = null!;

    public string SaleUrl { get; set; } = null!;

    public string? Metadata { get; set; }

    public long NftId { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? SaleCreatedDate { get; set; }

    public DateTime? SaleUpdatedDate { get; set; }

    public virtual UserNftDatum Nft { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
