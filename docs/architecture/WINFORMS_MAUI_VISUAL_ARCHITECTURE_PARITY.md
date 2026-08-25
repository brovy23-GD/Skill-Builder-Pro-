# WinForms + MAUI Visual Architecture Parity

## 1. Purpose and result

This audit compares the active MAUI and WinForms client experiences against the Master and Image System blueprints, then standardizes only the clearly conflicting WinForms desktop-background behavior. The result is **FAIL** for complete product parity: shared desktop asset resolution and non-stretched rendering are now established, but several equivalent pages are absent, placeholders, or materially different. No backend, database, authentication architecture, or approved image pixels were changed.

## 2. Current-state findings

- MAUI has dedicated Athlete pages and a responsive `ISportVisualService`; Coach, Parent, and Administrator use one generic `RoleHomePage`, with Administrator module buttons explicitly reporting that dedicated workspaces are not implemented.
- WinForms has richer Coach, Parent, and Administrator forms, but its Athlete `MainForm` combines Training and Training Builder concepts and lacks dedicated Trophy Room and Notifications pages.
- WinForms image selection was split among embedded resources, direct file loading, and local switch statements. Several forms used `Stretch`.
- Choose Experience already had correct live title, subtitle, four authenticated roles, and secondary Demo Mode. Internal legacy role value `Admin` remains unchanged to avoid an authentication refactor; visible copy is `ADMINISTRATOR`.
- All audited approved landscape desktop files are 1672×941. `locker_door_dynamic_approved.png` is the intentional 1145×1374 portrait exception.

## 3. Complete page inventory and parity matrix

`NOT IMPLEMENTED` and `LEGACY` are explicit inventory statuses; for the summary totals they count as FAIL.

| Page | MAUI page | WinForms equivalent | Exists | Purpose | Background | UI hierarchy | Navigation | Status |
|---|---|---|---|---|---|---|---|---|
| Choose Experience | `ChooseProfilePage` | `RoleSelectForm` | Both | Match | Match | Match | Match | PASS |
| Login | `LoginPage` | `LoginForm` | Both | Match | Partial | Partial | Match | PARTIAL |
| Athlete Home | `AthleteDashboardPage` | `HomePageControl` | Both | Partial | Match | Partial | Partial | PARTIAL |
| Training | `TrainingPage` | No dedicated page | MAUI only | No | N/A | No | No | FAIL |
| Training Builder | `TrainingBuilderPage` | `MainForm` Training tab | Both | Partial | Match | Partial | Partial | PARTIAL |
| Drill Library | `DrillLibraryPage` | Admin drill view / training workflow | Partial | Partial | Partial | Partial | Partial | PARTIAL |
| Film Room | `VideoPlayerPage` | `VideoPlayerForm` | Both | Partial | Partial | Partial | Partial | PARTIAL |
| Goals / Progress | `GoalsPage` | `GoalsPageControl` | Both | Match | Match | Partial | Partial | PARTIAL |
| Trophy Room | `TrophyRoomPage` | None | MAUI only | No | N/A | No | No | FAIL |
| Locker Room / Profile | `ProfilePage` | `LockerRoomForm` | Both | Match | Match | Partial | Partial | PARTIAL |
| Calendar | No dedicated MAUI page | `MainForm` Calendar tab | WinForms only | Partial | WinForms only | Partial | Partial | PARTIAL |
| Notifications | `NotificationsPage` | None | MAUI only | No | N/A | No | No | FAIL |
| Coach Home | generic `RoleHomePage` | `CoachDashboard` | Both | Partial | Same family | Different | Partial | PARTIAL |
| Coach Athlete Detail | None | Roster rows only | Neither dedicated | No | N/A | No | No | FAIL |
| Coach Training Builder | None | None | No | No | N/A | No | No | NOT IMPLEMENTED |
| Parent Home | generic `RoleHomePage` | `ParentDashboard` | Both | Partial | Same family | Different | Partial | PARTIAL |
| Admin Home | generic `RoleHomePage` | `AdminDashboardForm` | Both | Match | Same family | Partial | Partial | PARTIAL |
| Admin Users | placeholder module | `AdminDashboardForm` athlete/users surface | Partial | Partial | Partial | Partial | Partial | PARTIAL |
| Admin Drill Management | placeholder module | `AdminDashboardForm` drill surface | Partial | Partial | Partial | Partial | Partial | PARTIAL |
| Admin Audit | placeholder module | None | No | No | N/A | No | No | NOT IMPLEMENTED |

