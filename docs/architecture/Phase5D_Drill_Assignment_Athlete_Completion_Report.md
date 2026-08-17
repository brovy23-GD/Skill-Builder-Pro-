# Phase 5D Drill Assignment and Athlete Completion Report

## Outcome

Phase 5D implementation is complete. One common assignment engine now supports Parent multi-child assignments, Coach whole-Team and selected-Team assignments, and Athlete assignment discovery, start, and completion. The migration was generated and reviewed but was not applied. No ProgressLog, TrainingSchedule, progression, notification, MAUI, or WinForms behavior was changed.

## 1. Every file added

- `SkillBuilderPro.Core/Models/DrillAssignment.cs`
- `SkillBuilderPro.Core/Models/DrillAssignmentRecipient.cs`
- `SkillBuilderPro.Core/Models/DrillAssignmentStatuses.cs`
- `SkillBuilderPro.Core/Interfaces/IAssignmentService.cs`
- `SkillBuilderPro.API/Contracts/Assignments/AssignmentRequests.cs`
- `SkillBuilderPro.API/Contracts/Assignments/AssignmentResponses.cs`
- `SkillBuilderPro.API/Services/AssignmentService.cs`
- `SkillBuilderPro.API/Controllers/AthleteAssignmentsController.cs`
- `SkillBuilderPro.Core/Migrations/20260812182403_AddDrillAssignmentsAndRecipients.cs`
- `SkillBuilderPro.Core/Migrations/20260812182403_AddDrillAssignmentsAndRecipients.Designer.cs`
- `docs/architecture/Phase5D_Drill_Assignment_Athlete_Completion_Report.md`

## 2. Every file modified

- `SkillBuilderPro.Core/Data/AppDbContext.cs`
- `SkillBuilderPro.Core/Migrations/AppDbContextModelSnapshot.cs`
- `SkillBuilderPro.API/Controllers/ParentController.cs`
- `SkillBuilderPro.API/Controllers/CoachController.cs`
- `SkillBuilderPro.API/Program.cs`

The worktree contains earlier Phase 5A–5C/bootstrap changes. They were preserved and are not Phase 5D changes.

## 3. DrillAssignment schema

`DrillAssignments` contains: `Id int`, `DrillId int`, `AssignedByUserId int`, nullable `SourceTeamId int`, nullable `ScheduledForUtc datetime2`, nullable `DueAtUtc datetime2`, nullable `Instructions nvarchar(1000)`, required `Status nvarchar(20)`, `CountsTowardProgression bit`, `CreatedAtUtc datetime2`, and nullable `CancelledAtUtc datetime2`.

`AssignedByUserId`, status, and server timestamps are never accepted from assignment-creation request DTOs. `AssignedByUserId` comes from `ICurrentUser`.

## 4. DrillAssignmentRecipient schema

`DrillAssignmentRecipients` contains: `AssignmentId int`, `AthleteUserId int`, required `Status nvarchar(20)`, nullable `StartedAtUtc datetime2`, nullable `CompletedAtUtc datetime2`, nullable `AthleteNotes nvarchar(1000)`, and nullable `Rating int` constrained to 1–5.

Each row is the authoritative per-Athlete state. No transition updates another recipient.

## 5. Assignment status model

Central constants in `DrillAssignmentStatuses` are `Scheduled`, `Active`, `Cancelled`, and `Closed`. A database check constraint enforces them. Creation selects `Scheduled` only when `ScheduledForUtc` is in the future; otherwise it selects `Active`. Starting/completing an available scheduled assignment moves the parent to `Active`. Arbitrary client status is not accepted.

## 6. Recipient status model

Central constants in `DrillAssignmentRecipientStatuses` are `Assigned`, `InProgress`, `Completed`, `Missed`, and `Excused`. A database check constraint enforces them. New recipients are server-created as `Assigned`; Athlete start changes only their row to `InProgress`; completion changes only their row to `Completed`.

## 7. Primary and composite keys

- `DrillAssignments`: primary key `Id`, SQL Server identity.
- `DrillAssignmentRecipients`: composite primary key `(AssignmentId, AthleteUserId)`, which also enforces one recipient per Athlete per assignment.

