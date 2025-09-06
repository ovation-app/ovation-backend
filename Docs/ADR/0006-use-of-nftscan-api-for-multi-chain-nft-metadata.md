# ADR 0006: Use of NFTScan API for Multi-Chain NFT Metadata

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
We need access to multi-chain NFT metadata and ownership details in a standardized format. The Ovation platform requires comprehensive NFT data across multiple blockchains including Ethereum, BNB Chain, Polygon, Base, and Optimism.

## Decision  
Integrate the [NFTScan API](https://docs.nftscan.com/) as the primary NFT data provider to retrieve user-owned NFTs, metadata, and collection information. NFTScan supports several major chains and provides:
- Standardized NFT metadata across chains
- Real-time ownership data
- Collection information and statistics
- Transaction history
- Pricing data integration

## Alternatives Considered  
- **Direct Blockchain APIs**: Rejected due to complexity and maintenance overhead
- **Multiple Provider APIs**: Considered but NFTScan provides sufficient coverage
- **Custom Indexing**: Rejected due to development and maintenance costs
- **Alchemy Only**: Rejected due to limited multi-chain support

## Consequences  
**Positive:**
- Simplifies NFT data fetching across chains
- Centralized API reduces logic duplication
- Standardized data format across blockchains
- Comprehensive coverage of major chains
- Real-time data updates
- Cost-effective solution

**Negative:**
- Dependent on a third-party service's availability and pricing model
- Potential rate limiting constraints
- Limited customization options
- Single point of failure
- Data accuracy depends on NFTScan's indexing

## Implementation Notes  
- NFTScan service implementation in Infrastructure layer
- Rate limiting and retry policies
- Caching strategy for frequently accessed data
- Fallback mechanisms for service unavailability
- API key management and rotation
