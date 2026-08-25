# Training Builder UI Overlay Implementation

**Implementation date:** 2026-08-22  
**Result:** PASS

## 1. Purpose

This implementation applies the approved Training Builder overlay blueprint to the existing MAUI and WinForms Training Builder experiences. It preserves the frozen sport artwork, centralized visual resolver, live control types, bindings, commands, and workflow behavior while making placement responsive and protecting each background's central training environment.

## 2. Files changed

- `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml` — shared semantic control tree, local surfaces, accessibility metadata, and bounded lists.
- `SkillBuilderPro.MAUI/Views/TrainingBuilderPage.xaml.cs` — five responsive arrangements driven by the existing visual classification and limited sport exceptions.
- `SkillBuilderPro.MAUI/Services/SportVisualService.cs` — exposes the existing viewport classifier for reuse by the page; resolver rules and filenames are unchanged.
- `SkillBuilderPro.WinForms/Forms/MainForm.cs` — proportional Training Builder rails and aspect-preserving background presentation.
- `docs/architecture/TRAINING_BUILDER_UI_OVERLAY_BLUEPRINT.md` — implementation status only.
- `docs/architecture/SKILL_BUILDER_PRO_IMAGE_SYSTEM_BLUEPRINT.md` — dated implementation reference only.
- `docs/architecture/TRAINING_BUILDER_UI_OVERLAY_IMPLEMENTATION.md` — this evidence report.

No PNG was edited, regenerated, recolored, blurred, darkened, cropped, or renamed.

## 3. Existing behavior preserved

The selected sport, category and skill selection, drill loading/selection, Watch, Add, session list, reps/time editing, move up/down, remove, workout name, duration summary, error/retry, Back, Exit Demo, and viewport updates retain their existing controls, bindings, and commands. Disabled Save Session and Start Training actions remain disabled. Orientation rearrangement moves the same live controls and does not recreate the view model or reset the selected sport.

## 4. Responsive architecture

One MAUI semantic tree is reused. Named cards are moved among shared Grid/StackLayout zones according to `VisualDeviceClass` and `VisualOrientation`, using the classifier already owned by `SportVisualService`. There are no sport-specific pages, duplicate full XAML trees, `AbsoluteLayout`, or arbitrary canvas coordinates. Percentage guidance is represented through proportional Grid columns, margins, bounded rail widths, and viewport-derived list heights.

## 5. Five MAUI layout states

1. **Phone Portrait:** single document-flow workflow with compact header/navigation and a selected-drill-only sticky action inside the safe workflow width.
2. **Phone Landscape:** approximately 40% independently scrolling workflow rail and 60% protected environment.
3. **Tablet Portrait:** approximately 47% workflow rail; the right side remains the creative window with session/summary below.
4. **Tablet Landscape:** approximately 42% workflow, 29% protected center, and a right session rail.
5. **Desktop / Windows:** premium 27/46/27 two-rail composition, with each rail capped at 520 DIP and the center left free of persistent panels.

## 6. Sport-specific exceptions

All nine documented adjustments are represented without separate pages: Basketball's narrow right support rail; Football's stronger phone-portrait backing; Baseball's backed copy and protected home-plate corridor; Softball's compact portrait gaps/padding; Soccer's rail-contained CTA; Soccer's narrower landscape/desktop workflow rail; Hockey's stronger local graphite surfaces; Hockey's compact rail-width phone action; and Hockey's protected center ice/net transition.

## 7. Local surface styling

Graphite local cards use silver/white typography and Performance Blue accents. The global tone layer remains subtle (`#42040910`, approximately 26% alpha) and is not the primary readability mechanism. Default local surfaces are approximately 75% opaque, Hockey surfaces approximately 86%, and Football phone-portrait surfaces approximately 82%. No giant full-screen opaque panel was introduced.

## 8. Typography implementation

The shared tree uses a clear title, sport context, section-label, primary-selection, drill-title, supporting-copy, metadata, button, and status hierarchy. Font sizes remain platform-scalable; truncation is bounded with `MaxLines` instead of shrinking text below accessible sizes.

## 9. Safe-area behavior

The page remains within MAUI's platform content area and adds viewport-proportional outer margins (roughly 3–4%). The phone action is local to the workflow width and uses bottom spacing rather than covering the full screen. Windows chrome is handled by the page client area. No platform-specific negative offsets are used.

## 10. Scrolling behavior

Phone portrait uses one document-flow scroll container. Landscape/tablet/desktop rails scroll independently where needed. Drill and session `CollectionView` heights are bounded from the active viewport, preventing unbounded nested scrolling. Back remains in the fixed header, while the selected-drill Add action remains immediately reachable in phone states and inline elsewhere. The workout-name Entry participates in normal focus traversal and scroll-to-focus behavior.

## 11. Accessibility work

Logical source order follows the workflow. Headings and screen-reader descriptions were added, actionable controls keep at least 44–48 DIP targets, labels identify selection and editing context, and high-contrast local surfaces support readability. Existing keyboard-capable controls were retained. Background refresh remains driven by sport and viewport state, not keyboard appearance.

## 12. WinForms implementation

