# ADR 0002: Use of MediatR for Application Layer Communication

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
We want to decouple the use case execution from the controllers and avoid direct service calls in the API layer. The Ovation platform requires a clean separation between presentation and business logic, with support for CQRS pattern implementation.

## Decision  
Use [MediatR](https://github.com/jbogard/MediatR) for CQRS-style request/response messaging between the API and Application layers. MediatR will handle:
- Command/Query separation (CQRS pattern)
- Request/Response pipeline processing
- Validation behavior integration
- Handler registration and discovery

## Alternatives Considered  
- **Direct Service Injection**: Rejected due to tight coupling
- **Application Service Pattern**: Considered but MediatR provides better CQRS support
- **Custom Mediator**: Rejected due to complexity and maintenance overhead

## Consequences  
**Positive:**
- Clean separation of concerns between layers
- Reduces direct dependencies in controllers
- Enables CQRS pattern implementation
- Centralized request/response handling
- Built-in pipeline behaviors (validation, logging)
- Easy to add cross-cutting concerns

**Negative:**
- Adds slight overhead and complexity
- Learning curve for developers unfamiliar with MediatR
- Additional abstraction layer
- Potential performance impact for simple operations

## Implementation Notes  
- Commands for write operations (Create, Update, Delete)
- Queries for read operations (Get, Search, List)
- Pipeline behaviors for validation and logging
- Handler registration in Application layer ServiceExtensions
- BaseHandler class for common dependencies
