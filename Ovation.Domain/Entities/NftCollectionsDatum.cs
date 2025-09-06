using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class NftCollectionsDatum
{
    public long Id { get; set; }

    public int? ItemTotal { get; set; }

    public string? FloorPrice { get; set; }

    public string? ContractName { get; set; }

    public string? ContractAddress { get; set; }

    public string? LogoUrl { get; set; }

    public string? Description { get; set; }

    public string? Symbol { get; set; }

    public sbyte? Verified { get; set; }

    public sbyte? Spam { get; set; }

    public string? MetaData { get; set; }

    public string? Chain { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<NftsDatum> NftsData { get; } = new List<NftsDatum>();

    public virtual ICollection<UserNftCollectionDatum> UserNftCollectionData { get; } = new List<UserNftCollectionDatum>();
}
