# Service vs CQRS Rules

Use these rules for new implementation work.

## Default Decision

| Operation | Pattern |
|---|---|
| any read | application service |
| write with one persistence boundary and no side effects | application service |
| write with domain events, multiple repositories, external calls, strategy/policy dispatch, or background work | CQRS command |

## What Counts as a Side Effect

- dispatching domain events
- writing through multiple repositories
- calling external providers or clients
- enqueuing background work
- selecting behavior through strategies or policies

## Existing Demo Exception

This repo intentionally keeps two contrasting demo flows:

- `OrderProcessing` remains the direct application-service example
- `RegisterUser` remains the CQRS command-dispatcher example

Treat those flows as comparison demos. For new work, follow the rule table above instead of copying the comparison setup blindly.
