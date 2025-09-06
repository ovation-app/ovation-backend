using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserPathRecord
{
    public int Id { get; set; }

    public byte[]? PathId { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual PathType? Path { get; set; }

    public virtual User User { get; set; } = null!;
}
