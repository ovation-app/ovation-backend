# Ovation - Social NFT Portfolio Management Platform

## 🚀 Overview

Ovation is a comprehensive social NFT (Non-Fungible Token) and blockchain portfolio management platform built with ASP.NET Core 8.0. The platform enables users to manage their digital assets across multiple blockchain networks, connect with other collectors, discover new opportunities, and build their NFT portfolio with social features.

The platform serves as a centralized hub for NFT collectors, providing real-time portfolio tracking, social networking, gamification elements, and comprehensive analytics across multiple blockchain ecosystems.

## 🏗️ System Architecture

Ovation follows **Clean Architecture** principles with a clear separation of concerns across four main layers:

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                      │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              Ovation.WebAPI                        │   │
│  │  • RESTful API Controllers                        │   │
│  │  • SignalR Real-time Communication               │   │
│  │  • JWT Authentication                            │   │
│  │  • Swagger Documentation                         │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                        │
│  ┌─────────────────────────────────────────────────────┐   │
│  │            Ovation.Application                     │   │
│  │  • CQRS with MediatR                            │   │
│  │  • Business Logic Handlers                      │   │
│  │  • Request/Response DTOs                       │   │
│  │  • Validation & AutoMapper                     │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    DOMAIN LAYER                           │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              Ovation.Domain                       │   │
│  │  • Core Business Entities                        │   │
│  │  • Domain Rules & Interfaces                    │   │
│  │  • Value Objects                               │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 INFRASTRUCTURE LAYER                      │
│  ┌─────────────────────────────────────────────────────┐   │
│  │           Ovation.Infrastructure                  │   │
│  │  • Database Context (Entity Framework)           │   │
│  │  • Repository Implementations                   │   │
│  │  • External API Integrations                   │   │
│  │  • Background Services & Jobs                  │   │
│  │  • Authentication & Authorization              │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## 🎯 Core Features & Functionalities

### 1. **Multi-Blockchain NFT Management**
- **Supported Networks**: Ethereum, Solana, Tezos, TON, Archway, Stargaze, and EVM-compatible chains
- **NFT Portfolio Tracking**: Real-time portfolio valuation and analytics
- **Collection Management**: Browse and manage NFT collections across chains
- **Transaction History**: Comprehensive transaction tracking and analytics
- **Privacy Controls**: Granular NFT visibility settings

### 2. **Social Features & User Management**
- **User Profiles**: Complete profile management with social links and experiences
- **Follow System**: Follow/unfollow users with follower/following lists
- **Social Discovery**: Find and connect with other NFT collectors
- **User Verification**: Multiple verification types for authenticity
- **Privacy Management**: Control visibility of NFTs and profile information

### 3. **Authentication & Security**
- **Multi-Provider Login**: Google, X (Twitter), and traditional email/password
- **JWT Authentication**: Secure token-based authentication
- **Password Management**: Change password, forgot password with OTP
- **Account Verification**: Email verification with OTP codes
- **Claimable Profiles**: Special registration flow for invited users

### 4. **Discovery & Analytics**
- **NFT Rankings**: Top holders, highest valued NFTs, blue chip holders
- **Market Analytics**: Sales volume, trending collections, market insights
- **Creator Rankings**: Top creators and contributors
- **Net Worth Tracking**: User portfolio value calculations
- **Featured Profiles**: Curated featured user profiles

### 5. **Real-time Features**
- **SignalR Integration**: Live notifications and updates
- **Real-time Portfolio Updates**: Instant portfolio value changes
- **Live Notifications**: Real-time user activity notifications
- **WebSocket Support**: For live data updates

### 6. **Search & Discovery**
- **User Search**: Find users by username or profile information
- **NFT Search**: Search across NFT collections and individual tokens
- **Collection Search**: Discover new NFT collections
- **Advanced Filtering**: Pagination and filtering capabilities

### 7. **Gamification & Achievements**
- **Badge System**: Achievement and recognition system
- **Blue Chip Recognition**: Special recognition for premium NFT holders
- **User Tasks**: Progressive task completion system
- **Milestone Tracking**: User journey and progression tracking

## 🔧 Technical Architecture

