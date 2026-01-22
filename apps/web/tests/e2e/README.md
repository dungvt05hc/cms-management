# Product List E2E Tests

This E2E test suite validates the product list page with cursor pagination, sorting, and infinite scroll functionality.

## Prerequisites

1. **Backend API must be running** on `http://localhost:5000`
2. **Test data**: The backend should have at least 30+ products seeded for pagination tests
3. **Next.js dev server or build** must be running on `http://localhost:3000`

## Running E2E Tests

### Option 1: With Local Backend (Development)

```bash
# Terminal 1: Start backend API
cd src/Api
dotnet run

# Terminal 2: Start frontend
cd apps/web
npm run dev

# Terminal 3: Run E2E tests
cd apps/web
npm run e2e
```

### Option 2: With Docker Compose (Recommended for CI)

```bash
# Start all services
docker-compose up -d

# Wait for services to be ready
sleep 10

# Run E2E tests
cd apps/web
npm run e2e

# Stop services
docker-compose down
```

## Test Scenarios Covered

1. **Basic Product List Display**: Verifies products page loads and displays initial products
2. **Sort Order Change**: Tests changing sort from default to price ascending/descending
3. **Infinite Scroll**: Validates that scrolling loads more products using cursor pagination
4. **Product Card Elements**: Ensures all required UI elements are present (name, price, buttons)
5. **Price Sort Validation**: Verifies actual price order matches selected sort direction

## Test Data Requirements

For tests to pass completely, the backend should have:
- At least 30 products (to test pagination across multiple pages)
- Products with varying prices (to test sorting)
- Products should be active and have at least one variant with price

## Skipped Tests

Tests gracefully skip if:
- No products exist in the database
- Less than 2 products (for sort validation)
- No more items to paginate (for infinite scroll test)

This allows tests to run in any environment without failing due to missing test data.
