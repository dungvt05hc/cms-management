# CMS Management System

A full-stack e-commerce Content Management System built with .NET 9, React, and PostgreSQL.

## Architecture Overview

This project follows Clean Architecture principles with clear separation of concerns:

```
src/
├── Api/              # API Layer - REST endpoints, authentication, routing
├── Application/      # Application Layer - business logic, use cases, handlers
├── Domain/          # Domain Layer - entities, business rules, interfaces
└── Infrastructure/  # Infrastructure Layer - data access, external services
```

### Layer Responsibilities

#### **Api Layer** (`src/Api`)
- ASP.NET Core Web API with minimal API and controllers
- JWT authentication and role-based authorization
- OpenAPI/Swagger documentation
- Health check endpoints
- CORS configuration
- Dependency injection wiring

#### **Application Layer** (`src/Application`)
- Use case handlers (CQRS pattern)
- Request/response DTOs
- FluentValidation validators
- Business logic orchestration
- Service abstractions (interfaces)

**Key Features:**
- **Auth**: Registration, login, OTP verification, password reset
- **Admin Auth**: Staff management and admin login
- **Products**: CRUD operations, search, suggestions, variants
- **Categories**: Hierarchical category tree management
- **Cart**: Shopping cart operations
- **Orders**: Order management, status tracking, reordering
- **Checkout**: Multi-step checkout with vouchers and shipping
- **Shipping**: Quote calculation, method selection
- **Payments**: Payment gateway integration (Payoo)
- **Invoices**: E-invoice generation and issuance
- **Notifications**: User notification system
- **Addresses**: Customer address management
- **Devices**: Push notification device registration

#### **Domain Layer** (`src/Domain`)
Pure business logic with no external dependencies.

**Key Entities:**
- `User` - Customer accounts
- `StaffUser` - Admin/staff accounts with roles
- `Product`, `ProductVariant`, `ProductGroup` - Product catalog
- `Category` - Hierarchical product categories
- `Order`, `OrderItem` - Order management
- `Cart`, `CartItem` - Shopping cart
- `Address` - Customer addresses
- `ShippingMethod`, `ShippingConfig`, `ShippingCarrier` - Shipping logic
- `Voucher` - Discount vouchers
- `InvoiceProfile` - E-invoice configuration
- `Notification` - User notifications
- `DeviceToken` - Push notification devices

#### **Infrastructure Layer** (`src/Infrastructure`)
Implements application abstractions with external dependencies.

**Key Components:**
- **Persistence**: EF Core with PostgreSQL (`AppDbContext`)
- **Migrations**: Database versioning
- **Auth**: JWT token generation, password hashing
- **Services**:
  - Email sender (stub implementation)
  - Shipping quote provider
  - Payment gateway integration
  - Invoice issuer
  - Bootstrap services (super admin, shipping methods)

---

## Prerequisites

- **.NET SDK 9.x** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL** (Docker recommended) - Version 14+
- **Node.js 20.x** - For frontend development
- **Docker Desktop** - For containerized PostgreSQL

---

## Getting Started

### 1. Database Setup

Start PostgreSQL using Docker:

```bash
docker run -d \
  --name cms-postgres \
  -e POSTGRES_USER=admin \
  -e POSTGRES_PASSWORD=Admin123456aA \
  -e POSTGRES_DB=commerce_management \
  -p 5432:5432 \
  postgres:14
```

### 2. Configuration

The default configuration is in `src/Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=commerce_management;Username=admin;Password=Admin123456aA"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-min-32-characters-long-for-production",
    "Issuer": "cms-management",
    "Audience": "cms-management-api",
    "ExpirationMinutes": 60
  }
}
```

For development, create `src/Api/appsettings.Development.json` to override settings (already gitignored).

### 3. Build and Run

From the `src` directory:

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Apply migrations
cd Api
dotnet ef database update

# Run API
dotnet run
```

The API will be available at `http://localhost:5000` (or the port specified).

**OpenAPI documentation**: `http://localhost:5000/openapi/v1.json`

### 4. Run Tests

```bash
# Run all tests
dotnet test

# Run integration tests only
dotnet test tests/Api.IntegrationTests

# Run unit tests only
dotnet test tests/Application.UnitTests

# With detailed output
dotnet test --logger "console;verbosity=detailed"
```

---

## Project Structure Details

### API Endpoints

The API follows REST conventions with the following main routes:

- **Auth**: `/auth/register`, `/auth/login`, `/auth/verify-otp`, `/auth/forgot-password`, `/auth/reset-password`
- **Admin Auth**: `/admin/auth/login`, `/admin/staff`
- **Products**: `/products`, `/products/{id}`, `/products/slug/{slug}`, `/products/suggestions`
- **Categories**: `/categories`, `/categories/tree`
- **Cart**: `/cart`, `/cart/items`
- **Orders**: `/orders`, `/orders/{id}`, `/orders/{id}/cancel`, `/orders/{id}/confirm`
- **Checkout**: `/checkout/preview`, `/checkout/voucher`, `/checkout/submit`
- **Shipping**: `/shipping/methods`, `/shipping/quote`
- **Addresses**: `/addresses`, `/addresses/{id}/set-default`
- **Notifications**: `/notifications`
- **Devices**: `/devices` (push notifications)
- **Health**: `/health`

