using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserNotification
{
    public byte[] Id { get; set; } = null!;

    public string Reference { get; set; } = null!;

    public string? ReferenceId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public sbyte Viewed { get; set; }

    public byte[]? InitiatorId { get; set; }

    public byte[] ReceiverId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? Initiator { get; set; }

    public virtual User Receiver { get; set; } = null!;
}
