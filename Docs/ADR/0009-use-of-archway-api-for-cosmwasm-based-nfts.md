# ADR 0009: Use of Archway API for CosmWasm-based NFTs

**Status:** Accepted  
**Date:** 2025-05-05  

## Context  
We are expanding to Cosmos chains like Archway that use CosmWasm smart contracts for NFTs.

## Decision  
Use Archway’s JSON-RPC or REST API for smart contract interaction, combined with indexers (where available) to fetch NFT balances and metadata.

## Consequences  
- Aligns with expansion to Cosmos ecosystem.
- Requires parsing CosmWasm smart contract storage manually if no indexer is used.
- Varying tooling maturity across networks.
