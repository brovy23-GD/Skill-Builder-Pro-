# Training / Training Builder Architecture Audit

Repository: `C:\Users\brovy\source\repos\SkillBuilderPro`  
Scope: current MAUI implementation only; static inspection performed 2026-08-20.  
Implementation files were not changed. This report is the only file created.

## Current Implementation

### Finding

Training and Training Builder are currently implemented as **one page and one view model**, not as separate flows.

The athlete shell has one Training tab:

```xml
<!-- SkillBuilderPro.MAUI/AppShell.xaml:14-15 -->
<ShellContent
    Title="Training" Route="Training" ContentTemplate="{DataTemplate views:TrainingPage}" />
```

That route loads `SkillBuilderPro.MAUI/Views/TrainingPage.xaml`. Although the page title is `Training`, its visible heading and main content identify it as the builder:

```xml
<!-- SkillBuilderPro.MAUI/Views/TrainingPage.xaml:6 -->
<Label Text="ATHLETE TRAINING" .../>
<Label Text="TRAINING BUILDER" .../>
<Label Text="BUILD THE WORK. MASTER THE DETAILS." .../>
```

The same page then renders:

- Sport, category, and subcategory filters (`TrainingPage.xaml:8`).
- A filtered drill list and a single selected drill (`TrainingPage.xaml:9`).
- An `OPEN TRAINING VIDEO` action (`TrainingPage.xaml:9`; handler at `AthletePages.xaml.cs:24`).
- Active assignments and a `REQUESTS` action (`TrainingPage.xaml:11`; handler at `AthletePages.xaml.cs:21`).

The page uses one `TrainingViewModel`:

```csharp
// SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs:15-20
public partial class TrainingPage:ContentPage,IQueryAttributable
{
 public TrainingPage(TrainingViewModel vm){InitializeComponent();BindingContext=vm;}
 ...
 protected override void OnAppearing(){base.OnAppearing();((TrainingViewModel)BindingContext).LoadCommand.Execute(null);}
```

`TrainingViewModel` combines drill/filter state and assignment state (`SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs:25-40`):

- `Active` and `Completed` assignments at line 29.
- `Drills`, `Sports`, `Categories`, and `SubCategories` at line 30.
- `SelectedDrill`, `SelectedSport`, `SelectedCategory`, `SelectedSubCategory`, and sport background at line 31.
- Sport/category/subcategory filtering at lines 33-37.
- Drill loading from `api/drills` and assignment loading from `api/athlete/assignments` in the same `Load` command at line 39.

### No separate Training Builder page

No `TrainingBuilderPage` class or XAML exists. The complete current page inventory under `SkillBuilderPro.MAUI/Views` contains `TrainingPage` and `TrainingRequestsPage`, but no other page whose implementation provides the required workout-builder flow.

Pages that could be mistaken for a builder are not equivalent:

- `SportListPage` chooses a sport and uses `DrillsViewModel`.
- `CategoryListPage` chooses a category.
- `DrillListPage` supports selecting up to five drills, but only launches the first selected drill's video and does not construct or save a workout.
- `DrillLibraryPage` is a drill/video library and player, not a session builder.
- `VideoPlayerPage` is a player page.
- `TrainingRequestsPage` lists training requests.

No current MAUI page owns workout name, per-drill reps/time, ordering, calculated total duration, Save Session, or a multi-drill Start Training operation.

### Responsibility map

