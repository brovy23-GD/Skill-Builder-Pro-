# Skill Builder Pro Training Builder UI Overlay Placement Blueprint

**Status:** AUTHORITATIVE / BLUEPRINT APPROVED FOR IMPLEMENTATION REVIEW  
**Completed:** 2026-08-22  
**Result:** PASS  
**Scope:** Placement architecture only; no UI or image implementation was changed.

## 1. Purpose

Training Builder answers **“What do I want to train?”** Its live controls must feel embedded in the selected performance environment while leaving that environment visually dominant. This document defines normalized overlay zones for MAUI and the equivalent desktop composition for WinForms. It is the source of truth for the next implementation task.

Training remains a separate “What am I doing today?” Chicago visual family. Nothing here changes either resolver.

## 2. Architectural principles

1. Background art is frozen, static creative; all athlete/session information remains live UI.
2. Placement is `Training Builder + Device Class + Orientation`, using the existing `VisualDeviceClass` and `VisualOrientation` classifications.
3. Share one hierarchy and five responsive master layouts across all sports. Apply a sport exception only where image evidence requires it.
4. Use normalized viewport zones, adaptive grids, safe-area padding, maximum widths, and scrolling—not absolute pixel coordinates.
5. Preserve the visual center, SBP branding, architectural transitions, sport equipment, and primary field/court/rink geometry.
6. Use compact local smoked surfaces. Do not globally blur/darken artwork or install a full-screen opaque panel.
7. Level 1 controls remain visible; Levels 2–4 may scroll. A primary action may use a safe-area-aware sticky local action bar on phone.

Coordinates in this document are percentages of the **safe viewport** after system insets. `X/Y` locate the zone’s top-left; `W/H` are maximum zone bounds, not fixed control sizes.

## 3. Existing UI inventory

The current MAUI page is a background Image, translucent full-screen BoxView, and one ScrollView containing a vertical stack of glass/surface Borders. At widths below 700 device-independent units, header actions and three filter Pickers stack; otherwise they use horizontal columns. There is no AbsoluteLayout. The page re-resolves its image on viewport changes.

The current WinForms Training Builder is built programmatically in `MainForm`: a fixed-size left workflow card and a conditionally visible right schedule-preview card, repositioned on resize over the desktop sport background.

### 3.1 MAUI inventory (43 logical elements)

