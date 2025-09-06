# Ovation.Application Layer Overview

## Architecture Overview

The Ovation.Application layer implements the **Application Layer** of Clean Architecture, serving as the core business logic orchestrator for the Ovation platform. This layer follows the **CQRS (Command Query Responsibility Segregation)** pattern using **MediatR** for handling commands and queries, with a focus on NFT portfolio management and social features.

## Technology Stack

- **.NET 8.0** - Target framework
- **MediatR** - CQRS implementation and request/response handling
- **AutoMapper** - Object mapping between DTOs and domain entities
- **FluentValidation** - Input validation with pipeline behavior
- **Entity Framework Core** - Data access abstractions
- **ULID** - Unique identifier generation

## Core Architecture Patterns

### 1. CQRS Pattern
The application layer implements CQRS with clear separation between:
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, Search, List)

### 2. MediatR Pipeline
- **ValidationBehavior**: Automatic request validation using FluentValidation
- **BaseHandler**: Common repository and service dependencies
- **Request/Response Pattern**: Structured communication between layers

### 3. Repository Pattern
All data access is abstracted through repository interfaces, providing:
- Clean separation of concerns
- Testability
- Dependency injection support

## Feature Modules

### 1. Authentication & Authorization (`AuthFeatures`)
**Purpose**: User authentication and account management
**Key Functionalities**:
- Multi-provider login (Google, X/Twitter, Normal)
- User registration and profile creation
- Password management and OTP verification
- Account verification and validation
- Claimable profile creation

**Commands**:
- `RegisterUserCommand`
- `NormalLoginCommand` / `GoogleLoginCommand` / `XLoginCommand`
- `ChangePasswordCommand`
- `CreateClaimableProfileCommand`
- `GetUserDataCommand`

**Queries**:
- `AuthUserDataQuery`
- `CheckEmailQuery` / `CheckUsernameQuery`
- `VerifyOtpQuery` / `VerifyAcctQuery`
- `ForgetPasswordQuery`

### 2. Asset Management (`AssetFeatures`)
**Purpose**: NFT and token portfolio management
**Key Functionalities**:
- Collection and token data retrieval
- Owner distribution analysis
- Transaction history tracking
- Multi-chain asset support

**Queries**:
- `GetCollectionQuery` / `GetCollectionTokensQuery`
- `GetOwnerDistributionQuery`
- `GetTokenTransactionQuery`

### 3. Profile Management (`ProfileFeatures`)
**Purpose**: User profile and portfolio management
**Key Functionalities**:
- User profile data retrieval
- NFT portfolio overview
- Social connections and experiences
- Verification status
- Favorite NFTs management
- Portfolio statistics

**Queries**:
- `GetUserQuery` / `GetAuthUserQuery`
- `GetUserNftsHandler`
- `GetUserWalletsQuery`
- `GetUserPortfolioQuery`
- `GetUserStatQuery`
- `GetUserSocialsQuery`
- `GetUserExperiencesQuery`
- `GetUserBadgesQuery`
- `GetUserFavNftQuery`
- `GetNftOverviewQuery`

### 4. Discovery & Analytics (`DiscoverFeatures`)
**Purpose**: NFT market analysis and trending data
**Key Functionalities**:
- NFT rankings and valuations
- Top creators and contributors
- Blue chip NFT holders
- Market trends and analytics
- Founder NFT tracking
- Net worth calculations

**Queries**:
- `GetBluechipHoldersQuery`
- `GetTopCreatorsQuery` / `GetTopContributorsQuery`
- `GetNftRankingQuery`
- `GetMostViewedQuery` / `GetMostSoldQuery`
- `GetHighestValuedNftQuery`
- `GetFounderNftRankingQuery`
- `GetFeaturedProfileQuery`
- `GetNetworthQuery`

### 5. Social Features (`FollowFeatures`)
**Purpose**: User social interactions and connections
**Key Functionalities**:
- Follow/unfollow users
- Follower/following management
- Social network building

**Commands**:
- `FollowUserCommand`
- `UnfollowUserCommand`

**Queries**:
- `GetUserFollowersQuery`
- `GetUserFollowingsQuery`
- `GetUsernameFollowersQuery`
- `GetUsernameFollowingsQuery`

### 6. Notification System (`NotificationFeatures`)
**Purpose**: Real-time notifications and alerts
**Key Functionalities**:
- Notification management
- Read/unread status tracking
- Bulk notification operations

**Commands**:
- `ReadNotificationCommand`
- `ReadAllNotificationsCommand`

**Queries**:
- `GetNotificationsQuery`
- `GetNotificationCountQuery`

### 7. Search & Discovery (`SearchFeatures`)
**Purpose**: Content discovery and search functionality
**Key Functionalities**:
- User search
- NFT search
- Collection search

**Queries**:
- `SearchUserQuery`
- `SearchNftQuery`
- `SearchCollectionQuery`

### 8. Wallet Management (`WalletFeatures`)
**Purpose**: Multi-chain wallet integration
**Key Functionalities**:
- Wallet addition and management
- Multi-chain support (Ethereum, Solana, Tezos, etc.)

