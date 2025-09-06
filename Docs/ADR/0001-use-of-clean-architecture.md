# ADR 0001: Use of Clean Architecture

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
To ensure maintainability, scalability, and testability, we need a well-structured architecture for our .NET Web API. The Ovation platform handles complex multi-blockchain NFT portfolio management with social features, requiring clear separation of concerns and independent testability.

## Decision  
We will adopt the Clean Architecture pattern (also known as Onion Architecture). It separates concerns by dividing the system into concentric layers with dependency inversion:

- **Domain Layer (Ovation.Domain)**: Contains enterprise logic, entities (49 entities), and business rules. No external dependencies.
- **Application Layer (Ovation.Application)**: Contains use cases, CQRS handlers, DTOs, and interfaces. Depends only on Domain.
- **Infrastructure Layer (Ovation.Infrastructure)**: Contains data access (EF Core), external services (7 blockchain APIs), and implementations. Depends on Application and Domain.
- **Presentation Layer (Ovation.WebAPI)**: Contains controllers, SignalR hubs, and HTTP entry points. Depends on Application.

## Alternatives Considered  
- **Layered Architecture**: Rejected due to tight coupling between layers
- **Hexagonal Architecture**: Considered but Clean Architecture provides better .NET ecosystem support
- **Microservices**: Considered for future but monolith is appropriate for current scale

## Consequences  
**Positive:**
- Increased separation of concerns and maintainability
- Easier unit testing of core business logic
- Independent development of layers
- Clear dependency direction (inward dependencies only)
- Easy to mock dependencies for testing

**Negative:**
- Slower initial development due to more boilerplate
- Steeper learning curve for new developers
- More complex project structure
- Potential over-engineering for simple features

## Implementation Notes  
- Dependency injection configured in each layer's ServiceExtensions
- Repository pattern implemented in Infrastructure layer
- CQRS pattern implemented using MediatR in Application layer
- Domain entities contain business logic and validation rules