| Concern | Current implementation | Exact evidence |
|---|---|---|
| Training shell route/tab | `AppShell.xaml` → `TrainingPage` | `SkillBuilderPro.MAUI/AppShell.xaml:14-15` |
| Training page UI and combined builder/hub content | `TrainingPage.xaml` | `SkillBuilderPro.MAUI/Views/TrainingPage.xaml:1-14` |
| Training page code-behind/navigation/responsive layout | Shared `AthletePages.xaml.cs` | `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs:15-25` |
| Sport selection on current Training page | `TrainingPage` picker + `TrainingViewModel.SelectedSport` | `TrainingPage.xaml:8`; `AthleteViewModels.cs:30-39` |
| Sport-dependent background | `TrainingViewModel` + `ISportVisualService` | `AthleteViewModels.cs:25,31-33,39`; `Services/SportVisualService.cs:3-24` |
| Current inline drill browsing | `TrainingPage` filtered `CollectionView` | `TrainingPage.xaml:8-9`; `AthleteViewModels.cs:30,33-39` |
| Separate legacy drill-browsing flow | `SportListPage` → `CategoryListPage` → `DrillListPage`, backed by singleton `DrillsViewModel` | `DrillsViewModel.cs:44-117,119-157`; `AppShell.xaml.cs:11-17`; `MauiProgram.cs:62-66` |
| Selected drill on current Training page | One `SelectedDrill` | `TrainingPage.xaml:9`; `AthleteViewModels.cs:31,37-38` |
| Multi-select drill state | `DrillsViewModel.SelectedDrills`, populated by `DrillListPage` | `DrillsViewModel.cs:24-25`; `DrillListPage.xaml:47-52`; `DrillListPage.xaml.cs:21-45` |
| Workout/session construction | No complete implementation | No workout-name, reps/time, ordering, aggregate duration, save, or multi-drill start state/actions exist in current MAUI views/view models. |
| Active/completed assignments | `TrainingViewModel` | `AthleteViewModels.cs:29,39`; endpoint `api/athlete/assignments` at line 39 |
| Training requests | `TrainingRequestsPage` + `RequestsViewModel` | `TrainingRequestsPage.xaml:1-10`; `AthletePages.xaml.cs:21,26`; `AthleteViewModels.cs:41-42` |
| Drill video/library | `DrillLibraryPage`, reached from `TrainingPage` for one selected drill | `AthletePages.xaml.cs:24`; route registered at `AppShell.xaml.cs:23-25` |

### Existing buttons and navigation on TrainingPage

| Button | Location | Current behavior |
|---|---|---|
| `RETRY` | `TrainingPage.xaml:5` | Executes `{Binding LoadCommand}`. |
| `BACK` | `TrainingPage.xaml:6` | `BackClicked`: pops the navigation stack when possible; otherwise routes to `//Home` (`AthletePages.xaml.cs:22`). |
| `EXIT` | `TrainingPage.xaml:6` | Routes to `//Home` (`AthletePages.xaml.cs:23`). |
| `CLEAR SELECTION` | `TrainingPage.xaml:8` | Executes `ClearSelectionCommand`; clears selected drill/category/subcategory and refreshes filters (`AthleteViewModels.cs:38`). |
| `OPEN TRAINING VIDEO` | `TrainingPage.xaml:9` | Requires one `SelectedDrill`, then routes to `DrillLibraryPage?drillId=...&fromTraining=true` (`AthletePages.xaml.cs:24`). |
| `REQUESTS` | `TrainingPage.xaml:11` | Routes to `TrainingRequestsPage` (`AthletePages.xaml.cs:21`). |

There are currently no TrainingPage buttons for Training Builder, a general Drill Library entry, Film Room, Calendar, Save Session, or Start Training.

### Current hub requirement coverage

| Required Training-hub responsibility | Current status |
|---|---|
| Sport selection | Present, but embedded in the builder filter card (`TrainingPage.xaml:8`). |
| Today's training | Not implemented as a distinct TrainingPage section. Active assignments are shown, but there is no dedicated “today” selection/presentation. |
| Assignments | Partially present: active assignments list (`TrainingPage.xaml:11`; `AthleteViewModels.cs:29,39`). Completed assignments are loaded but not bound on the page. |
| Performance summary | Not present on TrainingPage. `DashboardViewModel` and progression endpoints contain reusable summary data elsewhere. |
| Recent training | Not rendered on TrainingPage. `Completed` assignments are loaded (`AthleteViewModels.cs:29,39`) but unused by current Training XAML. |
| Entry to Training Builder | Not present because builder UI is embedded on the same page. |
| Entry to Drill Library | Only the selected-drill-specific `OPEN TRAINING VIDEO` path exists; no general hub entry. |
| Entry to Film Room | No button, route, or `FilmRoomPage` exists in current MAUI Views. |
| Entry to Calendar | No button, route, or `CalendarPage` exists in current MAUI Views. |

## Training vs Training Builder Overlap

The current `TrainingPage` overlaps the two required concepts as follows:

