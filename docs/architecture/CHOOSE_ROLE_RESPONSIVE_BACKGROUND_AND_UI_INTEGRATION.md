# Choose Your Experience Responsive Background and UI Integration

**Implementation date:** 2026-08-23  
**Result:** PASS

## 1. Purpose

This change integrates the approved Skill Builder Pro Performance Headquarters master as the responsive background family for the entry page and keeps all four authenticated role actions plus the separate Demo Mode action as live application UI. It is isolated from authentication architecture, databases, drills, Training, Training Builder, Athlete Home, and other page families.

## 2. Approved master image

The required master existed, was a readable PNG, and was inspected before any write. Its actual dimensions are **1672 × 941** and its file size was 3,490,056 bytes. The master remained byte-for-byte unchanged after derivative creation.

## 3. Master source SHA-256

`5D81763595A18877403047BB2A236989E0046FF6A69E4DD5B1916132168D26ED`

The same hash was recorded before and after processing.

## 4. Source asset location

Locked master:

`DesignAssets/Backgrounds/ChooseRole/Source/choose_role_master_approved.png`

No master or documentation-only image is packaged by MAUI or WinForms.

## 5. Before-change inventory

| Item | Before implementation |
|---|---|
| MAUI page | `SkillBuilderPro.MAUI/Views/ChooseProfilePage.cs` |
| Startup | `App.CreateWindow` → `NavigationPage` → `ChooseProfilePage` |
| Route | Startup/root page; no Shell route |
| Background | Fixed `weight_room.png` |
| Layout | Code-created `VerticalStackLayout` + wrapping `FlexLayout`; only width `<600` adjustment |
| Role action | `RoleClicked` called `IAthleteApiService.SelectRole(role)` then pushed `LoginPage(api, role)` |
| Post-login navigation | `LoginPage` → `ShellFactory.Create(api)`; Athlete → `AppShell`, Coach/Parent/Administrator → `RoleHomePage` |
| Demo | Existing MAUI Athlete demo action called `EnterDemoMode()` and replaced the window page with `AppShell`; WinForms exposed Demo only inside `LoginForm` |
| Existing descriptions | Outdated copy (`Train. Track. Improve.`, etc.) |
| Responsive visual classifier | Not used by Choose Profile |
| Baked role text in approved master | None |
| WinForms equivalent | `RoleSelectForm`, fixed `weight_room` background and fixed 900×400 selector |

## 6. Responsive asset architecture

The locked master remains under `Source`. Five deterministic crop-to-fill/resize derivatives are under `Production`; the optional contact sheet is under `Reference`. Landscape variants preserve essentially the full composition. Portrait variants use a centered architectural-axis crop, intentionally preserving the floor logo, rear doors, stairs, Performance District, and skyline as far as the target aspect ratio permits. No content-aware fill, generative extension, recoloring, relighting, stretching, or architectural editing was used.

## 7. Five production variants

- `DesignAssets/Backgrounds/ChooseRole/Production/choose_role_phone_portrait.png`
- `DesignAssets/Backgrounds/ChooseRole/Production/choose_role_phone_landscape.png`
- `DesignAssets/Backgrounds/ChooseRole/Production/choose_role_tablet_portrait.png`
- `DesignAssets/Backgrounds/ChooseRole/Production/choose_role_tablet_landscape.png`
- `DesignAssets/Backgrounds/ChooseRole/Production/choose_role_desktop.png`

Documentation contact sheet:

`DesignAssets/Backgrounds/ChooseRole/Reference/choose_role_sizes_overview.png`

## 8. Exact dimensions and canonical filenames

| Variant | Canonical filename | Dimensions |
|---|---|---:|
| Phone Portrait | `choose_role_phone_portrait.png` | 1080 × 1920 |
| Phone Landscape | `choose_role_phone_landscape.png` | 1920 × 1080 |
| Tablet Portrait | `choose_role_tablet_portrait.png` | 1200 × 1920 |
| Tablet Landscape | `choose_role_tablet_landscape.png` | 1920 × 1200 |
| Desktop | `choose_role_desktop.png` | 1672 × 941 |

## 9. Runtime locations

MAUI packages exactly the five production files from `SkillBuilderPro.MAUI/Resources/Images`. WinForms embeds only `SkillBuilderPro.WinForms/Resources/choose_role_desktop.png` through its existing `.resx` resource architecture. The master and overview are not runtime resources.

## 10. Centralized resolver integration

