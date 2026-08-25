# Responsive Training Builder Source Asset Audit

**Audit timestamp:** 2026-08-22 17:38:21 -05:00  
**Result:** FAIL  
**Audited root:** `DesignAssets/Backgrounds/Builder`  
**Scope:** Source PNG inventory and validation only. The user clarified that `Builder` is the intended root in place of the originally named `TrainingBuilder` directory.

## Executive summary

| Measure | Result |
|---|---:|
| Expected files | 36 |
| Candidate assets found | 30 |
| Fully verified correct | 12 / 36 |
| Correctly named | 12 / 36 |
| Dimension-compliant | 18 / 36 |
| Missing expected assets | 6 |
| Failing expected entries | 24 |
| Unexpected/wrong-folder assets | 1 |
| Duplicate binary files | 0 |
| Unreadable/corrupt PNG files | 0 |

Only Football and Softball contain complete, canonically named six-file sets with compliant dimensions. Basketball, Soccer, and Hockey contain usable-looking candidates, but all are raw ChatGPT filenames and four production variants per sport have noncompliant dimensions. Baseball has no source folder or files. The Hockey overview candidate is misplaced in the Soccer folder.

## Validation rules

Each sport is expected to contain these six files:

| Variant | Canonical filename suffix | Required dimensions |
|---|---|---:|
| Phone portrait | `phone_portrait` | 1080 × 1920 |
| Phone landscape | `phone_landscape` | 1920 × 1080 |
| Tablet portrait | `tablet_portrait` | 1200 × 1920 |
| Tablet landscape | `tablet_landscape` | 1920 × 1200 |
| Desktop | `desktop` | 1672 × 941 |
| Sizes overview | `sizes_overview` | Flexible; actual dimensions recorded |

Canonical filenames follow `training_builder_<sport>_<variant>.png`. A file is counted as fully correct only when its filename, sport folder, and dimensions all comply.

## Complete 36-file matrix

