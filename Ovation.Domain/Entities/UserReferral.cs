using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserReferral
{
    public int Id { get; set; }

    public byte[] InviteeId { get; set; } = null!;

    public byte[] InviterId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User Invitee { get; set; } = null!;

    public virtual User Inviter { get; set; } = null!;
}