`ISportVisualService` now exposes `GetChooseRoleBackground` overloads for viewport and explicit `VisualDeviceClass`/`VisualOrientation`. It reuses the existing `ClassifyViewport` and canonical variant suffix logic. The page contains no scattered filename switch. Existing Athlete Home, Training, Training Builder, and sport resolver methods were not changed.

## 11. Live UI architecture

`ChooseProfilePage` continues to create one shared live control tree. A background `Image` uses `AspectFill`; a subtle supporting tone layer remains input-transparent; the title, subtitle, four authenticated role cards, role monograms, descriptions, and secondary Demo surface are live MAUI controls. `SizeChanged` maps the existing classifier to five layout states and repositions the same controls. There are no duplicate pages, duplicated role handlers, or `AbsoluteLayout` coordinates.

## 12. Exact live copy

- `CHOOSE YOUR EXPERIENCE`
- `SELECT HOW YOU'LL ENTER SKILL BUILDER PRO`
- `ATHLETE` — `Train. Compete. Elevate.`
- `COACH` — `Lead. Develop. Win.`
- `PARENT` — `Support. Guide. Empower.`
- `ADMINISTRATOR` — `Manage. Oversee. Optimize.`
- `DEMO MODE` — `Explore Skill Builder Pro`

No canonical role-icon asset files existed in MAUI resources. Compact Performance Blue live monogram badges (`A`, `C`, `P`, `AD`) are therefore used instead of inventing raster assets; they remain live, scalable, and screen-reader described.

## 13. Responsive layout states

| State | Live UI arrangement |
|---|---|
| Phone Portrait | Safe-area-aware compact 2×2 authenticated grid plus secondary Demo surface; scroll fallback available |
| Phone Landscape | Compact centered 2×2 authenticated grid plus secondary Demo surface |
| Tablet Portrait | Centered 2×2 authenticated grid plus secondary Demo surface |
| Tablet Landscape | Centered four-card row plus secondary Demo surface; focal corridor remains open above |
| Desktop / Windows | Centered four-card row plus smaller Demo surface in the intended lower foreground |

## 14. Role card styling and interaction states

Cards use translucent near-black graphite, muted silver descriptions, crisp white role names, subtle gunmetal borders, and restrained Performance Blue monogram/focus accents. MAUI retains native Button pressed/focused/disabled behavior. WinForms adds blue keyboard-focus borders and subtle hover elevation without turning entire cards bright blue. No global opaque panel was introduced.

## 15. Demo Mode visual hierarchy

Demo Mode is not modeled as a fifth authenticated role card. It is a smaller centered graphite surface below the four-role group, with a restrained blue outline/monogram, exact `DEMO MODE` title, and exact `Explore Skill Builder Pro` description. Its action does not open Login. MAUI calls the existing `EnterDemoMode()` state method and opens `AppShell`; WinForms reuses the existing Aubrey Rovy Athlete demo identity and opens `MainForm` with its demo flag.

## 16. Accessibility

- One real Button per role preserves logical tab order and keyboard activation.
- Each role Button exposes the role and exact description through semantic/accessibility properties.
- Monogram badges have meaningful role-icon descriptions.
- Touch targets are at least 48 DIP in MAUI.
- Text remains platform-scalable and descriptions permit two lines.
- The root ScrollView provides a small-screen/text-scaling fallback.
- Proportional margins respect the platform content area and avoid negative offsets.
- WinForms buttons provide `AccessibleName`, `AccessibleDescription`, deterministic tab order, and visible focus borders.
- Demo Mode follows Administrator in keyboard order and exposes `Explore Skill Builder Pro. No sign-in required.`

## 17. Complete asset verification matrix

