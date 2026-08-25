# MAUI Visual Architecture Audit

**Repository:** `C:\Users\brovy\source\repos\SkillBuilderPro`  
**Audit scope:** current, non-backup MAUI page implementations under `SkillBuilderPro.MAUI/Views`, their current view models/services, shell routing, and `SkillBuilderPro.MAUI/Resources/Images`.  
**Method:** static repository inspection only. No visual-design decision is made here. “Background” below means a raster image used as the page-level visual background, not a solid `BackgroundColor`. Line references are to the audited working tree.

## Executive findings

- There are **17 current `ContentPage` classes** under `SkillBuilderPro.MAUI/Views`: 14 XAML-backed pages and 3 code-only pages.
- **11 pages have an image background**: 9 hardcoded pages/page variants and 2 dynamically bound, sport-specific pages. **6 pages have no background image**.
- Negative `ZIndex` remains on a page background image or overlay in **5 pages**: `AthleteDashboardPage`, `GoalsPage`, `NotificationsPage`, `TrainingPage`, and `TrophyRoomPage`.
- `SportVisualService` is used by **TrainingPage** (through `TrainingViewModel`) and **NotificationsPage** (through `NotificationsViewModel`). No other visual/background service was found in current MAUI XAML/C#.
- There is **one login page class**, `LoginPage`, reused for Athlete, Coach, Parent, and Administrator. Its primary button text is changed in code according to the selected role.
- Based strictly on absence from all current MAUI XAML/C# image references, **14 image assets are likely legacy/duplicate background assets**. This is a reference audit, not proof that an asset is safe to delete.

## Page-by-page audit

