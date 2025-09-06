# ADR 0005: Use of FluentValidation for Input Validation

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
We need to validate DTOs received in the API layer cleanly and consistently. The Ovation platform handles complex multi-blockchain data with various validation requirements including wallet addresses, blockchain-specific formats, and business rule validation.

## Decision  
Use [FluentValidation](https://docs.fluentvalidation.net/) for comprehensive input validation, integrated with MediatR pipeline behaviors. This includes:
- Validator classes for each request DTO
- Pipeline behavior for automatic validation
- Custom validation rules for blockchain-specific data
- Integration with MediatR request/response pipeline

## Alternatives Considered  
- **DataAnnotations**: Rejected due to limited flexibility and testing complexity
- **Custom Validation**: Rejected due to maintenance overhead
- **ModelState Validation**: Rejected due to tight coupling to MVC
- **Manual Validation**: Rejected due to inconsistency and maintenance issues

## Consequences  
**Positive:**
- Centralized, reusable validation logic
- Easier to unit test than DataAnnotations
- Fluent, readable validation rules
- Excellent integration with MediatR
- Support for complex validation scenarios
- Clear separation of validation concerns
- Easy to mock for testing

**Negative:**
- Additional dependency and learning curve
- More boilerplate for simple validations
- Potential performance overhead for complex validations
- Requires discipline to maintain consistency

## Implementation Notes  
- Validator classes in Application layer
- Pipeline behavior for automatic validation execution
- Custom validators for blockchain address validation
- Error message localization support
- Integration with MediatR request pipeline
