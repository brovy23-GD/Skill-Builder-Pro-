# Android Runtime Visual and Authenticated Drill Repair

Date: 2026-08-20

## Outcome

The focused repair compiles for Android Debug: **BUILD PASS**, 40 warnings, 0 errors. Android emulator verification is still required; this report does not claim runtime or visual success.

## Authenticated Athlete drill root cause

The Builder calls the correct real endpoint, `GET /api/drills`. A live read-only request to the development API at `http://localhost:5000/api/drills` returned **HTTP 200** and exactly three real records:

| ID | Sport | Category | Subcategory | Video |
|---:|---|---|---|---|
| 239 | Basketball | Offense | Shooting | valid YouTube URL |
| 240 | Basketball | Offense | Dribbling | valid YouTube URL |
| 241 | Basketball | Offense | Passing | valid YouTube URL |

The authenticated failure was a client/data-state interaction, not missing Athlete read permission:

1. `TrainingBuilderViewModel.Load()` loaded `api/drills`, but then discarded every drill whose `VideoUrl` failed `YouTubeUrl.IsValid`. A real drill should remain selectable even when WATCH is unavailable.
2. The Builder initialized `SelectedSport` from the passed sport or authenticated Athlete sport. It inserted a supported preferred sport into the picker even if the live API returned no drills for that sport. The currently live dataset is Basketball-only, so a valid Athlete sport such as Softball produced empty categories and drills.
3. The XAML used one generic empty message and did not distinguish incomplete filters, a valid zero-result selection, or an API failure. This made the real-data mismatch look like a disabled/broken Builder.
4. Category/subcategory were not automatically fabricated; changing sport did refresh collections, but a sport with no API rows legitimately left them empty. There was no guidance telling the Athlete to choose a sport backed by current live content.

The repository does not prove that a 900-drill dataset is live. The live endpoint observed during this pass exposes only the three rows above. No Demo drills were substituted for an authenticated user.

## Authenticated drill and authorization path

- Endpoint: `GET /api/drills` from `TrainingBuilderViewModel` through `IAthleteApiService.GetAsync<List<Drill>>()`.
- Status observed: HTTP 200 from the running development API. A control request to `GET /api/auth/me` without a token returned HTTP 401.
- Bearer behavior: after successful login, `AthleteApiService.SetToken()` assigns `http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token)`. Subsequent Builder requests therefore carry the JWT. The live 200 probe itself was anonymous and did not use the test account's credentials.
- Authorization: `DrillsController.GetDrills()` has `[AllowAnonymous]`, so Athlete read access is permitted. POST, PUT, and DELETE remain restricted to Coach/Administrator. Authorization was not weakened.
- Account: no password or credential for `athlete2@skillbuilderpro.local` was read, stored, or hardcoded. The account's current sport could not be independently queried without authenticating or accessing the database; the repaired Builder safely handles null, unsupported, and currently unpopulated sports.

## Drill-flow repair

File: `SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs`

- Authenticated users retain every real API drill. Video validity now controls WATCH only, not ADD.
- The sport picker exposes the six supported sport choices from `SportVisualService`; it does not manufacture drill rows.
- Passed sport still takes precedence, then Demo sport or authenticated `CurrentUser.Sport`, then the first sport that actually has live API content.
- Sport change clears only category/subcategory selections, refreshes categories, subcategories, and available drills, and does **not** clear `SessionItems`.
- Category change clears the stale subcategory and refreshes the downstream collections.
- Available drills remain empty until Sport, Category, and Skill/Subcategory are selected.
- Incomplete filters show `SELECT A SPORT, CATEGORY, AND SKILL TO FIND DRILLS`.
- A valid complete selection with zero rows shows `NO DRILLS FOUND FOR THIS SELECTION`.
- A null API result shows `Drills could not be loaded from the service. Try again.` or the existing service-unavailable message, with the existing RETRY action.
- `CanAddSelectedDrill` is true whenever a real row is selected. `CanPreviewAvailableDrill` additionally requires a valid YouTube URL.
- `AddSelectedDrillCommand` creates a `TrainingSessionDraftItem` using the real `Drill` object/`int Id`, adds it to `Draft.Items`, resets only the available selection, recalculates totals, and leaves earlier items intact. Multiple drills are supported; no ID/type conversion occurs.
- Reordering/removal logic and transient-only persistence remain unchanged. No API contract was invented.

