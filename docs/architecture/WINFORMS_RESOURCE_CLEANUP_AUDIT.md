# WinForms Resource Cleanup Audit

## Result

The WinForms runtime resource folder was reduced from 60 files to 25 active production files. All retained background art is desktop-oriented. Obsolete and uncertain files were archived without permanent deletion.

## Active runtime resources

All desktop backgrounds are 1672×941 unless noted.

- `choose_role_desktop.png`
- `home_athlete_desktop.png`
- `home_coach_desktop.png`
- `home_parent_desktop.png`
- `home_administrator_desktop.png`
- `login_desktop.png`
- `training_baseball_chicago_desktop.png`
- `training_basketball_chicago_desktop.png`
- `training_football_chicago_desktop.png`
- `training_hockey_chicago_desktop.png`
- `training_soccer_chicago_desktop.png`
- `training_softball_chicago_desktop.png`
- `training_builder_baseball_desktop.png`
- `training_builder_basketball_desktop.png`
- `training_builder_football_desktop.png`
- `training_builder_hockey_desktop.png`
- `training_builder_soccer_desktop.png`
- `training_builder_softball_desktop.png`
- `drill_library_desktop.png`
- `goals_desktop.png`
- `trophy_desktop.png`
- `profile_desktop.png`
- `profile_locker_door.png` (1145×1374)
- `create_profile_desktop.png`
- `sb_pro_logo_button_40x40.png` (40×40)

## Renamed files

- `weight_room.png` → `login_desktop.png`
- `CoachOffice.png` → `home_coach_desktop.png`
- `parentsbackground.png` → `home_parent_desktop.png`
- `AdminDashApproved.png` → `home_administrator_desktop.png`
- `goals_background_approved.png` → `goals_desktop.png`
- `trophy_room_background_approved.png` → `trophy_desktop.png`
- `locker_room_background_approved.png` → `profile_desktop.png`
- `locker_door_dynamic_approved.png` → `profile_locker_door.png`
- `create_profile.png` → `create_profile_desktop.png`

## Drill Library cleanup

The supplied approved 1672×941 master was copied byte-for-byte to:

- `DesignAssets/Backgrounds/DrillLibrary/Source/drill_library_master_approved.png`
- `DesignAssets/Backgrounds/DrillLibrary/Production/drill_library_desktop.png`
- `SkillBuilderPro.WinForms/Resources/drill_library_desktop.png`

Legacy `drill_library.png` and `old_drill_library.png` were archived. Admin and athlete video/player surfaces now obtain the canonical desktop asset through `DesktopVisualResolver`; no old Drill Library resource name remains in active WinForms code.

## Archived resources

Thirty-six files were moved under `DesignAssets/Backgrounds/WinFormsArchive/`:

- DrillLibrary: 2 superseded backgrounds.
- Home: 11 competing/generated Home and administrator variants.
- Training: 6 superseded generic sport/training backgrounds.
- Profile: 3 duplicate/reference packages.
- Misc: 14 unused icons, duplicate music files, portrait/reference photos, ZIP packages, PowerPoint/prompt material, and the misplaced executable.

No archive item is compiled or copied into runtime output.

## Duplicate and orphan findings

- `LockerRoom.png` was byte-identical to the retained profile background and was archived.
- `Resources/Soundtrack 2.mp3`, `Soundtrack.mp3`, and `Theme.mp3` were byte-identical to the active files already under `Music/`; the Resources copies were archived.
- Search, bell, and dropdown icons had no active code references and were archived.
- Portrait personal photos, generated `New Images`, ZIPs, presentation/prompt files, and `SoundtrackCreateProfile.mp3` had no runtime references.
- The executable beneath `SkillBuilderPro_TopBar_Assets` was not an application resource and was archived for review.

## Code and resource changes

- `DesktopVisualResolver` is now the single source for active desktop visual filenames, including Drill Library and the SBP logo.
- Silent embedded legacy fallbacks were removed. A missing canonical desktop file now raises a clear `FileNotFoundException` instead of displaying obsolete art.
- Admin Drill Library, Admin Reports, Video Player, Create Profile, Locker Room, athlete calendar, role selection, Login, Home, Training, Builder, Goals, Trophy, and role dashboards resolve current files.
- `Resource1.resx` and its generated designer were removed after all live dependencies were migrated.
- The project copies the curated top-level PNG runtime set with one consistent MSBuild rule.

## Form/page audit

- Canonical athlete Home is `MainForm`; Demo Athlete uses the same form with demo state.
- Coach, Parent, and Administrator intentionally use their role-specific dashboards.
- `ApiDrillsForm` is active and opened from `MainForm`.
- `LoginCredentialsForm` has no construction/navigation reference and appears dead or superseded by `LoginForm`; it was documented but not deleted.
- `AdminDashboardFrom.cs` is misleadingly named, but contains the active `AdminDashboardForm`; it is not a duplicate form.

## Build

After removing `bin` and `obj`, restore completed successfully and the WinForms project built successfully with 0 errors and 148 existing warnings. Warning families include the WebView2 `WindowsBase` assembly conflict, nullable-reference warnings, event-handler nullability mismatches, and one existing unreachable-code warning. No broken image/resource reference was reported.