| # | Element | Control type | Current text/purpose | Parent | Placement now | Responsive | Sport-dependent | Dynamic / interactive | Always visible / scroll |
|---:|---|---|---|---|---|---|:---:|---|---|
| 1 | Builder background | Image | Selected performance environment | Root Grid | Fill / AspectFill | Asset changes by viewport | Yes | Dynamic, no | Persistent |
| 2 | Tone overlay | BoxView | `#42040910` contrast veil | Root Grid | Full fill | None | No | No | Persistent |
| 3 | Content host | ScrollView | Scrolls all live content | Root Grid | Full fill | Padding changes | No | Interactive | Persistent host |
| 4 | Header surface | Border | Local header backing | BuilderContent | First stacked card | Header reflows | No | No | First viewport |
| 5 | Page title | Label | `TRAINING BUILDER` | HeaderGrid | Header left/top | Stacks on phone | No | Dynamic style only | Yes |
| 6 | Active sport context | Label | `CURRENT SPORT • {sport}` | Header stack | Under title | Stacks on phone | Yes | Dynamic | Yes |
| 7 | Back | Button | `BACK` | HeaderActions | Header right | Moves below title on phone | No | Interactive | Yes |
| 8 | Exit Demo | Button | `EXIT DEMO` | HeaderActions | Beside Back | Same | No | Dynamic + interactive | Demo only, yes |
| 9 | Error message | Label | API/load error | Error Border | Inline left | None | No | Dynamic | Conditional, near top |
| 10 | Retry | Button | `RETRY` | Error Border | Inline right | None | No | Interactive | Conditional, near top |
| 11 | Session surface | Border | Workout identity group | BuilderContent | Second/third card | Stacked | No | No | Scroll allowed |
| 12 | Session heading | Label | `SESSION` | Session surface | Top | None | No | No | Scroll allowed |
| 13 | Workout-name label | Label | `WORKOUT NAME` | Session surface | Above Entry | None | No | No | Scroll allowed |
| 14 | Workout-name input | Entry | Workout name | Session surface | Full local width | Fluid width | No | Dynamic + interactive | Scroll allowed |
| 15 | Find-drills surface | Border | Discovery workflow | BuilderContent | Stacked card | Fluid | No | No | Scroll allowed |
| 16 | Find-drills heading | Label | `FIND DRILLS` | Find surface | Top | None | No | No | Scroll allowed |
| 17 | Sport selector | Label + Picker | Sport / `Select sport` | FilterGrid | Column 1 | Stacks below 700 | Yes | Dynamic + interactive | Level 1/2; visible early |
| 18 | Category selector | Label + Picker | Category | FilterGrid | Column 2 | Stacks below 700 | Indirectly | Dynamic + interactive | Scroll allowed |
| 19 | Skill selector | Label + Picker | `SUBCATEGORY / SKILL` | FilterGrid | Column 3 | Stacks below 700 | Indirectly | Dynamic + interactive | Scroll allowed |
| 20 | Available-drill list | CollectionView | Selectable drill results | Find surface | Fixed 300 height | Same | Yes | Dynamic + interactive | Internal scroll |
| 21 | Drill empty state | Label | Selection/no-results guidance | CollectionView | EmptyView | Wraps | Yes | Dynamic | Conditional |
| 22 | Result drill name | Label | Drill title | Result Border | Row 1 | Fluid | Yes | Dynamic | Internal scroll |
| 23 | Result skill | Label | Subcategory | Result Border | Row 2 | Fluid | Yes | Dynamic | Internal scroll |
| 24 | Result duration | Label | `ESTIMATED {duration}` | Result Border | Row 3 | Fluid | Yes | Dynamic | Internal scroll |
| 25 | Result Watch | Button | `WATCH` | Result action stack | Row right | May wrap at narrow width | No | Interactive | Internal scroll |
| 26 | Result Add | Button | `ADD` | Result action stack | Row right | May wrap | No | Interactive | Internal scroll |
| 27 | Watch selected | Button | `WATCH SELECTED DRILL` | Find FlexLayout | Below list | Wraps | No | Dynamic + interactive | Scroll allowed |
| 28 | Add selected | Button | `ADD SELECTED DRILL` | Find FlexLayout | Below list | Wraps | No | Dynamic + interactive | Scroll allowed |
| 29 | Your-session surface | Border | Draft workout contents | BuilderContent | Stacked card | Fluid | No | No | Scroll allowed |
| 30 | Session-item list | CollectionView | Draft items / empty text | Session surface | Fixed 390 height | Same | Yes | Dynamic + interactive | Internal scroll |
| 31 | Item order | Label | `#{order}` | Item Border | Header left | Fluid | No | Dynamic | Internal scroll |
| 32 | Item name | Label | Drill name | Item Border | Header remainder | Fluid | Yes | Dynamic | Internal scroll |
| 33 | Reps input | Label + Entry | `REPS` | Item Grid | Left column | Two columns currently | No | Dynamic + interactive | Internal scroll |
| 34 | Time input | Label + Entry | `TIME (MIN)` | Item Grid | Right column | Two columns currently | No | Dynamic + interactive | Internal scroll |
| 35 | Move up | Button | `UP` | Item FlexLayout | Action row | Wraps | No | Interactive | Internal scroll |
| 36 | Move down | Button | `DOWN` | Item FlexLayout | Action row | Wraps | No | Interactive | Internal scroll |
| 37 | Item Watch | Button | `WATCH` | Item FlexLayout | Action row | Wraps | No | Interactive | Internal scroll |
| 38 | Remove item | Button | `REMOVE` | Item FlexLayout | Action row | Wraps | No | Interactive | Internal scroll |
| 39 | Summary surface | Border | Duration and final commands | BuilderContent | Final card | Fluid | No | No | Scroll allowed |
| 40 | Total-duration badge | Label | `{minutes} MIN` | Summary Grid | Right | Fluid | No | Dynamic | Scroll allowed |
| 41 | Duration caption | Label | `TOTAL SESSION DURATION` | Summary surface | Below heading | None | No | No | Scroll allowed |
| 42 | Save Session | Button | Disabled persistence notice | Summary surface | Full width | Fluid | No | Disabled | Scroll allowed |
| 43 | Start Training | Button | Disabled phase notice | Summary surface | Full width | Fluid | No | Disabled | Scroll allowed |

