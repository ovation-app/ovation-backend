using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Ovation.Application.Constants;
using Ovation.Application.DTOs.Stargaze;

namespace Ovation.Persistence.Services.Clients
{
    class GraphQLService(IHttpClientFactory factory)
    {
        public async Task<StargazeCollectionData> GetStargazeCollectionAsync(string collectionAddress)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                query Collection($address: String!) {
                    collection(address: $address) {
                        name
                        stats {
                            numOwners
                        }
                        contractAddress
                        categories {
                            public
                            private
                        }
                        contractUri
                        creationTime
                        creator {
                            name {
                                name
                                image
                                ownerAddr
                            }
                        }
                        tokenCounts {
                            total
                            listed
                            active
                        }
                        floor {
                          amount
                          amountUsd
                          denom
                          id
                          symbol
                          rate
                        }
                        floorPriceStars
                        website
                        royaltyInfo {
                          paymentAddress
                          sharePercent
                        }
                        mintStatus
                        media {
                          type
                          urls
                          fallbackUrl
                        }
                    }
                }",
                Variables = new { collectionAddress }
            };

            var httpClient = factory.CreateClient(Constant.StargazeQl);
            var client = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = httpClient.BaseAddress }, new SystemTextJsonSerializer(), httpClient);

            var response = await client.SendQueryAsync<StargazeCollectionData>(query);
            return response.Data;
        }
    }
}
