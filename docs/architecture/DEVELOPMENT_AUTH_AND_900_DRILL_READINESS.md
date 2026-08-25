# Development Environment, Authentication, Test Account, Role, and 900-Drill Readiness

Date: 2026-08-21  
Scope: Development environment only. No 900-drill import was executed.

## Executive Result

- All five standardized Development accounts pass `POST /api/auth/login` and authenticated `GET /api/auth/me` with the expected single application role.
- Parent login is repaired.
- The API Development database is current through `20260815020507_AddAdministratorAuditLogs`.
- The API Debug build passes with 0 warnings and 0 errors.
- The MAUI `net10.0-android` Debug build passes; C#/XAML compilation and Android APK packaging completed with no build error emitted.
- The live Development database contains 3 drills, all Basketball / Offense.
- The external source and audit companion exist and validate to 900 records, 6 sports, 150 per sport, 180 sport/category/subcategory groups, and 5 videos per group.
- The 900 drills are **NOT YET imported**.
- The environment and authenticated roles are ready for importer development and testing, but the existing importer is not safe to execute. The overall import execution status is therefore **NOT READY** until a transactional deterministic upsert replaces the destructive seeder.

## 1. Parent Login Root Cause and Repair

### Reproduced failure

Before repair, the locked Parent email/password combination returned:

```text
POST http://localhost:5000/api/auth/login
HTTP 401
```

### Exact root cause

The prior startup path called:

```csharp
await DevelopmentExistingAccountResetInitializer.InitializeAsync(
    scope.ServiceProvider);
```

`SkillBuilderPro.API/Data/DevelopmentExistingAccountResetInitializer.cs` only reset Parent when this Boolean configuration key was true:

```csharp
var enabled = configuration.GetValue<bool>($"{section}:ResetPassword");
if (!enabled || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;
```

The actual User Secrets audit found `DevelopmentParent:ResetPassword` missing. A missing Boolean resolves to `false`, so the existing Parent password hash was left unchanged even though a Parent password was configured. The login controller then correctly rejected the stale hash with HTTP 401. There was no evidence of a role, normalized-email, duplicate-user, profile, JWT, or expected-role rejection causing this failure.

A separate startup-readiness blocker was also found: `20260815020507_AddAdministratorAuditLogs` was pending, and `Program.cs` intentionally paused role/account initialization while migrations were pending. The inspected migration only creates `AuditLogs` plus its foreign key and indexes. It was applied explicitly; it did not delete or rewrite existing data.

### Repair

`SkillBuilderPro.API/Data/DevelopmentTestAccountInitializer.cs` now owns all five Development accounts. It:

- runs only when `IHostEnvironment.IsDevelopment()` is true;
- requires Email, Password, and DisplayName from configuration;
- creates a missing account without deleting other users;
- optionally reconciles a configured `PreviousEmail` with `SetEmailAsync` and `SetUserNameAsync`, retaining the Identity user ID;
- compares the configured secret with `CheckPasswordAsync` and resets only when different;
- ensures the one required application role and removes only incorrect roles from `ApplicationRoles.All`;
- preserves an existing `UserProfile` and all relationships;
- creates a profile only when missing and reactivates the standardized Development account if needed;
- never logs or stores the password in tracked source.

`SkillBuilderPro.API/Program.cs` invokes this reconciler after migrations, role initialization, and achievement definition synchronization. The old fragmented initializers remain in source for historical reference but have no runtime caller from `Program.cs`.

Observed ID preservation:

| Account | Result |
|---|---|
| Administrator | Existing Identity user ID 6 retained |
| Coach | Legacy email reconciled; Identity user ID 5 retained |
| Parent | Existing Identity user ID 8 retained |
| Athlete 1 | Account was missing; Identity user ID 18 and matching profile created |
| Athlete 2 | Existing Identity user ID 7 retained |

No Identity user was deleted. No database reset, `EnsureDeleted`, table truncation, or relationship deletion was performed.

## 2. Standard Development Accounts

Passwords are intentionally omitted.

