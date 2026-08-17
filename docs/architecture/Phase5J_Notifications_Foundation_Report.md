# Phase 5J Notifications Foundation Report

1. Added: `Core/Models/Notification.cs`; API notification contracts, service, processor/background service, factory, controllers; migration pair; report.
2. Modified: AppDbContext, Program, appsettings, AssignmentService, TrainingRequestService, GoalService, ProgressionMilestoneService, model snapshot.
3. Notification: recipient/optional actor, type/source key, safe title/message, semantic related entity, action route, read state/timestamps.
4. NotificationEvent: event/source identity, recipient/actor/subject, safe rendered content/reference, occurrence/processing timestamps, attempts/error.
5. Types: 10 centralized constants covering request received/approved/declined/cancelled, assignment created/completed, goal completed, rank/skill/achievement earned.
6. Source keys are deterministic domain identifiers shown in the specification patterns.
7. Notification uniqueness: `(RecipientUserId, Type, SourceKey)`.
8. Event uniqueness: `(EventType, SourceKey, RecipientUserId)`.
9. Recipients are derived exclusively inside domain services; no public notification create API.
10. Authoritative transitions enqueue durable events; processor materializes inbox records.
11. Scoped bounded polling processor plus hosted service.
12. Options: 10 seconds, batch 20, maximum 5 attempts, validated configuration.
13. Failed events remain pending and retry below MaxAttempts.
14. Events at MaxAttempts remain unprocessed for future Admin repair tooling.
15. Processor failures do not invoke providers or modify source domain state.
16. Notification insert and ProcessedAtUtc use one SaveChanges automatic transaction.
17. Existence check plus final unique index ensures insertion idempotency.
18. Duplicate races are sanitized and treated as processed when the notification exists.
19. New TrainingRequest targets exactly one Parent/Coach received event.
20. Actual Pending-to-Approved transition emits one Athlete event.
21. Actual Pending-to-Declined transition emits one Athlete event.
22. Actual Pending-to-Cancelled transition emits one targeted recipient event.
23. Repeated approval returns prior state without emission.
24. Repeated decline returns prior state without emission.
25. Repeated cancel cannot transition and emits nothing.
26. Assignment creation emits one event per materialized Athlete recipient.
27. Team/selected assignment recipients alone receive events.
28. Completion emits a separate event keyed from assignment/Athlete completion.
29. Only assignment creator receives completion notification.
30. Active-to-Completed goal transition emits goal event(s).
31. Athlete-created goal yields one Athlete notification.
32. Parent-created goal yields Athlete and creator notifications.
33. Coach-created goal yields Athlete and creator notifications.
34. New rank-history insert emits RankEarned.
35. New skill-history insert emits SkillLevelEarned.
36. New AthleteAchievement insert emits AchievementEarned.
37. No migration or startup backfill; existing durable history is excluded by existing-row checks.
38. Existing startup pending-migration guard prevents normal initialization; hosted processor safely retries until schema is applied.
39. Athlete: list, detail, unread-count, read, read-all.
40. Parent: same five routes.
41. Coach: same five routes.
42. Admin inbox impersonation/diagnostics deferred.
43. Explicit safe NotificationResponse; no EF entity/email.
44. Page DTO contains items/page/pageSize/total/unread count.
45. Unread count is derived by indexed SQL count.
46. Mark-read is recipient-scoped and idempotent.
47. Mark-all uses set-based ExecuteUpdateAsync.
48. Every query/update predicates RecipientUserId.
49. Missing and cross-user IDs both return 404.
50. New notifications are unread; read cannot be reversed.
51. Repeated read preserves original ReadAtUtc.
52. Semantic `/training-requests`, `/assignments`, `/goals`, `/trophy-room` hints only.
53. No notes, request messages, emails, tokens, or exception internals in messages.
54. System-generated minimal text protects minor privacy.
55. Related entity uses semantic type/id, not polymorphic FK.
56. User recipient/actor/subject FKs only.
57. All notification user FKs use NoAction.
58. Recipient/read/date and recipient/date indexes.
59. Pending event attempts/date index.
60. Both event and final notification unique indexes included.
61. ProcessingAttempts nonnegative check only.
62. Migration: `20260813193507_AddNotificationFoundation`.
63. Up audit: creates only Notifications/NotificationEvents, FKs, indexes, one safe check.
64. Destructive Up operations: NO.
65. Data backfill: NO.
66. Migration applied: NO.
67. Phase 5I approval retains execution-strategy serializable transaction; event joins its final save.
68. Phase 5I request idempotency preserved.
69. Goal synchronization remains terminal/idempotent; emission occurs only during transition.
70. Existing-history checks prevent Trophy Room repair duplicate events.
71. Phase 5F processor semantics unchanged; completion notification event is separately durable.
72. Phase 5G progression rules/calculation unchanged.
73. TrainingSchedule untouched.
74. Direct provider calls: NO.
75. Email: NO.
76. SMS: NO.
77. Push: NO.
78. MAUI modified: NO.
79. WinForms modified: NO.
80. AI modified: NO.
81. Preference table: NO.
82. Scheduled reminders: NO.
83. No delete endpoint or automatic retention purge.
84. Inbox projects/paginates at SQL level without N+1.
85. Page defaults 1/20, size clamped 1-100, newest then Id descending.
86. Unread query uses composite recipient/read/date index.
87. Read-all is one set-based update and preserves already-read timestamps.
88. Logs use IDs/type/attempt only, not content.
89. LastError is fixed sanitized text, max 1000.
90. Added validated `NotificationProcessing` configuration.
91. Runtime/manual verification not performed because migration was not applied.
92. Database scenarios are blocked pending reviewed migration application/API restart.
93. New request should produce exactly one recipient unread item.
94. Approval should produce exactly one Athlete item; repeat produces none.
95. Decline should produce exactly one Athlete item.
96. Cancel should produce exactly one targeted-recipient item.
97. Coach received/approval behavior mirrors exact targeted flow.
98. Assignment creation should produce one item per recipient.
99. Completion should produce one creator item.
100. Goal transition should notify Athlete plus distinct external creator.
101. Rank test is statically verified; avoid fabricated evidence.
102. Skill test is statically verified; avoid fabricated evidence.
103. Achievement test is statically verified; avoid fabricated evidence.
104. Existing Phase 5H/5I records should produce no historical spam.
105. Test unread increments, one read decrements, read-all reaches zero.
106. Test read twice and confirm unchanged ReadAtUtc.
107. Test multiple unread plus pre-read item and confirm only unread rows update.
108. Test Athlete/Parent/Coach using each other's IDs; expect 404.
109. Duplicate SQL: group Notifications by RecipientUserId,Type,SourceKey having count > 1; expect zero rows.
110. Restart after processing and confirm unique indexes prevent duplicates.
111. Risks: runtime concurrency/startup verification pending; outbox insertion after generated-ID assignment/request creation uses a second save.
112. Debt: no failed-event Admin replay UI/lease/retention policy; safe generic messages omit display names in some events.
113. DTOs support bell badge, inbox, unread styling, navigation and read actions.
114. Future Push: implement channel worker downstream of durable events/preferences.
115. Future Email: opt-in channel with verified destinations and minor-safe templates.
116. Future SMS: explicit consent/guardian controls and delivery auditing.
117. Add per-event/channel preferences in a later migration.
118. Add timezone-aware quiet hours later, not in the domain transaction.
119. Backend is ready for notification bell UI after runtime acceptance.
120. Backend is ready for initial MAUI inbox after runtime acceptance.
121. Next: `dotnet ef database update --project SkillBuilderPro.Core --startup-project SkillBuilderPro.API`, then restart API (only after review).
122. Swagger: create request; poll recipient; approve/decline/cancel; create/complete assignment; complete goal; verify paging/count/read/read-all/cross-user/repeats.
123. Transaction risk: MODERATE pending runtime testing; processor itself uses one automatic transaction and approval retry strategy remains correct.
124. Authorization risk: LOW pending negative runtime verification.
125. Idempotency risk: LOW-MODERATE; dual unique constraints are strong, concurrent runtime test pending.
126. Integrity: PASS statically; conservative FKs/indexes and schema-only migration.
127. Readiness: implementation and isolated compile complete; migration application/runtime acceptance pending.
128. Recommended next phase: notification runtime acceptance, then a separately reviewed MAUI notification bell/inbox phase.

## Builds

- Core requested build: PASS, 0 warnings, 0 errors.
- API default build: BLOCKED because the running API locks `SkillBuilderPro.API/bin/Debug/net10.0/SkillBuilderPro.Core.dll`; isolated build PASS, 0 warnings, 0 errors. Release build PASS.
