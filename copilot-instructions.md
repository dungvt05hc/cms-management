# Copilot Instructions — cms-management (Clean Architecture + REST)

You are building an e-commerce website with a CMS module and an internal admin portal.
Work in **vertical slices**: **1 GitHub Issue = 1 PR**.

## 0) Absolute rules
- Implement exactly what the Issue asks—no extra features.
- Keep PR small, focused, and mergeable.
- Do not refactor unrelated code.
- Always add tests for new behavior (unit + integration where applicable).
- Never commit secrets; never log tokens/PII/payment refs.

## 1) Architecture (must follow)
Projects:
- `src/Api` — ASP.NET Core Web API (REST controllers, middleware, Swagger)
- `src/Application` — Use cases (Commands/Queries), validators, interfaces (ports)
- `src/Domain` — Entities, value objects, domain rules/enums
- `src/Infrastructure` — EF Core DbContext, migrations, external adapters (Payoo/Shipping/FCM/Email/Invoice)

Dependency boundaries:
- Api -> Application -> Domain
- Infrastructure -> Application + Domain
- Domain references nothing else.
- Controllers must stay thin: no business logic, no EF Core queries.

## 2) REST conventions
- Use plural resources: `/products`, `/categories`, `/orders`, `/cart/items`.
- Buyer account scope: `/me/...`
- Admin scope: `/admin/...`
- Search: `/search/suggest`
- Use correct status codes: 200/201/204, 400, 401, 403, 404, 409.
- Errors: return `ProblemDetails` consistently (no stack traces).

## 3) “Controller thin” rule
Controllers should:
- validate route/query binding only (basic)
- call ONE Application use-case
- map use-case result -> HTTP response
- not contain domain rules, totals calculation, payment logic, or DB queries

## 4) Use-case naming & folder structure
Each slice goes under `src/Application/Features/<Area>/<Action>/`.

Naming:
- Commands: `CreateOrderCommand`, `ApplyVoucherCommand`, `RegisterDeviceCommand`
- Queries: `GetProductBySlugQuery`, `ListProductsQuery`
- Handlers: `CreateOrderHandler`, etc.
- Validators: `CreateOrderValidator`, etc.
- Results: `CreateOrderResult` (optional) or return DTOs

API Controllers:
- `ProductsController`, `CategoriesController`, `CartController`, `OrdersController`
- Admin: `AdminProductsController`, `AdminShippingController`, etc.

## 5) Invariants that MUST be enforced + tested
Payoo payment:
- For online Payoo payments: **NO order is created until payment success is verified**.
- Payment callback must be **idempotent**.

Vouchers:
- Max 1 discount voucher + max 1 shipping voucher.
- Shipping promo cannot exceed shipping fee.

Address book:
- Max 5 addresses per buyer.

Orders:
- Buyer can cancel only when status is `Processing`.
- Confirm received; auto-confirm after `[n]` hours (configurable).

Logging:
- Always include `X-Correlation-Id`.
- Redact/mask email/phone, tokens, payment refs.

## 6) External integrations pattern (ports/adapters)
Application defines interfaces:
- `IPaymentGateway` (Payoo)
- `IShippingQuoteProvider`
- `IInvoiceIssuer`
- `INotificationSender` (FCM)
- `IEmailSender`

Infrastructure implements them.
MVP: stubs are acceptable, must be deterministic and testable.

## 7) Quality gates before PR is ready
Backend:
- `dotnet restore`
- `dotnet build -c Release`
- `dotnet test -c Release`
- `dotnet format` (if configured)

Frontend (if present):
- `npm ci`
- `npm run lint`
- `npm run build`
- `npm test`
- `npm run e2e` (Playwright)

## 8) PR output expectations
PR description MUST include:
- Summary
- Closes issue #
- How to test
- API changes
- Security notes (authz, redaction)
- Any assumptions/open questions
