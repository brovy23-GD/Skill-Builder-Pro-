# Skill Builder Pro — Master Architecture & Visual UX Blueprint

**Project:** Skill Builder Pro  
**Purpose:** Living architectural, product, and visual-design blueprint for the repository.  
**Status:** Active / evolving  
**Rule:** Update this document as architecture, navigation, visuals, data, testing, or deployment decisions change.

---

## 1. Product Vision

Skill Builder Pro is a multi-platform sports-performance platform built around an elite, premium athletic-training experience.

The product combines:
- athlete development
- coach-assigned training
- drill/video learning
- goals and progression
- achievements and rank
- performance review
- parent visibility
- administrator control
- multi-sport training experiences

The visual identity is part of the product itself. The background creative is the hook; live UI supports the creative rather than covering it.

---

## 2. Current Technical Architecture

### Backend
- ASP.NET Core API
- .NET 10
- EF Core
- SQL persistence
- ASP.NET Core Identity
- JWT authentication
- role-based authorization

### Authenticated Roles
- Athlete
- Parent
- Coach
- Administrator

### Clients
- .NET MAUI
  - Android
  - iOS
  - Windows
- WinForms

### Local Development Endpoints
- WinForms API: `http://localhost:5000`
- MAUI Windows API: `http://127.0.0.1:5000`
- Android Emulator API: `http://10.0.2.2:5000`

### Planned Hosted Architecture
- Azure-hosted ASP.NET Core API
- Azure-hosted SQL/database
- MAUI Android/iOS/Windows clients consuming one stable hosted API
- Local development remains primary environment for fast coding/debugging
- Azure staging becomes cross-device integration environment

---

## 3. Repository Projects

Current solution projects:

- `SkillBuilderPro.API`
- `SkillBuilderPro.Client`
- `SkillBuilderPro.Core`
- `SkillBuilderPro.MAUI`
- `SkillBuilderPro.WinForms`

Primary modern client direction: **.NET MAUI**

---

## 4. Visual Brand Standard

### Core Visual Identity
- deep graphite
- near-black / charcoal
- blackened steel
- dark gunmetal
- Performance Blue as the restrained saturated accent
- brushed silver
- aluminum
- cool gray
- blue-gray
- soft silver-white
- dark stone
- glass
- polished concrete
- carbon fiber
- leather
- metallic inlays

### Lighting
- bright but controlled
- clean reflections
- restrained Performance Blue LED edges
- realistic dimensional environments

### UI Treatment
- technical athletic typography
- smoky transparent panels
- white/silver primary text where appropriate
- darker text where the artwork is naturally bright
- muted blue-gray secondary text
- Performance Blue for active state, progress, focus, and key actions

### Avoid
- generic SaaS dashboard appearance
- giant cards
- excessive neon
- clutter
- baked-in live data
- static names/numbers/ranks/streaks/goals/notifications/buttons inside background art

### Governing Visual Rule
> Never cover a strong Skill Builder Pro environmental focal point simply because there is UI space to fill.

The room, field, court, rink, locker, office, or trophy display is part of the user experience.

---

## 5. Responsive Visual Strategy

The application must support:
- Android phone portrait
- Android phone landscape
- iPhone portrait
- iPhone landscape
- tablet portrait/landscape
- Windows desktop

### Creative Master Targets
- Phone Portrait: `1080 × 1920`
- Tablet Portrait: `1200 × 1920`
- Tablet Landscape: `1920 × 1200`
- Desktop / Landscape: current approved landscape masters remain valid where appropriate

Most current primary creatives are `1672 × 941` landscape and should remain canonical landscape art where approved.

### Important Rule
Do not stretch a landscape creative into portrait.

Create an intentional portrait composition of the same environment:
- same room
- same visual identity
- same lighting
- same materials
- same brand
- different professional composition

### Runtime Layout Rule
MAUI live UI responds to actual viewport and orientation.

Creative selection and UI reflow are separate responsibilities:
1. choose the correct background for form factor/orientation
2. place live UI responsively over the approved safe zones

---

