using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserSocial
{
    public int Id { get; set; }

    public string? LinkedIn { get; set; }

    public string? Lens { get; set; }

    public string? Forcaster { get; set; }

    public string? Blur { get; set; }

    public string? Foundation { get; set; }

    public string? Twitter { get; set; }

    public string? Magic { get; set; }

    public string? Ethico { get; set; }

    public string? Website { get; set; }

    public string? Socials { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
