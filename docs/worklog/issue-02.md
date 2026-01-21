# Worklog — Issue #01 — Bootstrap (Clean Architecture + REST + React + CI)

## Status
✅ Completed on `main` (baseline foundation merged)

## CI Pass
- Backend (.NET): ✅ restore/build/test
- Frontend (React/Next.js): ✅ install/lint/test/build
- E2E (Playwright): ✅ install browsers + run tests (or smoke)

> Evidence: GitHub Actions run(s) on `main` are green after the foundation merge.

## Repo Structure (baseline)
.
├─ src/
│ ├─ Cms.sln
│ ├─ Api/
│ ├─ Application/
│ ├─ Domain/
│ └─ Infrastructure/
├─ tests/
│ ├─ Api.IntegrationTests/
│ └─ Application.UnitTests/
├─ apps/
│ └─ web/
├─ .github/
│ ├─ workflows/
│ │ └─ ci.yml
│ └─ ISSUE_TEMPLATE/
│ └─ vertical-slice.md
├─ AGENTS.md
├─ PULL_REQUEST_TEMPLATE.md
├─ copilot-instructions.md
└─ README.md

## What was delivered (summary)
- Clean Architecture skeleton (Api/Application/Domain/Infrastructure)
- REST conventions: controller-thin approach (controllers only orchestrate)
- `/health` endpoint (used by CI/E2E smoke)
- Unit + integration test projects
- Web app skeleton under `apps/web`
- CI workflow that builds/tests BE + FE + E2E

## Commands to run locally
### Backend
```bash
dotnet restore src/Cms.sln
dotnet build src/Cms.sln -c Release
dotnet test src/Cms.sln -c Release
dotnet run --project src/Api --urls http://127.0.0.1:5001
# then open:
# http://127.0.0.1:5001/health

Frontend
cd apps/web
npm ci
npm run lint --if-present
npm test --if-present
npm run build
npm run start
# then open:
# http://127.0.0.1:3000
E2E (Playwright)
cd apps/web
npx playwright install --with-deps
npm run e2e
Notes / Known limitations
No database wiring yet (intentionally minimal foundation).
Auth, catalog, checkout, CMS modules are implemented in later issues.
Reference PR (foundation merge)
PR: <PASTE_PR_URL_HERE>
Example: https://github.com/<org>/<repo>/pull/<number>

---

## `docs/worklog/issue-02.md`
```md
# Worklog — Issue #02 — Platform Conventions (REST errors/logging/correlation + guardrails)

## Status
✅ Completed on `main` (conventions & guardrails available for all future slices)

## CI Pass
- Backend (.NET): ✅ restore/build/test
- Frontend (React/Next.js): ✅ build pipeline green
- E2E (Playwright): ✅ green on main (or smoke)

> Evidence: GitHub Actions run(s) on `main` remain green after this foundation PR.

## Repo Structure (relevant additions)
.
├─ src/
│ ├─ Api/
│ │ ├─ (middlewares / filters if added)
│ │ └─ Program.cs
│ ├─ Application/
│ │ └─ (use-cases + validation conventions)
│ └─ Infrastructure/
│ └─ (logging providers / adapters if added)
├─ .github/
│ ├─ workflows/ci.yml
│ └─ ISSUE_TEMPLATE/vertical-slice.md
├─ AGENTS.md
├─ PULL_REQUEST_TEMPLATE.md
└─ copilot-instructions.md

## What was delivered (summary)
- REST API conventions enforced for future PRs:
  - Controller-thin rule (business logic stays in Application/Domain)
  - Standard error responses (ProblemDetails)
  - Correlation id propagation (`X-Correlation-Id`)
  - Logging rules (no secrets/PII; mask tokens; safe structured logs)
- Copilot guardrails:
  - `AGENTS.md` for boundaries + naming
  - `PULL_REQUEST_TEMPLATE.md` for consistent PR quality gates
  - `copilot-instructions.md` to keep implementation “on rails”
  - `vertical-slice.md` template for consistent issue scope

## Commands to run locally (verification)
### Backend verification
```bash
dotnet test src/Cms.sln -c Release
dotnet run --project src/Api --urls http://127.0.0.1:5001
# verify:
# - /health returns 200
# - correlation id present in responses/headers (if implemented)
# - errors return ProblemDetails (if you trigger a 404/validation in later slices)
CI verification (same as workflow)
# backend
dotnet restore src/Cms.sln
dotnet build src/Cms.sln -c Release --no-restore
dotnet test  src/Cms.sln -c Release --no-build

# frontend
cd apps/web
npm ci
npm run build
Notes / Policies (for future slices)
Never log: passwords, OTP, tokens, payment references, full email/phone.
Any new endpoint must:
return proper HTTP status codes
return ProblemDetails on validation errors
enforce authz boundaries (/admin/* vs /me/*)
Any BE+FE slice should include a minimal E2E happy path.
Reference PR (foundation conventions merge)
PR: <https://github.com/dungvt05hc/cms-management/pull/25>
Example: https://github.com/<org>/<repo>/pull/<number>

---