Expected chain after repair:

```text
Sport -> Category -> Skill/Subcategory -> Available Drills -> select -> WATCH or ADD -> YOUR SESSION
```

## Login runtime repair

Files:

- `SkillBuilderPro.MAUI/Views/LoginPage.xaml`
- `SkillBuilderPro.MAUI/Views/LoginPage.xaml.cs`

The permanent portrait-safe rows are:

| Row | Grid columns | Control placement |
|---|---|---|
| Email | `*,58` | column 0 `EmailEntry`; column 1 always-visible `PASTE` |
| Password | `*,58,44` | column 0 masked `PasswordEntry`; column 1 always-visible `PASTE`; column 2 eye button |

Both PASTE buttons use `Clipboard.Default.GetTextAsync()`. Empty or unavailable clipboard operations display a small inline message. Password paste always restores `IsPassword=true`; the eye control remains independent. Compact font/padding and fixed action columns prevent the password PASTE action from being pushed outside phone portrait width. The surrounding `ScrollView` remains. Exact CTAs remain Athlete `ENTER LOCKER ROOM`, Coach `ENTER COACH'S OFFICE`, Parent `ENTER PARENT HUB`, and Administrator `ENTER ADMIN CENTER`.

## Training Builder background runtime repair

Files:

- `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml`
- `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml.cs`
- `SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs`
- canonical mapper: `SkillBuilderPro.MAUI/Services/SportVisualService.cs`

The image is declared first, overlay second, live scroll content third. The Image uses `AspectFill`, fills both axes, and has no negative `ZIndex`. The overlay also no longer relies on negative stacking.

The prior path depended solely on a binding update during query/application/load lifecycle. Although `Background` raised `PropertyChanged`, the verified Android result showed that the renderer retained a blank layer. The page now sets `BuilderBackground.Source = ImageSource.FromFile(assetName)` after `BindingContext` assignment and again whenever the ViewModel raises `PropertyChanged` for `Background`. Sport state and canonical mapping remain in the ViewModel/service.

Exact mapping:

| Sport | Existing MAUI asset |
|---|---|
| Basketball | `basketball_training.png` |
| Football | `football_training.png` |
| Baseball | `baseball_training.png` |
| Softball | `softball_training.png` |
| Soccer | `soccer_training.png` |
| Hockey | `hockey_training.png` |

All six files were verified under `SkillBuilderPro.MAUI/Resources/Images`. No artwork was created. A passed sport still takes precedence, Aubrey Demo resolves through `DemoDataService.Sport` to Softball, authenticated state uses `CurrentUser.Sport`, and picker changes update `Background` then immediately assign the corresponding visible image.

## Contrast and Notifications

The current worktree's conservative named contrast surfaces apply dark smoky local backplates rather than replacing or globally darkening artwork. The affected Athlete compositions are Athlete Home/Dashboard, Training, Training Builder, Notifications, Goals, Trophy Room, Drill Library, and Profile/Locker Room through their existing local/named glass and elite surfaces. Notifications continues to use neutral `home_background_approved.png` and does not use a sport training image. Dedicated Notifications artwork remains deferred.

## Exact focused diff