### 3.2 WinForms inventory (10 logical elements)

| # | Element | Control type | Current purpose | Parent | Placement now | Responsive | Dynamic / interactive | Visibility / scroll |
|---:|---|---|---|---|---|---|---|---|
| 44 | Workflow card/title | Panel + Label | `Training Builder` | Training tab | Fixed 400×600, 40 px left | Reposition only | No | Always / tab scroll |
| 45 | Category selector | Label + ComboBox | Training Category | Left card | Fixed vertical slot | None | Dynamic + interactive | Always |
| 46 | Drill-source status | Label | `Drills: API/Offline` | Left card | Under category | None | Dynamic | Always |
| 47 | Drill selector | Label + CheckedListBox | Select max 5 | Left card | Fixed 340×220 | None | Dynamic + interactive | Internal scroll |
| 48 | Training-days selector | Label + ComboBox | Day preset | Left card | Fixed lower slot | None | Interactive | Always |
| 49 | Generate Schedule | Button | Primary workflow action | Left card | Full-width lower action | None | Interactive | Always |
| 50 | Clear Drills | Button | Secondary action | Left card | Bottom left | None | Interactive | Always |
| 51 | Training Video | Button | Secondary action | Left card | Bottom right | None | Interactive | Always |
| 52 | Preview card/title | Panel + Label | `Schedule Preview` | Training tab | Fixed 400×600, right aligned | Reposition only | Dynamic | After generation |
| 53 | Schedule preview | ListBox | Generated schedule text | Right card | Fixed 340×470 | None | Dynamic | Conditional / internal scroll |

**Total inventoried: 53 logical UI elements.** MAUI and WinForms currently implement related but not identical workflows; this blueprint preserves actual control types and does not pretend feature parity already exists.

## 4. UI priority hierarchy

| Level | Current elements | Rule |
|---|---|---|
| Level 1 — Primary | Page title, active sport, Back, Sport Picker, Add selected/result Add, Generate Schedule | Page identity and current decisive action remain immediately reachable; phone primary action may be sticky above the bottom inset. |
| Level 2 — Active workflow | Workout Name, Category Picker, Skill Picker, drill result list, session list, reps/time, training days, checked drill list | Occupies the primary workflow zone and scrolls when necessary. |
| Level 3 — Supporting | Error/Retry, drill skill/duration, source status, total duration, schedule preview | Lives on compact local surfaces; never competes with Level 1. |
| Level 4 — Secondary/optional | Watch actions, Up/Down/Remove, Clear, Training Video, Save/Start disabled notices, Exit Demo | May wrap, collapse into overflow/expander in a future task, or sit later in scroll order. Existing controls remain unchanged until implemented deliberately. |

## 5. Background analysis methodology

All 30 canonical production PNGs were inspected at their actual 1080×1920, 1920×1080, 1200×1920, 1920×1200, or 1672×941 dimensions. Review identified luminance, texture density, branding, equipment, architecture, playing surface, horizon/outdoor transition, and cropping differences. Safe-zone recommendations protect content across `AspectFill`, safe-area insets, and resizing.

General evidence:

- Portrait art places architecture/branding mostly in the upper 15–35%, playing surface in the middle, and equipment/markings in the lower 35%.
- Landscape art places strong branding/architecture across the upper/left band and dense equipment across the lower foreground.
- Bright outdoor views appear upper/right for Football, Baseball, Softball, and Soccer; Hockey adds very bright ice across the center/lower image.
- The center floor/court/rink often carries a primary SBP mark and is a protected creative region, not a panel location.

## 6. Device-class master layout system

### Phone portrait

- Header: compact local glass at top; title, sport, Back.
- Workflow: one-column stack beginning below the hero/brand band; controls use near-full width but leave side art visible.
- Drill/session lists: bounded internal lists inside the page scroll; avoid nested gesture ambiguity by using deliberate height limits.
- Primary Add action: local bottom action strip above device inset while a drill is selected; otherwise remain inline.
- Preserve an open hero window between header and first large list where possible.

### Phone landscape

- Use **controls | environment** with a left workflow rail (38–42% width) and a protected right environment window.
- Header is inside the rail; Level 2 content scrolls within the rail.
- The right 52–58% should receive no persistent panel. Hockey may reverse only if future blue creative proves left-side focal content; current art does not require reversal.