| Page | Exact implementation path(s) | Role/area evidence | Current background image | Classification | Negative background/overlay `ZIndex` | Exact repository evidence |
|---|---|---|---|---|---|---|
| `AthleteDashboardPage` | `SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml`; code-behind in `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs` | Athlete: `AppShell.xaml:11-12` maps route `Home` to this page; page actions include `ATHLETE PROFILE` at XAML line 28. | `home_background_approved.png` | Hardcoded; not sport-specific | **Yes**, overlay only: `BoxView ZIndex="-1"` | `AthleteDashboardPage.xaml:3`: `<Image Source="home_background_approved.png" .../>`; line 4: `<BoxView ... ZIndex="-1"/>` |
| `CategoryListPage` | `SkillBuilderPro.MAUI/Views/CategoryListPage.xaml`; `SkillBuilderPro.MAUI/Views/CategoryListPage.xaml.cs` | Athlete drill browsing route: registered by `AppShell.xaml.cs:11-13`; page title is `Select Category`. | None | Missing (solid `BackgroundColor` only) | No | `CategoryListPage.xaml:6-9`: `Title="Select Category"`, `BackgroundColor="#121212"`, then content grid; no page-level image exists in the file. |
| `ChooseProfilePage` | `SkillBuilderPro.MAUI/Views/ChooseProfilePage.cs` | Shared role selector: line 9 declares Athlete, Coach, Parent, Administrator; line 25 creates role-specific `LoginPage`. | `weight_room.png` | Hardcoded; not sport-specific | No | `ChooseProfilePage.cs:9`: role tuple list; line 12: `BackgroundImageSource="weight_room.png"`; line 25: `new LoginPage(api,role)`. |
| `DrillLibraryPage` | `SkillBuilderPro.MAUI/Views/DrillLibraryPage.xaml`; `SkillBuilderPro.MAUI/Views/DrillLibraryPage.xaml.cs` | Athlete drill/training route: `AppShell.xaml.cs:23-25` registers the route; `AthletePages.xaml.cs:24` navigates to it from Training. | `drill_library.png` | Hardcoded; not sport-specific | No (positive overlay indices only) | `DrillLibraryPage.xaml:11-14`: background canvas and `<Image Source="drill_library.png" ...>`; lines 174 and 176 use positive `ZIndex="3"` and `ZIndex="4"`. |
| `DrillListPage` | `SkillBuilderPro.MAUI/Views/DrillListPage.xaml`; `SkillBuilderPro.MAUI/Views/DrillListPage.xaml.cs` | Athlete drill browsing route: registered by `AppShell.xaml.cs:15-17`. | None | Missing (solid `BackgroundColor` only) | No | `DrillListPage.xaml:6-7`: `BackgroundColor="#121212"`; no page-level background image exists in the XAML or code-behind. |
| `GoalsPage` | `SkillBuilderPro.MAUI/Views/GoalsPage.xaml`; code-behind in `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs` | Athlete: `AppShell.xaml:16` maps the `Goals` tab to this page. | `goals_background_approved.png` | Hardcoded; not sport-specific | **Yes**, overlay only: `BoxView ZIndex="-1"` | `GoalsPage.xaml:3`: `<Image Source="goals_background_approved.png" .../><BoxView ... ZIndex="-1"/>`. |
| `LoginPage` | `SkillBuilderPro.MAUI/Views/LoginPage.xaml`; `SkillBuilderPro.MAUI/Views/LoginPage.xaml.cs` | Shared Athlete/Coach/Parent/Administrator login: `ChooseProfilePage.cs:9,25`; role-specific button mapping at `LoginPage.xaml.cs:12`. | `weight_room.png` | Hardcoded; not sport-specific | No | `LoginPage.xaml:2`: `BackgroundImageSource="weight_room.png"`; `LoginPage.xaml.cs:12` sets the role label and primary button. |
| `NotificationsPage` | `SkillBuilderPro.MAUI/Views/NotificationsPage.xaml`; code-behind in `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs`; responsive view-model partial in `SkillBuilderPro.MAUI/ViewModels/NotificationsViewModel.Responsive.cs` | Athlete: opened by `AthleteDashboardPage` (`AthletePages.xaml.cs:5`) and route registered at `AppShell.xaml.cs:26`. | Runtime result of `SportVisualService.GetTrainingBackground(...)`: one of `basketball_training.png`, `football_training.png`, `baseball_training.png`, `softball_training.png`, `soccer_training.png`, `hockey_training.png`, or fallback `strength_training.png` | Dynamically bound and sport-specific | **Yes**: background image `ZIndex="-2"`; overlay `ZIndex="-1"` | `NotificationsPage.xaml:3-4`: `<Image Source="{Binding Background}" ... ZIndex="-2"/>` and overlay. `NotificationsViewModel.Responsive.cs:7-8`: `Background => visuals.GetTrainingBackground(...)`. `AthleteViewModels.cs:43`: constructor injects `ISportVisualService visuals`. |
| `PasswordCapturePage` | `SkillBuilderPro.MAUI/Views/PasswordCapturePage.cs` | Registration credential modal, not a login page: created by `RegisterPage.cs:143`; title `Secure Your Account` at line 12. | None | Missing (solid `BackgroundColor` only) | No | `PasswordCapturePage.cs:15-17`: content `Grid` with `BackgroundColor = Color.FromArgb("#080C12")`; no `BackgroundImageSource` or background `Image`. |
| `ProfilePage` | `SkillBuilderPro.MAUI/Views/ProfilePage.xaml`; code-behind in `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs` | Athlete: `AppShell.xaml:18` maps the `Profile` tab to this page; UI labels include `ATHLETE PROFILE`/`ATHLETE BIO`. | Page background `locker_room_background_approved.png`; additionally foreground locker artwork `locker_door_dynamic_approved.png` | Hardcoded; not sport-specific | No | `ProfilePage.xaml:3`: page-level `<Image Source="locker_room_background_approved.png" .../>`; line 25: locker-door image inside `AbsoluteLayout`. |
| `RegisterPage` | `SkillBuilderPro.MAUI/Views/RegisterPage.xaml`; `SkillBuilderPro.MAUI/Views/RegisterPage.cs` | Athlete onboarding UI; reachable for Athlete or Parent because `LoginPage.xaml.cs:12,15` shows Create for those roles, but XAML explicitly says `ATHLETE ONBOARDING` and `CREATE ATHLETE PROFILE` at lines 16-17. | `create_profile.png` | Hardcoded; not sport-specific | No | `RegisterPage.xaml:7-10`: `<Image Source="create_profile.png" .../>` followed by a normal overlay `<BoxView .../>`; no negative `ZIndex`. |
| `RoleHomePage` | `SkillBuilderPro.MAUI/Views/RoleHomePage.cs` | Coach, Parent, Administrator: `ShellFactory.cs:6` sends non-Athlete roles here; role branches occur throughout `RoleHomePage.cs:28-33,62-72,147-168`. | Parent: `parent_dashboard_approved.png`; Coach: `coach_office.png`; Administrator: `admin_command_center_approved.png`; fallback: `weight_room.png` | Dynamically selected by role in code; role-specific, not sport-specific | No | `RoleHomePage.cs:28-34`: `BackgroundImageSource = role switch { ... }`. |
| `SportListPage` | `SkillBuilderPro.MAUI/Views/SportListPage.xaml`; `SkillBuilderPro.MAUI/Views/SportListPage.xaml.cs` | Athlete drill browsing page: title `Select Sport`; page exists in Athlete shell DI/routing ecosystem, although it is not a tab. | None | Missing (solid `BackgroundColor` only) | No | `SportListPage.xaml:6-9`: `Title="Select Sport"`, `BackgroundColor="#121212"`, content grid; no page-level image exists in XAML/code-behind. |
| `TrainingPage` | `SkillBuilderPro.MAUI/Views/TrainingPage.xaml`; code-behind in `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs`; VM in `SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs` | Athlete: `AppShell.xaml:14-15` maps the `Training` tab to this page; XAML line 6 says `ATHLETE TRAINING`. | Runtime result of `SportVisualService`: `basketball_training.png`, `football_training.png`, `baseball_training.png`, `softball_training.png`, `soccer_training.png`, `hockey_training.png`, or `strength_training.png` | Dynamically bound and sport-specific | **Yes**: background image `ZIndex="-2"`; overlay `ZIndex="-1"` | `TrainingPage.xaml:3`: `<Image ... Source="{Binding Background}" ... ZIndex="-2"/>` plus overlay. `AthleteViewModels.cs:25,31,33,39`: service injection, fallback property, selection update, and load update. |
| `TrainingRequestsPage` | `SkillBuilderPro.MAUI/Views/TrainingRequestsPage.xaml`; code-behind in `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs` | Athlete: opened from `TrainingPage` at `AthletePages.xaml.cs:21`; route registered at `AppShell.xaml.cs:27`. | None | Missing (resource-backed solid `BackgroundColor` only) | No | `TrainingRequestsPage.xaml:4`: `BackgroundColor="{StaticResource PageBackground}"`; no page-level image exists in XAML/code-behind. |
| `TrophyRoomPage` | `SkillBuilderPro.MAUI/Views/TrophyRoomPage.xaml`; code-behind in `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs` | Athlete: `AppShell.xaml:17` maps the `Trophy` tab to this page. | `trophy_room_background_approved.png` | Hardcoded; not sport-specific | **Yes**, overlay only: `BoxView ZIndex="-1"` | `TrophyRoomPage.xaml:2`: `<Image Source="trophy_room_background_approved.png" .../><BoxView ... ZIndex="-1"/>`. |
| `VideoPlayerPage` | `SkillBuilderPro.MAUI/Views/VideoPlayerPage.xaml`; `SkillBuilderPro.MAUI/Views/VideoPlayerPage.xaml.cs` | Athlete drill/video route: registered by `AppShell.xaml.cs:19-21`. | None | Missing (solid `BackgroundColor` only) | No | `VideoPlayerPage.xaml:6`: `BackgroundColor="#080B14"`; no page-level background image exists in XAML/code-behind. |

