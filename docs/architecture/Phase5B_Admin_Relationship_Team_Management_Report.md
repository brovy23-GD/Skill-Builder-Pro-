# Phase 5B: Administrator Relationship and Team Management — Implementation Report

## Scope and outcome

Phase 5B is complete. It adds Administrator-only API contracts, controllers, and a dedicated mutation/query service for maintaining the Phase 5A Parent/Athlete and Team relationship tables. No Parent-facing or Coach-facing cross-user access was activated. Progress, Schedules, JWT behavior, WinForms, MAUI, assignments, notifications, and later-phase domain features were not changed.

No database schema change was necessary. No migration was created or applied.

## 1. Every file added

- `SkillBuilderPro.API/Contracts/Admin/AdminRelationshipRequests.cs`
- `SkillBuilderPro.API/Contracts/Admin/AdminTeamRequests.cs`
- `SkillBuilderPro.API/Contracts/Admin/AdminRelationshipResponses.cs`
- `SkillBuilderPro.API/Services/AdminServiceResult.cs`
- `SkillBuilderPro.API/Services/IAdminRelationshipService.cs`
- `SkillBuilderPro.API/Services/AdminRelationshipService.cs`
- `SkillBuilderPro.API/Controllers/AdminRelationshipsController.cs`
- `SkillBuilderPro.API/Controllers/AdminTeamsController.cs`
- `docs/architecture/Phase5B_Admin_Relationship_Team_Management_Report.md`

## 2. Every file modified

- `SkillBuilderPro.API/Program.cs`
  - Registers `IAdminRelationshipService` as a scoped service.
- `SkillBuilderPro.API/Services/RelationshipAccessService.cs`
  - Role-validates every candidate Athlete ID before returning an aggregate `AthleteAccessScope`, preventing stale relationship rows from granting access after an Athlete role is removed.

No Core entity, DbContext, migration, controller for existing features, JWT component, or client project was modified in Phase 5B.

## 3. Admin service architecture

`IAdminRelationshipService` is the single mutation/query boundary for all Phase 5B data. `AdminRelationshipService` owns:

- Identity-user lookup through `UserManager<ApplicationUser>`.
- Canonical Identity-role validation.
- Administrator revalidation for mutation calls.
- Parent/Athlete and Team membership duplicate prevention.
- Creation versus reactivation behavior.
- Server-generated timestamps and lifecycle state.
- `CreatedByUserId` assignment from the authenticated Administrator ID.
- Team string trimming/normalization.
- `TeamRole` canonicalization and allow-list validation.
- Non-destructive activation/deactivation.
- Safe response mapping without exposing Identity internals.

Service results use `AdminServiceResult<T>` and `AdminServiceStatus` to communicate success, created, validation, not-found, conflict, and forbidden outcomes without leaking EF entities into controllers.

## 4. Admin controller architecture

Two thin controllers organize the REST surface:

- `AdminRelationshipsController` at `/api/admin/relationships` for Parent/Athlete links.
- `AdminTeamsController` at `/api/admin/teams` for Teams, Team Coaches, and Team Athletes.

Each controller:

- Is an `[ApiController]`.
- Has class-level `[Authorize(Roles = ApplicationRoles.Administrator)]`.
- Delegates business logic to `IAdminRelationshipService`.
- Obtains mutation actor identity from `ICurrentUser.UserId`, which reads the server-validated JWT subject claim.
- Maps service outcomes to 201, 200, 400, 404, 409, or 403 responses.

Route names do not provide security; the authorization attribute does.

## 5. ParentAthlete endpoints

| Method | Route | Behavior |
|---|---|---|
| GET | `/api/admin/relationships/parent-athletes` | Lists all active and inactive links with safe user summaries and role-validity flags |
| GET | `/api/admin/relationships/parent-athletes/{parentUserId}/{athleteUserId}` | Gets one composite-key relationship |
| POST | `/api/admin/relationships/parent-athletes` | Creates a valid link or reactivates an existing inactive row |
| POST | `/api/admin/relationships/parent-athletes/{parentUserId}/{athleteUserId}/deactivate` | Sets `IsActive=false` |
| POST | `/api/admin/relationships/parent-athletes/{parentUserId}/{athleteUserId}/reactivate` | Revalidates roles and sets `IsActive=true` |

No hard-delete endpoint exists.

## 6. Team endpoints

