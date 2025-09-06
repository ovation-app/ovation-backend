# Ovation Backend API Documentation

## 📋 Overview

The Ovation Backend API is a comprehensive RESTful API built with ASP.NET Core 8.0 that provides endpoints for NFT portfolio management, social features, and multi-blockchain integration. The API follows REST conventions and uses JWT authentication for secure access.

## 🔐 Authentication

### JWT Token Authentication
All protected endpoints require a valid JWT token in the Authorization header:

```http
Authorization: Bearer <your-jwt-token>
```

### Token Filters
- **TokenFilter**: Standard authentication for most endpoints
- **CoreFilter**: Developer token authentication for core endpoints
- **Authorize**: ASP.NET Core authorization for protected resources

## 📚 API Endpoints

### Authentication (`/api/auth`)

#### User Registration
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "securePassword123",
  "username": "username",
  "displayName": "Display Name"
}
```

**Response:**
```json
{
  "message": "Success",
  "userData": {
    "userId": "guid",
    "email": "user@example.com",
    "username": "username",
    "displayName": "Display Name"
  },
  "token": "jwt-token"
}
```

#### User Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

#### Google OAuth Login
```http
POST /api/auth/login/google
Content-Type: application/json

{
  "socialId": "google-user-id"
}
```

#### X (Twitter) OAuth Login
```http
POST /api/auth/login/x
Content-Type: application/json

{
  "socialId": "twitter-user-id"
}
```

#### Email Verification
```http
GET /api/auth/account/verify/{code}/{otp}
```

#### Password Reset
```http
POST /api/auth/forget-password
Content-Type: application/json

{
  "email": "user@example.com"
}
```

#### Change Password
```http
PATCH /api/auth/change-password
Authorization: Bearer <token>
Content-Type: application/json

{
  "currentPassword": "oldPassword",
  "newPassword": "newPassword123"
}
```

### User Profile (`/api/profile`)

#### Get Current User Profile
```http
GET /api/profile
Authorization: Bearer <token>
```

#### Get User Profile by Username
```http
GET /api/profile/{username}
Authorization: Bearer <token>
```

#### Update Personal Information
```http
PATCH /api/profile/personal-info
Authorization: Bearer <token>
Content-Type: application/json

{
  "displayName": "New Display Name",
  "bio": "Updated bio",
  "location": "New Location"
}
```

#### Update Profile Image
```http
PATCH /api/profile/profile-image
Authorization: Bearer <token>
Content-Type: application/json

{
  "imageUrl": "https://example.com/image.jpg"
}
```

#### Update Cover Image
```http
PATCH /api/profile/cover-image
Authorization: Bearer <token>
Content-Type: application/json

{
  "imageUrl": "https://example.com/cover.jpg"
}
```

#### Update Social Links
```http
PATCH /api/profile/socials
Authorization: Bearer <token>
Content-Type: application/json

{
  "twitter": "https://twitter.com/username",
  "linkedin": "https://linkedin.com/in/username",
  "lens": "https://lens.xyz/username"
}
```

### NFT Management (`/api/profile`)

#### Get User NFTs
```http
GET /api/profile/nft/{userId}?page=1&limit=20&chain=eth
Authorization: Bearer <token>
```

**Query Parameters:**
- `page`: Page number (default: 1)
- `limit`: Items per page (default: 20)
- `chain`: Blockchain filter (eth, solana, tezos, etc.)
- `collection`: Collection filter
- `sortBy`: Sort field (value, date, name)

#### Get NFT Overview
```http
GET /api/profile/nft/overview/{userId}
Authorization: Bearer <token>
```

#### Get User Collections
```http
GET /api/profile/collection/{userId}?page=1&limit=20
Authorization: Bearer <token>
```

#### Update NFT Privacy
```http
PATCH /api/profile/nft/privacy
Authorization: Bearer <token>
Content-Type: application/json

[
  {
    "nftId": 123,
    "isVisible": true
  }
]
```

#### Update NFT Sale Information
```http
PATCH /api/profile/nft/{nftId}/sale
Authorization: Bearer <token>
Content-Type: application/json

{
  "isForSale": true,
  "price": 1.5,
  "currency": "ETH"
}
```

### Wallet Management (`/api/profile`)

#### Add Wallet
```http
POST /api/profile/wallet
Authorization: Bearer <token>
Content-Type: application/json

