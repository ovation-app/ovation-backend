# Ovation Web API - Technical Overview

## Overview
The Ovation Web API is a comprehensive RESTful API built with ASP.NET Core 8.0 that serves as the backend for the Ovation platform - a social NFT (Non-Fungible Token) and blockchain portfolio management application. The API follows Clean Architecture principles and implements the CQRS pattern with MediatR for handling business logic.

## Core Features & Functionalities

### 1. Authentication & Authorization
- **JWT-based Authentication**: Implements JWT Bearer token authentication with configurable issuer and lifetime
- **Social Login Support**: Google and X (Twitter) OAuth integration
- **User Registration**: Multiple registration flows including claimable profiles
- **Password Management**: Change password, forgot password with OTP verification
- **Account Verification**: Email verification with OTP codes
- **Custom Token Filter**: Additional security layer with token-based authorization

### 2. User Profile Management
- **Profile CRUD Operations**: Complete profile management with personal info, cover images, social links
- **Experience Management**: Add, update, and delete user experiences
- **Wallet Integration**: Connect and manage multiple blockchain wallets
- **Privacy Controls**: NFT visibility settings and privacy management
- **User Verification**: Multiple verification types for user authenticity
- **Social Features**: Follow/unfollow functionality with follower/following lists

### 3. NFT & Portfolio Management
- **NFT Display**: View user's NFT collections with pagination and filtering
- **Portfolio Analytics**: Portfolio records, transaction history, and value tracking
- **Collection Management**: Browse and manage NFT collections
- **Favorite NFTs**: Mark and manage favorite NFTs
- **Featured Items**: Highlight specific NFTs or collections
- **NFT Privacy**: Control visibility of individual NFTs

### 4. Discovery & Ranking System
- **Top NFT Holders**: Rankings based on NFT holdings
- **Highest Valued NFTs**: Rankings by NFT value
- **Blue Chip Holders**: Special rankings for premium NFT holders
- **Net Worth Rankings**: User rankings by total portfolio value
- **Contributors & Creators**: Community contributor rankings
- **Most Viewed/Followed**: Social engagement rankings
- **Sales Analytics**: Most sold NFTs and sales volume rankings
- **Featured Profiles**: Curated featured user profiles

### 5. Search & Discovery
- **User Search**: Search functionality for finding users
- **NFT Search**: Search across NFT collections and individual tokens
- **Collection Search**: Search for specific NFT collections
- **Pagination Support**: Efficient data loading with cursor-based pagination

### 6. Notification System
- **Real-time Notifications**: SignalR integration for live notifications
- **Notification Management**: Read/unread status management
- **Bulk Operations**: Mark all notifications as read
- **Notification Counts**: Track unread notification counts

### 7. Asset & Collection Management
- **Collection Details**: Comprehensive collection information
- **Token Management**: Individual NFT token details and transactions
- **Owner Distribution**: Analytics on collection ownership distribution
- **Transaction History**: Detailed transaction tracking for NFTs

### 8. Additional Features
- **Newsletter Management**: Email subscription and management
- **Feedback System**: User feedback collection and management
- **Affiliation System**: User referral and affiliation tracking
- **Badge System**: Achievement and recognition system
- **Path Management**: User journey and progression tracking
- **Webhook Support**: External system integration capabilities

## Technical Architecture

### 1. Architecture Pattern
- **Clean Architecture**: Separation of concerns across layers
- **CQRS Pattern**: Command Query Responsibility Segregation with MediatR
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Built-in .NET DI container

### 2. Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Authentication**: JWT Bearer tokens
- **Real-time Communication**: SignalR
- **API Documentation**: Swagger/OpenAPI
- **JSON Serialization**: Newtonsoft.Json
- **Logging**: Serilog with custom enrichers
- **Observability**: OpenTelemetry and Sentry integration

### 3. Security Features
- **JWT Authentication**: Secure token-based authentication
- **CORS Configuration**: Cross-origin resource sharing setup
- **Antiforgery Protection**: CSRF protection
- **Custom Authorization Filters**: Token-based access control
- **Input Validation**: Comprehensive request validation

### 4. Performance & Scalability
- **Distributed Caching**: Memory cache implementation
- **Pagination**: Efficient data loading with cursor-based pagination
- **Background Services**: Asynchronous job processing
- **Static File Serving**: Optimized file delivery
- **HTTPS Redirection**: Secure communication enforcement

### 5. Monitoring & Observability
- **Sentry Integration**: Error tracking and performance monitoring
- **OpenTelemetry**: Distributed tracing and metrics
- **Custom Logging**: Structured logging with custom enrichers
- **User Context Tracking**: Sentry user context for debugging

### 6. API Design
- **RESTful Endpoints**: Standard REST API design
- **Consistent Response Format**: Standardized API responses
- **Error Handling**: Comprehensive error management
- **API Versioning**: Swagger documentation with versioning
- **Rate Limiting**: Request throttling capabilities

## Configuration & Deployment

### 1. Environment Configuration
- **Development/Production**: Environment-specific settings
- **Port Configuration**: Configurable port binding (default: 8080)
- **CORS Policies**: Flexible CORS configuration
- **JWT Settings**: Configurable token parameters

### 2. Dependencies
- **Application Layer**: Business logic and feature handlers
- **Infrastructure Layer**: Data persistence and external services
- **Domain Layer**: Core business entities and rules

### 3. External Integrations
- **Blockchain APIs**: Multiple blockchain service integrations
- **Social Platforms**: Google and X authentication
- **Email Services**: Newsletter and notification delivery
- **Webhook Systems**: External system notifications

## Key Technical Aspects

### 1. Clean Architecture Implementation
- Clear separation between API, Application, Domain, and Infrastructure layers
- Dependency inversion with interfaces
- Testable and maintainable code structure

### 2. CQRS with MediatR
- Commands for write operations
- Queries for read operations
- Centralized request/response handling
- Improved performance and scalability

### 3. Real-time Features
- SignalR hub for live notifications
- WebSocket support for real-time updates
- Token-based authentication for SignalR connections

### 4. Multi-blockchain Support
- EVM chains (Ethereum, Polygon, etc.)
- Solana blockchain
- TON blockchain
- Archway blockchain
- Tezos blockchain
- Stargaze blockchain

### 5. Comprehensive Error Handling
- Custom exception types
- Consistent error response format
- Proper HTTP status codes
- Detailed error logging

This API serves as a robust foundation for a comprehensive NFT social platform, providing users with tools to manage their digital assets, connect with other collectors, and discover new opportunities in the NFT ecosystem.
