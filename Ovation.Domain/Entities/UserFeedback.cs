using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserFeedback
{
    public int Id { get; set; }

    public string? Satisfactory { get; set; }

    public string? UsefulFeature { get; set; }

    public string? Improvement { get; set; }

    public string? Confusion { get; set; }

    public string? LikelyRecommend { get; set; }

    public string? Addition { get; set; }

    public string? BiggestPain { get; set; }

    public string UserEmail { get; set; } = null!;

    public byte[]? UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? User { get; set; }
}