| Current feature | Required owner | Current owner | Assessment |
|---|---|---|---|
| Sport selection | Training hub (and builder needs a sport for its workout) | `TrainingPage`/`TrainingViewModel` | Shared concern is currently coupled to builder filtering and background selection. |
| Category/subcategory filters | Training Builder | `TrainingPage`/`TrainingViewModel` | Builder responsibility already present. |
| Available drills | Training Builder | `TrainingPage`/`TrainingViewModel` | Present, filtered by sport/category/subcategory. |
| One selected drill | Training Builder precursor | `TrainingPage`/`TrainingViewModel` | Present, but not a selected-workout collection. |
| Active assignments | Training hub | `TrainingPage`/`TrainingViewModel` | Hub responsibility present on the combined page. |
| Training requests | Training hub/supporting flow | Entry on `TrainingPage`; separate `TrainingRequestsPage` | Already separated as a route. |
| Open selected drill video | Drill Library/training execution entry | `TrainingPage` → `DrillLibraryPage` | Existing behavior should remain available after a split. |
| Workout name | Training Builder | Missing | No property or input. |
| Selected drill collection with reps/time/order | Training Builder | Missing | `DrillsViewModel.SelectedDrills` is a plain drill collection, capped at five, with no workout-item metadata. |
| Total duration | Training Builder | Missing | Individual `Drill.Duration` is displayed, but no aggregate session duration exists. |
| Save Session | Training Builder | Missing | No command, button, MAUI client contract, or API call was found. |
| Start Training | Training Builder | Missing | `DrillListPage` can play the first selected video; no session-level start action exists. |

The page is therefore not merely mislabeled. It structurally combines a partial hub (assignments/requests) with a partial builder (filters/available drills/single selection), while several requirements for both target experiences remain absent.

## Existing Reusable Components

### View models and state

1. **`TrainingViewModel`** — directly reusable for the first safe split.
   - Loads drills through the existing authenticated/demo-aware `IAthleteApiService` (`AthleteViewModels.cs:39`).
   - Loads active and completed athlete assignments through the current endpoint (`AthleteViewModels.cs:29,39`).
   - Handles the `sport` query parameter via `TrainingPage.ApplyQueryAttributes` and `SetRequestedSport` (`AthletePages.xaml.cs:19`; `AthleteViewModels.cs:32`).
   - Implements sport/category/subcategory filtering (`AthleteViewModels.cs:33-37`).
   - Resolves the sport background (`AthleteViewModels.cs:31,33,39`).

2. **`DrillsViewModel`** — reusable concepts/code, but not a complete builder model.
   - Loads all drills using `DrillApiClient` (`DrillsViewModel.cs:44-90`).
   - Provides sport/category drill browsing (`DrillsViewModel.cs:92-157`).
   - Maintains `SelectedDrills` (`DrillsViewModel.cs:24-25`), populated by multi-select UI (`DrillListPage.xaml.cs:21-45`).
   - It is registered singleton (`MauiProgram.cs:66`), so its selection state currently spans the legacy drill-browsing pages. Reusing it directly for mutable workout construction would also share that state; a dedicated builder view model is safer.

3. **`DashboardViewModel` / progression models** — reusable data sources for a hub performance summary, subject to selecting the exact product fields.
   - The dashboard already loads progression, goals, assignments, and unread count in `AthleteViewModels.cs:13-16`.
   - `TrainingViewModel.Completed` already provides a candidate source for recent completed training (`AthleteViewModels.cs:29,39`).

4. **`RequestsViewModel`** — reusable unchanged for the existing separate requests flow (`AthleteViewModels.cs:41-42`).

### Services and clients

- `IAthleteApiService`: existing authenticated/demo-aware access for `api/drills`, `api/athlete/assignments`, and other athlete endpoints (`AthleteViewModels.cs:39`). Keeping this service avoids API/authentication behavior changes.
- `ISportVisualService` / `SportVisualService`: existing sport list and background mapping (`SportVisualService.cs:3-24`), registered singleton at `MauiProgram.cs:60`.
- `DrillApiClient`: existing drill source used by the legacy browse flow (`DrillsViewModel.cs:17,39-53`; registered at `MauiProgram.cs:59`).
- `DemoDataService`: current demo drills and assignments are already used by `TrainingViewModel` (`AthleteViewModels.cs:39`).

### Pages and routes

- `DrillLibraryPage` is reusable as the drill detail/video execution destination. The current Training route passes `drillId` and `fromTraining=true` (`AthletePages.xaml.cs:24`).
- `TrainingRequestsPage` is already separated and can remain unchanged.
- `SportListPage`, `CategoryListPage`, and `DrillListPage` provide reusable browsing behavior, but they form an older parallel flow using singleton `DrillsViewModel`. They should not silently become the builder without addressing missing workout-item state and persistence.

### Missing reusable contract

No current MAUI workout/session draft model or persistence contract was found for:

- Workout name.
- Ordered workout items.
- Per-item reps or time.
- Aggregate duration.
- Save Session.
- Start Training as a multi-drill session.

Core/API training schedule and assignment domain types exist elsewhere in the repository, but current MAUI code does not expose them as a workout-builder client contract. The split can be performed without changing APIs, but a genuinely persistent Save Session cannot be claimed until an existing compatible endpoint is identified or a new contract is explicitly authorized.

