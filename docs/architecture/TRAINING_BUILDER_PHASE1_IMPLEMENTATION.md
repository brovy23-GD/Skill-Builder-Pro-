# Training / Training Builder Phase 1 Implementation

## Result

**BUILD PASS** — `net10.0-android` Debug completed with **40 warnings and 0 errors**.

No visual-success claim is made. Android emulator verification is still required.

## Implemented Structure

- Preserved the existing Training shell tab and exact `//Training` route in `SkillBuilderPro.MAUI/AppShell.xaml`.
- Converted `TrainingPage` into the athlete Training hub while preserving:
  - `IQueryAttributable`.
  - Selected-sport initialization from the `sport` query parameter.
  - `Source="{Binding Background}"` and `SportVisualService` behavior.
  - Existing drill and assignment API loading.
  - Demo/authenticated service behavior.
  - Training Requests navigation.
- Added `TrainingBuilderPage` as a pushed route, not a shell tab.
- Added a transient, isolated `TrainingBuilderViewModel`; the singleton `DrillsViewModel` is not used as mutable builder state.
- Added an in-memory `TrainingSessionDraft` and ordered `TrainingSessionDraftItem` model.
- Added builder support for:
  - Workout name.
  - Sport/category/subcategory filtering.
  - Available drill selection.
  - Multiple session items.
  - Per-item reps and duration minutes.
  - Derived total session duration.
  - Remove, move up, and move down with normalized order.
  - Drill preview through the existing `DrillLibraryPage?drillId=...&fromTraining=true` route.
- Save and Start controls are explicitly disabled and labeled as Phase 1/non-persistent.
- No Film Room or Calendar controls/routes were added.
- No API/server contract was added or changed, and the builder makes no POST request.

## Files Added

1. `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml`
2. `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml.cs`
3. `SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs`
4. `SkillBuilderPro.MAUI/Models/TrainingSessionDraft.cs`

## Files Modified

1. `SkillBuilderPro.MAUI/Views/TrainingPage.xaml`
2. `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs`
3. `SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs`
4. `SkillBuilderPro.MAUI/AppShell.xaml.cs`
5. `SkillBuilderPro.MAUI/MauiProgram.cs`

No unrelated implementation page was modified by this phase. Other pre-existing working-tree changes were preserved.

## Navigation Verification

### Training route unchanged

`SkillBuilderPro.MAUI/AppShell.xaml` remains:

```xml
<ShellContent
    Title="Training" Route="Training" ContentTemplate="{DataTemplate views:TrainingPage}" />
```

Therefore `//Training` still resolves to `TrainingPage`.

### Builder is a pushed route

`SkillBuilderPro.MAUI/AppShell.xaml.cs` now contains:

```csharp
Routing.RegisterRoute(nameof(TrainingBuilderPage),typeof(TrainingBuilderPage));
```

It was not added to `AppShell.xaml` as a tab.

### Sport handoff

`TrainingPage` passes its current sport with URI encoding:

```csharp
async void TrainingBuilderClicked(object? s,EventArgs e)
{
    var sport=((TrainingViewModel)BindingContext).SelectedSport??string.Empty;
    await Shell.Current.GoToAsync($"{nameof(TrainingBuilderPage)}?sport={Uri.EscapeDataString(sport)}");
}
```

`TrainingBuilderPage.ApplyQueryAttributes` decodes the value and calls the isolated builder view model's `SetRequestedSport` before/while its drill data loads.

## Hub Behavior

The hub now renders:

- Current sport and Change Sport picker.
- Sport-specific background through the existing binding.
- Today's Training summary from the first existing active assignment.
- Active assignments.
- Recent/completed training when returned by the existing assignment endpoint.
- Entry buttons for Training Builder, Drill Library, and Training Requests.

The existing `TrainingViewModel.LoadCommand` still uses only:

```text
GET api/drills
GET api/athlete/assignments
```

Demo mode continues to use `DemoDataService.Drills` and `DemoDataService.Assignments`.

## Builder Draft Behavior

`TrainingBuilderViewModel` is registered transient alongside `TrainingBuilderPage`. Each pushed builder page therefore receives isolated draft state.

Each `TrainingSessionDraftItem` stores:

- The existing Core `Drill` reference.
- Derived `DrillId`.
- `Reps`.
- `DurationMinutes`.
- `Order`.

`TotalDurationMinutes` is derived by summing non-negative item durations. Item duration edits notify the view model and update the total. Remove and reorder operations normalize every item's one-based order.

The builder loads drills with the existing authenticated/demo-aware `IAthleteApiService` and resolves its selected-sport background with `ISportVisualService`. Its background image has no negative `ZIndex`.

