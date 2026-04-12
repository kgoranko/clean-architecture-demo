---
name: new-demo-feature
description: Add a new demo feature using this repo's service-vs-CQRS rules
argument-hint: <FeatureName> <service|cqrs> [ModuleName]
---

# New Demo Feature

Create a new demo feature named **$0**.

## Workflow Gate

Before editing files:

1. inspect the relevant flow and target folders
2. present a concise implementation plan
3. ask one combined approval question, for example: `If you approve the plan, I'll create a local safety commit first and proceed with the implementation.`
4. treat `go` or `do it` as approval plus a local safety commit
5. if the user explicitly says to continue without the commit, skip the commit
6. if the reply is unclear, ask again before editing files

If the user later asks to push to Git:

1. prepare one compact final commit text
2. show that text to the user and ask for approval
3. after approval, squash local commits into one commit and use the approved text

If a helper script is useful for analysis, generation, or validation, place it under `.artifacts/codex-scripts/` so it stays out of Git and can be reused later.

`$1` decides the application pattern:

- `service` for reads and simple writes
- `cqrs` for multi-step or side-effect-heavy writes

Optional `$2` is the UI folder name under `src/Web.App/Pages/Modules/`.

For folder naming, pick the closest business aggregate name from the feature:

- `OrderProcessing` -> `Orders`
- `RegisterUser` -> `Users`

## Placement Rules

- shared domain types: `src/Domain/{Aggregate}/`
- shared application code lives directly under `src/Application/{Aggregate}/`
- shared outbound application contracts live under `src/Application/Abstractions/{Concern}/`
- infrastructure collaborators: `src/Infrastructure/`
- SQL Server persistence mapping: `src/Infrastructure/Persistence/`
- table setup script: `docs/sql/create-demo-tables.sql`
- UI pages:
  - shared entry or overview: `src/Web.App/Pages/`
  - module-like demo page: `src/Web.App/Pages/Modules/{ModuleName}/`

## If Pattern Is `service`

Create or extend:

- `src/Application/{Aggregate}/Abstractions/I{FeatureName}Service.cs`
- `src/Application/{Aggregate}/Dtos/*`
- `src/Application/{Aggregate}/Services/{FeatureName}Service.cs`

The page may inject the application service directly.

## If Pattern Is `cqrs`

Create or extend:

- `src/Application/{Aggregate}/{CommandName}/{CommandName}Command.cs`
- `src/Application/{Aggregate}/{CommandName}/{CommandName}CommandHandler.cs`
- `src/Application/{Aggregate}/{CommandName}/{CommandName}CommandValidator.cs` when needed

The page should use `ICommandDispatcher` instead of the concrete handler implementation.

## Always Enforce

- `Result<T>` for success or failure
- no business exceptions
- no infrastructure references from `Application`
- domain entities are rich aggregates with private setters
- create aggregates through `Create(...)` factory methods and use `Rehydrate(...)` only for repository materialization
- put derived state and state transitions on the entity instead of setting domain properties directly from `Application`
- do not use `new Entity { ... }` object initializers in `Application` for domain aggregates
- infrastructure implementations use `*Provider`, `*Client`, `*Runner`, or `*Adapter`, not `*Service`
- concrete application/infrastructure services should declare lifetime through `ITransientService`, `IScopedService`, or `ISingletonService`
- CQRS cross-cutting concerns should be implemented as `ICommandBehavior<TCommand, TResponse>` rather than handler decorators
- no generic `src/Application/Features` folder
- page copy stays English unless the task explicitly calls for localized messaging

## Domain Event Demo Rule

This repository intentionally demonstrates both event approaches:

- use a custom semantic event when the occurrence is part of the business language, for example `OrderSavedDomainEvent`
- use generic lifecycle events such as `EntityCreatedDomainEvent<TEntity>` and `EntityUpdatedDomainEvent<TEntity>` only when demonstrating persistence lifecycle hooks
- create generic lifecycle events in `DemoDbContext.SaveChangesAsync()` from tracked entity state, then dispatch them through `IDomainEventsDispatcher`
- do not raise generic lifecycle events from repositories or manually dispatch them from command handlers
- register/consume closed generic handlers such as `IDomainEventHandler<EntityCreatedDomainEvent<User>>` when the handler must catch only one entity type
- do not convert the order flow to generic lifecycle events; keep it as the custom-event example

## Finish With

```powershell
dotnet build CleanArchitecture.slnx
dotnet test tests/ArchitectureTests
```