## 6. Visual Composition Framework

Every page should define:

### Hero Zone
The strongest visual area:
- SBP branding
- field/court/rink
- trophy display
- locker
- office architecture
- lighting
- skyline/environment

Keep major UI away from it.

### Live UI Zone
Intentional negative space for:
- titles
- athlete identity
- metrics
- cards
- controls
- progress
- buttons

### Navigation Zone
Predictable placement for:
- Back
- Exit
- Notifications
- Profile
- compact primary navigation

---

# 7. Athlete Experience Blueprint

## 7.1 Athlete Home

### Purpose
Identity + next action + main navigation.

### Must Show
- welcome/name
- current sport
- overall athlete rank
- streak
- next training / next assignment
- notifications

### Main Navigation
- Training
- Goals
- Trophy Room
- Locker Room
- Drill Library
- Film Room
- Calendar

### Exclude From Home
- performance summary
- sport selector
- Training Builder direct button

### Layout Intent
Home should remain the cleanest athlete screen and allow the facility creative to dominate.

---

## 7.2 Training

### Purpose
Central athlete training command page.

### Header Direction
Example:
- `BACK TO THE GRIND, BOBBY`

### Must Show
- current sport
- Change Sport
- today's training
- assignments
- weekly performance summary
- recent training
- streak

### Primary Actions
- Start Training
- Build Session
- Drill Library
- Film Room
- Training Calendar
- Change Sport

### Sport Selection Rule
Sport selection belongs on Training, not Home.

Selected sport should drive:
- Training Page Chicago sport-environment background
- Training Builder functional field/court/rink background
- Drill Library defaults
- relevant categories/subcategories
- sport-specific recommendations

### Training Background Mapping
- Basketball → `chicago_basketball.png`
- Football → `chicago_football.png`
- Baseball → `chicago_baseball.png`
- Softball → `softball_training_page.png` (approved softball field fallback; distinct Chicago creative is still a gap)
- Soccer → `chicago_soccer.png`
- Hockey → `chicago_hockey.png`

Training Page owns premium Chicago sport-environment artwork. It must not use the functional `*_training.png` Builder set.

### Visual Resolver Responsibility

`ISportVisualService.GetTrainingPageBackground(sport)` is the only Training Page resolver. Its responsibility is the Chicago/premium environment set above. Training Page precedence is: explicitly requested/selected sport, current selection, authenticated Athlete sport, Demo sport, supported fallback.

---

## 7.3 Training Builder

### Purpose
Build a custom training session.

### Must Show
- current sport
- workout name
- category
- subcategory / skill
- available drills
- selected drills
- reps/time
- order
- total workout summary

### Actions
- Add Drill
- Remove
- Reorder
- Save Session
- Start Training

### Visual Rule
Training Builder should feel like an elite athletic session-construction environment, not an administrative form.

### Background Rule
Training Builder owns the functional sport field/court/rink artwork packaged in MAUI: `basketball_training.png`, `football_training.png`, `baseball_training.png`, `softball_training.png`, `soccer_training.png`, and `hockey_training.png`. It must not use the Chicago Training Page set.

`ISportVisualService.GetTrainingBuilderBackground(sport)` is the only Athlete Training Builder resolver. Builder precedence is: sport explicitly passed from Training, current Builder selection, authenticated Athlete sport, Demo sport (Aubrey = Softball), supported fallback. Changing Builder sport refreshes category, skill/subcategory, available drills, and visible Builder art without deleting the transient selected-session items.

Save Session and Start Training remain visibly nonpersistent/disabled until a real API contract exists.

---

## 7.4 Drill Library

### Purpose
Browse and learn drills.

### Must Show
- current sport
- category
- subcategory
- level
- search
- drill/video list

### Actions
- Watch
- Save/Favorite
- Add to Training
- Back

### Role
Drill Library = find training drills and instructional video content.

---

## 7.5 Film Room

### Purpose
Review real athlete performance footage and coaching feedback.

### Must Show
- responsive 16:9 video player
- clip title
- date
- athlete/session/game context
- tags
- coach notes / feedback
- related skill

