using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class Badge
{
    public byte[] BadgeId { get; set; } = null!;

    public string BadgeName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public sbyte Order { get; set; }

    public int Active { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Milestone> Milestones { get; } = new List<Milestone>();

    public virtual ICollection<UserBadge> UserBadges { get; } = new List<UserBadge>();
}
