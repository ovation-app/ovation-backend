namespace Ovation.Application.DTOs
{
    public class NFTValue
    {
        public string? Chain { get; set; }

        public decimal Native { get; set; }

        public decimal Usd { get; set; }

        public string? Name { get; set; }

        public string? ImageUrl { get; set; }

        public byte[]? wallet { get; set; }

        public string? TradeSymbol { get; set; }
    }
}
