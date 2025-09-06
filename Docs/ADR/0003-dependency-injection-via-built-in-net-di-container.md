# ADR 0003: Dependency Injection via Built-in .NET DI Container

**Status:** Accepted  
**Date:** 2025-05-05  

## Context  
We need a reliable DI container to manage dependencies across layers.

## Decision  
Use the built-in Microsoft.Extensions.DependencyInjection for registering services and handling lifetime scopes.

## Consequences  
- Simplifies registration.
- Ensures compatibility with .NET middleware and third-party libraries.
- Less feature-rich than containers like Autofac.
