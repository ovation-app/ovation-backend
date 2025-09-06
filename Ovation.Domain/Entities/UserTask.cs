using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserTask
{
    public int Id { get; set; }

    public DateTime CompletedAt { get; set; }

    public string? TaskName { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual MilestoneTask? TaskNameNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
