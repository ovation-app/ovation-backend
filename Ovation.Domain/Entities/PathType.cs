using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class PathType
{
    public byte[] Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<UserPathRecord> UserPathRecords { get; } = new List<UserPathRecord>();

    public virtual ICollection<UserPath> UserPaths { get; } = new List<UserPath>();
}
