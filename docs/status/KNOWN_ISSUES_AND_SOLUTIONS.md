# Known Issues & Workarounds

_Last updated: 2025-11-14_

This file lists the non-blocking problems we routinely hit while working on the
repo. Blocking compilation issues belong in `docs/status/ISSUES_TO_FIX.md`.
Historical write-ups live under `docs/status/archive/`.

## Nullability Noise from Generated Three.js Factories

- **Where**: `GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/Graphics/Rendering/ThreeJs/ThreeJsObjectFactory.cs`
  (`~13k` lines).
- **Symptom**: Hundreds of `CS8625` warnings ("Cannot convert null literal to
  non-nullable reference type") during every build/test run.
- **Cause**: The file is generated from the BabylonJS/ThreeJS metadata and still
  assumes nullable reference types are disabled.
- **Workaround**: Ignore the warnings for now. When touching the generator,
  switch nullable annotations on and set default values for `_uuid`, `_name`,
  `_userData`, etc.

## NuGet Vulnerability Data Permission Errors

- **Where**: restore phase for any project that references NuGet feeds.
- **Symptom**: `warning NU1900` complaining that
  `/home/schmidt/dotnet-cli-home/.local/share/NuGet/http-cache/.../vuln_index.dat-new`
  cannot be created.
- **Cause**: The sandboxed `DOTNET_CLI_HOME` path is not writable on this
  machine.
- **Workaround**: Export the environment variables suggested in `AGENTS.md`
  (`DOTNET_CLI_HOME`, `DOTNET_ROOT`, `NUGET_HTTP_CACHE_PATH`) before running
  `dotnet restore`, or set `DOTNET_DISABLE_VULNERABILITY_CHECK=1` for CI.

## Float64 vs Generic Trajectory Parity

- **Where**: `GeometricAlgebraFulcrumLib.UnitTests/Modeling/Trajectories/*EquivalenceTests.cs`
- **Symptom**: The generic/float64 comparison helpers still assume every
  float64 API returns raw `double` values. In reality, many float64 trajectory
  classes expose `Float64Scalar` or custom frame types.
- **Workaround**: When adding new tests, convert `Float64Scalar` to `double`
  via `.ScalarValue`, and convert `LinFloat64Normal3D` to vectors before
  comparing them. This keeps the compiler happy until the helpers are updated.

## Decimal Formatting Expectations

- **Where**: `GeometricAlgebraFulcrumLib.UnitTests/Utilities/Text/StringBuilderExtensionsTests.cs`
- **Symptom**: Tests expect decimal commas (e.g., `"1,5"`) but the current
  implementation appends numbers using invariant culture, producing `"1.5"`.
- **Workaround**: Either run the tests with a culture that uses commas _and_
  update the formatter to honor `CultureInfo`, or adjust the tests to accept
  invariant output. For now we simply document the discrepancy; see
  `docs/status/ISSUES_TO_FIX.md` for follow-up work.

## Windows-Only Invalid Filename Characters

- **Where**: `GeometricAlgebraFulcrumLib.UnitTests/Utilities/Text/StringUtilsTests.cs`
- **Symptom**: Linux allows `<`, `>` and `:` in file names, so the tests that
  assert those characters are stripped (using `Path.GetInvalidFileNameChars()`)
  fail on this platform.
- **Workaround**: Run the filename/path tests only on Windows or normalize the
  implementation to always remove the superset of invalid characters regardless
  of OS.

## Archive

- Previous "Known Issues & Solutions" log:
  `docs/status/archive/KNOWN_ISSUES_AND_SOLUTIONS.archive.md`
