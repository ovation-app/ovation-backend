using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserPortfolioRecord
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public decimal? UsdValue { get; set; }

    public string? MultiValue { get; set; }

    public string? Chain { get; set; }

    public string? Address { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