## Background classification summary

### Hardcoded image backgrounds

- `AthleteDashboardPage` — `home_background_approved.png` (`AthleteDashboardPage.xaml:3`)
- `ChooseProfilePage` — `weight_room.png` (`ChooseProfilePage.cs:12`)
- `DrillLibraryPage` — `drill_library.png` (`DrillLibraryPage.xaml:12`)
- `GoalsPage` — `goals_background_approved.png` (`GoalsPage.xaml:3`)
- `LoginPage` — `weight_room.png` (`LoginPage.xaml:2`)
- `ProfilePage` — `locker_room_background_approved.png` (`ProfilePage.xaml:3`)
- `RegisterPage` — `create_profile.png` (`RegisterPage.xaml:7`)
- `TrophyRoomPage` — `trophy_room_background_approved.png` (`TrophyRoomPage.xaml:2`)
- `RoleHomePage` assigns hardcoded filenames through a runtime role switch: Parent `parent_dashboard_approved.png`, Coach `coach_office.png`, Administrator `admin_command_center_approved.png`, fallback `weight_room.png` (`RoleHomePage.cs:28-34`). It is dynamic by role, but each mapping is a literal filename.

### Dynamically bound, sport-specific image backgrounds

The service map is exact at `SkillBuilderPro.MAUI/Services/SportVisualService.cs:11-20`:

