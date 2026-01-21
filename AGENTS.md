# AGENTS.md — cms-management (Clean Architecture + REST)

This repository builds an e-commerce website with a CMS module (articles/static pages/help) and an internal management portal (admin/operations).  
Work in **vertical slices**: **1 Issue = 1 PR**.

---

## 1) Architecture (mandatory boundaries)

### Projects
- `src/Api` — ASP.NET Core Web API (REST Controllers, Swagger, middleware)
- `src/Application` — Use cases (Commands/Queries), validators, DTOs, interfaces (ports)
- `src/Domain` — Entities, Value Objects, Domain services/rules, enums
- `src/Infrastructure` — EF Core DbContext, migrations, external service adapters (Payoo/Shipping/FCM/Email), repositories

### Dependency rule (never break this)
- **Api -> Application -> Domain**
- **Infrastructure -> Application + Domain**
- Domain must not reference Application/Infrastructure.
- Application must not reference Api/Infrastructure implementations (only interfaces).
- Api should contain minimal logic: mapping HTTP <-> application requests/responses.

---

## 2) REST controller conventions

### Route naming
- Use plural nouns:
  - `/products`, `/categories`, `/orders`, `/cart/items`
  - Admin routes under `/admin/...`
- Buyer account routes under `/me/...`
- Search suggest under `/search/suggest`

### HTTP semantics
- `GET` = fetch (200)
- `POST` = create/action (201/200)
- `PUT` = replace/update (200/204)
- `PATCH` = partial update (200)
- `DELETE` = delete (204)

### Standard status codes
- 200 OK / 201 Created / 204 No Content
- 400 validation error (ProblemDetails)
- 401 unauthenticated
- 403 unauthorized
- 404 not found
- 409 conflict (duplicate slug/SKU, invalid state transition)

### Error format
- Use `ProblemDetails` consistently.
- Validation errors must contain field-level details.
- Never leak stack traces or internal exception messages to clients.

---

## 3) Naming & folder structure (how to keep slices clean)

### Feature folders (recommended)
In `src/Application/Features/<Area>/<Action>/`:
- `<Action>Command.cs` or `<Action>Query.cs`
- `<Action>Handler.cs`
- `<Action>Validator.cs`
- `<Action>Result.cs` (optional)

In `src/Api/Controllers/`:
- Controllers are thin; delegate to Application use-cases.

In `src/Domain/Entities/`:
- `Product`, `ProductVariant`, `Category`, `ProductGroup`, `Order`, `OrderItem`, `Cart`, `CartItem`, `Voucher`, `InvoiceProfile`, `DeviceToken`, `User`, `StaffUser`

In `src/Infrastructure/Persistence/`:
- `AppDbContext`
- Migrations under the Infrastructure (or a dedicated Persistence project if you prefer)

### Naming rules
- Commands/Queries: `CreateProductCommand`, `GetProductBySlugQuery`
- Handlers: `CreateProductHandler`
- Validators: `CreateProductValidator`
- Controllers: `ProductsController`, `AdminProductsController`, `CartController`
- DTOs: `ProductDto`, `OrderDto`, `CheckoutPreviewDto`
- Avoid abbreviations unless in glossary (DVVC, COD, GTGT, FCM).

---

## 4) Cross-cutting requirements (must follow)

### Logging & correlation id
- Every request/response must carry `X-Correlation-Id`.
- Use structured logs (no string concatenation for important fields).
- Never log secrets or PII. Mask email/phone and payment refs.

### Authentication / Authorization
- Buyer endpoints must restrict data to the authenticated buyer.
- `/admin/*` must be gated by Admin/Staff policies.
- Tests must include negative auth cases for admin endpoints.

### Critical business invariants
These must be enforced in Application/Domain logic and covered by tests:

**Payments (Payoo)**
- For Payoo online payments: **NO order is created until payment success is verified**.
- Payment callback must be **idempotent** (safe on retries).

**Vouchers**
- Max 1 discount voucher + max 1 shipping voucher.
- Shipping promotion cannot exceed shipping fee.

**Address book**
- Max 5 addresses per buyer.

**Order states**
- Buyer can cancel only when status is `Processing`.
- Confirm received; auto-confirm after `[n]` hours (configurable).

---

## 5) External integrations (adapters-first)

Implement external services behind interfaces in Application:
- `IPaymentGateway` (Payoo adapter in Infrastructure)
- `IShippingQuoteProvider` (Shipping SDK adapter in Infrastructure)
- `IInvoiceIssuer` (VAT e-invoice adapter in Infrastructure)
- `INotificationSender` (FCM adapter in Infrastructure)
- `IEmailSender` (email adapter in Infrastructure)

For MVP, stubs are acceptable, but must be deterministic and testable.

---

## 6) Tests (required)

### Minimum per PR
- Unit tests for validators and critical calculations/invariants.
- Integration tests for controller endpoints (happy + key negative).
- No flaky tests: deterministic data setup.

### Suggested test structure
- `tests/UnitTests/...`
- `tests/IntegrationTests/...`

---

## 7) Commands (must pass before PR is ready)
- `dotnet restore`
- `dotnet build -c Release`
- `dotnet test -c Release`

Frontend (if present):
- `npm ci`
- `npm run build`
- `npm run test`
- `npm run e2e` (Playwright)

---

## 8) PR rules (Copilot must follow)
- **ONE Issue = ONE PR**
- Keep diffs minimal and focused
- Update OpenAPI if public endpoints changed
- Include “How to test” steps
- No secrets, no unrelated refactors

## Default agent routing (Issue → Agent)

We use GitHub Copilot Custom Agents. For consistency, we follow this default routing based on the issue title prefix:

### 1) [BE] issues (Backend only)
**Default agent:** `api-slice-builder`
- Owns: REST controllers, Application use-cases, Domain rules, Infrastructure persistence/adapters, EF migrations, backend tests.
- Must: `dotnet build/test` green.

### 2) [FE] issues (Frontend only)
**Default agent:** `frontend-e2e-builder`
- Owns: `apps/web` screens, client calls, UI states, and FE build/lint.
- If E2E is required by the issue, add/update Playwright tests.

### 3) [BE+FE] issues (Full vertical slice in one PR)
**Default agent:** `fullstack-slice-builder` ✅
- Owns: End-to-end slice in ONE PR with two phases:
  - **Phase A (Backend):** implement API contract + tests + (migration if required)
  - **Phase B (Frontend + E2E):** implement UI + Playwright happy path
- Must: CI green for Backend + Frontend + E2E.

### 4) Hardening / review pass
**Default agent:** `quality-guardian`
- Owns: security pass, refactor, performance, logging/PII checks, flaky test fixes.
- Used when: issue title starts with `[HARDEN]` or when explicitly assigned.

---

## Assignment rule of thumb

- If the issue title prefix is `[BE+FE]`, ALWAYS start with `fullstack-slice-builder` (unless the issue explicitly says otherwise).
- If an issue is marked `[BE+FE]` but is too large for one PR, split into two issues:
  - `[BE] ...` then `[FE] ...` (still follow 1 issue = 1 PR).
- Always keep PR scope limited to the Acceptance Criteria in the issue.