| Method | Route | Behavior |
|---|---|---|
| GET | `/api/admin/teams` | Lists active and inactive Teams |
| GET | `/api/admin/teams/{teamId}` | Gets one Team |
| POST | `/api/admin/teams` | Creates an active Team with server-owned audit fields |
| PUT | `/api/admin/teams/{teamId}` | Updates only Name, Sport, Season, AgeGroup, and Organization |
| POST | `/api/admin/teams/{teamId}/deactivate` | Sets `Team.IsActive=false` |
| POST | `/api/admin/teams/{teamId}/reactivate` | Sets `Team.IsActive=true` |

`IsActive` is not accepted by create/update DTOs. No hard-delete endpoint exists.

## 7. TeamCoach endpoints

| Method | Route | Behavior |
|---|---|---|
| GET | `/api/admin/teams/{teamId}/coaches` | Lists active and inactive Coach memberships with role-validity flags |
| POST | `/api/admin/teams/{teamId}/coaches` | Creates or reactivates a Coach membership on an active Team |
| PUT | `/api/admin/teams/{teamId}/coaches/{coachUserId}` | Updates only the canonical relationship-specific `TeamRole` |
| POST | `/api/admin/teams/{teamId}/coaches/{coachUserId}/deactivate` | Sets membership `IsActive=false` |
| POST | `/api/admin/teams/{teamId}/coaches/{coachUserId}/reactivate` | Revalidates Team and Coach role, then activates membership |

No membership is hard-deleted.

## 8. TeamAthlete endpoints

| Method | Route | Behavior |
|---|---|---|
| GET | `/api/admin/teams/{teamId}/athletes` | Lists the complete active/inactive roster with role-validity flags |
| POST | `/api/admin/teams/{teamId}/athletes` | Creates or reactivates an Athlete membership on an active Team |
| POST | `/api/admin/teams/{teamId}/athletes/{athleteUserId}/deactivate` | Sets `IsActive=false` and server-generates `LeftAtUtc` |
| POST | `/api/admin/teams/{teamId}/athletes/{athleteUserId}/reactivate` | Revalidates Team and Athlete role, activates, and clears `LeftAtUtc` |

No roster row is hard-deleted.

## 9. DTOs added

Requests:

- `CreateParentAthleteRequest`: `ParentUserId`, `AthleteUserId`.
- `CreateTeamRequest`: `Name`, `Sport`, `Season`, `AgeGroup`, `Organization`.
- `UpdateTeamRequest`: the same editable Team fields only.
- `AddTeamCoachRequest`: `CoachUserId`, `TeamRole`.
- `UpdateTeamCoachRequest`: `TeamRole` only.
- `AddTeamAthleteRequest`: `AthleteUserId`.

Responses:

- `AdminUserSummary`: `UserId`, `DisplayName`, `Email`, `ExpectedRole`, `HasExpectedRole`.
- `ParentAthleteResponse`.
- `TeamResponse`.
- `TeamCoachResponse`.
- `TeamAthleteResponse`.

No request or response exposes `ApplicationUser`, EF navigation state, password hashes, security stamps, tokens, or other Identity internals.

## 10. Administrator authorization implementation

Both controllers use:

```csharp
[Authorize(Roles = ApplicationRoles.Administrator)]
```

Mutation actions obtain `administratorUserId` only from `ICurrentUser.UserId`. Request DTOs contain no Administrator ID or role field. The service defensively confirms that the actor still has the canonical Administrator Identity role before every mutation. Selection IDs in requests identify only relationship targets.

## 11. Parent role validation

Parent/Athlete creation and reactivation look up the selected Parent by Identity ID and use `UserManager.IsInRoleAsync` with `ApplicationRoles.Parent`. Missing users produce 404; existing users with the wrong role produce 400. Relationship reads expose `HasExpectedRole=false` if the role later becomes stale.

## 12. Athlete role validation

Parent/Athlete creation/reactivation and Team roster creation/reactivation require `ApplicationRoles.Athlete` from Identity. Missing users produce 404; role mismatches produce 400. Roster/relationship reads flag stale roles. `RelationshipAccessService.GetAccessibleAthleteIdsAsync` now filters candidates through server-side Athlete-role validation.

## 13. Coach role validation

TeamCoach creation/reactivation and TeamRole updates require `ApplicationRoles.Coach` from Identity. Missing users produce 404 during creation; role mismatches produce 400. Existing membership reads flag stale roles. Coach-to-Athlete access continues to require active TeamCoach, Team, and TeamAthlete state, and direct checks validate both source and target roles.

