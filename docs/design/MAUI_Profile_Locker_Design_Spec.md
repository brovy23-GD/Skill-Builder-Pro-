# MAUI Profile Locker Design Specification

Use the unchanged existing `LockerRoom.png` concept: a central personal locker door, Athlete display name, tap-to-open animation, then a translucent safe profile summary. Real session shows LOG OUT; Demo shows EXIT DEMO MODE. No private account details or fabricated live values. Implementation deferred at the API runtime gate.
# Phase 6A Finish Verification

The profile starts with a closed central locker and reveals the profile panel through a tap animation. The approved locker visual is classified GOOD. Closed-locker rendering was interactively verified on Windows.

## Final Dynamic Door Design

The closed state uses `locker_door_dynamic_approved.png` over `locker_room_background_approved.png`. DisplayName is a live nameplate overlay; no number is shown unless a real supported value exists. The built-in prompt is not duplicated. The semantic description is “Open your locker and view athlete profile.” Tap preserves the scale/slide/fade profile reveal.