{
  "walletAddress": "0x1234567890abcdef",
  "chain": "eth",
  "walletName": "My Ethereum Wallet"
}
```

#### Get User Wallets
```http
GET /api/profile/wallet
Authorization: Bearer <token>
```

#### Delete Wallet
```http
DELETE /api/profile/wallet/{walletId}
Authorization: Bearer <token>
```

### Social Features (`/api/profile`)

#### Follow User
```http
POST /api/profile/follow/{userId}
Authorization: Bearer <token>
```

#### Unfollow User
```http
DELETE /api/profile/follow/{userId}
Authorization: Bearer <token>
```

#### Get Followers
```http
GET /api/profile/followers/{userId}?page=1
Authorization: Bearer <token>
```

#### Get Following
```http
GET /api/profile/followings/{userId}?page=1
Authorization: Bearer <token>
```

#### View User Profile
```http
POST /api/profile/view/{userId}
Authorization: Bearer <token>
```

### Experience Management (`/api/profile`)

#### Add Experience
```http
POST /api/profile/experience
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Software Engineer",
  "company": "Tech Company",
  "description": "Worked on blockchain projects",
  "startDate": "2023-01-01",
  "endDate": "2023-12-31",
  "isCurrent": false
}
```

#### Update Experience
```http
PATCH /api/profile/experience/{experienceId}
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Senior Software Engineer",
  "company": "Tech Company",
  "description": "Led blockchain development team"
}
```

#### Delete Experience
```http
DELETE /api/profile/experience/{experienceId}
Authorization: Bearer <token>
```

### Favorite NFTs (`/api/profile`)

#### Update Favorite NFTs
```http
PATCH /api/profile/fav-nft
Authorization: Bearer <token>
Content-Type: application/json

[
  {
    "nftId": 123
  },
  {
    "nftId": 456
  }
]
```

#### Get Favorite NFTs
```http
GET /api/profile/fav-nft/{userId}
Authorization: Bearer <token>
```

#### Remove Favorite NFT
```http
DELETE /api/profile/fav-nft
Authorization: Bearer <token>
Content-Type: application/json

{
  "data": [123, 456]
}
```

### Featured Items (`/api/profile`)

#### Update Featured Items
```http
PATCH /api/profile/featured-item
Authorization: Bearer <token>
Content-Type: application/json

{
  "featureItems": [
    {
      "id": 123,
      "type": "nft"
    },
    {
      "id": 456,
      "type": "collection"
    }
  ]
}
```

#### Get Featured Items
```http
GET /api/profile/featured/{userId}
Authorization: Bearer <token>
```

### User Statistics (`/api/profile`)

#### Get User Statistics
```http
GET /api/profile/stat/{userId}
Authorization: Bearer <token>
```

**Response:**
```json
{
  "message": "Success",
  "data": {
    "followersCount": 150,
    "followingCount": 75,
    "nftCount": 25,
    "collectionCount": 5,
    "netWorth": 2.5,
    "badgesCount": 3
  }
}
```

### Search (`/api/search`)

#### Search Users
```http
GET /api/search/user?query=username&page=1
Authorization: Bearer <token>
```

#### Search NFTs
```http
GET /api/search/nft?query=nft-name&next=cursor
Authorization: Bearer <token>
```

#### Search Collections
```http
GET /api/search/collection?query=collection-name&next=cursor
Authorization: Bearer <token>
```

### Discovery (`/api/discover`)

#### Get Top NFT Holders
```http
GET /api/discover/top-nft?page=1&userPath=All
Authorization: Bearer <token>
```

#### Get Highest Valued NFTs
```http
GET /api/discover/highest-valued-nft?page=1&userPath=All
Authorization: Bearer <token>
```

#### Get NFT Rankings
```http
GET /api/discover/nft-ranking?page=1&userPath=All
Authorization: Bearer <token>
```

### Notifications (`/api/notification`)

#### Get Notifications
```http
GET /api/notification?page=1
Authorization: Bearer <token>
```

#### Get Unread Count
```http
GET /api/notification/count
Authorization: Bearer <token>
```

#### Mark Notification as Read
```http
PATCH /api/notification/view/{notificationId}
Authorization: Bearer <token>
```

#### Mark All Notifications as Read
```http
PATCH /api/notification/view
Authorization: Bearer <token>
```

### Home Feed (`/api/home`)

#### Get Users
```http
GET /api/home/users
Authorization: Bearer <token>
```

#### Get NFTs
```http
GET /api/home/nfts
Authorization: Bearer <token>
```

#### Get NFTs from Wallet
```http
POST /api/home/wallet
Authorization: Bearer <token>
Content-Type: application/json