| Email | Required role | Credential source |
|---|---|---|
| `admin@skillbuilderpro.local` | Administrator | .NET User Secrets |
| `coach@skillbuilderpro.local` | Coach | .NET User Secrets |
| `parent@skillbuilderpro.local` | Parent | .NET User Secrets |
| `athlete1@skillbuilderpro.local` | Athlete | .NET User Secrets |
| `athlete2@skillbuilderpro.local` | Athlete | .NET User Secrets |

The prior Coach email configured in User Secrets was `Dacoach@bears.com`. It was safe to reconcile because the target standardized Coach email did not already exist. The reconciler renamed that Identity record through `UserManager`, preserving user ID 5 and its profile/domain relationships.

No unmergeable legacy standardized-role account was identified. Other historical users remain untouched and explain why total role counts exceed the five standardized accounts.

## 3. User Secrets Structure

The active Development key structure is:

```text
DevelopmentAdmin:Email
DevelopmentAdmin:Password
DevelopmentAdmin:DisplayName

DevelopmentCoach:Email
DevelopmentCoach:Password
DevelopmentCoach:DisplayName
DevelopmentCoach:PreviousEmail   (migration aid; may be removed after the rename is established)

DevelopmentParent:Email
DevelopmentParent:Password
DevelopmentParent:DisplayName

DevelopmentAthlete1:Email
DevelopmentAthlete1:Password
DevelopmentAthlete1:DisplayName

DevelopmentAthlete2:Email
DevelopmentAthlete2:Password
DevelopmentAthlete2:DisplayName

Jwt:SigningKey
```

Obsolete `DevelopmentAthlete:*` and `*:ResetPassword` User Secrets were removed. Deterministic comparison through `CheckPasswordAsync` makes reset flags unnecessary and avoids resetting a matching password/security stamp on every startup.

The API project User Secrets ID is declared in `SkillBuilderPro.API/SkillBuilderPro.API.csproj`. `WebApplication.CreateBuilder` loads User Secrets in Development. No `appsettings.Development.json` exists. `SkillBuilderPro.API/appsettings.json` contains an empty `Jwt:SigningKey`; the real Development value remains in User Secrets.

## 4. Authentication API Verification Matrix

Actual endpoint: `POST http://localhost:5000/api/auth/login` followed by `GET http://localhost:5000/api/auth/me` with `Authorization: Bearer <token>`. Tokens were checked for presence but never printed.

| ACCOUNT | LOGIN | `/auth/me` | EXPECTED ROLE | ACTUAL ROLE |
|---|---|---|---|---|
| `admin@skillbuilderpro.local` | PASS — HTTP 200, JWT issued | PASS — HTTP 200 | Administrator | Administrator |
| `coach@skillbuilderpro.local` | PASS — HTTP 200, JWT issued | PASS — HTTP 200 | Coach | Coach |
| `parent@skillbuilderpro.local` | PASS — HTTP 200, JWT issued | PASS — HTTP 200 | Parent | Parent |
| `athlete1@skillbuilderpro.local` | PASS — HTTP 200, JWT issued | PASS — HTTP 200 | Athlete | Athlete |
| `athlete2@skillbuilderpro.local` | PASS — HTTP 200, JWT issued | PASS — HTTP 200 | Athlete | Athlete |

No login or `/auth/me` server exception was observed.

Authentication evidence in `SkillBuilderPro.API/Controllers/AuthController.cs`:

```csharp
var user = await _userManager.FindByEmailAsync(request.Email.Trim());
var signInResult = await _signInManager.CheckPasswordSignInAsync(
    user,
    request.Password,
    lockoutOnFailure: true);
```

An active `UserProfile` is required before a JWT is returned. `/api/auth/me` resolves the JWT subject to the Identity user and again requires an active profile.

## 5. MAUI Role Routing Verification

Repository evidence:

```csharp
// SkillBuilderPro.MAUI/ShellFactory.cs
var role = api.IsDemoMode ? "Athlete" : api.User?.Roles.FirstOrDefault() ?? api.SelectedRole;
if (string.Equals(role, "Athlete", StringComparison.OrdinalIgnoreCase))
    return new AppShell();
return new RoleHomePage(api, role ?? "User");
```

