# Testing and CI

## Foundation

`SkillBuilderPro.Tests` is a .NET 10 xUnit project referencing `SkillBuilderPro.Core` and `SkillBuilderPro.Client`. It uses `Microsoft.NET.Test.Sdk`, xUnit, the Visual Studio xUnit runner, and `coverlet.collector`.

The suite currently contains 73 discovered test cases. Tests are deterministic and require no SQL Server, API process, external network, YouTube access, MAUI workload, physical device, or UI interaction. Repository tests use a uniquely named EF Core in-memory database per test.

## Covered behavior

- Skill-level thresholds and boundaries.
- Skill and rank progress percentage calculation and clamping.
- Progression score milestone, breadth, and streak components.
- Rank score and active-skill breadth requirements.
- Current and longest streak calculation, including duplicate dates and stale runs.
- Athlete authorization scope validation, deduplication, scoped access, and administrator access.
- Canonical application-role and assignment-status contracts.
- Assignment operation-result factory semantics.
- Drill API client endpoint construction and response delegation.
- Raw HTTP client success, status-code failure, malformed-payload, transport-failure, and timeout behavior.
- Request method, URI, and JSON serialization contracts.
- Drill, user, and progress repository CRUD, predicate, range, tracking, and persistence behavior.
- `AppDbContext` model construction and entity mapping exercised through isolated repository tests.

## Current coverage

The verified baseline is:

- Previous line coverage: **10.64%** (86/808 lines) in Release.
- Current line coverage: **60.64%** (490/808 lines) in Release.
- Current branch coverage: **94.44%** (34/36 branches).
- Tests: 73 passed, 0 failed, 0 skipped.

Generated Entity Framework migration files are excluded through `SkillBuilderPro.Tests/coverage.runsettings`. No production service, model, repository, DbContext, or Client source file is excluded. A diagnostic Debug run without that generated-file exclusion measured 0.40% (102/24,938 lines), which is not a useful measure of hand-authored production logic.

## Local commands

```powershell
dotnet restore SkillBuilderPro.Tests/SkillBuilderPro.Tests.csproj
dotnet build SkillBuilderPro.Tests/SkillBuilderPro.Tests.csproj --no-restore
dotnet test SkillBuilderPro.Tests/SkillBuilderPro.Tests.csproj --no-build
dotnet test SkillBuilderPro.Tests/SkillBuilderPro.Tests.csproj --no-build --collect:"XPlat Code Coverage" --settings SkillBuilderPro.Tests/coverage.runsettings --results-directory artifacts/test-results
```

The Cobertura report is written beneath `artifacts/test-results/**/coverage.cobertura.xml`. The root `line-rate` and `branch-rate` attributes provide the reproducible summary percentages.

## Continuous integration

`.github/workflows/dotnet-ci.yml` runs for pushes and pull requests on Ubuntu. It installs .NET 10, restores and builds the test project and its Core/Client references, runs all tests in Release configuration, enforces a 25% line-coverage floor, prints line and branch coverage, and uploads TRX/Cobertura artifacts.

The workflow intentionally does not build API, MAUI, or WinForms. Those projects require platform workloads or infrastructure that are unrelated to this cross-platform unit-test job.

## Current gaps

Not yet covered are authenticated API handlers, progression/assignment service implementations located outside Core, notifications, goals, scheduling, MAUI view models, WinForms logic, SQL Server-specific integration behavior, and end-to-end client/API flows. The EF in-memory tests verify repository contracts and model execution but do not replace relational-database integration tests.

## Resume discussion

This expansion is best described as building a layered .NET quality gate: deterministic domain tests, fault-injected HTTP client tests using a custom message handler, isolated EF Core repository tests, Cobertura reporting, and a GitHub Actions coverage threshold. Coverage increased from 10.64% to 60.64% without excluding hand-authored production code; only generated migrations remain excluded.
