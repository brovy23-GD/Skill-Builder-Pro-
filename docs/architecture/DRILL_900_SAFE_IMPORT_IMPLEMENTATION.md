# 900-Drill Safe Import Implementation

Date: 2026-08-21  
Status: **VERIFIED LIVE (Development)**

## Importer architecture

The API now has an explicit Development-only command boundary (`import-drills`) backed by a typed source DTO, complete pre-write validation, deterministic planning, a single explicit serializable transaction, EF Core's SQL retry execution strategy, dry-run support, and a structured result. Normal web startup never imports drills.

`Drill.ExternalSourceKey` is nullable so historical rows remain valid and has a filtered unique index. Canonical keys are `skillbuilderpro-900-v1:{sourceId}`. Source IDs never replace the EF primary key. Existing importer-owned rows retain `Drill.Id`; mutable mapped fields are updated only when they differ.

The initial execution attempt encountered EF's configured retry-strategy transaction guard before any write. No transaction committed. The transaction was then correctly placed inside `CreateExecutionStrategy().ExecuteAsync`, the API rebuilt, a clean dry run repeated, and only then was the successful import run.

## Files changed for this importer

- `SkillBuilderPro.API/Program.cs` — service registration and explicit command dispatch.
- `SkillBuilderPro.API/DrillImport/DrillImportModels.cs` — typed DTO/result/plan models.
- `SkillBuilderPro.API/DrillImport/DrillImportValidation.cs` — source, hash, distribution, mapping, and video-format validation.
- `SkillBuilderPro.API/DrillImport/DrillImportService.cs` — dry-run, deterministic upsert, transaction, baseline comparison, and verification counts.
- `SkillBuilderPro.API/DrillImport/DrillImportCommand.cs` — Development guard, arguments, pending-migration guard, exit codes, and JSON output.
- `SkillBuilderPro.Core/Models/Drill.cs` — nullable `ExternalSourceKey`.
- `SkillBuilderPro.Core/Data/AppDbContext.cs` — filtered unique index.
- `SkillBuilderPro.Core/Migrations/20260821101812_AddDrillExternalSourceKey.cs` and designer — additive migration.
- `SkillBuilderPro.Core/Migrations/AppDbContextModelSnapshot.cs` — model snapshot.
- `SkillBuilderPro.API/Data/DrillExcelSeeder.cs` and `SkillBuilderPro.API/seed_60_drills.sql` — legacy/do-not-run warnings only.
- `docs/operations/DRILL_IMPORT_GUIDE.md` — operator procedure.
- `docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md` — verified live data status.
- This report.

No authentication, account credential, API contract, artwork, or MAUI visual file was changed for the import task.

## Source verification

Authoritative file: `C:\Users\brovy\OneDrive\Desktop\Skill Builder Pro\drills_seed.json`

Expected and actual SHA-256: `AA46D3C4923452C8BA87F365D8672F1B9F5C2AB98EFD5595A7BC6F1E2F50D247`

Validation passed: 900 rows; six supported sports; 150 rows per sport; 180 sport/category/subcategory groups; 5 rows per group; unique source IDs, import keys, and video URLs; valid required fields, durations, and difficulty values. YouTube syntactic parsing produced zero warnings. External availability was intentionally not crawled.

## Dry run

| Result | Count |
|---|---:|
| Source | 900 |
| Valid | 900 |
| Invalid | 0 |
| Would insert | 900 |
| Would update | 0 |
| Would remain unchanged | 0 |
| Duplicate import keys | 0 |
| Video warnings | 0 |
| Database writes | 0 |

## First committed import

| Result | Count/status |
|---|---:|
| Inserted | 900 |
| Updated | 0 |
| Unchanged | 0 |
| Invalid | 0 |
| Video warnings | 0 |
| Legacy matches attached | 0 |
| Transaction committed | YES |
| Import-owned drills | 900 |
| Legacy drills | 3 |
| Total database drills | 903 |

The three existing Basketball drills did not satisfy the conservative exact match (normalized YouTube identity plus name, sport, category, and subcategory), so they remained untouched and retained their IDs and relationships.

## Distribution and API verification

Canonical database distribution is Baseball 150, Basketball 150, Football 150, Hockey 150, Soccer 150, and Softball 150. There are 180 canonical groups and zero groups whose canonical count differs from 5.

`GET http://localhost:5000/api/drills` returned HTTP 200 with 903 rows: 900 importer-owned canonical rows and 3 preserved legacy rows. Representative category/subcategory filters for every sport returned 5 rows: Baseball Fielding/Double Plays; Basketball Shooting/Form Shooting; Football Speed & Agility/Ladder Drills; Hockey Skating/Edge Work; Soccer Dribbling/First Touch; Softball Fielding/Ground Balls.

