# GeometricAlgebraFulcrumLib Unit Tests

_Last updated: 2025-11-14_

## Status

- `DOTNET_ROOT="$(pwd)/.dotnet8" DOTNET_SYSTEM_GLOBALIZATION_CULTURE=de-DE DOTNET_SYSTEM_GLOBALIZATION_UI_CULTURE=de-DE \`
  `$DOTNET_ROOT/dotnet test GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj -c Release`
  builds successfully but currently fails **24 tests**:
  - Pure rotor regression suite (`LinearMaps/RotorsTests.cs`) – `CreatePureRotor`
    now throws `ArgumentException`.
  - Adaptive curve sampler parity (`Modeling/Trajectories/CurveSamplers3DEquivalenceTests.cs`)
    – Float64 and generic samplers produce different node counts.
  - Trajectory equivalence tests (`AdaptiveArcLength…`, `RotatedNormals…`,
    `RouletteMapped…`, `RoulettePath3DEquivalenceTests.cs`) – derivatives and
    frames no longer match their Float64 baselines.
  - `Utilities/Text/StringBuilderExtensionsTests` – still expect decimal commas.
  - `Utilities/Text/StringUtilsTests` – rely on Windows-only invalid filename
    characters (`<`, `>`, `:`), so they fail under Linux.
- Detailed notes: `docs/status/ISSUES_TO_FIX.md` and
  `docs/status/TODO_TEST_COVERAGE.md`.

## Running the Suites

```bash
cd GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests

# After exporting DOTNET_ROOT / DOTNET_CLI_HOME as described in AGENTS.md
dotnet test -c Release              # full run (currently fails during build)
dotnet test -c Release --filter Algebra
dotnet test -c Release --filter Modeling --no-build
dotnet watch test --filter Modeling # fast iteration on the problematic suite
```

## Suite Overview

| Folder | Notes |
|--------|-------|
| `Algebra/` | Core GA products, processors, and regression tests. |
| `LinearMaps/` | Rotors, reflectors, versors, outermorphisms. |
| `AutoDiff/` | Automatic differentiation and symbolic simplification. |
| `Modeling/` | CGA/PGA modeling, trajectories, and float64 vs generic parity tests (current blocker). |
| `Processing/`, `Storage/`, `Utilities/` | Support libraries (bit utils, storage backends, pipelines). |

All suites use NUnit; new files should follow the `<Feature>Tests.cs` naming
pattern, mirrored namespace structure, and explicit tolerance assertions (e.g.,
`Assert.That(x, Is.EqualTo(expected).Within(1e-12))`).

## References

- General instructions: `AGENTS.md`, `docs/guides/DEVELOPMENT_GUIDE.md`
- Status dashboards: `docs/status/*.md`
- Documentation hub: `DOCUMENTATION_INDEX.md`
