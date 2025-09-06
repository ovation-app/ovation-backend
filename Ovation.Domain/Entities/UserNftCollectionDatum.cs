using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserNftCollectionDatum
{
    public long Id { get; set; }

    public byte[]? CollectionId { get; set; }

    public int? OwnsTotal { get; set; }

    public int? ItemTotal { get; set; }

    public string? FloorPrice { get; set; }

    public string? ContractName { get; set; }

    public string? ContractAddress { get; set; }

    public string? LogoUrl { get; set; }

    public string? Description { get; set; }

    public string? Symbol { get; set; }

    public sbyte? Verified { get; set; }

    public sbyte Created { get; set; }

    public sbyte? Spam { get; set; }

    public string? Chain { get; set; }

    public DateOnly? CustodyDate { get; set; }

    public byte[] UserId { get; set; } = null!;

    public byte[] UserWalletId { get; set; } = null!;

    public long? ParentCollection { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual NftCollectionsDatum? ParentCollectionNavigation { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserNftDatum> UserNftData { get; } = new List<UserNftDatum>();

    public virtual UserWallet UserWallet { get; set; } = null!;
}
