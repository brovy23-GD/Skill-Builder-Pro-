# Phase 5F Outbox ProgressLog Consumer Report

## Outcome

Phase 5F is complete. A bounded internal hosted processor now consumes durable `AssignmentRecipientCompleted` events and creates at most one linked Athlete-owned `ProgressLog` when `CountsTowardProgression` is true. No public processor API, downstream progression, notifications, AI, or TrainingSchedule changes were introduced. The migration was generated and audited but not applied.

## 1. Every file added

- `SkillBuilderPro.API/Services/AssignmentCompletionProcessingOptions.cs`
- `SkillBuilderPro.API/Services/IAssignmentCompletionEventProcessor.cs`
- `SkillBuilderPro.API/Services/AssignmentCompletionEventProcessor.cs`
- `SkillBuilderPro.API/Services/AssignmentCompletionEventBackgroundService.cs`
- `SkillBuilderPro.Core/Migrations/20260813032421_AddCompletionEventProgressLink.cs`
- `SkillBuilderPro.Core/Migrations/20260813032421_AddCompletionEventProgressLink.Designer.cs`
- `docs/architecture/Phase5F_Outbox_ProgressLog_Consumer_Report.md`

## 2. Every file modified

- `SkillBuilderPro.Core/Models/ProgressLog.cs`
- `SkillBuilderPro.Core/Data/AppDbContext.cs`
- `SkillBuilderPro.Core/Migrations/AppDbContextModelSnapshot.cs`
- `SkillBuilderPro.API/Program.cs`
- `SkillBuilderPro.API/appsettings.json`

Existing dirty Phase 5A–5E changes were preserved.

## 3. Existing ProgressLog schema audit

Before Phase 5F, `ProgressLog` had identity `Id`; required `DrillId`; required `LogDate`; required integer `Rating` with 1–5 validation; non-null `Notes` with a 300-character limit and empty-string default; nullable `OwnerUserId`; Drill and Owner navigations. There was no event/assignment linkage or uniqueness assumption. Existing service queries constrain owner in SQL and expose manual creation/deletion under existing ownership rules.

## 4. ProgressLog fields used by consumer

The consumer sets `DrillId`, `LogDate`, `Rating`, `Notes`, `OwnerUserId`, and `AssignmentCompletionEventId`. `Id` remains database generated.

## 5. New ProgressLog linkage fields

Added nullable `long? AssignmentCompletionEventId` plus a JSON-ignored navigation. No duplicate `AssignmentId` was added because the event already carries the assignment relationship.

## 6. Linkage idempotency strategy

The processor checks for an existing linked ProgressLog before insertion. A filtered unique database index is the final guarantee. If a link already exists while the event is pending, the processor marks the event processed without adding another row.

## 7. Database uniqueness strategy

`IX_ProgressLogs_AssignmentCompletionEventId` is unique with filter `[AssignmentCompletionEventId] IS NOT NULL`. Many legacy/manual null-linked rows remain valid; exactly one non-null ProgressLog may reference each completion event.

## 8. Event processor interface

`IAssignmentCompletionEventProcessor` exposes one internal operation: `ProcessPendingBatchAsync(CancellationToken)`, returning the number of pending IDs selected for the cycle.

## 9. Event processor implementation

`AssignmentCompletionEventProcessor` selects a bounded batch, handles each event, loads assignment/recipients, validates durable data, checks existing linkage, honors progression evidence, creates ProgressLog, updates attempts/processed/error fields, and handles persistence races with sanitized recovery logic.

## 10. BackgroundService design

`AssignmentCompletionEventBackgroundService` runs while the API is active, creates a new async DI scope per polling cycle, resolves the scoped processor, processes one bounded batch, catches safe cycle-level failures, delays, and continues until host cancellation. It never holds a scoped DbContext across cycles.

## 11. DI registration

Options are bound/validated on start. `IAssignmentCompletionEventProcessor` is scoped. `AssignmentCompletionEventBackgroundService` is registered as a hosted service and receives only the scope factory, options, and logger.

## 12. Polling configuration

`AssignmentCompletionProcessing:PollingSeconds` defaults/configures to 10 seconds and must be positive.

## 13. Batch size configuration

`AssignmentCompletionProcessing:BatchSize` is 20 and validated between 1 and 100.

## 14. Max attempts configuration

`AssignmentCompletionProcessing:MaxAttempts` is 5 and validated between 1 and 20. Events at the configured maximum remain unprocessed with their sanitized `LastError` and are excluded from automatic polling.

## 15. Pending-event query

SQL filters `ProcessedAtUtc == null` and `ProcessingAttempts < MaxAttempts`, selects IDs only, and applies `Take(BatchSize)`. It does not load all pending events.

## 16. Processing order

Pending events order by `CreatedAtUtc ASC`, then `Id ASC` for deterministic oldest-first processing.

## 17. Event validation rules

