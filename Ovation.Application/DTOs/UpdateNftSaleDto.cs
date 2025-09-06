namespace Ovation.Application.DTOs;

public class UpdateNftSaleDto
{
    public bool ForSale { get; set; }
    public decimal SalePrice { get; set; }
    public string SaleCurrency { get; set; } = string.Empty!;
    public string SaleUrl { get; set; } = string.Empty!;
    public object? Metadata { get; set; }
} 