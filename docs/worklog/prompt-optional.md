Use Fullstack Slice Builder agent. Implement this issue as ONE PR with two phases:

Phase A (Backend):
- Implement the API contract exactly as written (REST controllers, thin controllers).
- Clean Architecture boundaries: Api -> Application -> Domain -> Infrastructure.
- Add integration tests: happy path + at least 1 negative case.
- If migration is required in the issue, add EF Core migration.
- Ensure dotnet build/test pass.

Phase B (Frontend + E2E):
- Implement UI routes/screens in apps/web that match the issue UI contract.
- Add Playwright E2E happy path using data-testid selectors.
- Ensure npm build + e2e pass.

Constraints:
- No scope creep beyond AC.
- Use ProblemDetails for errors.
- Never log secrets/OTP/passwords/tokens; mask phone/email.
- CI must be green. Add “Closes #<issue-number>” in PR description.
