using Ovation.Application.DTOs.Apis;
using Refit;

namespace Ovation.Persistence.Services.Apis
{
    interface IMagicEden
    {
        [Get("/v3/rtp/{chain}/collections/v7")]
        Task<MagicEdenCollectionData> GetCollectionsAsync([AliasAs("chain")] string userId,
            [AliasAs("id")] string contract, [AliasAs("limit")] int limit);
    }
}
