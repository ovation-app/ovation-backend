# Docker Setup Guide

## 📋 Overview

This guide provides comprehensive instructions for setting up and running the Ovation Backend using Docker. Docker provides a consistent environment across development, staging, and production deployments.

## 🐳 Prerequisites

### Required Software
- **Docker Desktop** - [Download here](https://www.docker.com/products/docker-desktop/)
- **Docker Compose** - Included with Docker Desktop
- **Git** - [Download here](https://git-scm.com/downloads)

### System Requirements
- **RAM**: Minimum 4GB, Recommended 8GB+
- **Disk Space**: Minimum 10GB free space
- **CPU**: 2+ cores recommended

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/ovation-app/ovation-backend.git
cd ovation-backend
```

### 2. Environment Setup
```bash
# Copy environment template
cp env.example .env

# Edit environment variables
nano .env
```

### 3. Start Services
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f ovation-api
```

### 4. Verify Setup
- **API**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health

## 🔧 Configuration

### Environment Variables

#### Required Variables
```bash
# Database
MYSQL_ROOT_PASSWORD=rootpassword
MYSQL_DATABASE=ovation_db
MYSQL_USER=ovation_user
MYSQL_PASSWORD=ovation_password

# Application
OVATION_KEY=your-super-secret-jwt-key-here
ASPNETCORE_ENVIRONMENT=Development
```

#### Optional Variables
```bash
# External APIs
NFTSCAN_KEY=your-nftscan-api-key
ALCHEMY_KEY=your-alchemy-api-key
MAGICEDEN_KEY=your-magiceden-api-key

# Social APIs
X_CONSUMER_KEY=your-x-consumer-key
X_CONSUMER_SECRET=your-x-consumer-secret

# Monitoring
SENTRY_DNS=your-sentry-dsn
```

### Docker Compose Services

#### MySQL Database
```yaml
mysql:
  image: mysql:8.0
  environment:
    MYSQL_ROOT_PASSWORD: ${MYSQL_ROOT_PASSWORD}
    MYSQL_DATABASE: ${MYSQL_DATABASE}
    MYSQL_USER: ${MYSQL_USER}
    MYSQL_PASSWORD: ${MYSQL_PASSWORD}
  ports:
    - "3306:3306"
  volumes:
    - mysql_data:/var/lib/mysql
```

#### Redis Cache
```yaml
redis:
  image: redis:7-alpine
  ports:
    - "6379:6379"
  volumes:
    - redis_data:/data
```

#### Ovation API
```yaml
ovation-api:
  build: .
  ports:
    - "8080:8080"
  environment:
    - OVATION_DB=Server=mysql;Database=${MYSQL_DATABASE};User=${MYSQL_USER};Password=${MYSQL_PASSWORD};Port=3306;
    - OVATION_KEY=${OVATION_KEY}
  depends_on:
    - mysql
    - redis
```

## 🏗️ Development Setup

### Local Development with Docker

#### 1. Start Development Environment
```bash
# Start with development configuration
docker-compose up -d

# View real-time logs
docker-compose logs -f ovation-api
```

#### 2. Database Management
```bash
# Connect to MySQL
docker-compose exec mysql mysql -u root -p

# Run migrations
docker-compose exec ovation-api dotnet ef database update

# Backup database
docker-compose exec mysql mysqldump -u root -p ovation_db > backup.sql
```

#### 3. API Development
```bash
# Restart API service
docker-compose restart ovation-api

# View API logs
docker-compose logs ovation-api

# Execute commands in container
docker-compose exec ovation-api dotnet --version
```

### Hot Reload Development

#### Option 1: Volume Mounting
```yaml
# docker-compose.override.yml
version: '3.8'
services:
  ovation-api:
    volumes:
      - .:/src
      - /src/bin
      - /src/obj
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - DOTNET_WATCH=true
```

#### Option 2: Local Development
```bash
# Run database only
docker-compose up -d mysql redis

# Run API locally
dotnet run --project Ovation.WebAPI
```

## 🚀 Production Deployment

### Production Configuration

#### 1. Production Environment
```bash
# Use production compose file
docker-compose -f docker-compose.yml -f docker-compose.production.yml up -d
```

#### 2. Environment Variables
```bash
# Production .env
MYSQL_ROOT_PASSWORD=super-secure-production-password
MYSQL_PASSWORD=super-secure-production-password
OVATION_KEY=super-secure-production-jwt-key-minimum-64-characters
ASPNETCORE_ENVIRONMENT=Production
SENTRY_DNS=https://your-key@sentry.io/project-id
```

#### 3. SSL Configuration
```bash
# Generate SSL certificates
mkdir -p nginx/ssl
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/private.key \
  -out nginx/ssl/certificate.crt
```

### Production Features

#### Load Balancing
```yaml
# Multiple API instances
ovation-api:
  deploy:
    replicas: 3
    resources:
      limits:
        memory: 1G
        cpus: '1.0'
```

#### Health Checks
```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
  timeout: 20s
  retries: 10
  start_period: 40s
```

#### Resource Limits
```yaml
deploy:
  resources:
    limits:
      memory: 1G
      cpus: '1.0'
    reservations:
      memory: 512M
      cpus: '0.5'
```

## 📊 Monitoring Setup

### Prometheus & Grafana

#### 1. Enable Monitoring
```bash
# Start with monitoring profile
docker-compose --profile monitoring up -d
```

#### 2. Access Dashboards
- **Prometheus**: http://localhost:9090
- **Grafana**: http://localhost:3000 (admin/admin)

#### 3. Configure Metrics
```yaml
# prometheus.yml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'ovation-api'
    static_configs:
      - targets: ['ovation-api:8080']
```

### Log Management

#### Centralized Logging
```yaml
# Add to docker-compose.yml
logging:
  driver: "json-file"
  options:
    max-size: "10m"
    max-file: "3"
```

#### Log Aggregation
```bash
# View all logs
docker-compose logs

# View specific service logs
docker-compose logs ovation-api

# Follow logs in real-time
docker-compose logs -f ovation-api
```

## 🔧 Troubleshooting

### Common Issues

#### 1. Port Conflicts
```bash
# Check port usage
netstat -tulpn | grep :8080

# Change ports in .env
API_PORT=8081
MYSQL_PORT=3307
```

#### 2. Database Connection Issues
```bash
# Check MySQL status
docker-compose exec mysql mysqladmin ping

# Reset database
docker-compose down -v
docker-compose up -d
```

#### 3. Memory Issues
```bash
# Check resource usage
docker stats

# Increase Docker memory limit
# Docker Desktop -> Settings -> Resources -> Memory
```

#### 4. Build Issues
```bash
# Clean build
docker-compose build --no-cache

# Rebuild specific service
docker-compose build ovation-api
```

### Debugging Commands

#### Container Management
```bash
# List containers
docker-compose ps

# Execute commands in container
docker-compose exec ovation-api bash

# View container logs
docker-compose logs ovation-api

# Restart service
docker-compose restart ovation-api
```

#### Database Management
```bash
# Connect to database
docker-compose exec mysql mysql -u root -p

# Backup database
docker-compose exec mysql mysqldump -u root -p ovation_db > backup.sql

# Restore database
docker-compose exec -T mysql mysql -u root -p ovation_db < backup.sql
```

#### Network Debugging
```bash
# Check network connectivity
docker-compose exec ovation-api ping mysql

# Inspect network
docker network inspect ovation-backend_ovation-network
```

## 🔒 Security Considerations

### Production Security

#### 1. Environment Variables
```bash
# Use Docker secrets for sensitive data
echo "super-secret-password" | docker secret create mysql_password -
```

#### 2. Network Security
```yaml
# Restrict network access
networks:
  ovation-network:
    driver: bridge
    internal: true
```

#### 3. Container Security
```yaml
# Run as non-root user
user: "1000:1000"

# Read-only filesystem
read_only: true
```

### SSL/TLS Configuration

#### Nginx SSL Setup
```nginx
server {
    listen 443 ssl;
    ssl_certificate /etc/nginx/ssl/certificate.crt;
    ssl_certificate_key /etc/nginx/ssl/private.key;
    
    location / {
        proxy_pass http://ovation-api:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

## 📈 Performance Optimization

### Resource Optimization

#### 1. Memory Management
```yaml
# Set memory limits
deploy:
  resources:
    limits:
      memory: 1G
    reservations:
      memory: 512M
```

#### 2. CPU Optimization
```yaml
# CPU limits
deploy:
  resources:
    limits:
      cpus: '1.0'
    reservations:
      cpus: '0.5'
```

#### 3. Connection Pooling
```yaml
# Database connection pooling
environment:
  - MYSQL_CONNECTION_POOL_SIZE=20
  - MYSQL_CONNECTION_TIMEOUT=30
```

### Scaling

#### Horizontal Scaling
```bash
# Scale API instances
docker-compose up -d --scale ovation-api=3
```

#### Load Balancing
```nginx
upstream ovation_api {
    server ovation-api_1:8080;
    server ovation-api_2:8080;
    server ovation-api_3:8080;
}
```

## 🧪 Testing with Docker

### Integration Testing
```bash
# Run tests in container
docker-compose exec ovation-api dotnet test

# Run specific test
docker-compose exec ovation-api dotnet test --filter "TestCategory=Integration"
```

### Test Database
```yaml
# Add test database service
test-db:
  image: mysql:8.0
  environment:
    MYSQL_DATABASE: ovation_test_db
    MYSQL_ROOT_PASSWORD: testpassword
  profiles:
    - testing
```

## 📚 Additional Resources

### Docker Commands Reference
```bash
# Start services
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f

# Execute commands
docker-compose exec service_name command

# Scale services
docker-compose up -d --scale service_name=3

# Build images
docker-compose build

# Pull latest images
docker-compose pull
```

### Useful Scripts
```bash
# Start development environment
./scripts/dev-start.sh

# Stop all services
./scripts/dev-stop.sh

# Reset database
./scripts/reset-db.sh

# Backup database
./scripts/backup-db.sh
```

---

**Need help with Docker setup?** Check the [Troubleshooting](#-troubleshooting) section or create an issue in the GitHub repository.
