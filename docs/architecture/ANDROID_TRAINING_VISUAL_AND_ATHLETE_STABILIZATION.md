# Android Training Visual and Athlete Stabilization

Date: 2026-08-20  
Scope: MAUI Athlete runtime/authentication/training/builder/drill/logout/portrait stabilization. No commit was created.

## Result

The implementation separates Training Page and Training Builder visual responsibilities, preserves authenticated real-drill behavior, makes Builder background assignment defensive across the Android handler lifecycle, keeps Login paste controls permanent, exposes authenticated SIGN OUT, and applies phone-first local contrast/layout repairs to Athlete Home, Goals, and Trophy Room.

Android build success verifies compilation and MAUI resource processing only. Emulator verification remains required and no visual/runtime success is claimed here.

## Locked visual architecture

- **Training Page** is the cinematic Chicago-rooted athlete command center and calls `ISportVisualService.GetTrainingPageBackground(sport)`.
- **Training Builder** is the functional workout construction environment and calls `ISportVisualService.GetTrainingBuilderBackground(sport)`.
- The old ambiguous `GetTrainingBackground` method has no remaining callers and was removed.
- Both sets are MAUI bundled images, selected by logical lowercase filename. No absolute/file URI or WinForms runtime path is used.

### Matrix A — Training Page

| Sport | Repository asset | Dimensions | MAUI source path | Chicago environment? |
|---|---|---:|---|---|
| Basketball | `chicago_basketball.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/chicago_basketball.png` | Yes |
| Football | `chicago_football.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/chicago_football.png` | Yes |
| Baseball | `chicago_baseball.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/chicago_baseball.png` | Yes |
| Softball | `softball_training_page.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/softball_training_page.png` | No; approved softball-field fallback |
| Soccer | `chicago_soccer.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/chicago_soccer.png` | Yes |
| Hockey | `chicago_hockey.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/chicago_hockey.png` | Yes |

The five Chicago files and the Softball fallback are byte-identical copies of the existing approved WinForms source assets. SHA-256 comparisons matched. A distinct Chicago Softball environment does not exist in the audited repository; that creative gap is documented rather than silently mapping Softball to Baseball or Builder art.

### Matrix B — Training Builder

| Sport | Repository asset | Dimensions | Source path | Exists |
|---|---|---:|---|---|
| Basketball | `basketball_training.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/basketball_training.png` | Yes |
| Football | `football_training.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/football_training.png` | Yes |
| Baseball | `baseball_training.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/baseball_training.png` | Yes |
| Softball | `softball_training.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/softball_training.png` | Yes |
| Soccer | `soccer_training.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/soccer_training.png` | Yes |
| Hockey | `hockey_training.png` | 1672×941 | `SkillBuilderPro.MAUI/Resources/Images/hockey_training.png` | Yes |

`SkillBuilderPro.MAUI.csproj` recursively includes `Resources\Images\**\*` as `MauiImage`, so all assets pass through the MAUI Android image pipeline. No duplicate logical lowercase filenames were found.

## Resolver callers

`GetTrainingPageBackground` callers:

- `SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs` — `TrainingViewModel.OnSelectedSportChanged`
- `SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs` — `TrainingViewModel.Load`

`GetTrainingBuilderBackground` callers:

- `SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs` — `SetRequestedSport`
- `SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs` — `OnSelectedSportChanged`
- `SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs` — `Load`

## Training and Builder precedence

Training Page:

1. query-requested/explicitly selected sport
2. current Training selection
3. authenticated Athlete `CurrentUser.Sport`
4. Demo sport
5. first supported fallback

Training Builder:

1. sport explicitly passed by Training
2. current Builder selection
3. authenticated Athlete `CurrentUser.Sport`
4. Demo sport (`DemoDataService.Sport`; Aubrey resolves to Softball)
5. first supported fallback

Changing Builder sport updates Builder artwork, clears stale category/skill filter selections, and refreshes categories/skills/available drills. It does not clear `TrainingSessionDraft.Items`.

## Builder blank-image investigation and repair

Verified static facts:

- Before load, `Background` is now the valid bundled fallback `basketball_training.png`.
- A query is applied through `TrainingBuilderPage.IQueryAttributable.ApplyQueryAttributes`; it calls `SetRequestedSport`, which immediately resolves the Builder asset and selection.
- `AppShell` registers `TrainingBuilderPage` as a pushed route and DI creates the page with its `TrainingBuilderViewModel`; no second manually constructed VM was found.
- `BuilderBackground` exists after `InitializeComponent`.
- All exact filenames exist, use lowercase Android-safe names, and are included as `MauiImage`.
- The background Image is first in the root Grid, the translucent overlay is second, and the transparent ScrollView/live content is third.
- Neither background nor overlay has negative `ZIndex`; overlay alpha is `0x42` and does not make the image opaque-black.
- The image uses `AspectFill`, Fill/Fill, and `InputTransparent=True`.

The prior repair assigned the source after BindingContext and on `Background.PropertyChanged`, but the verified emulator still rendered blank. Repository inspection cannot prove the internal Android renderer timing without an emulator trace; the remaining concrete lifecycle weakness was that source application could occur before a native handler existed and was not guaranteed again after Loaded/HandlerChanged/OnAppearing.

The robust implementation now reapplies the exact service-resolved logical filename:

1. immediately after `InitializeComponent` and BindingContext assignment
2. whenever ViewModel `Background` raises `PropertyChanged`
3. on page `Loaded`
4. on Image `HandlerChanged`
5. at the start of `OnAppearing`
6. once more through the page Dispatcher after appearing/load begins

DEBUG-only logging records the applied filename, handler readiness, and image layout dimensions. Production UI remains clean. This is the smallest repository-supported defense against the identified lifecycle gap; emulator evidence is still required to confirm it resolves the native rendering failure.

## Authenticated real-drill flow

- Endpoint: `GET /api/drills`.
- Live development observation: HTTP 200 with three records, all Basketball / Offense / Shooting, Dribbling, or Passing.
- `AthleteApiService.SetToken()` sets `HttpClient.DefaultRequestHeaders.Authorization` to Bearer after login, so authenticated Builder requests carry JWT state.
- `DrillsController.GetDrills` allows read access. POST/PUT/DELETE remain Coach/Administrator-only; authorization was not weakened.
- Authenticated Builder never substitutes `DemoDataService` content.
- All real API drills remain ADD-able even without a valid video. WATCH alone requires `YouTubeUrl.IsValid`.
- Required chain is Sport → Category → Skill → Available Drills → select → WATCH/ADD → YOUR SESSION.
- Incomplete filters show `SELECT A SPORT, CATEGORY, AND SKILL TO FIND DRILLS`.
- A complete zero-result selection shows `NO DRILLS FOUND FOR THIS SELECTION`.
- API null/failure shows the existing service message or `Drills could not be loaded from the service. Try again.` with RETRY.
- ADD creates a draft item from the real `Drill`/integer ID. Multiple items, reps, duration, order, remove, and reorder remain transient and functional. Filter changes do not destroy the draft.
- The external 900-drill dataset remains not proven live and was not imported or claimed.

## Login and authentication

Portrait Login grids:

| Row | Columns | Controls |
|---|---|---|
| Email | `*,58` | column 0 editable `EmailEntry`; column 1 permanent `PASTE` |
| Password | `*,58,44` | column 0 masked `PasswordEntry`; column 1 permanent `PASTE`; column 2 independent eye button |

Both paste actions call `Clipboard.Default.GetTextAsync()`. Password paste restores masking. Empty/unavailable clipboard failures appear inline. The form remains in a ScrollView. Exact CTAs remain Athlete `ENTER LOCKER ROOM`, Coach `ENTER COACH'S OFFICE`, Parent `ENTER PARENT HUB`, Administrator `ENTER ADMIN CENTER`.

Authentication evidence:

- Android Debug default endpoint: `http://10.0.2.2:5000/` (subject to the existing explicit preference override).
- role selector calls `SelectRole` and creates shared `LoginPage(api, role)`.
- Login calls `LoginAsync(email,password,selectedRole)`.
- successful Athlete routes to `AppShell`; Coach/Parent/Administrator retain their existing role experience.
- errors remain visible inline.
- successful login sets token, `CurrentUser`, selected role, and `IsDemoMode=false`.
- no password, credential, token, or JWT signing key was added to source.