| Sport | Expected canonical file | Actual source file | Actual dimensions | Name | Dimensions | Folder | Overall | Notes |
|---|---|---|---:|:---:|:---:|:---:|:---:|---|
| Basketball | `training_builder_basketball_phone_portrait.png` | `ChatGPT Image Aug 22, 2026, 03_55_44 PM (5).png` | 863 × 1822 | FAIL | FAIL | PASS | FAIL | Visually identified phone-portrait candidate. |
| Basketball | `training_builder_basketball_phone_landscape.png` | `ChatGPT Image Aug 22, 2026, 03_55_43 PM (4).png` | 1822 × 863 | FAIL | FAIL | PASS | FAIL | Visually identified phone-landscape candidate. |
| Basketball | `training_builder_basketball_tablet_portrait.png` | `ChatGPT Image Aug 22, 2026, 03_55_43 PM (3).png` | 1086 × 1448 | FAIL | FAIL | PASS | FAIL | Visually identified tablet-portrait candidate. |
| Basketball | `training_builder_basketball_tablet_landscape.png` | `ChatGPT Image Aug 22, 2026, 03_55_43 PM (2).png` | 1448 × 1086 | FAIL | FAIL | PASS | FAIL | Visually identified tablet-landscape candidate. |
| Basketball | `training_builder_basketball_desktop.png` | `ChatGPT Image Aug 22, 2026, 03_55_43 PM (1).png` | 1672 × 941 | FAIL | PASS | PASS | FAIL | Visually identified desktop candidate. |
| Basketball | `training_builder_basketball_sizes_overview.png` | `ChatGPT Image Aug 22, 2026, 03_55_43 PM (6).png` | 1672 × 941 | FAIL | PASS | PASS | FAIL | Overview dimensions are flexible. |
| Football | `training_builder_football_phone_portrait.png` | Same as expected | 1080 × 1920 | PASS | PASS | PASS | PASS | — |
| Football | `training_builder_football_phone_landscape.png` | Same as expected | 1920 × 1080 | PASS | PASS | PASS | PASS | — |
| Football | `training_builder_football_tablet_portrait.png` | Same as expected | 1200 × 1920 | PASS | PASS | PASS | PASS | — |
| Football | `training_builder_football_tablet_landscape.png` | Same as expected | 1920 × 1200 | PASS | PASS | PASS | PASS | — |
| Football | `training_builder_football_desktop.png` | Same as expected | 1672 × 941 | PASS | PASS | PASS | PASS | — |
| Football | `training_builder_football_sizes_overview.png` | Same as expected | 1800 × 1200 | PASS | PASS | PASS | PASS | Overview dimensions are flexible. |
| Baseball | `training_builder_baseball_phone_portrait.png` | Missing | — | FAIL | FAIL | FAIL | FAIL | Baseball folder is absent. |
| Baseball | `training_builder_baseball_phone_landscape.png` | Missing | — | FAIL | FAIL | FAIL | FAIL | Baseball folder is absent. |
| Baseball | `training_builder_baseball_tablet_portrait.png` | Missing | — | FAIL | FAIL | FAIL | FAIL | Baseball folder is absent. |
| Baseball | `training_builder_baseball_tablet_landscape.png` | Missing | — | FAIL | FAIL | FAIL | FAIL | Baseball folder is absent. |
| Baseball | `training_builder_baseball_desktop.png` | Missing | — | FAIL | FAIL | FAIL | FAIL | Baseball folder is absent. |
| Baseball | `training_builder_baseball_sizes_overview.png` | Missing | — | FAIL | FAIL | FAIL | FAIL | Baseball folder is absent. |
| Softball | `training_builder_softball_phone_portrait.png` | Same as expected | 1080 × 1920 | PASS | PASS | PASS | PASS | — |
| Softball | `training_builder_softball_phone_landscape.png` | Same as expected | 1920 × 1080 | PASS | PASS | PASS | PASS | — |
| Softball | `training_builder_softball_tablet_portrait.png` | Same as expected | 1200 × 1920 | PASS | PASS | PASS | PASS | — |
| Softball | `training_builder_softball_tablet_landscape.png` | Same as expected | 1920 × 1200 | PASS | PASS | PASS | PASS | — |
| Softball | `training_builder_softball_desktop.png` | Same as expected | 1672 × 941 | PASS | PASS | PASS | PASS | — |
| Softball | `training_builder_softball_sizes_overview.png` | Same as expected | 1800 × 1200 | PASS | PASS | PASS | PASS | Overview dimensions are flexible. |
| Soccer | `training_builder_soccer_phone_portrait.png` | `ChatGPT Image Aug 22, 2026, 04_58_13 PM (5).png` | 941 × 1672 | FAIL | FAIL | PASS | FAIL | Visually identified phone-portrait candidate. |
| Soccer | `training_builder_soccer_phone_landscape.png` | `ChatGPT Image Aug 22, 2026, 04_58_13 PM (4).png` | 1672 × 941 | FAIL | FAIL | PASS | FAIL | Visually identified phone-landscape candidate. |
| Soccer | `training_builder_soccer_tablet_portrait.png` | `ChatGPT Image Aug 22, 2026, 04_58_13 PM (3).png` | 1086 × 1448 | FAIL | FAIL | PASS | FAIL | Visually identified tablet-portrait candidate. |
| Soccer | `training_builder_soccer_tablet_landscape.png` | `ChatGPT Image Aug 22, 2026, 04_58_12 PM (2).png` | 1448 × 1086 | FAIL | FAIL | PASS | FAIL | Visually identified tablet-landscape candidate. |
| Soccer | `training_builder_soccer_desktop.png` | `ChatGPT Image Aug 22, 2026, 04_58_12 PM (1).png` | 1672 × 941 | FAIL | PASS | PASS | FAIL | Visually identified desktop candidate. |
| Soccer | `training_builder_soccer_sizes_overview.png` | `ChatGPT Image Aug 22, 2026, 04_58_13 PM (6).png` | 1672 × 941 | FAIL | PASS | PASS | FAIL | Overview dimensions are flexible. |
| Hockey | `training_builder_hockey_phone_portrait.png` | `ChatGPT Image Aug 22, 2026, 05_31_05 PM (5).png` | 941 × 1672 | FAIL | FAIL | PASS | FAIL | Visually identified phone-portrait candidate. |
| Hockey | `training_builder_hockey_phone_landscape.png` | `ChatGPT Image Aug 22, 2026, 05_31_05 PM (4).png` | 1672 × 941 | FAIL | FAIL | PASS | FAIL | Visually identified phone-landscape candidate. |
| Hockey | `training_builder_hockey_tablet_portrait.png` | `ChatGPT Image Aug 22, 2026, 05_31_05 PM (3).png` | 1086 × 1448 | FAIL | FAIL | PASS | FAIL | Visually identified tablet-portrait candidate. |
| Hockey | `training_builder_hockey_tablet_landscape.png` | `ChatGPT Image Aug 22, 2026, 05_31_05 PM (2).png` | 1448 × 1086 | FAIL | FAIL | PASS | FAIL | Visually identified tablet-landscape candidate. |
| Hockey | `training_builder_hockey_desktop.png` | `ChatGPT Image Aug 22, 2026, 05_31_04 PM (1).png` | 1672 × 941 | FAIL | PASS | PASS | FAIL | Visually identified desktop candidate. |
| Hockey | `training_builder_hockey_sizes_overview.png` | `Soccer/ChatGPT Image Aug 22, 2026, 05_31_05 PM (6).png` | 1448 × 1086 | FAIL | PASS | FAIL | FAIL | Hockey overview content is misplaced in the Soccer folder. |