## 8. Indexes

- `DrillAssignments(AssignedByUserId, CreatedAtUtc)` supports creator/history queries and future cancellation authorization.
- `DrillAssignments(SourceTeamId, ScheduledForUtc)` supports Team assignment history/upcoming queries.
- EF-required `DrillAssignments(DrillId)` supports the Drill FK.
- `DrillAssignmentRecipients(AthleteUserId, Status, AssignmentId)` supports Athlete list/status filtering and record lookup.
- `DrillAssignmentRecipients(AthleteUserId, CompletedAtUtc)` supports Athlete completion history and later adherence queries.

No speculative assignment-status index was added because current queries begin from recipient ownership.

## 9. Foreign keys

- `DrillAssignments.DrillId -> Drills.Id`
- `DrillAssignments.AssignedByUserId -> AspNetUsers.Id`
- `DrillAssignments.SourceTeamId -> Teams.Id` (nullable)
- `DrillAssignmentRecipients.AssignmentId -> DrillAssignments.Id`
- `DrillAssignmentRecipients.AthleteUserId -> AspNetUsers.Id`

## 10. Delete behaviors

Drill, assigner, source-Team, and Athlete-user foreign keys use `NoAction` to preserve history. Assignment-to-recipient uses cascade because a recipient has no independent meaning, while normal APIs expose no hard-delete operation. Cancellation is represented by state, never deletion.

## 11. IAssignmentService design

One `IAssignmentService` and one `AssignmentService` own Drill/date validation, relationship authorization, active-profile validation, roster resolution, duplicate validation, transaction/persistence, lifecycle transitions, SQL-filtered discovery, and internal view mapping. Parent and Coach controllers use the same engine. `IRelationshipAccessService` remains relationship-focused.

## 12. Parent assignment flow

`POST /api/parent/assignments` requires the Parent role. The controller derives the actor from JWT, and the service validates the Drill, UTC dates, unique recipient IDs, every active ParentAthlete relationship, each target's canonical Athlete role through `IRelationshipAccessService`, and each active profile. Any invalid target returns 404 without creating anything. Success creates one assignment with `SourceTeamId = null` and all recipient rows atomically.

## 13. Parent multi-child behavior

One request produces one `DrillAssignment` plus one recipient per linked child. Duplicate IDs are rejected with 400. One unauthorized child rejects the entire request; no partial assignment is committed.

## 14. Coach entire-Team assignment flow

`POST /api/coach/teams/{teamId}/assignments` requires the Coach role. The service validates active TeamCoach and Team state through `IRelationshipAccessService`, resolves the active canonical-Athlete roster server-side, filters to active profiles, rejects an empty valid roster with 409, and creates one assignment with `SourceTeamId = teamId`.

## 15. Team recipient materialization

Whole-Team recipients are copied into recipient rows during creation. Later TeamAthlete changes do not alter historical recipients, and later joiners do not inherit old assignments.

## 16. Selected-Team Athlete assignment behavior

`POST /api/coach/teams/{teamId}/assignments/selected` requires unique positive Athlete IDs. Every ID must be in the server-resolved active roster, have the canonical Athlete role, and have an active profile. Any invalid ID returns 404 and the transaction creates nothing.

## 17. Individual Coach assignment behavior

The selected-Team route is the single coherent API for both one and many selected Team Athletes. A one-element `AthleteUserIds` array is the individual-Coach flow. No `CoachAthlete` model or ambiguous athlete-only route was introduced.

## 18. Atomic validation and failure behavior

All recipient and relationship validation finishes before entities are added. Assignment and recipients are then saved in one explicit EF transaction. A persistence race rolls back and returns a sanitized 409; raw `DbUpdateException` details are not exposed.

## 19. Duplicate-recipient handling

Incoming duplicate Athlete IDs are rejected with 400 before persistence. The composite key provides a second database guarantee. No request can create duplicate recipient rows.

## 20. Athlete assignment list design