## Proposed Smallest Safe Split

### Architecture

1. **Keep `TrainingPage` as the existing `//Training` shell tab and route.** This preserves all incoming navigation, including notification deep links to `//Training?sport=...` (`AthletePages.xaml.cs:34`) and request fallback navigation to `//Training` (`AthletePages.xaml.cs:26`).

2. **Turn `TrainingPage` into the hub without changing its route identity.** Retain its `TrainingViewModel`, `IQueryAttributable` behavior, sport selection/background, assignment loading, error/retry behavior, and requests navigation. Initially leave unused drill-filter members in `TrainingViewModel`; removing them during the same split creates unnecessary behavioral risk.

3. **Add a routed `TrainingBuilderPage` with a dedicated `TrainingBuilderViewModel`.** The new route is pushed from Training rather than becoming another shell tab. The builder view model should use the same `IAthleteApiService` and `ISportVisualService` conventions and may initially extract/copy the proven drill filtering behavior from `TrainingViewModel`.

4. **Use an explicit workout draft model instead of overloading `Core.Models.Drill`.** A minimal local draft requires a workout name, selected sport/category/subcategory, an ordered collection of draft items, and per-item reps/time. Each draft item references the existing drill and holds builder-only metadata. Total duration should be derived from the ordered items.

5. **Preserve existing Drill Library execution behavior.** Builder drill preview can continue to navigate to `DrillLibraryPage` by `drillId`. Training hub gets a general Drill Library entry using the existing registered route, while the current selected-drill path remains available in the builder.

6. **Do not invent Save Session API behavior.** If no compatible endpoint exists, implement draft state and UI separately from persistence, with Save disabled or explicitly local only according to a subsequent product/API decision. “Save Session” must not be wired to assignment or training-request endpoints because those contracts represent different domain behavior.

7. **Treat Film Room and Calendar as navigation seams, not hidden aliases.** No corresponding MAUI pages/routes currently exist. Hub buttons should only become active when real destinations are added or approved; routing them to Drill Library would misrepresent architecture.

### Why this is the smallest safe split

- The stable public route `//Training` remains unchanged.
- Notification and request navigation remain valid.
- Existing drill and assignment API calls remain unchanged.
- `TrainingRequestsPage` and `DrillLibraryPage` remain intact.
- Builder state is isolated from the singleton legacy `DrillsViewModel` and from hub state.
- The initial split does not require deleting proven filtering code or changing server contracts.
- Cleanup of now-unused builder members in `TrainingViewModel` can follow after route and emulator tests.

## Files That Would Change

No implementation files were changed during this audit. The smallest implementation set would be:

### Add

| File | Purpose |
|---|---|
| `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml` | Separate workout-building UI: workout name, filters, available drills, selected ordered drills, reps/time, duration, Save Session, Start Training. |
| `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml.cs` | Builder route/query handling, responsive layout, and navigation to drill preview/execution where code-behind remains the project convention. |
| `SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs` | Isolated builder draft/filter/selection/order/duration commands using existing services. |
| `SkillBuilderPro.MAUI/Models/TrainingSessionDraft.cs` | UI draft and ordered workout-item metadata; exact filename could differ, but a distinct model is required unless these records are nested in the builder view model. |

### Modify

| File | Smallest required change |
|---|---|
| `SkillBuilderPro.MAUI/Views/TrainingPage.xaml` | Replace embedded builder grid with hub sections/entry points while preserving sport selection, assignments, error/loading state, and background binding. Add Training Builder and existing-destination entry buttons. |
| `SkillBuilderPro.MAUI/Views/AthletePages.xaml.cs` | Keep `TrainingPage` route/query/load behavior; add hub button handlers if commands are not used. A safer follow-up would move `TrainingPage` into its own code-behind file, but that move is not required for the smallest split. |
| `SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs` | Add only hub-facing properties/commands needed for today's training, performance summary, and recent training. Preserve current endpoint calls; postpone removing drill-builder members until verified. |
| `SkillBuilderPro.MAUI/AppShell.xaml.cs` | Register `TrainingBuilderPage` as a pushed route; keep the existing Training shell route unchanged. |
| `SkillBuilderPro.MAUI/MauiProgram.cs` | Register `TrainingBuilderPage` and `TrainingBuilderViewModel` as transient. |

### Conditional, not part of the route split itself

