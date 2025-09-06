# Ovation Backend Architecture Documentation

## 📋 Overview

The Ovation backend follows **Clean Architecture** principles with a clear separation of concerns across four main layers. This architecture ensures maintainability, testability, and scalability while supporting multi-blockchain NFT portfolio management and social features.

## 🏗️ System Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                      │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              Ovation.WebAPI                        │   │
│  │  • RESTful API Controllers                        │   │
│  │  • SignalR Real-time Communication               │   │
│  │  • JWT Authentication                            │   │
│  │  • Swagger Documentation                         │   │
│  │  • CORS Configuration                           │   │
│  │  • Error Handling & Validation                 │   │
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
│  │  • Feature Modules (17 modules)                │   │
│  │  • Pipeline Behaviors                          │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    DOMAIN LAYER                           │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              Ovation.Domain                       │   │
│  │  • Core Business Entities (49 entities)         │   │
│  │  • Domain Rules & Interfaces                    │   │
│  │  • Value Objects                               │   │
│  │  • Domain Events                               │   │
│  │  • Business Logic                              │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 INFRASTRUCTURE LAYER                      │
│  ┌─────────────────────────────────────────────────────┐   │
│  │           Ovation.Infrastructure                  │   │
│  │  • Database Context (Entity Framework)           │   │
│  │  • Repository Implementations (20 repos)        │   │
│  │  • External API Integrations (7 services)       │   │
│  │  • Background Services & Jobs (15+ jobs)       │   │
│  │  • Authentication & Authorization              │   │
│  │  • SignalR Hubs & Services                      │   │
│  │  • Observability & Monitoring                   │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## 🔄 Request Flow Architecture

### HTTP Request Processing Flow

```
1. HTTP Request → WebAPI Controller
   ↓
2. Controller → MediatR Command/Query
   ↓
3. MediatR → ValidationBehavior (FluentValidation)
   ↓
4. ValidationBehavior → Application Handler
   ↓
5. Handler → Repository Interface
   ↓
6. Repository → Database/External API
   ↓
7. Response flows back through layers
```

### Data Flow Architecture

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
│ Filters     │    │  AutoMapper  │    │ External    │
└─────────────┘    └──────────────┘    └─────────────┘
```

## 🎯 Feature Modules Architecture

### Application Layer Feature Organization

```
Ovation.Application/
├── Features/
│   ├── AuthFeatures/           # Authentication & Authorization
│   ├── ProfileFeatures/        # User Profile Management
│   ├── AssetFeatures/          # NFT & Asset Management
│   ├── DiscoverFeatures/       # Discovery & Analytics
│   ├── FollowFeatures/         # Social Features
│   ├── NotificationFeatures/    # Real-time Notifications
│   ├── SearchFeatures/         # Search Functionality
│   ├── WalletFeatures/         # Wallet Management
│   ├── BadgeFeatures/          # Gamification System
│   ├── HomeFeatures/           # Home Feed
│   ├── FeedbackFeatures/        # User Feedback
│   ├── NewsletterFeatures/      # Email Communication
│   ├── PathFeatures/            # User Journey
│   ├── WebhookFeatures/         # External Integration
│   ├── DevTokenFeatures/         # Developer Tools
│   ├── AffilationFeatures/      # Referral System
│   └── MarketplaceFeatures/      # Marketplace Integration
├── DTOs/                        # Data Transfer Objects
├── Common/                      # Shared Components
└── Constants/                   # Application Constants
```

## 🔗 Multi-Blockchain Integration Architecture

### Blockchain Services Layer

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

### External API Integration Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                External API Integrations                  │
├─────────────────────────────────────────────────────────────┤
│  • NFTScan API      (Primary NFT data provider)          │
│  • Alchemy API      (Enhanced Ethereum data)             │
│  • Magic Eden API   (Solana NFT marketplace)            │
│  • DappRadar API    (DeFi and NFT analytics)           │
│  • Mintify API      (NFT marketplace data)               │
│  • GraphQL APIs     (Stargaze, other platforms)        │
│  • Social APIs      (Google OAuth, X API)               │
│  • AI Services      (Google AI Model)                   │
└─────────────────────────────────────────────────────────────┘
```