### Actions
- Play
- Previous
- Next Clip
- Add to Training
- Create Goal
- Back

### V1
- playback
- clip metadata
- coach notes
- tags
- related actions

### Future
- slow motion
- frame-by-frame
- drawing tools
- before/after comparison
- AI feedback

### Difference From Drill Library
- Drill Library = learn drills
- Film Room = review performance

---

## 7.6 Goals & Progress

### Purpose
Define and track athlete-development goals.

### Must Show
- overall rank
- rank progress
- active goals
- progress %
- next unlock
- milestones
- recent progress

### Actions
- Add Goal
- Edit Goal
- Complete Goal
- Rank Details

### Design Intent
Motivational, aspirational, less card-heavy.

---

## 7.7 Trophy Room

### Purpose
Celebrate athlete progression and legacy.

### Must Show
- overall rank
- unlocked achievements
- locked achievements
- milestones
- streak records
- recent level-ups
- next unlock

### Actions
- View Achievement
- Rank Details
- Back

### Design Intent
This should be one of the most cinematic pages in the app. UI density should be intentionally lower.

---

## 7.8 Locker Room / Athlete Profile

### Closed Locker State
Live:
- athlete name
- jersey number
- `ENTER LOCKER ROOM`

### Open Locker Room
Must Show:
- athlete name
- sport
- jersey number
- position
- rank
- basic profile
- team
- selected key identity data

### Actions
- Edit Profile
- Athlete Settings
- Back

### Locker Rule
The locker-door interaction is part of the brand experience.

---

## 7.9 Calendar

### Purpose
Understand what is coming.

### Must Show
- month/week/day
- training sessions
- coach assignments
- goals/due dates
- games/events

### Actions
- Month / Week / Day
- Open Training
- Open Assignment
- Back

---

## 7.10 Notifications

### Purpose
Premium activity feed.

### Notification Types
- coach assignment
- goal update
- level/rank progress
- achievement unlock
- system notification

### Actions
- Open
- Mark Read
- Mark All Read
- Back

---

# 8. Athlete Navigation Architecture

```text
HOME

├── TRAINING
│   ├── Start Training
│   ├── Training Builder
│   ├── Drill Library
│   ├── Film Room
│   └── Calendar
│
├── GOALS
│   └── Goal Detail / Rank Progression
│
├── TROPHY ROOM
│   └── Achievement Detail
│
├── LOCKER ROOM
│   └── Athlete Profile
│
├── DRILL LIBRARY
│
├── FILM ROOM
│
├── CALENDAR
│
└── NOTIFICATIONS
```

Direct access from Home to Drill Library, Film Room, and Calendar is intentional even though they also belong inside Training.

---

# 9. Role Login Standard

All authenticated role login screens must use consistent field semantics:

### Fields
- `Email`
- `Password`

### Role-Specific Primary Buttons
- Athlete → `ENTER LOCKER ROOM`
- Coach → `ENTER COACH'S OFFICE`
- Parent → `ENTER PARENT HUB`
- Administrator → `ENTER ADMIN CENTER`

### Secondary
- Back
- Forgot Password (when implemented)

### Android Paste Standard

The shared MAUI Login page uses normal editable `Entry` controls inside a `ScrollView`. Email has a permanent adjacent PASTE control. Password has a permanent adjacent PASTE control and a separate eye control. Both use `Clipboard.Default.GetTextAsync()`; password paste preserves masking and clipboard failures are shown inline. Do not require Android long-press paste as the primary workflow.

Do not use a generic large `LOGIN` CTA when a branded role-specific entry makes the experience stronger.

---

# 10. Coach Experience Blueprint

## Coach Home / Coach's Office

### Purpose
Coach command center.

### Must Show
- coach identity
- athletes/team overview
- pending assignments
- recent athlete activity
- notifications
- quick progress signals

### Primary Actions
- Athletes
- Assign Training
- Athlete Progress
- Goals
- Film Room / Review Clips
- Calendar
- Notifications

