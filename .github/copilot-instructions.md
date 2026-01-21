# Coding guidelines for Zwift TTT race simulator

---
description: Comprehensive coding guidelines and architecture patterns for AI agents contributing to the Zwift TTT race simulator project.
applyTo: **/*.cs
---

## 🏗️ Architecture Overview

This project generates **one workout file per rider** simulating a Team Time Trial rotation. The data flow is:

**CSV Input** → **CsvParser** (parse rider data) → **RotationComposer** (simulate rotation) → **Workout Converter** (format by rider) → **ZwoExporter** / **ImageExporter** (output)

### Key Components

- **Model layer** (`ZwiftTTTSim.Core/Model/`): Domain models representing the rotation hierarchy
  - `RiderPowerPlan`: Complete rider state including name, pull duration, power targets per position (1st=pulling, 2nd, 3rd, 4th+), and embedded `RiderData`
  - `RiderData`: Rider metadata (FTP, weight) used to calculate intensity as power/FTP ratio
  - `Pull` (new): Represents one complete rotation cycle with a pulling rider and all other riders in their positions
  - `PullPosition` (new): Represents a single rider's state within a pull (which rider, their position, target power for that position)
  - `WorkoutStep`: Duration + power pair; used internally for export conversion
  
- **Services layer** (`ZwiftTTTSim.Core/Services/`): Business logic
  - `RotationComposer`: **Core simulation logic** - orchestrates the rotation sequence by generating a list of `Pull` objects, each representing one complete rotation cycle
  - `CsvParser`: Parses rider input (7 fields: name, weight, FTP, pull duration, then 4 power values for positions 1-4+)
  - Exporters: Generate output files (ZWO XML workouts, PNG visualizations) from `Pull` structures

### Critical Pattern: Hierarchical Rotation Model

`RotationComposer.CreatePullsList()` generates a `List<Pull>` representing the entire rotation sequence. Each `Pull` contains all riders in their current positions with durations and target powers determined by their position and the pulling rider's pull duration. The key insight:

- **One Pull = One Complete Rotation Cycle**: A pull groups all riders simultaneously, reflecting the physical reality of team time trials
- **Position-Based Power**: Each rider's power in a pull is determined by their current position via `RiderPowerPlan.GetPowerByPosition(position)` — not by rider identity
- **Position Clamping**: Lookups beyond position 4 return the last defined power value (see `RiderPowerPlanTests.cs`)

## Coding Standards

### Naming & Structure

- **PascalCase** for classes/methods/properties; **camelCase** for locals/parameters
- **One class per file** matching the class name (`RiderData.cs`, not grouped files)
- **File-scoped namespaces** only; never use block-scoped
- No regions; keep methods focused (~20 line max before refactoring)

### Testing the Domain Model

- **xUnit framework** for tests; use descriptive test names indicating scenario
- **RotationComposerTests**: Comprehensive theory and fact tests covering:
  - Pull count verification (riders × rotations)
  - Power assignment correctness by position
  - Duration tracking per pull
  - Position clamping edge cases (position 4+)
  - Multi-rotation cycle repeatability
- Tests must be independent and runnable in any order
- Theory tests should cover meaningful team size/rotation combinations (e.g., 4/6/8 riders × 1-3 rotations)

### Documentation

- **XML doc comments** for all public members describing purpose, parameters, returns

### Error Handling

- Use meaningful exception messages for invalid inputs (e.g., CsvParser validates 7+ CSV fields per line)
- Throw custom exceptions when appropriate; avoid generic Exception
- Validate inputs early in business logic (see TODO comments in `RotationComposer`)

## Development Workflows

**Build & Run:**
```bash
dotnet run --project ZwiftTTTSim.CLI -- --input sample-riders.csv
```

**Run Tests:**
```bash
dotnet test
```

**Generate workouts with custom settings:**
```bash
dotnet run --project ZwiftTTTSim.CLI -- -i my-riders.csv -o output/ -r 10
```

Output workouts appear in `workouts/` folder as `{RiderName}_TTT_Workout.zwo` files.

## Important: System.CommandLine Version

⚠️ **This project is pinned to `System.CommandLine` version `2.0.0-beta4.22272.1`** for a specific reason:

- **Do NOT upgrade** to versions 2.0.2, 2.1.x, or later
- Beta5 introduced breaking API changes that broke the CLI
- Later "stable" releases continue to have API instability
- See [dotnet/command-line-api#2576](https://github.com/dotnet/command-line-api/issues/2576) for details

Beta4 remains the most stable version despite being labeled as "beta". It has a well-tested, consistent API. When `System.CommandLine` 3.0 or a true stable release becomes available, consider upgrading after thorough testing.