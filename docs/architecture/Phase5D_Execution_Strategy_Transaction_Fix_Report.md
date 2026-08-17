# Phase 5D Execution Strategy Transaction Fix Report

## Outcome

The Phase 5D runtime transaction bug is fixed. Assignment creation no longer opens a user-initiated transaction that conflicts with `SqlServerRetryingExecutionStrategy`. No route, DTO, authorization rule, schema, migration, or unrelated application behavior was changed.

## Root Cause

The shared assignment-creation method opened an explicit transaction with `BeginTransactionAsync`, then executed one `SaveChangesAsync` and explicitly called `CommitAsync` or `RollbackAsync`. SQL Server's configured retrying execution strategy does not support a user-initiated transaction unless the complete unit is executed through `DbContext.Database.CreateExecutionStrategy()`.

The explicit transaction was unnecessary because the complete `DrillAssignment` plus all tracked `DrillAssignmentRecipient` entities are persisted by one `SaveChangesAsync` call. EF Core automatically wraps that relational SaveChanges operation in a transaction.

## Exact Files Modified

- `SkillBuilderPro.API/Services/AssignmentService.cs`
- `docs/architecture/Phase5D_Execution_Strategy_Transaction_Fix_Report.md`

## Exact Methods Changed

- `AssignmentService.CreateAsync`

No public Parent, Coach, or Athlete assignment method was changed. All creation paths continue to converge on the same private `CreateAsync` method.

## Explicit Transaction Behavior Before

`CreateAsync` previously:

1. Called `_dbContext.Database.BeginTransactionAsync`.
2. Constructed one assignment and all validated recipient entities.
3. Added the complete entity graph to the DbContext.
4. Called one `_dbContext.SaveChangesAsync`.
5. Called `transaction.CommitAsync` on success.
6. Called `transaction.RollbackAsync` when catching `DbUpdateException`.

The initial `BeginTransactionAsync` caused the runtime failure under `SqlServerRetryingExecutionStrategy` before the Parent assignment could persist.

## Transaction Behavior After

`CreateAsync` now:

1. Constructs one assignment and all already-validated recipient entities.
2. Adds the complete entity graph to the same DbContext.
3. Calls one `_dbContext.SaveChangesAsync`.
4. Relies on EF Core's automatic transaction and configured execution strategy.
5. Continues catching `DbUpdateException` and returns the existing sanitized conflict response.

There is no Phase 5D `BeginTransactionAsync`, `CommitAsync`, or `RollbackAsync` call remaining in `AssignmentService`.

## Why One SaveChangesAsync Remains Atomic

EF Core automatically uses a database transaction for a relational `SaveChangesAsync` operation when multiple statements are required. The new assignment and every recipient are tracked in one DbContext and persisted in that single call. The automatic transaction commits all generated inserts together or rolls all of them back together. Therefore an assignment cannot persist with only part of its recipient set.

All Drill, scheduling, duplicate-recipient, active-profile, and relationship validation still finishes before the entity graph is added and persisted.

## Parent Assignment Behavior Preserved

`CreateForParentAsync` is unchanged. It still derives the Parent actor ID from JWT through the controller, validates every active Parent/Athlete relationship and canonical Athlete role, rejects the entire request if any recipient is unauthorized, and calls the shared creation method only after validation succeeds. Parent multi-child assignments remain one assignment with multiple atomic recipient inserts.

## Coach Assignment Behavior Preserved

`CreateForTeamAsync` and `CreateForSelectedTeamAthletesAsync` are unchanged. Active TeamCoach, Team, TeamAthlete, canonical Athlete-role, active-profile, server-side roster, and whole-request validation remain intact. Both whole-Team and selected-Team creation still call the same shared `CreateAsync` method and persist one assignment plus the materialized recipients in one SaveChanges operation.

## Duplicate and Persistence Failure Handling Preserved

Duplicate Athlete IDs are still rejected before persistence, and the recipient composite primary key remains the database-level uniqueness guarantee. `DbUpdateException` is still caught. Callers receive the existing sanitized 409 conflict message; raw exception or database details are not exposed. EF Core automatically rolls back a failed SaveChanges transaction.

## Athlete Start and Complete Audit

`StartAsync` and `CompleteAsync` contain no explicit user-initiated transactions. Each uses one `SaveChangesAsync` call after authorization and transition validation. They do not conflict with `SqlServerRetryingExecutionStrategy` and were left unchanged.

## Migration

- Migration created: **NO**.
- Migration applied: **NO**.

No database model, schema configuration, migration file, or model snapshot was changed by this fix.

## Core Build Result

Command:

`dotnet build SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --no-restore`

Result: **SUCCEEDED — 0 warnings, 0 errors.**

## API Build Result

The exact requested command:

`dotnet build SkillBuilderPro.API\SkillBuilderPro.API.csproj --no-restore`

could not replace the default output executable because the local Swagger API process, `SkillBuilderPro.API` PID 27540, had `bin\Debug\net10.0\SkillBuilderPro.API.exe` locked. Result: **environment file-lock failure — 10 retry warnings, 2 copy errors; no compiler errors were reported.**

The same API project was then built without restore to an isolated output directory. Result: **SUCCEEDED — 0 warnings, 0 errors.** The temporary output was removed after verification.

## Unresolved Issues

- The currently running local API process must be restarted to load and runtime-test the corrected assembly.
- The default-output API build will remain file-lock blocked while that process is running; this is an environment/runtime lock, not a source compilation failure.
- The Parent assignment Swagger request should be retried after restart to confirm the original execution-strategy exception is resolved against the local database.
