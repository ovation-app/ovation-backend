# ADR 0001: Use of Clean Architecture

**Status:** Accepted  
**Date:** 2025-05-05  

## Context  
To ensure maintainability, scalability, and testability, we need a well-structured architecture for our .NET Web API.

## Decision  
We will adopt the Clean Architecture pattern. It separates concerns by dividing the system into concentric layers:
- **Domain Layer**: Contains enterprise logic and entities.
- **Application Layer**: Contains use cases and interfaces for services.
- **Infrastructure Layer**: Contains data access, file storage, and external services.
- **API Layer**: Contains controllers and HTTP entry points.

## Consequences  
- Increased separation of concerns.
- Easier unit testing of core logic.
- Slower initial development due to more boilerplate.
