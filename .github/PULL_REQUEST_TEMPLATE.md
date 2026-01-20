## Summary
- What does this PR deliver (1–3 bullets)?
- Which user/admin value does it unlock?

## Issue
Closes #

## Scope (what’s included)
- [ ] API (controllers/routes)
- [ ] Application (use cases/handlers/validators)
- [ ] Domain (entities/rules/enums)
- [ ] Infrastructure (EF Core/external adapters)
- [ ] Tests (unit/integration)
- [ ] Docs/OpenAPI updates

## Out of scope (explicitly NOT included)
- -

## API Contract
### New/changed endpoints
- `METHOD /path`
  - Auth: Anonymous / Buyer / Staff / Admin
  - Request:
  - Response:
  - Error codes:

### Backward compatibility
- [ ] No breaking changes
- If breaking changes are required, explain why and provide migration notes:
  - -

## How to test
### Local
1. `dotnet restore`
2. `dotnet build -c Release`
3. `dotnet test -c Release`
4. (Optional) Run API:
   - `dotnet run --project src/Api`
   - Open Swagger: `/swagger`

### Manual checks
- Steps:
  1)
  2)

## Data / Migration
- [ ] Migration added (if schema changed)
- Migration name:
- Notes (seed, backfill, destructive changes):
  - -

## Quality gates
- [ ] `dotnet build -c Release` passes
- [ ] `dotnet test -c Release` passes
- [ ] Formatting/lint (if configured) passes
- [ ] CI is green

## Security & Privacy
- [ ] No secrets committed
- [ ] Logs redact tokens/PII/payment refs
- [ ] AuthZ checked (buyer owns buyer resources; admin/staff gated)
- [ ] Payment invariant respected (Payoo: no order before success)

## Observability
- Correlation ID present in responses and logs
- Key events logged (structured): e.g. order created, payment initiated, payment callback handled

## Screenshots / UI (if applicable)
- Before:
- After:

## Notes / Assumptions / Follow-ups
- Assumptions:
  - -
- Follow-ups (new issues):
  - -