Static authenticated MAUI data-flow inspection confirms `TrainingBuilderViewModel` loads `api/drills` through `IAthleteApiService`, not `DemoDataService`; sport changes refresh categories, subcategories, and available drills without silently deleting selected session items. ADD supports multiple transient draft items. Preview passes `drillId` to the existing `DrillLibraryPage`, which resolves the real API drill and uses the existing video path. Emulator runtime verification remains separate from this build/data proof.

## Second-run idempotency

The exact same command and source were run again: inserted 0, updated 0, unchanged 900, transaction committed YES. Database count remained 903; canonical count remained 900; no duplicate row was created.

## Unrelated data preservation

| Entity | Before | After |
|---|---:|---:|
| Identity users | 18 | 18 |
| User profiles | 18 | 18 |
| Athlete goals | 4 | 4 |
| Drill assignments | 11 | 11 |
| Assignment recipients | 11 | 11 |
| Progress/completed training logs | 6 | 6 |
| Training schedules | 1 | 1 |
| Athlete progressions | 1 | 1 |
| Athlete skill progress | 1 | 1 |
| Athlete rank history | 1 | 1 |
| Athlete skill-level history | 1 | 1 |
| Athlete achievements | 3 | 3 |
| Training requests | 5 | 5 |
| Notifications | 3 | 3 |
| Notification events | 3 | 3 |

Unrelated data preserved: **YES**. The importer never deletes drills, preserves existing primary keys, and does not touch unrelated tables.

## Audit and legacy safety

The current Administrator audit model requires a real authenticated Administrator user ID. The command did not invent a system actor or poor audit schema; structured command output and application logging record the run, and a purpose-built system-operation audit identity/shape remains a follow-up.

`DrillExcelSeeder` has no startup caller, `seed_60_drills.sql` is not executed automatically, and the contaminated old JSON is unused. Both historical executable artifacts now carry explicit LEGACY / DO NOT RUN warnings. Operational usage is documented in `docs/operations/DRILL_IMPORT_GUIDE.md`.

Focused standalone tests were not added because the repository has no importer test boundary and creating a new test architecture would exceed the smallest safe implementation. The clean dry run, exact distribution proofs, rollback/non-commit attempt, first run, API query, and mandatory second run exercise the production command path directly.

## Build verification

- API Debug: **PASS** — 0 warnings, 0 errors.
- MAUI `net10.0-android` Debug: **PASS** — 40 warnings, 0 errors. Warnings are existing obsolete MAUI API/nullability diagnostics and are separate from importer correctness.
- The API process used for HTTP verification was stopped before the final API build.

## Blueprint and operations

The master blueprint now records the verified Development import, stable identity, counts, distribution, idempotency, legacy handling, and preservation result. It does not overstate emulator verification. Operator commands, failure behavior, and legacy warnings are in `docs/operations/DRILL_IMPORT_GUIDE.md`.

## SBP VISUAL QUALITY AUDIT

This is an advisory source-asset audit against the repository's locked elite brand standard. No artwork was modified. All inspected masters are 1672x941 landscape files; the current resolver/page architecture does not select dedicated phone portrait, phone landscape, tablet portrait, or tablet landscape variants. Responsive absence is therefore recorded separately from creative quality.