| Sport key | Exact filename |
|---|---|
| Basketball | `basketball_training.png` |
| Football | `football_training.png` |
| Baseball | `baseball_training.png` |
| Softball | `softball_training.png` |
| Soccer | `soccer_training.png` |
| Hockey | `hockey_training.png` |
| Strength | `strength_training.png` |

`SportVisualService.cs:23-24` returns `strength_training.png` when the sport is null or unmapped.

- `TrainingPage`: XAML binds `Image.Source` to `Background` (`TrainingPage.xaml:3`). `TrainingViewModel` injects `ISportVisualService` (`AthleteViewModels.cs:25`), initializes `Background` to `strength_training.png` (line 31), and updates it through `GetTrainingBackground` (lines 33 and 39).
- `NotificationsPage`: XAML binds `Image.Source` to `Background` (`NotificationsPage.xaml:3`). `NotificationsViewModel` injects `ISportVisualService` (`AthleteViewModels.cs:43`), and `NotificationsViewModel.Responsive.cs:7-8` resolves its background from demo/user sport.

### Pages with no background image

These pages have solid/resource background colors but no page-level background image in current XAML/C#:

1. `CategoryListPage`
2. `DrillListPage`
3. `PasswordCapturePage`
4. `SportListPage`
5. `TrainingRequestsPage`
6. `VideoPlayerPage`

## Negative `ZIndex` inventory

Every current negative `ZIndex` under MAUI XAML/C# is listed below; no code-set negative `ZIndex` was found.

| Page | Element | Exact evidence |
|---|---|---|
| `AthleteDashboardPage` | Overlay `BoxView` | `SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml:4`: `ZIndex="-1"` |
| `GoalsPage` | Overlay `BoxView` | `SkillBuilderPro.MAUI/Views/GoalsPage.xaml:3`: `ZIndex="-1"` |
| `NotificationsPage` | Background `Image` | `SkillBuilderPro.MAUI/Views/NotificationsPage.xaml:3`: `ZIndex="-2"` |
| `NotificationsPage` | Overlay `BoxView` | `SkillBuilderPro.MAUI/Views/NotificationsPage.xaml:4`: `ZIndex="-1"` |
| `TrainingPage` | Background `Image` | `SkillBuilderPro.MAUI/Views/TrainingPage.xaml:3`: `ZIndex="-2"` |
| `TrainingPage` | Overlay `BoxView` | `SkillBuilderPro.MAUI/Views/TrainingPage.xaml:3`: `ZIndex="-1"` |
| `TrophyRoomPage` | Overlay `BoxView` | `SkillBuilderPro.MAUI/Views/TrophyRoomPage.xaml:2`: `ZIndex="-1"` |

## Login-page inventory

Only **one actual login page class** exists: `SkillBuilderPro.MAUI/Views/LoginPage.xaml` + `LoginPage.xaml.cs`. `ChooseProfilePage` launches that same class with the selected role (`ChooseProfilePage.cs:25`). `PasswordCapturePage` is a registration password modal, not login (`RegisterPage.cs:143`; `PasswordCapturePage.cs:12,34`).

| Login variant (same page class) | Email placeholder | Password placeholder | Primary button text | Evidence |
|---|---|---|---|---|
| Athlete | `Email` | `Password` | `ENTER LOCKER ROOM` | Placeholders: `LoginPage.xaml:3`; runtime mapping: `LoginPage.xaml.cs:12` |
| Coach | `Email` | `Password` | `ENTER COACH'S OFFICE` | Same evidence |
| Parent | `Email` | `Password` | `ENTER PARENT DASHBOARD` | Same evidence |
| Administrator | `Email` | `Password` | `ENTER COMMAND CENTER` | Same evidence |
| Fallback/unknown role | `Email` | `Password` | `SIGN IN` | XAML default and switch fallback at `LoginPage.xaml:3`; `LoginPage.xaml.cs:12` |

