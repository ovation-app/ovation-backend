using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class DappRadarCollectionDatum
{
    public int Id { get; set; }

    public int CollectionId { get; set; }

    public string Name { get; set; } = null!;

    public string? Link { get; set; }

    public string? Logo { get; set; }

    public int? DappId { get; set; }

    public string? AveragePrice { get; set; }

    public string? Volume { get; set; }

    public int? Sales { get; set; }

    public string? Metadata { get; set; }

    public string? FloorPrice { get; set; }

    public string? Traders { get; set; }

    public string? MarketCap { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
