using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class Milestone
{
    public int MilestoneId { get; set; }

    public string MilestoneName { get; set; } = null!;

    public string? Description { get; set; }

    public string BadgeName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Badge BadgeNameNavigation { get; set; } = null!;

    public virtual ICollection<MilestoneTask> MilestoneTasks { get; } = new List<MilestoneTask>();
}
