# Modules

This folder is reserved for future dedicated module boundaries.

Use it when a future demo needs its own compact module boundary:

```text
src/Modules/{Module}/
  {Module}/
    Domain/
    Application/
    DependencyInjection.cs
  {Module}.Infrastructure/
    Persistence/
    DependencyInjection.cs
```

Until then, the current demo remains in the shared `Domain`, `Application`, and `Infrastructure` projects.