Training Builder now computes the left workflow and right session/preview cards from the tab client area. The left rail is approximately 25% wide at 3% X/5% Y and 90% high; the right rail is approximately 23% wide at 74% X/8% Y and 86% high, with sensible width/height clamps. Existing ComboBox, CheckedListBox, ListBox, buttons, events, and workflow behavior are unchanged.

## 13. Image scaling behavior

Only the WinForms Training Builder background now uses `ImageLayout.Zoom`, preserving the desktop asset's aspect ratio and permitting letterboxing instead of distortion. Other WinForms page background behavior is unchanged. MAUI continues using the centralized Builder asset resolution and aspect-fill presentation already established by the image system.

## 14. Debug-overlay result

A visual DEBUG overlay was intentionally not added because it would add a second presentation layer and unnecessary page complexity. DEBUG-only diagnostics log the layout state, viewport, selected sport, and resolved Builder asset. Release builds contain no debug UI.

## 15. Complete 30-combination verification matrix

Verification is structural: resolver output, asset presence, layout-state routing, control placement, bounded scrolling, surface rules, and sport persistence paths were inspected and compiled. Emulator/device screenshot QA remains a known limitation.

| Sport | Device Class | Orientation | Background Asset | Layout State | Header Placement | Workflow Placement | Primary CTA Placement | Protected Hero Preserved | Sport Exception Applied | Result | Notes |
|---|---|---|---|---|---|---|---|:---:|---|:---:|---|
| Basketball | Phone | Portrait | `training_builder_basketball_phone_portrait.png` | Phone Portrait | Top local card | Full-width document flow | Local sticky strip | Yes | Standard | PASS | Bounded lists; no center panel |
| Basketball | Phone | Landscape | `training_builder_basketball_phone_landscape.png` | Phone Landscape | Left rail top | Left ~40% rail | Rail-contained | Yes | Standard | Right environment clear |
| Basketball | Tablet | Portrait | `training_builder_basketball_tablet_portrait.png` | Tablet Portrait | Full top | Left ~47% | Left workflow | Yes | Standard | Right creative window retained |
| Basketball | Tablet | Landscape | `training_builder_basketball_tablet_landscape.png` | Tablet Landscape | Left rail top | Left ~42% | Left workflow | Yes | Right support rail | PASS | Hoop/rack corridor avoided |
| Basketball | Desktop | Landscape | `training_builder_basketball_desktop.png` | Desktop | Left rail top | Left ~27% | Left workflow | Yes | Right rail ~23% | PASS | Center hero clear |
| Football | Phone | Portrait | `training_builder_football_phone_portrait.png` | Phone Portrait | Top local card | Full-width document flow | Local sticky strip | Yes | 82% local backing | PASS | Bright sky readability |
| Football | Phone | Landscape | `training_builder_football_phone_landscape.png` | Phone Landscape | Left rail top | Left ~40% rail | Rail-contained | Yes | Standard | PASS | Environment clear |
| Football | Tablet | Portrait | `training_builder_football_tablet_portrait.png` | Tablet Portrait | Full top | Left ~47% | Left workflow | Yes | Standard | PASS | Creative window retained |
| Football | Tablet | Landscape | `training_builder_football_tablet_landscape.png` | Tablet Landscape | Left rail top | Left ~42% | Left workflow | Yes | Standard | PASS | Center field clear |
| Football | Desktop | Landscape | `training_builder_football_desktop.png` | Desktop | Left rail top | Left ~27% | Left workflow | Yes | Standard | PASS | Two-rail composition |
| Baseball | Phone | Portrait | `training_builder_baseball_phone_portrait.png` | Phone Portrait | Backed top card | Backed document flow | Local sticky strip | Yes | Bright-zone backing | PASS | No unbacked copy |
| Baseball | Phone | Landscape | `training_builder_baseball_phone_landscape.png` | Phone Landscape | Left rail top | Left ~40% rail | Rail-contained | Yes | Home-plate corridor | PASS | Right environment clear |
| Baseball | Tablet | Portrait | `training_builder_baseball_tablet_portrait.png` | Tablet Portrait | Full top | Left ~47% | Left workflow | Yes | Home-plate corridor | PASS | Creative window retained |
| Baseball | Tablet | Landscape | `training_builder_baseball_tablet_landscape.png` | Tablet Landscape | Left rail top | Left ~42% | Left workflow | Yes | Home-plate corridor | PASS | Center corridor clear |
| Baseball | Desktop | Landscape | `training_builder_baseball_desktop.png` | Desktop | Left rail top | Left ~27% | Left workflow | Yes | Two rails/corridor | PASS | Home plate protected |
| Softball | Phone | Portrait | `training_builder_softball_phone_portrait.png` | Phone Portrait | Compact top card | Compact document flow | Local sticky strip | Yes | Reduced gaps/padding | PASS | Cage/turf context retained |
| Softball | Phone | Landscape | `training_builder_softball_phone_landscape.png` | Phone Landscape | Left rail top | Left ~40% rail | Rail-contained | Yes | Standard | PASS | Environment clear |
| Softball | Tablet | Portrait | `training_builder_softball_tablet_portrait.png` | Tablet Portrait | Full top | Left ~47% | Left workflow | Yes | Standard | PASS | Creative window retained |
| Softball | Tablet | Landscape | `training_builder_softball_tablet_landscape.png` | Tablet Landscape | Left rail top | Left ~42% | Left workflow | Yes | Standard | PASS | Center turf clear |
| Softball | Desktop | Landscape | `training_builder_softball_desktop.png` | Desktop | Left rail top | Left ~27% | Left workflow | Yes | Standard | PASS | Two-rail composition |
| Soccer | Phone | Portrait | `training_builder_soccer_phone_portrait.png` | Phone Portrait | Top local card | Full-width document flow | Workflow-width sticky | Yes | CTA rail-contained | PASS | No viewport-wide bottom bar |
| Soccer | Phone | Landscape | `training_builder_soccer_phone_landscape.png` | Phone Landscape | Left rail top | Left ~38% rail | Rail-contained | Yes | Narrow landscape rail | PASS | Field environment clear |
| Soccer | Tablet | Portrait | `training_builder_soccer_tablet_portrait.png` | Tablet Portrait | Full top | Left ~47% | Left workflow | Yes | CTA rail-contained | PASS | Creative window retained |
| Soccer | Tablet | Landscape | `training_builder_soccer_tablet_landscape.png` | Tablet Landscape | Left rail top | Left ~38% | Left workflow | Yes | Narrow landscape rail | PASS | Center field clear |
| Soccer | Desktop | Landscape | `training_builder_soccer_desktop.png` | Desktop | Left rail top | Left ~24% | Left workflow | Yes | Narrow desktop rail | PASS | Center hero enlarged |
| Hockey | Phone | Portrait | `training_builder_hockey_phone_portrait.png` | Phone Portrait | 86% graphite card | Backed document flow | Compact workflow strip | Yes | Strong backing/action | PASS | No text floated on ice |
| Hockey | Phone | Landscape | `training_builder_hockey_phone_landscape.png` | Phone Landscape | 86% graphite rail | Left ~40% rail | Compact rail action | Yes | Strong backing/action | PASS | Ice/net transition clear |
| Hockey | Tablet | Portrait | `training_builder_hockey_tablet_portrait.png` | Tablet Portrait | 86% graphite card | Left ~47% | Left workflow | Yes | Strong backing/center | PASS | Bright ice protected |
| Hockey | Tablet | Landscape | `training_builder_hockey_tablet_landscape.png` | Tablet Landscape | 86% graphite rail | Left ~42% | Left workflow | Yes | Strong backing/center | PASS | Net and center ice clear |
| Hockey | Desktop | Landscape | `training_builder_hockey_desktop.png` | Desktop | 86% graphite rail | Left ~27% | Left workflow | Yes | Strong backing/center | PASS | Indoor/outdoor transition clear |

