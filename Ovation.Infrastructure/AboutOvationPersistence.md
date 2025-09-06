# Ovation Infrastructure Layer

## Overview

The Ovation.Infrastructure layer serves as the persistence and external service integration layer of the Ovation application. It implements the Clean Architecture pattern by providing concrete implementations of interfaces defined in the Application layer, handling data persistence, external API integrations, authentication, and background processing.

## Core Features & Functionalities

### 1. Data Persistence
- **Database Context**: `OvationDbContext` manages Entity Framework Core configuration for MySQL database
- **Repository Pattern**: Implements repository interfaces for all domain entities including:
  - User management (UserRepository, ProfileRepository)
  - NFT and wallet data (AssetRepository, WalletRepository)
  - Social features (FollowRepository, NotificationRepository)
  - Business logic (BadgeRepository, DiscoverRepository, SearchRepository)
  - Content management (NewsletterRepository, FeedbackRepository)
- **Unit of Work**: Centralized transaction management through `UnitOfWork`

### 2. Multi-Blockchain NFT Integration
The infrastructure supports multiple blockchain networks through dedicated service implementations:

#### Supported Blockchains:
- **Ethereum/EVM Chains**: `EvmsService` for ERC-721/ERC-1155 tokens
- **Solana**: `SolanaService` for Solana NFT collections
- **Tezos**: `TezosService` for Tezos NFT marketplace (Objkt)
- **TON**: `TonService` for TON blockchain NFTs
- **Stargaze**: `StargazeService` for Cosmos-based NFT platform
- **Archway**: `ArchwayService` for Archway blockchain

#### External API Integrations:
- **NFTScan API**: Primary NFT data provider for EVM, Solana, and TON chains
- **GraphQL Services**: For Stargaze and other GraphQL-based platforms
- **REST APIs**: Direct integrations with blockchain-specific APIs
- **Collection Price Services**: Real-time NFT collection pricing data

### 3. Authentication & User Management
- **AuthManager**: Central authentication service
- **PasswordHasher**: Secure password hashing and verification
- **UserManager**: User account management operations
- **JWT Token Handling**: Token-based authentication system

### 4. Background Processing & Scheduled Jobs
Powered by Quartz.NET scheduler with the following job types:

#### Data Synchronization Jobs:
- `GetEvmsCollectionDataJob`: Syncs EVM NFT collection data
- `GetSolanaCollectionDataJob`: Syncs Solana NFT collection data
- `GetTezosCollectionDataJob`: Syncs Tezos NFT collection data
- `GetTonCollectionDataJob`: Syncs TON NFT collection data
- `GetStargazeCollectionDataJob`: Syncs Stargaze NFT collection data

#### User-Related Jobs:
- `SyncXFollowersDataJob`: Synchronizes X (Twitter) follower data
- `GetUserXMetricJob`: Fetches user X metrics
- `HandleReferralJob`: Processes user referral logic
- `HandleUserTaskJob`: Manages user task completion
- `HandleWalletOwnerJob`: Processes wallet ownership changes
- `NftPrivacyChangedDataJob`: Handles NFT privacy setting changes

#### Data Maintenance Jobs:
- `DeleteWalletDataJob`: Cleanup of wallet data
- `SyncUserNftIdJob`: Synchronizes user NFT identifiers
- `GetUserNftCustodyDateJob`: Tracks NFT custody dates

### 5. Real-time Communication
- **SignalR Integration**: Real-time notifications and updates
- **NotificationService**: Centralized notification management
- **WebSocket Support**: For live data updates

### 6. Observability & Monitoring
- **Sentry Integration**: Error tracking and performance monitoring
- **OpenTelemetry**: Distributed tracing and metrics collection
- **Custom Logging**: Structured logging with custom enrichers
- **Entity Framework Instrumentation**: Database query monitoring

## Technical Architecture

### Dependency Injection Configuration
The `ServiceExtensions.ConfigurePersistence()` method registers all infrastructure services:

```csharp
// Database and Repository Registration
services.AddDbContext<OvationDbContext>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IUserRepository, UserRepository>();
// ... other repositories

// External Service Registration
services.AddTransient<EvmsService>();
services.AddTransient<SolanaService>();
services.AddTransient<TezosService>();
// ... other blockchain services

// Background Job Registration
services.AddQuartz(configure => {
    // Job configurations
});
```

### HTTP Client Management
The `ClientsServiceExtension.ConfigureClients()` method configures named HTTP clients for each blockchain API with:
- Optimized connection pooling (15-minute lifetime)
- Proper headers and authentication
- Circuit breaker patterns for resilience

### Database Schema
The `OvationDbContext` manages a comprehensive schema including:
- **User Management**: Users, profiles, social connections
- **NFT Data**: Collections, individual NFTs, transactions
- **Portfolio Tracking**: Wallet values, sales records, portfolio metrics
- **Gamification**: Badges, achievements, user tasks
- **Analytics**: User stats, experience tracking, referral data

## Key Technical Aspects

### 1. Multi-Tenant Architecture
- Support for multiple blockchain networks
- Unified data model across different chains
- Chain-specific service implementations

### 2. Scalability Features
- Background job processing for heavy operations
- Connection pooling for external APIs
- Asynchronous data synchronization
- Caching strategies for frequently accessed data

### 3. Security Considerations
- Secure password hashing
- JWT token management
- API key management for external services
- Data privacy controls

### 4. Monitoring & Observability
- Comprehensive error tracking with Sentry
- Performance monitoring with OpenTelemetry
- Database query optimization tracking
- Background job execution monitoring

### 5. Data Consistency
- Unit of Work pattern for transaction management
- Optimistic concurrency control
- Data validation at the persistence layer
- Audit trail capabilities

## External Dependencies

### NuGet Packages
- **Entity Framework Core**: ORM for MySQL database
- **Quartz.NET**: Background job scheduling
- **SignalR**: Real-time communication
- **Refit**: HTTP client for API consumption
- **GraphQL.Client**: GraphQL API integration
- **Sentry**: Error tracking and monitoring
- **OpenTelemetry**: Observability framework
- **MailKit**: Email functionality
- **Ulid**: Unique identifier generation

### Database
- **MySQL**: Primary database with Entity Framework Core
- **Connection String**: Environment variable `OVATION_DB`

### External APIs
- **NFTScan**: Multi-chain NFT data provider
- **Blockchain APIs**: Direct integrations with each supported blockchain
- **GraphQL Endpoints**: For platforms like Stargaze

## Performance Considerations

1. **Connection Pooling**: Optimized HTTP client configurations
2. **Background Processing**: Heavy operations moved to background jobs
3. **Caching**: Strategic caching for frequently accessed data
4. **Database Optimization**: Indexed queries and optimized Entity Framework usage
5. **Async/Await**: Non-blocking operations throughout the infrastructure

This infrastructure layer provides a robust, scalable foundation for the Ovation application, supporting complex multi-blockchain NFT operations while maintaining high performance and reliability standards.
