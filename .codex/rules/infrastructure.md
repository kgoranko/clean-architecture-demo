# Infrastructure Rules

## Responsibilities

`Infrastructure` implements persistence and external boundaries.

Examples:

- repository implementations
- EF Core `DbContext` and entity configurations
- email, payment, and time providers
- domain-event dispatch plumbing

## Naming

- infrastructure implementations use names such as `*Repository`, `*Provider`, `*Client`, `*Runner`, or `*Adapter`
- do not create `*Service` classes in `Infrastructure`

## Persistence

- the active demo persistence entry point is `src/Infrastructure/Persistence/DemoDbContext.cs`
- EF Core configurations live under `src/Infrastructure/Persistence/Configurations`
- the SQL Server bootstrap script lives at `docs/sql/create-demo-tables.sql`
- keep schema changes reflected in the SQL script unless the user explicitly requests EF migrations

## Result and Error Handling

- prefer explicit failure propagation over swallowing infrastructure errors
- return `Result` or `Result<T>` where repository contracts require business-safe error propagation
- log meaningful infrastructure failures with structured logging

## Domain Events

- `DemoDbContext.SaveChangesAsync()` is responsible for dispatching generic lifecycle events after persistence succeeds
- infrastructure wiring should keep that dispatch centralized rather than scattering manual event calls through handlers and repositories

## Registration

- concrete infrastructure services declare lifetime through `ITransientService`, `IScopedService`, or `ISingletonService`
- registration is performed through Scrutor scanning with `AsSelfWithInterfaces()`
