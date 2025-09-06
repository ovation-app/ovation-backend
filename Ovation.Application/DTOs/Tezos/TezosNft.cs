namespace Ovation.Application.DTOs.Tezos
{
    public class TezosNft
    {
        public long? Id { get; set; }
        public Account? Account { get; set; }
        public Token? Token { get; set; }
        public string? Balance { get; set; }
        public int? TransfersCount { get; set; }
        public int? FirstLevel { get; set; }
        public DateTime? FirstTime { get; set; }
        public int? LastLevel { get; set; }
        public DateTime? LastTime { get; set; }
    }
}
