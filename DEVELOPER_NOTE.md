# Developer Note: Climb-Aware Pacing Infrastructure

## Architectural Choices & Model Foundation (#49)

To support JSON serialization correctly across varying segment definitions, the segment entities have been designed with simple, primitive properties equipped with public getters and setters. This object-oriented design directly fulfills the requirement specified in issue #49, establishing a serialization-ready model foundation without requiring complex custom JSON converters that may hinder future development.

## Unifying Mixed Segment Types (#55)

The `ISegment` interface was introduced to unify different types of race sections. By standardizing on `TimeSpan Duration { get; }`, the application can predictably query the expected time block of any sequence regardless of its topological or physiological demands. This enables the simulation pipeline to aggregate various route layouts—accommodating the "mixed segment types" outlined in issue #55—while adhering to the Open-Closed Principle. New segment models, such as sprint sections, can be introduced in the future by simply implementing `ISegment`.

## Internal Normalization (#52)

Currently, the simulated paceline expects discrete time intervals to compose rotations properly and allocate exertion periods constraint bounds. `FlatSegment` dynamically resolves target speeds and distances into uniform `TimeSpan` outputs, embedding a guard clause to safely handle zero or negative velocity inputs. Conversely, `ClimbSegment` wraps fixed-time objectives directly from literal seconds. By encapsulating these calculations within the models themselves, the core logic guarantees a normalized interpretation across the `PacelinePlanComposer` before workout projection occurs (#52).

## Next Steps

Integration with parsers should evaluate introducing a strategy pattern or generic converter logic to dynamically yield `ISegment` derivatives. Furthermore, `PacelinePlanComposer` logic will need to be refactored to gracefully segment internal rotations using iterative blocks bound to the cumulative sum of normalized segment durations.

## Race CLI Integration (#57)

The CLI has been updated to accept a targeted race plan via the `--race <file>` flag. Providing this flag overrides any simple `--rotations` configuration globally, effectively unifying segment management and file parsing.

### Race File JSON Sample

```json
{
  "name": "WTRL TTT - Greater London Flat",
  "route": [
    {
      "type": "flat",
      "distanceKm": 5.5,
      "avgSpeedKph": 43.0
    },
    {
      "type": "climb",
      "durationSeconds": 300
    },
    {
      "type": "flat",
      "distanceKm": 10.0,
      "avgSpeedKph": 45.0
    }
  ]
}
```

Internally, the `RaceConfigParser` uses a `JsonConverter<ISegment>` discriminative factory within `System.Text.Json`, isolating the casting logic from standard implementation leaks ensuring compatibility across the simulator modules.

