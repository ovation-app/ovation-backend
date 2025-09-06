using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserFavoriteNft
{
    public int Id { get; set; }

    public string? FavoriteNfts { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
