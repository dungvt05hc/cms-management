# AGENTS.md — cms-management

## Repo goal
Build a CMS product with a clean, testable architecture. Work in vertical slices: 1 Issue = 1 PR.

## Tech baseline (preferred)
- Backend: .NET 9, ASP.NET Core with Clean Architecture
- Architecture: Vertical Slice under `src/Api/Features`
- DB: PostgreSQL + EF Core migrations (in `src/Persistence`)
- Auth: JWT + Roles/Policies (Admin/Editor/Viewer)
- Observability: structured logging + correlation-id middleware
- Tests: xUnit; integration tests preferred for endpoints

## Commands (must pass before PR is ready)
Backend:
- `dotnet restore`
- `dotnet build -c Release`
- `dotnet test -c Release`
- (optional) `dotnet format` if configured

Frontend (if `apps/admin` exists):
- `npm ci`
- `npm run build`
- `npm test`
- `npm run e2e` (Playwright/Cypress if configured)

## PR rules
- ONE issue per PR.
- No broad refactors.
- Update OpenAPI when public endpoints change.
- Include “How to test” steps and any assumptions.
- Never commit secrets.

## Domain conventions (CMS)
Core concepts (unless issue says otherwise):
- ContentType (schema/fields)
- Entry (content instance)
- Media (file metadata + storage)
- Users/Roles (Admin/Editor/Viewer)
- AuditLog (who changed what)
