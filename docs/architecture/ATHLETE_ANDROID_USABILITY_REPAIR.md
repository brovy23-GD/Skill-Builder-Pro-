# Athlete Android Usability Repair

## Result

**BUILD PASS** — `net10.0-android` Debug completed with **40 warnings and 0 errors**.

This was a controlled usability repair. No new artwork, video player, API contract, Film Room route, Calendar route, or global typography/color change was introduced. Android emulator re-verification remains required; this report does not claim visual success.

## Repairs Applied

### Training Builder background

`SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml` retains the live sport binding:

```xml
<Image x:Name="BuilderBackground"
       Source="{Binding Background}"
       Aspect="AspectFill"
       Opacity="1"
       HorizontalOptions="Fill"
       VerticalOptions="Fill"
       InputTransparent="True" .../>
```

- The background image has no negative `ZIndex`.
- Image, translucent overlay, and live content remain in Android-safe declaration order.
- `InputTransparent="True"` prevents decorative layers from intercepting controls.
- No artwork was changed or added.

`TrainingBuilderViewModel` still injects `ISportVisualService` and calls `GetTrainingBackground` when:

- A sport query is received.
- `SelectedSport` changes.
- Initial drill/sport data finishes loading.

Sport initialization precedence is now explicit:

1. A supported sport passed from `TrainingPage` takes precedence.
2. Otherwise Demo Mode uses `DemoDataService.Sport` (`Softball` for Aubrey).
3. Otherwise the authenticated athlete's current sport is used.
4. Existing service/list fallbacks remain available.

The preferred supported sport is inserted into the picker list if current drill results do not contain it, so the passed/current sport is not silently replaced by the first drill sport. Softball was not hardcoded globally.

### Training Builder structure

The builder is now organized into live MAUI sections:

- **Header** — Training Builder, current sport, Back, conditional Exit Demo.
- **Session** — Workout Name.
- **Find Drills** — sport, category, subcategory, available drills, Watch, Add.
- **Your Session** — ordered selected drills, reps, time, Up, Down, Watch, Remove.
- **Summary** — derived total duration and the existing explicitly disabled Phase 1 Save/Start controls.

The responsive code now stacks both the header and the three filter controls on narrow Android layouts. No final visual-polish decisions were made.

### Demo drill preview

The available-drill section now exposes `WATCH SELECTED DRILL`, and each session item exposes `WATCH`.

Both actions call one helper in `TrainingBuilderViewModel`:

```csharp
private static Task OpenDrillPreviewAsync(int drillId) =>
    Shell.Current.GoToAsync(
        $"{nameof(Views.DrillLibraryPage)}?drillId={drillId}&fromTraining=true");
```

This reuses the existing registered `DrillLibraryPage` route and its existing `drillId` resolution/video path. In Demo Mode, `DrillLibraryPage.ResolveDrillAsync` already resolves the ID from `DemoDataService.Drills`, which contains the same playable demo URLs used elsewhere. No second player architecture was created.

### Notifications background

Only the negative `ZIndex` was removed from the Notifications background image:

```diff
-<Image Source="{Binding Background}" ... ZIndex="-2" .../>
+<Image Source="{Binding Background}" .../>
```

The overlay remains immediately after the image and is otherwise unchanged. `NotificationsViewModel.Responsive.cs` still resolves `Background` through:

```csharp
visuals.GetTrainingBackground(
    api.IsDemoMode ? DemoDataService.Sport : api.User?.Sport)
```

No notification background service or binding was replaced.

### Exit Demo

A visible `EXIT DEMO` action was added to the primary Athlete Home header and the Training Builder header.

- Visibility is bound to `IsDemoMode`; normal authenticated users do not see it.
- The command checks `api.IsDemoMode` again before acting.
- It calls the existing `IAthleteApiService.LogoutAsync()` mechanism, which clears Demo Mode, selected role, authorization state, and stored auth state.
- It replaces the window page with `NavigationPage(new ChooseProfilePage(api))`, returning to role selection without terminating the application.
- Existing authenticated Profile `SIGN OUT` behavior remains separate and unchanged.

## API and Architecture Verification

- No API/server files were modified.
- No new DTO or endpoint was introduced.
- `TrainingBuilderViewModel` contains no `PostAsync` call.
- Save Session and Start Training remain disabled Phase 1 draft controls.
- Notifications still uses `ISportVisualService`.
- Training Builder still uses `ISportVisualService`.
- Builder watch/preview uses the existing `DrillLibraryPage` route and player.
- Demo exit uses existing `LogoutAsync`; it does not terminate the app.

## Exact Git Diff

The exact tracked-file diff was inspected with:

