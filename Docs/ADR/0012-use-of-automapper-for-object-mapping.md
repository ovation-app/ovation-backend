# ADR 0012: Use of AutoMapper for Object Mapping

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
The Ovation platform requires frequent mapping between Domain entities, DTOs, and external API models. With 49 domain entities and numerous DTOs, manual mapping would be error-prone and maintenance-intensive. We need a reliable, performant solution for object-to-object mapping.

## Decision  
Use AutoMapper for automatic object mapping between different layers and data structures. This includes:
- Domain entities to DTOs mapping
- DTOs to Domain entities mapping
- External API models to Domain entities mapping
- Custom mapping profiles for complex scenarios
- Performance optimization with compiled mappings

## Alternatives Considered  
- **Manual Mapping**: Rejected due to maintenance overhead and error-proneness
- **Custom Mapping Library**: Rejected due to development overhead
- **Reflection-based Mapping**: Rejected due to performance concerns
- **Mapster**: Considered but AutoMapper has better .NET ecosystem support

## Consequences  
**Positive:**
- Reduces boilerplate code significantly
- Type-safe mapping with compile-time checking
- Excellent performance with compiled mappings
- Rich configuration options for complex scenarios
- Easy to test and maintain
- Built-in null handling and validation

**Negative:**
- Additional dependency and learning curve
- Potential performance overhead for simple mappings
- Debugging complexity for mapping issues
- Configuration complexity for edge cases
- Memory usage for mapping configurations

## Implementation Notes  
- Mapping profiles in Application layer
- Configuration in ServiceExtensions
- Custom value resolvers for complex mappings
- Performance testing and optimization
- Integration with MediatR handlers
- Error handling for mapping failures
