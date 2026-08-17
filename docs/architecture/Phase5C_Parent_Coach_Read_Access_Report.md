# Phase 5C Parent and Coach Read Access Report

## Outcome

Phase 5C is complete. Parent and Coach users now have read-only discovery and explicit Athlete-targeted Progress/Schedule routes governed by the centralized `IRelationshipAccessService` and `AthleteAccessScope`. Existing self routes, Administrator behavior, and all mutations remain unchanged. No schema change or migration was created/applied.

## 1. Files added

- `SkillBuilderPro.API/Contracts/Access/AthleteSummaryResponse.cs`
- `SkillBuilderPro.API/Services/IRelationshipDiscoveryService.cs`
- `SkillBuilderPro.API/Services/RelationshipDiscoveryService.cs`
- `SkillBuilderPro.API/Controllers/ParentController.cs`
- `SkillBuilderPro.API/Controllers/CoachController.cs`
- `docs/architecture/Phase5C_Parent_Coach_Read_Access_Report.md`

## 2. Files modified

- `SkillBuilderPro.Core/Interfaces/IRelationshipAccessService.cs`
- `SkillBuilderPro.Core/Interfaces/IProgressService.cs`
- `SkillBuilderPro.Core/Interfaces/IScheduleService.cs`
- `SkillBuilderPro.API/Services/RelationshipAccessService.cs`
- `SkillBuilderPro.API/Services/ProgressService.cs`
- `SkillBuilderPro.API/Services/ScheduleService.cs`
- `SkillBuilderPro.API/Controllers/ProgressController.cs`
- `SkillBuilderPro.API/Controllers/SchedulesController.cs`
- `SkillBuilderPro.API/Program.cs`

The worktree also contains earlier Phase 5A/5B/bootstrap changes; they were preserved and are not Phase 5C additions.

## 3. Final Parent read API design

Parent discovery is role-specific and thin: `GET /api/parent/athletes` requires `ApplicationRoles.Parent`, derives the Parent ID from the JWT, and returns only active, role-valid linked Athletes. Progress/Schedule access uses the generic explicit Athlete-targeted routes, avoiding duplicate Parent-specific domain endpoints.

## 4. Final Coach read API design

Coach discovery uses `GET /api/coach/teams` and `GET /api/coach/teams/{teamId}/roster`, both requiring `ApplicationRoles.Coach`. Athlete Progress/Schedules use the same generic explicit routes as Parents. This provides dashboard discovery without duplicating domain-query logic.

## 5. Generic Athlete-targeted routes

All require one of Athlete, Parent, Coach, or Administrator and then enforce resource-level `AthleteAccessScope`:

- `GET /api/Progress/athlete/{athleteUserId}`
- `GET /api/Progress/athlete/{athleteUserId}/{progressId}`
- `GET /api/Progress/athlete/{athleteUserId}/average/{drillId}`
- `GET /api/Schedules/athlete/{athleteUserId}?completed=...`
- `GET /api/Schedules/athlete/{athleteUserId}/{scheduleId}`

The supplied Athlete ID is a query target, never authorization proof.

## 6. Parent Athlete discovery

`GET /api/parent/athletes` returns `AthleteSummaryResponse`: UserId, DisplayName, Sport, and ExperienceLevel. It excludes email and all Identity/security internals. Only active profiles with active ParentAthlete links and the canonical Athlete role are returned.

## 7. Coach Team discovery

`GET /api/coach/teams` returns active Teams reached through an active TeamCoach relationship for the JWT Coach. `CoachTeamResponse` contains TeamId, Name, Sport, Season, AgeGroup, and Organization.

## 8. Coach roster

`GET /api/coach/teams/{teamId}/roster` first validates active Coach management of that active Team. It returns active, canonical Athlete members with active profiles. Unauthorized/nonexistent Team access returns 404.

## 9. Progress service changes

Added explicit owner-constrained read methods:

- `GetAllForAthleteAsync`
- `GetByIdForAthleteAsync`
- `GetAverageRatingForAthleteAsync`

Every EF query includes `OwnerUserId == athleteUserId`; the collection returns all Progress owned by the target Athlete, and ID ownership checks execute in SQL. The optional `drillId` query parameter was removed from the base Athlete Progress route; Drill-specific aggregation remains available through `GET /api/Progress/athlete/{athleteUserId}/average/{drillId}`. Existing methods and mutations were not changed.

## 10. Schedule service changes

Added:

- `GetAllForAthleteAsync`
- `GetByIdForAthleteAsync`

Queries constrain `OwnerUserId` in SQL and preserve the existing completed-status filter. Existing Schedule reads/mutations remain intact.

## 11. AthleteAccessScope integration

Each explicit Athlete route derives the actor from `ICurrentUser`, validates the target still has the Athlete role, obtains the actor's current `AthleteAccessScope`, and calls `CanAccessAthlete`. Administrator unrestricted access remains the explicit `IsAdministrator` flag; null is not used.

## 12. IRelationshipAccessService changes

Three generic discovery methods were added:

