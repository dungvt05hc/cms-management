---
name: Vertical Slice (1 Issue = 1 PR)
about: One mergeable slice (BE / FE / BE+FE) following Clean Architecture + REST + optional E2E
title: "[BE+FE][Area] Short slice title"
labels: ["slice", "copilot"]
assignees: []
---

# Goal
Describe the single user/admin value this slice delivers (one flow only).

# Context / Requirement reference
Link the requirement section(s) or paste a short excerpt:
- docs/requirements/requirements.md#...

# Slice Type (pick one)
- [ ] BE only
- [ ] FE only
- [ ] BE + FE (same PR)

# Scope (Must)
> Keep scope small. If you can’t finish within one PR, split into another slice.

## Backend (BE) — Clean Architecture + REST
### API (src/Api)
- [ ] Add/modify **REST controller** endpoint(s) under:
  - Buyer scope: `/...` or `/me/...`
  - Admin scope: `/admin/...`
- [ ] Controller is **thin** (no EF queries, no business rules). Only:
  - request binding + authorization + call one use-case + map to HTTP.
- [ ] Return status codes correctly: 200/201/204, 400, 401, 403, 404, 409.
- [ ] Errors use **ProblemDetails** (no stack traces in responses).

### Application (src/Application)
- [ ] Add one use-case per action:
  - Command: `DoThingCommand` + `DoThingHandler`
  - Query: `GetThingQuery` + `GetThingHandler`
- [ ] Add validation (`FluentValidation`) and return consistent error shape.
- [ ] Define ports (interfaces) for any external integration:
  - `IPaymentGateway`, `IShippingQuoteProvider`, `IInvoiceIssuer`, `INotificationSender`, `IEmailSender`, etc.

### Domain (src/Domain)
- [ ] Put business invariants/rules here (entities/value objects/enums).
- [ ] Domain does not reference other layers.

### Infrastructure (src/Infrastructure)
- [ ] EF Core persistence (DbContext + configs) for new entities.
- [ ] Add migration if schema changes.
- [ ] Implement adapters for ports (stubs allowed for MVP; must be deterministic & testable).

### Security / AuthZ
- [ ] AuthN: Anonymous / Buyer / Staff / Admin clearly stated for each endpoint.
- [ ] AuthZ policies enforced (buyer cannot hit `/admin/*`, staff cannot hit `/me/*` unless intended).
- [ ] Never log secrets/tokens/payment references/OTP/password.

## Frontend (FE) — React/Next.js (apps/web)
### UI
- [ ] Add/modify route(s) and screen(s) for this slice:
  - Storefront: `/`, `/category/[slug]`, `/p/[slug]`, `/cart`, `/checkout`, `/account/...`
  - Admin (later): `/admin/...` (if applicable)
- [ ] UI has states: loading / empty / error / unauthorized.
- [ ] Use stable selectors for tests: `data-testid="..."`.

### API integration
- [ ] Add typed client call(s) for new endpoints (single place for fetch wrappers).
- [ ] Handle errors with user-friendly messages; no raw error dumps.

### Telemetry (only if required by slice)
- [ ] GA/GTM hooks or events (keep minimal and opt-in).

## Tests
### Backend tests
- [ ] Unit tests for domain rules and/or validators.
- [ ] Integration tests for endpoints (happy path + at least 1 negative case).
- [ ] Verify ProblemDetails and status codes.

### Frontend tests
- [ ] Unit tests (optional for MVP) for utilities/components if meaningful.

### E2E (Playwright)
- [ ] If FE is included: add/update an E2E **happy path** test for this slice.
- [ ] Avoid flaky steps (use `data-testid`, wait for stable UI).

## Observability
- [ ] Correlation id is included: `X-Correlation-Id`.
- [ ] Logs are structured and safe (mask email/phone; never log tokens/refs).

# Out of scope
Explicitly list what is NOT included (prevents scope creep):
- -

# Acceptance Criteria (Gherkin)
## Happy path
Given ...
When ...
Then ...

## Negative / edge cases
Given ...
When ...
Then ...

# API Contract draft (if BE included)
> Keep this concise but explicit.

## Endpoints
- `METHOD /path`
  - Auth: Anonymous / Buyer / Staff / Admin
  - Request:
    - body: `{ ... }` (if any)
    - query: `?a=&b=` (if any)
  - Response:
    - 200/201/204: `{ ... }`
  - Errors:
    - 400: validation (ProblemDetails)
    - 401/403: auth
    - 404: not found (if applicable)
    - 409: conflict (if applicable)

# UI Contract draft (if FE included)
## Route(s)
- `/...`

## UI behavior
- What user sees:
- Actions available:
- Validation rules:
- Error states:

# Data notes
- New entities/tables:
- Migration required: [ ] yes  [ ] no
- Seed/test fixtures needed: [ ] yes  [ ] no
- Idempotency required (payments/callbacks/webhooks): [ ] yes  [ ] no

️

# Quality gates (Must pass before PR is ready)
## Backend
- [ ] `dotnet restore`
- [ ] `dotnet build -c Release`
- [ ] `dotnet test -c Release`

## Frontend (apps/web)
- [ ] `npm ci`
- [ ] `npm run lint`
- [ ] `npm run build`
- [ ] `npm test`
- [ ] `npm run e2e` (if FE included)

# Agent assignment (pick who owns this slice)
- [ ] Agent A — Backend slice owner (Clean Architecture + REST + tests)
- [ ] Agent B — Frontend + E2E owner (React/Next.js + Playwright)
- [ ] Agent C — Hardening/review (security, performance, refactor, logging/PII)

# PR checklist (for the implementer)
- [ ] PR title matches issue title.
- [ ] PR description includes: Summary, Closes #, How to test, API changes, Security notes.
- [ ] No secrets committed; logs are safe.
- [ ] CI green.
