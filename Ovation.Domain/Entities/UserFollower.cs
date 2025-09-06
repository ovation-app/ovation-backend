using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserFollower
{
    public byte[] FollowId { get; set; } = null!;

    public byte[] FollowerId { get; set; } = null!;

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User Follower { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