### Design Intent
The Coach's Office environment should remain visually prominent.

---

## Coach Athlete Detail

### Must Show
- athlete identity
- sport
- rank/progression
- assignments
- active goals
- recent activity
- relevant film clips

### Actions
- Assign Training
- Review Progress
- View Goals
- Review Film
- Back

---

## Coach Training Builder

### Purpose
Build and assign training to athletes.

### Must Show
Same core builder as athlete Training Builder plus:
- athlete/team selector
- assignment due date
- coach notes

### Actions
- Save Template
- Assign to Athlete
- Assign to Team
- Preview
- Back

---

# 11. Parent Experience Blueprint

## Parent Home

### Purpose
Visibility without coach/admin-level control.

### Must Show
- child/athlete identity
- current sport
- upcoming training
- recent progress
- goals
- assignments
- calendar
- notifications

### Primary Actions
- View Athlete
- Training
- Goals
- Calendar
- Notifications

### Design Intent
Clean and informative with less control density than Coach or Admin.

---

# 12. Administrator Experience Blueprint

## Admin Home / Command Center

### Purpose
System operations.

### Must Show
- users summary
- roles
- drill/content summary
- audit activity
- system status / operational summary

### Primary Actions
- Users
- Roles
- Drill Management
- Audit
- Content / Import
- Back / Logout

---

## Admin Users

### Must Show
- users
- role
- status
- search/filter

### Actions
- View
- Edit
- Role actions

---

## Admin Drill Management

### Must Show
- total drill count
- sport counts
- categories/subcategories
- content validation
- import status

### Actions
- Add
- Edit
- Import
- Validate

---

## Admin Audit

### Must Show
- timestamp
- user
- role
- action
- target/resource
- result

### Actions
- Filter
- View Detail

---

# 13. Choose Experience Screen

### Purpose
Premium branded entry point.

### Must Show
- Skill Builder Pro identity
- Athlete
- Coach
- Parent
- Administrator
- Demo Mode

### Design Intent
Minimal, high-impact, visually memorable.

### Responsive production implementation (2026-08-23)

The Choose Experience screen uses the frozen Skill Builder Pro Performance Headquarters master and all five deterministic responsive variants resolved through the centralized device-class/orientation visual service. `CHOOSE YOUR EXPERIENCE`, its subtitle, the four authenticated role names/descriptions, accessible role monograms, and the secondary `DEMO MODE` / `Explore Skill Builder Pro` action remain live MAUI/WinForms UI. Athlete, Coach, Parent, and Administrator retain the existing role-selection → role-aware Login → authenticated destination contract. Demo Mode is the verified fifth entry action, is not an authenticated role, bypasses Login, preserves the established Athlete demonstration context, and uses an exit-to-Choose-Experience path separate from authenticated logout. All five backgrounds, five layouts, and five entry actions are verified in `docs/architecture/CHOOSE_ROLE_RESPONSIVE_BACKGROUND_AND_UI_INTEGRATION.md`.

---

# 14. Current Background Asset Strategy

Current primary MAUI/WinForms backgrounds are largely `1672 × 941` landscape.

Known major assets include:
- `home_background_approved.png`
- `goals_background_approved.png`
- `trophy_room_background_approved.png`
- `locker_room_background_approved.png`
- `locker_door_dynamic_approved.png`
- `coach_office.png`
- `parent_dashboard_approved.png`
- `admin_command_center_approved.png`
- `drill_library.png`
- `baseball_training.png`
- `basketball_training.png`
- `football_training.png`
- `softball_training.png`
- `soccer_training.png`
- `hockey_training.png`
- `strength_training.png`
- `weight_room.png`

Do not regenerate approved assets unless explicitly decided.

Training Page also packages lowercase Android-safe MAUI copies of the approved Chicago WinForms source art: `chicago_basketball.png`, `chicago_football.png`, `chicago_baseball.png`, `chicago_soccer.png`, and `chicago_hockey.png`. A distinct Chicago Softball master is not currently present; `softball_training_page.png` is the approved existing softball-field fallback. These landscape masters must not be destructively resized into portrait.

