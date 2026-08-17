# Development Administrator Bootstrap Report

## Outcome

A minimal, idempotent, development-only Administrator bootstrap has been added for local Swagger and Phase 5B testing. It uses ASP.NET Core Identity APIs, reads credentials exclusively from configuration/User Secrets, creates the existing `UserProfile` structure when needed, and never changes public registration.

No credentials were added to source control. No database migration was required or created.

## Files added

- `SkillBuilderPro.API/Data/DevelopmentAdminInitializer.cs`
- `docs/architecture/Development_Admin_Bootstrap_Report.md`

## Files modified

- `SkillBuilderPro.API/Program.cs`
  - Calls `DevelopmentAdminInitializer.InitializeAsync` after canonical Identity role initialization and only inside `if (app.Environment.IsDevelopment())`.

No authentication controller, registration rule, JWT component, Phase 5B controller, client project, entity, DbContext mapping, or migration was changed.

## Bootstrap design

`DevelopmentAdminInitializer` follows the existing startup/Identity initializer architecture. Startup first checks for pending migrations, initializes canonical roles, and then invokes the development bootstrap.

The bootstrap has two independent environment safeguards:

1. `Program.cs` invokes it only when `app.Environment.IsDevelopment()` is true.
2. The initializer resolves `IHostEnvironment` and returns immediately unless `IsDevelopment()` is true.

It reads:

- `DevelopmentAdmin:Email`
- `DevelopmentAdmin:Password`
- `DevelopmentAdmin:DisplayName`

If any value is absent or whitespace-only, it logs a safe skip message and makes no database changes. `DisplayName` is checked against the existing `UserProfile.FullName` maximum length of 100 characters before any account change.

## New-account behavior

When the configured email does not identify an existing Identity user, the initializer:

1. Creates `ApplicationUser` with the normalized email as both `Email` and `UserName`.
2. Uses `UserManager.CreateAsync(user, configuredPassword)`, so all configured Identity password validation remains enforced.
3. Assigns `ApplicationRoles.Administrator` through `UserManager.AddToRoleAsync`.
4. Creates a `UserProfile` with:
   - `UserId` from the new Identity user.
   - `FullName` from `DevelopmentAdmin:DisplayName`.
   - `IsActive = true`.
   - `DateCreated = DateTime.UtcNow`.
5. Logs only: `Development Administrator account is available.`

If a later initialization step fails after creating the user, the initializer attempts to delete that newly created user rather than leaving a partial bootstrap account.

## Existing-account behavior

When the configured email already exists:

- No duplicate user is created.
- The password is not checked, reset, or changed.
- Existing roles are not removed.
- The canonical Administrator role is added only if it is missing.
- An existing profile is not overwritten.
- If the configured user has no profile, the initializer creates the required active `UserProfile` using the configured display name.
- If an existing profile is inactive, the initializer does not silently reactivate it; it emits a safe warning and the existing inactive state remains authoritative.

If the initializer adds Administrator to an existing user and a later profile operation fails, it removes only the Administrator role that this run added. Pre-existing roles remain untouched.

## Identity and role safety

- User lookup and creation use `UserManager<ApplicationUser>`.
- Administrator assignment uses `UserManager` and the canonical `ApplicationRoles.Administrator` constant.
- No direct writes to Identity tables occur.
- No existing roles are removed during successful initialization.
- No password reset occurs.
- No ParentAthlete, Team, TeamCoach, or TeamAthlete records are created.
- No relationships, Teams, production data, or client state are seeded.
- `/api/auth/register` remains unchanged and still permits only Athlete and Parent public registration.
- There is no Administrator registration endpoint or authorization bypass.

## Logging safety

The initializer logs only generic state:

- Configuration incomplete/bootstrap skipped.
- Development Administrator account available.
- Existing configured profile inactive.

It does not log the email, password, JWT, password hash, security stamp, or other credential material.

## Production behavior

