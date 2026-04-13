# Application Rules

## Current Patterns

This repository uses both:

- application services for reads and simple orchestration
- CQRS commands for behavior that benefits from a command pipeline

The active CQRS path uses:

- `IDispatcher`
- `IRequestHandler<TRequest, TResponse>`
- `IRequestBehavior<TRequest, TResponse>`

UI entry points should depend on `IDispatcher` rather than resolving handlers directly. Commands and queries can keep semantic handler wrappers such as `ICommandHandler<TCommand, TResponse>` or `IQueryHandler<TQuery, TResponse>`, but those wrappers flow through the shared request dispatcher contracts.

## Folder Layout

Inside `src/Application`, keep each aggregate or feature area directly under its own root folder:

```text
src/Application/{Aggregate}/
  Abstractions/
  Dtos/
  Services/
  {CommandName}/
```

Rules:

- service interfaces live in `Abstractions/`
- service request/response DTOs live in `Dtos/`
- application service implementations live in `Services/`
- CQRS writes use one folder per command
- a command folder contains the command, handler, and validator when needed
- do not add a generic `Features/` container

Shared outbound contracts that support orchestration across aggregates live under:

```text
src/Application/Abstractions/{Concern}/
```

## Command Rules

- commands are `public sealed record`
- handlers are `internal sealed`
- validators stay in the same command folder
- handlers return `Result` or `Result<T>`
- do not inject `DbContext` directly into handlers
- do not reference infrastructure implementations from `Application`

## Service Rules

- service implementations belong in `Services/`
- `*Service` is reserved for the Application layer
- services return `Result` or `Result<T>` for business outcomes
- if a service grows command-like orchestration concerns, promote it to a CQRS command flow

## Registration

- concrete services in `Application` declare lifetime with `ITransientService`, `IScopedService`, or `ISingletonService`
- DI registration uses Scrutor scanning with `AsSelfWithInterfaces()`
