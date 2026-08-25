# MAUI Phone Portrait Responsive Repair

## Scope

This pass repairs the live MAUI UI layer for narrow portrait phones. It does not regenerate or edit approved background art, change desktop composition, alter authentication, or restore Training Builder card reparenting.

## Implemented architecture

- Shell native navigation headers are disabled so pages use one compact SBP header instead of a second oversized platform header.
- Shell tab colors use the SBP charcoal/white/blue-neutral palette and a restrained translucent background.
- `ResponsiveLayout` centralizes the phone breakpoint and safe bottom content padding.
- Home actions stack below the welcome block on phones; Training, Goals, Trophy, Requests, Notifications, Profile, Drill Library, Video Player, and Builder reserve space above bottom navigation and the home indicator.
- Existing portrait asset routing remains authoritative for Choose Experience, Home, Training, and Training Builder. The baked-in role UI remains excluded from artwork; role selection stays live MAUI controls.
- Shared glass surfaces are less opaque so approved environments remain visible, with restrained strokes for legibility.
- Drill Library actions wrap rather than requiring horizontal scrolling. Its player remains 16:9 in portrait.
- Video Player uses a compact phone header, safe content padding, and a width-derived 16:9 player.
- Training Builder phone portrait remains one vertical workflow. All five cards retain their permanent `CardsGrid` parent; responsive code changes only grid placement/sizing. Results/session viewports are bounded to avoid dominating the phone viewport.

## Asset coverage

Dedicated responsive portrait assets currently exist for Choose Experience, athlete Home, each supported sport's Training page, and each supported sport's Training Builder page. Goals, Trophy, Profile, Requests, Notifications, and Drill Library do not currently have matching dedicated portrait variants. Their existing approved art was preserved rather than substituted or regenerated.

## Build validation

- Windows `net10.0-windows10.0.19041.0`: succeeded with 109 pre-existing warning instances (obsolete MAUI APIs, event-handler nullability, nullable dereference, and MVVM Toolkit WinRT/AOT generator guidance); 0 errors.
- Android `net10.0-android`: XAML/C# compilation completed, then Android packaging failed because `java.exe` exited with code 2; 40 warnings, 1 environment/toolchain error.
- iOS `net10.0-ios`, `ios-arm64`: blocked on Windows. The requested command first fails because repository/platform configuration supplies `PlatformTarget=x64` (`NETSDK1032`). With `PlatformTarget=arm64`, the no-restore build fails because `project.assets.json` has no `net10.0-ios/ios-arm64` target (`NETSDK1047`). A restore and configured Mac/iOS toolchain are required.

## Physical-device QA checklist

Status: not executed; no physical iPhone or paired Mac build host is available in this environment.

- Launch on a notched iPhone in portrait; confirm status bar and home indicator clearances.
- Verify Choose Experience shows clean approved art with only live MAUI role cards and all four roles plus Demo reachable.
- Verify Home header does not wrap or collide; notification and demo-exit actions remain tappable.
- Verify Training background changes by sport and all actions scroll clear of the tab bar.
- Verify Builder for every sport: header fits, filters work, drill results select, Add works, session controls remain reachable, Summary is reachable, and no card disappears after resize/navigation.
- Verify Drill Library metadata, video, wrapped Save/Start/YouTube/Complete actions, playlist navigation, and bottom content.
- Verify Video Player renders 16:9 and Back/Exit/external-open controls remain reachable.
- Verify Goals, Trophy, Profile locker/open dossier, Requests, and Notifications scroll to their final control without tab/home-indicator overlap.
- Repeat key paths on a small Android phone in portrait and with larger accessibility text.

## Remaining blockers

- Required iOS device build/deploy needs a restored `ios-arm64` asset graph, ARM64 platform configuration, Apple signing, and a paired Mac/iPhone environment.
- Dedicated portrait artwork is absent for Goals, Trophy, Profile, Requests, Notifications, and Drill Library.
- Existing build warnings were not broadened into this visual repair; they should be handled separately.
