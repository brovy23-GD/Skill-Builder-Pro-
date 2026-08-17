# Phase 5G Progression Engine Foundation Report

## Outcome

Phase 5G is complete. Deterministic event-linked ProgressLog evidence now drives recalculable per-skill progression, overall rank/score, UTC streaks, materialized state, authorized read APIs, and best-effort Phase 5F processor integration. No migration was applied and no later-phase feature was implemented.

## 1. Every file added

- `SkillBuilderPro.Core/Models/AthleteProgression.cs`
- `SkillBuilderPro.Core/Models/AthleteSkillProgress.cs`
- `SkillBuilderPro.Core/Progression/ProgressionRules.cs`
- `SkillBuilderPro.Core/Interfaces/IProgressionService.cs`
- `SkillBuilderPro.API/Services/ProgressionService.cs`
- `SkillBuilderPro.API/Contracts/Progression/ProgressionResponses.cs`
- `SkillBuilderPro.API/Controllers/AthleteProgressionController.cs`
- `SkillBuilderPro.API/Controllers/AdminProgressionController.cs`
- `SkillBuilderPro.Core/Migrations/20260813155235_AddAthleteProgressionFoundation.cs`
- `SkillBuilderPro.Core/Migrations/20260813155235_AddAthleteProgressionFoundation.Designer.cs`
- `docs/architecture/Phase5G_Progression_Engine_Foundation_Report.md`

## 2. Every file modified

- `SkillBuilderPro.Core/Data/AppDbContext.cs`
- `SkillBuilderPro.Core/Migrations/AppDbContextModelSnapshot.cs`
- `SkillBuilderPro.API/Services/AssignmentCompletionEventProcessor.cs`
- `SkillBuilderPro.API/Controllers/ParentController.cs`
- `SkillBuilderPro.API/Controllers/CoachController.cs`
- `SkillBuilderPro.API/Program.cs`

## 3. Qualifying evidence policy

Only `ProgressLog` rows with non-null `AssignmentCompletionEventId` and `OwnerUserId == AthleteUserId` qualify. These are verified, idempotently linked assignment completions.

## 4. Manual ProgressLog counting policy

Manual/legacy rows remain supported but do not count because their event linkage is null.

## 5. Skill taxonomy key

Skill key is Sport + Category + SubCategory. DrillGroup remains grouping metadata and is excluded.

## 6. Taxonomy normalization

Grouping trims values, compares canonical uppercase invariant keys, and maps null/blank SubCategory to one empty key. Stored display values are trimmed values from the first matching Drill record, avoiding case-duplicate rows under SQL Server's normal case-insensitive collation plus the unique index.

## 7. AthleteSkillProgress schema

Identity Id; AthleteUserId; Sport/Category/SubCategory nvarchar(100); CurrentLevel; QualifyingCompletions; nullable AverageRating; Current/LongestStreak; nullable LastCompletedAtUtc; ProgressToNextLevel; UpdatedAtUtc.

## 8. AthleteProgression schema

AthleteUserId PK; OverallRank; ProgressionScore; TotalQualifyingCompletions; ActiveSkillCount; Current/LongestOverallStreak; nullable LastCompletedAtUtc; ProgressToNextRank; UpdatedAtUtc.

## 9. Unique keys

AthleteProgression uses AthleteUserId PK. AthleteSkillProgress has unique `(AthleteUserId, Sport, Category, SubCategory)`.

## 10. Foreign keys

Both entities reference `AspNetUsers.Id` through AthleteUserId.

## 11. Delete behaviors

Both Athlete FKs use NoAction to preserve derived audit state against accidental user deletion.

## 12. Skill level names

1 Foundation, 2 Developing, 3 Competent, 4 Advanced, 5 Elite.

## 13. Skill level thresholds

Minimum qualifying completions are 0, 3, 8, 15, and 25 respectively.

## 14. Rating role in skill progression

Completion count determines level. AverageRating is secondary display evidence only; missing ratings do not block advancement.

## 15. ProgressToNextLevel formula

For current threshold C, next threshold N, and completions X: floor `(X-C)*100/(N-C)`, clamped 0–100. Level 5 returns 100.

## 16. Overall rank ladder

1 Rookie, 2 Rising Star, 3 Competitor, 4 Playmaker, 5 All-Star, 6 Elite, 7 Champion, 8 Legend.

## 17. Progression score formula

