---
name: review-architecture
description: Review this demo repo for layer violations, Result-pattern misuse, and structure drift
argument-hint: [path]
---

# Review Architecture

Review the supplied path, or the recently changed files if no path is provided.

## Check These Rules

1. `Application` does not reference `Infrastructure`.
2. `Domain` does not reference `Application` or `Infrastructure`.
3. UI pages stay in `Web.App`.
4. Demo-specific UI lives under `src/Web.App/Pages/Modules/*` when it is not a shared shell page.
5. `Result<T>` is used for business failures.
6. Domain entities are not anemic property bags when the use case needs aggregate behavior.
7. `Application` does not construct domain aggregates via `new Entity { ... }` object initializers.
8. Service-vs-CQRS choice matches the side effects in the feature.
9. New structural work keeps the repo aligned with `AGENTS.md` and `docs/guides/project-structure.md`.

## Verification

Run:

```powershell
dotnet build CleanArchitecture.slnx
dotnet test tests/ArchitectureTests
```
