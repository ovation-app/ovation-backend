# Environment Configuration Guide

## 📋 Overview

This document provides comprehensive information about all environment variables and configuration options used in the Ovation backend application. Proper configuration is essential for the application to function correctly across different environments.

## 🔧 Environment Variables

### Core Application Variables

#### Database Configuration
```bash
# MySQL Database Connection String
OVATION_DB=Server=localhost;Database=ovation_db;User=ovation_user;Password=your_password;Port=3306;
```

**Description**: Primary database connection string for MySQL database
**Required**: Yes
**Format**: Standard MySQL connection string
**Example**: `Server=localhost;Database=ovation_db;User=ovation_user;Password=secure_password123;Port=3306;`

#### JWT Authentication
```bash
# JWT Secret Key for token signing
OVATION_KEY=your-super-secret-jwt-key-here
```

**Description**: Secret key used for signing JWT tokens
**Required**: Yes
**Security**: Must be a strong, unique key (minimum 32 characters)
**Example**: `ovation-super-secret-jwt-key-2024-production-ready`

#### Email Configuration
```bash
# Email service password (Gmail App Password)
EMAIL_KEY=your-email-app-password
```

**Description**: Password for the email service (Gmail App Password)
**Required**: Yes (for email functionality)
**Security**: Use Gmail App Password, not regular password
**Example**: `abcd efgh ijkl mnop`

### External API Integrations

#### NFTScan API
```bash
# NFTScan API Key for multi-chain NFT data
NFTSCAN_KEY=your-nftscan-api-key
```