| Variant | Expected | Actual | Production SHA-256 | Runtime SHA-256 | Match | Result |
|---|---:|---:|---|---|:---:|:---:|
| Phone Portrait | 1080×1920 | 1080×1920 | `C9E3469B9EB838FECB80FA662AD5551BC631E3ED5C11DD3E2ABF92D7432CEA42` | `C9E3469B9EB838FECB80FA662AD5551BC631E3ED5C11DD3E2ABF92D7432CEA42` | Yes | PASS |
| Phone Landscape | 1920×1080 | 1920×1080 | `08F2BE6879AFF0BCF61823FC7967C48312B034C186AC5C826A060A12E9EB7550` | `08F2BE6879AFF0BCF61823FC7967C48312B034C186AC5C826A060A12E9EB7550` | Yes | PASS |
| Tablet Portrait | 1200×1920 | 1200×1920 | `431A5CA6C37AB2AEE4F9F1590E7793AC61D313DE2041D361C66E7AD1ED3C404E` | `431A5CA6C37AB2AEE4F9F1590E7793AC61D313DE2041D361C66E7AD1ED3C404E` | Yes | PASS |
| Tablet Landscape | 1920×1200 | 1920×1200 | `303EC7E1572921B8E7020C37887DEE540B58A7FDB7996E1BE49313F4557F8F42` | `303EC7E1572921B8E7020C37887DEE540B58A7FDB7996E1BE49313F4557F8F42` | Yes | PASS |
| Desktop | 1672×941 | 1672×941 | `5D81763595A18877403047BB2A236989E0046FF6A69E4DD5B1916132168D26ED` | `5D81763595A18877403047BB2A236989E0046FF6A69E4DD5B1916132168D26ED` | Yes | PASS |

The WinForms desktop runtime hash is also `5D81763595A18877403047BB2A236989E0046FF6A69E4DD5B1916132168D26ED`.

## 18. Complete responsive UI verification matrix

| Device class | Orientation | Background | Layout | Title | Subtitle | Athlete | Coach | Parent | Administrator | Demo Mode | Focal area preserved | Result |
|---|---|---|---|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| Phone | Portrait | `choose_role_phone_portrait.png` | Phone Portrait 2×2 + Demo | Yes | Yes | Yes | Yes | Yes | Yes | Reachable | Yes, central axis | PASS |
| Phone | Landscape | `choose_role_phone_landscape.png` | Phone Landscape 2×2 + Demo | Yes | Yes | Yes | Yes | Yes | Yes | Reachable | Yes | PASS |
| Tablet | Portrait | `choose_role_tablet_portrait.png` | Tablet Portrait 2×2 + Demo | Yes | Yes | Yes | Yes | Yes | Yes | Visible | Yes, central axis | PASS |
| Tablet | Landscape | `choose_role_tablet_landscape.png` | Tablet Landscape 1×4 + Demo | Yes | Yes | Yes | Yes | Yes | Yes | Visible | Yes | PASS |
| Desktop | Landscape | `choose_role_desktop.png` | Desktop 1×4 + Demo | Yes | Yes | Yes | Yes | Yes | Yes | Visible | Yes | PASS |

Verification covered source structure, classifier mapping, copy, controls, and compiled runtime paths. Physical device screenshot QA is listed under known limitations.

## 19. Complete entry-action matrix

| Entry action | Authenticated? | Existing handler/command | Expected destination | Verified actual code destination | State correct | Result |
|---|:---:|---|---|---|:---:|:---:|
| Athlete | Yes | `RoleClicked` → `SelectRole("Athlete")` → `LoginPage` | Athlete login / experience | Authenticated success → `AppShell` | Yes | PASS |
| Coach | Yes | `RoleClicked` → `SelectRole("Coach")` → `LoginPage` | Coach login / experience | Authenticated success → `RoleHomePage(api, "Coach")` | Yes | PASS |
| Parent | Yes | `RoleClicked` → `SelectRole("Parent")` → `LoginPage` | Parent login / experience | Authenticated success → `RoleHomePage(api, "Parent")` | Yes | PASS |
| Administrator | Yes | `RoleClicked` → `SelectRole("Administrator")` → `LoginPage` | Administrator login / experience | Authenticated success → `RoleHomePage(api, "Administrator")` | Yes | PASS |
| Demo Mode | No | `DemoClicked` → `EnterDemoMode()` | Existing Athlete demonstration | Direct `AppShell`; no Login or authenticated user | Yes | PASS |

`AthleteApiService.LoginAsync` verifies that the authenticated account contains the selected role before storing the token/user/selected role. Demo sets `User = null`, `IsDemoMode = true`, clears bearer/secure token state, and uses `SelectedRole = "Athlete"` only as client demonstration context; it does not assign an ASP.NET Core Identity role. WinForms retains its established display-to-role mapping and now bypasses `LoginForm` for entry-page Demo Mode while reusing its established Aubrey Rovy Athlete demo identity.

## 20. Demo Mode exit verification

