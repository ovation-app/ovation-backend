# Contributing to Ovation Backend

## 🎯 Welcome Contributors!

Thank you for your interest in contributing to the Ovation Backend! This document provides guidelines and information for contributors to help make the contribution process smooth and effective.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Contributing Guidelines](#contributing-guidelines)
- [Pull Request Process](#pull-request-process)
- [Issue Reporting](#issue-reporting)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Documentation](#documentation)
- [Community](#community)

## 🤝 Code of Conduct

This project follows our [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior to [contact@ovation.network](mailto:contact@ovation.network).

## 🚀 Getting Started

### Prerequisites

Before contributing, ensure you have:

- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MySQL 8.0+** - [Download here](https://dev.mysql.com/downloads/mysql/)
- **Git** - [Download here](https://git-scm.com/downloads)
- **Docker Desktop** (Optional) - [Download here](https://www.docker.com/products/docker-desktop/)

### Recommended Tools

- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- **MySQL Workbench** or **Dbeaver** for database management
- **Postman** or **Insomnia** for API testing

## 🛠️ Development Setup

### 1. Fork and Clone

```bash
# Fork the repository on GitHub, then clone your fork
git clone https://github.com/YOUR_USERNAME/ovation-backend.git
cd ovation-backend

# Add upstream remote
git remote add upstream https://github.com/ovation-app/ovation-backend.git
```

### 2. Environment Setup

```bash
# Create environment file
cp .env.example .env

# Edit .env with your configuration
# See docs/ENVIRONMENT_CONFIGURATION.md for details
```

### 3. Database Setup

```bash
# Create MySQL database
mysql -u root -p
CREATE DATABASE ovation_db_dev;
CREATE USER 'ovation_dev'@'localhost' IDENTIFIED BY 'dev_password';
GRANT ALL PRIVILEGES ON ovation_db_dev.* TO 'ovation_dev'@'localhost';
FLUSH PRIVILEGES;
```

### 4. Run the Application

```bash
# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update --project Ovation.WebAPI

# Run the application
dotnet run --project Ovation.WebAPI
```

### 5. Verify Setup

- Visit http://localhost:8080/swagger to see the API documentation
- Test a basic endpoint to ensure everything is working

## 📝 Contributing Guidelines

### Types of Contributions

We welcome various types of contributions:

#### 🐛 Bug Fixes
- Fix existing issues
- Improve error handling
- Resolve performance issues

#### ✨ New Features
- Add new API endpoints
- Implement new blockchain integrations
- Add new social features
- Enhance existing functionality

#### 📚 Documentation
- Improve API documentation
- Add code comments
- Update setup guides
- Create tutorials

#### 🧪 Testing
- Add unit tests
- Improve test coverage
- Add integration tests
- Performance testing

#### 🔧 Infrastructure
- Docker improvements
- CI/CD enhancements
- Monitoring improvements
- Security enhancements

### Contribution Workflow

#### 1. Create a Branch

```bash
# Create a new branch for your feature/fix
git checkout -b feature/your-feature-name
# or
git checkout -b fix/issue-number-description
```

#### 2. Make Changes

- Follow the [coding standards](#coding-standards)
- Write tests for new functionality
- Update documentation as needed
- Ensure all tests pass

#### 3. Commit Changes

```bash
# Stage your changes
git add .

# Commit with descriptive message
git commit -m "feat: add new NFT search endpoint

- Add SearchNftQueryHandler
- Implement pagination support
- Add comprehensive tests
- Update API documentation

Closes #123"
```

#### 4. Push and Create PR

```bash
# Push your branch
git push origin feature/your-feature-name

# Create a Pull Request on GitHub
```

## 🔄 Pull Request Process

### Before Submitting

- [ ] **Code Quality**: Follow coding standards and best practices
- [ ] **Tests**: Add/update tests for new functionality
- [ ] **Documentation**: Update relevant documentation
- [ ] **Performance**: Consider performance implications
- [ ] **Security**: Review security implications
- [ ] **Breaking Changes**: Document any breaking changes

### PR Template

When creating a Pull Request, please include:

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Refactoring

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed
- [ ] Performance testing (if applicable)

## Checklist
- [ ] Code follows project coding standards
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] Tests pass locally
- [ ] No breaking changes (or documented)

## Related Issues
Closes #123
```

### Review Process

1. **Automated Checks**: CI/CD pipeline runs tests and checks
2. **Code Review**: Maintainers review the code
3. **Testing**: Manual testing by reviewers
4. **Approval**: At least one maintainer approval required
5. **Merge**: Squash and merge to main branch

### PR Guidelines

- **Keep PRs Small**: Focus on single features or fixes
- **Descriptive Titles**: Use clear, descriptive titles
- **Detailed Descriptions**: Explain what and why
- **Link Issues**: Reference related issues
- **Update Documentation**: Include documentation changes
- **Add Tests**: Include tests for new functionality

## 🐛 Issue Reporting

### Before Creating an Issue

1. **Search Existing Issues**: Check if the issue already exists
2. **Check Documentation**: Review relevant documentation
3. **Reproduce**: Ensure you can reproduce the issue
4. **Gather Information**: Collect relevant details

### Issue Templates

#### Bug Report
```markdown
**Describe the Bug**
A clear description of what the bug is.

**To Reproduce**
Steps to reproduce the behavior:
1. Go to '...'
2. Click on '....'
3. Scroll down to '....'
4. See error

**Expected Behavior**
What you expected to happen.

**Environment**
- OS: [e.g., Windows 10, macOS 12.0, Ubuntu 20.04]
- .NET Version: [e.g., 8.0.0]
- MySQL Version: [e.g., 8.0.32]
- Browser: [e.g., Chrome 91, Firefox 89]

**Additional Context**
Add any other context about the problem here.
```

#### Feature Request
```markdown
**Is your feature request related to a problem?**
A clear description of what the problem is.

**Describe the solution you'd like**
A clear description of what you want to happen.

**Describe alternatives you've considered**
A clear description of any alternative solutions.

**Additional context**
Add any other context or screenshots about the feature request.
```

## 📏 Coding Standards

### C# Coding Conventions

#### Naming Conventions
```csharp
// Classes: PascalCase
public class UserRepository { }

// Methods: PascalCase
public async Task<User> GetUserAsync(Guid userId) { }

// Properties: PascalCase
public string UserName { get; set; }

// Fields: camelCase with underscore prefix for private
private readonly IUserRepository _userRepository;

// Constants: PascalCase
public const string DefaultConnectionString = "...";

// Enums: PascalCase
public enum UserStatus { Active, Inactive, Suspended }
```

#### Code Organization
```csharp
// File organization
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ovation.Application.DTOs;
using Ovation.Application.Features.UserFeatures.Requests.Commands;

namespace Ovation.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        // Fields
        private readonly IUserRepository _userRepository;

        // Constructor
        public UserController(IServiceProvider service) : base(service) { }

        // Public methods
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            // Implementation
        }

        // Private methods
        private async Task<bool> ValidateUserAsync(User user)
        {
            // Implementation
        }
    }
}
```

#### Error Handling
```csharp
// Use specific exception types
throw new NotFoundException("User not found");
throw new ValidationException("Invalid email format");
throw new UnauthorizedException("Access denied");

// Handle exceptions appropriately
try
{
    var result = await _service.ProcessAsync();
    return Ok(result);
}
catch (NotFoundException ex)
{
    return NotFound(new { Message = ex.Message });
}
catch (ValidationException ex)
{
    return BadRequest(new { Message = ex.Message });
}
```

#### Async/Await Patterns
```csharp
// Use async/await consistently
public async Task<IActionResult> GetUserAsync(Guid userId)
{
    var user = await _userRepository.GetByIdAsync(userId);
    return Ok(user);
}

// Avoid blocking async calls
// ❌ Don't do this
var result = _service.GetDataAsync().Result;

// ✅ Do this instead
var result = await _service.GetDataAsync();
```

### Architecture Patterns

#### Repository Pattern
```csharp
public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}
```

#### CQRS with MediatR
```csharp
// Command
public class CreateUserCommand : IRequest<ResponseData<User>>
{
    public string Email { get; set; }
    public string Username { get; set; }
}

// Handler
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ResponseData<User>>
{
    public async Task<ResponseData<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

#### Dependency Injection
```csharp
// Register services in ServiceExtensions.cs
public static class ServiceExtensions
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly));
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
```

## 🧪 Testing Guidelines

### Unit Testing

```csharp
[Test]
public async Task GetUser_WithValidId_ReturnsUser()
{
    // Arrange
    var userId = Guid.NewGuid();
    var expectedUser = new User { Id = userId, Username = "testuser" };
    _mockRepository.Setup(r => r.GetByIdAsync(userId))
                   .ReturnsAsync(expectedUser);

    // Act
    var result = await _handler.Handle(new GetUserQuery(userId), CancellationToken.None);

    // Assert
    Assert.That(result.Status, Is.True);
    Assert.That(result.Data.Username, Is.EqualTo("testuser"));
}
```

### Integration Testing

```csharp
[Test]
public async Task GetUser_IntegrationTest()
{
    // Arrange
    var client = _factory.CreateClient();
    var token = await GetAuthTokenAsync();

    // Act
    var response = await client.GetAsync("/api/user")
                               .WithBearerToken(token);

    // Assert
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
}
```

### Test Organization

```
Tests/
├── Unit/
│   ├── Controllers/
│   ├── Handlers/
│   ├── Repositories/
│   └── Services/
├── Integration/
│   ├── API/
│   ├── Database/
│   └── External/
└── Performance/
    ├── LoadTests/
    └── StressTests/
```

## 📚 Documentation

### Code Documentation

```csharp
/// <summary>
/// Retrieves a user by their unique identifier.
/// </summary>
/// <param name="userId">The unique identifier of the user.</param>
/// <returns>A task that represents the asynchronous operation. The task result contains the user if found.</returns>
/// <exception cref="NotFoundException">Thrown when the user is not found.</exception>
public async Task<User> GetUserAsync(Guid userId)
{
    // Implementation
}
```

### API Documentation

- Use XML documentation comments
- Provide clear parameter descriptions
- Include example requests/responses
- Document error scenarios

### README Updates

- Update setup instructions
- Add new features to feature list
- Update environment variables
- Include new dependencies

## 🏷️ Good First Issues

Looking for your first contribution? Check out these beginner-friendly issues:

### Documentation
- [ ] Improve API documentation
- [ ] Add code examples
- [ ] Update setup guides
- [ ] Create tutorials

### Testing
- [ ] Add unit tests for existing features
- [ ] Improve test coverage
- [ ] Add integration tests
- [ ] Performance testing

### Code Quality
- [ ] Refactor repetitive code
- [ ] Add input validation
- [ ] Improve error messages
- [ ] Code cleanup

### Features
- [ ] Add new API endpoints
- [ ] Implement new blockchain support
- [ ] Add new social features
- [ ] Enhance existing functionality

## 🤝 Community

### Communication Channels

- **GitHub Issues**: Bug reports and feature requests
- **GitHub Discussions**: General discussions and questions
- **Discord**: Real-time community chat (coming soon)
- **Email**: [contact@ovation.network](mailto:contact@ovation.network)

### Getting Help

1. **Check Documentation**: Review existing documentation first
2. **Search Issues**: Look for similar issues or discussions
3. **Ask Questions**: Use GitHub Discussions for questions
4. **Join Community**: Participate in community discussions

### Recognition

Contributors are recognized in:
- **CONTRIBUTORS.md**: List of all contributors
- **Release Notes**: Feature contributors mentioned
- **Community Hall of Fame**: Outstanding contributors

## 📋 Checklist for Contributors

### Before Contributing
- [ ] Read and understand the Code of Conduct
- [ ] Set up development environment
- [ ] Understand the project architecture
- [ ] Review existing issues and discussions

### During Development
- [ ] Follow coding standards
- [ ] Write comprehensive tests
- [ ] Update documentation
- [ ] Consider performance implications
- [ ] Review security implications

### Before Submitting
- [ ] Self-review your code
- [ ] Run all tests locally
- [ ] Update documentation
- [ ] Create descriptive PR
- [ ] Link related issues

## 🎉 Thank You!

Thank you for contributing to Ovation Backend! Your contributions help make this project better for everyone. We appreciate your time and effort in making the NFT portfolio management platform more robust and feature-rich.

---

**Questions?** Feel free to reach out via [GitHub Discussions](https://github.com/ovation-app/ovation-backend/discussions) or [email](mailto:contact@ovation.network).
