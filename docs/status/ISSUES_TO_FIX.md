# Issues to Fix

_Last updated: 2025-11-14_

```
export DOTNET_ROOT="$(pwd)/.dotnet8"
export PATH="$DOTNET_ROOT:$PATH"
export DOTNET_CLI_HOME="$(pwd)/.dotnet-cli-home"
export NUGET_HTTP_CACHE_PATH="$DOTNET_CLI_HOME/.nuget/http-cache"
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_SYSTEM_GLOBALIZATION_CULTURE=de-DE
export DOTNET_SYSTEM_GLOBALIZATION_UI_CULTURE=de-DE
$DOTNET_ROOT/dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj -c Release
```

Build succeeds, but **24 tests fail**. The sections below group the failures by
root cause. Paths/line numbers refer to the current repo checkout.

---

## 1. Pure Rotor Regression Suite (8 tests)

**Tests:** `Rotation_PreservesNorm`, `PureRotor_InverseUndoesRotation`,
`PureRotor_PreservesKVectorGrade`, `PureRotor_PreservesNorm`,
`PureRotor_PreservesOuterProduct`, `PureRotor_PreservesScalarProduct`,
`PureRotor_RotatesSourceToTarget`,
`PureRotor_RotorCondition_RTimesReverseEqualsOne`.

**Files:** `GeometricAlgebraFulcrumLib.UnitTests/Algebra/ProcessorSpecificTests.cs`
line 134 and `LinearMaps/RotorsTests.cs` lines 53–223.

**Failure:** `XGaFloat64Vector.CreatePureRotor(targetVector, assumeUnitVectors)`
throws `ArgumentException ("value")` because the randomly generated source and
target vectors occasionally cannot produce a valid rotor (non-unit,
co-linear, or zero norm). All affected tests abort before asserting anything.

**Next steps:**
1. Update the rotor test helpers to normalize the random vectors and guard
   against degenerate cases.
2. Investigate whether `CreatePureRotor` should return a diagnostic instead of
   throwing an unhandled exception when inputs are invalid.

---

## 2. Adaptive Curve Sampler Divergence (2 tests)

**Tests:** `CurveSamplers3DEquivalenceTests.TestAdaptiveCurveSampler_Count`
(expected 55 nodes, got 54) and
`CurveSamplers3DEquivalenceTests.TestAdaptiveCurveSampler_GetPoints`
(expected 53 sampled points, got 52).

**Files:** `GeometricAlgebraFulcrumLib.UnitTests/Modeling/Trajectories/CurveSamplers3DEquivalenceTests.cs`
lines 82 and 117.

**Failure:** The generic sampler drops one level of subdivision compared to the
Float64 sampler, so the point counts differ.

**Next steps:**
1. Compare `Float64AdaptivePath3D` vs `AdaptivePath3D<T>` generation loops,
   especially around max level and edge-frame tolerances.
2. Once the sampler logic matches, re-run both tests to ensure counts and point
   arrays line up.

---

## 3. Trajectory Equivalence (4 tests)

**Tests:**  
`RotatedNormalsPath3DEquivalenceTests.RotatedNormalsPath3D_ConstantAngle_ShouldMatchFloat64`  
`RotatedNormalsPath3DEquivalenceTests.RotatedNormalsPath3D_FunctionAngle_ShouldMatchFloat64`  
`RouletteMappedPath3DEquivalenceTests.RouletteMappedPath3D_ShouldMatchFloat64`  
`RoulettePath3DEquivalenceTests.RoulettePathMatchesFloat64Baseline`

**Files:** `GeometricAlgebraFulcrumLib.UnitTests/Modeling/Trajectories/*.cs`
lines 85–136.

**Failure:** Generic path frames/derivatives no longer match the Float64
baselines (e.g., Normal1 differs by 0.72 in the constant-angle test, and
roulette derivatives diverge by ~10.6). These are genuine behavioral
regressions rather than assertion issues.

**Next steps:**
1. Instrument the generic implementations (`RotatedNormalsPath3D<T>`,
   `RouletteMappedPath3D<T>`, `RoulettePath3D<T>`) to log intermediate frames
   and confirm whether angles/vectors are normalized like the Float64 versions.
2. Once fixed, keep the assertions comparing scalars via `.ScalarValue` to
   avoid the previous tolerance exceptions.

---

## 4. StringBuilder Formatting (6 tests)

**Tests:** `Append_WithFormat_BothParts_ShouldFormatCorrectly`,
`Append_WithFormat_ImaginaryOnly_ShouldFormatCorrectly`,
`Append_WithFormat_RealOnly_ShouldFormatCorrectly`,
`Append_WithFormat_ShouldSupportChaining`,
`AppendComplexNumber_WithFormat_ShouldFormatNumbers`,
`AppendLine_WithFormat_ShouldFormatAndAddNewline`
in `Utilities/Text/StringBuilderExtensionsTests.cs`.

**Failure:** Tests look for decimal commas (`"1,5"`, `"5,1"`), but the current
`StringBuilderExtensions` implementation formats numbers using the invariant
culture, producing `"1.5"` and `"5.1"`.

**Next steps:**
1. Decide whether these APIs should honor `CultureInfo.CurrentCulture` or stick
   to invariant formatting.  
2. If culture-aware formatting is desired, pass the supplied `IFormatProvider`
   down to every `Append...` helper; otherwise, update the tests to assert the
   invariant output.

---

## 5. File/Path Sanitizers on Linux (3 tests)

**Tests:** `ToValidFileName_RemovesInvalidCharacters`,
`ToValidFileName_WithCustomReplaceChar`,
`ToValidPath_RemovesInvalidCharacters` in
`Utilities/Text/StringUtilsTests.cs`.

**Failure:** Linux allows characters (`<`, `>`, `:`) that are illegal on Windows.
`Path.GetInvalidFileNameChars()` therefore returns a different set, so the
tests that assert `<` is removed fail on this platform.

**Next steps:**
1. Make the tests conditional on `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)`
   or update the implementation to always remove a superset of invalid chars.
2. Record the platform-specific behavior in `docs/status/KNOWN_ISSUES_AND_SOLUTIONS.md`.

---

Keep this file updated whenever the failing test set changes. Historical
results live under `docs/status/archive/`.