Supported type must equal centralized `AssignmentRecipientCompleted`; assignment reference must match; event Drill must match assignment Drill; recipient must exist for event Athlete; recipient must be Completed; completion timestamp must exist; and timestamp difference from event occurrence must be no more than one second.

## 18. Assignment validation

The event FK and eager-loaded assignment provide the authoritative assignment. The processor explicitly confirms loaded `Id` and `DrillId` match durable event values and reads `CountsTowardProgression` from the assignment.

## 19. Recipient validation

The recipient is selected from the assignment by `AthleteUserId`. Missing, non-Completed, or timestamp-less recipients are permanent validation failures and cannot produce ProgressLog.

## 20. Drill mapping

`ProgressLog.DrillId = AssignmentCompletionEvent.DrillId`. Event and assignment Drill IDs are validated equal; existing FKs preserve Drill existence.

## 21. OwnerUserId mapping

`ProgressLog.OwnerUserId = AssignmentCompletionEvent.AthleteUserId`. Creator, Parent, Coach, Team, and processor identity are never used as ownership.

## 22. Completion timestamp mapping

`ProgressLog.LogDate = AssignmentCompletionEvent.OccurredAtUtc`, preserving historical completion time rather than processor runtime.

## 23. Rating mapping

Assignment recipient Rating maps directly. Because completion Rating is optional, `ProgressLog.Rating` is minimally widened from `int` to nullable `int?`; no zero or fabricated score is used. Manual DTO validation remains 1–5 and existing values remain valid.

## 24. Notes mapping

Recipient `AthleteNotes` maps to ProgressLog Notes. When absent, the established ProgressLog empty-string convention is retained. ProgressLog stores up to 300 characters while assignment notes allow 1000; longer notes are rejected as a permanent sanitized validation failure rather than silently truncated.

## 25. CountsTowardProgression=true behavior

For a valid event with no existing link, the processor adds one ProgressLog and marks the event processed in the same SaveChanges unit.

## 26. CountsTowardProgression=false behavior

The processor increments attempts, marks the event processed, clears LastError, and creates no ProgressLog. The event does not remain pending.

## 27. ProgressLog creation flow

The processor, not the completion controller, creates ProgressLog after durable event discovery and validation. Generated rows naturally appear through existing owner-filtered Progress APIs.

## 28. Event ProcessedAtUtc flow

Successful create, intentional progression-disabled skip, and existing-link recovery set `ProcessedAtUtc = DateTime.UtcNow`. Validation/persistence failures leave it null.

## 29. ProcessingAttempts behavior

Attempts increment once when actual handling begins. Successful, skipped, and failed handling persist an increment. Poll scans alone do not increment. Persistence-race recovery reloads database state and records the attempt after the failed atomic SaveChanges rolls back.

## 30. LastError behavior

Each attempt clears stale error first. Permanent validation failures store concise predefined messages. Retryable persistence failure stores `ProgressLog persistence failed; the event will be retried.` Successful later handling clears it. No raw SQL/exception details or sensitive values are stored.

## 31. Success retry behavior

An event below MaxAttempts is retried next cycle. If a linked ProgressLog is found, it is marked processed and LastError remains cleared without another insert.

## 32. Repeated processing behavior

Processed events are excluded by SQL. A manually reset pending event with an existing linkage is safely marked processed without duplicate ProgressLog.

## 33. Duplicate race handling

The filtered unique index rejects competing inserts. `DbUpdateException` is not exposed; tracking is cleared, linkage is re-queried, and an existing row yields successful recovery. If no row is visible, a sanitized retryable failure is recorded subject to MaxAttempts.

## 34. One-SaveChanges atomicity

For producing events, new ProgressLog, event attempt increment, cleared LastError, and ProcessedAtUtc are persisted in one `SaveChangesAsync`. EF's automatic relational transaction commits or rolls back all changes together.

## 35. SQL retry execution strategy compatibility

Normal processing uses EF Core SaveChanges and configured SQL Server retry execution strategy. There is no user-initiated transaction around the unit.

## 36. Manual transaction usage

Manual transaction usage: **NO**.

## 37. Public processing endpoint

Public processing endpoint: **NO**.

## 38. Admin replay endpoint

Admin replay endpoint: **NO**. SQL state plus structured logs are the Phase 5F observability surface.

## 39. Logging design

Structured logs include bounded discovery count, event/assignment/Athlete IDs, attempt number, success/skip/recovery classification, predefined validation reason, and exception type only for cycle failures. They exclude notes, JWTs, secrets, connection strings, SQL text, and raw exception messages.

## 40. Migration name

`20260813032421_AddCompletionEventProgressLink` (logical name `AddCompletionEventProgressLink`). Exactly one Phase 5F migration was generated.

## 41. Migration Up operation audit

`Up` contains: nullable widening of `ProgressLogs.Rating`; nullable `AssignmentCompletionEventId` addition; filtered unique index creation; and FK addition. It contains no drop, destructive narrowing, unrelated alteration/rename, raw SQL, or data operation.

## 42. Foreign keys

