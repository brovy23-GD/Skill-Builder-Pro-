# Cross-Platform Asset and Safe-Zone Audit

Audit date: 2026-08-15

## Resource contract

All files below are included by `SkillBuilderPro.MAUI.csproj` through `MauiImage Include="Resources\Images\**\*"`. Runtime names use the exact lowercase filenames shown. Every filename contains only Android-safe lowercase ASCII letters, digits, and underscores. PNG is supported by Android, iOS, iPadOS, and Windows. The physical base path is `C:\Users\brovy\source\repos\SkillBuilderPro\SkillBuilderPro.MAUI\Resources\Images\`.

MAUI flattens these image resource names at build time. Pages must reference the filename, not the source subdirectory. No approved bitmap was edited during this pass.

## Active major backgrounds

| Page / use | Runtime filename | Exact physical suffix | Pixels | Ratio | Bytes | Phone portrait | Phone landscape / tablet | Desktop | Important and runtime-safe zones |
|---|---|---|---:|---:|---:|---|---|---|---|
| Athlete Home | `home_background_approved.png` | `Backgrounds\home_background_approved.png` | 1672×941 | 1.7779 | 2,413,437 | High crop risk at left/right with `AspectFill`; keep identity and metrics inside the central 88% live-content region | Low/medium crop risk | Low | Preserve central SBP/facility branding; live content uses padded scrolling overlay |
| Create Profile | `create_profile.png` | `Backgrounds\create_profile.png` | 1672×941 | 1.7779 | 2,204,677 | High horizontal crop risk; form no longer follows artwork coordinates | Low/medium | Low | Production environment only; live form stacks on phone, hybrid on tablet, four-column on wide |
| Training (base) | `strength_training.png` | `strength_training.png` | 1672×941 | 1.7779 | 2,424,271 | High horizontal crop risk | Low/medium | Low | Keep center facility visible; live filters and drills stack below 700 DIP |
| Training (Baseball) | `baseball_training.png` | `baseball_training.png` | 1672×941 | 1.7779 | 2,423,181 | High | Low/medium | Low | Center-field environment is safe behind translucent live controls |
| Training (Softball) | `softball_training.png` | `softball_training.png` | 1672×941 | 1.7779 | 2,594,194 | High | Low/medium | Low | Center environment and signage are protected by responsive overlay padding |
| Training (Basketball) | `basketball_training.png` | `basketball_training.png` | 1672×941 | 1.7779 | 1,754,019 | High | Low/medium | Low | Court center is the primary safe zone |
| Training (Football) | `football_training.png` | `football_training.png` | 1672×941 | 1.7779 | 2,371,006 | High | Low/medium | Low | Field center is the primary safe zone |
| Training (Soccer) | `soccer_training.png` | `soccer_training.png` | 1672×941 | 1.7779 | 2,382,629 | High | Low/medium | Low | Field center is the primary safe zone |
| Training (Hockey) | `hockey_training.png` | `hockey_training.png` | 1672×941 | 1.7779 | 1,991,619 | High | Low/medium | Low | Rink center is the primary safe zone |
| Goals | `goals_background_approved.png` | `Backgrounds\goals_background_approved.png` | 1672×941 | 1.7779 | 2,569,269 | High; cards must scroll over central region | Low/medium | Low | Preserve environmental branding; no controls depend on bitmap coordinates |
| Trophy Room | `trophy_room_background_approved.png` | `Backgrounds\trophy_room_background_approved.png` | 1672×941 | 1.7779 | 2,480,528 | High; trophy environment remains atmospheric | Low/medium | Low | Central trophy presentation remains visible; ranks/achievements are live scrolling UI |
| Drill Library | `drill_library.png` | `Backgrounds\drill_library.png` | 1672×941 | 1.7779 | 2,368,780 | High; player uses available width at 16:9 | Low/medium | Low | Central media region; phone has 16-DIP sides and no desktop translation |
| Locker Room | `locker_room_background_approved.png` | `Backgrounds\locker_room_background_approved.png` | 1672×941 | 1.7779 | 2,198,858 | High; door is the central foreground object | Low/medium | Low | Center doorway is safe; dossier becomes a scrolling one-column composition on phone |
| Locker Door | `locker_door_dynamic_approved.png` | `Backgrounds\locker_door_dynamic_approved.png` | 1086×1448 | 0.7500 | 1,797,146 | Low; native portrait asset | Medium in landscape | Medium | Source-coordinate nameplate `(334,94,418,78)` and number plate `(347,690,392,318)`; overlays are children of moving door |
| Choose Experience / Login | `weight_room.png` | `Backgrounds\Roles\weight_room.png` | 1672×941 | 1.7779 | 2,099,994 | High | Low/medium | Keep title/logo in padded center; role cards become one column on phone |
| Administrator | `admin_dashboard.png` | `Backgrounds\Roles\admin_dashboard.png` | 1672×941 | 1.7779 | 1,772,867 | High | Low/medium | Preserve shelving and embedded branding; live command content stays central |
| Coach | `coach_office.png` | `Backgrounds\Roles\coach_office.png` | 1672×941 | 1.7779 | 1,787,761 | High | Low/medium | Central working area is the overlay-safe region |
| Parent | `parent_dashboard.png` | `Backgrounds\Roles\parent_dashboard.png` | 1672×941 | 1.7779 | 2,067,928 | High | Low/medium | Central dashboard area is the overlay-safe region |
| Athlete role | `locker_room.png` | `Backgrounds\Roles\locker_room.png` | 1672×941 | 1.7779 | 1,767,411 | High | Low/medium | Central locker opening remains visible |

## Packaged supporting artwork

| Filename | Exact physical suffix | Pixels | Ratio | Bytes | Current status / safe-zone guidance |
|---|---|---:|---:|---:|---|
| `goals_progress_approved.png` | `Backgrounds\goals_progress_approved.png` | 1122×1402 | 0.8003 | 1,959,711 | Portrait-friendly supporting creative; not the active Goals background |
| `trophy_room_approved.png` | `Backgrounds\trophy_room_approved.png` | 1672×941 | 1.7779 | 2,178,782 | Alternate approved creative; not the active Trophy background |
| `home_training_facility_maui.png` | `Backgrounds\home_training_facility_maui.png` | 1448×1086 | 1.3333 | 2,337,875 | Tablet-oriented alternate; not active Home source |
| `home_training_facility_winforms.png` | `Backgrounds\home_training_facility_winforms.png` | 1672×941 | 1.7779 | 2,797,866 | Desktop alternate; not active in MAUI |
| `calendar_baseball.png` | `Backgrounds\Sports\calendar_baseball.png` | 1672×941 | 1.7779 | 2,423,181 | Packaged sports calendar artwork; wide-image portrait crop risk |
| `calendar_softball.png` | `Backgrounds\Sports\calendar_softball.png` | 1672×941 | 1.7779 | 2,594,194 | Same guidance |
| `calendar_basketball.png` | `Backgrounds\Sports\calendar_basketball.png` | 1672×941 | 1.7779 | 1,754,019 | Same guidance |
| `calendar_football.png` | `Backgrounds\Sports\calendar_football.png` | 1672×941 | 1.7779 | 2,371,006 | Same guidance |
| `calendar_soccer.png` | `Backgrounds\Sports\calendar_soccer.png` | 1672×941 | 1.7779 | 2,382,629 | Same guidance |
| `calendar_hockey.png` | `Backgrounds\Sports\calendar_hockey.png` | 1672×941 | 1.7779 | 1,991,619 | Same guidance |

## Platform and safe-area findings

- Android resource casing and filename safety: PASS.
- iOS PNG compatibility: PASS.
- Android and iOS local HTTP development transport is configured; production must use HTTPS.
- Pages use full-page image layers with `AspectFill`; live layouts no longer depend on wide-background field coordinates on Create Profile.
- Phone portrait is the highest crop-risk mode for all 1672×941 art. The approved sources were preserved; critical live UI stays in padded, scrollable overlays.
- System safe areas are handled by MAUI page layout and scroll padding. Back/Exit remain live top controls on training, goals, trophy, profile dossier, and drill player pages.
- Runtime visual verification was not possible in this environment because ADB could not start/connect. Crop ratings are source/layout audits, not claimed device screenshots.