## 14. TeamRole validation

`TeamRole` is trimmed and compared case-insensitively against centralized `TeamRoles.All`. The stored value is the canonical constant:

- `HeadCoach`
- `AssistantCoach`
- `SkillsCoach`

Unknown values receive 400. `TeamRole` is never treated as an application Identity role.

## 15. Duplicate handling

- A matching active ParentAthlete row returns 409.
- A matching active TeamCoach row returns 409.
- A matching active TeamAthlete row returns 409.
- Matching inactive rows are reactivated rather than inserted because each relationship has a composite primary key.
- New relationship inserts also catch a database update race and return 409 rather than exposing the database exception.
- Repeating an activate/deactivate lifecycle action when already in that state returns 409.

No Team-name uniqueness rule was invented because the Phase 5A schema does not define one and organizations may legitimately reuse names across sport/season.

## 16. ParentAthlete reactivation behavior

Reactivation revalidates both canonical roles and sets `IsActive=true`. It preserves the original `CreatedAtUtc` and `CreatedByUserId`, retaining the audit identity of the original relationship establishment. Phase 5A has no separate reactivation audit columns, so no artificial timestamp overwrite occurs.

## 17. TeamCoach reactivation behavior

Reactivation requires an active Team and a user who still has the Coach Identity role. An inactive row supplied through the add operation is reactivated and its `TeamRole` is updated to the newly validated canonical value. The dedicated reactivate route preserves the existing TeamRole. `JoinedAtUtc` remains the original association timestamp.

## 18. TeamAthlete reactivation behavior

Reactivation requires an active Team and a user who still has the Athlete Identity role. It sets `IsActive=true` and `LeftAtUtc=null`. The existing composite-key row is reused; no duplicate is inserted.

## 19. JoinedAtUtc rejoin decision

`JoinedAtUtc` is preserved on TeamCoach and TeamAthlete reactivation. With only one composite-key row and no membership-event table, preserving it records the earliest known association. Resetting it would overwrite historical information. A future roster-history model would be required to represent every leave/rejoin interval precisely.

## 20. LeftAtUtc behavior

TeamAthlete deactivation sets `LeftAtUtc=DateTime.UtcNow` server-side. Reactivation clears it to `null`, accurately indicating that the current membership is active. Clients cannot submit either timestamp or `IsActive`.

## 21. Team deactivation behavior

Team deactivation changes only `Team.IsActive=false`. Existing TeamCoach and TeamAthlete rows, including their active flags and timestamps, remain unchanged. This is the least destructive and most auditable behavior, and it permits Team reactivation without reconstructing memberships. `IRelationshipAccessService` already requires `Team.IsActive`, so an inactive Team grants no Coach-to-Athlete access even when membership rows remain active.

Adding or reactivating memberships on an inactive Team returns 409.

## 22. Role-change limitation

No full role-governance workflow was added. Existing rows remain stored if an Identity role changes. Mitigations in this phase are:

- Every creation/reactivation operation revalidates required canonical roles.
- TeamRole update revalidates the Coach role.
- Relationship responses explicitly state `ExpectedRole` and `HasExpectedRole` rather than implying stale links are valid.
- Aggregate Athlete access filters candidates by the current Athlete Identity role.
- Existing direct Parent/Coach access checks continue to validate expected roles.

Future role-management code must deactivate or review affected relationships transactionally when roles are removed.

## 23. Validation and error semantics

- **400 Bad Request**: non-positive body/route IDs via data annotations, missing/oversized request strings, whitespace-only Name/Sport, invalid TeamRole, self-link, or an existing user with an invalid canonical role.
- **404 Not Found**: selected Identity user, Team, or relationship/membership does not exist.
- **409 Conflict**: duplicate active relationship, repeated lifecycle state, adding/reactivating membership on an inactive Team, or concurrent relationship insertion conflict.
- **401 Unauthorized**: no valid authenticated JWT or no usable server-validated subject.
- **403 Forbidden**: authenticated user lacks Administrator role, or defensive service-level Administrator validation fails.

Team strings are trimmed. Optional whitespace-only Team strings become `null`. `IsActive`, audit IDs, and timestamps are always server-controlled.

## 24. Swagger impact

