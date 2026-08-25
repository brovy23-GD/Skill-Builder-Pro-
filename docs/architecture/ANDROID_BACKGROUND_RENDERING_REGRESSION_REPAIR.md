# Android Background Rendering Regression Repair

Date: 2026-08-20  
Scope: emergency MAUI Android background-layer regression only. No API, database, drill, authentication, video, resolver mapping, or artwork changes were made.

## Root cause

The common blocker was the interaction between two changes:

1. `SkillBuilderPro.MAUI/Resources/Styles/Styles.xaml` had a global implicit `Style TargetType="BoxView"` that assigned an opaque theme `BackgroundColor` (`Gray950` in Light theme or `Gray200` in Dark theme).
2. The latest Android-safe stacking pass correctly moved each full-screen overlay above its background Image by removing negative `ZIndex`, but the page overlays set only `BoxView.Color`. They did not explicitly override the inherited `BackgroundColor`.

That made the overlay a full-viewport styled surface above the artwork. The requested smoky `Color` did not neutralize the separate inherited opaque `VisualElement.BackgroundColor`, so the styled backing could paint across the entire page and hide the Image. This affected both static and dynamically bound images, proving it was not a sport resolver or binding-only defect.

Drill Library remained working because it declares its Image first and has no full-screen BoxView overlay above it. ChooseProfile remained working because it uses `ContentPage.BackgroundImageSource` and also has no affected full-screen overlay.

## Repair

- The implicit BoxView style now has `BackgroundColor="Transparent"`.
- Each page-level overlay explicitly uses its intended translucent `BackgroundColor` and `InputTransparent="True"`.
- Each page Image is first and input-transparent.
- Overlay is second with no `ZIndex`.
- Live content is third.
- ScrollView and large content containers are explicitly transparent.
- Local smoky/glass/elite Borders remain local and were not removed.
- No negative `ZIndex` remains in MAUI Views.

Final layer order:

```text
Grid (dark fallback only)
  Image (first, AspectFill, Fill/Fill, InputTransparent)
  BoxView (second, explicit translucent BackgroundColor, InputTransparent)
  ScrollView/Grid live UI (third, transparent)
    local translucent content surfaces only
```

## Working-versus-regressed comparison

| Page | Root | Image source | Static/bound | Child order before repair | Full-screen overlay | Common inherited blocker | Resource evidence |
|---|---|---|---|---|---|---|---|
| Drill Library (working control) | ContentPage → Grid | `drill_library.png` | Static | Image → ScrollView → optional positive-Z overlays | None | None | File exists under MAUI Images/Backgrounds; MauiImage recursive include |
| ChooseProfile (working control) | code-built ContentPage | `BackgroundImageSource="weight_room.png"` | Static page property | Native page background → local content | None | None | File exists under MAUI Images/Backgrounds/Roles |
| Athlete Home | ContentPage → Grid | `home_background_approved.png` | Static | Image → BoxView → ScrollView | `#16050A10` | implicit opaque BoxView `BackgroundColor` | Exists; Image source non-null |
| Training | ContentPage → Grid | `{Binding Background}` | Bound | Image → BoxView → ScrollView | `#14040910` | same | Chicago files exist; VM fallback/source non-null |
| Training Builder | ContentPage → Grid | `{Binding Background}` plus direct assignment | Bound | Image → BoxView → ScrollView | `#42040910` | same | six Builder files exist; VM fallback/source non-null |
| Goals | ContentPage → Grid | `goals_background_approved.png` | Static | Image → BoxView → ScrollView | `#14050A10` | same | Exists; Image source non-null |
| Trophy Room | ContentPage → Grid | `trophy_room_background_approved.png` | Static | Image → BoxView → ScrollView | `#1004090E` | same | Exists; Image source non-null |
| Notifications | ContentPage → Grid | `home_background_approved.png` | Static | Image → BoxView → content Grid | `#18050A10` | same | Exists; Image source non-null |
| Profile / Locker Room | ContentPage → Grid | `locker_room_background_approved.png` | Static | Image → BoxView → content Grid | `#2004090E` | same | Exists; Image source non-null |

All broken roots used a dark fallback `BackgroundColor`; it is behind the first-child Image and was not itself the blocker. No ScrollView had an explicit opaque background, but the repair makes transparency explicit. No shared glass/elite style was applied to a root Grid or full-page ScrollView. `GlassPanelStyle`, `GlassCardStyle`, `GlassHeaderStyle`, and `EliteSurfaceStyle` are applied only to local Borders. Their stronger alpha values were not the common full-screen cause.

## Page-specific verification

### Athlete Home

- Root Grid; valid static `home_background_approved.png`.
- Image first, full-fill, now input-transparent.
- Overlay second, now explicit `BackgroundColor="#16050A10"` and input-transparent.
- ScrollView/stack third and explicitly transparent.
- Local header and metric surfaces do not fill the viewport.

### Training

- `ISportVisualService.GetTrainingPageBackground` remains unchanged:
  - Basketball → `chicago_basketball.png`
  - Football → `chicago_football.png`
  - Baseball → `chicago_baseball.png`
  - Softball → `softball_training_page.png`
  - Soccer → `chicago_soccer.png`
  - Hockey → `chicago_hockey.png`
- All files exist and are bundled by the recursive `MauiImage` item.
- `TrainingViewModel.Background` is initialized and resolved to a non-null logical filename. No resolver bug was found.
- Image/overlay/transparent ScrollView order now matches the known-good architecture.