Score = total completions + 5 points per level above Foundation across skills + 3 points per active skill beyond the first + min(longest overall streak, 10). It is fully recalculated.

## 18. Rank thresholds

Minimum scores: Rookie 0, Rising Star 10, Competitor 25, Playmaker 50, All-Star 90, Elite 140, Champion 210, Legend 300.

## 19. Breadth requirements

Minimum active skills by rank: 0, 1, 1, 2, 2, 3, 3, 4. Higher ranks reward multiple skill areas without requiring multiple sports.

## 20. Overall ProgressToNextRank formula

Uses the same clamped interval percentage between current and next score thresholds. Legend returns 100. Breadth remains an independent rank gate and is visible through ActiveSkillCount.

## 21. Streak definition

One streak day is a distinct UTC calendar date with at least one qualifying completion. Multiple same-day completions count once.

## 22. Current streak algorithm

Consecutive distinct days ending today UTC or yesterday UTC are active; yesterday receives grace until today ends. An older last day yields zero.

## 23. Longest streak algorithm

Sort/distinct UTC dates, scan consecutive one-day intervals, and retain the maximum run. Input order and duplicate dates cannot distort it.

## 24. Skill streak behavior

The same algorithm runs on qualifying timestamps within each normalized skill group.

## 25. Overall streak behavior

The same algorithm runs across all qualifying Athlete timestamps, with same-day duplicates collapsed.

## 26. IProgressionService design

Exposes targeted `RecalculateAthleteAsync`, `GetAthleteProgressionAsync`, and `GetAthleteSkillsAsync`; rules and persistence stay out of controllers.

## 27. Recalculation strategy

One Athlete's event-linked ProgressLogs and Drill taxonomy are projected in one query, grouped, fully calculated, then all current rows are upserted/removed and saved once.

## 28. Idempotency strategy

Every metric derives from current evidence rather than previous counters. Repeated recalculation produces the same values.

## 29. Double-count prevention

Each qualifying ProgressLog row is counted once; Phase 5F event-link uniqueness prevents duplicate evidence per event. No blind increments exist.

## 30. Stale-row behavior

Skill rows absent from recalculated evidence are removed. If no evidence remains, all skill rows and overall state are removed; reads return a non-persisted Rookie default.

## 31. ProgressLog query design

SQL filters by Athlete OwnerUserId and non-null event linkage, joins/projections through Drill, and loads only date, rating, and taxonomy for one Athlete.

## 32. Drill taxonomy query behavior

Drill is joined in the evidence projection, avoiding N+1 queries. Sport/Category/SubCategory form the normalized grouping key.

## 33. Event processor integration

After Phase 5F persists a new linked ProgressLog or recovers an existing link, it invokes targeted recalculation for that Athlete. Completion controllers remain uncoupled.

## 34. Progression failure after ProgressLog success

Recalculation runs after the Phase 5F atomic SaveChanges. Failures are safely logged by Athlete ID and exception type; ProgressLog/event processing remain committed and read repair remains available.

## 35. Materialization timing

Promptly after processor success, and targeted on every authorized progression read when evidence/materialized state exists. No all-Athlete startup scan occurs.

## 36. Staleness detection

Phase 5G favors correctness: if an Athlete has evidence or materialized state, reads perform targeted recalculation. This detects count, rating, timestamp, and taxonomy corrections rather than relying on an incomplete timestamp heuristic.

## 37. Repair/recalculate design

Internal `RecalculateAthleteAsync(int)` is safe at any time and is not publicly exposed. A unique-key race triggers one clear/reload/recalculate retry.

## 38. Athlete routes

- `GET /api/athlete/progression`
- `GET /api/athlete/progression/skills`

Identity comes only from JWT/current user.

## 39. Parent routes

- `GET /api/parent/athletes/{athleteUserId}/progression`
- `GET /api/parent/athletes/{athleteUserId}/progression/skills`

Active relationship and canonical roles are enforced by `IRelationshipAccessService`; unauthorized targets return 404.

## 40. Coach routes

- `GET /api/coach/athletes/{athleteUserId}/progression`
- `GET /api/coach/athletes/{athleteUserId}/progression/skills`

Active Coach-Team-Athlete authorization is centralized; unauthorized targets return 404.

## 41. Admin access decision

Explicit admin-only routes were added at `/api/admin/athletes/{athleteUserId}/progression` and `/skills`; target canonical Athlete role is validated and invalid targets return 404.

