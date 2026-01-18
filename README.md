Using Codex Prompts (Business → Scaffold)
This repo includes two “runbook” prompts for Codex that let you:
Read and understand business requirements from Excel, then generate product docs + backlog
Scaffold and implement the monorepo (.NET 9 API + React + Postgres) based on those docs
Prompt files
docs/prompts/02_understand-business-from-excel.md
Generates:
docs/product/Requirements-Spec.md
docs/product/Backlog.csv
docs/product/Data-Model.md
docs/product/User-Flows.md
docs/product/Open-Questions.md
docs/product/MVP-Recommendation.md
docs/prompts/01_scaffold-monorepo-dotnet9-react-postgres.md
Uses the above docs as source of truth and scaffolds the repo + MVP slice.
Prerequisites
Docker Desktop
.NET SDK 9.x
Node.js 20.x
(Optional) GitHub CLI (gh) and/or a GitHub token if you want Codex to open PRs
Recommended workflow (do this in order)
Step 1 — Business understanding (Excel → Specs/Backlog)
Open Codex in your IDE (Agent mode recommended).
Open docs/prompts/02_understand-business-from-excel.md
Copy all content and paste into Codex chat.
Approve file reads and command runs if Codex asks.
When finished, confirm these files exist:
docs/product/Backlog.csv
docs/product/Requirements-Spec.md
docs/product/Data-Model.md
docs/product/User-Flows.md
docs/product/Open-Questions.md
Tip: If Codex supports file referencing, you can paste a shorter message like:
“Run the instructions in @docs/prompts/02_understand-business-from-excel.md”
Step 2 — Scaffold + MVP implementation (.NET 9 + React + Postgres)
Open docs/prompts/01_scaffold-monorepo-dotnet9-react-postgres.md
Paste it into Codex chat.
Approve actions (creating files, running dotnet/npm, starting docker, etc.)
When done, Codex should verify:
docker compose up -d (Postgres)
dotnet build / dotnet test
npm ci / npm run lint / npm run test / npm run build
API client generation works and the web app uses it
CI workflow is created under .github/workflows/ci.yml
How to use “@file” context (if supported by your Codex)
When you ask Codex to follow a prompt, prefer referencing the file to avoid copy/paste mistakes:
Business analysis:
“Execute @docs/prompts/02_understand-business-from-excel.md”
Scaffold:
“Execute @docs/prompts/01_scaffold-monorepo-dotnet9-react-postgres.md using @docs/product/Backlog.csv as the source of truth.”
Safety / best practices
Never paste secrets into chat. Use environment variables and .env.example.
If Codex proposes destructive commands (delete directories, reset DB), review carefully before approving.
If Codex is unsure about a requirement, it must write it to:
docs/product/Open-Questions.md
Troubleshooting
Codex can’t read the Excel path
If /mnt/data/TaskBreakDown_Bidding.xlsx is not available in your local environment:
Place the Excel file in your repo root (recommended) and update the prompt path, e.g.:
File: ./TaskBreakDown_Bidding.xlsx
Client generation fails
Make sure you run (or Codex runs) in the correct order:
Build API Release
Generate packages/contracts/openapi.json
Generate TS client into packages/contracts/src/generated
Build web
CI fails on Postgres
Ensure connection strings in CI use the GitHub Actions Postgres service hostname (often localhost + mapped port, or postgres depending on config).
If using Testcontainers, ensure Docker is available in CI runner and tests don’t require privileged mode.