`GET /api/athlete/assignments` requires the Athlete role and derives the Athlete ID from JWT. It begins from `DrillAssignmentRecipients` filtered by `AthleteUserId` in SQL, orders newest first, and optionally accepts one of the centralized recipient statuses. It returns explicit DTOs with Drill, assigner, optional Team, schedule, instructions, assignment state, recipient state, timestamps, notes, rating, and progression-evidence flag.

## 21. Athlete assignment detail design

`GET /api/athlete/assignments/{assignmentId}` constrains both JWT Athlete ID and assignment ID in the database query. Nonexistent and other-Athlete assignments both return 404.

## 22. Athlete start workflow

`POST /api/athlete/assignments/{assignmentId}/start` permits `Assigned -> InProgress`. Already `InProgress` is idempotent and returns the current representation. Completed, Missed, Excused, cancelled, or not-yet-available assignments return 409. Other-Athlete/nonexistent assignments return 404.

## 23. Athlete completion workflow

`POST /api/athlete/assignments/{assignmentId}/complete` permits completion from `Assigned` or `InProgress`. Already completed is idempotent and returns current state. Cancelled, not-yet-available, Missed, or Excused assignments return 409. The request may provide notes and rating but cannot provide Athlete ID, status, or timestamps.

## 24. StartedAtUtc behavior

Start sets `StartedAtUtc = DateTime.UtcNow` server-side. Direct completion from Assigned is allowed and sets `StartedAtUtc` to the same server timestamp as completion, preserving a complete lifecycle without trusting client time.

## 25. CompletedAtUtc behavior

Completion sets `CompletedAtUtc = DateTime.UtcNow` server-side. No client completion timestamp is accepted. Other recipient timestamps and statuses remain unchanged.

## 26. AthleteNotes and Rating behavior

Notes are optional, trimmed, and limited to 1000 characters. Rating is optional and constrained to 1–5 by DTO validation, service validation, model validation, and a database check constraint.

## 27. Assignment cancellation design or deferral

Cancellation is deferred. The schema and centralized assignment state include `Cancelled` and `CancelledAtUtc`, and start/complete already block cancelled assignments, but no cancellation route is exposed. Correct creator/relationship/admin policy should be reviewed before Phase 5E adds cancellation. No hard-delete route exists.

## 28. CountsTowardProgression behavior

Creation DTOs default `CountsTowardProgression` to `true`; the chosen value is stored on the assignment and returned to Athletes. No progression calculation or side effect is implemented.

## 29. Completion-event foundation or deferral

An application-event abstraction is explicitly deferred because none exists and introducing dispatch/outbox semantics would significantly expand Phase 5D. Before progression or notifications are implemented, Phase 5E should add an idempotent completion event/outbox design keyed by `(AssignmentId, AthleteUserId)`.

## 30. ProgressLog integration decision

Completion does not create or modify `ProgressLog`. Recipient completion remains authoritative. A later reviewed event consumer may create a linked ProgressLog only after an idempotent schema/linkage design prevents duplicates.

## 31. TrainingSchedule interaction

New Parent/Coach assignments create only `DrillAssignment` plus recipients. No `TrainingSchedule` row is created or modified. Legacy TrainingSchedule remains supported and is neither removed nor migrated.

## 32. Response DTOs added

- `DrillAssignmentResponse`
- `AssignmentRecipientResponse`
- `AthleteAssignmentResponse`
- `AssignmentDrillSummary`
- `AssignmentUserSummary`
- `AssignmentTeamSummary`

No EF entity or Identity security/navigation graph is exposed.

## 33. Authorization implementation

Parent routes use `ApplicationRoles.Parent`; Coach routes use `ApplicationRoles.Coach`; Athlete routes use `ApplicationRoles.Athlete`. Controllers derive actor identity from `ICurrentUser`. Services enforce relationship/resource authorization. Client DTOs contain no actor, owner, role, status, or authoritative timestamp fields.

## 34. RelationshipAccessService changes

No Phase 5D change was needed. Existing `CanParentAccessAthleteAsync`, `CanCoachManageTeamAsync`, and `GetCoachTeamAthleteIdsAsync` provide the required live, role-aware relationship checks. Assignment business logic was not added to the relationship service.

## 35. Transaction behavior

