namespace Ovation.Application.DTOs
{
    public class Trade
    {
        public string? TradeSymbol { get; set; }
        public string TradePrice { get; set; } = "";
        public string FloorPrice { get; set; } = "";
        public string? Chain { get; set; }

        public Trade(string? tradeSymbol, string? tradePrice, string floorprice, string chain)
        {
            TradePrice = tradePrice;
            Chain = chain;
            TradeSymbol = tradeSymbol;
            FloorPrice = floorprice;
        }
    }
}
