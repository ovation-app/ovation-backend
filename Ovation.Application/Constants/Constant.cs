using Ovation.Application.DTOs;
using Ovation.Application.DTOs.Enums;
using System.Collections.Concurrent;

namespace Ovation.Application.Constants
{
    public class Constant
    {
        public const string CORS = "OvationCors";
        public static string OvationKey = Environment.GetEnvironmentVariable("OVATION_KEY")!;
        public const string TokenLoc = "AppSettings:Token";


        public const string NoReplyEmail = "no-reply@ovation.network";
        public static string NoReplyEmailPassword = Environment.GetEnvironmentVariable("EMAIL_KEY")!;
        public const string EmailServer = "smtp.gmail.com";


        public const string Solana = "solana";
        public const string Archway = "archway";
        public const string Ton = "ton";
        public const string Cosmos = "cosmos";
        public const string Tezos = "tezos";
        public const string Stargaze = "stargaze";
        public const string StargazeQl = "stargazeQl";
        public const string Abstract = "abstract";
        public const string MagicEden = "magicEden";
        public const string Evm = "evm";
        public const string X = "x";
        public const string XOAuth = "XOAuth";

        public const string Eth = "eth";
        public const string Polygon = "polygon";
        public const string Base = "base";
        public const string Optimism = "optimism";

        public const string Alchemy = "Alchemy-";
        public const string Mintify = "Mintify-";

        public const decimal SolanaValue = 174.24M;
        public const decimal TonValue = 6.30M;
        public const decimal ArchwayValue = 0.03268M;
        public const decimal StargazeValue = 0.002508M;
        public const int StarsConvert = 1_000_000;
        public const decimal Microtez = 1000000M;
        public const decimal TezosValue = 1.34M;
        public const decimal EthValue = 2532.60M;
        public const long ArchConvert = 1000000000000000000;


        public static string MoralisApiKey = Environment.GetEnvironmentVariable("MORALIS_KEY")!;
        public static string SentryDNS = Environment.GetEnvironmentVariable("SENTRY_DNS")!;

        public static string NFTScanAPIKey { get; } = Environment.GetEnvironmentVariable("NFTSCAN_KEY")!;
        public static string MagicEdenApiKey { get; } = $"Bearer {Environment.GetEnvironmentVariable("MAGICEDEN_KEY")!}";
        public static string XToken { get; } = $"Bearer {Environment.GetEnvironmentVariable("X_KEY")}";
        public static string AlchemyKey { get; } = Environment.GetEnvironmentVariable("ALCHEMY_KEY")!;
        public static string DappRadarKey { get; } = Environment.GetEnvironmentVariable("DAPP_RADAR_KEY")!;
        public static string MintifyKey { get; } = Environment.GetEnvironmentVariable("MINTIFY_KEY")!;


        public static readonly ConcurrentDictionary<Guid, string> _userIdToConnectionId = new ConcurrentDictionary<Guid, string>();
        public static readonly ConcurrentDictionary<Guid, List<NotificationDto>> _offlineNotification = new ConcurrentDictionary<Guid, List<NotificationDto>>();

        public static readonly ConcurrentDictionary<Guid, List<string>> _offlineMessageNotification = new ConcurrentDictionary<Guid, List<string>>();

        public static readonly ConcurrentDictionary<Guid, int> _userOTP = new ConcurrentDictionary<Guid, int>();

        //public static readonly Dictionary<Guid, int> _emailOTP = new Dictionary<Guid, int>();

        public static readonly string _founderNft = "0x495f947276749Ce646f68AC8c248420045cb7b5e".ToLower();
        public static readonly string _founderNftTokenId = "14350992107356367745555198297131324031365234497482008347432898888468411711688";

