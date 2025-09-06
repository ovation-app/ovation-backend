using Ovation.Application.DTOs.Apis;
using Refit;

namespace Ovation.Persistence.Common.Interfaces.Apis
{
    internal interface IDappRadar
    {
        [Get("/v2/nfts/collections")]
        Task<DappRadarGetCollections> GetCollectionsAsync([AliasAs("resultsPerPage")] int limit,
            [AliasAs("page")] int page = 1);
    }
}
