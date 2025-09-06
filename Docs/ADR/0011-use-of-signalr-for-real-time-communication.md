# ADR 0011: Use of SignalR for Real-time Communication

**Status:** Accepted  
**Date:** 2025-01-27  
**Deciders:** Development Team  
**Consulted:** Architecture Review Board  

## Context  
The Ovation platform requires real-time communication capabilities for notifications, portfolio updates, and social interactions. Users need instant updates about NFT activities, follower notifications, and portfolio value changes without requiring page refreshes.

## Decision  
Use ASP.NET Core SignalR for real-time bidirectional communication between the server and connected clients. This includes:
- WebSocket-based real-time communication
- Hub-based architecture for different communication channels
- JWT authentication for WebSocket connections
- Connection management and user session tracking
- Real-time notification delivery system

## Alternatives Considered  
- **Server-Sent Events (SSE)**: Rejected due to unidirectional communication limitation
- **WebSocket Raw**: Rejected due to complexity and lack of built-in features
- **Polling**: Rejected due to performance and scalability issues
- **Third-party Services**: Considered but SignalR provides sufficient features

## Consequences  
**Positive:**
- Real-time bidirectional communication
- Built-in connection management and scaling
- Automatic fallback to other transports (Long Polling, Server-Sent Events)
- Excellent integration with ASP.NET Core
- Built-in authentication and authorization support
- Efficient resource usage with connection pooling

**Negative:**
- Additional complexity in connection management
- Potential scaling challenges with many concurrent connections
- WebSocket connection state management overhead
- Browser compatibility considerations
- Memory usage for maintaining connections

## Implementation Notes  
- NotificationService hub for real-time notifications
- JWT authentication for WebSocket connections
- Connection state management and cleanup
- Error handling and reconnection logic
- Integration with background jobs for notification delivery
- CORS configuration for WebSocket connections
