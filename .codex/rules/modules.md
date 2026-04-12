# Module Rules

Use a dedicated module boundary only when the feature is large enough to justify it.

If it is still a small shared demo feature, keep it in the shared `Domain`, `Application`, and `Infrastructure` projects.

## Compact Module Layout

Default to the compact pattern:

```text
src/Modules/{Module}/
  {Module}/
    Domain/
    Application/
    DependencyInjection.cs
    {Module}.csproj
  {Module}.Infrastructure/
    Persistence/
    DependencyInjection.cs
    {Module}.Infrastructure.csproj

src/Web.App/Pages/Modules/{Module}/
```

## Rules

- keep dependencies flowing inward
- do not bypass the shared-layer dependency flow
- register application and infrastructure boundaries from `Web.App`
- if a presentation-specific registration layer is introduced, keep it in `Web.App`
- add a module only when the user request clearly benefits from a dedicated boundary
