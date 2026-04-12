# Testing Rules

## Baseline Verification

After structural or behavioral changes, run:

```powershell
dotnet build CleanArchitecture.slnx
dotnet test tests/ArchitectureTests/ArchitectureTests.csproj
```

## Test Projects

- `tests/ArchitectureTests` enforces layer boundaries and structural rules
- `tests/Application.UnitTests` is for isolated unit tests of handlers, services, validators, and domain logic
- `tests/Application.IntegrationTests` is for end-to-end application and persistence scenarios
- `tests/TestInfrastructure` is for shared test helpers

## What to Test

Unit test:

- domain entity factories and behavior methods
- value-object creation and validation
- command handlers and application services with mocked collaborators
- validators

Integration test:

- dispatcher or service flows against real persistence
- repository round trips
- `DbContext` configuration and event-dispatch behavior

## Review Mindset

When reviewing a change, prioritize:

- broken layer boundaries
- mismatched service-vs-CQRS decisions
- business failures handled outside `Result`
- anemic domain entities where the feature clearly needs aggregate behavior