## 16. Build results

| Project / target | Result | Warnings | Errors |
|---|:---:|---:|---:|
| MAUI `net10.0-windows10.0.19041.0` | PASS | 109 | 0 |
| MAUI `net10.0-android` | PASS | 0 | 0 |
| WinForms | PASS | 1 | 0 |

The MAUI Windows warnings are existing obsolete API, nullability, and MVVM Toolkit AOT/WinRT diagnostics. The WinForms warning is the existing `MSB3277` WindowsBase 4.0/5.0 conflict involving the WebView2 prerelease dependency. No task-introduced compiler error or warning was identified.

## 17. Known limitations

- Verification was source-, asset-, and build-based; physical Android/iOS devices and interactive screenshot comparison were not available in this implementation pass.
- MAUI safe-area handling relies on the platform content area plus proportional page margins; no custom per-platform inset service was introduced.
- The approved normalized zones are implemented as responsive ratios and bounded rails, not exact pixel coordinates.
- Existing project warnings were intentionally not cleaned up.

## 18. Final PASS/FAIL checklist

| Acceptance item | Result |
|---|:---:|
| Approved background creative unchanged | PASS |
| Resolver architecture unchanged | PASS |
| Training/Builder separation unchanged | PASS |
| Existing Builder commands/bindings preserved | PASS |
| Phone Portrait layout implemented | PASS |
| Phone Landscape layout implemented | PASS |
| Tablet Portrait layout implemented | PASS |
| Tablet Landscape layout implemented | PASS |
| Desktop layout implemented | PASS |
| 30 combinations verified | PASS |
| Sport-specific exceptions implemented | PASS — 9/9 |
| Center hero corridors preserved | PASS |
| Giant opaque overlay avoided | PASS |
| Local surfaces readable | PASS |
| Safe areas respected | PASS |
| Drill scrolling bounded | PASS |
| Session scrolling bounded | PASS |
| Primary CTA reachable | PASS |
| Accessibility reviewed | PASS |
| WinForms proportional layout implemented | PASS |
| WinForms background no longer distorted | PASS |
| MAUI Windows build passes | PASS |
| MAUI Android build passes | PASS |
| WinForms build passes | PASS |
| Documentation updated | PASS |

**Final result: PASS.**