{
  "walletAddress": "0x1234567890abcdef",
  "chain": "eth"
}
```

### Asset Management (`/api/asset`)

#### Get Collection Details
```http
GET /api/asset/collection/{collectionId}
Authorization: Bearer <token>
```

#### Get Collection Tokens
```http
GET /api/asset/collection/{collectionId}/tokens?page=1&limit=20
Authorization: Bearer <token>
```

#### Get Token Details
```http
GET /api/asset/token/{tokenId}
Authorization: Bearer <token>
```

#### Get Token Transactions
```http
GET /api/asset/token/{tokenId}/transactions?page=1&limit=20
Authorization: Bearer <token>
```

#### Get Owner Distribution
```http
GET /api/asset/collection/{collectionId}/owners
Authorization: Bearer <token>
```

### Badge System (`/api/badge`)

#### Get User Badges
```http
GET /api/badge/{userId}?page=1
Authorization: Bearer <token>
```

#### Get Blue Chip NFTs
```http
GET /api/badge/blue-chip
Authorization: Bearer <token>
```

### Newsletter (`/api/newsletter`)

#### Subscribe to Newsletter
```http
POST /api/newsletter/subscribe
Content-Type: application/json

{
  "email": "user@example.com"
}
```

### Feedback (`/api/feedback`)

#### Submit Feedback
```http
POST /api/feedback
Authorization: Bearer <token>
Content-Type: application/json

{
  "subject": "Bug Report",
  "message": "Description of the issue",
  "type": "bug"
}
```

### Webhooks (`/api/webhook`)

#### Add NFT Activity
```http
POST /api/webhook/nft-activity
Content-Type: application/json

{
  "nftId": 123,
  "activityType": "transfer",
  "fromAddress": "0x123...",
  "toAddress": "0x456...",
  "transactionHash": "0x789...",
  "blockNumber": 12345678
}
```

### Developer Tools (`/api/dev`)

#### Token Filter
```http
GET /api/dev/token-filter?chain=eth&type=erc721
Authorization: Bearer <developer-token>
```

#### Core Token Filter
```http
GET /api/dev/core-token-filter?chain=eth
Authorization: Bearer <developer-token>
```

### Path Management (`/api/path`)

#### Get Paths
```http
GET /api/path
Authorization: Bearer <token>
```

#### Get Path by ID
```http
GET /api/path/{pathId}
Authorization: Bearer <token>
```

#### Create Path (Core Only)
```http
POST /api/path
Authorization: Bearer <developer-token>
Content-Type: application/json

{
  "name": "Path Name",
  "description": "Path Description",
  "type": "user"
}
```

### Wallet Management (`/api/wallet`)

#### Get Wallets
```http
GET /api/wallet
Authorization: Bearer <token>
```

#### Get Wallet by ID
```http
GET /api/wallet/{walletId}
Authorization: Bearer <token>
```

#### Create Wallet (Core Only)
```http
POST /api/wallet
Authorization: Bearer <developer-token>
Content-Type: application/json