```csharp
// SkillBuilderPro.MAUI/Views/LoginPage.xaml.cs
var result = await api.LoginAsync(..., role);
if (result.Ok)
    Application.Current!.Windows[0].Page = ShellFactory.Create(api);
```

| Login role | Repository destination | Verification |
|---|---|---|
| Athlete | `AppShell`; shell tabs include `Home`, unchanged `//Training`, Goals, Trophy, Profile | Static route PASS; API role PASS |
| Coach | `RoleHomePage(api, "Coach")`; Coach workspace loads teams/assignments/notifications and uses `coach_office.png` | Static route PASS; API role PASS |
| Parent | `RoleHomePage(api, "Parent")`; Parent workspace loads linked athletes/assignments/notifications and uses `parent_dashboard_approved.png` | Static route PASS; API role PASS |
| Administrator | `RoleHomePage(api, "Administrator")`; title is `ADMIN COMMAND CENTER` and background is `admin_command_center_approved.png` | Static route PASS; API role PASS |

No concrete role-routing bug was found, so no role page was redesigned. Emulator destination rendering still requires the role-switch sequence below.

## 6. Sign-Out, Demo Exit, and Session Switching

`SkillBuilderPro.MAUI/Services/AthleteApiService.cs` uses a singleton service instance registered in `MauiProgram.cs`, so login state is shared consistently across pages.

Authenticated logout evidence:

```csharp
public Task LogoutAsync()
{
    User = null;
    IsDemoMode = false;
    SelectedRole = null;
    http.DefaultRequestHeaders.Authorization = null;
    SecureStorage.Default.Remove(TokenKey);
    SecureStorage.Default.Remove(ExpiryKey);
    return Task.CompletedTask;
}
```

`RoleHomePage` and Athlete Profile call `LogoutAsync()` and replace the root page with `NavigationPage(new ChooseProfilePage(api))`. This clears CurrentUser, role, Bearer header, stored access token, stored expiry, Demo state, and the old shell root before another role signs in.

Demo entry independently clears authenticated state and stored tokens. Demo exits are labeled `EXIT DEMO` / `EXIT DEMO MODE`, call the same state-clearing mechanism, and return to Choose Profile without terminating the application. Authenticated Profile displays `SIGN OUT`; Demo displays `EXIT DEMO MODE`.

Static session-switch analysis passes. It does not substitute for emulator execution.

## 7. Development Database Baseline

Aggregate counts were queried read-only through EF Core against the same `SkillBuilderProDb` connection used by the API after reconciliation and before any drill import.

| Entity | Count |
|---|---:|
| Identity users | 18 |
| User profiles | 18 |
| Administrator role memberships | 3 |
| Athlete role memberships | 13 |
| Coach role memberships | 2 |
| Parent role memberships | 1 |
| Drills | 3 |
| Athlete goals | 4 |
| Drill assignments | 11 |
| Assignment recipients | 11 |
| Progress logs / completed training records | 6 |
| Training schedules | 1 |
| Athlete progressions | 1 |
| Athlete skill-progress records | 1 |
| Athlete rank-history records | 1 |
| Athlete skill-level-history records | 1 |
| Athlete achievements | 3 |
| Training requests | 5 |
| Notifications | 3 |
| Notification events | 3 |
| Administrator audit logs | 0 |

Role memberships total 19 across 18 users because at least one historical user has multiple role memberships. The standardized initializer corrects only the five named Development accounts; it does not delete or rewrite unrelated historical users.

## 8. Current Live Drill Verification

```text
GET http://localhost:5000/api/drills
HTTP 200
Total: 3
```

| Sport | Category | Subcategory | Count |
|---|---|---|---:|
| Basketball | Offense | Dribbling | 1 |
| Basketball | Offense | Passing | 1 |
| Basketball | Offense | Shooting | 1 |

