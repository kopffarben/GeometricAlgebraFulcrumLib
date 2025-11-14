# Test Coverage Roadmap

_Last updated: 2025-11-14_

The legacy 1,900‑line plan has been archived in
`docs/status/archive/TODO_TEST_COVERAGE.archive.md`. This trimmed version tracks
what matters right now: getting the modeling suites _correct_ again and
re-gaining confidence in the generic vs float64 parity tests.

## Current State

- **Build**: `DOTNET_ROOT="$(pwd)/.dotnet8" DOTNET_SYSTEM_GLOBALIZATION_CULTURE=de-DE DOTNET_SYSTEM_GLOBALIZATION_UI_CULTURE=de-DE \`
  `$DOTNET_ROOT/dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj -c Release`
  completes, but **24 tests fail** (see `docs/status/ISSUES_TO_FIX.md`).
- **Stable suites**: `AutoDiff/`, `Storage/`, and most of `Utilities/`
  continue to pass on Linux.
- **In-flight suites**:
  - `Algebra/ProcessorSpecificTests` + `LinearMaps/RotorsTests` (pure rotor regression).
  - `Modeling/Trajectories` (adaptive sampler + rotated normals/roulette equivalence).
  - `Utilities/Text` (culture-specific formatting and Windows-only filename sanitizers).

## Focus Areas

1. **Pure Rotor Coverage**
   - Patch `XGaFloat64Vector.CreatePureRotor` or the associated tests so that
     random source/target vectors always yield a valid rotor.
   - Keep the algebra tests enabled so we regain confidence in the Float64
     processor invariants.
2. **Adaptive Sampler Parity**
   - Align `AdaptivePath3D<T>` with the Float64 implementation so that node
     counts and sampled points match.
3. **Trajectory Equivalence**
   - Fix the `RotatedNormalsPath3D` and roulette parity tests before adding
     more CGa/PGA scenarios. These tests are the gatekeepers for generic vs
     Float64 behavior.
4. **Text Helpers**
   - Decide on culture-aware formatting and platform-specific filename rules,
     then update both code and tests accordingly.

## How to Measure Progress

- Run `dotnet test ... -c Release --no-restore --filter Modeling` locally to
  iterate on the trajectory issues without waiting for the full suite.
- Track the pass/fail/skip numbers (and git revision) whenever the status
  changes.
- When the full test command passes again, update the stats in
  `README.md`, `GeometricAlgebraFulcrumLib.UnitTests/README.md`, and the
  documentation site.

## References

- Archived long-form plan: `docs/status/archive/TODO_TEST_COVERAGE.archive.md`
- Current failures: `docs/status/ISSUES_TO_FIX.md`
- Known runtime quirks: `docs/status/KNOWN_ISSUES_AND_SOLUTIONS.md`
