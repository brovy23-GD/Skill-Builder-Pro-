# Development Coach Bootstrap Report

## Outcome

A minimal, idempotent, development-only Coach account bootstrap has been added for local Phase 5B TeamCoach testing. It follows the established Development Administrator initializer pattern without modifying that initializer, public registration, JWT behavior, Phase 5B authorization, or application clients.

No credentials were committed. No schema change or migration was required.

## Files added

- `SkillBuilderPro.API/Data/DevelopmentCoachInitializer.cs`
- `docs/architecture/Development_Coach_Bootstrap_Report.md`

## Files modified

- `SkillBuilderPro.API/Program.cs`
  - Invokes `DevelopmentCoachInitializer.InitializeAsync` inside the existing `app.Environment.IsDevelopment()` startup block, after canonical roles and the unchanged Development Administrator initializer.

`DevelopmentAdminInitializer.cs` was not modified.

## Development-only safeguards

The bootstrap has two independent environment checks:

1. `Program.cs` calls it only inside `if (app.Environment.IsDevelopment())`.
2. `DevelopmentCoachInitializer` resolves `IHostEnvironment` and immediately returns unless `IsDevelopment()` is true.

It cannot create or modify Coach accounts in Production, even if the configuration keys are accidentally present there.

## Configuration

The initializer reads only these configuration keys:

- `DevelopmentCoach:Email`
- `DevelopmentCoach:Password`
- `DevelopmentCoach:DisplayName`

If any value is missing or whitespace-only, it safely skips all account/database work and logs:

```text
Development Coach bootstrap skipped because configuration is incomplete.
```

No actual values were added to `appsettings.json`, source code, migrations, or other committed configuration.

## Account creation and existing-account behavior

If the configured email does not exist, the initializer:

1. Creates an `ApplicationUser` through `UserManager<ApplicationUser>`.
2. Uses the trimmed configured email for both `Email` and `UserName`.
3. Uses the configured password through `UserManager.CreateAsync`, preserving the current Identity password policy.
4. Adds the canonical `ApplicationRoles.Coach` role through `UserManager.AddToRoleAsync`.
5. Creates the existing `UserProfile` structure with the configured display name, active state, and server-generated UTC creation time.

If the configured user already exists:

- No duplicate is created.
- Its password is not reset or changed.
- Existing roles are not removed.
- Coach is added only when missing.
- An existing profile is not overwritten.
- A missing profile is created.
- An inactive profile remains inactive and triggers a safe warning rather than automatic reactivation.

`DisplayName` is validated against the existing `UserProfile.FullName` 100-character maximum before any account change.

## Failure safety

If initialization fails after creating a new user, the initializer attempts to delete only that newly created partial account. If it added Coach to an existing user and a later profile operation fails, it removes only the Coach role added during that failed run. Pre-existing roles and unrelated users remain untouched.

## Role and data safety

- Identity lookup, creation, and role assignment use `UserManager<ApplicationUser>`.
- Identity tables are never manipulated directly.
- Public `/api/auth/register` remains unchanged and continues to permit only Athlete and Parent.
- No public Coach registration was added.
- No ParentAthlete, Team, TeamCoach, TeamAthlete, assignment, or other test-domain record is seeded.
- Development Administrator credentials and behavior are unchanged.
- No password, JWT, password hash, security stamp, or email is logged.
- Successful startup logs only:

```text
Development Coach account is available.
```

## Database and migration impact

- Schema changed: **NO**
- Migration created: **NO**
- Migration applied: **NO**
- Automatic Team/relationship data created: **NO**

The initializer uses the existing Identity and UserProfiles schema.

## Build results

### Core

Required command:

```powershell
dotnet build SkillBuilderPro.Core\SkillBuilderPro.Core.csproj --no-restore
```

Result: **Passed — 0 warnings, 0 errors.**

### API

Required Debug command:

```powershell
dotnet build SkillBuilderPro.API\SkillBuilderPro.API.csproj --no-restore
```

Result: **Environment-blocked after compilation.** A user-running API process held `bin\Debug\net10.0\SkillBuilderPro.API.exe` open. MSBuild emitted ten `MSB3026` retry warnings and then `MSB3027`/`MSB3021` copy errors. The running process was intentionally not terminated.

Compilation was independently verified in the unlocked Release output:

```powershell
dotnet build SkillBuilderPro.API\SkillBuilderPro.API.csproj --no-restore --configuration Release
```

Result: **Passed — 0 warnings, 0 errors.**

There are no C# compiler warnings or errors in the Coach bootstrap.

## Exact PowerShell User Secrets commands

From the repository root, replace the placeholders with local values. Do not commit or share the actual password.

```powershell
dotnet user-secrets set "DevelopmentCoach:Email" "<YOUR_LOCAL_COACH_EMAIL>" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
dotnet user-secrets set "DevelopmentCoach:Password" "<YOUR_STRONG_LOCAL_COACH_PASSWORD>" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
dotnet user-secrets set "DevelopmentCoach:DisplayName" "<YOUR_LOCAL_COACH_DISPLAY_NAME>" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
```

The password must satisfy the existing ASP.NET Core Identity password policy.

## Exact local testing procedure

### 1. Set the User Secrets

Run the three commands above. Ensure the configured Coach email is the intended local test account; if it already belongs to an existing local user, Coach will be added without removing that user's other roles.

### 2. Stop and restart the API

Stop the currently running API so the new binary can build, then run:

```powershell
dotnet build SkillBuilderPro.API\SkillBuilderPro.API.csproj --no-restore
dotnet run --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj --launch-profile SkillBuilderPro.API
```

The launch profile sets the environment to Development.

### 3. Confirm bootstrap completion

Wait for this log entry:

```text
Development Coach account is available.
```

If the log reports incomplete configuration, verify all three User Secrets. If startup reports pending migrations, apply only previously reviewed migrations before restarting.

### 4. Log in as Coach

In Swagger, call `POST /api/auth/login` with the exact local User Secret email/password:

```json
{
  "Email": "<YOUR_LOCAL_COACH_EMAIL>",
  "Password": "<YOUR_STRONG_LOCAL_COACH_PASSWORD>"
}
```

Confirm the response's current-user roles include `Coach`. Record the returned current-user `UserId` as `<COACH_USER_ID>`, and copy the Coach JWT only if you also want to verify that Coach receives 403 on Administrator endpoints.

### 5. Switch Swagger back to Administrator

Call `/api/auth/login` using the configured Development Administrator credentials. Copy the returned Administrator JWT, select **Authorize** in Swagger, replace the prior token with the Administrator token, and authorize.

### 6. Add Coach to Team 1

Call:

```text
POST /api/admin/teams/1/coaches
```

with:

```json
{
  "coachUserId": <COACH_USER_ID>,
  "teamRole": "HeadCoach"
}
```

Expected result for an active Team 1 with no existing active membership: **201 Created**.

If Team 1 does not exist or is inactive, use the Phase 5B Administrator Team APIs to select an existing active test Team; the bootstrap intentionally does not create one. If the membership already exists and is active, the expected Phase 5B response is 409 rather than 201.

## Cleanup and operational notes

The bootstrap does not reset passwords when configuration changes after account creation. Remove the secrets when the local bootstrap is no longer needed:

```powershell
dotnet user-secrets remove "DevelopmentCoach:Email" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
dotnet user-secrets remove "DevelopmentCoach:Password" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
dotnet user-secrets remove "DevelopmentCoach:DisplayName" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
```

Removing User Secrets prevents future bootstrap runs; it does not delete or alter the already-created local Identity account.
