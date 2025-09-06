# ADR 0003: Dependency Injection via Built-in .NET DI Container

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
We need a reliable DI container to manage dependencies across layers in our Clean Architecture implementation. The container must support complex dependency graphs, lifetime management, and integration with .NET ecosystem components.

## Decision  
Use the built-in Microsoft.Extensions.DependencyInjection for registering services and handling lifetime scopes. This includes:
- Service registration in each layer's ServiceExtensions
- Lifetime management (Singleton, Scoped, Transient)
- Interface-to-implementation mapping
- Integration with ASP.NET Core middleware

## Alternatives Considered  
- **Autofac**: Rejected due to additional complexity and .NET Core compatibility issues
- **Ninject**: Rejected due to limited .NET Core support
- **Simple Injector**: Considered but built-in container provides sufficient features
- **Manual DI**: Rejected due to maintenance overhead

## Consequences  
**Positive:**
- Simplifies registration and configuration
- Ensures compatibility with .NET middleware and third-party libraries
- No additional dependencies or learning curve
- Excellent performance and memory efficiency
- Built-in support for configuration binding
- Seamless integration with ASP.NET Core

**Negative:**
- Less feature-rich than containers like Autofac
- Limited advanced scenarios support
- No built-in decorator pattern support
- Less flexible lifetime management

## Implementation Notes  
- ServiceExtensions classes in each layer for organized registration
- Repository interfaces registered in Application layer
- Repository implementations registered in Infrastructure layer
- Scoped lifetime for most services to ensure proper disposal
- Singleton for stateless services and caches