The two `[ApiController]` classes and all explicit request/response DTOs are discovered automatically by the existing Swagger configuration. The existing HTTP Bearer JWT scheme is unchanged. An Administrator JWT can authorize the new route groups through Swagger; request schemas do not expose EF entities or internal Identity fields.

## 25. Database/schema changes

None. Phase 5B uses the already-applied Phase 5A tables and columns exactly as modeled. `AppDbContext` and its model snapshot were not changed during Phase 5B.

## 26. Migration created

**NO.** No Phase 5B migration was generated because no schema change was required.

## 27. Migration applied

**NO.** No migration command was run during Phase 5B.

## 28. Core build result

Command:

```powershell
dotnet build SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --no-restore
```

Final result: **Passed — 0 warnings, 0 errors.**

## 29. API build result

Command:

```powershell
dotnet build SkillBuilderPro.API\SkillBuilderPro.API.csproj --no-restore
```

Final result: **Passed — 0 warnings, 0 errors.**

One intermediate build failed only because the runtime-verification process still held `SkillBuilderPro.API.exe`; after stopping that exact process, the required API build passed cleanly. This was an environment/process lock, not a compiler error.

## 30. Functional/manual test results

Completed non-mutating verification:

- API host reached HTTP listening state using an ephemeral process-only JWT signing key; no secret was written to source.
- Anonymous `GET /api/admin/teams` returned **401**.
- Anonymous `GET /api/admin/relationships/parent-athletes` returned **401**.
- Static inspection confirms every endpoint is covered by class-level canonical Administrator authorization.
- Core and API compilation validates controller routing/action signatures, service registration, DTOs, EF queries, and result mappings.
- No production seed users, passwords, Teams, or relationships were created.

## 31. Tests that could not be performed

Database/account-dependent manual scenarios 2–30 from the requested plan could not be executed in this sandbox. The configured SQL Server connection failed with: `The instance of SQL Server you attempted to connect to requires encryption but this machine does not support it.` The runtime also logged denied access to the user-profile ASP.NET Data Protection key directory, though the HTTP host still started sufficiently for anonymous 401 checks.

Consequently, the following require verification in the normal local environment with existing development accounts and the applied Phase 5A migration:

- Athlete, Parent, and Coach tokens receive 403.
- Administrator receives successful responses.
- Parent/Athlete CRUD lifecycle and role mismatch cases.
- Team lifecycle.
- TeamCoach lifecycle and TeamRole cases.
- TeamAthlete lifecycle and `LeftAtUtc` cases.
- Live relationship-access checks for active/inactive Parent and Team paths.

No authorization bypass or test credentials were introduced to work around the environment.

## 32. Security risks and unresolved questions

- Role removal can leave stored relationships semantically stale; reads flag this and access checks validate roles, but a future role-governance workflow should deactivate affected links.
- The one-row composite Team membership model cannot preserve every leave/rejoin interval. It preserves initial `JoinedAtUtc` and current state only.
- Team deactivation intentionally preserves active membership flags. Reactivating the Team restores eligibility through those still-active memberships, subject to current role validation. Administrators should review memberships before reactivation if organizational policy requires it.
- Collection response mapping performs Identity/profile lookups per user. This is correct and safe for initial admin-scale use but may need batched projections if relationship volumes become large.
- There is no optimistic concurrency token. Composite-key constraints and conflict handling protect duplicate inserts, but simultaneous lifecycle updates remain last-write-wins.
- Full live authorization/CRUD testing remains required in the user's normal local environment because this sandbox cannot connect to the configured SQL Server.

## 33. Recommended Phase 5C next step

After local Phase 5B verification, implement a narrowly scoped Parent/Coach read-authorization phase that consumes `AthleteAccessScope` and `IRelationshipAccessService`. Begin with read-only, explicitly selected Athlete resources and resource-level authorization tests. Do not enable mutations or assignments until cross-user read boundaries, inactive-link behavior, and stale-role handling are verified end to end.

## Local verification checklist

Use Swagger with existing development accounts; do not create production seed credentials:

1. Confirm 401 anonymous and 403 for Athlete, Parent, and Coach on both controller route groups.
2. Confirm Administrator CRUD/lifecycle operations and all 400/404/409 cases.
3. Confirm timestamps and actor IDs are server-derived.
4. Confirm Team deactivation immediately disables derived Coach/Athlete access without modifying membership rows.
5. Confirm Parent/Coach access service results for active and inactive relationship paths.
6. Confirm existing Progress/Schedule ownership behavior remains unchanged for every role.