| Page / asset | Current file | Status | Brand quality | Responsive status | Main issue | Recommended next action |
|---|---|---|---|---|---|---|
| Choose Role / Login | `weight_room.png` | PASS WITH RESPONSIVE WORK NEEDED | Elite dark facility, controlled blue, integrated floor mark | Landscape master only | Shared art and portrait crop need UI-safe verification | Derive same-environment responsive compositions; preserve central negative space |
| Athlete Home | `home_background_approved.png` | PASS WITH RESPONSIVE WORK NEEDED | Premium multi-sport architecture and dimensional materials | Landscape master only | Floor hero mark can collide with portrait UI | Add approved responsive variants and protected-zone metadata |
| Training — Baseball | `chicago_baseball.png` | PASS WITH RESPONSIVE WORK NEEDED | Premium stadium, sharp and credible | Landscape master only | Bright sky/lights vary contrast | Responsive crop plus local UI contrast QA |
| Training — Basketball | `chicago_basketball.png` | PASS WITH RESPONSIVE WORK NEEDED | Same premium Chicago/SBP world | Landscape master only | No orientation variants | Preserve focal court and create responsive crops |
| Training — Football | `chicago_football.png` | PASS WITH RESPONSIVE WORK NEEDED | Same premium Chicago/SBP world | Landscape master only | No orientation variants | Preserve field/sky hierarchy in variants |
| Training — Hockey | `chicago_hockey.png` | PASS WITH RESPONSIVE WORK NEEDED | Same premium Chicago/SBP world | Landscape master only | Bright ice requires contrast-aware UI | Responsive crops and hockey-specific contrast QA |
| Training — Soccer | `chicago_soccer.png` | PASS WITH RESPONSIVE WORK NEEDED | Same premium Chicago/SBP world | Landscape master only | No orientation variants | Preserve field focal line in variants |
| Training — Softball | `softball_training_page.png` | PASS WITH MINOR POLISH RECOMMENDED | Sharp, expensive night field with integrated branding | Landscape master only | Night standalone field is less Chicago/architectural than other Training assets | Keep approved art; review cross-sport environmental continuity later |
| Builder — Baseball | `baseball_training.png` | PASS WITH RESPONSIVE WORK NEEDED | Clean functional premium training environment | Landscape master only | Live-control safe zones untested in portrait | Create same-field responsive variants |
| Builder — Basketball | `basketball_training.png` | PASS WITH RESPONSIVE WORK NEEDED | Premium court, restrained blue, strong open floor | Landscape master only | Floor brightness varies control contrast | Responsive variants and local backing QA |
| Builder — Football | `football_training.png` | PASS WITH RESPONSIVE WORK NEEDED | High-end indoor performance facility | Landscape master only | No orientation variants | Preserve center field as functional UI space |
| Builder — Hockey | `hockey_training.png` | PASS WITH RESPONSIVE WORK NEEDED | Sharp premium rink and realistic ice | Landscape master only | Ice is a high-luminance UI surface | Responsive variants with dark local control backing |
| Builder — Soccer | `soccer_training.png` | PASS WITH RESPONSIVE WORK NEEDED | Coherent indoor SBP facility | Landscape master only | Foreground gear constrains lower-right UI | Define protected gear zone in responsive layouts |
| Builder — Softball | `softball_training.png` | PASS WITH RESPONSIVE WORK NEEDED | Coherent functional field asset | Landscape master only | No orientation variants | Preserve infield and equipment focal points |
| Goals | `goals_background_approved.png` | PASS WITH RESPONSIVE WORK NEEDED | Premium bright multi-field campus | Landscape master only | Sky is bright behind potential title UI | Responsive crop and compact contrast backing |
| Trophy Room | `trophy_room_background_approved.png` | PASS WITH RESPONSIVE WORK NEEDED | Elite metallic trophy environment | Landscape master only | Central trophy hero needs protection | Establish hero exclusion zone in variants |
| Locker Room | `locker_room_background_approved.png` | PASS WITH RESPONSIVE WORK NEEDED | Elite black-metal locker environment | Landscape master only | Central architectural logo/door can be covered | Protect central door and floor mark |
| Notifications | `home_background_approved.png` | MISSING DEDICATED CREATIVE | Borrowed asset is high quality but not notification-specific | Landscape master only | Reuses Athlete Home environment | Create a dedicated Notifications environment in a later art task |
| Drill Library | `drill_library.png` | PASS WITH RESPONSIVE WORK NEEDED | Elite film-room architecture with intentional central live zone | Landscape master only | Portrait may crop shelving/branding | Responsive variants must retain the central board safe zone |
| Coach Home | `Backgrounds/Roles/coach_office.png` | PASS WITH RESPONSIVE WORK NEEDED | Premium office, realistic materials and integrated identity | Landscape master only | Intentionally dark; UI contrast and board protection needed | Responsive variants with board safe zone |
| Parent Home | `Backgrounds/Roles/parent_dashboard_approved.png` | PASS WITH RESPONSIVE WORK NEEDED | Polished campus overlook and restrained blue | Landscape master only | Bright sky and field detail compete with live data | Responsive crops plus compact local backing |
| Administrator Home | `Backgrounds/Roles/admin_command_center_approved.png` | PASS WITH MINOR POLISH RECOMMENDED | Visually expensive command center | Landscape master only | Background monitor bakes illustrative dashboard data behind live UI | Keep approved art; later verify that decorative screen content does not read as current app state |

Assets meeting the creative quality bar include all inspected approved role, athlete, Training, Builder, and Drill Library masters. Every current asset needs responsive/orientation work; this does not lower its art-quality classification. Minor consistency review is limited to Training Softball's different environmental language and Admin's decorative baked dashboard. Notifications is the only reviewed page missing dedicated creative. Across the six sports, Performance Blue and professional facility realism are coherent; the main consistency gap is the Softball Training hub scene, not the functional Builder set. No inspected background bakes athlete identity, rank, streak, goals, assignments, progress, or notifications into the art.

Android emulator screenshots are still required before claiming responsive or visual runtime success.