## 42. Athlete progression DTO

Includes Athlete ID, rank name/number, score, next-rank progress/name/threshold/gap, totals, active skills, streaks, last completion, and update timestamp.

## 43. Skill progression DTO

Includes taxonomy, level/name, completions, nullable average rating, streaks, last completion, next-level progress/name, completions needed, and update timestamp.

## 44. No-evidence behavior

Returns Rookie/1, score 0, completions/skills/streaks 0, next rank Rising Star, threshold/gap 10, progress 0, null completion/update, and empty skills without write-on-empty-read.

## 45. Active skill definition

Number of normalized skill keys with at least one qualifying completion.

## 46. Average rating behavior

Average only non-null ratings; no rated evidence yields null. Internal double precision is retained.

## 47. LastCompletedAtUtc behavior

Overall is max qualifying LogDate; skill is max within that group.

## 48. Concurrency/upsert design

Unique keys prevent duplicate rows. Tracked upsert uses one SaveChanges; a DbUpdate unique race clears state and performs one deterministic retry. No distributed lock is added.

## 49. SaveChanges/transaction behavior

Overall plus all skill inserts/updates/deletes use one `SaveChangesAsync`, yielding an atomic derived snapshot.

## 50. Execution-strategy compatibility

No user transaction is used; EF automatic transactions remain compatible with SQL Server retrying execution strategy.

## 51. Migration name

`20260813155235_AddAthleteProgressionFoundation` (logical name `AddAthleteProgressionFoundation`).

## 52. Migration Up audit

Up contains two CreateTable operations, PKs, two NoAction FKs, one unique index, and seven check constraints. No existing-table alterations, drops, renames, raw SQL, or data operations exist.

## 53. New tables

`AthleteProgressions` and `AthleteSkillProgress` only.

## 54. Indexes

Only unique `(AthleteUserId, Sport, Category, SubCategory)` beyond PK/FK needs. No speculative level index.

## 55. Check constraints

Rank 1–8, level 1–5, percentages 0–100, counts/scores/streaks nonnegative, and nullable average rating 1–5.

## 56. Destructive operations

Destructive Up operations: **NO**.

## 57. Data backfill

Data backfill: **NO**.

## 58. Migration applied

Migration applied: **NO**.

## 59. Tests added

Unit/rule tests added: **NO**. No test project exists, and no new framework was introduced.

## 60. Skill-rule test results

Prepared checks cover thresholds 0/3/8/15/25 and 40% progress at five completions in Developing. Execution was blocked because the temporary harness could not read user NuGet.Config under sandbox permissions; the harness was removed. Production projects compile successfully.

## 61. Streak test results

Prepared cases cover empty, same-day duplicates, consecutive days, gap/current reset, historical longest, expired current, yesterday grace, and unordered/distinct handling. Harness execution was environment-blocked as above.

## 62. Rank-rule test results

Prepared cases cover score formula and breadth gating (score 50 with one skill remains Competitor; with two reaches Playmaker). Harness execution was environment-blocked as above.

## 63. Core build

`dotnet build SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --no-restore`: **SUCCEEDED — 0 warnings, 0 errors.**

## 64. API build

Exact Debug build was environment-blocked by running API PID 25056 locking the copied Core DLL: 10 retry warnings and 2 copy errors, no source compiler errors. Isolated `--no-restore` build: **SUCCEEDED — 0 warnings, 0 errors.** Release migration build also succeeded. Temporary outputs were removed.

## 65. Runtime/manual verification performed

Static verification covered compilation, model/rules, SQL evidence filter, role/resource authorization, processor sequencing, atomic snapshot, migration audit, and absence of prohibited integrations. Runtime database verification was not performed because migration application was prohibited.

## 66. Tests blocked by migration/environment

Progression tables do not exist until reviewed migration application/API restart. Rule harness restore was blocked by inaccessible user NuGet.Config. Local accounts/evidence remain user-managed.

## 67. Local Athlete 7 expected verification behavior

After application/restart, the first progression request targets recalculation. Existing linked Drill 239 evidence should create Basketball/Offense/Shooting skill state with actual database count and rule-derived level; overall remains Rookie unless evidence meets thresholds.

## 68. Manual threshold-crossing test plan

Complete qualifying assignments in one skill to counts 3, 8, 15, and 25; after processing verify exact single level transitions, percentages, and stable repeated reads/recalculation.

