using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class VerifyUser
{
    public int Id { get; set; }

    public byte[] UserCode { get; set; } = null!;

    public int Otp { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
