# ADR 0010: Use of Stargaze API for IBC NFTs in Cosmos Ecosystem

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
Stargaze is a key player in the Cosmos NFT space and a major NFT marketplace in the Cosmos ecosystem. We want to allow users to view their IBC-compatible NFTs and access Stargaze-specific collections and features.

## Decision  
Use [Stargaze API](http://nft-api.mainnet.stargaze-apis.com/) to fetch NFT collections, ownership, and metadata. This includes:
- NFT collection data and metadata
- User ownership and balance queries
- Transaction history and activity
- Marketplace integration data
- IBC-compatible NFT support

## Alternatives Considered  
- **Direct Cosmos RPC**: Rejected due to complexity
- **Third-party Aggregators**: Considered but Stargaze API provides better coverage
- **Custom Indexing**: Rejected due to development overhead
- **Skip Stargaze Integration**: Rejected due to ecosystem importance

## Consequences  
**Positive:**
- Access to Stargaze NFT marketplace
- IBC-compatible NFT support
- Integration with Cosmos ecosystem
- Unique NFT collections and features
- Future-proofing for Cosmos growth

**Negative:**
- Stargaze-specific schema parsing required
- Depends on Stargaze indexer availability and Cosmos ecosystem standards
- Limited ecosystem compared to Ethereum
- Additional complexity for IBC handling
- Potential service dependency issues

## Implementation Notes  
- Stargaze service implementation in Infrastructure layer
- IBC protocol handling and data parsing
- Stargaze-specific schema mapping
- Integration with Cosmos ecosystem standards
- Error handling and fallback mechanisms
