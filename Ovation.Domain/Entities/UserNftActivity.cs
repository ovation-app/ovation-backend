using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserNftActivity
{
    public int Id { get; set; }

    public string? EventDetails { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