```powershell
git diff -- SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml.cs SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs SkillBuilderPro.MAUI/Views/NotificationsPage.xaml SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs
```

`TrainingBuilderPage.xaml`, `TrainingBuilderPage.xaml.cs`, and `TrainingBuilderViewModel.cs` remain new/untracked Phase 1 files relative to repository HEAD, so their complete current contents are their exact additions. This repair changed those additions in place.

Tracked-file diff summary relative to HEAD:

```text
SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs  | 7 +++++--
SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml | 4 ++--
SkillBuilderPro.MAUI/Views/NotificationsPage.xaml    | 2 +-
3 files changed, 8 insertions(+), 5 deletions(-)
```

Exact repair hunks:

```diff
--- SkillBuilderPro.MAUI/Views/NotificationsPage.xaml
+++ SkillBuilderPro.MAUI/Views/NotificationsPage.xaml
@@
-<Image Source="{Binding Background}" Aspect="AspectFill" Opacity="1" ZIndex="-2" HorizontalOptions="Fill" VerticalOptions="Fill"/>
+<Image Source="{Binding Background}" Aspect="AspectFill" Opacity="1" HorizontalOptions="Fill" VerticalOptions="Fill"/>

--- SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml
+++ SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml
@@
-<Grid Grid.Column="1">...NOTIFICATIONS...</Grid>
+<VerticalStackLayout Grid.Column="1">...NOTIFICATIONS...<Button Text="EXIT DEMO" IsVisible="{Binding IsDemoMode}" Command="{Binding ExitDemoCommand}" .../></VerticalStackLayout>

--- SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs
+++ SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs
@@ DashboardViewModel
+public bool IsDemoMode=>api.IsDemoMode;
+[RelayCommand] async Task ExitDemo(){if(!api.IsDemoMode)return;await api.LogoutAsync();Application.Current!.Windows[0].Page=new NavigationPage(new Views.ChooseProfilePage(api));}

--- SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml
+++ SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml
@@
+Android-safe bound background with no negative ZIndex
+responsive Header / Session / Find Drills / Your Session / Summary sections
+conditional EXIT DEMO
+WATCH SELECTED DRILL and per-session-item WATCH controls

--- SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml.cs
+++ SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml.cs
@@
-two-column BuilderGrid responsive manipulation
+phone/wide HeaderGrid and FilterGrid responsive manipulation

--- SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs
+++ SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs
@@
+query/current/demo sport precedence and immediate SportVisualService background resolution
+CanPreviewAvailableDrill and PreviewAvailableDrillCommand
+shared DrillLibraryPage preview route helper
+demo-only ExitDemoCommand using existing LogoutAsync
```

The working tree already contained earlier controlled/Phase 1 changes relative to HEAD, including the Home background negative-`ZIndex` removal and Training hub additions. They were preserved and were not reverted.

## Build Verification

Command:

```powershell
dotnet build SkillBuilderPro.MAUI/SkillBuilderPro.MAUI.csproj -f net10.0-android -c Debug
```

Result:

```text
Build succeeded.
    40 Warning(s)
    0 Error(s)
Time Elapsed 00:01:39.97
```

### Errors

**0 errors.**

### Warnings

**40 warnings**, separately reported. They are existing obsolescence/nullability warnings, primarily:

- `CS0618`: existing `DisplayAlert` and `Application.MainPage` use.
- `CS8622`: existing/generated XAML event-handler nullability mismatches.
- `CS8602`: existing possible null dereference in `DrillsViewModel.cs`.

## Required Emulator Re-verification

Build success is not visual or runtime proof. Verify on Android:

1. Aubrey enters Training Builder as Softball when no explicit sport overrides it.
2. A sport passed from Training takes precedence and renders its approved background.
3. Changing builder sport changes its background and drill filters.
4. Background remains visible behind the live overlay/content in portrait and landscape.
5. Notifications renders the current/demo sport background.
6. Available-drill and session-item WATCH actions open the existing Drill Library and play demo videos.
7. Back navigation returns to the builder without corrupting its transient draft state.
8. EXIT DEMO on Home and Builder clears Demo Mode and returns to role selection.
9. EXIT DEMO is absent for authenticated athletes, whose Profile sign-out remains functional.

## Documented Follow-up: Athlete Contrast/Layout Polish

All Athlete pages require a dedicated follow-up contrast and layout polish pass. That pass should evaluate each page and each approved background using:

- Background-aware text contrast.
- Darker/translucent backing where needed.
- Repositioning content into negative-space zones.
- Purposeful portrait and landscape layouts.
- Preservation of the approved background artwork.

This follow-up must be page-specific. This repair did not globally darken text or alter shared typography/colors.