- `GetParentAthleteIdsAsync`
- `GetCoachTeamIdsAsync`
- `GetCoachTeamAthleteIdsAsync`

They remain domain-neutral relationship/access questions and contain no Progress/Schedule logic.

## 13. Parent authorization flow

JWT Parent role -> active ParentAthlete rows -> canonical target Athlete validation -> scope membership -> owner-constrained domain query. Deactivation is observed on every request; no access cache exists.

## 14. Coach authorization flow

JWT Coach role -> active TeamCoach -> active Team -> active TeamAthlete -> canonical target Athlete validation -> scope membership -> owner-constrained domain query.

## 15. Athlete self-access

Existing `GET /api/Progress`, `GET /api/Progress/{id}`, averages, and Schedule self routes retain Phase 4 semantics. The new explicit Athlete routes also allow an Athlete only when the target ID is their own. Existing mutation ownership is unchanged.

## 16. Administrator behavior

Existing broad self-route behavior remains unchanged. On explicit Athlete routes, Administrator receives an explicit unrestricted scope but the target must still be a current canonical Athlete.

## 17. Resource-by-ID non-disclosure

Unauthorized target access returns 404 before querying the record. Authorized queries constrain both record ID and OwnerUserId; nonexistent or mismatched records also return 404. Callers cannot distinguish forbidden existence from absence.

## 18–21. Inactive relationship behavior

- Inactive ParentAthlete: excluded immediately.
- Inactive Team: excludes Team discovery, roster, and derived Athlete access.
- Inactive TeamCoach: excludes Team discovery, roster, and derived Athlete access.
- Inactive TeamAthlete: excluded from roster and derived Athlete access.

Reactivation is reflected on the next request because queries are live and uncached.

## 22. Role staleness

Source Parent/Coach roles are revalidated through Identity. Every discovered or scoped target ID is revalidated for the canonical Athlete role. A stale database relationship alone grants no read access.

## 23. Collection filtering

Progress and Schedule collections filter by OwnerUserId in EF queries. Relationship discovery first returns only authorized IDs, then profile/team projection uses SQL `Contains` filtering. Unauthorized domain rows are never loaded and filtered in memory.

## 24. Response DTOs

- `AthleteSummaryResponse`
- `CoachTeamResponse`

Existing ProgressLog and TrainingSchedule response shapes were retained to avoid unrelated contract changes; their Identity navigations are already JSON-ignored. New discovery APIs never expose EF entities.

## 25. Mutation permissions

No mutation route, authorization attribute, owner assignment, or service mutation was broadened. Parent/Coach explicit routes are GET-only. Existing mutations still operate only under Phase 4 ownership/Admin semantics and cannot target another Athlete via the new route design.

## 26–28. Database and migrations

- Schema changes: **none**
- Migration created: **NO**
- Migration applied: **NO**

No migration file or snapshot change was generated for Phase 5C.

## 29. Core build

`dotnet build SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --no-restore`

**Passed: 0 warnings, 0 errors.**

## 30. API build

`dotnet build SkillBuilderPro.API\SkillBuilderPro.API.csproj --no-restore`

**Passed: 0 warnings, 0 errors.**

## 31. Runtime/manual verification

Compilation and static route/security verification completed. The implementation confirms:

- Every new controller/route requires authentication.
- Parent/Coach discovery has canonical role authorization.
- Generic reads have role authorization plus scope authorization.
- Specific-record queries constrain both target Athlete and resource ID.
- No new POST/PUT/PATCH/DELETE route exists.
- No local numeric test ID appears in production code.

## 32. Tests blocked by environment

Live JWT/database scenarios were not run from this environment because they require the user's configured local User Secrets, accounts, SQL Server, and relationship lifecycle data. The 25 requested scenarios should be run locally in Swagger using the verified development accounts and relationships. No test records or secrets were created here.

## 33. Risks and unresolved questions

- Relationship role filtering currently performs per-Athlete Identity role checks after the initial relationship SQL query. Correctness is prioritized; high-volume deployments may later benefit from a single joined Identity-role query.
- Discovery excludes inactive/missing UserProfiles even if the Identity relationship exists. This matches login/account inactivity semantics but should remain an explicit product rule.
- Existing ProgressLog/TrainingSchedule entities remain response contracts on established endpoints. A later contract-hardening phase could introduce safe DTOs consistently without changing Phase 5C authorization.
- TrainingSchedule remains legacy/current and intentionally unchanged beyond authorized reads.
- Full local lifecycle regression testing remains required.

## 34. Recommended Phase 5D

After completing the local Phase 5C verification matrix, design the common DrillAssignment/recipient model and resource authorization around the same AthleteAccessScope. Keep assignment creation separate from direct Progress/Schedule mutation, define assignment lifecycle and recipient ownership first, and review its migration before implementation/application.

## Local verification checklist

Use Athlete, Parent, Coach, and Administrator JWTs to execute all 25 scenarios from the Phase 5C request. In particular, deactivate/reactivate ParentAthlete, Team, TeamCoach, and TeamAthlete independently and verify access changes immediately; then confirm Parent/Coach mutation attempts cannot target the Athlete and anonymous calls return 401.
