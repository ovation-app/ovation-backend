# ADR 0010: Use of Stargaze API for IBC NFTs in Cosmos Ecosystem

**Status:** Accepted  
**Date:** 2025-05-05  

## Context  
Stargaze is a key player in the Cosmos NFT space. We want to allow users to view their IBC-compatible NFTs.

## Decision  
Use [Stargaze API](http://nft-api.mainnet.stargaze-apis.com/) to fetch NFT collections, ownership, and metadata.

## Consequences  
- Stargaze-specific schema parsing required.
- Depends on Stargaze indexer availability and Cosmos ecosystem standards.
