# AGENTS.md

## Purpose

This repository is a self-contained Clean Architecture demo. Everything needed to implement work here should live in this repo.

Use `AGENTS.md` as the entry point and `.codex/rules/*` as the detailed source of truth for implementation guidance.

## Read First

1. `AGENTS.md`
2. `.codex/rules/workflow.md`
3. `.codex/rules/architecture.md`
4. `.codex/rules/application.md`
5. `.codex/rules/service-vs-cqrs.md`
6. `.codex/rules/domain.md`
7. `.codex/rules/infrastructure.md`
8. `.codex/rules/code-style.md`
9. `.codex/rules/testing.md`
10. `.codex/rules/modules.md` when introducing a dedicated module boundary
11. `docs/guides/project-structure.md`

## Current Project Shape

- `src/SharedKernel`
  Shared primitives such as `Result`, `Error`, domain-event abstractions, and service markers.
- `src/Domain`
  Shared domain entities and aggregate persistence contracts.
- `src/Application`
  Application services, CQRS commands, handlers, validators, behaviors, and outbound contracts.
- `src/Infrastructure`
  Repository implementations, EF Core persistence, providers, and integration plumbing.
- `src/Web.App`
  The active Razor Pages UI project.
- `src/Modules`
  Reserved for future dedicated module boundaries.
- `src/Web.Gateway`
  Reserved for future gateway or reverse-proxy work.
- `tests/ArchitectureTests`
  Machine-enforced architecture conventions.
- `tests/Application.UnitTests`
  Unit tests for isolated application and domain behavior.
- `tests/Application.IntegrationTests`
  Integration tests for dispatcher, service, and persistence flows.
- `tests/TestInfrastructure`
  Shared test helpers.

## Non-Negotiables

- inspect first, present a plan, then ask one combined approval question; default `go` or `do it` to approval plus a local safety commit, and ask again only if the reply is unclear
- keep dependency flow `SharedKernel <- Domain <- Application <- Infrastructure <- Web.App`
- keep active UI in `src/Web.App`
- keep aggregate-root application folders directly under `src/Application/{Aggregate}`
- keep shared outbound application abstractions under `src/Application/Abstractions/{Concern}`
- keep domain entities rich and prevent `new Entity { ... }` construction from `Application`
- keep aggregate persistence contracts in `Domain` by default and orchestration-facing provider abstractions in `Application`
- use `ICommandDispatcher` for CQRS entry points and `ICommandBehavior<TCommand, TResponse>` for command pipeline concerns
- keep the generic lifecycle-event path centralized in `DemoDbContext.SaveChangesAsync()`
- use `.artifacts/codex-scripts/` for helper scripts instead of tracked folders
- do not keep empty placeholder folders in `Web.App`
- when asked to push to Git, first propose one compact final commit text, get user approval for that text, then squash local commits into one commit before pushing

## Topic Map

- workflow rules: `.codex/rules/workflow.md`
- layering and project shape: `.codex/rules/architecture.md`
- application layout and command pipeline: `.codex/rules/application.md`
- service-vs-command decision rules: `.codex/rules/service-vs-cqrs.md`
- rich domain and event rules: `.codex/rules/domain.md`
- persistence and provider rules: `.codex/rules/infrastructure.md`
- style and build expectations: `.codex/rules/code-style.md`
- verification and test focus: `.codex/rules/testing.md`
- future module boundaries: `.codex/rules/modules.md`

## Local Skills

Use the local skills under `.codex/skills/` when they match the task:

- `onboard`
- `preflight`
- `new-demo-feature`
- `new-demo-module`
- `review-architecture`