## 🗄️ Database Architecture

### Entity Relationship Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Database Schema                        │
├─────────────────────────────────────────────────────────────┤
│  User Management:                                          │
│  • Users, UserProfiles, UserSocials                      │
│  • VerifiedUsers, VerifyUsers                            │
│                                                           │
│  NFT Data:                                               │
│  • UserNftDatum, UserNftCollectionDatum                 │
│  • UserNftSale, UserNftTransaction                      │
│  • UserNftActivity, UserNftRecord                       │
│                                                           │
│  Portfolio Tracking:                                     │
│  • UserWallet, UserWalletValue                          │
│  • UserWalletSalesRecord, UserPortfolioRecord           │
│                                                           │
│  Social Features:                                        │
│  • UserFollower, UserProfileView                       │
│  • UserNotification, UserFeedback                       │
│                                                           │
│  Gamification:                                           │
│  • Badges, UserBadge, Milestones                        │
│  • UserExperience, UserTask                             │
│                                                           │
│  Analytics:                                              │
│  • UserStat, UserReferral                               │
│  • External Data (NFTScan, DappRadar)                   │
└─────────────────────────────────────────────────────────────┘
```

### Repository Pattern Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                Repository Implementations                 │
├─────────────────────────────────────────────────────────────┤
│  • UserRepository          • AssetRepository              │
│  • ProfileRepository       • WalletRepository             │
│  • FollowRepository        • BadgeRepository              │
│  • NotificationRepository  • DiscoverRepository           │
│  • SearchRepository        • NewsletterRepository         │
│  • FeedbackRepository      • DevRepository                │
│  • HomeRepository          • MarketplaceRepository        │
│  • WebhookRepository       • UnitOfWork                   │
└─────────────────────────────────────────────────────────────┘
```

## ⚡ Background Processing Architecture

### Quartz.NET Job Scheduler

```
┌─────────────────────────────────────────────────────────────┐
│                Background Jobs                           │
├─────────────────────────────────────────────────────────────┤
│  Data Synchronization Jobs:                              │
│  • GetEvmsCollectionDataJob                              │
│  • GetSolanaCollectionDataJob                           │
│  • GetTezosCollectionDataJob                            │
│  • GetTonCollectionDataJob                              │
│  • GetStargazeCollectionDataJob                        │
├─────────────────────────────────────────────────────────────┤
│  User-Related Jobs:                                      │
│  • SyncXFollowersDataJob                                │
│  • GetUserXMetricJob                                    │
│  • HandleReferralJob                                    │
│  • HandleUserTaskJob                                    │
│  • HandleWalletOwnerJob                                 │
├─────────────────────────────────────────────────────────────┤
│  Data Maintenance Jobs:                                  │
│  • DeleteWalletDataJob                                  │
│  • SyncUserNftIdJob                                     │
│  • GetUserNftCustodyDateJob                            │
├─────────────────────────────────────────────────────────────┤
│  NFT Data Synchronization Jobs:                         │
│  • SyncEvmNftDataJob                                   │
│  • SyncSolanaNftDataJob                                │
└─────────────────────────────────────────────────────────────┘
```

## 🔄 Real-time Communication Architecture

