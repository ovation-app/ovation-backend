# ADR 0008: Use of Tezos API for Accessing Tezos-based NFTs

**Status:** Accepted  
**Date:** 2025-05-05  

## Context  
Support for Tezos-based NFTs is required due to its popularity for clean NFT drops and low gas fees.

## Decision  
Use a Tezos NFT API (such as [TzKT](https://api.tzkt.io/) or [OBJKT](https://data.objkt.com/docs/)) depending on performance and data depth to fetch NFT holdings and metadata.

## Consequences  
- Enables integration with Tezos ecosystem.
- Slightly more complexity due to non-EVM model.
- Less standardized metadata formats than Ethereum-based platforms.
