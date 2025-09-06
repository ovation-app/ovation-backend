using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserFeaturedItem
{
    public int Id { get; set; }

    public string? Featured { get; set; }

    public string? ItemsType { get; set; }

    public string? ItemId { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