### Authentication & Authorization

The system uses JWT Bearer tokens with role-based access control:

**Roles:**
- `SuperAdmin` - Full system access
- `Admin` - Administrative access
- `Staff` - Limited staff access
- `Customer` - Regular user access (implicit)

**Policies:**
- `AdminOnly` - Requires SuperAdmin or Admin role
- `StaffOrAdmin` - Requires SuperAdmin, Admin, or Staff role

### Database Migrations

```bash
# Add new migration
cd src/Api
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Rollback to specific migration
dotnet ef database update PreviousMigrationName

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script
```

### Bootstrap Data

The application automatically bootstraps essential data on startup:

1. **Super Admin Account** (`SuperAdminBootstrapService`)
   - Creates default super admin if none exists
   - Used for initial system access

2. **Shipping Methods** (`ShippingBootstrapService`)
   - Seeds default shipping carriers and methods
   - Configures shipping zones and rates

---

## Development Workflow

### Adding a New Feature

Follow the Clean Architecture pattern:

1. **Domain** - Define entities in `src/Domain/Entities/`
2. **Application** - Create feature folder in `src/Application/Features/`
   ```
   Features/
   └── NewFeature/
       ├── CreateNewFeature/
       │   ├── CreateNewFeatureHandler.cs
       │   ├── CreateNewFeatureRequest.cs
       │   ├── CreateNewFeatureResponse.cs
       │   └── CreateNewFeatureValidator.cs
       └── GetNewFeature/
           ├── GetNewFeatureHandler.cs
           ├── GetNewFeatureRequest.cs
           └── GetNewFeatureResponse.cs
   ```
3. **Infrastructure** - Implement abstractions if needed
4. **Api** - Add controller in `src/Api/Controllers/`
5. **Tests** - Add integration tests in `tests/Api.IntegrationTests/`

### Code Style

The project enforces StyleCop rules (see `stylecop.json`):
- Use file-scoped namespaces
- Include copyright headers
- Follow naming conventions
- Document public APIs

### Dependency Injection

Register services in the appropriate layer:
- **Application**: `src/Application/DependencyInjection.cs`
- **Infrastructure**: `src/Infrastructure/DependencyInjection.cs`

---

## Testing Strategy

### Integration Tests (`tests/Api.IntegrationTests`)

Uses `WebApplicationFactory` for in-memory API testing:

```csharp
public class MyEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MyEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task TestEndpoint() { ... }
}
```

**Coverage:**
- Authentication flows
- CRUD operations
- Business rule validation
- Authorization checks
- Error handling

### Unit Tests (`tests/Application.UnitTests`)

Focuses on business logic in handlers and services:
- Handler logic
- Validation rules
- Domain entity behavior

---

## Configuration Reference

### Environment Variables

Override settings via environment variables:

```bash
export ConnectionStrings__DefaultConnection="Host=prod-db;..."
export JwtSettings__Secret="production-secret-key"
dotnet run
```

### JWT Configuration

- **Secret**: Minimum 32 characters, store securely in production
- **Issuer/Audience**: Configure for your domain
- **ExpirationMinutes**: Token lifetime (default: 60)

### Database Connection

For production, use connection pooling and SSL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod.db;Database=cms;Username=app;Password=***;SSL Mode=Require;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100"
  }
}
```

---

## Troubleshooting

### Database Connection Issues

```bash
# Check PostgreSQL is running
docker ps | grep postgres

# Check connection from CLI
psql -h localhost -U admin -d commerce_management

# View logs
docker logs cms-postgres
```

### Migration Errors

```bash
# Reset database (CAUTION: deletes all data)
dotnet ef database drop
dotnet ef database update

# Check migration history
dotnet ef migrations list
```

### Build Errors

```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build

# Check for StyleCop issues
dotnet build /p:TreatWarningsAsErrors=true
```

---

## Production Deployment

### Checklist

- [ ] Change JWT secret to strong random key
- [ ] Use production database connection string with SSL
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Enable HTTPS redirection
- [ ] Configure CORS for production domains
- [ ] Set up logging (Application Insights, Serilog, etc.)
- [ ] Configure health checks endpoint
- [ ] Enable rate limiting
- [ ] Set up automated backups
- [ ] Configure monitoring and alerts

### Docker Deployment

```dockerfile
# Example Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Api/Api.csproj", "Api/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Api/Api.csproj"
COPY src/ .
RUN dotnet build "Api/Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api/Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
```

---

## Contributing

1. Follow Clean Architecture principles
2. Write tests for new features
3. Ensure StyleCop compliance
4. Update documentation
5. Create descriptive commit messages

---

## License

Copyright (c) CMS Management. All rights reserved.

---

## Additional Resources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)