You are an autonomous coding agent running inside my repo workspace. Scaffold a NEW monorepo web app with:
- Backend: .NET 9 ASP.NET Core Web API (apps/api)
- Frontend: React + Vite + TypeScript (apps/web)
- Database: Postgres via docker-compose (infra/docker)
- API contract + typed client generation from OpenAPI (packages/contracts)
- CI: GitHub Actions builds + tests + lint for both backend and frontend
- Linting/formatting: C# + TS/React (eslint + prettier); keep defaults sensible and consistent

YOU MUST USE THESE BUSINESS INPUTS AS SOURCE OF TRUTH
- /docs/product/Requirements-Spec.md
- /docs/product/Backlog.csv
- /docs/product/Data-Model.md
- /docs/product/User-Flows.md
If anything in those docs is ambiguous, record it in /docs/product/Open-Questions.md and implement the safest minimal default.

HARD CONSTRAINTS
- Do NOT add secrets to the repo. Provide .env.example files and reference env vars.
- Keep it minimal but complete: implement the MVP slice from /docs/product/Backlog.csv (first 3–7 stories) as an end-to-end vertical slice that exercises DB + API + UI.
- Everything must run on macOS and CI (Ubuntu).
- Definition of Done:
  1) `docker compose up -d` starts Postgres
  2) `dotnet build` and `dotnet test` pass (document exact commands)
  3) `npm ci`, `npm run lint`, `npm run test`, `npm run build` pass (web)
  4) API client generation works and web uses the generated client
  5) GitHub Actions workflow passes on a fresh checkout

REPO STRUCTURE (create exactly)
repo-root/
  apps/
    api/
      src/Api/                      # .NET 9 Web API
      src/Application/              # use cases / handlers (vertical slices or simple services)
      src/Domain/                   # entities / value objects
      src/Infrastructure/           # EF Core, integrations
      tests/Api.Tests/              # xUnit tests (include at least 1 integration test)
      Api.sln
    web/
      src/
        app/
        features/
        shared/
      vitest.config.ts
      package.json
  packages/
    contracts/
      openapi.json                  # generated OpenAPI artifact
      src/generated/                # generated TS client/types
      src/index.ts                  # exports createApiClient + types
      package.json
  infra/
    docker/
      docker-compose.yml
  docs/
    product/
      (already exists)
  .github/
    workflows/
      ci.yml
  .editorconfig
  README.md
  package.json                      # root npm workspaces + scripts
  .gitignore
  .env.example

TECH DECISIONS (use these defaults unless business docs require otherwise)
BACKEND (.NET 9)
- ASP.NET Core Web API using minimal hosting model
- EF Core + Npgsql provider
- Database migrations included (create initial migration(s) for MVP entities)
- Configuration via env var: ConnectionStrings__Default
- Add Swagger/OpenAPI via Swashbuckle
- Add health endpoint: GET /health
- Implement endpoints required for the MVP stories (from Backlog.csv), with:
  - Basic validation (400 on invalid inputs)
  - Error responses that are consistent (ProblemDetails)
- Tests:
  - Use Testcontainers for Postgres preferred; if blocked, use GitHub Actions Postgres service connection and config tests accordingly
  - At least one test that hits API and verifies DB persistence

FRONTEND (React)
- Vite + React + TS
- Implement UI required for MVP stories
- Use generated client from packages/contracts (no handwritten fetches for MVP endpoints)
- Env var for API base URL:
  - Vite env: VITE_API_BASE_URL (default http://localhost:5080)
- Lint: eslint + prettier
- Test: vitest + react testing library (one smoke test for MVP)

API CONTRACT + CLIENT GENERATION (must work in CI)
- Generate OpenAPI JSON from the built API assembly using Swashbuckle CLI:
  - Add dotnet tool so CI can run:
    `dotnet tool restore` then
    `dotnet swagger tofile --output packages/contracts/openapi.json apps/api/src/Api/bin/Release/net9.0/Api.dll v1`
- Generate TS client/types in packages/contracts using openapi-typescript.
  - Provide `npm run gen:client` at repo root
  - Output to: packages/contracts/src/generated
  - Export from packages/contracts: `createApiClient(baseUrl)` that web can import.

WORKSPACES + SCRIPTS (root package.json)
- Use npm workspaces:
  - "apps/web"
  - "packages/contracts"
- Root scripts:
  - "dev:db" -> docker compose up -d (infra/docker)
  - "dev:api" -> run API
  - "dev:web" -> run web
  - "gen:openapi" -> builds api Release + swagger tofile into packages/contracts/openapi.json
  - "gen:client" -> generates TS output
  - "gen" -> runs both gen steps
  - "test" -> backend + web tests
  - "lint" -> web eslint/prettier (and dotnet format if available)
  - "build" -> backend + web build

CI (.github/workflows/ci.yml)
- Trigger on push + PR
- Steps:
  1) checkout
  2) setup-dotnet (9.x)
  3) setup-node (20.x)
  4) start Postgres service (GitHub Actions services) with env vars
  5) dotnet restore/build/test for apps/api
  6) build API Release and generate openapi.json
  7) npm ci at repo root (workspaces)
  8) generate TS client
  9) web lint/test/build
- Cache npm and nuget (optional)

LOCAL DEV UX
- Provide README.md with:
  - prerequisites (Docker, .NET 9, Node 20)
  - how to start db/api/web
  - how to run migrations
  - how to regenerate client
  - how to run tests + lint

IMPLEMENTATION STEPS (you must execute these, not just describe)
1) Create folder structure and solution/projects
2) Implement Domain/Application/Infrastructure structure minimally
3) Implement MVP endpoints and DB migrations from Backlog.csv
4) Implement packages/contracts generation scripts
5) Implement web app consuming generated client for MVP features
6) Add tests (at least one DB-backed integration test)
7) Add CI workflow
8) Run all commands to verify DoD. Fix until green.

OUTPUT EXPECTATION
- Make real commits with clear messages (if git is available). If not, ensure files are correct.
- Do NOT ask me questions unless blocked by a missing tool/command.
- If you must choose ports, use:
  - API: http://localhost:5080
  - Web: http://localhost:5173
  - Postgres: localhost:5432 (db: appdb, user: appuser, pass: apppass)

Now start. Implement everything above based on the business docs, run tests/build/lint, and ensure it’s passing.