Authenticated Training Builder calls this real endpoint through `AthleteApiService`; Demo uses the distinct `api/drills/demo` path. The working authenticated drill/video path was not changed.

The precise historical operation that reduced the database to these three rows is not auditable from current data: `AuditLogs` was only just added and contains zero rows. Repository evidence does show multiple historical destructive loaders capable of replacing drill data. It would be speculation to attribute the current three rows to one of them without an audit record.

## 9. 900-Drill Source Readiness

### Primary source

```text
C:\Users\brovy\OneDrive\Desktop\Skill Builder Pro\drills_seed.json
```

- Exists: YES
- Size: 496,736 bytes
- Modified UTC: `2026-08-16T03:45:27.2832770Z`
- SHA-256: `AA46D3C4923452C8BA87F365D8672F1B9F5C2AB98EFD5595A7BC6F1E2F50D247`
- Records: 900
- Schema: `id, name, sport, category, subCategory, description, difficulty, duration, videoUrl, dateCreated`
- Required-field omissions: 0
- Duplicate IDs: 0
- Video URLs present: 900
- Unique video URLs: 900

Sport distribution:

| Sport | Count |
|---|---:|
| BASEBALL | 150 |
| BASKETBALL | 150 |
| FOOTBALL | 150 |
| HOCKEY | 150 |
| SOCCER | 150 |
| SOFTBALL | 150 |

There are 180 unique sport/category/subcategory groups. Every group contains exactly 5 rows (minimum 5, maximum 5, groups not equal to 5: 0).

Four drill names are duplicated even though IDs and video URLs are unique:

- `15-minute POST-WORKOUT STRETCH for Injury Prevention & Flexibility` — 2 rows, Hockey and Softball
- `Agility Drill using Cones to Improve Speed and Explosive Power for Kids #speedandagility` — 3 Basketball rows
- `Big League Prep Pitching Stride Direction` — 2 Softball rows
- `Elite Speed Training Workout for Athletes | Boost Speed and Agility Fast!` — 2 Football rows

This proves `Name` alone is not a valid import identity.

### Audit companion

```text
C:\Users\brovy\OneDrive\Desktop\Skill Builder Pro\SkillBuilderPro_900_Drills_Audit.json
```

- Exists: YES
- Size: 299,608 bytes
- Modified UTC: `2026-08-16T03:45:27.2966338Z`
- SHA-256: `FB64F1B371939EB582D28F07A5DBEBED3C43801672848D579A6C7D330766BAF2`
- Root schema: `generatedAt, recordCount, uniqueVideoUrlCount, subCategoryCount, videosPerSubCategory, results, correctionCount, correctionVerifiedAtUtc`
- Declared record count: 900
- Declared unique video URLs: 900
- Declared subcategory groups: 180
- Declared videos per group: 5
- Declared corrections: 21

The expected folder and repository were searched recursively for the two expected filenames. Only these primary copies were found; no duplicate/stale copy with the same expected filename was found.

The repository does contain `SkillBuilderPro.API/Resources/drills_seed_CONTAMINATED_OLD.json`, which is explicitly a legacy contaminated source and is not the validated 900 source.

## 10. Current Import Architecture Analysis

### Existing paths

1. `SkillBuilderPro.Core/Migrations/20260802012553_BaselineSchema.cs` contains historical `InsertData` drill rows.
2. `SkillBuilderPro.API/seed_60_drills.sql` begins with `DELETE FROM Drills;` and is unsafe for preservation requirements.
3. `SkillBuilderPro.API/Data/DrillExcelSeeder.cs` is registered but its startup invocation is commented out.
4. `SkillBuilderPro.API/Resources/drills_seed_CONTAMINATED_OLD.json` is quarantined by filename and startup comments.
5. Administrator drill CRUD exists in `SkillBuilderPro.API/Controllers/AdminOperationsController.cs`, but there is no bulk import endpoint/service.

### Why `DrillExcelSeeder` must not run

Exact destructive evidence:

```csharp
await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Drills;");
```

It also:

- executes schema DDL directly instead of relying on reviewed EF migrations;
- merges hardcoded drills and 60 generated placeholder drills into the import;
- deduplicates only with `HashSet<string>` of drill name, which would silently drop valid source rows;
- claims a transaction in its comment but does not start one;
- catches the top-level exception and logs it instead of making the operation fail atomically;
- reads `Resources/drills_seed.json`, while the validated file intentionally remains external and no validated file exists at that repository path.

Automatic execution remains disabled. It was not enabled or invoked.

### Recommended next importer implementation

Implement a separate explicit Development/Admin import command or service, not startup seeding:

1. Require an explicit absolute source path and expected SHA-256.
2. Deserialize to a typed import DTO and validate all 900 records before opening a write transaction.
3. Validate exact sport distribution, 180 composite groups, 5 videos per group, unique source IDs, unique video URLs, supported sports, valid duration/difficulty, and valid playable video URL form.
4. Define stable identity. Preferred: add a reviewed nullable unique `ExternalSourceKey`/`ImportKey` column, populated from a dataset/version plus source `id`. A carefully normalized composite key is the fallback. Do not use Name alone and do not assume source IDs can overwrite database primary keys.
5. Run one EF/SQL transaction with explicit insert/update/unchanged counts.
6. Upsert only importer-owned drill rows. Never `DELETE FROM Drills`; preserve current rows and referenced assignment/history records unless a separately reviewed relationship-safe retirement policy exists.
7. Reject duplicate deterministic keys before writing.
8. Do not add dummy or hardcoded fallback drills.
9. Fail and roll back on any validation/write error; do not swallow the exception.
10. Re-query DB and `GET /api/drills`, verify distributions, validate authenticated Builder filters and Drill Library playback, and compare every non-drill baseline count above.
11. Record an administrator audit/import-run record containing source hash and counts, not secrets.

Schema transformation required: source uses camel-case JSON names and uppercase sports; API models use Pascal-case properties and currently display title-case sport values. Normalize sport/category/subcategory casing consistently while preserving source strings needed for deterministic identity. Source `duration` is a string and maps to `Drill.Duration`; source `difficulty` maps to nullable integer. Source `id` needs a dedicated stable import key rather than assignment to the live `Drill.Id` identity column.

## 11. Development Environment Consistency

| Concern | Evidence | Result |
|---|---|---|
| API environment | `SkillBuilderPro.API/Properties/launchSettings.json` sets `ASPNETCORE_ENVIRONMENT=Development` and `http://0.0.0.0:5000` | PASS |
| Database | API and read-only baseline use `SkillBuilderProDb` on the configured Development SQL Server | PASS |
| Migrations | `AddAdministratorAuditLogs` inspected and applied; startup then reported infrastructure ready | PASS |
| User Secrets | Project UserSecretsId present; account keys and JWT signing key loaded in Development | PASS |
| Role initialization | `IdentityRoleInitializer` ensures Athlete, Parent, Coach, Administrator before account reconciliation | PASS |
| Account initialization | Unified initializer runs only in Development and only after migration/role checks | PASS |
| Android emulator | `ApiEndpointResolver` returns `http://10.0.2.2:5000/` for virtual Android Debug | PASS |
| Windows | `ApiEndpointResolver` returns `http://127.0.0.1:5000/` for WinUI Debug | PASS |
| Physical-device Debug | Resolver currently uses `http://192.168.1.126:5000/`, overridable by the `SkillBuilderPro.ApiBaseUrl` preference | Configuration noted; verify on current LAN before device testing |
| Production | Non-Debug resolver remains `https://api.skillbuilderpro.com/`; Development initializer exits outside Development | Unchanged |

The API process used for verification was stopped before final builds. Start it again with the Development launch profile before emulator testing.

## 12. Security Review

- No actual password was written to tracked source, JSON, blueprint, or this report.
- No JWT token or signing key was printed or saved.
- `secrets.json` is not tracked.
- No non-empty signing key was found in tracked JSON.
- The tracked connection string identifies the Development SQL endpoint/database and uses Windows integrated security; no database password is present. Connection details are not reproduced here beyond the database name required to distinguish the tested backend.
- Authentication and role authorization were not weakened.
- Athlete drill/video behavior, Demo Mode, and administrator modification authorization were not changed.