### Tablet portrait

- Compact header across top.
- Use a 44–48% left workflow column and a right visual window for the first screen; later session/summary sections span the lower scroll area.
- Keep filters vertically stacked or 2+1, not a compressed three-column row.

### Tablet landscape

- Use a 40–44% left workflow rail and a 52–56% right environment/session region.
- Discovery and current-session panes may alternate in the rail rather than stacking every card full-width.
- Preview/session information may use a compact bottom-right surface without obscuring the center mark.

### Desktop / Windows

- Use a premium two-rail composition: a left workflow rail and an optional right session/summary rail, preserving the center 34–42% as the hero environment.
- MAUI maximum rail width: approximately 440–520 device-independent units. WinForms equivalent: 24–28% of safe client width per rail, capped near its current 400 logical pixels.
- Do not stretch the phone stack to 1120 across the center. Keep the floor/field/rink focal corridor open.

## 7. Sport-specific safe-zone findings

### Basketball

- Focal/protected: center court, floor SBP mark, right hoop, upper-center wall logo; foreground ladders, balls, ropes, and racks.
- Dark/safer: left wall/upper-left perimeter and localized far-right wall, excluding hoop/signage.
- Placement: standard left rail works; right summary rail must remain narrow and stop above/right equipment rack. Portrait should avoid lower 45% floor equipment.

### Football

- Focal/protected: blue midfield/runway mark, yard geometry, foreground ladders/hurdles/football, left facility SBP wall, bright sky.
- Contrast risk: upper/right sky is bright and unsuitable for unbacked white text.
- Placement: smoked left rail over the shaded facility edge; keep right half open. On portrait, compact header needs strong local backing because sky occupies the upper field.

### Baseball

- Focal/protected: center home-plate lane, outdoor diamond/skyline, batting cage, left/right training equipment, wall branding.
- Safer: dark left wall strip for compact header; side rails only. The central lane must remain clear.
- Placement: desktop two-rail works especially well; right rail requires local backing over bright doorway/field edge.

### Softball

- Focal/protected: central cage/tee, turf plate/circle, lower equipment, upper-left SBP wall, bright right opening.
- Safer: far-left shaded wall for controls; avoid the center cage and lower foreground balls/bags.
- Placement: standard left rail; reduce vertical panel footprint in portrait to retain cage and turf context.

### Soccer

- Focal/protected: large lower-center SBP floor logo, hurdles/ladder/ropes, right mannequin line and ball bin, upper-left brand, open field.
- Highest density: lower 30–45% across nearly the full width.
- Placement: header/controls stay upper-left on local smoke; avoid bottom sticky surfaces wider than 60%. Landscape rail should be slightly narrower than standard.

### Hockey

- Focal/protected: center SBP ice logo, sticks/pucks/cones/hurdles/net across lower half, dark performance bay left, bright outdoor rink/right transition, upper-left athlete/signage.
- Contrast risk: ice is the brightest Builder surface; translucent surfaces need higher opacity and a solid local fallback behind text.
- Placement: left rail works only above/along the dark performance bay; do not float text directly over ice. Phone action strip must be compact and high-opacity. Preserve center ice and net.

## 8. Protected creative regions

| Device layout | Generally protected normalized region | Reason |
|---|---|---|
| Phone portrait | X 12–92%, Y 18–58%; plus X 8–94%, Y 68–96% | Upper/middle facility identity; lower equipment and floor mark |
| Phone landscape | X 43–98%, Y 8–94% | Primary environment window and field/court/rink geometry |
| Tablet portrait | X 48–98%, Y 12–72%; X 8–94%, Y 74–98% | Visual window plus foreground equipment |
| Tablet landscape | X 44–98%, Y 8–94% | Environment and central sports action plane |
| Desktop | X 29–71%, Y 8–96% | Central premium hero corridor and primary branding/playing surface |

These are default “no persistent panel” regions. Small transient focus indicators are acceptable; large Borders, lists, and opaque cards are not.

## 9. Overlay surface rules

