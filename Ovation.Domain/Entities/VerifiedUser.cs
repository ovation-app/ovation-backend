using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class VerifiedUser
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Handle { get; set; } = null!;

    public string TypeId { get; set; } = null!;

    public string? MetaData { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
