# ADR 0005: Use of FluentValidation for Input Validation

**Status:** Accepted  
**Date:** 2025-05-05  

## Context  
We need to validate DTOs received in the API layer cleanly and consistently.

## Decision  
Use [FluentValidation](https://docs.fluentvalidation.net/) and built-in [DataAnnotations](System.ComponentModel.DataAnnotations) to validate request models, registered with MediatR pipelines.

## Consequences  
- Centralized, reusable validation logic.
- Easier to unit test than [DataAnnotations].