## Persistence Boundary

No Save Session endpoint or API DTO was invented. `TrainingBuilderViewModel` contains no `PostAsync` call. The visible persistence control is:

```xml
<Button Text="SAVE SESSION — PERSISTENCE NOT AVAILABLE"
        IsEnabled="False" .../>
```

Start Training is also disabled and labeled `PHASE 1 DRAFT ONLY`; no assignment, request, schedule, completion, or progression API is called from the draft.

## Exact Git Diff

The tracked-file diff was inspected with:

```powershell
git diff -- SkillBuilderPro.MAUI/Views/TrainingPage.xaml SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs SkillBuilderPro.MAUI/AppShell.xaml.cs SkillBuilderPro.MAUI/MauiProgram.cs
```

Exact tracked-file delta:

```text
SkillBuilderPro.MAUI/AppShell.xaml.cs                |  1 +
SkillBuilderPro.MAUI/MauiProgram.cs                  |  4 ++--
SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs |  4 +++-
SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs      |  4 +++-
SkillBuilderPro.MAUI/Views/TrainingPage.xaml         | 14 +++++++-------
5 files changed, 16 insertions(+), 11 deletions(-)
```

Exact semantic hunks:

```diff
--- SkillBuilderPro.MAUI/AppShell.xaml.cs
+++ SkillBuilderPro.MAUI/AppShell.xaml.cs
@@
+Routing.RegisterRoute(nameof(TrainingBuilderPage),typeof(TrainingBuilderPage));

--- SkillBuilderPro.MAUI/MauiProgram.cs
+++ SkillBuilderPro.MAUI/MauiProgram.cs
@@
+builder.Services.AddTransient<TrainingBuilderPage>();
+builder.Services.AddTransient<TrainingBuilderViewModel>();

--- SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs
+++ SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs
@@
+public string TodayTrainingTitle=>Active.FirstOrDefault()?.Drill.Name??"No training assigned today";
+public string TodayTrainingSummary=>...;
@@ LoadCommand completion
+OnPropertyChanged(nameof(TodayTrainingTitle));
+OnPropertyChanged(nameof(TodayTrainingSummary));

--- SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs
+++ SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs
@@
-TrainingStageSizeChanged adjusted TrainingBuilderGrid/DrillCard
+TrainingStageSizeChanged adjusts TrainingContent padding only
+TrainingBuilderClicked passes the encoded selected sport to TrainingBuilderPage
+DrillLibraryClicked pushes the existing DrillLibraryPage route

--- SkillBuilderPro.MAUI/Views/TrainingPage.xaml
+++ SkillBuilderPro.MAUI/Views/TrainingPage.xaml
@@
-Embedded TRAINING BUILDER filter/drill grid
+TRAINING hub heading
+Current sport / Change Sport
+Today's Training summary
+Training Builder / Drill Library / Training Requests entries
+Active assignment details
+Recent completed training
```

The four added files are new, so their entire current contents constitute their exact additions at the paths listed under “Files Added.” The diff also shows the earlier controlled Training background fix relative to repository HEAD (`ZIndex="-2"` removed from `TrainingBackground`); that change was already present in the working tree before this phase and was preserved.

## Build Verification

Command:

```powershell
dotnet build SkillBuilderPro.MAUI/SkillBuilderPro.MAUI.csproj -f net10.0-android -c Debug
```

Final result:

```text
Build succeeded.
    40 Warning(s)
    0 Error(s)
Time Elapsed 00:00:20.15
```

### Warnings

The 40 warnings are existing obsolescence/nullability warnings, principally:

- `CS0618` for legacy `DisplayAlert` and `Application.MainPage` use.
- `CS8622` nullability mismatches in existing/generated XAML event hookups.
- `CS8602` in existing `DrillsViewModel.cs`.

### Errors

Final build: **0 errors**.

An initial build exposed one Core-vs-MAUI `Drill` name collision in the new draft model. It was corrected with the explicit alias `CoreDrill = SkillBuilderPro.Core.Models.Drill`; the required final build then passed.

## Required Emulator Verification

Build success confirms compilation only. Android emulator testing must verify:

1. `//Training` opens the hub.
2. Query sport initialization and sport background switching.
3. Active/recent assignment rendering in demo and authenticated modes.
4. Builder push/back/exit navigation and sport handoff.
5. Phone scrolling and responsive single-column layout.
6. Drill filtering, multiple adds, reps/time editing, total recalculation, remove, and reorder.
7. Drill preview and return behavior.
8. Disabled Save/Start controls and absence of persistence calls.

No visual success is claimed before those emulator checks.
