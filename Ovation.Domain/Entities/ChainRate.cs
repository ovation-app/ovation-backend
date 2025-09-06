using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class ChainRate
{
    public string Symbol { get; set; } = null!;

    public decimal? UsdRate { get; set; }
}
