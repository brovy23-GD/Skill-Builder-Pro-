# Development Coach Password Reset Report

## Outcome

A safe, opt-in, development-only password reset utility has been added to the existing Development Coach initializer. It resets only the configured Development Coach account through ASP.NET Core Identity when `DevelopmentCoach:ResetPassword` is `true`.

No public endpoint was added, no authentication or authorization behavior was weakened, and no database migration was required.

## Files modified

- `SkillBuilderPro.API/Data/DevelopmentCoachInitializer.cs`
  - Reads the optional `DevelopmentCoach:ResetPassword` flag.
  - Verifies the configured account has the canonical Coach role.
  - Generates an Identity password-reset token.
  - Resets the password to the current `DevelopmentCoach:Password` value.
  - Logs a generic success message without credential material.

## Files added

- `docs/architecture/Development_Coach_Password_Reset_Report.md`

No other application file was changed. In particular, the Administrator initializer, AuthController, JWT configuration, Phase 5B authorization, MAUI, and WinForms remain unchanged.

## Development-only enforcement

The reset is protected by the existing two-layer Coach-bootstrap guard:

1. `Program.cs` invokes `DevelopmentCoachInitializer` only inside `if (app.Environment.IsDevelopment())`.
2. `DevelopmentCoachInitializer` independently checks `IHostEnvironment.IsDevelopment()` and returns otherwise.

The new reset block additionally requires:

```text
DevelopmentCoach:ResetPassword = true
```

When the flag is false or absent, no reset token is generated and the password is not changed. Production cannot invoke this reset path even if the flag is accidentally configured.

## Reset behavior

During a Development startup with complete Coach configuration and the reset flag enabled, the initializer:

1. Locates only the Identity user matching the configured `DevelopmentCoach:Email`.
2. Completes the existing safe Coach bootstrap behavior if the account/profile does not yet exist.
3. Requires the configured user to have `ApplicationRoles.Coach` before resetting.
4. Calls `UserManager.GeneratePasswordResetTokenAsync(user)`.
5. Calls `UserManager.ResetPasswordAsync(user, token, configuredPassword)` using the current `DevelopmentCoach:Password` User Secret.
6. Preserves the user's profile, IDs, roles, Team memberships, and relationship data.
7. Logs only:

```text
Development Coach password reset completed.
```

The token and password are held only in process memory and are never logged or written to source.

## Identity safety

- `PasswordHash` is never assigned directly.
- No direct `AspNetUsers` or security-field update occurs.
- Identity's configured password validators continue to apply.
- The password reset uses the normal Identity token provider and updates Identity security state through supported APIs.
- Other users are never queried by arbitrary request data or modified.
- Existing roles are not removed.
- `UserProfile` is not modified by the reset operation.
- No Team, TeamCoach, TeamAthlete, ParentAthlete, assignment, or test-domain data is created by the reset.
- No password-reset API route exists.

## One-time execution procedure

The flag is deliberately manual and opt-in. It will reset at each Development startup while it remains true, so remove it or set it to false immediately after observing the success log.

From the repository root in PowerShell:

```powershell
dotnet user-secrets set "DevelopmentCoach:ResetPassword" "true" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
```

Confirm the current intended password is already stored:

```powershell
dotnet user-secrets set "DevelopmentCoach:Password" "<YOUR_CURRENT_LOCAL_COACH_PASSWORD>" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
```

Do not replace the placeholder in source or this report; use the actual desired local password only in the terminal command.

Stop and restart the API in Development:

```powershell
dotnet run --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj --launch-profile SkillBuilderPro.API
```

Wait for:

```text
Development Coach password reset completed.
```

Then stop the API and remove the one-time flag:

```powershell
dotnet user-secrets remove "DevelopmentCoach:ResetPassword" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
```

Alternatively, explicitly disable it:

```powershell
dotnet user-secrets set "DevelopmentCoach:ResetPassword" "false" --project .\SkillBuilderPro.API\SkillBuilderPro.API.csproj
```

Restart the API normally. The reset will no longer run.

## Login verification

After disabling/removing the flag and restarting, call `POST /api/auth/login` with the configured Coach email and the current User Secret password:

```json
{
  "Email": "<YOUR_LOCAL_COACH_EMAIL>",
  "Password": "<YOUR_CURRENT_LOCAL_COACH_PASSWORD>"
}
```

Expected result: successful authentication, a JWT response, and a current-user role collection containing `Coach`. If login still fails, confirm the existing `UserProfile.IsActive` value is true; the bootstrap intentionally does not reactivate inactive profiles.

## Logging

The new code logs no email, password, reset token, JWT, hash, security stamp, or other credential data. Only the generic completion message is emitted.

## Database and migration impact

- Schema change: **NO**
- Migration created: **NO**
- Migration applied: **NO**
- Relationship/Team data changed by implementation: **NO**

The only runtime data change, when explicitly enabled, is the configured Coach account's Identity-managed password/security state.

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

An additional Release precheck also passed with 0 warnings and 0 errors.

## Immediate operational warning

Do not leave `DevelopmentCoach:ResetPassword=true`. Although it is Development-only and targets only the configured Coach, it intentionally resets that password on every Development startup until the flag is removed or disabled.
