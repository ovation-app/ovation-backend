using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class Newsletter
{
    public int Id { get; set; }

    public string SubscriberEmail { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
