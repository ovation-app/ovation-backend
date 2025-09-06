using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserAffilation
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int Invited { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
