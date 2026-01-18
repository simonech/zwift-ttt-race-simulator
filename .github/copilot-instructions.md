# Coding guidelines for Zwift TTT race simulator

---
description: Comprehensive coding guidelines and architecture patterns for AI agents contributing to the Zwift TTT race simulator project.
applyTo: **/*.cs
---

## 🏗️ Architecture Overview

This project generates **one workout file per rider** simulating a Team Time Trial rotation. The data flow is:

**CSV Input** → **CsvParser** (parse rider data) → **WorkoutCreator** (simulate rotation) → **ZwoExporter** / **ImageExporter** (output)

### Key Components

- **Model layer** (`ZwiftTTTSim.Core/Model/`): Data structures for rider state and workouts
  - `RiderPowerPlan`: Encodes a rider's pull duration and power targets for each rotation position (1st=pulling, 2nd, 3rd, 4th+)
  - `WorkoutStep`: Duration + power pair representing a single interval in a rider's workout
  - `RiderData`: Rider metadata (FTP, weight) used to calculate intensity as power/FTP ratio
  
- **Services layer** (`ZwiftTTTSim.Core/Services/`): Business logic
  - `WorkoutCreator`: **Core simulation logic** - rotates riders through positions, assigning power targets per position per step
  - `CsvParser`: Parses rider input (7 fields: name, weight, FTP, pull duration, then 4 power values for positions 1-4+)
  - Exporters: Generate output files (ZWO XML workouts, PNG visualizations)

### Critical Pattern: Position-Based Power Assignment

`WorkoutCreator.CreateWorkouts()` rotates riders through a paceline, moving the pull leader to the back each rotation. **Critically**, each rider's power for each step is determined by their current position via `GetPowerByPosition(position)` — not by who they're following. See `RiderPowerPlanTests.cs` for position lookups beyond position 4 returning the last defined power value.

## Coding Standards

### Naming & Structure

- **PascalCase** for classes/methods/properties; **camelCase** for locals/parameters
- **One class per file** matching the class name (`RiderData.cs`, not grouped files)
- **File-scoped namespaces** only; never use block-scoped
- No regions; keep methods focused (~20 line max before refactoring)

### Documentation & Testing

- **XML doc comments** for all public members describing purpose, parameters, returns
- **xUnit framework** for tests; use descriptive test names indicating scenario
- Write tests for all public methods and critical paths (e.g., rotation logic edge cases, position lookups beyond 4th rider)
- Tests must be independent and runnable in any order

### Error Handling

- Use meaningful exception messages for invalid inputs (e.g., CsvParser validates 7+ CSV fields per line)
- Throw custom exceptions when appropriate; avoid generic Exception
- Validate inputs early in business logic (see TODO comments in `WorkoutCreator`)

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