- Header/local rail base: graphite/near-black at 68–80% opacity; Hockey and bright outdoor edges may use 78–88%.
- Workflow cards: 62–76%; selected/focused surface up to 82%.
- Secondary metadata: 48–64%, only when WCAG contrast remains adequate.
- Full-screen tone layer: current subtle layer may remain during implementation evaluation, but it must not exceed roughly 18–28% black-equivalent opacity and must never become the primary contrast solution.
- Borders: 1 logical pixel equivalent, cool silver at 16–28% or Performance Blue at 35–55% for active/focus state.
- Blur: optional only when supported and performant, localized to the surface, approximately 8–16 logical-pixel radius; always pair with an opaque-color fallback. Do not depend on blur for readability.
- Radius: restrained and consistent; do not wrap every label in a card.
- Shadows: one subtle depth cue per major surface, not multiple glowing layers.
- White/silver text on dark smoke. Performance Blue signals focus, selection, progress, and primary action—not whole-surface saturation.

## 10. Typography hierarchy

Use a relative type scale with base body size `1.0`. Platform accessibility scaling remains enabled.

| Role | Importance / relative size | Weight / case | Color | Alignment | Lines / truncation |
|---|---|---|---|---|---|
| Page Title | Highest, 1.7–2.0 | Bold, uppercase | Cool white/silver | Start | 1; never truncate normal title |
| Sport Context | High, 1.05–1.2 | Semibold, uppercase label + title-case value | White with blue accent | Start | 1; ellipsis only pathological |
| Section Label | High, 1.15–1.35 | Bold, uppercase | White | Start | 1 |
| Primary Selection | High, 1.0–1.15 | Semibold, mixed/title case | White | Start | 1; ellipsis |
| Secondary Selection | Medium, 0.95–1.05 | Medium | Silver-white | Start | 1; ellipsis |
| Drill Title | High within list, 1.0–1.1 | Bold | White | Start | 2; ellipsis after 2 |
| Supporting Copy | Medium-low, 0.88–0.98 | Regular | Muted silver | Start | 2–3; wrap |
| Metadata | Low, 0.78–0.88 | Medium, uppercase sparingly | Blue-gray/silver | Start | 1; ellipsis |
| Button Text | Action, 0.9–1.0 | Bold, uppercase | White | Center | 1; do not shrink below accessible size |
| Status Text | Contextual, 0.85–0.95 | Semibold | Silver; semantic warning/error color | Start | 2; wrap |

## 11. Button and control placement

| Device | Sport selector | Category / skill | Drill selector | Primary CTA | Secondary actions |
|---|---|---|---|---|---|
| Phone portrait | Immediately after header | Stacked below sport | Bounded list in workflow scroll | Compact sticky bottom action when selection exists; inline fallback | Later in scroll; wrap 2-up where labels fit |
| Phone landscape | Top of left rail | Stacked/compact in rail | Rail internal list | Bottom of left rail | Collapsible/scrolling below primary |
| Tablet portrait | Top-left workflow column | 2+1 or stacked | Below filters in left column | Bottom of workflow column | Session actions in lower spanning section |
| Tablet landscape | Top of left rail | Three controls only if accessible width; otherwise 2+1 | Left rail | Rail footer | Right/bottom session surface |
| Desktop | Left rail under header | Compact vertical or 2+1 | Left rail | Left rail footer | Right session rail; Back in header |

Current controls remain Pickers, Entries, CollectionViews, and Buttons in MAUI; ComboBoxes, CheckedListBox, ListBox, and Buttons in WinForms. Chips, tabs, drawers, and expanders are future recommendations only and require a separate implementation decision.

## 12. Safe-area, edge, and responsive rules

- Horizontal safe margin: `max(platform safe inset, 3.0% viewport width)`; phone portrait may use 3.5–4.5%.
- Vertical top margin: `max(top inset, 2.0% viewport height)` plus navigation/status-bar clearance.
- Bottom action clearance: `max(bottom inset, 2.0% viewport height)`; never overlap Android gesture/navigation or iOS home indicator.
- Keep interactive controls at least one accessible touch target from viewport edges.
- Landscape rails must remain scrollable when usable height is reduced by system chrome or keyboard.
- Keyboard appearance must scroll the focused Entry into view without moving the background asset or changing sport.
- Desktop resize: preserve rail max widths and hero corridor until the existing classifier changes class; do not proportionally enlarge cards indefinitely.
- Use Grid/VisualState/StateTriggers in the implementation phase. Reuse `VisualDeviceClass`/`VisualOrientation`; do not add a second incompatible breakpoint system. The current `<700` UI branch should be replaced or aligned with the central classification in the implementation task.

