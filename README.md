# Clean Architecture Demo

This repository is a small, self-contained .NET 10 Clean Architecture demo.

It demonstrates two application-boundary styles:

- `DirectDi` - a Razor Page calling an application service directly
- `CQRS` - a Razor Page flowing through `ICommandDispatcher`, command behaviors, and a handler

## Structure

```text
src/
  SharedKernel/
  Domain/
  Application/
  Infrastructure/
  Modules/
  Web.App/
  Web.Gateway/

tests/
  ArchitectureTests/
  Application.UnitTests/
  Application.IntegrationTests/
  TestInfrastructure/
```

## Guidance

- project rules: `AGENTS.md`
- Codex rule files: `.codex/rules/`
- structure notes: `docs/guides/project-structure.md`
- guidance decision: `docs/adr/0001-establish-self-contained-guidance.md`

## Database Setup

The demo uses SQL Server persistence through `ConnectionStrings:DemoDatabase` in `src/Web.App/appsettings.json`.

1. Create the database, for example `CleanArchitectureDemo`.
2. Execute `docs/sql/create-demo-tables.sql` inside that database.
3. Adjust the connection string if you are not using LocalDB.

## Verification

```powershell
dotnet build CleanArchitecture.slnx
dotnet test tests/ArchitectureTests/ArchitectureTests.csproj
```