## Per-sport result

| Sport | Fully correct | Result | Primary issue |
|---|---:|:---:|---|
| Basketball | 0 / 6 | FAIL | Six raw filenames; four production dimensions incorrect. |
| Football | 6 / 6 | PASS | Complete and compliant. |
| Baseball | 0 / 6 | FAIL | Entire six-file set missing. |
| Softball | 6 / 6 | PASS | Complete and compliant. |
| Soccer | 0 / 6 | FAIL | Six raw filenames; four production dimensions incorrect; contains misplaced Hockey overview. |
| Hockey | 0 / 6 | FAIL | Six raw filenames; four production dimensions incorrect; overview stored in Soccer. |

## File integrity and duplicate review

- All 30 discovered PNG files were readable and had valid reported dimensions.
- All extensions are lowercase `.png`.
- SHA-256 comparison found 30 unique hashes; no byte-identical duplicates exist.
- No unexpected non-PNG files were found in the audited sport folders.
- `DesignAssets/Archive/Builder` contained no archived assets at audit time.

## SHA-256 inventory

| Relative file | SHA-256 |
|---|---|
| `Basketball/ChatGPT Image Aug 22, 2026, 03_55_43 PM (1).png` | `FFE0963FF2B5BEE250BC7AE9F96475BABFA62037EEFDAD59350E9BB574439639` |
| `Basketball/ChatGPT Image Aug 22, 2026, 03_55_43 PM (2).png` | `94F03B4C63929B40BCEAB97C55AE194BE1B1F5912FE30B7A3841823657C3535D` |
| `Basketball/ChatGPT Image Aug 22, 2026, 03_55_43 PM (3).png` | `902CB195446E3FAD6685326FDEB68A72D5C2831A78951E77E2DC75C80D8CB68C` |
| `Basketball/ChatGPT Image Aug 22, 2026, 03_55_43 PM (4).png` | `3B3D5B78718F1CDD93A5B30A6C9191D0A42A50BD8B5187740730ED789377821D` |
| `Basketball/ChatGPT Image Aug 22, 2026, 03_55_44 PM (5).png` | `E6483FB730CB79FDCC4DF3B9DC789253E276C17689F9F78130DA27B7AC6675AA` |
| `Basketball/ChatGPT Image Aug 22, 2026, 03_55_43 PM (6).png` | `BDEAF05508E0A122D4CD579BCB206F801E8A5877897D82BF261477753C50875D` |
| `Football/training_builder_football_desktop.png` | `49608F16A3E28B3E3E03FE5F8D7C0CCE74FF1DE48656944EF71C2AE9E75275C7` |
| `Football/training_builder_football_phone_landscape.png` | `C39F47A9F9AD8212B596056603104637F67680AE2C19497CB19CFC37B9F8860A` |
| `Football/training_builder_football_phone_portrait.png` | `9E8BAB842DAF32499A873192FAF26044738EB1BC33DFCC9A9D4A0493F37584E7` |
| `Football/training_builder_football_sizes_overview.png` | `F53667E8284D28DB0ABB079CA0E21EED6DEBA3F23B29BD6CBE18BEA20ED6D446` |
| `Football/training_builder_football_tablet_landscape.png` | `F605CA29D6B0894E8CEAFFA51F7D44BF03DDE4B63096A0CD2CEEC04EE605DE13` |
| `Football/training_builder_football_tablet_portrait.png` | `6374FF7F60AFA2625E8EBEC85C7816CEB3DF7625071472202A63FCDA29194BE6` |
| `Softball/training_builder_softball_desktop.png` | `F9BC39B7A4801BC6C5CB530710734E042BCE78F45258A0E897B49EA2668B204D` |
| `Softball/training_builder_softball_phone_landscape.png` | `7CDB834ABFDB417AD1A0E8CDDE09821BEE834B393D4A32E8C0C08667740B6BA9` |
| `Softball/training_builder_softball_phone_portrait.png` | `124695978524C8096EA7207653D24A26D40852E11A7A02CA7F7D17FCE63E12C3` |
| `Softball/training_builder_softball_sizes_overview.png` | `1E152754BFEC710F5392E9B13CB91CBF5B55871B68C17C01B9D7511662983C55` |
| `Softball/training_builder_softball_tablet_landscape.png` | `C3129AEAF6F4313F7603E97A2672ACCF60EDF78A52466C6435C48FC848B7CC1F` |
| `Softball/training_builder_softball_tablet_portrait.png` | `C19873DE3E3A298BA7F5C9516002887F9EFC5B2D9844E92F6C754976E477E68C` |
| `Soccer/ChatGPT Image Aug 22, 2026, 04_58_12 PM (1).png` | `30523958D7DEA1258B0FB0A42719C7083954F24AC567A59CBC28895474742F4D` |
| `Soccer/ChatGPT Image Aug 22, 2026, 04_58_12 PM (2).png` | `43BA293FEF7C1E218A5659CC4F18AC9EC31A6DBC9086F5000054765479992983` |
| `Soccer/ChatGPT Image Aug 22, 2026, 04_58_13 PM (3).png` | `05377F2C175DF15882F663F48C8D2013E9D8C3E9C5011DACB14C153025364DED` |
| `Soccer/ChatGPT Image Aug 22, 2026, 04_58_13 PM (4).png` | `8CD549D5BA8F96C54DCBAA934DB7A889DCC325A432334934D59D2F9A0B94F85E` |
| `Soccer/ChatGPT Image Aug 22, 2026, 04_58_13 PM (5).png` | `37947EF0771E05C0D04B3CCD954096F9BBD116227F16439B9200FEBBE9EBC4EE` |
| `Soccer/ChatGPT Image Aug 22, 2026, 04_58_13 PM (6).png` | `145A87AAD34C91A37EEBD13F7B10F6B8431007976CCB4795C0791B5455C64400` |
| `Soccer/ChatGPT Image Aug 22, 2026, 05_31_05 PM (6).png` | `79CF621BEE4CA1096F235480393B1AF1E5B306763CD6F2B83BD5213F51FABDCF` |
| `Hockey/ChatGPT Image Aug 22, 2026, 05_31_04 PM (1).png` | `099E3C30F7A2A618ED02C2E473E258E005E81358364D571DC5E60BC8028E4ECB` |
| `Hockey/ChatGPT Image Aug 22, 2026, 05_31_05 PM (2).png` | `4B31615274D030E704FDAFF36913616F32292AC58AD9DCBD49398BE78687F15A` |
| `Hockey/ChatGPT Image Aug 22, 2026, 05_31_05 PM (3).png` | `11DF84C423466AF687A80590855191426B6A6E3CEF3C9A3D6EA2B499BB0D3139` |
| `Hockey/ChatGPT Image Aug 22, 2026, 05_31_05 PM (4).png` | `9363A7396E72D5186CB79681E79C072FA6905891910565E8C25B94E503E6373B` |
| `Hockey/ChatGPT Image Aug 22, 2026, 05_31_05 PM (5).png` | `1ADD542299C50FE7303FF4C9AAEC10250EDACF9B713F475CCD3EAF5D909FE47F` |