Totals: 20 audited; PASS 1; PARTIAL 13; FAIL/NOT IMPLEMENTED 6.

## 4. Background audit and verification matrix

| Page/family | Canonical desktop asset | MAUI runtime | WinForms runtime | Dimensions | Aspect preserved | Legacy removed | Result |
|---|---|---|---|---:|---|---|---|
| Choose Experience | `choose_role_desktop.png` | Same | Same via resolver | 1672×941 | Yes | N/A | PASS |
| Athlete Home | `home_athlete_desktop.png` | Same | Same via resolver | 1672×941 | Yes, aspect-fill helper | N/A | PASS |
| Training Basketball | `training_basketball_chicago_desktop.png` | Same | Packaged/resolver-ready; no dedicated page | 1672×941 | Yes | No | FAIL |
| Training Football | `training_football_chicago_desktop.png` | Same | Packaged/resolver-ready; no dedicated page | 1672×941 | Yes | No | FAIL |
| Training Baseball | `training_baseball_chicago_desktop.png` | Same | Packaged/resolver-ready; no dedicated page | 1672×941 | Yes | No | FAIL |
| Training Softball | `training_softball_chicago_desktop.png` | Same | Packaged/resolver-ready; no dedicated page | 1672×941 | Yes | No | FAIL |
| Training Soccer | `training_soccer_chicago_desktop.png` | Same | Packaged/resolver-ready; no dedicated page | 1672×941 | Yes | No | FAIL |
| Training Hockey | `training_hockey_chicago_desktop.png` | Same | Packaged/resolver-ready; no dedicated page | 1672×941 | Yes | No | FAIL |
| Training Builder, all six sports | `training_builder_<sport>_desktop.png` | Same family | Same via resolver | 1672×941 | Yes, Zoom | N/A | PASS |
| Goals | `goals_background_approved.png` | Same | Same via resolver | 1672×941 | Yes, Zoom | N/A | PASS |
| Trophy Room | `trophy_room_background_approved.png` | Same | Packaged but page absent | 1672×941 | N/A | No | FAIL |
| Locker Room | `locker_room_background_approved.png` | Same | Same via resolver | 1672×941 | Yes, Zoom | N/A | PASS |
| Coach | `coach_office.png` / `CoachOffice.png` | Current family | Embedded family via resolver | 1672×941 | Yes, Zoom | No | PARTIAL |
| Parent | `parent_dashboard_approved.png` / `parentsbackground.png` | Current family | Embedded family via resolver | 1672×941 | Yes, Zoom | No | PARTIAL |
| Administrator | `admin_command_center_approved.png` / `AdminDashApproved.png` | Current family | Embedded family via resolver | 1672×941 | Yes, Zoom | No | PARTIAL |

The six Chicago and six Builder WinForms desktop files are copied to runtime output. No approved artwork was regenerated. No legacy art was archived: repository-reference validation found live references, so retirement would be unsafe.

## 5. Canonical desktop creative and resolver

`IDesktopVisualResolver` / `DesktopVisualResolver` now centralizes Choose Experience, Login, Athlete Home, Training, Training Builder, Goals, Trophy, Locker, Coach, Parent, and Administrator desktop selection. Training and Builder have separate methods and mappings. Exact runtime files are preferred, with an explicit same-page/family fallback. `BackgroundRenderHelper` remains the reusable proportional aspect-fill renderer; standard form/tab backgrounds now use `Zoom` rather than `Stretch`.

