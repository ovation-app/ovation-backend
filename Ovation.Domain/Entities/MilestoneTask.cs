using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class MilestoneTask
{
    public int TaskId { get; set; }

    public string TaskName { get; set; } = null!;

    public string? Description { get; set; }

    public string MilestonesName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Milestone MilestonesNameNavigation { get; set; } = null!;

    public virtual ICollection<UserTask> UserTasks { get; } = new List<UserTask>();
}
