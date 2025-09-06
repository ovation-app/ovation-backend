# ADR 0006: Use of NFTScan API for Multi-Chain NFT Metadata

**Status:** Accepted  
**Date:** 2025-05-05  

## Context  
We need access to multi-chain NFT metadata and ownership details in a standardized format.

## Decision  
Integrate the [NFTScan API](https://docs.nftscan.com/) to retrieve user-owned NFTs, metadata, and collection information. NFTScan supports several major chains (e.g., Ethereum, BNB, Polygon).

## Consequences  
- Simplifies NFT data fetching across chains.
- Centralized API reduces logic duplication.
- Dependent on a third-party service's availability and pricing model.
