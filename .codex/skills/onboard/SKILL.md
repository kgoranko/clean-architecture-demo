---
name: onboard
description: Read the local Codex guidance and align future work with this repo's architecture
---

# Onboard

Use this skill before substantial work in this repository.

## Workflow Gate

Before changing code, behavior, or structure:

1. inspect the relevant code first
2. present a concise implementation plan
3. ask one combined approval question, for example: `If you approve the plan, I'll create a local safety commit first and proceed with the implementation.`
4. treat `go` or `do it` as approval plus a local safety commit
5. if the user explicitly says to continue without the commit, skip the commit
6. if the reply is unclear, ask again before editing files

If the user later asks to push to Git:

1. prepare one compact final commit text
2. show that text to the user and ask for approval
3. after approval, squash local commits into one commit and use the approved text

If a helper script is useful for analysis or validation, place it under `.artifacts/codex-scripts/`. That folder is intentionally out of Git, and useful scripts may stay there for later reuse.

## Read First

1. `AGENTS.md`
2. `.codex/rules/workflow.md`
3. `.codex/rules/architecture.md`
4. `.codex/rules/application.md`
5. `.codex/rules/service-vs-cqrs.md`
6. `.codex/rules/domain.md`
7. `.codex/rules/infrastructure.md`
8. `.codex/rules/modules.md`
9. `docs/guides/project-structure.md`
10. `src/Web.App/Program.cs`
11. `src/Application/DependencyInjection.cs`
12. `src/Infrastructure/DependencyInjection.cs`

## What to Understand

- shared layers live in `src/SharedKernel`, `src/Domain`, `src/Application`, and `src/Infrastructure`
- aggregate-root application folders live directly in `src/Application/{Aggregate}`
- shared application-level outbound contracts live in `src/Application/Abstractions/{Concern}`
- service interfaces go in `Abstractions`, DTOs in `Dtos`, services in `Services`
- infrastructure implementations use provider/client/runner/adapter naming instead of `*Service`
- CQRS writes use one folder per command under the aggregate root
- active UI lives in `src/Web.App`
- demo-specific pages live in `src/Web.App/Pages/Modules/*`
- future module-like work should use the compact pattern described in `.codex/rules/modules.md`

## Required Mental Model

- `Application Service` for reads and simple single-boundary writes
- `CQRS Command Handler` for multi-step or side-effect-heavy writes
- `Domain` entities are rich aggregates with private setters, factories, and behavior methods
- `Application` orchestrates aggregate creation and method calls, but should not populate domain entities via object initializers
- `Result<T>` for business outcomes
- strict layer boundaries and zero-warning builds
