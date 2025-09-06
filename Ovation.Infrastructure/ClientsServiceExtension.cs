using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Ovation.Persistence.Common.Interfaces.Apis;
using Ovation.Persistence.Delegates;
using Ovation.Persistence.Services.Apis;
using Refit;

namespace Ovation.Persistence
{
    public static class ClientsServiceExtension
    {
        public static void ConfigureClients(this IServiceCollection services)
        {
            services.AddHttpClient(Constant.Archway, (client) =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("accept", "application/json");

                client.BaseAddress = new Uri($"https://api.mainnet.archway.io/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {      
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            services.AddHttpClient(Constant.Tezos, (client) =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("accept", "application/json");

                client.BaseAddress = new Uri($"https://api.tzkt.io/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            services.AddHttpClient(Constant.Stargaze, (client) =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("accept", "application/json");

                client.BaseAddress = new Uri("https://nft-api.mainnet.stargaze-apis.com/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            services.AddHttpClient(Constant.StargazeQl, (client) =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("accept", "application/json");

                client.BaseAddress = new Uri("https://graphql.mainnet.stargaze-apis.com/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            services.AddHttpClient(Constant.Ton, (client) =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("X-API-Key", Constant.NFTScanAPIKey);
                client.DefaultRequestHeaders.Add("accept", "application/json");

                client.BaseAddress = new Uri($"https://tonapi.nftscan.com/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            services.AddHttpClient(Constant.Solana, (client) =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("X-API-Key", Constant.NFTScanAPIKey);
                client.DefaultRequestHeaders.Add("accept", "application/json");

                client.BaseAddress = new Uri("https://solanaapi.nftscan.com/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            services.AddHttpClient(Constant.StargazeQl, (client) =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("accept", "application/json");

                client.BaseAddress = new Uri("https://graphql.mainnet.stargaze-apis.com/graphql");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            services.AddScoped<GraphQLHttpClient>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = factory.CreateClient(Constant.StargazeQl);
                return new GraphQLHttpClient(new GraphQLHttpClientOptions(), new SystemTextJsonSerializer(), httpClient);
            });


            //EVM chains Typed HttpClient nftscan

            foreach (var chain in Constant._evmChains)
            {
                if (Constant._evmChainsToLinks.TryGetValue(chain.ToLower(), out string baseUrl))
                {
                    if (!string.IsNullOrEmpty(baseUrl))
                    {
                        services.AddHttpClient(chain, (client) =>
                        {
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Add("X-API-Key", Constant.NFTScanAPIKey);
                            client.DefaultRequestHeaders.Add("accept", "application/json");

                            client.BaseAddress = new Uri(baseUrl);
                        })
                        .ConfigurePrimaryHttpMessageHandler(() =>
                        {
                            return new SocketsHttpHandler()
                            {
                                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                            };
                        })
                        .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
                    }                    
                }
            }

            //alchemy typed client
            foreach (var chain in Constant._alchemyChains)
            {
                if (Constant._alchemyChainsToLinks.TryGetValue(chain.ToLower(), out string baseUrl))
                {
                    if (!string.IsNullOrEmpty(baseUrl))
                    {
                        services.AddHttpClient($"{Constant.Alchemy}{chain}", (client) =>
                        {
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Add("X-API-Key", Constant.NFTScanAPIKey);
                            client.DefaultRequestHeaders.Add("accept", "application/json");

                            client.BaseAddress = new Uri(baseUrl);
                        })
                        .ConfigurePrimaryHttpMessageHandler(() =>
                        {
                            return new SocketsHttpHandler()
                            {
                                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                            };
                        })
                        .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
                    }
                }
            }


            services.AddRefitClient<IX>()
            .ConfigureHttpClient(client =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", Constant.XToken);
                client.DefaultRequestHeaders.Add("accept", "application/json");

                client.BaseAddress = new Uri("https://api.x.com/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            //services.AddHttpClient<IXTweet>()
            ////.AddHttpMessageHandler<XOAuthHandler>()
            //.ConfigureHttpClient(client =>
            //{
            //    client.BaseAddress = new Uri("https://api.x.com/");
            //})
            //.ConfigurePrimaryHttpMessageHandler(() =>
            //{
            //    return new SocketsHttpHandler()
            //    {
            //        PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            //    };
            //})
            //.SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            services.AddHttpClient(Constant.XOAuth, (client) =>
            {
                client.BaseAddress = new Uri("https://api.x.com/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            services.AddHttpClient(Constant.Abstract, (client) =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("accept", "application/json");

                client.BaseAddress = new Uri("https://abstract-mainnet.g.alchemy.com/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            // Magic Eden Typed Client
            //services.AddHttpClient(Constant.MagicEden, (client) =>
            //{
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add("accept", "application/json");
            //    client.DefaultRequestHeaders.Add("Authorization", Constant.MagicEdenApiKey);

            //    client.BaseAddress = new Uri("https://api-mainnet.magiceden.dev/");
            //})
            //.ConfigurePrimaryHttpMessageHandler(() =>
            //{
            //    return new SocketsHttpHandler()
            //    {
            //        PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            //    };
            //})
            //.SetHandlerLifetime(Timeout.InfiniteTimeSpan);


            services.AddRefitClient<IDappRadar>()
            .ConfigureHttpClient(c =>
            {
                c.DefaultRequestHeaders.Clear();
                c.BaseAddress = new Uri("https://apis.dappradar.com");
                c.DefaultRequestHeaders.Add("x-api-key", Constant.DappRadarKey);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);



            services.AddRefitClient<IMagicEden>()
            .ConfigureHttpClient(c =>
            {
                c.DefaultRequestHeaders.Clear();
                c.BaseAddress = new Uri("https://api-mainnet.magiceden.dev");
                c.DefaultRequestHeaders.Add("Authorization", Constant.MagicEdenApiKey);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);



            foreach (var chain in Constant._mintifySupportedChains)
            {
                if (Constant._mintifyChainsToLinks.TryGetValue(chain.ToLower(), out string baseUrl))
                {
                    if (!string.IsNullOrEmpty(baseUrl))
                    {
                        services.AddHttpClient($"{Constant.Mintify}{chain}", (client) =>
                        {
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Add("API-KEY", Constant.MintifyKey);
                            client.DefaultRequestHeaders.Add("accept", "application/json");

                            client.BaseAddress = new Uri(baseUrl);
                        })
                        .ConfigurePrimaryHttpMessageHandler(() =>
                        {
                            return new SocketsHttpHandler()
                            {
                                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                            };
                        })
                        .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
                    }
                }
            }
        }
    }
}
