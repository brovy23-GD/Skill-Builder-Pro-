# Phase 5A: Relationship and Team Foundation — Implementation Report

## Scope and outcome

Phase 5A is complete. The implementation adds the relationship/team data model, an explicit athlete-access scope, and a centralized relationship-access service. It does not add relationship-management or team-management endpoints, does not broaden Progress or Schedule access, and does not change JWT, existing controller authorization, WinForms, or MAUI behavior.

The single Phase 5A migration was generated and reviewed but was **not applied**.

## 1. Files added

- `SkillBuilderPro.Core/Models/ParentAthlete.cs`
- `SkillBuilderPro.Core/Models/Team.cs`
- `SkillBuilderPro.Core/Models/TeamCoach.cs`
- `SkillBuilderPro.Core/Models/TeamAthlete.cs`
- `SkillBuilderPro.Core/Models/TeamRoles.cs`
- `SkillBuilderPro.Core/Security/AthleteAccessScope.cs`
- `SkillBuilderPro.Core/Interfaces/IRelationshipAccessService.cs`
- `SkillBuilderPro.API/Services/RelationshipAccessService.cs`
- `SkillBuilderPro.Core/Migrations/20260812022115_Phase5ARelationshipTeamFoundation.cs`
- `SkillBuilderPro.Core/Migrations/20260812022115_Phase5ARelationshipTeamFoundation.Designer.cs`
- `docs/architecture/Phase5A_Relationship_Team_Foundation_Report.md`

## 2. Files modified

- `SkillBuilderPro.Core/Data/AppDbContext.cs`
  - Added four DbSets and explicit EF Core mappings for keys, relationships, indexes, and conservative delete behavior.
- `SkillBuilderPro.Core/Migrations/AppDbContextModelSnapshot.cs`
  - Updated by EF Core to describe only the four new Phase 5A entities and relationships.
- `SkillBuilderPro.API/Program.cs`
  - Registered `IRelationshipAccessService` with scoped lifetime.

`ApplicationUser.cs`, controllers, existing Progress/Schedule services, and client projects were not modified.

## 3. ParentAthlete schema

Table: `ParentAthletes`

| Column | SQL type | Nullable | Purpose |
|---|---|---:|---|
| ParentUserId | int | No | Identity user acting as Parent |
| AthleteUserId | int | No | Linked Identity user acting as Athlete |
| IsActive | bit | No | Soft activation/deactivation of the relationship |
| CreatedAtUtc | datetime2 | No | Relationship creation timestamp |
| CreatedByUserId | int | No | Authenticated/admin actor that established the link |

No relationship is created or backfilled automatically.

## 4. Team schema

Table: `Teams`

| Column | SQL type | Nullable | Constraints/purpose |
|---|---|---:|---|
| Id | int identity | No | Primary key |
| Name | nvarchar(120) | No | Required team name |
| Sport | nvarchar(50) | No | Required sport taxonomy value |
| Season | nvarchar(50) | Yes | Optional season label |
| AgeGroup | nvarchar(50) | Yes | Optional age-group label |
| Organization | nvarchar(150) | Yes | Optional organization label |
| IsActive | bit | No | Soft activation/deactivation |
| CreatedAtUtc | datetime2 | No | Creation timestamp |
| CreatedByUserId | int | No | Identity user that created the team |

`Sport` uses the same string-oriented normalization assumption as the existing `Drill.Sport` taxonomy. Normalization/validation belongs in future team-management workflows; no management API was added in this phase.

## 5. TeamCoach schema

Table: `TeamCoaches`

| Column | SQL type | Nullable | Purpose |
|---|---|---:|---|
| TeamId | int | No | Team membership key |
| CoachUserId | int | No | Identity user acting as Coach |
| TeamRole | nvarchar(30) | No | Relationship-specific coaching role |
| IsActive | bit | No | Soft activation/deactivation |
| JoinedAtUtc | datetime2 | No | Membership start timestamp |

The application Identity role remains separate from `TeamRole`. Future write workflows must verify the canonical `Coach` Identity role before creating this relationship.

## 6. TeamAthlete schema

Table: `TeamAthletes`

| Column | SQL type | Nullable | Purpose |
|---|---|---:|---|
| TeamId | int | No | Team membership key |
| AthleteUserId | int | No | Identity user acting as Athlete |
| IsActive | bit | No | Soft activation/deactivation |
| JoinedAtUtc | datetime2 | No | Membership start timestamp |
| LeftAtUtc | datetime2 | Yes | Optional membership end timestamp |

`IsActive`, `JoinedAtUtc`, and `LeftAtUtc` preserve the minimum roster-history context without introducing a roster-versioning system.

## 7. Primary and composite keys

