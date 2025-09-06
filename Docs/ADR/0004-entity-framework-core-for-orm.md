# ADR 0004: Entity Framework Core for ORM

**Status:** Accepted  
**Date:** 2025-05-05  

## Context  
We need a reliable and maintainable way to interact with our SQL database.

## Decision  
Use EF Core in the Infrastructure layer to implement repository interfaces defined in the Application layer.

## Consequences  
- Productivity via LINQ and change tracking.
- Coupling to EF Core and potential performance considerations.
- Complex queries may require optimization.