## 13. Master placement matrix

| Device | Orientation | UI element/group | Zone | X % | Y % | W % | H % | Alignment | Max width | Visibility | Scroll behavior | Notes |
|---|---|---|---|---:|---:|---:|---:|---|---|---|---|---|
| Phone | Portrait | Title + sport | Header | 4 | 3 | 68 | 9 | Start | 72% | Always | Fixed above workflow | Compact glass |
| Phone | Portrait | Back / Exit Demo | Navigation | 74 | 3 | 22 | 9 | End | 22% | Back always; Exit conditional | Fixed | May stack two compact buttons |
| Phone | Portrait | Error / Retry | Status | 4 | 12 | 92 | 8 | Stretch | 92% | Conditional | Page scroll start | Local semantic surface |
| Phone | Portrait | Workout identity | Primary control A | 4 | 13 | 92 | 11 | Stretch | 92% | Always | Page scroll | Error collapses and releases space |
| Phone | Portrait | Sport/category/skill | Primary control B | 4 | 25 | 92 | 25 | Stretch | 92% | Always | Page scroll | One-column stack |
| Phone | Portrait | Drill results | Workflow | 4 | 51 | 92 | 31 | Stretch | 92% | Always | Bounded internal list | Preserve background between surfaces where possible |
| Phone | Portrait | Add selected | Primary action | 4 | 87 | 92 | 9 | Stretch | 92% | Selection only | Sticky above inset / inline fallback | Never cover lower 14% equipment without smoke |
| Phone | Portrait | Session + summary | Support | 4 | 83 | 92 | Auto | Stretch | 92% | After discovery | Page + bounded lists | Begins after drill workflow in document flow |
| Phone | Landscape | Title/sport/nav | Left-rail header | 3 | 4 | 38 | 15 | Start | 42% | Always | Fixed rail header | Compact |
| Phone | Landscape | Filters/workout | Left workflow | 3 | 20 | 38 | 36 | Stretch | 42% | Always | Rail scroll | Controls stacked |
| Phone | Landscape | Drill results | Left workflow | 3 | 57 | 38 | 29 | Stretch | 42% | Always | Internal/rail scroll | Do not expand into hero |
| Phone | Landscape | Primary CTA | Left action | 3 | 87 | 38 | 9 | Stretch | 42% | Contextual | Fixed rail footer | Safe-area aware |
| Phone | Landscape | Environment | Protected creative | 44 | 4 | 53 | 92 | Center | — | Always | None | No persistent panels |
| Tablet | Portrait | Header/nav | Header | 4 | 3 | 92 | 9 | Split | 92% | Always | Fixed | Local glass only |
| Tablet | Portrait | Workout + filters | Left workflow | 4 | 14 | 43 | 42 | Stretch | 48% | Always | Column scroll | 2+1 filter permitted |
| Tablet | Portrait | Environment | Creative window | 50 | 14 | 46 | 50 | Center | — | Always | None | Protect field/court/rink |
| Tablet | Portrait | Drill results | Left workflow | 4 | 57 | 43 | 29 | Stretch | 48% | Always | Internal list | — |
| Tablet | Portrait | Primary CTA | Left action | 4 | 87 | 43 | 8 | Stretch | 48% | Contextual | Fixed/inline | — |
| Tablet | Portrait | Session + summary | Lower support | 50 | 66 | 46 | 29 | Stretch | 48% | Contextual | Internal/page scroll | Use smoke over bright art |
| Tablet | Landscape | Header/nav | Left-rail header | 3 | 4 | 40 | 12 | Start | 44% | Always | Fixed | — |
| Tablet | Landscape | Workout + filters | Left workflow | 3 | 17 | 40 | 31 | Stretch | 44% | Always | Rail scroll | Three columns only if accessible |
| Tablet | Landscape | Drill results | Left workflow | 3 | 49 | 40 | 36 | Stretch | 44% | Always | Internal list | — |
| Tablet | Landscape | Primary CTA | Left action | 3 | 87 | 40 | 9 | Stretch | 44% | Contextual | Fixed rail footer | — |
| Tablet | Landscape | Session/summary | Right support | 69 | 60 | 28 | 35 | Stretch | 32% | Contextual | Internal scroll | Keep center-right visual open |
| Tablet | Landscape | Environment | Protected creative | 45 | 4 | 51 | 54 | Center | — | Always | None | No persistent panel |
| Desktop | Landscape | Header + nav | Header rail | 3 | 4 | 25 | 13 | Start | 480 DIP / 28% | Always | Fixed | Left aligned |
| Desktop | Landscape | Workout + filters | Left workflow | 3 | 18 | 25 | 37 | Stretch | 480 DIP / 28% | Always | Rail scroll | Vertical/2+1 |
| Desktop | Landscape | Drill results | Left workflow | 3 | 56 | 25 | 30 | Stretch | 480 DIP / 28% | Always | Internal list | — |
| Desktop | Landscape | Primary CTA | Left action | 3 | 87 | 25 | 9 | Stretch | 480 DIP / 28% | Contextual | Fixed rail footer | — |
| Desktop | Landscape | Session items | Right workflow | 73 | 18 | 24 | 48 | Stretch | 480 DIP / 26% | Contextual | Internal list | Optional until first item |
| Desktop | Landscape | Summary/actions | Right action | 73 | 68 | 24 | 28 | Stretch | 480 DIP / 26% | Contextual | Fixed/short scroll | — |
| Desktop | Landscape | Environment | Protected hero | 30 | 5 | 40 | 90 | Center | — | Always | None | No opaque coverage |