- `ParentAthletes`: composite primary key `(ParentUserId, AthleteUserId)`. This also guarantees one row per Parent/Athlete pair.
- `Teams`: primary key `Id`, SQL Server identity.
- `TeamCoaches`: composite primary key `(TeamId, CoachUserId)`. This also guarantees one row per Team/Coach pair.
- `TeamAthletes`: composite primary key `(TeamId, AthleteUserId)`. This also guarantees one row per Team/Athlete pair.

No additional unique constraints are required because the composite keys enforce the required relationship uniqueness.

## 8. Indexes

- `IX_ParentAthletes_AthleteUserId` on `ParentAthletes(AthleteUserId)`.
- `IX_ParentAthletes_CreatedByUserId` on `ParentAthletes(CreatedByUserId)`; generated for the foreign key.
- `IX_Teams_Sport_IsActive` on `Teams(Sport, IsActive)`.
- `IX_Teams_CreatedByUserId` on `Teams(CreatedByUserId)`; generated for the foreign key.
- `IX_TeamCoaches_CoachUserId` on `TeamCoaches(CoachUserId)`.
- `IX_TeamAthletes_AthleteUserId` on `TeamAthletes(AthleteUserId)`.

The leading columns of each composite primary key also provide indexes for Team-based membership queries.

## 9. Foreign keys

- `ParentAthletes.ParentUserId -> AspNetUsers.Id`
- `ParentAthletes.AthleteUserId -> AspNetUsers.Id`
- `ParentAthletes.CreatedByUserId -> AspNetUsers.Id`
- `Teams.CreatedByUserId -> AspNetUsers.Id`
- `TeamCoaches.TeamId -> Teams.Id`
- `TeamCoaches.CoachUserId -> AspNetUsers.Id`
- `TeamAthletes.TeamId -> Teams.Id`
- `TeamAthletes.AthleteUserId -> AspNetUsers.Id`

All multiple relationships to `ApplicationUser` are configured explicitly, preventing ambiguous convention-based mappings.

## 10. Delete behaviors

| Relationship | Behavior |
|---|---|
| ParentAthlete -> Parent ApplicationUser | NoAction |
| ParentAthlete -> Athlete ApplicationUser | NoAction |
| ParentAthlete -> CreatedBy ApplicationUser | NoAction |
| Team -> CreatedBy ApplicationUser | NoAction |
| TeamCoach -> Coach ApplicationUser | NoAction |
| TeamAthlete -> Athlete ApplicationUser | NoAction |
| TeamCoach -> Team | Restrict |
| TeamAthlete -> Team | Restrict |

Identity-user deletion cannot silently erase relationship/history rows. Team deletion is also restricted rather than cascading because `Team.IsActive` provides a non-destructive lifecycle and memberships contain useful historical context. A future administrative deletion workflow would need to make an explicit archival/removal decision.

## 11. TeamRole constants and design

`TeamRoles` centralizes the initially supported relationship-specific values:

- `HeadCoach`
- `AssistantCoach`
- `SkillsCoach`

It also exposes `All` as a read-only collection for future server-side validation. These are not ASP.NET Core Identity roles and no client-supplied team role is trusted by the foundation service. A lookup table was intentionally not introduced.

## 12. ApplicationUser navigation changes

No navigation collections were added to `ApplicationUser`. Each new relationship has clear, explicitly configured navigation properties on the relationship entity, while `Team` has only the useful `Coaches` and `Athletes` collections. This avoids adding five or more overlapping collections to the Identity credential entity and prevents ambiguous multiple relationships to `AspNetUsers`.

## 13. IRelationshipAccessService methods

- `CanParentAccessAthleteAsync(parentUserId, athleteUserId, cancellationToken)`
- `CanCoachAccessAthleteAsync(coachUserId, athleteUserId, cancellationToken)`
- `CanCoachManageTeamAsync(coachUserId, teamId, cancellationToken)`
- `GetAccessibleAthleteIdsAsync(actorUserId, cancellationToken)`
- `IsUserInRoleAsync(userId, expectedRole, cancellationToken)`

The API implementation is registered as scoped and uses `AppDbContext` plus `UserManager<ApplicationUser>`. Expected application roles are validated against canonical `ApplicationRoles`; arbitrary role strings are rejected.

No assignment-specific behavior was added.

## 14. AthleteAccessScope design

`AthleteAccessScope` is an immutable access result containing:

- `ActorUserId`: required positive authenticated Identity user ID.
- `IsAdministrator`: explicit unrestricted-access flag.
- `AthleteUserIds`: a deduplicated read-only set of authorized Athlete Identity IDs.
- `CanAccessAthlete(athleteUserId)`: evaluates explicit administrator override or set membership.

No `null` value means unrestricted access. This type prepares a safer replacement for the existing nullable ownership-scope convention without changing current Progress or Schedule behavior in Phase 5A.

