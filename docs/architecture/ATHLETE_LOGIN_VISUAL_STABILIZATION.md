# Athlete Login and Visual Stabilization

Date: 2026-08-20  
Scope: focused MAUI Android stabilization only. No API/server contracts, credentials, database data, artwork, or video-player architecture were changed.

## Result

BUILD PASS for `SkillBuilderPro.MAUI`, `net10.0-android`, Debug. Compilation produced 40 warnings and 0 errors in 00:02:01.92. Runtime authentication, clipboard behavior, background rendering, and visual quality still require Android emulator verification; this report does not claim runtime or visual success.

## Login stabilization

Files changed:

- `SkillBuilderPro.MAUI/Views/LoginPage.xaml`
- `SkillBuilderPro.MAUI/Views/LoginPage.xaml.cs`

Repository evidence and implementation:

- The shared `LoginPage(IAthleteApiService api, string role)` remains the login surface for Athlete, Coach, Parent, and Administrator.
- Email remains a normal MAUI `Entry` with `Keyboard="Email"`; password remains a normal MAUI `Entry` with `IsPassword="True"`.
- Explicit adjacent `PASTE` controls invoke `Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.Default.GetTextAsync()` for Android-reliable clipboard access.
- Password paste restores masking and the eye icon. The existing show/hide control remains live and updates its semantic description.
- The form is inside a `ScrollView` with bottom space so Android keyboard interaction can scroll controls into view.
- Existing inline error behavior remains, with a darker translucent backing for legibility.
- Exact primary button text is now:
  - Athlete: `ENTER LOCKER ROOM`
  - Coach: `ENTER COACH'S OFFICE`
  - Parent: `ENTER PARENT HUB`
  - Administrator: `ENTER ADMIN CENTER`

## Authentication and routing evidence

- `SkillBuilderPro.MAUI/Services/ApiEndpointResolver.cs` resolves Android emulator Debug traffic to `http://10.0.2.2:5000/` when no preference override is configured.
- `SkillBuilderPro.MAUI/MauiProgram.cs` supplies the resolver result to the athlete API `HttpClient`.
- `SkillBuilderPro.MAUI/Views/ChooseProfilePage.xaml.cs` calls `SelectRole(role)` and constructs the shared `LoginPage` with that role.
- `SkillBuilderPro.MAUI/Views/LoginPage.xaml.cs` calls `IAthleteApiService.LoginAsync(email, password, role)`. Failure text is assigned to the visible `ErrorLabel`; success replaces the root with `ShellFactory.Create(api)`.
- `SkillBuilderPro.MAUI/Services/ShellFactory.cs` sends Athlete to `AppShell` and non-athlete roles to their existing role home.
- Successful service login clears demo mode before storing authenticated state. Demo entry remains Athlete-only. Existing logout/demo-exit mechanisms remain separate.
- No credentials, endpoint contracts, request/response models, or server behavior were changed. Actual credential validation was not executed in this build-only pass.

## Training Builder background

Files inspected/currently involved:

- `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml`
- `SkillBuilderPro.MAUI/ViewModels/TrainingBuilderViewModel.cs`
- `SkillBuilderPro.MAUI/Services/SportVisualService.cs`
- working comparison: `SkillBuilderPro.MAUI/Views/TrainingPage.xaml`

Evidence:

- Builder background remains `Source="{Binding Background}"`, `Aspect="AspectFill"`, full-fill, and has no negative `ZIndex`.
- The translucent overlay now follows the repository's working Training stacking shape with `ZIndex="-1"`; it remains after the background in XAML and before content.
- `TrainingBuilderViewModel` continues to obtain artwork through `ISportVisualService.GetTrainingBackground(...)`.
- A passed supported sport is stored as the requested sport and takes precedence. Otherwise Demo Mode uses `DemoDataService.Sport` (Aubrey's Softball), then an authenticated user's sport, then the existing supported fallback.
- `SelectedSport` changes recalculate `Background`; Softball is not globally hardcoded and no artwork was added.
- Drill WATCH/PREVIEW continues to navigate through the existing `DrillLibraryPage` `drillId` route; no video player or Demo video path was replaced.

The build verifies XAML and bindings compile. Android emulator inspection is still required to confirm the image actually renders behind the overlay and content.

## Notifications background

Files changed:

- `SkillBuilderPro.MAUI/Views/NotificationsPage.xaml`
- `SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs`
- `SkillBuilderPro.MAUI/ViewModels/NotificationsViewModel.Responsive.cs`

Notifications no longer presents a sport training visual. Its image is the existing neutral approved asset `home_background_approved.png`, with no negative image `ZIndex`. The existing overlay/content ordering is preserved. `NotificationsViewModel` no longer depends on `ISportVisualService` solely to select a training background. Dedicated Notifications portrait and landscape creative remains a documented follow-up; no new artwork was created.

## Conservative shared contrast adjustments

`SkillBuilderPro.MAUI/Resources/Styles/Styles.xaml` received only named shared-surface/text adjustments: `GlassPanelStyle`, `GlassCardStyle`, `GlassHeaderStyle`, `EliteSurfaceStyle`, `PageEyebrowStyle`, `PageSubtitleStyle`, and `SecondaryTextStyle`. These increase translucent backing opacity and secondary-text contrast without changing global base typography, replacing backgrounds, baking UI into images, or redesigning unrelated pages. Emulator review is required across Athlete Home, Goals, Training, Training Builder, Notifications, Trophy Room, and Dashboard in portrait and landscape.

## Exact focused git diff

The exact tracked diff was produced with:

```text
git diff -- SkillBuilderPro.MAUI/Views/LoginPage.xaml SkillBuilderPro.MAUI/Views/LoginPage.xaml.cs SkillBuilderPro.MAUI/Views/NotificationsPage.xaml SkillBuilderPro.MAUI/ViewModels/AthleteViewModels.cs SkillBuilderPro.MAUI/ViewModels/NotificationsViewModel.Responsive.cs SkillBuilderPro.MAUI/Resources/Styles/Styles.xaml
```

Focused change records:

```diff
--- LoginPage.xaml
+++ LoginPage.xaml
@@
- <Grid Padding="24">...<Entry x:Name="EmailEntry" .../><Grid ColumnDefinitions="*,52"><Entry x:Name="PasswordEntry" .../>...</Grid>...
+ <Grid><ScrollView><VerticalStackLayout Padding="24,28,24,40" MinimumHeightRequest="620" ...>
+  ...
+  <Grid ColumnDefinitions="*,Auto" ColumnSpacing="8">
+   <Entry x:Name="EmailEntry" Placeholder="Email" Keyboard="Email" .../>
+   <Button Grid.Column="1" Text="PASTE" Clicked="PasteEmailClicked" .../>
+  </Grid>
+  <Grid ColumnDefinitions="*,Auto,52" ColumnSpacing="8">
+   <Entry x:Name="PasswordEntry" Placeholder="Password" IsPassword="True" .../>
+   <Button Grid.Column="1" Text="PASTE" Clicked="PastePasswordClicked" .../>
+   <ImageButton ... Clicked="PasswordVisibilityClicked" .../>
+  </Grid>
+  <Label x:Name="ErrorLabel" ... BackgroundColor="#B0080D13" Padding="10"/>
+  ...
+ </VerticalStackLayout></ScrollView></Grid>

--- LoginPage.xaml.cs
+++ LoginPage.xaml.cs
@@
+using Microsoft.Maui.ApplicationModel.DataTransfer;
@@
-"Parent"=>"ENTER PARENT DASHBOARD" ... "Administrator"=>"ENTER COMMAND CENTER"
+"Parent"=>"ENTER PARENT HUB" ... "Administrator"=>"ENTER ADMIN CENTER"
@@
+async void PasteEmailClicked(...){if(Clipboard.Default.HasText)EmailEntry.Text=await Clipboard.Default.GetTextAsync()??string.Empty;EmailEntry.Focus();}
+async void PastePasswordClicked(...){if(Clipboard.Default.HasText)PasswordEntry.Text=await Clipboard.Default.GetTextAsync()??string.Empty;PasswordEntry.IsPassword=true;PasswordVisibilityButton.Source="eye.svg";PasswordEntry.Focus();}

--- NotificationsPage.xaml
+++ NotificationsPage.xaml
@@
-<Image Source="{Binding Background}" Aspect="AspectFill" Opacity="1" ZIndex="-2" .../>
+<Image Source="home_background_approved.png" Aspect="AspectFill" Opacity="1" ... SemanticProperties.Description="Skill Builder Pro athlete environment"/>

--- AthleteViewModels.cs
+++ AthleteViewModels.cs
@@
-public partial class NotificationsViewModel(IAthleteApiService api,ISportVisualService visuals):LoadableViewModel
+public partial class NotificationsViewModel(IAthleteApiService api):LoadableViewModel

--- NotificationsViewModel.Responsive.cs
+++ NotificationsViewModel.Responsive.cs
@@
-using SkillBuilderPro.MAUI.Services;
-public string Background => visuals.GetTrainingBackground(api.IsDemoMode ? DemoDataService.Sport : api.User?.Sport);
+// Dedicated Notifications portrait/landscape artwork is still required.
+// The page currently owns a neutral approved SBP background directly.

--- TrainingBuilderPage.xaml (currently an untracked Phase 1 addition relative to HEAD)
+++ TrainingBuilderPage.xaml
@@
+<Image x:Name="BuilderBackground" Source="{Binding Background}" Aspect="AspectFill" Opacity="1" ... InputTransparent="True" .../>
+<BoxView Color="#42040910" ZIndex="-1" InputTransparent="True"/>

--- Styles.xaml
+++ Styles.xaml
@@
-GlassPanelStyle BackgroundColor="#54121A26"
+GlassPanelStyle BackgroundColor="#78121A26"
-GlassCardStyle BackgroundColor="#48131D2A"
+GlassCardStyle BackgroundColor="#68131D2A"
-GlassHeaderStyle BackgroundColor="#42101823"
+GlassHeaderStyle BackgroundColor="#70101823"
-PageEyebrowStyle TextColor="#75AEEA"
+PageEyebrowStyle TextColor="#91C4F4"
-PageSubtitleStyle TextColor="#A6B4C5"
+PageSubtitleStyle TextColor="#D2DCE6"
-SecondaryTextStyle TextColor="#A6B4C5"
+SecondaryTextStyle TextColor="#CED8E2"
-EliteSurfaceStyle BackgroundColor="#58121821"
+EliteSurfaceStyle BackgroundColor="#78121821"
```

The working tree already contained broader Training/Training Builder and Athlete usability changes from preceding controlled phases. They were preserved and are visible in `git status`; this pass did not delete or reset them.

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
Time Elapsed 00:02:01.92
```

Warnings are pre-existing deprecation/nullability categories in files such as `VideoPlayerPage.xaml.cs`, `AthletePages.xaml.cs`, generated XAML event bindings, `DrillsViewModel.cs`, `DrillListPage.xaml.cs`, `DrillLibraryPage.xaml.cs`, and `RoleHomePage.cs`. No build errors occurred.

## Required emulator verification

1. On each role login, focus, type, long-press, paste, use explicit PASTE, toggle password visibility, show the keyboard, submit invalid credentials, and confirm exact CTA text.
2. With the API listening on the host at port 5000, authenticate from the Android emulator and confirm role routing plus visible failure behavior when the server is unavailable.
3. Enter Athlete Demo as Aubrey, open Training Builder without a query, and confirm the Softball visual; repeat with a passed non-Softball sport and change the Picker to confirm precedence and live updates.
4. Confirm Builder WATCH/PREVIEW opens the same Demo drill playback flow as Drill Library.
5. Confirm Notifications displays `home_background_approved.png`, not a sport training background, and remains readable in portrait and landscape.
6. Review the named shared styles on the listed Athlete pages for unintended contrast/layout regressions.

