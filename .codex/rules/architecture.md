# Architecture Rules

## Active Dependency Flow

```text
SharedKernel <- Domain <- Application <- Infrastructure <- Web.App
```

- `SharedKernel` contains shared primitives such as `Result`, `Error`, base entities, domain-event abstractions, and service markers
- `Domain` references only `SharedKernel`
- `Application` references `Domain` and `SharedKernel`
- `Infrastructure` references `Application`, `Domain`, and `SharedKernel`
- `Web.App` composes the running application and is the only active UI project

`Application` must not reference `Infrastructure`.

`Domain` must not reference `Application` or `Infrastructure`.

## Source Tree

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

## Web Layer

- all active UI lives in `src/Web.App`
- shared Razor Pages layout and shell stay in `src/Web.App/Pages/Shared`
- demo-specific pages go under `src/Web.App/Pages/Modules/{DemoName}`
- do not create empty placeholder folders inside `src/Web.App`

## Future Growth

- `src/Modules` is reserved for future dedicated module boundaries
- `src/Web.Gateway` is reserved for future gateway or reverse-proxy work
- if a feature does not justify its own module boundary, keep it in the shared `Domain`, `Application`, and `Infrastructure` projects

## Architecture Tests

Use `tests/ArchitectureTests` to enforce layer boundaries, naming, and structural conventions.