{
  "address": "0x1234567890abcdef",
  "chain": "eth",
  "name": "Wallet Name"
}
```

## 📊 Data Models

### User Data Model
```json
{
  "userId": "guid",
  "email": "user@example.com",
  "username": "username",
  "displayName": "Display Name",
  "bio": "User bio",
  "location": "Location",
  "profileImage": "https://example.com/profile.jpg",
  "coverImage": "https://example.com/cover.jpg",
  "isVerified": true,
  "createdDate": "2023-01-01T00:00:00Z",
  "updatedDate": "2023-01-01T00:00:00Z"
}
```

### NFT Data Model
```json
{
  "nftId": 123,
  "tokenId": "12345",
  "contractAddress": "0x1234567890abcdef",
  "name": "NFT Name",
  "description": "NFT Description",
  "imageUrl": "https://example.com/nft.jpg",
  "chain": "eth",
  "collection": {
    "collectionId": 456,
    "name": "Collection Name",
    "description": "Collection Description"
  },
  "owner": {
    "userId": "guid",
    "username": "username",
    "walletAddress": "0x1234567890abcdef"
  },
  "value": 1.5,
  "currency": "ETH",
  "isVisible": true,
  "isForSale": false,
  "createdDate": "2023-01-01T00:00:00Z"
}
```

### Collection Data Model
```json
{
  "collectionId": 456,
  "name": "Collection Name",
  "description": "Collection Description",
  "imageUrl": "https://example.com/collection.jpg",
  "chain": "eth",
  "contractAddress": "0x1234567890abcdef",
  "totalSupply": 10000,
  "floorPrice": 0.5,
  "volumeTraded": 100.0,
  "ownersCount": 2500,
  "createdDate": "2023-01-01T00:00:00Z"
}
```

### Transaction Data Model
```json
{
  "transactionId": "guid",
  "transactionHash": "0x1234567890abcdef",
  "nftId": 123,
  "fromAddress": "0x1234567890abcdef",
  "toAddress": "0x9876543210fedcba",
  "value": 1.5,
  "currency": "ETH",
  "transactionType": "transfer",
  "blockNumber": 12345678,
  "gasUsed": 21000,
  "gasPrice": 20000000000,
  "createdDate": "2023-01-01T00:00:00Z"
}
```

### Notification Data Model
```json
{
  "notificationId": "guid",
  "userId": "guid",
  "initiatorId": "guid",
  "type": "follow",
  "title": "New Follower",
  "message": "username started following you",
  "isRead": false,
  "data": {
    "initiatorUsername": "username",
    "initiatorProfileImage": "https://example.com/profile.jpg"
  },
  "createdDate": "2023-01-01T00:00:00Z"
}
```

## 🔄 Response Format

### Success Response
```json
{
  "message": "Success",
  "data": { /* response data */ },
  "cursor": "next-page-cursor", // for paginated responses
  "rankData": { /* ranking information */ } // for ranking endpoints
}
```

### Error Response
```json
{
  "message": "Error description",
  "statusCode": 400,
  "success": false
}
```

### Pagination
Most list endpoints support pagination with the following parameters:
- `page`: Page number (1-based)
- `limit`: Items per page (default: 20, max: 100)
- `next`: Cursor for next page (for cursor-based pagination)

## 🚨 Error Codes

| Status Code | Description |
|-------------|-------------|
| 200 | Success |
| 400 | Bad Request - Invalid input |
| 401 | Unauthorized - Invalid or missing token |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource doesn't exist |
| 409 | Conflict - Resource already exists |
| 422 | Unprocessable Entity - Validation error |
| 500 | Internal Server Error |

## 🔧 Rate Limiting

The API implements rate limiting to ensure fair usage:
- **Standard endpoints**: 100 requests per minute per user
- **Search endpoints**: 50 requests per minute per user
- **Authentication endpoints**: 10 requests per minute per IP

Rate limit headers are included in responses:
```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1640995200
```

## 📡 Real-time Features

### SignalR Hub
The API provides real-time notifications through SignalR:

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notification", {
        accessTokenFactory: () => "your-jwt-token"
    })
    .build();

connection.start().then(() => {
    console.log("Connected to SignalR hub");
});

connection.on("ReceiveNotification", (notification) => {
    console.log("New notification:", notification);
});
```

### WebSocket Events
- `ReceiveNotification`: New notification received
- `PortfolioUpdate`: Portfolio value changed
- `NftActivity`: NFT activity occurred

## 🧪 Testing

### Postman Collection
A Postman collection is available for testing all endpoints:
- Import the collection from `/docs/postman/Ovation-API.postman_collection.json`
- Set up environment variables for different environments
- Use the pre-request scripts for authentication

### API Testing
```bash
# Test authentication
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password"}'

# Test protected endpoint
curl -X GET http://localhost:8080/api/profile \
  -H "Authorization: Bearer <token>"
```

## 📚 Additional Resources

- [Swagger Documentation](http://localhost:8080/swagger) - Interactive API documentation
- [Installation Guide](INSTALLATION.md) - Setup instructions
- [Environment Configuration](ENVIRONMENT_CONFIGURATION.md) - Configuration options
- [Architecture Overview](ARCHITECTURE.md) - System architecture

---

**Need help?** Check the [Troubleshooting Guide](TROUBLESHOOTING.md) or create an issue in the GitHub repository.