## 15. Parent access evaluation

Direct Parent-to-Athlete checks require:

1. The parent user to exist and have canonical Identity role `Parent`.
2. The athlete user to exist and have canonical Identity role `Athlete`.
3. An active `ParentAthlete` row for the exact pair.

The aggregate access scope includes Athlete IDs from active `ParentAthlete` rows when the actor's server-derived Identity roles include `Parent`.

## 16. Coach-to-Athlete access derivation

Coach access is derived only through active team relationships:

`TeamCoach (active) -> Team (active) -> TeamAthlete (active)`

Direct checks also require the source user to have canonical Identity role `Coach` and the target user to have canonical Identity role `Athlete`. `CanCoachManageTeamAsync` requires the Coach Identity role, an active Team, and an active TeamCoach membership. No `CoachAthlete` entity was introduced.

## 17. Explicit Administrator access

`GetAccessibleAthleteIdsAsync` obtains the actor and roles from ASP.NET Core Identity. An actor with the canonical `Administrator` role receives an `AthleteAccessScope` with `IsAdministrator = true`; an empty Athlete ID set does not carry special meaning. `CanAccessAthlete` recognizes this explicit flag.

## 18. Migration operations

Migration: `20260812022115_Phase5ARelationshipTeamFoundation`

The `Up` method contains:

- **CreateTable: 4**
  - `ParentAthletes`
  - `Teams`
  - `TeamAthletes`
  - `TeamCoaches`
- **CreateIndex: 6**
  - The six indexes listed above.
- **Foreign keys: 8**
  - The eight foreign keys listed above.
- **AddColumn: 0**
- **AlterColumn: 0**
- **DropColumn: 0**
- **DropTable: 0**
- **Rename operations: 0**
- **Raw SQL/data backfill operations: 0**

The migration's `Down` method drops only the four newly introduced Phase 5A tables, in dependency-safe order. It does not affect pre-existing tables.

## 19. Destructive-operation audit

The forward (`Up`) migration has no destructive operations. It does not drop, rename, or alter any existing table or column. It creates only the four approved Phase 5A tables and their supporting constraints/indexes.

## 20. Data backfill

There is no data backfill, raw SQL, automatic relationship creation, or arbitrary user/team association. All new tables begin empty.

## 21. Effect on existing users and data

Existing `AspNetUsers`, legacy `Users`, `ProgressLogs`, `Schedules`, and `Drills` rows and schemas are unchanged. The new foreign keys reference existing Identity users but do not modify them. Current JWT, role authorization, Progress ownership, Schedule ownership, and drill authorization code paths are untouched.

Because no relationship or team endpoint was added and the new access service was not wired into existing controllers, Parent and Coach cross-user API access remains disabled.

## 22. Core build result

Command:

```powershell
dotnet build SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --no-restore
```

Result: **Passed** — 0 warnings, 0 errors.

## 23. API build result

Command:

```powershell
dotnet build SkillBuilderPro.API\SkillBuilderPro.API.csproj --no-restore
```

Result: **Passed** — 0 warnings, 0 errors.

An initial `dotnet ef migrations add` attempt reported only `Build failed` without diagnostics. A direct API build immediately afterward passed with 0 warnings/errors, and generation succeeded with `--no-build` against those fresh binaries. The final post-generation Core and API builds both passed, confirming no compiler failure remains.

## 24. Risks and unresolved architectural questions

- No management endpoint exists yet, so future write workflows must set `CreatedByUserId` and timestamps server-side and validate canonical Identity roles before inserting relationships.
- `TeamRole` supported values are centralized but not enforced with a database check constraint. Future management-service validation is required; a lookup/check constraint can be considered if extensibility requirements settle.
- Existing links can become semantically stale if a user's Identity role later changes. Direct access checks validate target/source roles; governance for role changes and relationship deactivation remains a future administrative concern.
- Users with multiple canonical roles receive the union of applicable athlete links. This is deliberate but should remain part of future authorization tests.
- Team deletion is restricted. Future lifecycle workflows should prefer `IsActive = false` and define explicit archival rules rather than hard deletion.
- Existing Progress/Schedule services still use their verified Phase 4 ownership logic. Migration to `AthleteAccessScope` is intentionally deferred so Phase 5A does not broaden behavior.
- No integration tests were added because Phase 5A introduces no public endpoint. Future relationship-management and cross-user authorization phases need tests for inactive links, role mismatches, multi-team duplicates, and administrator behavior.

## 25. Exact local migration command

Review the migration against the intended database configuration, then apply it locally from the repository root with:

```powershell
dotnet ef database update 20260812022115_Phase5ARelationshipTeamFoundation --project SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --startup-project SkillBuilderPro.API\SkillBuilderPro.API.csproj --context AppDbContext
```

This command was **not run** during implementation.
