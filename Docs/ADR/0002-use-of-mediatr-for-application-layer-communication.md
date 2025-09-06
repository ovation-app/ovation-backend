# ADR 0002: Use of MediatR for Application Layer Communication

**Status:** Accepted  
**Date:** 2025-05-05  

## Context  
We want to decouple the use case execution from the controllers and avoid direct service calls in the API layer.

## Decision  
Use [MediatR](https://github.com/jbogard/MediatR) for CQRS-style request/response messaging between the API and Application layers.

## Consequences  
- Clean separation of concerns.
- Reduces direct dependencies in the controller.
- Adds slight overhead and complexity.