The XAML initially declares `Text="SIGN IN"`, but the constructor overwrites it for all four selectable roles. Secondary buttons are `CREATE PROFILE` and `DEMO AS ATHLETE` (`LoginPage.xaml:3`); code shows Create only for Athlete/Parent and Demo only for Athlete (`LoginPage.xaml.cs:12`).

## Role/page correspondence

| Role | Current pages/routes | Exact evidence |
|---|---|---|
| Athlete | `ChooseProfilePage` → shared `LoginPage` → `AppShell`; shell tabs are `AthleteDashboardPage`, `TrainingPage`, `GoalsPage`, `TrophyRoomPage`, `ProfilePage`; routed supporting pages are `CategoryListPage`, `DrillListPage`, `VideoPlayerPage`, `DrillLibraryPage`, `NotificationsPage`, and `TrainingRequestsPage`; `RegisterPage` and `PasswordCapturePage` implement onboarding. | `ChooseProfilePage.cs:9,25-26`; `ShellFactory.cs:6`; `AppShell.xaml:9-20`; `AppShell.xaml.cs:11-27`; `LoginPage.xaml.cs:15`; `RegisterPage.cs:143`. |
| Coach | `ChooseProfilePage` → shared `LoginPage` → `RoleHomePage(api,"Coach")`. | `ChooseProfilePage.cs:9,25`; `LoginPage.xaml.cs:12`; `ShellFactory.cs:6`; `RoleHomePage.cs:31,67-72,155-162`. |
| Parent | `ChooseProfilePage` → shared `LoginPage` → `RoleHomePage(api,"Parent")`; registration is also exposed from Parent login but its present XAML is athlete-profile onboarding. | `ChooseProfilePage.cs:9,25`; `LoginPage.xaml.cs:12,15`; `ShellFactory.cs:6`; `RoleHomePage.cs:30,67-72,147-154`; `RegisterPage.xaml:16-17`. |
| Administrator | `ChooseProfilePage` → shared `LoginPage` → `RoleHomePage(api,"Administrator")`, whose administrator module grid is built in code. | `ChooseProfilePage.cs:9,25`; `LoginPage.xaml.cs:12`; `ShellFactory.cs:6`; `RoleHomePage.cs:9-13,32,39-41,62,90-109,163-168`. |

## Visual/background service usage

- `SkillBuilderPro.MAUI/Services/SportVisualService.cs` is the only current service whose contract directly returns visual/background assets (`ISportVisualService.GetTrainingBackground`, lines 3-6).
- It is registered in DI at `SkillBuilderPro.MAUI/MauiProgram.cs` (the `ISportVisualService`/`SportVisualService` registration).
- Page consumers are:
  - `TrainingPage`, indirectly through `TrainingViewModel` (`AthleteViewModels.cs:25,33,39`; `TrainingPage.xaml:3`).
  - `NotificationsPage`, indirectly through `NotificationsViewModel` (`AthleteViewModels.cs:43`; `NotificationsViewModel.Responsive.cs:7-8`; `NotificationsPage.xaml:3`).
- `RoleHomePage` performs its own background selection with a local role switch (`RoleHomePage.cs:28-34`); it does **not** use a visual service.
- No other class named or functioning as a visual/background service was found in current MAUI XAML/C#.

## Likely legacy or duplicate image assets

The following files exist under `SkillBuilderPro.MAUI/Resources/Images` but their basenames are not referenced by any current MAUI `.xaml` or `.cs` file. They are therefore **likely** legacy/duplicate assets. Backup source files were excluded from “current implementation”; this audit does not establish that deletion is safe.

