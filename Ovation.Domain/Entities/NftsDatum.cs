using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class NftsDatum
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? TokenAddress { get; set; }

    public string? TokenId { get; set; }

    public string? ContractAddress { get; set; }

    public string? MinterAddress { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? AnimationUrl { get; set; }

    public string? FloorPrice { get; set; }

    public string? MintPrice { get; set; }

    public string? LastTradePrice { get; set; }

    public string? LastTradeSymbol { get; set; }

    public sbyte? Cnft { get; set; }

    public string? MetaData { get; set; }

    public long? CollectionId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual NftCollectionsDatum? Collection { get; set; }

    public virtual ICollection<UserNftDatum> UserNftData { get; } = new List<UserNftDatum>();
}