### SignalR Hub Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                SignalR Real-time System                  │
├─────────────────────────────────────────────────────────────┤
│  NotificationService Hub:                                │
│  • User Connection Management                            │
│  • Real-time Notifications                              │
│  • Offline Notification Queue                           │
│  • Portfolio Updates                                     │
│  • NFT Activity Tracking                                │
├─────────────────────────────────────────────────────────────┤
│  Connection Management:                                  │
│  • JWT Authentication for WebSocket                     │
│  • User Session Tracking                                │
│  • Connection State Management                          │
│  • Error Handling & Reconnection                        │
└─────────────────────────────────────────────────────────────┘
```

## 🔍 Observability Architecture

### Monitoring & Logging

```
┌─────────────────────────────────────────────────────────────┐
│                Observability Stack                       │
├─────────────────────────────────────────────────────────────┤
│  • Sentry Integration     (Error Tracking)               │
│  • OpenTelemetry         (Distributed Tracing)          │
│  • Custom Logging        (Structured Logging)           │
│  • Entity Framework      (Query Monitoring)             │
│  • Performance Metrics   (Application Metrics)          │
│  • Health Checks         (System Health)                │
└─────────────────────────────────────────────────────────────┘
```

## 🛡️ Security Architecture

### Authentication & Authorization

```
┌─────────────────────────────────────────────────────────────┐
│                Security Layers                           │
├─────────────────────────────────────────────────────────────┤
│  • JWT Authentication     (Token-based Auth)             │
│  • Password Hashing      (Secure Password Storage)       │
│  • CORS Configuration    (Cross-origin Security)         │
│  • Input Validation      (FluentValidation)             │
│  • API Key Management    (External Service Auth)        │
│  • Custom Filters        (TokenFilter, CoreFilter)      │
│  • HTTPS Enforcement     (Secure Communication)         │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 Deployment Architecture

### Container Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                Docker Container Setup                     │
├─────────────────────────────────────────────────────────────┤
│  Application Container:                                   │
│  • ASP.NET Core 8.0 Runtime                              │
│  • Ovation WebAPI                                        │
│  • SignalR Hub                                           │
│  • Background Job Scheduler                              │
├─────────────────────────────────────────────────────────────┤
│  Database Container:                                     │
│  • MySQL 8.0                                            │
│  • Persistent Volume                                     │
│  • Connection Pooling                                    │
├─────────────────────────────────────────────────────────────┤
│  Cache Container (Optional):                             │
│  • Redis                                                 │
│  • Session Storage                                       │
│  • Distributed Cache                                     │
└─────────────────────────────────────────────────────────────┘
```

## 📊 Performance Architecture

### Caching Strategy

```
┌─────────────────────────────────────────────────────────────┐
│                Caching Layers                            │
├─────────────────────────────────────────────────────────────┤
│  • Distributed Memory Cache  (In-memory caching)        │
│  • HTTP Client Pooling       (Connection reuse)          │
│  • Database Connection Pool  (EF Core pooling)           │
│  • Background Job Caching    (Job result caching)        │
│  • External API Caching     (Provider data caching)     │
└─────────────────────────────────────────────────────────────┘
```

### Scalability Features

```
┌─────────────────────────────────────────────────────────────┐
│                Scalability Components                    │
├─────────────────────────────────────────────────────────────┤
│  • Horizontal Scaling      (Multiple API instances)     │
│  • Background Processing    (Heavy operations offloaded)  │
│  • Database Optimization    (Indexed queries)           │
│  • Connection Pooling       (Efficient resource usage)   │
│  • Async/Await Patterns     (Non-blocking operations)   │
│  • Pagination Support       (Efficient data loading)     │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 Configuration Architecture

### Environment Configuration

```
┌─────────────────────────────────────────────────────────────┐
│                Configuration Management                   │
├─────────────────────────────────────────────────────────────┤
│  • Environment Variables   (Secure configuration)       │
│  • appsettings.json        (Base configuration)          │
│  • appsettings.{env}.json  (Environment-specific)        │
│  • Docker Environment      (Container configuration)      │
│  • Secret Management       (API keys, passwords)         │
│  • Feature Flags          (Conditional features)         │
└─────────────────────────────────────────────────────────────┘
```

## 🧪 Testing Architecture

### Test Organization

