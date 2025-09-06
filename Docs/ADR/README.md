# Architecture Decision Records (ADRs)

This directory contains Architecture Decision Records (ADRs) for the Ovation platform. ADRs document important architectural decisions, the context in which they were made, and their consequences.

## ADR Index

### Core Architecture
- [ADR 0001: Use of Clean Architecture](0001-use-of-clean-architecture.md)
- [ADR 0002: Use of MediatR for Application Layer Communication](0002-use-of-mediatr-for-application-layer-communication.md)
- [ADR 0003: Dependency Injection via Built-in .NET DI Container](0003-dependency-injection-via-built-in-net-di-container.md)
- [ADR 0004: Entity Framework Core for ORM](0004-entity-framework-core-for-orm.md)
- [ADR 0005: Use of FluentValidation for Input Validation](0005-use-of-fluentvalidation-for-input-validation.md)

### Data Access & External Integrations
- [ADR 0006: Use of NFTScan API for Multi-Chain NFT Metadata](0006-use-of-nftscan-api-for-multi-chain-nft-metadata.md)
- [ADR 0007: Use of Alchemy API for Enhanced Ethereum NFT Data](0007-use-of-alchemy-api-for-enhanced-ethereum-nft-data.md)
- [ADR 0008: Use of Tezos API for Accessing Tezos-based NFTs](0008-use-of-tezos-api-for-accessing-tezos-based-nfts.md)
- [ADR 0009: Use of Archway API for CosmWasm-based NFTs](0009-use-of-archway-api-for-cosmwasm-based-nfts.md)
- [ADR 0010: Use of Stargaze API for IBC NFTs in Cosmos Ecosystem](0010-use-of-stargaze-api-for-ibc-nfts-in-cosmos-ecosystem.md)

### Real-time Communication & Infrastructure
- [ADR 0011: Use of SignalR for Real-time Communication](0011-use-of-signalr-for-real-time-communication.md)
- [ADR 0012: Use of AutoMapper for Object Mapping](0012-use-of-automapper-for-object-mapping.md)
- [ADR 0013: Use of ULID for Unique Identifier Generation](0013-use-of-ulid-for-unique-identifier-generation.md)
- [ADR 0014: Use of Quartz.NET for Background Job Scheduling](0014-use-of-quartz-net-for-background-job-scheduling.md)

### Observability & Security
- [ADR 0015: Use of OpenTelemetry for Observability](0015-use-of-opentelemetry-for-observability.md)
- [ADR 0016: Use of Sentry for Error Tracking](0016-use-of-sentry-for-error-tracking.md)
- [ADR 0017: Use of JWT for Authentication](0017-use-of-jwt-for-authentication.md)

## ADR Template

When creating new ADRs, use the following template:

```markdown
# ADR XXXX: [Title]

**Status:** [Proposed | Accepted | Rejected | Superseded]  
**Date:** YYYY-MM-DD  
**Deciders:** [List of people who made the decision]  
**Consulted:** [List of people who were consulted]  

## Context  
[Describe the context and problem statement]

## Decision  
[Describe the decision and its implementation details]

## Alternatives Considered  
[List alternative solutions that were considered]

## Consequences  
**Positive:**
[List positive consequences]

**Negative:**
[List negative consequences]

## Implementation Notes  
[Additional implementation details and considerations]
```

## Status Definitions

- **Proposed**: The decision is under consideration
- **Accepted**: The decision has been approved and implemented
- **Rejected**: The decision was considered but not adopted
- **Superseded**: The decision has been replaced by a newer ADR

## Contributing

When making architectural decisions:

1. Create a new ADR using the template above
2. Follow the numbering sequence (next number after 0017)
3. Include all stakeholders in the decision process
4. Update this index when adding new ADRs
5. Review and update existing ADRs when making related changes

## Related Documentation

- [Architecture Overview](../ARCHITECTURE.md)
- [API Documentation](../API_DOCUMENTATION.md)
- [Provider Integrations](../PROVIDER_INTEGRATIONS.md)
