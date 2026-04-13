# Project Structure Guide

This repository is intentionally organized as a small, self-contained Clean Architecture demo with room for future growth.

## High-Level Layout

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

docs/
  adr/
  guides/
  sql/
```

## What This Repo Standardizes

- shared core layers live directly under `src/`
- UI lives in `Web.App`
- room exists for future module-specific code under `src/Modules`
- tests are split by purpose rather than kept in one folder
- docs live at the repo root instead of being buried inside a source project

## What Is Deliberately Different

- `Web.App` is Razor Pages, not Blazor
- the demo uses shared-layer features instead of real business modules
- `Web.Gateway` is a placeholder, not a running YARP application

## Where New Demo Work Should Go

- shared abstractions: `src/SharedKernel`
- shared domain logic: `src/Domain`
- application services and CQRS handlers: `src/Application`
- shared application-level outbound contracts:
  - `src/Application/Abstractions/{Concern}`
- service-style application features:
  - `src/Application/{Aggregate}/Abstractions`
  - `src/Application/{Aggregate}/Dtos`
  - `src/Application/{Aggregate}/Services`
- CQRS write features:
  - `src/Application/{Aggregate}/{CommandName}`
- repositories and adapters: `src/Infrastructure`
- EF Core SQL Server persistence:
  - `src/Infrastructure/Persistence/DemoDbContext.cs`
  - `src/Infrastructure/Persistence/Configurations`
  - `docs/sql/create-demo-tables.sql`
- UI pages:
  - shared shell: `src/Web.App/Pages/Shared`
  - module-like demo pages: `src/Web.App/Pages/Modules/{DemoName}`
- do not create placeholder folders inside `src/Web.App`; add presentation folders only when they contain real code or assets
- future compact module demos: `src/Modules/{Module}`

## Current Application Examples

- `src/Application/Orders/Abstractions/IOrderProcessingService.cs`
- `src/Application/Abstractions/Notifications/IEmailProvider.cs`
- `src/Application/Abstractions/Payments/IPaymentProvider.cs`
- `src/Application/Orders/Dtos/OrderProcessingRequest.cs`
- `src/Application/Orders/Dtos/OrderProcessingResponse.cs`
- `src/Application/Orders/Services/OrderProcessingService.cs`
- `src/Application/Users/RegisterUser/RegisterUserCommand.cs`
- `src/Application/Users/RegisterUser/RegisterUserCommandHandler.cs`
- `src/Application/Users/RegisterUser/RegisterUserCommandValidator.cs`

## Domain Entity Style

When adding or changing aggregates under `src/Domain/{Aggregate}`:

- prefer rich entities with private setters
- create new aggregates through `Create(...)` factory methods that return `Result<T>`
- use `Rehydrate(...)` only for repository/materialization scenarios
- keep derived values inside the aggregate, for example `User.FullName`
- expose business operations as methods on the aggregate, for example `Order.Confirm()` or `User.Deactivate()`
- do not build domain entities in `Application` with `new Entity { ... }`

## Domain Event Style

This demo keeps two event styles visible:

- `Order` uses the custom `OrderSavedDomainEvent` path
- `User` uses generic lifecycle events from `DemoDbContext.SaveChangesAsync()`
- `DemoDbContext.SaveChangesAsync()` creates and dispatches `EntityCreatedDomainEvent<User>` and `EntityUpdatedDomainEvent<User>` from tracked entity state
- repositories persist aggregates and do not raise or dispatch generic lifecycle events
- the welcome email flow listens to `EntityCreatedDomainEvent<User>` only
- keep both styles available so future demos can compare semantic custom events with generic lifecycle hooks

## Service vs CQRS Rule

Use the repo's local rule:

- read flows and simple single-boundary writes use application services
- multi-step or side-effect-heavy writes use command handlers

Current examples:

- `DirectDi` demo: application service
- `CQRS` demo: request dispatcher pipeline

## DI Registration Style

- concrete services declare lifetime with `ITransientService`, `IScopedService`, or `ISingletonService`
- `Application` and `Infrastructure` register those services through Scrutor scanning with `AsSelfWithInterfaces()`
- CQRS UI entry points should depend on `IDispatcher`
- validation and logging belong in `IRequestBehavior<TRequest, TResponse>` implementations
