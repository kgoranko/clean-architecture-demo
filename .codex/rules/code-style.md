# Code Style Rules

## Build Quality

This repo treats warnings as errors.

Relevant settings live in `Directory.Build.props`:

- `Nullable=enable`
- `AnalysisMode=All`
- `TreatWarningsAsErrors=true`
- `EnforceCodeStyleInBuild=true`

## Preferred C# Style

- use file-scoped namespaces
- prefer `sealed` classes unless inheritance is intentional
- use primary constructors for dependency-injected classes where it improves clarity
- use `sealed record` for commands, DTOs, and domain events
- use structured logging with message templates, not interpolated log strings
- use braces for conditionals, even when the body is short

## Naming and Text

- code identifiers stay in English
- user-facing and instructional copy is English by default in this demo
- if a task explicitly asks for localized user-facing text, follow that task

## Comments

- prefer self-explanatory code over comment noise
- add comments only when they explain intent or a non-obvious constraint