```diff
--- LoginPage.xaml
+++ LoginPage.xaml
@@
-<Grid ColumnDefinitions="*,Auto" ColumnSpacing="8">
+<Grid ColumnDefinitions="*,58" ColumnSpacing="6">
@@
-<Grid ColumnDefinitions="*,Auto,52" ColumnSpacing="8">
+<Grid ColumnDefinitions="*,58,44" ColumnSpacing="6">
+<!-- permanent compact PASTE in column 1; eye in password column 2 -->

--- LoginPage.xaml.cs
+++ LoginPage.xaml.cs
@@
+async void PasteEmailClicked(...)=>await PasteAsync(EmailEntry,false);
+async void PastePasswordClicked(...)=>await PasteAsync(PasswordEntry,true);
+async Task PasteAsync(Entry target,bool password){try{var text=await Clipboard.Default.GetTextAsync();...}catch(Exception){ShowInlineMessage("Clipboard is unavailable. Copy the text and try PASTE again.");}}

--- TrainingBuilderPage.xaml
+++ TrainingBuilderPage.xaml
@@
-<BoxView Color="#42040910" ZIndex="-1" InputTransparent="True"/>
+<BoxView Color="#42040910" InputTransparent="True"/>
@@
-Title="All categories"
+Title="Select category"
-<Label Text="SUBCATEGORY" .../><Picker Title="All subcategories" .../>
+<Label Text="SKILL" .../><Picker Title="Select skill" .../>
-EmptyView="NO DRILLS MATCH THESE FILTERS"
+<CollectionView.EmptyView><Label Text="{Binding DrillEmptyState}" TextColor="#F3F6F9" BackgroundColor="#A8121821" Padding="14" HorizontalTextAlignment="Center"/></CollectionView.EmptyView>

--- TrainingBuilderPage.xaml.cs
+++ TrainingBuilderPage.xaml.cs
@@
+viewModel.PropertyChanged += ViewModelPropertyChanged;
+ApplyBackground(viewModel.Background);
+private void ViewModelPropertyChanged(...){if(e.PropertyName==nameof(TrainingBuilderViewModel.Background))ApplyBackground(...);}
+private void ApplyBackground(string assetName)=>BuilderBackground.Source=ImageSource.FromFile(assetName);

--- TrainingBuilderViewModel.cs
+++ TrainingBuilderViewModel.cs
@@
-public bool CanPreviewAvailableDrill => SelectedAvailableDrill is not null;
+public bool CanPreviewAvailableDrill => YouTubeUrl.IsValid(SelectedAvailableDrill?.VideoUrl);
+public bool NeedsFilterSelection => ...;
+public string DrillEmptyState => NeedsFilterSelection ? "SELECT A SPORT, CATEGORY, AND SKILL TO FIND DRILLS" : "NO DRILLS FOUND FOR THIS SELECTION";
@@
-allDrills.AddRange(source.Where(drill => YouTubeUrl.IsValid(drill.VideoUrl)));
-Sports.Reset(allDrills.Select(...));
+allDrills.AddRange(source);
+Sports.Reset(visuals.SupportedSports.Where(sport => !string.Equals(sport,"Strength",StringComparison.OrdinalIgnoreCase)));
@@
+if(NeedsFilterSelection) AvailableDrills.Clear();
+else AvailableDrills.Reset(allDrills.Where(...));
```

The Builder files are untracked Phase 1 additions relative to repository HEAD, so ordinary `git diff` does not display them; their current full contents are the exact added-file state. Existing unrelated and earlier controlled worktree changes were preserved.

## Build verification

Command:

```text
dotnet build SkillBuilderPro.MAUI/SkillBuilderPro.MAUI.csproj -f net10.0-android -c Debug
```

Result:

```text
Build succeeded.
40 Warning(s)
0 Error(s)
Time Elapsed 00:00:30.84
```

Warnings are existing MAUI obsolescence/nullability warnings (including `DisplayAlert`, `Application.MainPage`, generated XAML handler nullability, and one `DrillsViewModel` nullable dereference). No compile errors occurred.

## Required emulator verification

1. Log in as the development Athlete and confirm the request succeeds against `10.0.2.2:5000` with the bearer token attached.
2. Open Builder with the Athlete sport. If it has no live rows, verify the readable no-results/guidance state and select Basketball.
3. Select Basketball -> Offense -> Shooting/Dribbling/Passing, select a drill, ADD it, then add two more and verify all remain in YOUR SESSION.
4. Change sport and filters and verify the existing draft remains intact.
5. Confirm WATCH works for the three live video URLs and a future non-video drill remains ADD-able but not WATCH-able.
6. Confirm each sport change updates the actual field/court/rink image and that no blank image layer remains.
7. Exercise both permanent Login PASTE actions with populated, empty, and unavailable clipboard states in portrait and landscape.
8. Visually inspect contrast on every listed Athlete page. Build success is not visual proof.

