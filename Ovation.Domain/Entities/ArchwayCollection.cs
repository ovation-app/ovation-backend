using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class ArchwayCollection
{
    public int Id { get; set; }

    public string ContractAddress { get; set; } = null!;

    public string CollectionName { get; set; } = null!;

    public string Image { get; set; } = null!;

    public decimal FloorPrice { get; set; }

    public int Supply { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
