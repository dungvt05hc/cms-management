# [BE+FE][Storefront] Product List - Cursor Pagination + Sort + Infinite Scroll + E2E

## Summary

This PR implements a complete fullstack slice for product listing with cursor-based pagination, price sorting, and infinite scroll functionality, following Clean Architecture principles.

## Changes

### Phase A: Backend (API + Tests)

#### API Enhancements (`GET /products`)
- ✅ **Cursor pagination** support with stable, deterministic cursors
- ✅ **Sort by price** (ascending/descending) using `sort` query parameter
- ✅ **Category filter** by slug OR id via `category` parameter
- ✅ **Backward compatible** with existing offset pagination (page/pageSize)
- ✅ **Anonymous access** (no authentication required)

#### Architecture
- **Controller** (`ProductsController.cs`): Thin controller delegates to handler
- **Use Case** (`GetProductsHandler.cs`): 
  - Added `HandleCursor()` method for cursor pagination
  - Maintains existing `Handle()` for offset pagination
  - Implements stable sorting with price + ID tiebreaker
- **Domain**: Uses existing `Product` and `ProductVariant` entities
- **DTOs**: Added `CursorPagedResult<T>` for cursor responses
- **Cursor Encoding**: Base64-encoded JSON with product ID, price, and timestamp

#### Key Files Added/Modified
- ✅ `src/Application/Features/Products/GetProducts/CursorPagedResult.cs` (new)
- ✅ `src/Application/Features/Products/GetProducts/ProductCursor.cs` (new)
- ✅ `src/Application/Features/Products/GetProducts/GetProductsQuery.cs` (enhanced)
- ✅ `src/Application/Features/Products/GetProducts/GetProductsHandler.cs` (enhanced)
- ✅ `src/Api/Controllers/ProductsController.cs` (enhanced)
- ✅ `src/Api/Controllers/AdminProductsController.cs` (updated for compatibility)

#### Tests
- ✅ **6 new integration tests** in `ProductsCursorPaginationTests.cs`:
  - Multi-page cursor pagination (35 products across 4 pages)
  - Price sorting (ascending/descending)
  - Cursor pagination with sorting maintains stable order
  - Category filtering by slug
  - Invalid cursor handling
- ✅ **All 46 tests pass** (16 product tests: 10 existing + 6 new)
- ✅ **Backward compatibility** verified with existing tests

### Phase B: Frontend (UI + E2E)

#### UI Components
- ✅ **ProductList component** (`components/ProductList.tsx`):
  - Client-side component with state management
  - Intersection Observer for infinite scroll
  - Sort dropdown (Newest First, Price Low-to-High, Price High-to-Low)
  - Loading states, error handling, empty state
  - Responsive grid layout
- ✅ **Product cards** display:
  - Product image placeholder
  - Name, price (from first variant)
  - "View" button (enabled)
  - "Buy" button (disabled placeholder)

#### Routes
- ✅ `/products` - All products page
- ✅ `/category/[slug]` - Category products page (uses ProductList with filter)

#### API Client
- ✅ Enhanced `lib/api.ts`:
  - `getProducts()` function with cursor pagination support
  - TypeScript interfaces for `CursorPagedResult<T>`
  - Support for all query parameters (category, sort, cursor, limit)

#### E2E Tests (`tests/e2e/product-list.spec.ts`)
- ✅ **6 comprehensive E2E scenarios**:
  1. Initial product list display
  2. Sort order changes and updates list
  3. Infinite scroll loads more products
  4. Product cards have required elements
  5. Price ascending sort validation
  6. Price descending sort validation
- ✅ **Graceful handling** of missing test data (tests skip if no products)
- ✅ **Data-testid selectors** for reliable test stability

## API Contract

### Request
```http
GET /products?category={slug}&sort={priceAsc|priceDesc}&cursor={base64}&limit={20}
```

**Query Parameters:**
- `category` (optional): Category slug or GUID
- `categoryId` (optional): Category GUID
- `q` (optional): Search query
- `sort` (optional): `priceAsc` or `priceDesc`
- `cursor` (optional): Base64-encoded cursor for pagination
- `limit` (optional): Page size (default 20, max 100)
- `page`, `pageSize` (optional): Offset pagination (backward compatible)

### Response (Cursor Pagination)
```json
{
  "items": [
    {
      "id": "guid",
      "name": "Product Name",
      "slug": "product-slug",
      "description": "...",
      "categoryId": "guid",
      "images": "...",
      "variants": [
        {
          "id": "guid",
          "sku": "SKU-001",
          "variantName": "Red - Large",
          "price": 99.99,
          "stockQuantity": 10
        }
      ],
      "isActive": true,
      "isFeatured": false,
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-01T00:00:00Z"
    }
  ],
  "nextCursor": "eyJwcm9kdWN0SWQiOi4uLn0=",
  "hasMore": true
}
```

## Testing

### Backend
```bash
dotnet restore src/Cms.sln
dotnet build src/Cms.sln -c Release
dotnet test src/Cms.sln -c Release
```

**Results:** ✅ 46 tests passed (16 product tests including 6 new cursor pagination tests)

### Frontend
```bash
cd apps/web
npm ci
npm run lint    # ✅ No errors
npm test        # ✅ Passes (no unit tests yet)
npm run build   # ✅ Build successful
```

### E2E (requires backend running)
```bash
# Start backend (terminal 1)
cd src/Api && dotnet run

# Run E2E (terminal 2)
cd apps/web && npm run e2e
```

See `apps/web/tests/e2e/README.md` for detailed E2E setup instructions.

## Architecture Compliance

✅ **Clean Architecture boundaries maintained:**
- Api → Application → Domain
- Infrastructure → Application
- No business logic in controllers
- No EF queries in controllers

✅ **Error handling:**
- ProblemDetails for validation errors
- Safe logging (no secrets, PII masked)
- Graceful degradation for invalid cursors

✅ **Testing:**
- Integration tests cover happy path + edge cases
- E2E tests validate end-to-end user flow
- Backward compatibility verified

## UI Screenshots

### Product List with Sort
- Grid layout with product cards
- Sort dropdown (Newest First, Price Low-to-High, Price High-to-Low)
- Infinite scroll trigger at bottom

### Product Card
- Image placeholder
- Product name
- Price (from first variant)
- View button (enabled)
- Buy button (disabled, placeholder for future)

## Acceptance Criteria

✅ **Given** buyer opens a category product list  
✅ **When** they change sort to priceAsc  
✅ **Then** the list order changes  
✅ **When** they scroll to bottom  
✅ **Then** more items load  

## Breaking Changes
None. This PR is backward compatible with existing offset pagination.

## Migration Required
No database migrations needed. Uses existing Product and ProductVariant tables.

## Documentation
- Added E2E test README (`apps/web/tests/e2e/README.md`)
- API contract documented above
- Code comments follow existing conventions

## Follow-up Issues
- [ ] Add product detail page (out of scope for this PR)
- [ ] Implement "Buy" button functionality (requires cart/checkout slice)
- [ ] Add product image upload/display (requires media management)
- [ ] Add search functionality UI (backend already supports `q` parameter)

## Checklist
- [x] Backend endpoints implemented + status codes correct
- [x] Controller thin; business logic in Application/Domain
- [x] Validation + ProblemDetails on errors
- [x] Tests: BE integration tests exist and pass (46 tests)
- [x] UI implemented + handles states (loading/empty/error)
- [x] E2E happy path exists (6 scenarios)
- [x] No secrets/PII in logs
- [x] CI green (backend tests pass, frontend builds successfully)
- [x] Backward compatibility maintained
- [x] Clean Architecture boundaries respected
