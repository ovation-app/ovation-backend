# ADR 0007: Use of Alchemy API for Enhanced Ethereum NFT Data

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
Abstract NFTs are a key focus for the Ovation platform. We need detailed metadata, transaction history, and Web3-enhanced APIs for Ethereum-based NFTs. NFTScan provides good coverage but Alchemy offers deeper integration and enhanced features for Ethereum ecosystem.

## Decision  
Integrate the [Alchemy NFT API](https://docs.alchemy.com/reference/nft-api-quickstart) to supplement and validate Abstract NFT data. Alchemy will provide:
- Enhanced Ethereum NFT metadata
- Detailed transaction history
- Web3-enhanced API features
- Real-time data updates
- Advanced querying capabilities

## Alternatives Considered  
- **NFTScan Only**: Rejected due to limited Ethereum-specific features
- **Direct Ethereum RPC**: Rejected due to complexity and maintenance overhead
- **Multiple Ethereum APIs**: Considered but Alchemy provides comprehensive coverage
- **Custom Indexing**: Rejected due to development and maintenance costs

## Consequences  
**Positive:**
- Provides high-reliability and fast indexing
- Enhanced Ethereum-specific features
- Better integration with Web3 ecosystem
- Comprehensive transaction history
- Real-time data accuracy

**Negative:**
- Redundant with NFTScan in some areas but deeper in others
- Additional cost if premium features are used
- Dependency on another third-party service
- Potential rate limiting constraints
- Additional complexity in data aggregation

## Implementation Notes  
- Alchemy service implementation in Infrastructure layer
- Data aggregation with NFTScan for comprehensive coverage
- Rate limiting and retry policies
- Cost monitoring and optimization
- Fallback mechanisms for service unavailability
