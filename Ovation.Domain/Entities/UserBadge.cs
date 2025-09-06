using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserBadge
{
    public int Id { get; set; }

    public DateTime EarnedAt { get; set; }

    public sbyte Active { get; set; }

    public string BadgeName { get; set; } = null!;

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Badge BadgeNameNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