| File area | Condition |
|---|---|
| MAUI API/client contract files | Only if a compatible Save Session/Start Session endpoint is confirmed or added. |
| API controllers/services and Core DTO/domain files | Only if persistent workout sessions do not already have an appropriate contract. This is a separate API feature, not required merely to separate the pages. |
| New `FilmRoomPage` / `CalendarPage` files and registrations | Required only when those product experiences are implemented. No such current pages exist. |

## Navigation Changes

### Preserve

| Existing navigation | Required outcome |
|---|---|
| Shell tab route `//Training` → `TrainingPage` | Remains unchanged (`AppShell.xaml:14-15`). |
| Notification action → `//Training?sport=...` | Remains valid; Training hub consumes sport query as today (`AthletePages.xaml.cs:19,34`). |
| `TrainingRequestsPage` back fallback → `//Training` | Remains valid (`AthletePages.xaml.cs:26`). |
| Training `REQUESTS` → `TrainingRequestsPage` | Remains available from hub (`AthletePages.xaml.cs:21`). |
| Selected drill → `DrillLibraryPage?drillId=...&fromTraining=true` | Moves with builder drill selection/preview or remains as an assignment action; route contract stays intact (`AthletePages.xaml.cs:24`). |

### Add

```text
//Training (shell tab / hub)
    ├── TrainingBuilderPage          new pushed route
    ├── DrillLibraryPage             existing route
    ├── TrainingRequestsPage         existing route
    ├── FilmRoomPage                 future; no current route/page
    └── CalendarPage                 future; no current route/page
```

Register the new builder route following the existing pattern in `AppShell.xaml.cs`:

```csharp
Routing.RegisterRoute(nameof(TrainingBuilderPage), typeof(TrainingBuilderPage));
```

The Training hub entry then uses a relative pushed route:

```csharp
await Shell.Current.GoToAsync(nameof(TrainingBuilderPage));
```

If selected sport should seed the builder, pass it as an encoded query parameter while keeping hub ownership of the selection:

```text
TrainingBuilderPage?sport={encodedSport}
```

The builder should accept that value as initial state; changes inside a pushed builder should not silently rewrite the hub's current state unless that synchronization is explicitly required.

## Verification Plan

1. **Static inventory check**
   - Confirm `//Training` still maps only to `TrainingPage`.
   - Confirm `TrainingBuilderPage` is a registered pushed route, not a second Training tab.
   - Confirm no existing route names were renamed or removed.

2. **Build verification**
   - Build `SkillBuilderPro.MAUI` for `net10.0-android` Debug.
   - Treat warnings separately from errors and record the exact result.

3. **Training hub functional verification**
   - Open the Training shell tab directly.
   - Open it through `//Training?sport=Softball` (and another supported sport) and confirm selection/background initialization.
   - Verify retry/error state, active assignments, completed/recent source, and request navigation against demo and authenticated modes.
   - Verify Back/Exit behavior remains unchanged where retained.

4. **Builder navigation and state verification**
   - Open Training Builder from Training.
   - Confirm initial sport handoff.
   - Change sport/category/subcategory and confirm available drills filter exactly as expected.
   - Add and remove multiple drills; edit reps/time; reorder; confirm total duration recalculates.
   - Navigate back and reopen to test the explicitly chosen draft-lifetime behavior.

5. **Existing route regression verification**
   - Open Training Requests and return to Training.
   - Open Drill Library from the hub.
   - Open a selected builder drill in Drill Library and return without losing draft state if preservation is required.
   - Trigger a Training notification deep link and verify it lands on the hub, not directly in the builder.

6. **Save/Start contract verification**
   - Before enabling Save Session, verify the exact request/response contract and persistence result; do not infer success from UI state.
   - Before enabling Start Training, verify ordered drill execution, current-item progression, completion semantics, and whether assignment completion is affected.
   - Ensure builder drafts are not posted to assignment or training-request endpoints unless the server contract explicitly supports that meaning.

7. **Android emulator visual/interaction verification**
   - Verify hub and builder stacking/background rendering on Android.
   - Verify phone and wider layouts, scrolling, pickers, multi-selection, reorder gestures/buttons, keyboard behavior for workout name/reps/time, and back navigation.
   - Build success alone must not be reported as visual success.

## Conclusion

The present MAUI architecture has one `TrainingPage` that combines a partial Training hub with a partial Training Builder. No separate complete builder exists under another name. The safest minimal split is to preserve `TrainingPage` and `//Training` as the hub, add a routed `TrainingBuilderPage` with isolated draft state, reuse existing athlete API, sport visual, drill-filtering, assignment, request, and Drill Library behavior, and defer any persistence claim until an exact Save Session API contract is verified.