## 13. Athlete Typography / Local Contrast Pass

No universal Label color or full-screen overlay was introduced. Existing shared text colors remain contextual: `PageTitleStyle` `#F4F7FA`, `PageSubtitleStyle` `#D2DCE6`, `SectionTitleStyle` `#E8EDF3`, `SecondaryTextStyle` `#CED8E2`, metric label `#B8CADB`, and metric value White.

| Page | Previous text/treatment | Current/new treatment | Background-region reason |
|---|---|---|---|
| Athlete Home | `WELCOME BACK`, athlete name, descriptor and Today's Training already use light contextual styles; `PRIMARY AREAS` eyebrow `#91C4F4` was directly over mixed facility art | Identity header remains on local `#A80C121A`; Today's Training/metrics remain on `EliteSurfaceStyle` `#78121821`; `PRIMARY AREAS` now has compact `#A80C121A` backing and padding | Mixed/busy facility artwork; localized backing preserves artwork |
| Training | Light heading/body colors | Header is on `#A80C121A`; sport, Today's Training, assignments, and recent training remain on `EliteSurfaceStyle` | Six Chicago environments have mixed brightness/stadium lights; no one color is trusted directly on art |
| Training Builder | Light headings/form labels | Header uses `GlassHeaderStyle`; every form/filter/list/session/summary area uses a translucent local surface; empty state has `#A8121821` | Functional fields/courts/rink vary in brightness; Hockey ice is protected by local surfaces |
| Goals | Section titles `#E8EDF3` and completed-title `#A6B4C5` could sit directly over bright/mixed art | Title/subtitle remain on `#A80C121A`; Active/Completed section headings now have compact `#A80C121A`; completed rows now use `EliteSurfaceStyle` and `SecondaryTextStyle` | Bright sky and detailed environmental art require localized protection |
| Trophy Room | `RANK JOURNEY` and `ACHIEVEMENTS` `#E8EDF3` were direct over trophy/floor art | Both section headings now have compact `#A80C121A`; milestone/achievement descriptions remain on `EliteSurfaceStyle` | Bright trophy/floor regions and mixed reflections |
| Notifications | White/title, `#D6E2EC` body, muted timestamp | Content remains on `GlassListItemStyle`; page header remains on local translucent surface | Neutral image is retained; text is not direct on art |
| Drill Library | White and muted detail text | Unchanged; selection header uses `#B00C141E`, details use `GlassCardStyle`, video/navigation uses local dark surfaces | Existing working background/video architecture already provides local contrast |
| Profile / Locker Room | Light profile details | Profile content remains on opaque/local dark panel and surfaces; locker name uses `#E8EDF3` on `#A8101419` | Protects text while leaving locker creative exposed |

Repository composition verifies no low-opacity gray is newly placed directly over bright sky or mixed artwork, Hockey Builder content is locally backed, Goals title/subtitle are locally backed, Home Today's Training is locally backed, Trophy milestone text is locally backed, and Training sport controls are locally backed across all six sports. These are code/composition checks only. **Visual success is not claimed until Android portrait and landscape screenshots are reviewed.**

## 14. Blueprint Update

`docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md` now includes the five Development emails/roles, states that credentials come from .NET User Secrets, records the five-account API verification, and retains the 3-drill live baseline. It explicitly does not mark 900 drills live.

## 15. Build Results

### API

```text
dotnet build SkillBuilderPro.API/SkillBuilderPro.API.csproj -c Debug
BUILD PASS
Warnings: 0
Errors: 0
```

### MAUI Android

```text
dotnet build SkillBuilderPro.MAUI/SkillBuilderPro.MAUI.csproj -f net10.0-android -c Debug
BUILD PASS
Errors: 0
Warnings emitted in captured final build output: 0
```

