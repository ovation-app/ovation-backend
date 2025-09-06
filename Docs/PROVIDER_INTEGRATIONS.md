# Provider Integrations Documentation

## 📋 Overview

The Ovation Backend integrates with multiple external providers to deliver comprehensive NFT portfolio management and blockchain data services. This document provides detailed information about each provider integration, including configuration, authentication, data formats, and best practices.

## 🔗 Supported Providers

### Primary NFT Data Providers

#### 1. NFTScan API
**Purpose**: Primary NFT data provider for multi-chain NFT information
**Supported Chains**: Ethereum, Polygon, Base, Optimism, Solana, TON
**Documentation**: [NFTScan API Docs](https://docs.nftscan.com/)

##### Configuration
```bash
# Environment Variable
NFTSCAN_KEY=your-nftscan-api-key
```

##### Authentication
```csharp
// HTTP Client Configuration
services.AddRefitClient<INFTScan>()
    .ConfigureHttpClient(c =>
    {
        c.DefaultRequestHeaders.Add("X-API-KEY", Constant.NFTScanAPIKey);
        c.BaseAddress = new Uri("https://restapi.nftscan.com");
    });
```

##### Data Formats
```json
// Collection Data
{
  "contract_address": "0x1234567890abcdef",
  "name": "Collection Name",
  "symbol": "SYMBOL",
  "description": "Collection Description",
  "image_url": "https://example.com/image.jpg",
  "total_supply": 10000,
  "floor_price": 0.5,
  "volume_traded": 100.0,
  "owners_count": 2500
}

// NFT Token Data
{
  "token_id": "12345",
  "contract_address": "0x1234567890abcdef",
  "name": "NFT Name",
  "description": "NFT Description",
  "image_url": "https://example.com/nft.jpg",
  "owner": "0x9876543210fedcba",
  "attributes": [
    {
      "trait_type": "Background",
      "value": "Blue"
    }
  ]
}
```

##### Rate Limits
- **Free Tier**: 100 requests/minute
- **Pro Tier**: 1000 requests/minute
- **Enterprise**: Custom limits

##### Error Handling
```csharp
try
{
    var response = await _nftScanService.GetCollectionAsync(contractAddress);
    return response;
}
catch (HttpRequestException ex) when (ex.Message.Contains("429"))
{
    // Rate limit exceeded
    throw new RateLimitExceededException("NFTScan rate limit exceeded");
}
catch (HttpRequestException ex) when (ex.Message.Contains("401"))
{
    // Invalid API key
    throw new UnauthorizedException("Invalid NFTScan API key");
}
```

#### 2. Alchemy API
**Purpose**: Enhanced Ethereum blockchain data and NFT information
**Supported Chains**: Ethereum, Polygon, Base, Optimism
**Documentation**: [Alchemy API Docs](https://docs.alchemy.com/)

##### Configuration
```bash
# Environment Variable
ALCHEMY_KEY=your-alchemy-api-key
```

##### Authentication
```csharp
services.AddRefitClient<IAlchemy>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri($"https://eth-mainnet.g.alchemy.com/nft/v3/{Constant.AlchemyKey}");
    });
```

##### Data Formats
```json
// NFT Metadata
{
  "contract": {
    "address": "0x1234567890abcdef"
  },
  "id": {
    "tokenId": "12345"
  },
  "title": "NFT Title",
  "description": "NFT Description",
  "tokenUri": "https://example.com/metadata.json",
  "media": [
    {
      "uri": "https://example.com/image.jpg",
      "mimeType": "image/jpeg"
    }
  ],
  "metadata": {
    "attributes": [
      {
        "trait_type": "Background",
        "value": "Blue"
      }
    ]
  }
}
```

##### Rate Limits
- **Free Tier**: 300 requests/minute
- **Growth Tier**: 1000 requests/minute
- **Scale Tier**: 5000 requests/minute

#### 3. Magic Eden API
**Purpose**: Solana NFT marketplace data and collection information
**Supported Chains**: Solana
**Documentation**: [Magic Eden API Docs](https://docs.magiceden.io/)

##### Configuration
```bash
# Environment Variable
MAGICEDEN_KEY=your-magiceden-api-key
```

##### Authentication
```csharp
services.AddRefitClient<IMagicEden>()
    .ConfigureHttpClient(c =>
    {
        c.DefaultRequestHeaders.Add("Authorization", Constant.MagicEdenApiKey);
        c.BaseAddress = new Uri("https://api-mainnet.magiceden.dev");
    });
```

##### Data Formats
```json
// Solana Collection Data
{
  "symbol": "COLLECTION_SYMBOL",
  "name": "Collection Name",
  "description": "Collection Description",
  "image": "https://example.com/image.jpg",
  "twitter": "https://twitter.com/collection",
  "discord": "https://discord.gg/collection",
  "website": "https://collection.com",
  "floorPrice": 0.5,
  "volume24h": 100.0,
  "volume7d": 500.0,
  "volume30d": 2000.0,
  "marketCap": 5000.0
}
```

##### Rate Limits
- **Free Tier**: 100 requests/minute
- **Pro Tier**: 1000 requests/minute

### Analytics Providers

#### 4. DappRadar API
**Purpose**: DeFi and NFT analytics, market insights
**Documentation**: [DappRadar API Docs](https://docs.dappradar.com/)

##### Configuration
```bash
# Environment Variable
DAPP_RADAR_KEY=your-dappradar-api-key
```

##### Authentication
```csharp
services.AddRefitClient<IDappRadar>()
    .ConfigureHttpClient(c =>
    {
        c.DefaultRequestHeaders.Add("x-api-key", Constant.DappRadarKey);
        c.BaseAddress = new Uri("https://apis.dappradar.com");
    });
```

##### Data Formats
```json
// Collection Analytics
{
  "collection_id": "12345",
  "name": "Collection Name",
  "floor_price": 0.5,
  "volume_24h": 100.0,
  "volume_7d": 500.0,
  "volume_30d": 2000.0,
  "market_cap": 5000.0,
  "owners_count": 2500,
  "total_supply": 10000,
  "rarity_score": 85.5
}
```

#### 5. Mintify API
**Purpose**: Multi-chain NFT marketplace data
**Supported Chains**: Ethereum, Polygon, BSC, Avalanche
**Documentation**: [Mintify API Docs](https://docs.mintify.xyz/)

##### Configuration
```bash
# Environment Variable
MINTIFY_KEY=your-mintify-api-key
```

##### Authentication
```csharp
foreach (var chain in Constant._mintifySupportedChains)
{
    services.AddHttpClient($"{Constant.Mintify}{chain}", client =>
    {
        client.DefaultRequestHeaders.Add("API-KEY", Constant.MintifyKey);
        client.BaseAddress = new Uri(Constant._mintifyChainsToLinks[chain]);
    });
}
```

### Social Media Providers

#### 6. X (Twitter) API
**Purpose**: Social media integration and user metrics
**Documentation**: [X API Docs](https://developer.twitter.com/en/docs)

##### Configuration
```bash
# Environment Variables
X_CONSUMER_KEY=your-x-consumer-key
X_CONSUMER_SECRET=your-x-consumer-secret
X_ACCESS_TOKEN=your-x-access-token
X_ACCESS_TOKEN_SECRET=your-x-access-token-secret
X_KEY=your-x-bearer-token
```

##### Authentication
```csharp
public class XService
{
    private readonly string _consumerKey = Environment.GetEnvironmentVariable("X_CONSUMER_KEY");
    private readonly string _consumerSecret = Environment.GetEnvironmentVariable("X_CONSUMER_SECRET");
    private readonly string _accessToken = Environment.GetEnvironmentVariable("X_ACCESS_TOKEN");
    private readonly string _accessTokenSecret = Environment.GetEnvironmentVariable("X_ACCESS_TOKEN_SECRET");
    private readonly string _bearerToken = Environment.GetEnvironmentVariable("X_KEY");
}
```

##### Data Formats
```json
// User Profile Data
{
  "id": "1234567890",
  "username": "username",
  "name": "Display Name",
  "description": "User bio",
  "profile_image_url": "https://example.com/profile.jpg",
  "public_metrics": {
    "followers_count": 1000,
    "following_count": 500,
    "tweet_count": 2000,
    "listed_count": 50
  },
  "verified": true
}
```

##### Rate Limits
- **Essential**: 300 requests/15 minutes
- **Elevated**: 1500 requests/15 minutes
- **Academic Research**: 300 requests/15 minutes

### AI Services

#### 7. Google AI Model API
**Purpose**: Content generation and analysis
**Documentation**: [Google AI Docs](https://ai.google.dev/docs)

##### Configuration
```bash
# Environment Variables
GOOGLE_AI_MODEL_API_KEY=your-google-ai-key
GOOGLE_AI_MODEL=gemini-pro
```

##### Authentication
```csharp
services.AddSingleton<IGenerativeModel>(provider =>
{
    var apiKey = Environment.GetEnvironmentVariable("GOOGLE_AI_MODEL_API_KEY");
    var model = Environment.GetEnvironmentVariable("GOOGLE_AI_MODEL");
    return new GenerativeModel(apiKey, model);
});
```

##### Data Formats
```json
// AI Response
{
  "candidates": [
    {
      "content": {
        "parts": [
          {
            "text": "Generated content response"
          }
        ]
      },
      "finishReason": "STOP",
      "safetyRatings": [
        {
          "category": "HARM_CATEGORY_HARASSMENT",
          "probability": "NEGLIGIBLE"
        }
      ]
    }
  ]
}
```

## 🔧 Provider Configuration

### HTTP Client Configuration

#### Connection Pooling
```csharp
public static class ClientsServiceExtension
{
    public static IServiceCollection ConfigureClients(this IServiceCollection services)
    {
        services.AddRefitClient<INFTScan>()
            .ConfigureHttpClient(c =>
            {
                c.DefaultRequestHeaders.Add("X-API-KEY", Constant.NFTScanAPIKey);
                c.BaseAddress = new Uri("https://restapi.nftscan.com");
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
```

#### Retry Policies
```csharp
services.AddHttpClient<INFTScanService>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                Console.WriteLine($"Retry {retryCount} after {timespan} seconds");
            });
}

private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30));
}
```

## 🛡️ Error Handling

### Provider-Specific Error Handling

#### Rate Limit Handling
```csharp
public class ProviderErrorHandler
{
    public async Task<T> HandleWithRetry<T>(Func<Task<T>> operation, string providerName)
    {
        try
        {
            return await operation();
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("429"))
        {
            // Rate limit exceeded
            await Task.Delay(TimeSpan.FromMinutes(1));
            return await operation();
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("401"))
        {
            throw new UnauthorizedException($"Invalid API key for {providerName}");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("403"))
        {
            throw new ForbiddenException($"Access forbidden for {providerName}");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            throw new NotFoundException($"Resource not found in {providerName}");
        }
        catch (Exception ex)
        {
            throw new ProviderException($"Error calling {providerName}: {ex.Message}", ex);
        }
    }
}
```

#### Circuit Breaker Pattern
```csharp
public class ProviderCircuitBreaker
{
    private readonly Dictionary<string, CircuitBreaker> _circuitBreakers = new();

    public async Task<T> ExecuteAsync<T>(string providerName, Func<Task<T>> operation)
    {
        if (!_circuitBreakers.ContainsKey(providerName))
        {
            _circuitBreakers[providerName] = new CircuitBreaker(
                failureThreshold: 5,
                timeout: TimeSpan.FromSeconds(30));
        }

        return await _circuitBreakers[providerName].ExecuteAsync(operation);
    }
}
```

## 📊 Data Synchronization

### Background Job Processing

#### NFTScan Data Sync Job
```csharp
[DisallowConcurrentExecution]
public class GetEvmsCollectionDataJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var collections = await _nftScanService.GetCollectionsAsync();
            await _repository.BulkInsertCollectionsAsync(collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing EVM collections");
            throw;
        }
    }
}
```

#### Solana Data Sync Job
```csharp
[DisallowConcurrentExecution]
public class GetSolanaCollectionDataJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var collections = await _magicEdenService.GetCollectionsAsync();
            await _repository.BulkInsertSolanaCollectionsAsync(collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing Solana collections");
            throw;
        }
    }
}
```

## 🧪 Testing with Mock Data

### Provider Simulators

#### NFTScan Mock Service
```csharp
public class MockNFTScanService : INFTScanService
{
    public async Task<CollectionData> GetCollectionAsync(string contractAddress)
    {
        await Task.Delay(100); // Simulate network delay
        
        return new CollectionData
        {
            ContractAddress = contractAddress,
            Name = "Mock Collection",
            Symbol = "MOCK",
            Description = "Mock collection for testing",
            ImageUrl = "https://example.com/mock-image.jpg",
            TotalSupply = 10000,
            FloorPrice = 0.5m,
            VolumeTraded = 100.0m,
            OwnersCount = 2500
        };
    }
}
```

#### Alchemy Mock Service
```csharp
public class MockAlchemyService : IAlchemyService
{
    public async Task<NFTMetadata> GetNFTMetadataAsync(string contractAddress, string tokenId)
    {
        await Task.Delay(100);
        
        return new NFTMetadata
        {
            Contract = new Contract { Address = contractAddress },
            Id = new TokenId { TokenId = tokenId },
            Title = "Mock NFT",
            Description = "Mock NFT for testing",
            TokenUri = "https://example.com/metadata.json",
            Media = new[]
            {
                new Media
                {
                    Uri = "https://example.com/nft.jpg",
                    MimeType = "image/jpeg"
                }
            }
        };
    }
}
```

### Test Configuration
```csharp
public class ProviderTestConfiguration
{
    public static IServiceCollection ConfigureMockProviders(this IServiceCollection services)
    {
        services.AddScoped<INFTScanService, MockNFTScanService>();
        services.AddScoped<IAlchemyService, MockAlchemyService>();
        services.AddScoped<IMagicEdenService, MockMagicEdenService>();
        
        return services;
    }
}
```

## 📈 Monitoring and Observability

### Provider Health Checks
```csharp
public class ProviderHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, object>();
        
        // Check NFTScan
        try
        {
            await _nftScanService.GetHealthAsync();
            results["NFTScan"] = "Healthy";
        }
        catch (Exception ex)
        {
            results["NFTScan"] = $"Unhealthy: {ex.Message}";
        }
        
        // Check Alchemy
        try
        {
            await _alchemyService.GetHealthAsync();
            results["Alchemy"] = "Healthy";
        }
        catch (Exception ex)
        {
            results["Alchemy"] = $"Unhealthy: {ex.Message}";
        }
        
        var isHealthy = results.Values.All(v => v.ToString().Contains("Healthy"));
        
        return isHealthy 
            ? HealthCheckResult.Healthy("All providers healthy", results)
            : HealthCheckResult.Unhealthy("Some providers unhealthy", null, results);
    }
}
```

### Provider Metrics
```csharp
public class ProviderMetrics
{
    private readonly Counter _nftScanRequests;
    private readonly Counter _alchemyRequests;
    private readonly Histogram _responseTime;
    
    public ProviderMetrics()
    {
        _nftScanRequests = Metrics.CreateCounter("nftscan_requests_total", "Total NFTScan requests");
        _alchemyRequests = Metrics.CreateCounter("alchemy_requests_total", "Total Alchemy requests");
        _responseTime = Metrics.CreateHistogram("provider_response_time_seconds", "Provider response time");
    }
    
    public void RecordNFTScanRequest()
    {
        _nftScanRequests.Inc();
    }
    
    public void RecordAlchemyRequest()
    {
        _alchemyRequests.Inc();
    }
    
    public void RecordResponseTime(double seconds)
    {
        _responseTime.Observe(seconds);
    }
}
```

## 🔒 Security Considerations

### API Key Management
```csharp
public class SecureApiKeyManager
{
    private readonly IConfiguration _configuration;
    
    public string GetApiKey(string providerName)
    {
        var key = _configuration[$"{providerName.ToUpper()}_KEY"];
        
        if (string.IsNullOrEmpty(key))
        {
            throw new ConfigurationException($"API key for {providerName} not configured");
        }
        
        return key;
    }
}
```

### Request Signing
```csharp
public class RequestSigner
{
    public string SignRequest(string method, string uri, string body, string secret)
    {
        var message = $"{method}{uri}{body}";
        var signature = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(message));
        return Convert.ToBase64String(signature);
    }
}
```

## 📚 Best Practices

### 1. Rate Limiting
- Implement exponential backoff for rate limit errors
- Use connection pooling to reduce overhead
- Cache responses when appropriate
- Monitor usage against provider limits

### 2. Error Handling
- Implement circuit breaker patterns
- Use retry policies with jitter
- Log all provider errors for debugging
- Provide fallback mechanisms

### 3. Performance
- Use async/await throughout
- Implement proper connection pooling
- Cache frequently accessed data
- Use background jobs for heavy operations

### 4. Security
- Store API keys securely
- Use HTTPS for all communications
- Implement request signing where required
- Monitor for suspicious activity

### 5. Monitoring
- Track provider health and availability
- Monitor response times and error rates
- Set up alerts for critical failures
- Use distributed tracing for debugging

## 🚀 Future Enhancements

### Planned Integrations
- **OpenSea API**: Additional NFT marketplace data
- **CoinGecko API**: Cryptocurrency price data
- **The Graph**: Decentralized indexing
- **IPFS**: Decentralized storage
- **ENS**: Ethereum Name Service

### Provider Expansion
- **Arbitrum**: Layer 2 scaling solution
- **Avalanche**: High-performance blockchain
- **Fantom**: Fast and secure blockchain
- **BNB Chain**: Binance Smart Chain

---

**This documentation provides comprehensive guidance for working with external providers in the Ovation Backend. For specific implementation details, refer to the source code and individual provider documentation.**
