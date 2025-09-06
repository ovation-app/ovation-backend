using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class DumpDatum
{
    public int Id { get; set; }

    public string? Data { get; set; }

    public string Type { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