### Training Builder

- `ISportVisualService.GetTrainingBuilderBackground` remains unchanged:
  - Basketball → `basketball_training.png`
  - Football → `football_training.png`
  - Baseball → `baseball_training.png`
  - Softball → `softball_training.png`
  - Soccer → `soccer_training.png`
  - Hockey → `hockey_training.png`
- All files exist and are MAUI images.
- `Background` starts as `basketball_training.png`, then updates through query/load/sport selection. Binding/direct source paths are non-null.
- Previous lifecycle reapplication remains preserved, but no additional lifecycle callback was added in this repair. The common cover layer—not another missing callback—was repaired.

### Goals, Trophy, Notifications, Profile

- Static source filenames are exact and physically present.
- Trophy retains `Shell.NavBarIsVisible="False"`.
- Notifications retains neutral `home_background_approved.png`.
- Profile retains the locker-room background, foreground locker door, gestures, SIGN OUT, and EXIT DEMO behavior.
- Each overlay now owns its translucent BackgroundColor explicitly and each large content host is transparent.

## Shared-style audit

| Style | Target | Full-page use found? | Finding |
|---|---|---:|---|
| implicit BoxView style | BoxView | Yes, inherited by every page overlay | Root cause; changed from opaque theme background to Transparent |
| GlassPanelStyle | Border | No | Local content backing only |
| GlassCardStyle | Border | No | Local content backing only |
| GlassHeaderStyle | Border | No | Local header backing only |
| EliteSurfaceStyle | Border | No | Local content backing only |
| PageEyebrowStyle | Label | No | Text only |
| PageSubtitleStyle | Label | No | Text only |
| SecondaryTextStyle | Label | No | Text only |

## Source/resource findings

- No background Image.Source was statically null.
- Training and Builder each have a non-null fallback and valid service mapping.
- No referenced filename was invalid or missing.
- No duplicate logical MAUI filename caused the regression.
- No API/data/auth state controls any static page source.
- Image size is controlled by Grid allocation and Fill/Fill; no fixed zero size was introduced.
- The common full-page opaque surface was the inherited BoxView BackgroundColor.

## Files changed

1. `SkillBuilderPro.MAUI/Resources/Styles/Styles.xaml`
2. `SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml`
3. `SkillBuilderPro.MAUI/Views/TrainingPage.xaml`
4. `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml`
5. `SkillBuilderPro.MAUI/Views/GoalsPage.xaml`
6. `SkillBuilderPro.MAUI/Views/TrophyRoomPage.xaml`
7. `SkillBuilderPro.MAUI/Views/NotificationsPage.xaml`
8. `SkillBuilderPro.MAUI/Views/ProfilePage.xaml`
9. `docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md`
10. `docs/architecture/ANDROID_BACKGROUND_RENDERING_REGRESSION_REPAIR.md`

No commit was created.

## Exact focused diff

```diff
--- Resources/Styles/Styles.xaml
+++ Resources/Styles/Styles.xaml
@@
 <Style TargetType="BoxView">
-  <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Gray950}, Dark={StaticResource Gray200}}" />
+  <Setter Property="BackgroundColor" Value="Transparent" />
 </Style>

--- each affected page overlay
+++ each affected page overlay
@@
-<BoxView Color="#..."/>
+<BoxView BackgroundColor="#..." InputTransparent="True"/>
@@
-<ScrollView>
+<ScrollView BackgroundColor="Transparent">
@@
-<VerticalStackLayout ...>
+<VerticalStackLayout BackgroundColor="Transparent" ...>
```

Static pages also add `InputTransparent="True"` and explicit Fill/Fill where absent on the background Image. Notifications/Profile large content Grids explicitly set `BackgroundColor="Transparent"`.

Exact tracked content is available with:

```text
git diff -- SkillBuilderPro.MAUI/Resources/Styles/Styles.xaml SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml SkillBuilderPro.MAUI/Views/TrainingPage.xaml SkillBuilderPro.MAUI/Views/GoalsPage.xaml SkillBuilderPro.MAUI/Views/TrophyRoomPage.xaml SkillBuilderPro.MAUI/Views/NotificationsPage.xaml SkillBuilderPro.MAUI/Views/ProfilePage.xaml docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md
```

`TrainingBuilderPage.xaml` is an untracked Phase 1 file relative to HEAD; its current full content is the exact added-file state.

## Build

Command:

```text
dotnet build SkillBuilderPro.MAUI/SkillBuilderPro.MAUI.csproj -f net10.0-android -c Debug
```

```text
Build succeeded.
40 Warning(s)
0 Error(s)
Time Elapsed 00:01:52.05
```

Warnings are the existing MAUI obsolescence/nullability categories (`DisplayAlert`, `Application.MainPage`, generated event-handler nullability, and the existing nullable dereference in `DrillsViewModel`). No errors occurred.

## Emulator verification

Required manual sequence:

1. Open Athlete Home and confirm `home_background_approved.png` is obviously visible behind the light overlay and local panels.
2. Open Goals, Trophy, Notifications, and Profile and confirm each exact static image.
3. Switch all six Training sports and confirm Chicago mapping.
4. Open Builder for all six sports and confirm functional mapping.
5. Confirm controls remain tappable, Profile locker-door interaction remains intact, and drill/auth/video/logout flows are unchanged.

Build success is not emulator proof; emulator verification remains required.
