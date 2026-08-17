# MAUI Training and Drill Library Design Specification

## Direction

Training uses the existing approved six sport/calendar environments. Authenticated drill data comes only from `GET /api/drills`; Demo Mode uses isolated local demonstration data and never calls protected APIs. Sport visuals map to the existing `calendar_basketball.png`, `calendar_football.png`, `calendar_baseball.png`, `calendar_softball.png`, `calendar_soccer.png`, and `calendar_hockey.png` assets. No new graphic is approved or generated.

## Interaction

Training displays live assignments plus a selectable live Drill collection. Drill identity is the API `Id`. The Training Video action navigates to the existing `DrillLibraryPage` with `drillId` and `fromTraining`; it never passes a video URL as authority and does not create a standalone video page. Drill Library resolves the matching API Drill, shows its details/video environment, recognizes validated YouTube watch, short, shorts, and embed identifiers, and retains an external browser fallback. When entered from Training it exposes Back to Training.

## Visual and responsive rules

Use translucent graphite panels, restrained Performance Blue borders, white primary type, and cool-gray secondary type. Preserve the approved background. Use responsive grids/collections and maximum widths instead of fixed desktop coordinates. Loading, empty, failure, and retry surfaces must use friendly text and must never expose raw API/SQL errors.

## Deferred work

Centralized sport-selection mapping and full multi-sport background switching remain incomplete. Generate Schedule remains disabled until an authorized backend scheduling contract is selected. The legacy `VideoPlayerPage` remains in the project for compatibility but is not the Training destination and should be retired only in a separately reviewed cleanup.
# Phase 6A Finish Verification

Training now supports live Sport, Category, Subcategory, and Drill filtering, with sport-specific visuals and retained DrillId navigation. Drill Library is responsive and presents API metadata. Video playback remains dependent on the authoritative API VideoUrl; the demo drill had no URL, so playback was not runtime-verified.

## Final Completion Audit

The six-sport mapping and API-taxonomy filters remain intact. The authenticated catalog exposed three drills and all had VideoUrl. Training still targets the existing DrillLibraryPage with `drillId` and `fromTraining=true`; no duplicate page exists. UI playback and return-state verification remain outstanding.

Final Home controls route to the existing Training shell; the six approved sport environments and existing Drill Library flow remain unchanged.