| Exact asset path | Repository-evidence basis for likely status |
|---|---|
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/dotnet_bot.png` | No current MAUI XAML/C# reference. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/goals_progress_approved.png` | No current reference; current Goals page references `goals_background_approved.png` at `GoalsPage.xaml:3`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/home_training_facility_maui.png` | No current reference; current Home references `home_background_approved.png` at `AthleteDashboardPage.xaml:3`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/home_training_facility_winforms.png` | No current MAUI XAML/C# reference; filename itself identifies the WinForms variant. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/trophy_room_approved.png` | No current reference; current Trophy page references `trophy_room_background_approved.png` at `TrophyRoomPage.xaml:2`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/Roles/admin_dashboard.png` | No current reference; Administrator maps to `admin_command_center_approved.png` at `RoleHomePage.cs:32`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/Roles/locker_room.png` | No current reference; Profile uses `locker_room_background_approved.png`, while login/selector/fallback use `weight_room.png`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/Roles/parent_dashboard.png` | No current reference; Parent maps to `parent_dashboard_approved.png` at `RoleHomePage.cs:30`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/Sports/calendar_baseball.png` | No current MAUI XAML/C# reference; `SportVisualService` maps Baseball to `baseball_training.png`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/Sports/calendar_basketball.png` | No current reference; service maps Basketball to `basketball_training.png`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/Sports/calendar_football.png` | No current reference; service maps Football to `football_training.png`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/Sports/calendar_hockey.png` | No current reference; service maps Hockey to `hockey_training.png`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/Sports/calendar_soccer.png` | No current reference; service maps Soccer to `soccer_training.png`. |
| `SkillBuilderPro.MAUI/Resources/Images/Backgrounds/Sports/calendar_softball.png` | No current reference; service maps Softball to `softball_training.png`. |

The two remaining potentially confusing role assets are current: `weight_room.png` is referenced by `ChooseProfilePage`, `LoginPage`, and the `RoleHomePage` fallback; `coach_office.png` is referenced by the Coach mapping. `locker_door_dynamic_approved.png` is also current but is foreground locker artwork rather than a page background (`ProfilePage.xaml:25`).

## Prioritized implementation list (evidence-derived; no visual choices)

This list identifies implementation work in priority order without selecting replacement artwork, colors, composition, or visual direction.

1. **Normalize negative layer ordering on the five evidenced pages.** Audit/replace negative `ZIndex` usage on `AthleteDashboardPage`, `GoalsPage`, `NotificationsPage`, `TrainingPage`, and `TrophyRoomPage`, preserving the existing image → overlay → content order. The exact seven negative declarations are inventoried above.
2. **Decide requirements for the six objectively missing image backgrounds before implementation.** The affected pages are `CategoryListPage`, `DrillListPage`, `PasswordCapturePage`, `SportListPage`, `TrainingRequestsPage`, and `VideoPlayerPage`. Repository evidence establishes absence only; it does not establish that each page should receive artwork.
3. **Centralize or formally document background resolution boundaries.** Sport-dependent resolution is centralized in `SportVisualService`, while role-dependent resolution remains inline in `RoleHomePage.cs:28-34` and other page backgrounds are literal XAML/code values. Any consolidation should preserve the current exact mappings unless a separate design/product decision changes them.
4. **Resolve the Parent registration-flow mismatch at the product/requirements level.** Parent login exposes `CREATE PROFILE` (`LoginPage.xaml.cs:12,15`), but the reached page explicitly presents `ATHLETE ONBOARDING` / `CREATE ATHLETE PROFILE` (`RegisterPage.xaml:16-17`). This is an implementation-flow fact, not a recommendation for a visual outcome.
5. **Validate the 14 likely-unreferenced assets against non-code consumers/build/package history before cleanup.** The table above is based on current XAML/C# references only. Do not delete solely from this audit; confirm there are no documentation, future feature, external content, or runtime string consumers.
6. **Add an automated reference check after ownership decisions are made.** Compare flattened MAUI image basenames against current XAML/C# literal/service mappings so future duplicates and unreferenced variants are reported consistently.

## Audit boundaries

- Files named `DrillLibraryPage.xaml.cs.BACKUP*` are repository backups, not compiled/current `.cs` implementations, and were excluded from page/service conclusions.
- Solid colors, card images, icons (`eye.svg`, `eye_off.svg`), uploaded profile photos, and the locker-door foreground artwork are not classified as page backgrounds.
- “Unreferenced” means no basename reference in current MAUI XAML/C# at audit time. Reflection, remote configuration, documentation, or future planned use cannot be disproved by static inspection.
- No files or assets were deleted or modified as part of the audit; this report is the only created file.