## Required remediation before approval

1. Supply the missing six-file Baseball set.
2. Normalize Basketball, Soccer, and Hockey production dimensions to the required pixel sizes without stretching or changing intended composition.
3. Rename the 18 raw Basketball, Soccer, and Hockey candidates to the canonical naming convention.
4. Move the Hockey sizes-overview asset from Soccer to Hockey and give it the canonical Hockey overview filename.
5. Re-run the same 36-entry verification after normalization.

No source assets were altered as part of this audit.

## Remediation and final verification — 2026-08-22

The original audit result above remains historical evidence and was accurate when recorded. After that audit:

- Baseball was supplied as a complete six-file canonical set.
- Basketball's six candidates were canonically named; four production variants were normalized by deterministic centered crop-to-fill, while desktop and overview pixels were unchanged.
- Soccer received the same canonical naming and four-variant deterministic normalization.
- Hockey received the same canonical naming and four-variant deterministic normalization.
- The Hockey overview was moved from Soccer into Hockey and canonically named.
- Football and Softball remained unchanged because their six-file sets were already compliant.

Final metadata verification found six sport folders, 36/36 readable canonical source PNGs, 30/30 production files at exact required dimensions, six documentation overview boards, no wrong-folder assets, and no unexpected files.

**Remediated final result: PASS (36/36).**