**Description**: API key for NFTScan service (primary NFT data provider)
**Required**: Yes (for NFT functionality)
**Rate Limit**: Varies by plan
**Documentation**: [NFTScan API Docs](https://docs.nftscan.com/)

#### Alchemy API
```bash
# Alchemy API Key for enhanced Ethereum data
ALCHEMY_KEY=your-alchemy-api-key
```

**Description**: API key for Alchemy service (Ethereum blockchain data)
**Required**: No (optional enhancement)
**Rate Limit**: Varies by plan
**Documentation**: [Alchemy API Docs](https://docs.alchemy.com/)

#### Magic Eden API
```bash
# Magic Eden API Key for Solana NFT marketplace
MAGICEDEN_KEY=your-magiceden-api-key
```

**Description**: API key for Magic Eden (Solana NFT marketplace)
**Required**: No (optional for Solana NFTs)
**Rate Limit**: Varies by plan
**Documentation**: [Magic Eden API Docs](https://docs.magiceden.io/)

#### DappRadar API
```bash
# DappRadar API Key for DeFi and NFT analytics
DAPP_RADAR_KEY=your-dappradar-api-key
```

**Description**: API key for DappRadar (DeFi and NFT analytics)
**Required**: No (optional analytics)
**Rate Limit**: Varies by plan
**Documentation**: [DappRadar API Docs](https://docs.dappradar.com/)

#### Mintify API
```bash
# Mintify API Key for NFT marketplace data
MINTIFY_KEY=your-mintify-api-key
```

**Description**: API key for Mintify (NFT marketplace data)
**Required**: No (optional marketplace data)
**Rate Limit**: Varies by plan
**Documentation**: [Mintify API Docs](https://docs.mintify.xyz/)

#### Moralis API
```bash
# Moralis API Key for blockchain data
MORALIS_KEY=your-moralis-api-key
```

**Description**: API key for Moralis (blockchain data provider)
**Required**: No (optional blockchain data)
**Rate Limit**: Varies by plan
**Documentation**: [Moralis API Docs](https://docs.moralis.io/)

### Social Media Integrations

#### X (Twitter) API Configuration
```bash
# X API Consumer Key
X_CONSUMER_KEY=your-x-consumer-key

# X API Consumer Secret
X_CONSUMER_SECRET=your-x-consumer-secret

# X API Access Token
X_ACCESS_TOKEN=your-x-access-token

# X API Access Token Secret
X_ACCESS_TOKEN_SECRET=your-x-access-token-secret

# X Bearer Token
X_KEY=your-x-bearer-token
```

**Description**: Complete X (Twitter) API configuration for social features
**Required**: No (optional social features)
**Rate Limit**: Varies by API tier
**Documentation**: [X API Docs](https://developer.twitter.com/en/docs)

### AI Services

#### Google AI Model API
```bash
# Google AI Model API Key
GOOGLE_AI_MODEL_API_KEY=your-google-ai-key

# Google AI Model Name
GOOGLE_AI_MODEL=gemini-pro
```

**Description**: Google AI service for content generation and analysis
**Required**: No (optional AI features)
**Rate Limit**: Varies by plan
**Documentation**: [Google AI Docs](https://ai.google.dev/docs)

### Monitoring & Observability

#### Sentry Integration
```bash
# Sentry DSN for error tracking
SENTRY_DNS=your-sentry-dsn
```

**Description**: Sentry DSN for error tracking and performance monitoring
**Required**: No (optional monitoring)
**Format**: `https://your-key@sentry.io/project-id`
**Documentation**: [Sentry Docs](https://docs.sentry.io/)

### Application Settings

#### Port Configuration
```bash
# Application port (default: 8080)
PORT=8080
```

**Description**: Port number for the application to listen on
**Required**: No (defaults to 8080)
**Range**: 1024-65535
**Example**: `8080`, `5000`, `3000`

#### Environment Type
```bash
# Application environment
ASPNETCORE_ENVIRONMENT=Development
```

**Description**: ASP.NET Core environment setting
**Required**: No (defaults to Production)
**Values**: `Development`, `Staging`, `Production`
**Impact**: Affects logging, error handling, and feature flags

## 📁 Configuration Files

### appsettings.json
```json
{
  "AppSettings": {
    "Token": "This is a custom Secret Key for ovationtoken"
  },
  "ConnectionStrings": {
    // Database connection strings go here
  },
  "Jwt": {
    "Issuer": "https://ovation.network",
    "LifeTime": "1"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ovation_db_dev;User=ovation_user;Password=dev_password;"
  }
}
```

### appsettings.Production.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=ovation_db;User=ovation_user;Password=secure_prod_password;"
  },
  "AllowedHosts": "ovation.network,www.ovation.network"
}
```

## 🔒 Security Considerations

### Environment Variable Security

#### Best Practices
1. **Never commit sensitive data** to version control
2. **Use strong, unique passwords** for all services
3. **Rotate API keys regularly** (every 90 days)
4. **Use environment-specific configurations**
5. **Implement proper access controls**

#### Sensitive Data Handling
```bash
# ❌ Never do this
OVATION_KEY=simple-password

# ✅ Do this instead
OVATION_KEY=ovation-super-secret-jwt-key-2024-production-ready-with-random-chars-xyz789
```

### API Key Management

#### Development Environment
- Use test/sandbox API keys when available
- Implement rate limiting to avoid quota exhaustion
- Use mock data for external services

#### Production Environment
- Use production API keys with proper permissions
- Implement circuit breakers for external services
- Monitor API usage and costs
- Set up alerts for quota limits

## 🌍 Environment-Specific Configurations

### Development Environment
```bash
# Development-specific settings
ASPNETCORE_ENVIRONMENT=Development
OVATION_DB=Server=localhost;Database=ovation_db_dev;User=dev_user;Password=dev_password;
OVATION_KEY=dev-jwt-key-not-for-production
PORT=8080

# Optional APIs (can use mock data)
NFTSCAN_KEY=dev-nftscan-key
ALCHEMY_KEY=dev-alchemy-key
```

### Staging Environment
```bash
# Staging-specific settings
ASPNETCORE_ENVIRONMENT=Staging
OVATION_DB=Server=staging-db;Database=ovation_db_staging;User=staging_user;Password=staging_password;
OVATION_KEY=staging-jwt-key-different-from-prod
PORT=8080

# Real APIs with staging endpoints
NFTSCAN_KEY=staging-nftscan-key
ALCHEMY_KEY=staging-alchemy-key
```

### Production Environment
```bash
# Production-specific settings
ASPNETCORE_ENVIRONMENT=Production
OVATION_DB=Server=prod-db-cluster;Database=ovation_db;User=prod_user;Password=super-secure-prod-password;
OVATION_KEY=production-jwt-key-ultra-secure-random-string
PORT=8080

# Production APIs
NFTSCAN_KEY=prod-nftscan-key
ALCHEMY_KEY=prod-alchemy-key
SENTRY_DNS=https://your-key@sentry.io/project-id
```

## 🐳 Docker Configuration

### Environment Variables in Docker
```dockerfile
# Dockerfile
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080
```

### Docker Compose Environment
```yaml
# docker-compose.yml
services:
  ovation-api:
    environment:
      - OVATION_DB=Server=mysql;Database=ovation_db;User=ovation_user;Password=ovation_password;
      - OVATION_KEY=your-super-secret-jwt-key-here
      - ASPNETCORE_ENVIRONMENT=Development
      - PORT=8080
```

### Docker Secrets (Production)
```yaml
# docker-compose.prod.yml
services:
  ovation-api:
    environment:
      - OVATION_DB_FILE=/run/secrets/ovation_db
      - OVATION_KEY_FILE=/run/secrets/ovation_key
    secrets:
      - ovation_db
      - ovation_key

secrets:
  ovation_db:
    file: ./secrets/ovation_db.txt
  ovation_key:
    file: ./secrets/ovation_key.txt
```

## 🔍 Configuration Validation

### Startup Validation
The application validates critical configuration on startup:

```csharp
// Example validation in Program.cs
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OVATION_DB")))
{
    throw new InvalidOperationException("OVATION_DB environment variable is required");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OVATION_KEY")))
{
    throw new InvalidOperationException("OVATION_KEY environment variable is required");
}
```

### Health Checks
```csharp
// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

## 📊 Configuration Monitoring

### Logging Configuration
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "Ovation": "Debug"
    }
  }
}
```

### Performance Monitoring
- Monitor API response times
- Track database connection health
- Monitor external API usage
- Alert on configuration errors

## 🛠️ Configuration Tools

### Environment Variable Management
- **Development**: Use `.env` files with dotenv
- **Staging**: Use environment-specific configuration files
- **Production**: Use secure secret management (Azure Key Vault, AWS Secrets Manager)

### Configuration Validation Tools
```bash
# Validate configuration
dotnet run --project Ovation.WebAPI --validate-config

# Test database connection
dotnet ef database update --project Ovation.WebAPI --dry-run
```

## 📚 Additional Resources

### External Service Documentation
- [NFTScan API](https://docs.nftscan.com/)
- [Alchemy API](https://docs.alchemy.com/)
- [Magic Eden API](https://docs.magiceden.io/)
- [DappRadar API](https://docs.dappradar.com/)
- [X API](https://developer.twitter.com/en/docs)
- [Google AI](https://ai.google.dev/docs)
- [Sentry](https://docs.sentry.io/)

### Configuration Best Practices
- Use environment-specific configuration files
- Implement proper secret management
- Validate configuration on startup
- Monitor configuration changes
- Document all configuration options

---

**Need help with configuration?** Check the [Installation Guide](INSTALLATION.md) or create an issue in the GitHub repository.