## Authenticated SIGN OUT

Profile/Locker Room now exposes the existing `ExitLabel`/`LogoutCommand` as a top-level accessible control, in addition to the open profile panel. For authenticated users the label is `SIGN OUT`; Demo remains separately labeled `EXIT DEMO MODE`.

`AthleteApiService.LogoutAsync()` clears `CurrentUser`, Demo state, selected role, the HttpClient Authorization header, and secure token/expiry keys. `ProfileViewModel.Logout` then replaces the root with `ChooseProfilePage`. It does not terminate the application and another role can immediately be selected.

## Portrait visual composition

Modified pages:

- **Athlete Home** — local smoky hero backing, compact notification action, phone-first padding/spacing, explicit 2×2 metrics, dedicated Today's Training surface, primary area separation, ScrollView retained.
- **Goals** — local smoky header backing, phone-friendly Back/Exit row, explicit 2×2 metrics, stronger active-goal surfaces, completed goals remain below in the ScrollView.
- **Trophy Room** — page-specific `Shell.NavBarIsVisible=False` removes the native white Trophy bar; custom header/back/exit uses local backing; phone summary stacks rank → hero art space → achievements; milestones stack vertically on phone; ScrollView/spacing retained.
- **Training** — Chicago art binding, normal Grid stacking, smoky local header, portrait-safe full-width sport picker.
- **Training Builder** — normal stacking and local functional surfaces.
- **Notifications** — retains neutral `home_background_approved.png`; normal image/overlay/content ordering; no Training or Builder art.
- **Profile/Locker Room** — obvious top-level SIGN OUT/EXIT DEMO control.

The current 1672×941 landscape assets are not stretched or rewritten on disk. AspectFill is a temporary responsive crop. Dedicated intentional portrait masters remain future work under the blueprint's 1080×1920 target.

## Exact files changed by this stabilization scope

1. `SkillBuilderPro.MAUI/Services/SportVisualService.cs`
2. `SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs`
3. `SkillBuilderPro.MAUI/ViewModels/NotificationsViewModel.Responsive.cs`
4. `SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs`
5. `SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml`
6. `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs`
7. `SkillBuilderPro.MAUI/Views/GoalsPage.xaml`
8. `SkillBuilderPro.MAUI/Views/LoginPage.xaml`
9. `SkillBuilderPro.MAUI/Views/LoginPage.xaml.cs`
10. `SkillBuilderPro.MAUI/Views/NotificationsPage.xaml`
11. `SkillBuilderPro.MAUI/Views/ProfilePage.xaml`
12. `SkillBuilderPro.MAUI/Views/TrainingPage.xaml`
13. `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml`
14. `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml.cs`
15. `SkillBuilderPro.MAUI/Views/TrophyRoomPage.xaml`
16. `SkillBuilderPro.MAUI/Resources/Styles/Styles.xaml`
17. `SkillBuilderPro.MAUI/Resources/Images/chicago_baseball.png`
18. `SkillBuilderPro.MAUI/Resources/Images/chicago_basketball.png`
19. `SkillBuilderPro.MAUI/Resources/Images/chicago_football.png`
20. `SkillBuilderPro.MAUI/Resources/Images/chicago_hockey.png`
21. `SkillBuilderPro.MAUI/Resources/Images/chicago_soccer.png`
22. `SkillBuilderPro.MAUI/Resources/Images/softball_training_page.png`
23. `docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md`
24. `docs/architecture/ANDROID_TRAINING_VISUAL_AND_ATHLETE_STABILIZATION.md`

Existing earlier uncommitted Training split wiring in `AppShell.xaml.cs`, `MauiProgram.cs`, and related Phase 1 files was preserved but is not falsely attributed as newly authored by this stabilization scope.

Shared named styles already adjusted in this stabilization chain: `GlassPanelStyle`, `GlassCardStyle`, `GlassHeaderStyle`, `EliteSurfaceStyle`, `PageEyebrowStyle`, `PageSubtitleStyle`, and `SecondaryTextStyle`. Page-specific backplates/layouts—not another blanket global text-color change—perform the new portrait readability repair.