Notifications temporarily uses neutral `home_background_approved.png`. Notifications must not use either the Chicago Training Page set or the functional Training Builder set until dedicated Notifications creative is approved.

---

# 15. Known Visual Engineering Finding

On Android, major page background images failed to render when their Image controls used negative `ZIndex`.

Working Drill Library used normal Grid child order without negative ZIndex.

Confirmed controlled fix:
- remove `ZIndex="-2"` from the page background Image
- preserve the BoxView overlay while validating behavior
- Android build and emulator visual verification required

Home was visually confirmed working after this change.
Goals and Trophy Room received the same controlled change and built successfully; visual verification is required/ongoing.

Training Builder requires a deliberately defensive Android image lifecycle: resolve only a bundled MAUI logical filename, assign after `InitializeComponent`/BindingContext, reassign when `Background` changes, and reapply on Loaded, HandlerChanged, and OnAppearing. Its Grid order is background Image, translucent overlay, live content; neither image nor overlay uses negative `ZIndex`.

### Full-Screen Overlay Rule

A full-screen `BoxView` overlay must explicitly set its own translucent `BackgroundColor` and `InputTransparent="True"`. The implicit BoxView style must remain transparent. Do not set only `BoxView.Color` while inheriting an opaque implicit `BackgroundColor`: once the overlay is correctly placed above the Image, that inherited surface can cover the entire artwork. Root/scroll/content containers above page art remain transparent; smoky/glass styles are local content backing only.

### Current Android Portrait Composition Lessons

- Athlete Home: keep identity/notification controls on a local smoky hero surface; use a disciplined 2×2 metric composition; keep Today's Training separate and scroll-reachable.
- Goals: protect the title/subtitle from bright sky with a local header surface; use a 2×2 metric composition and maintain vertical access to completed goals.
- Trophy Room: hide the native Shell navigation bar on this page because the custom title owns identity/navigation; stack rank, trophy hero space, achievements, and milestone groups vertically on phone widths.
- Existing `1672 × 941` masters are temporary landscape crops on portrait. A future portrait-art pass must create intentional same-environment compositions rather than stretch or destructively resize them.

### Authenticated Session Exit Standard

Authenticated Athlete Profile/Locker Room exposes `SIGN OUT`. It calls the shared logout mechanism, which clears `CurrentUser`, selected role, bearer Authorization header, secure token and expiry, leaves Demo Mode false, and returns to `ChooseProfilePage`. Demo uses the separately labeled `EXIT DEMO`/`EXIT DEMO MODE` path. Neither action terminates the app.

---

# 16. Visual Development Workflow

For every major page:

1. Lock page purpose
2. Lock exact information
3. Lock exact buttons/actions
4. Lock navigation
5. Determine actual background dimensions
6. Define hero/protected zones
7. Define live-UI safe zones
8. Create portrait/landscape/tablet composition plan
9. Mock live UI over the creative
10. Approve mockup
11. Implement real MAUI controls
12. Android portrait QA
13. Android landscape QA
14. iPhone portrait QA
15. iPhone landscape QA
16. Tablet QA
17. Windows QA

Background mockups are design blueprints only. Buttons, labels, cards, athlete information, metrics, and live data must remain real MAUI controls.

---

# 17. Data / Drill Library Roadmap

A validated external dataset is live in the Development database with:
- 900 drills
- 6 sports
- 150 per sport
- 180 subcategories
- 5 videos per subcategory
- unique canonical URLs

Verified Development import evidence (2026-08-21): the explicit, Development-only `import-drills` command validated SHA-256 `AA46D3C4923452C8BA87F365D8672F1B9F5C2AB98EFD5595A7BC6F1E2F50D247`, then committed 900 importer-owned drills. Stable identity is `skillbuilderpro-900-v1:{sourceId}` in the nullable, uniquely indexed `Drill.ExternalSourceKey`; EF `Drill.Id` remains the live primary key. The three pre-existing Basketball drills did not satisfy the conservative legacy match and remain untouched, producing 903 total database/API rows: 900 canonical plus 3 legacy.