```
┌─────────────────────────────────────────────────────────────┐
│                Testing Strategy                          │
├─────────────────────────────────────────────────────────────┤
│  • Unit Tests           (Business logic testing)        │
│  • Integration Tests     (API endpoint testing)          │
│  • Repository Tests      (Data access testing)           │
│  • Service Tests         (External service mocking)      │
│  • End-to-End Tests      (Full workflow testing)         │
│  • Performance Tests     (Load and stress testing)       │
└─────────────────────────────────────────────────────────────┘
```

## 📈 Monitoring Architecture

### Health Monitoring

```
┌─────────────────────────────────────────────────────────────┐
│                Health Monitoring                         │
├─────────────────────────────────────────────────────────────┤
│  • Application Health     (API endpoint health)          │
│  • Database Health        (Connection and query health)  │
│  • External Service Health (API provider health)         │
│  • Background Job Health  (Job execution monitoring)     │
│  • SignalR Health         (Real-time connection health)  │
│  • Performance Metrics    (Response times, throughput)  │
└─────────────────────────────────────────────────────────────┘
```

## 🔄 Data Flow Patterns

### CQRS Implementation

```
Commands (Write Operations):
┌─────────────────────────────────────────────────────────────┐
│  Controller → MediatR → Command Handler → Repository      │
│  ↓                                                         │
│  Database Update → Domain Event → Response                │
└─────────────────────────────────────────────────────────────┘

Queries (Read Operations):
┌─────────────────────────────────────────────────────────────┐
│  Controller → MediatR → Query Handler → Repository        │
│  ↓                                                         │
│  Database Query → AutoMapper → DTO → Response             │
└─────────────────────────────────────────────────────────────┘
```

### Event-Driven Architecture

```
Domain Events:
┌─────────────────────────────────────────────────────────────┐
│  Entity Change → Domain Event → Event Handler              │
│  ↓                                                         │
│  Notification → SignalR → Real-time Update                │
│  ↓                                                         │
│  Background Job → Data Synchronization                    │
└─────────────────────────────────────────────────────────────┘
```

## 🎯 Key Architectural Benefits

### 1. **Separation of Concerns**
- Clear boundaries between layers
- Independent development and testing
- Easy maintenance and updates

### 2. **Scalability**
- Horizontal scaling capabilities
- Background job processing
- Efficient resource utilization

### 3. **Testability**
- Dependency injection throughout
- Mockable interfaces
- Isolated unit testing

### 4. **Maintainability**
- Clean code organization
- Consistent patterns
- Comprehensive documentation

### 5. **Flexibility**
- Easy to add new features
- Support for multiple blockchains
- Extensible architecture

### 6. **Security**
- Multiple security layers
- Secure authentication
- Input validation

### 7. **Performance**
- Optimized data access
- Caching strategies
- Async operations

### 8. **Observability**
- Comprehensive monitoring
- Error tracking
- Performance metrics

## 🚀 Future Architecture Considerations

### Microservices Migration
```
Current Monolith → Microservices:
┌─────────────────────────────────────────────────────────────┐
│  • User Service        (Authentication & Profiles)        │
│  • NFT Service         (Asset Management)                  │
│  • Social Service      (Follow, Notifications)            │
│  • Analytics Service   (Discovery & Rankings)             │
│  • Notification Service (Real-time Communication)          │
│  • Integration Service  (External APIs)                   │
└─────────────────────────────────────────────────────────────┘
```

### Event Sourcing
```
Event Store Implementation:
┌─────────────────────────────────────────────────────────────┐
│  • Domain Events → Event Store → Event Handlers           │
│  • Read Models → CQRS Queries → API Responses             │
│  • Event Replay → Data Reconstruction                     │
└─────────────────────────────────────────────────────────────┘
```

---

**This architecture provides a solid foundation for the Ovation platform, ensuring scalability, maintainability, and extensibility while supporting complex multi-blockchain NFT operations and social features.**