Normalized regions may flow vertically when conditional blocks appear. They define ownership and maximum footprint, not overlapping absolute canvases.

## 14. Sport exception matrix

| Sport | Device | Zone | Standard works? | Override required? | Reason | Recommended adjustment |
|---|---|---|:---:|:---:|---|---|
| Basketball | All | Left workflow | Yes | No | Left/dark structure supports local glass | Use standard |
| Basketball | Desktop/tablet landscape | Right support | Mostly | Yes | Hoop/rack occupy right foreground | Cap right rail at 22–24%; start below wall signage |
| Football | Portrait | Header | Mostly | Yes | Bright sky reaches upper frame | Raise surface opacity to 78–84% |
| Football | Landscape/desktop | Left workflow | Yes | No | Shaded facility edge | Use standard; protect blue runway |
| Baseball | All landscape | Center hero | Yes | No | Strong central home-plate corridor | Keep two rails and center open |
| Baseball | Portrait | Right visual window | Mostly | Yes | Bright field/sky behind copy | No unbacked text; compact local backing |
| Softball | Portrait | Workflow vertical footprint | Mostly | Yes | Cage and turf context need visibility | Reduce gaps/card padding; avoid tall header |
| Softball | Landscape | Left workflow | Yes | No | Dark left facility surface | Use standard |
| Soccer | All | Bottom action | No | Yes | Dense equipment and large lower logo | Limit sticky bar to workflow rail; never full width |
| Soccer | Landscape/desktop | Left rail width | Mostly | Yes | Equipment density crosses lower-left | Use 36–40% landscape / 23–25% desktop |
| Hockey | All | Surface opacity | No | Yes | Bright ice undermines translucent contrast | Use 78–88% local graphite and solid fallback |
| Hockey | Portrait | Bottom action | Mostly | Yes | Ice logo/equipment dominate lower frame | Compact rail-width action; avoid center |
| Hockey | Landscape/desktop | Center hero | Yes | No | Dark left bay plus open rink supports rails | Preserve center ice/net and bright transition |

**Sport-specific exceptions requiring an adjustment: 9.** These are style/size adjustments inside the five layouts, not separate page implementations.

## 15. MAUI implementation guidance

1. Preserve `BuilderBackground`, source binding, defensive image lifecycle, and resolver calls.
2. Replace the one universal vertical stack with five VisualStates or state-triggered Grid templates driven by the existing device/orientation classification.
3. Keep one semantic control tree where practical; move groups between named grid zones rather than duplicate bindings/commands.
4. Separate Level 1 header/action from the scrolling Level 2–4 workflow on phone.
5. Avoid unbounded nested scrolling. Give drill/session CollectionViews calculated bounds and clear gesture ownership.
6. Convert the current global overlay into the lightest useful tone layer; deliver contrast through local surfaces.
7. Add accessibility ordering, focus traversal, keyboard-safe scrolling, screen-reader descriptions, and minimum touch targets during implementation.
8. Do not change Pickers/buttons or enable disabled phase actions as part of placement work.

