# ADR 0009: Use of Archway API for CosmWasm-based NFTs

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
We are expanding to Cosmos chains like Archway that use CosmWasm smart contracts for NFTs. The Cosmos ecosystem presents unique challenges with CosmWasm smart contracts and IBC (Inter-Blockchain Communication) protocols.

## Decision  
Use Archway's JSON-RPC and REST API for smart contract interaction, combined with available indexers to fetch NFT balances and metadata. This includes:
- CosmWasm smart contract interaction
- NFT balance and ownership queries
- Metadata retrieval and parsing
- Transaction history tracking
- IBC-compatible data handling

## Alternatives Considered  
- **Direct CosmWasm Interaction**: Rejected due to complexity
- **Third-party Indexers Only**: Rejected due to limited coverage
- **Custom Indexing Solution**: Rejected due to development overhead
- **Skip Cosmos Integration**: Rejected due to ecosystem importance

## Consequences  
**Positive:**
- Aligns with expansion to Cosmos ecosystem
- Access to IBC-compatible NFTs
- Future-proofing for Cosmos ecosystem growth
- Interoperability with other Cosmos chains
- Unique NFT collections and features

**Negative:**
- Requires parsing CosmWasm smart contract storage manually if no indexer is used
- Varying tooling maturity across networks
- Complex IBC protocol handling
- Limited ecosystem compared to Ethereum
- Additional development and maintenance complexity

## Implementation Notes  
- Archway service implementation in Infrastructure layer
- CosmWasm smart contract interaction patterns
- IBC protocol handling and data parsing
- Indexer integration where available
- Fallback mechanisms for manual contract parsing