**Commands**:
- `AddWalletCommand`

**Queries**:
- `GetWalletsQuery`
- `GetWalletQuery`

### 9. Badge System (`BadgeFeatures`)
**Purpose**: Achievement and gamification system
**Key Functionalities**:
- Badge management
- Blue chip NFT recognition
- Achievement tracking

**Queries**:
- `GetBadgesQuery`
- `GetBlueChipsQuery`

### 10. Home & Feed (`HomeFeatures`)
**Purpose**: Main dashboard and content feed
**Key Functionalities**:
- User discovery
- NFT showcase
- Trending content

**Queries**:
- `GetUsersQuery`
- `GetNftsQuery`

### 11. Feedback System (`FeedbackFeatures`)
**Purpose**: User feedback collection
**Key Functionalities**:
- Feedback submission
- User experience tracking

**Commands**:
- `AddFeedbackCommand`

### 12. Newsletter & Communication (`NewsletterFeatures`)
**Purpose**: Email communication and subscriptions
**Key Functionalities**:
- Newsletter subscriptions
- Nuclear playground features

**Commands**:
- `AddNewsSubcriberCommand`
- `NuclearPlayGroundCommand`

### 13. Path Management (`PathFeatures`)
**Purpose**: User journey and progression tracking
**Key Functionalities**:
- Path creation and management
- User progression tracking

**Commands**:
- `AddPathCommand`

**Queries**:
- `GetPathQuery`
- `GetPathsQuery`

### 14. Webhook Integration (`WebhookFeatures`)
**Purpose**: External service integration
**Key Functionalities**:
- NFT activity tracking
- Real-time data synchronization

**Commands**:
- `AddNftActivityCommand`

### 15. Developer Token Features (`DevTokenFeatures`)
**Purpose**: Token filtering and management for developers
**Key Functionalities**:
- Token filtering
- Core token management

**Queries**:
- `TokenFilterQuery`
- `CoreTokenFilterQuery`

### 16. Affiliation System (`AffilationFeatures`)
**Purpose**: Referral and affiliation tracking
**Key Functionalities**:
- Invited user tracking
- Affiliation data management

**Queries**:
- `GetAffilationDataQuery`
- `GetInvitedUserQuery`

## Technical Implementation Details

### 1. Validation Pipeline
- **ValidatorBehavior**: Automatic request validation using FluentValidation
- **BadRequestException**: Standardized error handling for validation failures
- **Pipeline Integration**: Seamless integration with MediatR pipeline

### 2. Multi-Chain Support
The application supports multiple blockchain networks:
- **Ethereum** (ETH, Polygon, Base, Optimism)
- **Solana**
- **Tezos**
- **Archway**
- **TON**
- **Stargaze**
- **Cosmos**

### 3. External API Integrations
- **NFTScan**: Multi-chain NFT data
- **Magic Eden**: Solana NFT marketplace
- **Alchemy**: Ethereum blockchain data
- **DappRadar**: DeFi and NFT analytics
- **Mintify**: NFT marketplace data

### 4. Real-time Features
- **SignalR Integration**: Real-time notifications and messaging
- **Concurrent Collections**: Thread-safe data structures for user sessions
- **Offline Notification Queue**: Persistent notification storage

### 5. Security Features
- **Password Hashing**: Secure password storage
- **OTP Verification**: Two-factor authentication
- **JWT Token Management**: Secure authentication tokens
- **Environment-based Configuration**: Secure API key management

### 6. Performance Optimizations
- **AutoMapper**: Efficient object mapping
- **Repository Pattern**: Optimized data access
- **Caching Strategies**: In-memory caching for frequently accessed data
- **Async/Await**: Non-blocking operations throughout

## DTOs and Data Transfer

The application uses extensive DTOs for:
- **API Communication**: Structured request/response objects
- **Multi-chain Data**: Blockchain-specific data structures
- **User Management**: Profile and authentication data
- **NFT Data**: Collection and token information
- **Analytics**: Market and portfolio data

## Error Handling

- **Custom Exceptions**: Domain-specific error types
- **Validation Errors**: Structured validation failure responses
- **Business Logic Errors**: Appropriate error codes and messages
- **Infrastructure Errors**: Graceful handling of external service failures

## Configuration Management

- **Environment Variables**: Secure configuration management
- **API Keys**: External service authentication
- **Chain Configuration**: Multi-chain network settings
- **Feature Flags**: Conditional feature activation

## Summary

The Ovation.Application layer serves as a comprehensive NFT portfolio management and social platform with:
- **16 Feature Modules** covering all aspects of the platform
- **CQRS Architecture** with MediatR for clean separation of concerns
- **Multi-chain Support** for major blockchain networks
- **Real-time Features** for interactive user experience
- **Robust Security** with proper authentication and authorization
- **Scalable Design** with repository pattern and dependency injection
- **Extensive Integration** with external NFT and blockchain services

This layer provides a solid foundation for the Ovation platform's business logic while maintaining clean architecture principles and ensuring maintainability and testability.
