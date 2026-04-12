# Workflow Rules

Use this workflow for any change that would modify code, behavior, or project structure.

## Required Gate

1. Inspect the relevant implementation first.
2. Present a concise plan.
3. Ask one combined approval question, for example: `If you approve the plan, I'll create a local safety commit first and proceed with the implementation.`
4. Treat short approvals such as `go` or `do it` as approval plus local safety commit.
5. If the user explicitly says to continue without the commit, skip the commit and proceed.
6. If the reply is unclear, ask again before editing files.
7. Only then start editing files.

Skip this gate only when the prompt explicitly instructs immediate implementation without waiting.

## Working Rules

- read files before modifying them
- prefer extending existing files over creating new ones
- keep changes minimal and directly tied to the request
- do not create empty placeholder folders or filler files
- keep temporary or analysis scripts out of tracked source folders

## Push Workflow

- when the user explicitly asks to `push` to Git, first prepare one compact final commit message for the push
- show that compacted text to the user and ask for approval before changing git history
- after the user approves the compacted text, squash local commits into one commit and use the approved compacted text for that squash
- do not squash local commits for ordinary work unless the user asked for push or explicitly asked for squash

## Local Helper Scripts

- when a script is useful for analysis, validation, test setup, or data inspection, prefer a small Python helper script over an opaque shell one-liner
- store helper scripts under `.artifacts/codex-scripts/`
- scripts in `.artifacts/codex-scripts/` may remain after the task for reuse because the folder is not tracked by Git
- do not place one-off helper scripts under `src/`, `tests/`, or `docs/` unless they are intended to become maintained project assets

## Completion

After structural or behavioral changes, run:

```powershell
dotnet build CleanArchitecture.slnx
dotnet test tests/ArchitectureTests/ArchitectureTests.csproj
```
