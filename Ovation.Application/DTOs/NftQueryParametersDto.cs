namespace Ovation.Application.DTOs
{
    public class NftQueryParametersDto
    {
        public string? Chain { get; set; }
        public bool? Created { get; set; }
        public bool? Private { get; set; }
        public bool? ForSale { get; set; }
        public string? Next { get; set; }
        public Guid? Wallet { get; set; }
    }
}
