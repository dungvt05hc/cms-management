# Home Page - Categories + Banner + Featured Products + E2E

This PR implements the storefront home page with categories navigation, banner placeholder, and featured products section.

## What's Implemented

### Backend (Phase A) ✅
- ✅ Added `IsFeatured` boolean field to Product entity
- ✅ Created EF Core migration for the new field
- ✅ Created public `ProductsController` at `/products` (anonymous access)
- ✅ Updated `GetProductsHandler` to support `featured` query parameter
- ✅ Updated `CreateProductCommand`, `UpdateProductCommand` to include `IsFeatured`
- ✅ Added integration tests for featured products endpoint (3 new tests)
- ✅ All 37 backend tests passing

### Frontend (Phase B) ✅
- ✅ Updated home page to show banner placeholder
- ✅ Added `FeaturedProducts` component showing featured products grid
- ✅ Updated `CategoryNavigation` to make categories clickable with Next.js Link
- ✅ Created `/category/[id]` page for category product lists
- ✅ Added API client functions for fetching featured products
- ✅ Created comprehensive E2E test suite in `home.spec.ts`
- ✅ Frontend build passes successfully

## API Endpoints

### Public Endpoints (Anonymous)
- `GET /categories/tree` - Get category tree (already existed)
- `GET /products?featured=true&pageSize=10` - Get featured products (new)
- `GET /products?categoryId={guid}` - Get products by category (new public access)

## How to Test Manually

### 1. Start Backend
```bash
cd src
dotnet run --project Api
# Backend runs on http://localhost:5000
```

### 2. Start Frontend
```bash
cd apps/web
npm ci
npm run dev
# Frontend runs on http://localhost:3000
```

### 3. Seed Test Data (Optional)
Use the admin endpoints to create categories and featured products:

```bash
# Login as admin
curl -X POST http://localhost:5000/admin/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Admin123!"}'

# Create a featured product
curl -X POST http://localhost:5000/admin/products \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "slug": "gaming-laptop",
    "description": "High-performance gaming laptop",
    "isActive": true,
    "isFeatured": true,
    "variants": [
      {"sku": "LAPTOP-001", "variantName": "16GB RAM", "price": 1299.99, "stockQuantity": 10}
    ]
  }'
```

### 4. Run E2E Tests
```bash
cd apps/web
npm run e2e
```

## E2E Test Coverage

The `home.spec.ts` includes:
1. ✅ Banner placeholder visibility
2. ✅ Featured products section (or empty state)
3. ✅ Category navigation from home to category page
4. ✅ Category expand/collapse functionality
5. ✅ Back to home navigation

## What to Verify

1. **Home Page** (`/`)
   - Banner placeholder with dashed border is visible
   - Categories tree shows on the left side
   - Featured products grid shows on the right (or "No featured products" message)
   - Clicking a category navigates to `/category/{id}`

2. **Category Page** (`/category/{id}`)
   - Shows "Category Products" title
   - Displays the category ID
   - Has "Back to Home" link

3. **API Responses**
   - `/categories/tree` returns hierarchical category structure
   - `/products?featured=true` returns only products with `isFeatured=true`
   - Both endpoints work without authentication

## Architecture Compliance

✅ **Clean Architecture Boundaries Maintained:**
- Api → Application → Domain
- Infrastructure → Application + Domain
- Controllers are thin (delegate to handlers)
- Business logic in Application layer
- No EF queries in controllers

✅ **REST Conventions:**
- Plural nouns: `/products`, `/categories`
- Standard HTTP verbs and status codes
- Anonymous access for public endpoints
- ProblemDetails for errors

✅ **Testing:**
- Integration tests for API endpoints
- E2E tests for user workflows
- Deterministic test data

## Files Changed

### Backend
- `src/Domain/Entities/Product.cs` - Added `IsFeatured` property
- `src/Infrastructure/Persistence/AppDbContext.cs` - Added `IsFeatured` configuration
- `src/Infrastructure/Migrations/` - Added migration for `IsFeatured`
- `src/Application/Features/Products/ProductDto.cs` - Added `IsFeatured` to DTO
- `src/Application/Features/Products/GetProducts/GetProductsQuery.cs` - Added `Featured` filter
- `src/Application/Features/Products/GetProducts/GetProductsHandler.cs` - Implemented featured filter
- `src/Application/Features/Products/CreateProduct/*` - Added `IsFeatured` to command/handler
- `src/Application/Features/Products/UpdateProduct/*` - Added `IsFeatured` to command/handler
- `src/Api/Controllers/ProductsController.cs` - New public products controller
- `src/Api/Controllers/AdminProductsController.cs` - Updated to include `IsFeatured`
- `src/Api/Controllers/UpdateProductRequest.cs` - Added `IsFeatured`
- `tests/Api.IntegrationTests/ProductsEndpointTests.cs` - New test file
- `tests/Api.IntegrationTests/AdminProductsEndpointTests.cs` - Updated existing tests

### Frontend
- `apps/web/lib/api.ts` - Added Product types and `getFeaturedProducts()` function
- `apps/web/components/CategoryNavigation.tsx` - Added Next.js Link for navigation
- `apps/web/components/FeaturedProducts.tsx` - New component
- `apps/web/app/page.tsx` - Updated home page with banner and featured products
- `apps/web/app/category/[id]/page.tsx` - New category page
- `apps/web/tests/e2e/home.spec.ts` - New E2E test suite

## CI/CD Status

- ✅ Backend build passes
- ✅ Backend tests pass (37 tests)
- ✅ Frontend build passes
- ⏳ E2E tests require both services running (manual verification recommended)

## Closes

Closes #{issue_number}