        public static readonly Dictionary<string, string> _blueChipNfts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"0xb47e3cd837ddf8e4c57f05d70ab865de6e193bbb".ToLower(), "CryptoPunks"},
            {"0x425c934d57d38f59888616fc09fe133bde48bacc".ToLower(), "Bored Ape Yacht Club"},
            {"0x60e4d786628fea6478f785a6d7e704777c86a7c6".ToLower(), "Mutant Ape Yacht Club"},
            {"0x893114a65354f8f2b319cba5b99106dc653e69b9".ToLower(), "CLONE X - X TAKASHI MURAKAMI"},
            {"0x7bd29408f11d2bfc23c34f18275bbf23bb716bc7".ToLower(), "Meebits"},
            {"0xed5af388653567af2f388e6224dc7c4b3241c544".ToLower(), "Azuki"},
            {"0x23581767a106ae21c074b2276d25e5c3e136a68b".ToLower(), "Moonbirds"},
            {"0x8a90cab2b38dba80c64b7734e58ee1db38b8992e".ToLower(), "Doodles"},
            {"0xba30e5f9bb24caa003e9f2f0497ad287fdf95623".ToLower(), "Bored Ape Kennel Club"},
            {"0x0000995e7ab2aa13c5c735484f3b816de7c353f4".ToLower(), "Cool Cats NFT"},
            {"0xe785e82358879f061bc3dcac6f0444462d4b5330".ToLower(), "World of Women"},
            {"0x08d7c0242953446436f34b4c78fe9da38c73668d".ToLower(), "PROOF Collective"},
            {"0x620b70123fb810f6c653da7644b5dd0b6312e4d8".ToLower(), "Space Doodles"},
            {"0x57a204aa1042f6e66dd7730813f4024114d74f37".ToLower(), "CyberKongz"},
            {"0x9c8ff314c9bc7f6e59a9d9225fb22946427edc03".ToLower(), "Nouns"},
            {"0xbd3531da5cf5857e7cfaa92426877b022e612cf8".ToLower(), "Pudgy Penguins"},
            {"0x39ee2c7b3cb80254225884ca001f57118c8f21b6".ToLower(), "The Potatoz"},
            {"0x769272677fab02575e84945f03eca517acc544cc".ToLower(), "The Captainz"},
            {"0x7b1dba3446c064dc54c86d9a7cd97e1f43c76b93".ToLower(), "Degods"},
            {"0xe012baf811cf9c05c408e879c399960d1f305903".ToLower(), "Otherside Koda"},
            {"0x59325733eb952a92e069c87f0a6168b29e80627f".ToLower(), "Mocaverse"},
            {"0xa8fcc9083506da7fde86a287ef187ac6de1b931a".ToLower(), "Chromie Squiggle by Snowfro"},
            {"0xd4e4078ca3495de5b1d4db434bebc5a986197782".ToLower(), "Autoglyphs"},
            //{"0xa319c382b0edee902e9aB4085aBDB4A3070FaaB4", "Fidenza by Tyler Hobbs"},
            {"0xe560d068e1f597970a7c129d1fea9fa1c9a877b0".ToLower(), "Ringers by Dmitri Cherniak"},
            {"0x524cab2ec69124574082676e6f654a18df49a048".ToLower(), "Lil Pudgys"},
            {"0xf4a4644e818c2843ba0aabea93af6c80b5984114".ToLower(), "Wrapped Cryptopunks"},
            {"0xa3aee8bce55beea1951ef834b99f3ac60d1abeeb".ToLower(), "VeeFriends"}
        };

        public static readonly List<string> _evmChains = new List<string>
        {
            "eth", "polygon", "bsc", "mint", "base", "optimism", "viction"
        };

        public static readonly List<string> _chains = new List<string>
        {
            "eth","polygon", "bsc", "avalanche", "mint", "cronos", "arbitrum", "zkSync-era", "mantle", "base", "optimism",
            "linea", "starknet", "sei", "gravity", "viction", "abstract", "tezos", "archway"
        };

        public static readonly Dictionary<string, string> _evmChainsToLinks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "eth", "https://restapi.nftscan.com/" },
            { "polygon", "https://polygonapi.nftscan.com/" },
            { "bsc", "https://bnbapi.nftscan.com/" },
            //{ "avalanche", "https://avaxapi.nftscan.com/" },
            //{ "fantom", "https://fantomapi.nftscan.com/" },
            { "mint", "https://mintapi.nftscan.com/" },
            //{ "cronos", "https://cronosapi.nftscan.com/" },
            //{ "arbitrum", "https://arbitrumapi.nftscan.com/" },
            //{ "zkSync-era", "https://zksyncapi.nftscan.com/" },
            //{ "mantle", "https://mantleapi.nftscan.com/" },
            { "base", "https://baseapi.nftscan.com/" },
            { "optimism", "https://optimismapi.nftscan.com/" },
            //{ "linea", "https://lineaapi.nftscan.com/" },
            //{ "moonbeam", "https://moonbeamapi.nftscan.com/" },
            //{ "starknet", "https://starknetapi.nftscan.com/" },
            //{ "scroll", "https://scrollapi.nftscan.com/" },
            //{ "blast", "https://blastapi.nftscan.com/" },
            //{ "sei", "https://seiapi.nftscan.com/" },
            //{ "gravity", "https://gravityapi.nftscan.com/" },
            { "viction", "https://victionapi.nftscan.com/" },
            //{ "bera", "https://beraapi.nftscan.com/" },
            //{ "platon", "https://platonapi.nftscan.com/" }
        };

        //DO not modify chains prices until wallet deletion is handled properly

        public static readonly Dictionary<string, decimal> _chainsToValue = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            { "eth", EthValue },
            { "ethereum", EthValue },
            { "weth", EthValue },
            { "polygon", 0.62M },
            { "matic", 0.62M },
            { "wmatic", 0.62M },
            { "bsc", 661.7M },
            { "avalanche", 42.42M },
            { "fantom", 1.23M },
            { "ftm", 1.23M },
            { "mint", 0.0006275M },
            { "cronos", 0.00010174M },
            { "cro", 0.00010174M },
            { "arbitrum", 0.886426M },
            { "zkSync-era", 0.168587M },
            { "mantle", 0.983M },
            { "mnt", 0.983M },
            { "base", 0.00000452M },
            { "optimism", 2.29M },
            { "linea", 0.99878M },
            { "moonbeam", 0.29401M },
            { "starknet", 0.604024M },
            { "scroll", 0.711560M },
            { "blast", 0.009696M },
            { "sei", 0.51M },
            { "gravity", 0.03123M },
            { "viction", 0.3831M },
            { "platon", 0.0078M },
            { "shs", 0.00000004276M },
            { "solana", SolanaValue },
            { "sol", SolanaValue },
            { "ton", TonValue },
            { "tezos", TezosValue },
            { "xtz", TezosValue },
            { "archway" , 0.99M },
            { "trsh", 0.0003625M },
            { "usdc", 0.9999M },
            { "usdt", 0.9999M },
            { "rari", 2.72M },
            { "sand", 0.7384M },
            { "bnb", 710.87M },
            { "busdt", 1.00M },
            { "busd", 1.00M },
            { "wbnb", 710.87M },
            { "xrp", 2.39M },
            { "stargaze", StargazeValue },
            { "stars", StargazeValue },
            { "abstract", EthValue },
            { "abs", EthValue },

        };

        //DO not modify chains prices until wallet deletion is handled properly

        public static readonly Dictionary<string, decimal> _chainsToValueFloor = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            { "eth", EthValue },
            { "ethereum", EthValue },
            { "weth", EthValue },
            { "polygon", 0.22M },
            { "matic", 0.22M },
            { "wmatic", 0.22M },
            { "bsc", 661.7M },
            { "avalanche", 42.42M },
            { "fantom", 1.23M },
            { "ftm", 1.23M },
            { "mint", 0.0006275M },
            { "cronos", 0.00010174M },
            { "cro", 0.00010174M },
            { "arbitrum", 0.886426M },
            { "zkSync-era", 0.168587M },
            { "mantle", 0.983M },
            { "mnt", 0.983M },
            { "base", EthValue },
            { "optimism", 2.29M },
            { "linea", 0.99878M },
            { "moonbeam", 0.29401M },
            { "starknet", 0.1550M },
            { "scroll", 0.711560M },
            { "blast", 0.009696M },
            { "sei", 0.51M },
            { "gravity", 0.03123M },
            { "viction", 0.3831M },
            { "platon", 0.0078M },
            { "shs", 0.00000004276M },
            { "solana", SolanaValue },
            { "sol", SolanaValue },
            { "ton", TonValue },
            { "tezos", TezosValue },
            { "xtz", TezosValue },
            { "archway" , 0.99M },
            { "trsh", 0.0003625M },
            { "usdc", 0.9999M },
            { "usdt", 0.9999M },
            { "rari", 2.72M },
            { "sand", 0.7384M },
            { "bnb", 710.87M },
            { "busdt", 1.00M },
            { "busd", 1.00M },
            { "wbnb", 710.87M },
            { "xrp", 2.39M },
            { "stargaze", StargazeValue },
            { "stars", StargazeValue },
            { "abstract", EthValue },
            { "abs", EthValue },

        };

        public static readonly List<string> _magicEdenSupportedChains = new List<string>
        {
            "ethereum", "base", "abstract", "arbitrum", "bsc", "polygon", "sei", "apechain", "bsc",
            "berachain"
        };

        public static readonly List<string> _mintifySupportedChains = new List<string>
        {
            "ethereum", "eth", "base", "abstract"
        };

        public static readonly Dictionary<string, string> _mintifyChainsToLinks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ethereum", "https://api.mintify.xyz/" },
            { "eth", "https://api.mintify.xyz/" },
            { "base", "https://api-base.mintify.xyz/" },
            { "abstract", "https://api-abstract.mintify.xyz/" }
        };

        public static readonly List<string> _alchemyChains = new List<string>
        {
            "eth","polygon", "avalanche", "arbitrum", "zkSync-era", "base", "optimism",
            "linea", "scroll", "blast", "rootstock", "abstract", "apechain",
            "shape", "soneium", "berachain"
        };

        public static readonly Dictionary<string, string> _alchemyChainsToLinks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "eth", "https://eth-mainnet.g.alchemy.com/" },
            { "ethereum", "https://eth-mainnet.g.alchemy.com/" },
            { "polygon", "https://polygon-mainnet.g.alchemy.com/" },
            { "avalanche", "https://avax-mainnet.g.alchemy.com/" },
            { "arbitrum", "https://arb-mainnet.g.alchemy.com/" },
            { "zkSync-era", "https://zksync-mainnet.g.alchemy.com/" },
            { "base", "https://base-mainnet.g.alchemy.com/" },
            { "optimism", "https://opt-mainnet.g.alchemy.com/" },
            { "linea", "https://linea-mainnet.g.alchemy.com/" },
            { "scroll", "https://scroll-mainnet.g.alchemy.com/" },
            { "blast", "https://blast-mainnet.g.alchemy.com/" },
            { "rootstock", "https://rootstock-mainnet.g.alchemy.com/" },
            { "abstract", "https://abstract-mainnet.g.alchemy.com/" },
            { "apechain", "https://apechain-mainnet.g.alchemy.com/" },
            { "shape", "https://shape-mainnet.g.alchemy.com/" },
            { "soneium", "https://soneium-mainnet.g.alchemy.com/" },
            { "berachain", "https://berachain-mainnet.g.alchemy.com/" },
        };

        public static readonly Dictionary<NotificationReference, string> _NotificationTitle = new Dictionary<NotificationReference, string>
        {
            {NotificationReference.Badge, "New Badge" },
            {NotificationReference.WalletOwnership, "Wallet Ownership Lost" }
        };
        public static readonly Dictionary<NotificationReference, string> _NotificationMessage = new Dictionary<NotificationReference, string>
        {
            {NotificationReference.Badge, "Congratulations - you just earned a new badge" },
            {NotificationReference.WalletOwnership, "Sorry, you've lost ownership to wallet address:" }
        };

    }
}