`ProgressLogs.AssignmentCompletionEventId -> AssignmentCompletionEvents.Id` uses default NoAction behavior. Deleting an event cannot cascade-delete progress history.

## 43. Indexes

One filtered unique index on `ProgressLogs.AssignmentCompletionEventId` where non-null. No redundant Assignment ID or polling schema index was added; Phase 5E already has `(ProcessedAtUtc, CreatedAtUtc)`.

## 44. Destructive operations

Destructive operations in migration `Up`: **NO**. Rating is widened to nullable, preserving existing data.

## 45. Data backfill

Data backfill: **NO**. Existing ProgressLogs retain null linkage; pending Phase 5E events remain pending for natural processing after deployment.

## 46. Migration applied

Migration applied: **NO**.

## 47. Core build

`dotnet build SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --no-restore`: **SUCCEEDED — 0 warnings, 0 errors.**

## 48. API build

The exact Debug command was blocked when running API PID 24484 locked the copied Core DLL: environment file-lock failure with 10 retry warnings and 2 copy errors, with no source compiler errors. The same project built to isolated output with `--no-restore`: **SUCCEEDED — 0 warnings, 0 errors.** Release design-time build also succeeded with 0 warnings/errors. Temporary isolated outputs were removed.

## 49. Runtime/manual verification performed

Static verification covered build, DI lifetimes, bounded SQL query/order, validation/mapping, one-SaveChanges atomicity, filtered uniqueness, race recovery, retries/errors, structured logging, migration operations, lack of public endpoints/manual transactions, and absence of Progress/TrainingSchedule route changes. Runtime processing was not performed because migration application was prohibited.

## 50. Tests blocked by migration/environment

Runtime processor tests require reviewed migration application and API restart. The existing running API holds old assemblies and locks Debug output. Local SQL/JWT/test data are user-managed.

## 51. Existing pending event behavior

No event ID is hard-coded. After migration application/restart, any event with null ProcessedAtUtc and attempts below MaxAttempts is automatically discovered oldest-first. Counts true yields one linked ProgressLog; counts false is processed without one.

## 52. ProgressLog manual creation compatibility

Manual Progress creation remains unchanged. DTO requires 1–5 Rating and defaults Notes to empty. Manual rows store null event linkage, and the filtered index permits unlimited legacy/manual null values.

## 53. Phase 5C Progress authorization preserved

No Progress controller/service authorization or query route changed. Generated rows use Athlete OwnerUserId and therefore inherit existing Athlete/Parent/Coach/Admin access and resource non-disclosure.

## 54. TrainingSchedule interaction

TrainingSchedule is untouched. The processor neither reads nor writes it and performs no dual-write.

## 55. Progression implementation status

No XP, rank, levels, streaks, goals, achievements, or Trophy Room behavior exists. `CountsTowardProgression` only gates ProgressLog evidence creation.

## 56. Notification implementation status

No Parent/Coach/Athlete notification, email, SMS, or push consumer was added.

## 57. Security risks and unresolved issues

- Recipient notes allow 1000 characters while ProgressLog Notes allows 300; Phase 5F does not silently truncate. A long note reaches MaxAttempts with a sanitized permanent validation error. Product review should choose truncation, separate evidence notes, or a later length alignment.
- Multi-instance correctness relies on the unique link and recovery rather than a claim lease; duplicate work is possible but duplicate ProgressLogs are prevented.
- Max-attempt events require future administrative inspection/replay tooling.
- No rowversion protects event attempt updates; database uniqueness protects the critical consumer side effect.

## 58. Recommended local Swagger/SQL verification sequence

1. Review and explicitly apply `AddCompletionEventProgressLink`; restart the API.
2. Confirm pending events and wait one polling interval; verify processed timestamp, attempts increment, one linked ProgressLog, exact owner/Drill/event/date, and rating/notes mapping.
3. Restart/wait multiple cycles and verify no duplicate. In controlled local SQL, reset one processed event to pending and verify existing-link recovery.
4. Complete a `CountsTowardProgression=false` assignment and verify event processed with no linked ProgressLog.
5. In disposable local data only, exercise unsupported type, recipient mismatch/non-Completed/timestamp mismatch, uniqueness race, and MaxAttempts; verify sanitized errors and continued processing.
6. Run all Phase 5F regressions for creation/list/start/complete/history/cancel, existing Progress/Schedule ownership, no TrainingSchedule writes, and no progression/notification effects.

## 59. Recommended operational considerations

Monitor pending count, oldest pending age, MaxAttempts failures, cycle errors, and processing throughput. Keep batch/interval conservative, use one processor per deployment initially, and establish a reviewed replay procedure before production. Never edit event/link rows without an audit trail.

## 60. Recommended Phase 5G next step

After the Phase 5F migration and verification pass, add Admin-only failed-event inspection/replay with concurrency-safe claiming/lease semantics and operational metrics. Resolve the notes-length policy first. Only then design a separately idempotent progression consumer; keep notifications, goals, achievements, and AI as independent consumers.