## 6. Page-by-page changes

- Choose Experience and Athlete Home now obtain their approved art from the resolver.
- The six Chicago Training and six Builder desktop families are explicit WinForms runtime content.
- The Training Builder tab resolves only Builder art. Calendar again resolves its calendar resource family rather than Builder art.
- Goals and Locker landing resolve their approved dedicated art.
- Coach, Parent, Administrator, Login, calendar, admin sub-surfaces, and locker door no longer use stretching.
- Login visible CTA copy is aligned to `ENTER PARENT HUB` and `ENTER ADMIN CENTER`. Legacy internal role key `Admin` is intentionally preserved.

## 7. Navigation parity

| Action | MAUI destination | WinForms destination | Equivalent | Result |
|---|---|---|---|---|
| Choose authenticated role | Role-aware Login | Role-aware Login | Yes | PASS |
| Demo Mode | Athlete demo Shell | Athlete demo MainForm | Yes | PASS |
| Login | Athlete Shell or generic role home | Role dashboard | Partial | PARTIAL |
| Athlete Home → Training | Dedicated Training | Combined Builder tab | No | FAIL |
| Training → Builder | Dedicated Builder route | Already combined | No | FAIL |
| Drill Library | Dedicated route | Partial/admin or training workflow | No | FAIL |
| Goals | Goals page | Goals tab | Yes | PASS |
| Trophy Room | Trophy page | Missing | No | FAIL |
| Locker Room | Profile route | Locker form | Yes | PASS |
| Coach | Generic role home | Coach dashboard | Partial | PARTIAL |
| Parent | Generic role home | Parent dashboard | Partial | PARTIAL |
| Admin | Generic command center placeholders | Admin dashboard | Partial | PARTIAL |
| Demo exit | Choose Experience | Close dashboard to role selector | Yes | PASS |

Overall navigation parity: **FAIL**.

## 8. Resize, DPI, safe zones, and visual review

- Forms retain `AutoScaleMode.Font`, maximized startup, and minimum desktop sizes; live overlays use Dock/Anchor/resize handlers on the audited major forms.
- Proportional image rendering prevents distortion at 1672×941, 1920×1080, 1440×900, and 1366×768. Exact interactive verification at every size and 100/125/150% DPI remains manual because no WinForms screenshot automation suite exists.
- Safe zones remain page-specific. The patch does not move approved focal content or redesign overlays.
- Full visual PASS requires manual screenshots at the four target resolutions and three DPI settings after missing-page architecture is implemented.

## 9. Build results and warnings

- WinForms: **PASS** using isolated output (`UseAppHost=false`, 148 warnings, 0 errors). The normal output path was locked by a running `SkillBuilderPro.exe`; the process was deliberately not terminated. Warnings are pre-existing nullable-reference issues plus the `WindowsBase` version conflict from WebView2.
- MAUI Windows: **PASS** (109 warnings, 0 errors). Warnings are existing obsolete API/nullability diagnostics and MVVM Toolkit WinRT AOT advisories.
- No warning was treated as parity scope or changed opportunistically.

## 10. Unresolved gaps and final status

Highest-priority gaps are: a dedicated WinForms Training page using Chicago art; WinForms Trophy Room and Notifications; direct Athlete Home navigation parity; true Coach Athlete Detail/Builder; Admin Audit; and replacing the generic MAUI role placeholder with equivalent dedicated role workspaces. These are product features, not safe incidental visual edits.

Legacy assets still referenced include the embedded Chicago names, `weight_room`, role backgrounds, and admin/drill surfaces. Archive only after their consumers have migrated and a second reference check is clean.

Final result: **FAIL** for complete parity; **PASS** for the targeted shared desktop resolver, approved family packaging, copy correction, and stretch elimination performed here.
