# Ovation Backend - Installation Guide

## 📋 Prerequisites

Before installing the Ovation backend, ensure you have the following software installed on your system:

### Required Software
- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MySQL 8.0+** - [Download here](https://dev.mysql.com/downloads/mysql/)
- **Docker Desktop** (Optional) - [Download here](https://www.docker.com/products/docker-desktop/)
- **Git** - [Download here](https://git-scm.com/downloads)

### Recommended Tools
- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- **MySQL Workbench** or **DBeaver** for database management
- **Postman** or **Insomnia** for API testing

## 🚀 Installation Methods

### Method 1: Local Development Setup

#### Step 1: Clone the Repository
```bash
git clone https://github.com/your-org/ovation-backend.git
cd ovation-backend
```

#### Step 2: Restore Dependencies
```bash
dotnet restore
```

#### Step 3: Database Setup
1. **Create MySQL Database**
   ```sql
   CREATE DATABASE ovation_db;
   CREATE USER 'ovation_user'@'localhost' IDENTIFIED BY 'your_password';
   GRANT ALL PRIVILEGES ON ovation_db.* TO 'ovation_user'@'localhost';
   FLUSH PRIVILEGES;
   ```

2. **Update Connection String**
   Create a `appsettings.Development.json` file in the `Ovation.WebAPI` directory:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ovation_db;User=ovation_user;Password=your_password;"
     }
   }
   ```

#### Step 4: Environment Variables
Create a `.env` file in the project root or set environment variables:

```bash
# Database
OVATION_DB=Server=localhost;Database=ovation_db;User=ovation_user;Password=your_password;

# JWT Configuration
OVATION_KEY=your-super-secret-jwt-key-here

# Email Configuration
EMAIL_KEY=your-email-app-password

# External API Keys (Optional for basic functionality)
NFTSCAN_KEY=your-nftscan-api-key
ALCHEMY_KEY=your-alchemy-api-key
MAGICEDEN_KEY=your-magiceden-api-key
DAPP_RADAR_KEY=your-dappradar-api-key
MINTIFY_KEY=your-mintify-api-key

# Social APIs (Optional)
X_CONSUMER_KEY=your-x-consumer-key
X_CONSUMER_SECRET=your-x-consumer-secret
X_ACCESS_TOKEN=your-x-access-token
X_ACCESS_TOKEN_SECRET=your-x-access-token-secret
X_KEY=your-x-bearer-token

# AI Services (Optional)
GOOGLE_AI_MODEL_API_KEY=your-google-ai-key
GOOGLE_AI_MODEL=gemini-pro

# Monitoring (Optional)
SENTRY_DNS=your-sentry-dsn
```

#### Step 5: Run Database Migrations
```bash
cd Ovation.WebAPI
dotnet ef database update
```

#### Step 6: Run the Application
```bash
dotnet run --project Ovation.WebAPI
```

The application will be available at:
- **API**: http://localhost:8080
- **Swagger Documentation**: http://localhost:8080/swagger

### Method 2: Docker Setup

#### Step 1: Clone and Navigate
```bash
git clone https://github.com/your-org/ovation-backend.git
cd ovation-backend
```

#### Step 2: Create Docker Compose File
Create a `docker-compose.yml` file in the project root:

```yaml
version: '3.8'

services:
  mysql:
    image: mysql:8.0
    container_name: ovation-mysql
    environment:
      MYSQL_ROOT_PASSWORD: rootpassword
      MYSQL_DATABASE: ovation_db
      MYSQL_USER: ovation_user
      MYSQL_PASSWORD: ovation_password
    ports:
      - "3306:3306"
    volumes:
      - mysql_data:/var/lib/mysql
    networks:
      - ovation-network

  redis:
    image: redis:7-alpine
    container_name: ovation-redis
    ports:
      - "6379:6379"
    networks:
      - ovation-network

  ovation-api:
    build: .
    container_name: ovation-api
    ports:
      - "8080:8080"
    environment:
      - OVATION_DB=Server=mysql;Database=ovation_db;User=ovation_user;Password=ovation_password;
      - OVATION_KEY=your-super-secret-jwt-key-here
      - ASPNETCORE_ENVIRONMENT=Development
    depends_on:
      - mysql
      - redis
    networks:
      - ovation-network

volumes:
  mysql_data:

networks:
  ovation-network:
    driver: bridge
```

#### Step 3: Build and Run
```bash
docker-compose up --build
```

## 🔧 Configuration

### Application Settings

#### JWT Configuration
```json
{
  "Jwt": {
    "Issuer": "https://ovation.network",
    "LifeTime": "1"
  }
}
```

#### CORS Configuration
The application is configured to allow all origins in development. For production, update the CORS policy in `Program.cs`.

#### Logging Configuration
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Database Configuration

#### Connection String Format
```
Server=server_address;Database=database_name;User=username;Password=password;Port=3306;
```

#### Entity Framework Configuration
The application uses Entity Framework Core with MySQL. Key configurations:
- **Provider**: Pomelo.EntityFrameworkCore.MySql
- **Migration Strategy**: Code First
- **Connection Pooling**: Enabled by default

## 🧪 Testing the Installation

### 1. Health Check
Visit http://localhost:8080/swagger to verify the API is running.

### 2. Database Connection Test
Check the database connection by running:
```bash
dotnet ef database update --project Ovation.WebAPI
```

### 3. API Endpoint Test
Test a basic endpoint:
```bash
curl http://localhost:8080/api/health
```

## 🐛 Troubleshooting

### Common Issues

#### 1. Database Connection Issues
**Error**: `Unable to connect to any of the specified MySQL hosts`

**Solutions**:
- Verify MySQL is running: `sudo systemctl status mysql`
- Check connection string format
- Ensure database and user exist
- Verify firewall settings

#### 2. Port Already in Use
**Error**: `Address already in use`

**Solutions**:
- Change port in `launchSettings.json`
- Kill process using port: `sudo lsof -ti:8080 | xargs kill -9`
- Use different port: `dotnet run --urls="http://localhost:5000"`

#### 3. Missing Environment Variables
**Error**: `Configuration value is null`

**Solutions**:
- Verify all required environment variables are set
- Check `.env` file format
- Ensure environment variables are loaded correctly

#### 4. Entity Framework Migration Issues
**Error**: `No database provider has been configured`

**Solutions**:
- Verify connection string is set
- Check Entity Framework configuration
- Run `dotnet ef database update` from correct directory

### Performance Issues

#### 1. Slow Database Queries
- Enable query logging in development
- Check database indexes
- Optimize Entity Framework queries

#### 2. High Memory Usage
- Monitor background job execution
- Check for memory leaks in SignalR connections
- Optimize caching strategies

## 📚 Additional Resources

### Development Tools
- **Swagger UI**: http://localhost:8080/swagger
- **Health Checks**: http://localhost:8080/health
- **SignalR Hub**: ws://localhost:8080/notification

### Useful Commands
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Clean solution
dotnet clean

# Update database
dotnet ef database update --project Ovation.WebAPI

# Add migration
dotnet ef migrations add MigrationName --project Ovation.WebAPI

# Remove migration
dotnet ef migrations remove --project Ovation.WebAPI
```

### Environment-Specific Setup

#### Development Environment
- Enable detailed logging
- Use local database
- Allow all CORS origins
- Enable Swagger UI

#### Production Environment
- Use production database
- Restrict CORS origins
- Disable Swagger UI
- Enable HTTPS redirection
- Configure proper logging levels

## 🔒 Security Considerations

### Environment Variables
- Never commit sensitive data to version control
- Use strong, unique passwords
- Rotate API keys regularly
- Use environment-specific configurations

### Database Security
- Use strong database passwords
- Limit database user permissions
- Enable SSL connections in production
- Regular security updates

### API Security
- Use HTTPS in production
- Implement rate limiting
- Validate all inputs
- Monitor for suspicious activity

---

**Need help?** Check the [Troubleshooting](#-troubleshooting) section or create an issue in the GitHub repository.
