# ADR 0001: Establish Self-Contained Guidance

## Status

Accepted

## Context

This demo repository had guidance that depended on paths and conventions stored outside the repo. That made the instructions unusable for teammates who only have this project locally.

The repo also needs implementation guidance that is specific to this demo:

- Clean Architecture layers
- a hybrid `Application Service` vs `CQRS` application layer
- rich domain entities and explicit `Result<T>` usage
- SQL Server demo persistence
- a Razor Pages UI in `Web.App`

## Decision

We keep the guidance self-contained by:

- treating `AGENTS.md` as the repo entry point
- storing detailed implementation rules under `.codex/rules/`
- keeping local task skills under `.codex/skills/`
- keeping human-facing structure notes in `docs/guides/`
- removing machine-specific references from repo guidance

## Consequences

Positive:

- all contributors can follow the same rules without local access to external projects
- Codex instructions and human-readable docs now point to local files only
- implementation guidance can evolve with this repo without dragging unrelated production details into the demo

Trade-offs:

- the repo now owns its own rule set and must keep it accurate
- some future changes may need both `.codex/rules/` and `docs/` updates to stay consistent
- `src/Web.Gateway` and most of `src/Modules` remain reserved until the demo grows into them
