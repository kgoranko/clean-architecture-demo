---
name: preflight
description: Run the minimum local quality gates before considering a change complete
---

# Preflight

Run these checks in order:

```powershell
dotnet build CleanArchitecture.slnx
dotnet test tests/ArchitectureTests
```

If the change touched project structure, also verify:

- files landed in the correct source tree
- `AGENTS.md` still reflects reality
- the repo still resembles the reference structure at a high level

If the change touched domain entities, also verify:

- aggregates use private setters and domain methods instead of public mutable state
- `Application` is not creating domain entities with `new Entity { ... }`