The canonical distribution is 150 for each of Baseball, Basketball, Football, Hockey, Soccer, and Softball; 180 sport/category/subcategory groups contain exactly 5 canonical drills each. A second identical run inserted 0, updated 0, and left 900 unchanged. All captured non-drill baseline counts remained unchanged. Authenticated Builder uses real API rows; DemoDataService is not a fallback for authenticated content. ADD does not require a playable video, while WATCH does.

Importer requirements:
- do not truncate unrelated data
- preserve Identity users
- preserve goals
- preserve assignments
- preserve progression
- preserve training history
- avoid duplicates
- safe to rerun
- verify database/API/UI count after import

## Development Test Accounts

The standardized Development-only test identities are:

| Email | Role |
|---|---|
| `admin@skillbuilderpro.local` | Administrator |
| `coach@skillbuilderpro.local` | Coach |
| `parent@skillbuilderpro.local` | Parent |
| `athlete1@skillbuilderpro.local` | Athlete |
| `athlete2@skillbuilderpro.local` | Athlete |

Passwords and the JWT signing key are supplied through the API project's .NET User Secrets. They must not be placed in tracked JSON, source, or documentation. Development startup reconciles these accounts only after migrations and Identity roles are current, retaining existing Identity IDs and profiles where practical.

Development verification on 2026-08-21: all five accounts passed `POST /api/auth/login` and authenticated `GET /api/auth/me` role verification. The safe importer is VERIFIED LIVE in Development: 900 canonical importer-owned drills and 3 preserved legacy drills are exposed by `GET /api/drills`.

---

# 18. Testing & Production Readiness

Priority engineering sequence after visual foundation:

1. Finish MAUI visual correctness
2. Finish responsive/orientation asset architecture
3. Finish role-by-role Android QA
4. Maintain and operationally verify the safe 900-drill importer
5. Complete emulator UI verification across all six sports
6. Add xUnit test suite
7. Add GitHub Actions CI
8. Resolve Admin migration
9. Finish Admin client wiring
10. Physical iPhone testing
11. Azure staging deployment
12. Production readiness review
13. Resume/portfolio metrics only after verification

---

# 19. GitHub Project / Roadmap

Recommended GitHub Project:

**Skill Builder Pro Roadmap**

Statuses:
- Backlog
- Ready
- In Progress
- Testing / Verification
- Blocked
- Done

Useful fields:
- Area
- Priority
- Type
- Platform
- Milestone

Suggested areas:
- Athlete
- Coach
- Parent
- Admin
- API
- Database
- MAUI
- WinForms
- Infrastructure
- Design

---

# 20. Documentation Rule

This document is the repository's living architecture and UX blueprint.

Recommended repository path:

`docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md`

Update it whenever a major decision changes:
- architecture
- role/navigation structure
- API/data strategy
- visual standards
- screen layouts
- responsive behavior
- testing
- CI/CD
- deployment
- product roadmap

Do not rely on chat history as the sole source of truth for long-term project architecture.

---

# 21. Responsive Training and Athlete Home Image Architecture

## Desktop client parity rule

MAUI Windows and WinForms are separate client implementations of the same Skill Builder Pro desktop experience. Equivalent pages must share page purpose, canonical desktop creative, live copy, primary actions, navigation intent, brand language, and visual hierarchy. WinForms must not maintain an independent legacy visual architecture; platform-specific implementation differences are acceptable only when they preserve this product contract.

## 21.1 Locked creative standard

Approved artwork is immutable creative. Color, lighting, logos, typography, camo, branding, stadium details, crowds, equipment, sky, and visual composition must not be redesigned or regenerated. Exact-size production derivatives may only use deterministic high-quality resize and the minimum centered crop needed to reach the required canvas.

`DesignAssets` is the permanent source of truth. Original approved images remain beside normalized derivatives and must not be overwritten. Reference boards are documentation assets and must never be packaged as runtime backgrounds.

## 21.2 Production sizes

