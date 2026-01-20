# Copilot Instructions — cms-management

You are building a E-comerce product. Always work in **one vertical slice per PR**.

## Guardrails
- 1 Issue = 1 PR. Never implement multiple features in one PR.
- Do not invent business rules. Follow the Issue acceptance criteria.
- Always add tests for new behavior.
- Keep changes scoped; avoid large refactors.

## Architecture
### Backend (.NET)
- Minimal API + Vertical Slice:
  - `src/Api/Features/<Area>/<Action>/Endpoint.cs`
  - Request/Response DTOs in the same folder
  - Validation (FluentValidation if included; otherwise manual)
  - Handler/service in-slice (or use Application layer if it exists)

### Persistence
- EF Core DbContext in `src/Persistence`
- Migrations stored alongside Persistence project
- Prefer explicit indexes for frequently queried columns (only if justified)

### Auth & Security
- Default: secure-by-default.
- Admin endpoints require Admin role/policy.
- Never log tokens, passwords, or PII.
- Return appropriate status codes; do not leak internal exception details.

## Quality gates
Before finalizing PR:
- `dotnet build -c Release`
- `dotnet test -c Release`
- Ensure CI is green

## PR description must include
- Summary
- How to test
- API contract changes (if any)
- Assumptions / open questions