Evidence includes successful C#/XAML generation of `SkillBuilderPro.MAUI.dll` and fresh unsigned/signed Android APK outputs. Build success does not prove runtime or visual success.

## 16. Files Changed by This Pass

Tracked/source changes:

- `SkillBuilderPro.API/Data/DevelopmentTestAccountInitializer.cs` — added unified Development-only reconciliation.
- `SkillBuilderPro.API/Program.cs` — replaced fragmented initializer calls with unified reconciliation after migration/role initialization.
- `SkillBuilderPro.MAUI/Views/AthleteDashboardPage.xaml` — compact backing for the direct-on-art Primary Areas heading.
- `SkillBuilderPro.MAUI/Views/GoalsPage.xaml` — compact section-heading backings and completed-goal row surface.
- `SkillBuilderPro.MAUI/Views/TrophyRoomPage.xaml` — compact section-heading backings.
- `docs/architecture/SKILL_BUILDER_PRO_MASTER_BLUEPRINT.md` — Development account and baseline clarification.
- `docs/architecture/DEVELOPMENT_AUTH_AND_900_DRILL_READINESS.md` — this report.

External Development configuration/state:

- API .NET User Secrets were standardized; values are not tracked or reported.
- `20260815020507_AddAdministratorAuditLogs` was applied to the Development database.
- The five accounts were reconciled as described; no existing user was deleted.

All unrelated dirty/untracked work present before this pass was preserved. No commit was created.

## 17. Required Emulator Role-Switch Test Plan

The following sequence must be run against `http://10.0.2.2:5000/` after starting the API Development profile.

### TEST 1 — Administrator

1. From Choose Profile, select Administrator.
2. Sign in with the standardized Administrator User Secret credential.
3. Verify `ADMIN COMMAND CENTER` and Administrator modules.
4. Select `LOG OUT` / authenticated sign-out.
5. Verify Choose Profile is restored.

### TEST 2 — Coach

1. Select Coach.
2. Sign in with the standardized Coach User Secret credential.
3. Verify Coach experience / Coach's Office background and live Coach workspace.
4. Sign out.
5. Verify Choose Profile and no Administrator state.

### TEST 3 — Parent

1. Select Parent.
2. Sign in with the standardized Parent User Secret credential.
3. Verify Parent experience / Parent Hub and live Parent workspace.
4. Sign out.
5. Verify Choose Profile and no Coach state.

### TEST 4 — Athlete 1

1. Select Athlete.
2. Sign in with the standardized Athlete 1 User Secret credential.
3. Verify Athlete `AppShell` and Athlete Home.
4. Open Profile and select `SIGN OUT`.
5. Verify Choose Profile.

### TEST 5 — Athlete 2

1. Select Athlete.
2. Sign in with the standardized Athlete 2 User Secret credential.
3. Verify Athlete Home.
4. Open Training, then Training Builder.
5. Verify the selected-sport background, sport/category/subcategory drill filtering, multiple ADD actions, and Drill Library playback for a live drill.
6. Open Profile and select `SIGN OUT`.
7. Verify Choose Profile and no Athlete 1 state.

### TEST 6 — Demo Mode

1. Enter Demo Mode.
2. Verify Aubrey/Demo Athlete content and Demo drill playback.
3. Select `EXIT DEMO` / `EXIT DEMO MODE`.
4. Verify Choose Profile.
5. Verify no authenticated CurrentUser, role, JWT header, or prior shell state is visible.

Capture Android portrait and landscape screenshots for Home, Training, Training Builder (including Hockey), Goals, Trophy Room, Notifications, Drill Library, and Profile. The typography changes are not visually approved until those screenshots are reviewed.

## Final Readiness Statement

All five Development identities and role claims are verified, the database baseline is captured, and the external source is located and structurally validated. The 900 drills are **NOT YET imported**. Because the only existing bulk seeder deletes all drills, mixes dummy/hardcoded rows, lacks a transaction, and incorrectly treats Name as unique, it must not be executed. **NOT READY FOR 900-DRILL IMPORT execution; ready for the next safe importer implementation phase.**
