using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class UserNftTransaction
{
    public long Id { get; set; }

    public string? TranxId { get; set; }

    public string? ContractAddress { get; set; }

    public string? ContractName { get; set; }

    public string? ContractTokenId { get; set; }

    public string? Name { get; set; }

    public string? Image { get; set; }

    public string? TokenId { get; set; }

    public string? EventType { get; set; }

    public string? To { get; set; }

    public string? From { get; set; }

    public long? Qty { get; set; }

    public string? TradePrice { get; set; }

    public string? TradeSymbol { get; set; }

    public string? Fee { get; set; }

    public string? ExchangeName { get; set; }

    public DateTime? TranxDate { get; set; }

    public string? Data { get; set; }

    public string Chain { get; set; } = null!;

    public byte[]? UserWalletId { get; set; }

    public byte[] UserId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual UserWallet? UserWallet { get; set; }
}