| Client/demo entry | Demo state established | Destination | Exit Demo available | Exit clears state | Returns to Choose Experience | Result |
|---|---|---|:---:|:---:|:---:|:---:|
| MAUI `ChooseProfilePage.DemoClicked` | `User=null`, `IsDemoMode=true`, token cleared | `AppShell` Athlete demo | Yes, Home and Builder | Yes, `LogoutAsync()` | Yes, new `ChooseProfilePage` root | PASS |
| WinForms `RoleSelectForm` Demo action | `IsDemoMode=true`, Aubrey Rovy Athlete context | `MainForm(current, true)` | Yes, `EXIT DEMO MODE` | Yes, demo flag is scoped to closed dashboard loop | Yes, outer program loop recreates `RoleSelectForm` | PASS |

Authenticated logout remains a separate path and is not used to enter Demo Mode.

## 21. Build results

| Project / target | Result | Warnings | Errors | Notes |
|---|:---:|---:|---:|---|
| MAUI Windows `net10.0-windows10.0.19041.0` | PASS | 109 | 0 | Existing obsolete API, nullability, and MVVM Toolkit WinRT/AOT warnings |
| MAUI Android `net10.0-android` | PASS | 40 | 0 | Built sequentially with `JavaMaximumHeapSize=2G`; existing obsolete API and nullability warnings |
| WinForms `net10.0-windows` | PASS | 148 | 0 | Existing WindowsBase/WebView2 conflict and project-wide nullable warnings |

No warning emitted by the successful builds was traced to the new Choose Role code.

## 22. Files changed

Added:

- Five production assets under `DesignAssets/Backgrounds/ChooseRole/Production`
- `DesignAssets/Backgrounds/ChooseRole/Reference/choose_role_sizes_overview.png`
- Five matching MAUI runtime assets under `SkillBuilderPro.MAUI/Resources/Images`
- `SkillBuilderPro.WinForms/Resources/choose_role_desktop.png`
- `docs/architecture/CHOOSE_ROLE_RESPONSIVE_BACKGROUND_AND_UI_INTEGRATION.md`

Modified:

- `SkillBuilderPro.MAUI/Services/SportVisualService.cs`
- `SkillBuilderPro.MAUI/Views/ChooseProfilePage.cs`
- `SkillBuilderPro.WinForms/Forms/RoleSelectForm.cs`
- `SkillBuilderPro.WinForms/Program.cs`
- `SkillBuilderPro.WinForms/Properties/Resource1.resx`
- `SkillBuilderPro.WinForms/Properties/Resource1.Designer.cs`
- `docs/architecture/SKILL_BUILDER_PRO_IMAGE_SYSTEM_BLUEPRINT.md`
- `docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md`

The locked source master was read but not modified.

## 23. Known limitations

- No interactive Android emulator, physical phone/tablet, or automated screenshot-diff session was available; verification is visual source inspection, deterministic metadata/hash checks, code-path inspection, and successful compilation.
- The portrait aspect ratios physically exclude much of the side walls. The deterministic crop prioritizes the required center axis rather than reconstructing missing architecture.
- No canonical Athlete/Coach/Parent/Administrator icon files were present. Live accessible monograms were used; future canonical vector icons can replace them without changing role behavior or backgrounds.
- The Android toolchain required a 2 GB Java heap for the successful packaging retry; no project setting was permanently changed.

## 24. Final PASS/FAIL checklist

| Acceptance criterion | Result |
|---|:---:|
| Approved master preserved unchanged and SHA-256 recorded | PASS |
| Five responsive production assets created at exact dimensions | PASS — 5/5 |
| No stretch, regeneration, or creative modification | PASS |
| Master and overview excluded from runtime | PASS |
| Five MAUI runtime assets integrated and hash-matched | PASS |
| Centralized resolver used | PASS |
| Title, subtitle, four authenticated cards, Demo Mode, descriptions, and icons are live UI | PASS |
| Exact approved copy | PASS |
| Authenticated role state/navigation preserved | PASS — 4/4 |
| Demo Mode exact copy and secondary hierarchy | PASS |
| Demo Mode bypasses authenticated Login/Identity | PASS |
| Demo state and Athlete demonstration context preserved | PASS |
| Demo exit returns to Choose Experience separately from logout | PASS |
| Entry actions verified | PASS — 5/5 |
| Five responsive layout states implemented | PASS — 5/5 |
| Accessibility reviewed | PASS |
| No unrelated page/background/auth/data change | PASS |
| MAUI Windows build | PASS |
| MAUI Android build | PASS |
| WinForms build | PASS |
| Documentation complete | PASS |

**Final result: PASS.**