Each creation uses `BeginTransactionAsync`, one `SaveChangesAsync`, and `CommitAsync`. Validation precedes the transaction. Database update failures roll back and return a sanitized conflict response. Start/complete each update one tracked recipient (and, when necessary, the parent assignment status) in one SaveChanges transaction.

## 36. Migration name

`20260812182403_AddDrillAssignmentsAndRecipients` (logical name `AddDrillAssignmentsAndRecipients`). Exactly one Phase 5D migration was generated.

## 37. Complete migration-operation audit

`Up` contains exactly:

- `CreateTable DrillAssignments`
- `CreateTable DrillAssignmentRecipients`
- five `CreateIndex` operations
- primary keys, two check constraints on recipient, one check constraint on assignment, and five foreign keys embedded in the table creation

`Up` contains no `AddColumn`, `AlterColumn`, `DropColumn`, `DropTable`, `Rename`, raw SQL, or data operation. `Down` drops only `DrillAssignmentRecipients` and `DrillAssignments`, in dependency order.

## 38. Destructive operations

Destructive migration operations in `Up`: **NO**.

## 39. Data backfill

Data backfill: **NO**.

## 40. Migration applied

Migration applied: **NO**. It was intentionally generated and reviewed only.

## 41. Core build result

`dotnet build SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --no-restore`

**Succeeded: 0 warnings, 0 errors.**

## 42. API build result

`dotnet build SkillBuilderPro.API\SkillBuilderPro.API.csproj --no-restore`

**Succeeded: 0 warnings, 0 errors.**

## 43. Runtime/manual verification performed

Compilation and static verification were performed. Routes, role attributes, JWT actor derivation, DTO authority boundaries, owner-keyed Athlete queries, transition guards, relationship calls, transaction boundaries, schema, migration operations, and lack of ProgressLog/TrainingSchedule writes were inspected. No local IDs are present in production Phase 5D code.

## 44. Tests blocked by environment

End-to-end JWT/SQL scenarios were not run because the new migration must not be applied automatically. Until a reviewer applies it to the configured local database, Phase 5D endpoints cannot execute against their tables. The environment also depends on local User Secrets and verified role accounts. No test data, secrets, or schema were changed.

## 45. Security risks and unresolved questions

- Cancellation authorization and endpoints are intentionally deferred.
- There is no optimistic concurrency token; current guarded transitions are suitable for initial use, but concurrent start/complete requests should be stress-tested and may later warrant a rowversion.
- The future completion event/outbox and ProgressLog linkage need an idempotency design before consumers are added.
- Assignment visibility is Athlete-only in Phase 5D; creator history/list/detail APIs were not required and should be designed before cancellation.
- Active-account eligibility currently follows the existing product convention of canonical role plus active `UserProfile`; a future explicit Identity account-state policy should be centralized if lockout/disable semantics expand.

## 46. Recommended local Swagger verification sequence

1. Review and explicitly apply `AddDrillAssignmentsAndRecipients` to the local database.
2. Authenticate Parent, create one-child and multi-child assignments, then test unrelated/mixed/duplicate/nonexistent-Drill failures and verify atomic row counts.
3. Authenticate Coach, create whole-Team, selected-many, and selected-one assignments; test inactive TeamCoach, Team, and TeamAthlete states and roster materialization across membership changes.
4. Authenticate each recipient Athlete; list/detail, start, and complete independently, verifying timestamps and that other recipients remain unchanged.
5. Test cross-role and anonymous calls, other-Athlete 404 behavior, completed restart conflict, future scheduling, invalid dates/ratings, and existing Phase 5C plus Progress/Schedule regression behavior.

Run all 34 scenarios from the Phase 5D request and retain request/response plus database evidence for lifecycle and atomicity checks.

## 47. Recommended Phase 5E next step

After migration application and the full Swagger matrix pass, add reviewed creator assignment history and cancellation authorization, then introduce an idempotent `AssignmentRecipientCompleted` outbox/application-event foundation keyed by `(AssignmentId, AthleteUserId)`. Only after that foundation should a separately reviewed migration link completion evidence to ProgressLog or progression consumers. Do not begin progression, goals, achievements, notifications, or TrainingRequest before that review.
