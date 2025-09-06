using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class BlueChip
{
    public int Id { get; set; }

    public string CollectionName { get; set; } = null!;

    public string? NftCount { get; set; }

    public string? ImageUrl { get; set; }

    public string ContractAddress { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<UserBlueChip> UserBlueChips { get; } = new List<UserBlueChip>();
}
