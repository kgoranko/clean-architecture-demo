# Domain Rules

## Entities and Aggregates

- entities use private setters
- aggregates are rich objects, not anemic property bags
- create new aggregates through `Create(...)` factory methods that return `Result<T>`
- use `Rehydrate(...)` only for repository or persistence materialization
- derived state belongs inside the aggregate, for example `User.FullName`
- state transitions and invariants belong on domain methods such as `Confirm()`, `Deactivate()`, or `MarkWelcomeEmailSent()`
- `Application` must not build domain entities with `new Entity { ... }`

## Value Objects

- value objects are immutable
- create them through factory methods returning `Result<T>`
- use value-object semantics when equality is based on atomic values instead of identity

## Errors and Outcomes

- use `Result` and `Result<T>` for business outcomes
- business-rule failures return `Result.Failure(...)`
- do not throw exceptions for normal business validation

## Domain Events

This repository keeps two domain-event styles visible on purpose:

- use a custom semantic event when the business occurrence matters, for example `OrderSavedDomainEvent`
- use generic lifecycle events such as `EntityCreatedDomainEvent<TEntity>` and `EntityUpdatedDomainEvent<TEntity>` only when demonstrating persistence lifecycle hooks

Rules for the generic lifecycle-event path:

- create generic lifecycle events in `DemoDbContext.SaveChangesAsync()` from tracked entity state
- dispatch those events through `IDomainEventsDispatcher`
- do not raise generic lifecycle events from repositories
- do not manually dispatch generic lifecycle events from command handlers
- closed generic handlers are entity-specific, for example `IDomainEventHandler<EntityCreatedDomainEvent<User>>`

## Boundary Placement

- aggregate persistence contracts belong in `Domain` by default, for example `IUserRepository` and `IOrderRepository`
- outbound provider abstractions used by application orchestration belong in `src/Application/Abstractions/{Concern}`
