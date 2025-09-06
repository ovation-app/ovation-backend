using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserProfile
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Location { get; set; }

    public string? Bio { get; set; }

    public string? ProfileImage { get; set; }

    public string? CoverImage { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
