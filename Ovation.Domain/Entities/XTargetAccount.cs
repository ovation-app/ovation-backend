using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class XTargetAccount
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public sbyte Engaged { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