## Exact diff verification

Tracked changes are inspectable exactly with:

```text
git diff -- SkillBuilderPro.MAUI/Resources/Styles/Styles.xaml SkillBuilderPro.MAUI/Services/SportVisualService.cs SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs SkillBuilderPro.MAUI/ViewModels/NotificationsViewModel.Responsive.cs SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs SkillBuilderPro.MAUI/Views/GoalsPage.xaml SkillBuilderPro.MAUI/Views/LoginPage.xaml SkillBuilderPro.MAUI/Views/LoginPage.xaml.cs SkillBuilderPro.MAUI/Views/NotificationsPage.xaml SkillBuilderPro.MAUI/Views/ProfilePage.xaml SkillBuilderPro.MAUI/Views/TrainingPage.xaml SkillBuilderPro.MAUI/Views/TrophyRoomPage.xaml docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md
```

The new Builder files and imported assets are untracked relative to HEAD; their exact added state is inspectable with `git diff --no-index -- NUL <path>` or directly from the listed paths. No files were deleted and no commit was created.

Key resolver diff:

```diff
-string GetTrainingBackground(string? sport);
+string GetTrainingPageBackground(string? sport);
+string GetTrainingBuilderBackground(string? sport);
```

Key Android Builder lifecycle diff:

```diff
+viewModel.PropertyChanged += ViewModelPropertyChanged;
+ApplyBackground(viewModel.Background);
+Loaded += (_, _) => ApplyBackground(viewModel.Background);
+BuilderBackground.HandlerChanged += (_, _) => ApplyBackground(viewModel.Background);
@@ OnAppearing
+ApplyBackground(viewModel.Background);
+Dispatcher.Dispatch(() => ApplyBackground(viewModel.Background));
```

Key Trophy fix:

```diff
-<ContentPage ... Title="Trophy">
+<ContentPage ... Title="Trophy" Shell.NavBarIsVisible="False">
```

## Master blueprint updates

Updated `docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md` after reading it completely:

- **7.2 Training** — exact Chicago/premium mapping, Softball gap/fallback, Training resolver responsibility, and precedence.
- **7.3 Training Builder** — exact functional mapping, separate resolver responsibility, precedence, downstream refresh/draft preservation, and nonpersistent Save/Start status.
- **9 Role Login Standard** — permanent Android paste layout/clipboard/masking behavior.
- **14 Current Background Asset Strategy** — packaged Chicago MAUI assets, Softball gap, non-destructive portrait rule, and neutral Notifications decision.
- **15 Known Visual Engineering Finding** — Builder lifecycle reapplication, Android stacking rule, Home/Goals/Trophy portrait lessons, and authenticated logout standard.
- **17 Data / Drill Library Roadmap** — observed three-record live reality, explicit not-live 900 dataset, authenticated real-data behavior, and WATCH versus ADD rule.

Historical planned Film Room, Calendar, persistence, multi-platform, data-import, test, and deployment architecture was retained.

## Emulator verification plan

1. **Login:** Athlete role; Email PASTE; Password PASTE; eye toggle; authenticate; repeat clipboard-empty failure.
2. **Training Page:** switch Basketball, Football, Baseball, Softball, Soccer, Hockey and verify the Matrix A environment each time.
3. **Training Builder:** open from Training for each sport and verify the corresponding Matrix B image. Confirm Training and Builder are visibly different for the same sport.
4. **Real drills:** Basketball → Offense → Shooting ADD; then Dribbling ADD; then Passing ADD; verify all three remain in YOUR SESSION through filter changes.
5. **WATCH:** play one live Basketball drill through the existing Drill Library/video path.
6. **SIGN OUT:** Profile → SIGN OUT → ChooseProfilePage; select another role and confirm no Athlete/Demo leakage.
7. **Home:** review hero, 2×2 metrics, Today's Training, navigation, and scroll reachability at 360–480 dp.
8. **Goals:** review header/subtitle contrast, 2×2 metrics, active and completed goals.
9. **Trophy:** confirm native white bar is absent; inspect rank/hero/achievements/milestones without overlap.
10. **Notifications:** confirm neutral Home environment remains and no sport art appears.

