# WinForms Page Architecture Refactor Plan

`MainForm` currently combines shell/navigation, Training Builder, schedules/calendar, goals, drill/video workflows, profile/session and Demo Mode. A giant rewrite is unsafe.

Recommended incremental extraction:

1. Keep `MainForm` as session/navigation container.
2. Extract a read-only `HomePageControl` first, using approved `Resources/home_training_facility_winforms.png` after runtime gate recovery.
3. Extract Profile/Locker, Training, Goals, Calendar, Trophy Room and Drill Library one at a time behind stable interfaces.
4. Preserve `_isDemoMode`, Exit Demo Mode, logout and current navigation during every extraction.
5. Add characterization tests/manual regression checks before moving mutation logic.

No WinForms code was refactored in this run because the mandatory API runtime gate failed. The approved Home asset was copied into the existing Resources architecture without altering its contents; it is not yet wired.
# Phase 6A Finish Update (2026-08-13)

The first safe extraction is complete: Home is implemented as `Controls/HomePageControl.cs` and composed by `MainForm`. Existing navigation/pages remain intact. Further WinForms page extraction is deferred; no broad rewrite was performed.

## Final Completion Update

HomePageControl and Home-first navigation remain intact. The final approved Home is integrated there, Goals is incrementally integrated in its existing tab, and the approved Locker Room plus dynamic door now render in LockerRoomForm while preserving timer movement. The click-to-retract behavior was runtime verified. Trophy integration remains deferred because no safe existing Trophy page exists.
