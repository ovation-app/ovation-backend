using System.Text.Json.Serialization;

namespace Ovation.Application.DTOs.Tezos
{
    public class TezosCollectionData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("kind")]
        public string? Kind { get; set; }

        [JsonPropertyName("tzips")]
        public List<string>? Tzips { get; set; }

        [JsonPropertyName("balance")]
        public int Balance { get; set; }

        [JsonPropertyName("creator")]
        public Creatorr? Creator { get; set; }

        [JsonPropertyName("numContracts")]
        public int NumContracts { get; set; }

        [JsonPropertyName("tokensCount")]
        public int TokensCount { get; set; }

        [JsonPropertyName("activeTokensCount")]
        public int ActiveTokensCount { get; set; }

        [JsonPropertyName("tokenBalancesCount")]
        public int TokenBalancesCount { get; set; }

        [JsonPropertyName("tokenTransfersCount")]
        public int TokenTransfersCount { get; set; }

        [JsonPropertyName("ticketsCount")]
        public int TicketsCount { get; set; }

        [JsonPropertyName("activeTicketsCount")]
        public int ActiveTicketsCount { get; set; }

        [JsonPropertyName("ticketBalancesCount")]
        public int TicketBalancesCount { get; set; }

        [JsonPropertyName("ticketTransfersCount")]
        public int TicketTransfersCount { get; set; }

        [JsonPropertyName("numDelegations")]
        public int NumDelegations { get; set; }

        [JsonPropertyName("numOriginations")]
        public int NumOriginations { get; set; }

        [JsonPropertyName("numTransactions")]
        public int NumTransactions { get; set; }

        [JsonPropertyName("numReveals")]
        public int NumReveals { get; set; }

        [JsonPropertyName("numMigrations")]
        public int NumMigrations { get; set; }

        [JsonPropertyName("transferTicketCount")]
        public int TransferTicketCount { get; set; }

        [JsonPropertyName("increasePaidStorageCount")]
        public int IncreasePaidStorageCount { get; set; }

        [JsonPropertyName("eventsCount")]
        public int EventsCount { get; set; }

        [JsonPropertyName("firstActivity")]
        public int FirstActivity { get; set; }

        [JsonPropertyName("firstActivityTime")]
        public DateTime FirstActivityTime { get; set; }

        [JsonPropertyName("lastActivity")]
        public int LastActivity { get; set; }

        [JsonPropertyName("lastActivityTime")]
        public DateTime LastActivityTime { get; set; }

        [JsonPropertyName("typeHash")]
        public int TypeHash { get; set; }

        [JsonPropertyName("codeHash")]
        public int CodeHash { get; set; }

        [JsonPropertyName("metadata")]
        public Metaadataa? Metadata { get; set; }
    }

    public class Creatorr
    {
        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }

    public class Metaadataa
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("authors")]
        public List<string>? Authors { get; set; }

        [JsonPropertyName("homepage")]
        public string? Homepage { get; set; }

        [JsonPropertyName("imageUri")]
        public string? ImageUri { get; set; }

        [JsonPropertyName("interfaces")]
        public List<string>? Interfaces { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