| Device use | Production canvas |
|---|---:|
| Phone portrait | 1080 × 1920 |
| Phone landscape | 1920 × 1080 |
| Tablet portrait | 1200 × 1920 |
| Tablet landscape | 1920 × 1200 |
| Desktop / Windows | 1672 × 941 |

The production set contains 30 Chicago Training backgrounds and 5 Athlete Home backgrounds. The documentation set contains six Training reference boards and one Athlete Home reference board. The resulting responsive set is 35 runtime images plus 7 documentation-only boards.

## 21.3 Source locations and canonical names

Training source families live under:

`DesignAssets/Backgrounds/Training/<Sport>/Chicago`

Each of Basketball, Football, Baseball, Softball, Soccer, and Hockey uses:

`training_<sport>_chicago_<phone_portrait|phone_landscape|tablet_portrait|tablet_landscape|desktop>.png`

Its documentation board is:

`training_<sport>_chicago_sizes_overview.png`

Athlete Home lives under:

`DesignAssets/Backgrounds/Athlete/Home`

It uses `home_athlete_<variant>.png` plus the documentation-only `home_athlete_sizes_overview.png`.

## 21.4 Runtime resolution

MAUI chooses a visual using page family, active sport, device class, and orientation. Device idiom and platform establish Phone, Tablet, or Desktop; the live viewport establishes orientation. Windows/Desktop always selects the dedicated `1672 × 941` master. Device-specific art selection does not replace responsive layout behavior: safe areas, system bars, cutouts, notches, home indicators, and live viewport geometry remain layout concerns.

Training uses `ISportVisualService.GetTrainingPageBackground(...)`. A valid current/selected sport resolves only to its matching Chicago family. Navigation sport takes precedence through the existing Training selection semantics; changing sport refreshes both content and artwork. An unsupported sport logs a development diagnostic and uses the responsive Athlete Home environment as a neutral, non-Builder fallback rather than silently showing another sport.

Athlete Home shares the same device/orientation classifier through `ISportVisualService.GetAthleteHomeBackground(...)` but remains a separate visual family.

## 21.5 Training and Training Builder separation

Training is the Chicago professional stadium/arena experience. Training Builder remains the functional sport-specific environment selected by `ISportVisualService.GetTrainingBuilderBackground(...)`. The Builder mappings (`basketball_training.png`, `football_training.png`, `baseball_training.png`, `softball_training.png`, `soccer_training.png`, and `hockey_training.png`) are independent and must never be replaced by Chicago assets or used as Training fallback imagery.

## 21.6 Legacy archive strategy

Superseded Chicago runtime files are preserved under:

`DesignAssets/Archive/Training_Legacy_PreResponsive`

They may leave active runtime folders only after repository-wide reference checks prove that canonical responsive assets replace them. Legacy Athlete Home art must remain active while another page, such as Notifications, still uses it. Historical documentation may retain legacy filenames as audit evidence.

## 21.7 Responsive Training Builder integration (2026-08-22)

Training Builder now owns a complete approved six-sport responsive source matrix at `DesignAssets/Backgrounds/Builder`: 36 canonical PNGs comprising 30 production variants and six documentation-only overview boards. The approved creative is frozen.

`ISportVisualService.GetTrainingBuilderBackground(...)` resolves `Page + Sport + Device Class + Orientation` to `training_builder_<sport>_<variant>.png`. `TrainingBuilderViewModel` refreshes the background when the selected sport or viewport changes. MAUI packages all five responsive variants per sport; WinForms embeds only each sport's desktop variant. Overview boards are excluded from both clients.

The earlier `basketball_training.png`, `football_training.png`, `baseball_training.png`, `softball_training.png`, `soccer_training.png`, and `hockey_training.png` runtime resources are retired. Preserved historical copies live under `DesignAssets/Archive/Builder/Legacy_PreResponsive`.

Training remains independent and continues to resolve the Chicago family through `GetTrainingPageBackground(...)`. No Training Builder mapping may resolve a Chicago asset, and no Training mapping may resolve a Builder asset.
