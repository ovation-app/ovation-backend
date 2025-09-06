namespace Ovation.Application.DTOs
{
    public class FavNfts
    {
        public string Id { get; set; } = string.Empty!;

        public string? Name { get; set; }

        public string? ContractAddress { get; set; }

        public string? TokenId { get; set; }

        public string? Type { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string? CollectionName { get; set; }

        public string? NativePrices { get; set; }

        public string? USD { get; set; }
    }
}
