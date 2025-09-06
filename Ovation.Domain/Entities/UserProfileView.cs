using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserProfileView
{
    public int Id { get; set; }

    public byte[] ViewerId { get; set; } = null!;

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual User Viewer { get; set; } = null!;
}