## 16. WinForms blueprint

For the 1672×941 desktop family:

- Left workflow rail: X 3%, Y 5%, W 24–27%, H 90%.
- Optional schedule preview rail: X 73–76%, Y 8%, W 21–24%, H 84–87%.
- Protected hero corridor: X 30%, Y 5%, W 40%, H 90%.
- Preserve current ComboBox, CheckedListBox, ListBox, and buttons.
- Replace fixed card sizes/locations in a later task with proportional client-area calculations plus minimum/maximum sizes; keep cards independently scrollable when the window is small.
- Match MAUI desktop materials: translucent graphite, restrained silver border, Performance Blue focus. Do not mimic phone stacking.
- The current `ImageLayout.Stretch` risks visual distortion and should be reviewed in the implementation task; this documentation task does not change it.

## 17. Developer-only visual debug overlay recommendation

Future DEBUG builds may add an opt-in overlay containing:

- normalized zone outlines and names;
- protected-region hatch/outline;
- `VisualDeviceClass` and `VisualOrientation`;
- viewport and safe-area dimensions;
- resolved asset filename and selected sport;
- current layout state and any sport exception.

Implement as a topmost input-transparent MAUI Grid/GraphicsView and a WinForms transparent/debug Panel. Guard with `#if DEBUG` plus a runtime toggle defaulting off. Do not package labels, outlines, or debug commands in Release; never bake them into PNGs.

## 18. Recommended implementation sequence

1. Add a single layout-state projection based on existing visual classification.
2. Create named MAUI zones and move the existing semantic groups without changing commands/control types.
3. Implement phone portrait and landscape; verify keyboard, scrolling, safe areas, and all six sports.
4. Implement tablet portrait and landscape using the same groups.
5. Implement premium MAUI desktop rails and center hero corridor.
6. Align WinForms proportional rails/materials without changing its workflow.
7. Add sport exception styles/widths only after shared layouts pass.
8. Add optional DEBUG overlay.
9. Test all 30 assets at native aspect, resized windows, orientation changes, text scaling, and long drill names.

## 19. Known limitations

- Analysis is based on repository source images and control code; no interactive emulator/device visual session was part of this documentation task.
- Current Hockey creative is the active red-accent rink family. A separately requested blue Hockey refresh was not performed because an unambiguous six-file blue Hockey set was not present in the repository at analysis time.
- MAUI and WinForms workflows differ today; the blueprint aligns visual hierarchy without inventing missing feature parity.
- Exact accessibility contrast ratios must be measured against rendered local surfaces during implementation.

## 20. Final approval checklist

| Item | Result |
|---|:---:|
| Current Training Builder UI inventoried | PASS — 53 elements |
| All 30 active backgrounds reviewed | PASS |
| Five responsive device layouts defined | PASS |
| All six sports evaluated | PASS |
| Safe zones documented | PASS |
| Protected creative zones documented | PASS |
| Text hierarchy documented | PASS |
| Button/control locations documented | PASS |
| Master placement matrix created | PASS |
| Sport exception matrix created | PASS — 9 required adjustments |
| MAUI guidance documented | PASS |
| WinForms guidance documented | PASS |
| No approved background altered | PASS |
| No full UI rewrite performed | PASS |

**Final result: PASS.**

## 21. Implementation status (2026-08-22)

- **Implementation report:** `docs/architecture/TRAINING_BUILDER_UI_OVERLAY_IMPLEMENTATION.md`
- **MAUI:** PASS — one semantic control tree implements all five responsive layout states and the nine approved sport adjustments.
- **WinForms:** PASS — Training Builder uses proportional desktop rails and aspect-preserving `ImageLayout.Zoom` without changing its controls or workflow.
- **Builds:** PASS — MAUI Windows, MAUI Android, and WinForms compile with zero errors. Existing project warnings are documented in the implementation report.
- **Approved deviations:** normalized placement is implemented with responsive Grid ratios, margins, and bounded widths rather than literal coordinates; safe areas use the platform content area plus proportional margins; a visual DEBUG overlay was omitted to avoid unnecessary presentation complexity, with DEBUG-only diagnostic logging retained instead.
