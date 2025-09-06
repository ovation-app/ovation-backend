namespace Ovation.Application.DTOs.Apis
{
    public class DappRadarGetCollections
    {
        public bool Success { get; set; }
        public string? Range { get; set; }
        public List<Result>? Results { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public int ResultCount { get; set; }
        public int ResultsPerPage { get; set; }
    }

    public class Result
    {
        public int CollectionId { get; set; }
        public string? Name { get; set; }
        public List<string>? Chains { get; set; }
        public string? Logo { get; set; }
        public string? Link { get; set; }
        public int? DappId { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal AvgPricePercentageChange { get; set; }
        public decimal Volume { get; set; }
        public decimal VolumePercentageChange { get; set; }
        public int? Sales { get; set; }
        public decimal SalesPercentageChange { get; set; }
        public decimal? MarketCap { get; set; }
        public decimal? MarketCapPercentageChange { get; set; }
        public decimal? FloorPrice { get; set; }
        public decimal? FloorPricePercentageChange { get; set; }
        public decimal Traders { get; set; }
        public decimal TradersPercentageChange { get; set; }
    }
}