### **Technology Stack**
- **Framework**: ASP.NET Core 8.0
- **Database**: MySQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens with custom filters
- **Real-time**: SignalR with WebSocket support
- **CQRS**: MediatR for command/query separation
- **Validation**: FluentValidation with pipeline behavior
- **Mapping**: AutoMapper for object mapping
- **Observability**: OpenTelemetry + Sentry integration
- **Background Jobs**: Quartz.NET scheduler
- **Containerization**: Docker support
- **Unique IDs**: ULID for unique identifier generation
- **Caching**: Distributed memory cache
- **JSON**: Newtonsoft.Json for serialization

### **Architecture Patterns**

#### 1. **Clean Architecture**
- **Dependency Inversion**: Interfaces in Application layer, implementations in Infrastructure
- **Separation of Concerns**: Clear boundaries between layers
- **Testability**: Each layer can be tested independently
- **Maintainability**: Easy to modify and extend

#### 2. **CQRS (Command Query Responsibility Segregation)**
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, Search, List)
- **MediatR**: Centralized request/response handling
- **Performance**: Optimized read/write operations

#### 3. **Repository Pattern**
- **Data Access Abstraction**: Clean separation from business logic
- **Unit of Work**: Transaction management
- **Testability**: Easy to mock for testing
- **Flexibility**: Easy to switch data sources

### **Multi-Blockchain Integration**

The system supports multiple blockchain networks through dedicated service implementations:

```
┌─────────────────────────────────────────────────────────────┐
│                Blockchain Services                        │
├─────────────────────────────────────────────────────────────┤
│  • EvmsService      (Ethereum, Polygon, Base, Optimism)  │
│  • SolanaService    (Solana blockchain)                 │
│  • TezosService     (Tezos/Objkt marketplace)          │
│  • TonService       (TON blockchain)                    │
│  • ArchwayService   (Archway blockchain)                │
│  • StargazeService  (Stargaze/Cosmos)                  │
│  • CollectionPriceService (Real-time pricing data)      │
│  • GraphQLService   (GraphQL API integration)           │
└─────────────────────────────────────────────────────────────┘
```

#### **External API Integrations**
- **NFTScan API**: Primary NFT data provider for EVM, Solana, and TON
- **Alchemy API**: Enhanced Ethereum NFT data and transactions
- **GraphQL Services**: For Stargaze and other GraphQL-based platforms
- **REST APIs**: Direct integrations with blockchain-specific APIs
- **Collection Price Services**: Real-time NFT collection pricing
- **Magic Eden**: Solana NFT marketplace integration
- **DappRadar**: DeFi and NFT analytics

### **Background Processing**

The system uses Quartz.NET for scheduled background jobs:

```
┌─────────────────────────────────────────────────────────────┐
│                Background Jobs                           │
├─────────────────────────────────────────────────────────────┤
│  • Data Synchronization Jobs                           │
│    - GetEvmsCollectionDataJob                          │
│    - GetSolanaCollectionDataJob                        │
│    - GetTezosCollectionDataJob                         │
│    - GetTonCollectionDataJob                           │
│    - GetStargazeCollectionDataJob                      │
├─────────────────────────────────────────────────────────────┤
│  • User-Related Jobs                                   │
│    - SyncXFollowersDataJob                             │
│    - GetUserXMetricJob                                 │
│    - HandleReferralJob                                 │
│    - HandleUserTaskJob                                 │
│    - HandleWalletOwnerJob                              │
├─────────────────────────────────────────────────────────────┤
│  • Data Maintenance Jobs                               │
│    - DeleteWalletDataJob                               │
│    - SyncUserNftIdJob                                  │
│    - GetUserNftCustodyDateJob                          │
├─────────────────────────────────────────────────────────────┤
│  • NFT Data Synchronization Jobs                      │
│    - SyncEvmNftDataJob                                │
│    - SyncSolanaNftDataJob                             │
└─────────────────────────────────────────────────────────────┘
```

## 🔄 Layer Interactions

### **Request Flow**
```
1. HTTP Request → WebAPI Controller
2. Controller → MediatR Command/Query
3. MediatR → Application Handler
4. Handler → Repository Interface
5. Repository → Database/External API
6. Response flows back through layers
```

### **Data Flow**
```
┌─────────────┐    ┌──────────────┐    ┌─────────────┐
│   WebAPI    │───▶│ Application  │───▶│ Domain      │
│  (DTOs)     │    │  (Handlers)  │    │ (Entities)  │
└─────────────┘    └──────────────┘    └─────────────┘
       │                   │                   │
       ▼                   ▼                   ▼
┌─────────────┐    ┌──────────────┐    ┌─────────────┐
│ Controllers │    │  MediatR     │    │ Repositories│
│ SignalR     │    │  Validation  │    │ Services    │
└─────────────┘    └──────────────┘    └─────────────┘
```