The initializer is never invoked by `Program` outside Development, and it independently refuses to run outside Development. Production configuration values—even if accidentally supplied—will not cause account creation or modification through this bootstrap.

## Database and migration impact

- Schema changes: **none**.
- Migration created: **NO**.
- Migration applied: **NO**.
- Existing Identity and `UserProfiles` tables are used without alteration.

## Build results

### Core

Command:

```powershell
dotnet build SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --no-restore
```

Result: **Passed — 0 warnings, 0 errors.**

### API

Command:

```powershell
dotnet build SkillBuilderPro.API\SkillBuilderPro.API.csproj --no-restore
```

Result: **Passed — 0 warnings, 0 errors.**

## Exact User Secrets commands

Run these commands from the repository root in PowerShell. Replace every placeholder with a local value. Do not commit or paste the real password into source files.

```powershell
dotnet user-secrets set "DevelopmentAdmin:Email" "<YOUR_LOCAL_ADMIN_EMAIL>" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
dotnet user-secrets set "DevelopmentAdmin:Password" "<YOUR_STRONG_LOCAL_ADMIN_PASSWORD>" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
dotnet user-secrets set "DevelopmentAdmin:DisplayName" "<YOUR_LOCAL_ADMIN_DISPLAY_NAME>" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
```

The password must satisfy the current ASP.NET Core Identity password policy. The bootstrap does not weaken that policy.

Optional verification that the keys exist locally:

```powershell
dotnet user-secrets list --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
```

Be aware that `user-secrets list` displays secret values in the terminal. Do not share or capture that output.

## Local startup and testing instructions

### 1. Set User Secrets

Run the three `dotnet user-secrets set` commands above. Ensure the existing JWT and database development configuration is also available through the project's established local configuration.

### 2. Start or restart the API in Development

Stop any currently running API instance, then run from the repository root:

```powershell
dotnet run --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj --launch-profile SkillBuilderPro.API
```

The launch profile sets `ASPNETCORE_ENVIRONMENT=Development`. Wait for this safe log message before attempting login:

```text
Development Administrator account is available.
```

If configuration is incomplete, the log will say the bootstrap was skipped. If migrations are pending, the existing startup workflow pauses Identity initialization; apply only previously reviewed migrations before retrying.

### 3. Log in through the API

In Swagger, open `POST /api/auth/login` and submit the same email and password stored in User Secrets:

```json
{
  "Email": "<YOUR_LOCAL_ADMIN_EMAIL>",
  "Password": "<YOUR_STRONG_LOCAL_ADMIN_PASSWORD>"
}
```

The existing login endpoint authenticates through ASP.NET Core Identity. A successful response contains the JWT access token and current-user information. Confirm the returned roles include `Administrator`.

### 4. Authorize Swagger

1. Copy only the returned access-token value.
2. Select **Authorize** in Swagger UI.
3. Paste the JWT token into the Bearer authorization field. With the existing HTTP Bearer scheme, enter the token itself; Swagger supplies the `Bearer` prefix.
4. Select **Authorize**, close the dialog, and call an endpoint such as `GET /api/admin/teams`.

Expected result: the Administrator token reaches the endpoint. Anonymous requests receive 401, and authenticated Athlete/Parent/Coach users receive 403.

## Operational notes

- Startup initialization is idempotent: restarting does not duplicate the configured user, profile, or Administrator role.
- Changing the configured password after the account exists does not reset the account password. Use normal Identity password-management tooling when that is implemented, or intentionally remove the local development account before recreating it.
- Pointing `DevelopmentAdmin:Email` at an existing local user intentionally grants that configured user Administrator while retaining their other roles. Verify the email carefully before startup.
- An inactive existing profile is not reactivated automatically. Resolve that local data state deliberately before login testing.
- Remove the three User Secrets when local Administrator testing is no longer required:

```powershell
dotnet user-secrets remove "DevelopmentAdmin:Email" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
dotnet user-secrets remove "DevelopmentAdmin:Password" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
dotnet user-secrets remove "DevelopmentAdmin:DisplayName" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
```
