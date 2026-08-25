# Safe Drill Import Operations Guide

## Scope

This guide covers the explicit Development importer for the canonical Skill Builder Pro 900-drill dataset. The command does not run during normal API startup and refuses to run outside the Development environment.

Authoritative source:

`C:\Users\brovy\OneDrive\Desktop\Skill Builder Pro\drills_seed.json`

Expected SHA-256:

`AA46D3C4923452C8BA87F365D8672F1B9F5C2AB98EFD5595A7BC6F1E2F50D247`

## Preconditions

- Back up the target Development database through the normal database operations process.
- Confirm the API configuration points to the intended Development database.
- Apply all EF Core migrations. The importer refuses to proceed while migrations are pending.
- Never use `SkillBuilderPro.API/Resources/drills_seed_CONTAMINATED_OLD.json`.
- Never run `DrillExcelSeeder` or `SkillBuilderPro.API/seed_60_drills.sql` for this operation.

## Dry run

From the repository root:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project SkillBuilderPro.API/SkillBuilderPro.API.csproj -- import-drills --source "C:\Users\brovy\OneDrive\Desktop\Skill Builder Pro\drills_seed.json" --sha256 "AA46D3C4923452C8BA87F365D8672F1B9F5C2AB98EFD5595A7BC6F1E2F50D247" --dry-run
```

Expected validation is 900 valid rows, 0 invalid rows, six sports with 150 each, 180 groups with 5 each, and no duplicate import keys. Stop if hash, count, distribution, or required-field validation fails.

## Import

Run only after a clean dry run:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project SkillBuilderPro.API/SkillBuilderPro.API.csproj -- import-drills --source "C:\Users\brovy\OneDrive\Desktop\Skill Builder Pro\drills_seed.json" --sha256 "AA46D3C4923452C8BA87F365D8672F1B9F5C2AB98EFD5595A7BC6F1E2F50D247"
```

Validation finishes before the write transaction opens. All inserts, updates, and legacy-key attachments occur in one explicit transaction through EF Core's configured execution strategy. A fatal write error rolls back the transaction and returns a nonzero process exit code.

## Idempotency verification

Run the same import command a second time. A stable source should report:

- Inserted: 0
- Updated: 0
- Unchanged: 900
- Total database count unchanged

Canonical records are identified by `Drill.ExternalSourceKey` beginning with `skillbuilderpro-900-v1:`. Rows with a null key are legacy/non-import-owned and are never overwritten unless they satisfy the importer's conservative exact-match attachment rule.

## Post-import checks

- Query `GET http://localhost:5000/api/drills` and distinguish all returned rows from the canonical 900.
- Verify 150 canonical rows per supported sport.
- Verify 180 canonical sport/category/subcategory groups and exactly 5 rows in every group.
- Compare captured non-drill baseline counts before and after the run.
- Build the API and Android MAUI target.

Video parsing warnings do not invalidate an otherwise addable drill. The importer does not call YouTube or test external video availability.

## Historical paths

`DrillExcelSeeder`, `seed_60_drills.sql`, and the contaminated old JSON are retained only as historical artifacts. They have no normal startup caller and must not be used for canonical imports.
