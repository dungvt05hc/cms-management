You are Codex running in agent mode with access to my local workspace files and the ability to run commands. I have an Excel file that defines business modules and requirements:

File: /mnt/data/TaskBreakDown_Bidding.xlsx
Sheets: "Task Breakdown" and "Phụ_Lục_01"

GOAL
Read both sheets, understand the business modules, and produce a developer-ready specification + backlog for building the system (monorepo: .NET 9 API + React). Your outputs will be used as the source of truth for scaffolding the repo.

HARD RULES
- Use the Excel as the source of truth. Don’t invent requirements not implied by the file.
- If something is ambiguous or missing, list it explicitly as “Open Questions” instead of guessing.
- Keep all outputs structured and easy to implement.
- Preserve original Vietnamese terms where they matter (field names, module names, roles), and include an English explanation next to them.
- Do not include any secrets. If credentials are mentioned, convert to placeholders.

WHAT YOU MUST DO (execute steps, not just describe)
1) Inspect the Excel file:
   - Write a small script (Python preferred) to read both sheets.
   - Print: number of rows/cols, detected header row(s), and a sample of 5–10 rows per sheet.
   - Detect merged cells and multi-row headers if present.
   - Save a normalized JSON snapshot to: /docs/product/_raw/excel-normalized.json
2) Build a normalized model:
   - Identify modules/epics/features.
   - Identify user roles and actors.
   - Identify workflows/processes (happy path + key variants).
   - Identify entities/data objects and their fields (if present).
   - Identify validations, rules, and acceptance criteria.
   - Identify integrations/external systems (if any).
3) Cross-check between sheets:
   - Map items from "Task Breakdown" to details in "Phụ_Lục_01" (or vice versa).
   - Flag mismatches, duplicates, missing references.
4) Produce the following outputs as files in /docs/product (create directories if needed):
   A) /docs/product/Business-Module-Overview.md
      - Module list with descriptions and scope boundaries
      - Actors/roles
      - Glossary (VN term -> EN explanation)
   B) /docs/product/Requirements-Spec.md
      - Per module: goals, assumptions, constraints
      - Functional requirements (numbered)
      - Non-functional requirements (only if stated)
      - Business rules (numbered)
      - Acceptance criteria per feature/story
   C) /docs/product/User-Flows.md
      - Step-by-step flows for each major workflow
      - Include at least 1 Mermaid flowchart or sequence diagram per workflow
   D) /docs/product/Data-Model.md
      - Entities, key fields, relationships
      - Include Mermaid ER diagram (best-effort from Excel info)
   E) /docs/product/Backlog.csv
      Columns:
        Epic,StoryID,StoryTitle,Description,AcceptanceCriteria,Priority,Dependencies,SourceSheet,SourceRow
      - One row per implementable story
   F) /docs/product/Open-Questions.md
      - Ambiguities, missing values, decisions needed (with pointers to exact sheet/row/column)
   G) /docs/product/MVP-Recommendation.md
      - Recommend an MVP slice (3–7 stories) and rationale, based purely on Excel scope and dependencies

TRACEABILITY REQUIREMENT
For every module/story/rule you extract, include a reference in the format:
  [Source: <SheetName> r<rowNumber> c<colNameOrIndex>]
If a requirement is inferred from multiple rows, list multiple sources.

QUALITY BAR
- Be concise but complete. Prefer bullet lists and tables.
- Do not output raw Excel dumps except small snippets needed for evidence.
- End by ensuring Backlog.csv is coherent (no missing IDs, dependencies refer to existing StoryIDs).

NOW DO IT
Start by creating: tools/parse_excel.py
- Use: pandas + openpyxl
- Include logic to handle merged cells (best effort) and multi-row headers
- Print a profiling summary to stdout

Then generate all docs listed above under /docs/product.
