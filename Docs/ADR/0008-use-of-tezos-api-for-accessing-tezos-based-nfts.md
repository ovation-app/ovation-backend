# ADR 0008: Use of Tezos API for Accessing Tezos-based NFTs

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
Support for Tezos-based NFTs is required due to its popularity for clean NFT drops, low gas fees, and environmental sustainability. The Tezos ecosystem has unique characteristics that require specialized API integration.

## Decision  
Use Tezos NFT APIs for accessing Tezos-based NFTs. Primary integration with [OBJKT API](https://data.objkt.com/docs/) for marketplace data and [TzKT API](https://api.tzkt.io/) for blockchain data. This includes:
- NFT holdings and metadata retrieval
- Transaction history and ownership data
- Collection information and statistics
- Marketplace integration data

## Alternatives Considered  
- **Direct Tezos RPC**: Rejected due to complexity and maintenance overhead
- **Single API Provider**: Rejected due to limited coverage
- **Custom Indexing**: Rejected due to development and maintenance costs
- **Third-party Aggregators**: Considered but direct APIs provide better control

## Consequences  
**Positive:**
- Enables integration with Tezos ecosystem
- Access to unique Tezos NFT collections
- Low-cost transaction support
- Environmental sustainability alignment
- Comprehensive marketplace data

**Negative:**
- Slightly more complexity due to non-EVM model
- Less standardized metadata formats than Ethereum-based platforms
- Multiple API dependencies
- Limited ecosystem compared to Ethereum
- Additional maintenance overhead

## Implementation Notes  
- Tezos service implementation in Infrastructure layer
- Multiple API integration for comprehensive coverage
- Non-EVM data model handling
- Metadata standardization across platforms
- Performance optimization for Tezos-specific queries