## 🛡️ Security & Performance

### **Security Features**
- **JWT Authentication**: Secure token-based authentication
- **Password Hashing**: Secure password storage
- **CORS Configuration**: Cross-origin resource sharing
- **Input Validation**: Comprehensive request validation
- **API Key Management**: Secure external service authentication

### **Performance Optimizations**
- **Connection Pooling**: Optimized HTTP client configurations
- **Caching**: Strategic caching for frequently accessed data
- **Background Processing**: Heavy operations moved to background jobs
- **Pagination**: Efficient data loading with cursor-based pagination
- **Async/Await**: Non-blocking operations throughout

### **Monitoring & Observability**
- **Sentry Integration**: Error tracking and performance monitoring
- **OpenTelemetry**: Distributed tracing and metrics
- **Custom Logging**: Structured logging with custom enrichers
- **Database Monitoring**: Entity Framework query optimization tracking

## 🚀 Deployment & Configuration

### **Environment Variables**
```bash
OVATION_DB=mysql_connection_string
OVATION_KEY=jwt_secret_key
PORT=8080
SENTRY_DNS=sentry_dsn
NFTSCAN_API_KEY=nftscan_api_key
ALCHEMY_API_KEY=alchemy_api_key
```

### **Docker Support**
```bash
# Build and run with Docker
docker build -t ovation-app .
docker run -p 8080:8080 ovation-app
```

### **Development Setup**
```bash
# Restore dependencies
dotnet restore

# Run the application
dotnet run --project Ovation.WebAPI

# Access Swagger documentation
http://localhost:8080/swagger
```

## 📊 Database Schema

The system manages a comprehensive schema including:

- **User Management**: Users, profiles, social connections, experiences, verification
- **NFT Data**: Collections, individual NFTs, transactions, activities, records
- **Portfolio Tracking**: Wallet values, sales records, portfolio metrics
- **Gamification**: Badges, achievements, user tasks, milestones
- **Analytics**: User stats, experience tracking, referral data
- **Social Features**: Followers, notifications, feedback, featured items
- **External Data**: NFTScan data, DappRadar collections, Archway collections
- **Developer Tools**: Developer tokens, chain rates, target accounts

> **⚠️ Deprecated Entities**: The `UserNft` and `UserNftCollection` entities are deprecated. Use `UserNftDatum` and `UserNftCollectionDatum` instead for new development.

## 🔗 API Documentation

The API provides comprehensive RESTful endpoints for:
- **Authentication**: Login, registration, password management, verification
- **User Management**: Profile CRUD, social features, experiences
- **NFT Management**: Portfolio, collections, transactions, favorites
- **Discovery**: Search, rankings, analytics, marketplace data
- **Real-time**: SignalR hub for live updates and notifications
- **Developer Tools**: Token filtering, core endpoints
- **Webhooks**: External system integration

Access the interactive API documentation at `/swagger` when running the application.

## 🎯 Key Benefits

1. **Multi-Chain Support**: Unified experience across multiple blockchains
2. **Social Integration**: Connect with other NFT collectors and build communities
3. **Real-time Updates**: Live portfolio and market data with SignalR
4. **Scalable Architecture**: Clean architecture for easy maintenance and extension
5. **Comprehensive Analytics**: Detailed portfolio and market insights
6. **Security First**: Robust authentication and authorization with custom filters
7. **Developer Friendly**: Well-documented APIs and clean code structure
8. **Gamification**: Achievement system with badges and milestones
9. **Background Processing**: Automated data synchronization and maintenance
10. **Observability**: Comprehensive monitoring with Sentry and OpenTelemetry

## 🤝 Contributing

The Ovation platform is built with clean architecture principles, making it easy to:
- Add new blockchain networks and integrations
- Extend social features and community functionality
- Implement new analytics and reporting features
- Add custom integrations and webhooks
- Modify business logic and domain rules
- Scale horizontally with microservices
- Add new gamification elements and achievements

Each layer has clear responsibilities and interfaces, ensuring maintainable and testable code. The CQRS pattern with MediatR provides excellent separation of concerns and makes the codebase highly extensible.

---

**Ovation** - Empowering NFT collectors to build, discover, and connect in the digital asset ecosystem.
