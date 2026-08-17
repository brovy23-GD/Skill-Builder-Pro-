# Phase 5I Goals and Training Requests Report

1. **Files added:** `SkillBuilderPro.Core/Models/AthleteGoal.cs`; `SkillBuilderPro.Core/Models/TrainingRequest.cs`; `SkillBuilderPro.API/Contracts/Goals/GoalContracts.cs`; `SkillBuilderPro.API/Contracts/TrainingRequests/TrainingRequestContracts.cs`; `SkillBuilderPro.API/Services/GoalService.cs`; `SkillBuilderPro.API/Services/TrainingRequestService.cs`; `SkillBuilderPro.API/Controllers/Phase5IControllers.cs`; migration `20260813182828_AddGoalsAndTrainingRequests.cs` and designer; this report.
2. **Files modified:** `SkillBuilderPro.Core/Data/AppDbContext.cs`; `SkillBuilderPro.Core/Migrations/AppDbContextModelSnapshot.cs`; `SkillBuilderPro.API/Program.cs`; `SkillBuilderPro.API/Services/AssignmentCompletionEventProcessor.cs`.
3. **AthleteGoal schema:** Id, AthleteUserId, CreatedByUserId/Role, type/taxonomy, target, status, title/description, due/created/updated/completed/cancelled timestamps.
4. **TrainingRequest schema:** Id, Athlete/recipient/role, optional Team/Drill/taxonomy/message, status/timestamps, optional ApprovedAssignmentId.
5. **Goal types:** QualifyingCompletions, SkillLevel, OverallRank, TrainingStreak.
6. **Goal statuses:** Active, Completed, Cancelled.
7. **Request statuses:** Pending, Approved, Declined, Cancelled.
8. **Creator identity:** JWT `ICurrentUser`; client cannot supply it.
9. **Goal authorization:** self Athlete; active ParentAthlete; active Coach-Team-Athlete path.
10. **Athlete self-goal:** POST creates only for the authenticated Athlete.
11. **Parent goal:** relationship-gated create/read; creator-owned cancel.
12. **Coach goal:** relationship-gated create/read; creator-owned cancel.
13. **Admin decision:** no Admin goal routes; optional scope deferred.
14. **Cancellation ownership:** only the creator can cancel an Active goal.
15. **Progress derivation:** materialized Phase 5G progression/skill state; never client input.
16. **Completion calculation:** total progression count or normalized skill count divided by target.
17. **Skill-level calculation:** current level number versus target 2-5.
18. **Rank calculation:** current rank number versus target 2-8.
19. **Streak calculation:** longest overall streak versus target.
20. **Completion:** synchronization transitions Active to Completed server-side.
21. **Timestamp:** rank/skill uses earliest matching Phase 5H history; count/streak currently use detection time.
22. **Permanence:** Completed goals are terminal and never downgraded.
23. **Repair:** `SynchronizeAthleteGoalsAsync` is targeted and idempotent.
24. **Pipeline:** sync follows ProgressLog, progression, and milestones/achievements.
25. **Read repair:** Athlete-targeted list/detail synchronizes first.
26. **Goal DTO:** explicit response including creator summary, values, percent, status, and dates.
27. **CurrentValue:** count, level, rank, or longest streak according to type.
28. **Percent:** clamped `floor(CurrentValue * 100 / TargetValue)` for all initial types.
29. **IsOverdue:** derived when Active and DueAtUtc is past.
30. **Normalization:** taxonomy is trimmed and uppercased.
31. **Taxonomy validation:** complete taxonomy must match an existing Drill skill.
32. **Targets:** count max 100000; level 2-5; rank 2-8; streak max 3650.
33. **Text validation:** title required/max 150; description max 1000.
34. **Request creation:** authenticated Athlete targets one authorized recipient.
35. **Recipient model:** exact recipient UserId and canonical Parent/Coach role are persisted.
36. **Parent request:** active ParentAthlete required at creation.
37. **Coach request:** active Team, TeamCoach, and TeamAthlete required.
38. **TeamId:** forbidden for Parent, required and persisted for Coach.
39. **RequestedDrillId:** optional; if supplied, existing Drill required; approver may select another Drill.
40. **Request taxonomy:** optional complete known taxonomy only.
41. **Message:** optional, trimmed, max 1000; request must have Drill, taxonomy, or message context.
42. **Athlete read:** own list/detail only.
43. **Athlete cancel:** Pending only; soft transition with CancelledAtUtc.
44. **Parent inbox:** exact targeted Parent only.
45. **Coach inbox:** exact targeted Coach only.
46. **Historical visibility:** inbox reads remain based on persisted target, not current relationship.
47. **Parent approval:** exact target plus current ParentAthlete revalidation.
48. **Coach approval:** exact target plus current stored Team path revalidation.
49. **Approval DTO:** DrillId, schedule/due, instructions, CountsTowardProgression.
50. **Assignment reuse:** calls existing `IAssignmentService` Parent or selected-Team-Athlete path.
51. **Linkage:** successful approval stores resulting AssignmentId.
52. **Atomicity:** assignment save and request-status save share one database transaction.
53. **Execution strategy:** the explicit multi-save transaction is executed inside `Database.CreateExecutionStrategy().ExecuteAsync`.
54. **Approval idempotency:** already Approved returns current representation; pending-only check plus unique link protects repeats.
55. **Decline:** exact target, Pending-only, server timestamp.
56. **Decline idempotency:** already Declined returns the existing representation.
57. **Cancellation:** Cancelled rows remain readable and cannot be approved.
58. **Unique protection:** filtered unique ApprovedAssignmentId index.
59. **Goal indexes:** `(AthleteUserId, Status, CreatedAtUtc)` plus creator FK index.
60. **Request indexes:** athlete/status/date, recipient/status/date, Team, Drill, unique assignment.
61. **Foreign keys:** all requested user/team/drill/assignment FKs exist.
62. **Delete behavior:** NoAction throughout.
63. **Checks:** positive goal target; valid goal type/status; valid request role/status.
64. **Migration:** `20260813182828_AddGoalsAndTrainingRequests`.
65. **Up audit:** exactly creates AthleteGoals and TrainingRequests, FKs, checks, and indexes.
66. **Destructive operations:** NO in Up.
67. **Backfill:** NO.
68. **Migration applied:** NO.
69. **Hard-coded IDs:** none.
70. **Athlete 7 expectation:** Shooting 4/5 maps to 80%, Active, assuming verified materialized data.
71. **Parent expectation:** authorized Parent can create/read/cancel own goal.
72. **Coach expectation:** authorized Coach can create/read/cancel own goal; unrelated Athlete is hidden.
73. **Parent request test:** expected Pending then Approved with normal assignment.
74. **Coach request test:** expected Team-targeted Pending then normal selected-athlete assignment.
75. **Decline test:** expected Declined with no assignment.
76. **Cancel test:** expected Cancelled history; approval rejected.
77. **Repeated approval:** expected same ApprovedAssignmentId, no second assignment.
78. **Negative authorization:** role attributes, exact ownership/target predicates, and relationship 404 behavior are present.
79. **Historical relationship:** creation/approval require active links; historical request reads do not.
80. **Goal history:** Athlete always sees own; Parent/Coach reads require current relationship.
81. **Efficiency:** progression materializations are used; current mapping issues one value query per goal.
82. **Scaling:** sync targets one Athlete, never scans all Athletes.
83. **Concurrency:** pending-state checks, transaction, and unique assignment link provide layered protection.
84. **Logging:** existing pipeline safe IDs only; no message/description/JWT logging added.
85. **Risks:** runtime Swagger/database concurrency tests remain pending; per-goal value queries are scalability debt.
86. **ProgressionRules:** reused for target display names and validated level/rank ranges.
87. **Phase 5F:** outbox behavior unchanged; goal sync is post-processing only.
88. **Phase 5G:** rules/recalculation unchanged.
89. **Phase 5H:** history/achievement behavior unchanged.
90. **Trophy Room:** unchanged; goals are separate.
91. **Progress ownership:** unchanged.
92. **TrainingSchedule:** untouched.
93. **Notifications:** not implemented.
94. **MAUI:** not modified.
95. **WinForms:** not modified.
96. **AI:** not implemented.
97. **Tests added:** no test project exists; no new infrastructure added.
98. **Runtime verification:** not performed because migration was intentionally not applied.
99. **Blocked tests:** database/Swagger scenarios require reviewed migration application and a restarted API.
100. **Swagger sequence:** apply migration; restart API; create/read/cancel four goal types; create Parent and Coach requests; test decline/cancel; approve and repeat approval; verify assignment and authorization negatives.
101. **Production:** add row-version concurrency if approval contention warrants it; monitor goal-sync query counts.
102. **Debt:** N+1 goal value queries and detection-time timestamps for count/streak goals.
103. **Goal MAUI readiness:** backend contract is sufficient for initial Goals UI after runtime verification.
104. **Request MAUI readiness:** backend contract is sufficient for Athlete outbox and Parent/Coach inbox after runtime verification.
105. **Routes:** 12 Goal routes and 12 TrainingRequest routes: Athlete goal POST/GET/GET-id/cancel; Parent and Coach athlete goal equivalents; Athlete request POST/GET/GET-id/cancel; Parent and Coach inbox GET/GET-id/approve/decline.
106. **Transaction risk:** LOW-MODERATE; correct retry strategy and transaction are used, runtime race testing pending.
107. **Authorization risk:** LOW-MODERATE; exact actor/target checks exist, negative runtime testing pending.
108. **Integrity verdict:** schema checks/FKs/unique link plus atomic approval support integrity; runtime concurrency verification pending.
109. **Phase 5J:** consume durable, idempotent notification outbox events for goal completion and request transitions without coupling delivery to these writes.
110. **Mobile UX:** separate Athlete Goals and Requests, recipient selector limited to authorized relationships, approver selects final Drill.
111. **Next steps:** review/apply this one migration, restart API, execute the Swagger sequence, then consider Phase 5J.
112. **Readiness:** Phase 5I implementation and compile verification complete; database/runtime acceptance remains pending migration application.

## Build results

- Core requested build: PASS, 0 warnings, 0 errors.
- API requested default build: BLOCKED by the already-running API locking `SkillBuilderPro.Core.dll`; isolated-output compile verification PASS, 0 warnings, 0 errors. Release build used for migration generation also PASS.