## 69. Multi-skill test plan

Complete an event-linked Drill with another SubCategory; verify a second row, ActiveSkillCount increment, breadth bonus, and deterministic score/rank recalculation.

## 70. CountsTowardProgression=false regression

Phase 5F creates no ProgressLog; therefore Phase 5G evidence/query/state remain unchanged.

## 71. Manual ProgressLog regression

Manual creation DTO still requires rating 1–5 and behavior is unchanged; null event linkage excludes the row from progression only.

## 72. Phase 5F processor regression

ProgressLog/event atomicity is unchanged. Progression runs afterward and cannot unprocess an event or remove evidence on failure.

## 73. Phase 5C Progress authorization regression

Progress controllers/services were not changed. OwnerUserId isolation and relationship authorization remain intact.

## 74. TrainingSchedule status

Untouched and never queried/written by progression.

## 75. Progression implementation scope

Current deterministic per-skill and overall state, score/rank, streaks, repair, reads, and processor hook only.

## 76. Goals implementation status

Not implemented.

## 77. Achievements implementation status

Not implemented.

## 78. Trophy Room implementation status

No history/backend artifacts or UI implemented.

## 79. Notifications implementation status

Not implemented.

## 80. TrainingRequest implementation status

Not implemented.

## 81. AI implementation status

Not implemented.

## 82. Security risks/unresolved questions

Taxonomy source fields lack model max lengths while materialized values are capped at 100; current Drill data must be verified within limits. Parent/Coach access is intentionally live, while Admin is explicit. Rule changes will recompute current rank and may downgrade state because no earned-history guarantee exists.

## 83. Scoring/rule tuning concerns

Thresholds/formula are transparent initial defaults, not product telemetry-derived. Validate pacing with real completion volumes before production; keep changes centralized in `ProgressionRules`.

## 84. Future timezone streak concern

UTC days are authoritative now. User-local timezone preferences and historical timezone changes require a later reviewed design.

## 85. Future rank-history concern

Only current state exists. Highest-rank/rank-history/milestone permanence belongs to a later phase.

## 86. Future achievement trigger concern

Recalculation emits no level/rank events. Achievements need idempotent transition/history evidence rather than comparing transient UI responses.

## 87. Recommended Swagger/SQL verification sequence

Review/apply migration; restart API; query Athlete/Parent/Coach/Admin routes; inspect two materialized tables; verify Athlete 7 evidence; cross thresholds; add second skill; test same-day streak; repeat reads; test unrelated 404/anonymous 401; confirm manual/counts-false exclusions and Phase 5F/Progress/Schedule regressions.

## 88. Recommended production considerations

Monitor recalculation duration/query volume, validate taxonomy lengths/quality, tune rules from observed data, add focused automated rule tests in the repository's future test project, and consider a revision/version marker if read-time recalculation becomes too costly.

## 89. Recommended Phase 5H next step

Add idempotent rank/level transition history and achievement evidence before Trophy Room consumption. Keep goals and notifications as independent consumers.

## 90. Trophy Room UI readiness

Current-state backend responses are ready for a future read-only Trophy Room/progression UI, but milestone/history/achievement displays are not yet supported.

## 91. Exact current API routes for future MAUI integration

- `GET /api/athlete/progression`
- `GET /api/athlete/progression/skills`
- `GET /api/parent/athletes/{athleteUserId}/progression`
- `GET /api/parent/athletes/{athleteUserId}/progression/skills`
- `GET /api/coach/athletes/{athleteUserId}/progression`
- `GET /api/coach/athletes/{athleteUserId}/progression/skills`
- `GET /api/admin/athletes/{athleteUserId}/progression`
- `GET /api/admin/athletes/{athleteUserId}/progression/skills`

## 92. Technical debt introduced

Targeted reads currently recalculate/write whenever evidence/materialized state exists; correct but potentially expensive at scale. No automated test project exists. Taxonomy materialization relies on current SQL collation for case-insensitive uniqueness and first-evidence display casing.

## 93. Final Phase 5G readiness verdict

**READY FOR MIGRATION REVIEW AND LOCAL VERIFICATION.** Phase 5G code and schema are complete, deterministic, idempotent, authorization-scoped, and build-clean in isolated output. It is not ready for production or Phase 5H until the migration is applied and the documented evidence/rule/authorization matrix passes.
