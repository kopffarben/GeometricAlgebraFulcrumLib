# Statistics API Comparison: Float64 vs Generic

**Analysis Date:** 2025-10-23
**Module:** GeometricAlgebraFulcrumLib.Modeling/Statistics
**Purpose:** Compare Float64 vs Generic implementations for API consistency

---

## Executive Summary

The Statistics module is **100% Float64-only** with **NO generic implementations**. Unlike other modules in GA-FuL, this module uses only `double` types throughout and does not support the generic scalar abstraction pattern used elsewhere in the library.

**Key Findings:**
- ✅ **Consistency:** All statistics code uses Float64 exclusively
- ❌ **No Generic Version:** Missing generic scalar support
- ⚠️ **Critical Bugs Found:** 3 serious bugs discovered
- ⚠️ **Precision Issues:** Multiple numerical stability concerns
- ✅ **Architecture:** Well-structured with continuous and discrete separation

---

## Module Structure

### Directory Layout
```
Statistics/
├── Root Level (4 utility files)
│   ├── CumulativeDistributionFunction.cs         - Float64 only
│   ├── RandomEuclideanVectorsComposer.cs         - Float64 only
│   ├── RandomGaMultivectorComposer.cs            - COMMENTED OUT (legacy)
│   └── RandomUtils.cs                            - Float64 only
│
├── Continuous/ (8 files)
│   ├── HistogramBinData.cs                       - Float64 only
│   ├── PiecewiseAffineFunction.cs                - Float64 only (large, 846 lines)
│   ├── ProbabilityDistributionFunction.cs        - Float64 only
│   ├── QuantizedHistogram.cs                     - Float64 + ulong (1549 lines)
│   ├── QuantizedHistogramBinData.cs              - Float64 + ulong
│   ├── QuantizedHistogramPdf.cs                  - Float64 only
│   ├── SparseIrregularHistogram.cs               - Float64 only
│   └── SparseRegularHistogram.cs                 - Float64 only (1510 lines)
│
└── Discrete/ (3 files)
    ├── DiscreteProbabilityFunction.cs            - Float64 only (abstract base)
    ├── DiscreteProbabilityMassFunction.cs        - Float64 only (760 lines)
    └── PmfRandomGenerator.cs                     - Float64 only
```

---

## API Analysis by Component

### 1. Root Level Utilities

#### CumulativeDistributionFunction
**Type:** Float64 only
**Purpose:** Cumulative distribution function implementation
**Key Methods:**
- `GetProbability(double value) -> double`
- `ProbabilityToValue(double probability) -> double`
- `GenerateValue(Random randGen) -> double`

**Bugs Found:**
```csharp
// BUG #1: Line 67 - Logic error in condition
if (value >= DomainMinValue) return 1d;  // Should be DomainMaxValue!

// BUG #2: Line 129 - Division by zero (same numerator and denominator)
var t = (p2 - p1) / (p2 - p1);  // Always equals 1!
// Should be: var t = (probability - p1) / (p2 - p1);
```

#### RandomEuclideanVectorsComposer
**Type:** Float64 only
**Purpose:** Generate random 2D/3D vectors using polar/spherical coordinates
**Key Methods:**
- `GetRandomUnitVector2Dp() -> LinFloat64Vector2D`
- `GetRandomUnitVector3Dp() -> LinFloat64Vector3D`
- `GetRandomVector2Dp() -> LinFloat64Vector2D` (with radius)
- `GetRandomVector3Dp() -> LinFloat64Vector3D` (with radius)

**Properties:**
- `RMin, RMax` - Radius range
- `ThetaMin, ThetaMax` - Polar angle range (0 to π)
- `PhiMin, PhiMax` - Azimuthal angle range (0 to 2π)

**Note:** Uses `MathNet.Numerics.Constants` for π values.

#### RandomGaMultivectorComposer
**Status:** Entirely commented out (254 lines)
**Note:** Legacy code from older GA implementation, not in use.

#### RandomUtils
**Type:** Float64 only
**Purpose:** Extension methods for random geometry generation
**Key Methods:**
- `NormalizeProbabilities(SortedDictionary<int, double>) -> SortedDictionary<int, double>` (internal)
- `ScaleProbabilities(SortedDictionary<int, double>, double) -> SortedDictionary<int, double>` (internal)
- `GetUnitVector3D(Random) -> LinFloat64Vector3D`
- `GetLineSegmentInside(Random, IFloat64BoundingBox2D) -> Float64LineSegment2D`
- `GetTriangleInside(Random, IFloat64BoundingBox2D) -> Float64Triangle2D`
- `GetTrianglesInside(Random, int, IFloat64BoundingBox2D) -> List<Float64Triangle2D>`

---

### 2. Continuous Statistics

#### HistogramBinData
**Type:** Float64 only (record type)
**Purpose:** Immutable data holder for histogram bin information
**Properties:**
- `Index: int`
- `MidValue: double` (validated: not NaN/Infinite)
- `Width: double` (validated: not NaN/Infinite, >= 0)
- `Height: double` (validated: not NaN/Infinite, >= 0)
- Computed: `MinValue`, `MaxValue`, `HalfWidth`, `Area`

**Methods:**
- `Contains(double value) -> bool`
- `GetLengthBefore/After/Between(double...) -> double`
- `GetAreaBefore/After/Between(double...) -> double`

#### PiecewiseAffineFunction
**Type:** Float64 only
**Purpose:** Piecewise linear function with discontinuities support
**Size:** 846 lines (large, complex)

**Inner Types:**
- `Breakpoint` - Function discontinuity/kink point
  - `X, Y, YMinus, YPlus: double`
  - Properties: `IsSymmetric`, `IsDiscrete`, `IsContinuous`, `IsLeftContinuous`, `IsRightContinuous`
- `Sample` - Simple (X, Y) pair
- `Segment` - Linear segment between breakpoints

**Static Factories:**
- `CreateContinuous(IReadOnlyList<double> xValues, IReadOnlyList<double> yValues)`
- `CreateContinuous(..., double angleTolerance)` - Adaptive sampling
- `CreateContinuous(Func<double, double> smoothFunc, ...)` - Sample smooth function

**Operations:**
- `GetValue(double x) -> double`
- `InsertBreakpoint(...)` - Mutable builder pattern
- `MakeFinite()`, `MakeOdd()`, `MakeEven()`
- `ShiftX(double deltaX)`, `ScaleY(double scalingFactor)`
- `GetArea() -> double`
- `GetMatlabCode(...) -> string` - Visualization support

**Properties:**
- `BreakpointCount`, `SampleCount`, `SegmentCount`
- `MinBreakpointX`, `MaxBreakpointX`, `LengthX`
- `IsFinite` - Checks endpoints are zero
- Enumerables: `XValues`, `YValues`, `Breakpoints`, `Samples`, `Segments`

#### ProbabilityDistributionFunction
**Type:** Float64 only
**Purpose:** Thin wrapper around PiecewiseAffineFunction for PDFs
**Factory:**
- `CreateNormal(double mean, double variance, double zeroEpsilon = Float64Utils.ZeroEpsilon)`

**Process:**
1. Compute range where PDF > zeroEpsilon (adaptive)
2. Sample Gaussian function
3. Create piecewise affine approximation
4. Normalize area to 1.0

#### QuantizedHistogram
**Type:** Float64 + ulong (heights are quantized integers)
**Purpose:** Memory-efficient histogram using integer counts
**Size:** 1549 lines (very large)

**Storage:** `SortedDictionary<int, ulong>` - Sparse bin storage

**Static Factories:**
- `CreateEmpty(double domainFirst, double domainLast, int binCount)`
- `CreateFromHistogram(double..., IReadOnlyDictionary<double, double>)`
- `CreateFromRandomSamples(double..., Random, int sampleCount = 10M)`
- `CreateUniform(double..., int binCount = 1, ulong binHeight = 1)`
- `CreateNormal(double mean, double stdDev, int binCount, int quantizationBits = 32)`
- `CreateExponential(double rate, int binCount, int quantizationBits = 32)`

**Operators Overloaded:**
- `+, -, *, /` - Arithmetic with scalars and other histograms

**Bin Operations:**
- `GetBinIndexContaining(double) -> int`
- `GetBinMidValue(int index) -> double`
- `GetBinHeight(int index) -> ulong`
- `SetBinHeight(int index, ulong height)`
- `AddBinHeight(int index, ulong heightDelta)`

**Domain Operations:**
- `ResetDomainRange(double, double)`
- `FlipDomain()`, `ShiftDomain(double delta)`
- `MapDomainUsingAffine(double scale, double offset)`
- `MapDomain(Func<double, double> map, int binCount)`
- `MapDomain(QuantizedHistogram, Func<double, double, double>, int binCount)`

**Height Operations:**
- `MapHeights(Func<ulong, ulong>)`
- `ScaleHeights(ulong factor)`
- `TrimHeights(double zeroEpsilon)`
- `TrimHeightsByArea(double zeroAreaRatioEpsilon)`

**Bin Management:**
- `PrependBins(int count)`, `AppendBins(int count)`
- `TrimBins()`, `TrimFirstBins()`, `TrimLastBins()`

**Statistics:**
- `GetMean()`, `GetVariance()`, `GetStandardDeviation()`, `GetRelativeStandardDeviation()`
- `GetSkewness()`, `GetSkewnessCoefficient()`
- `GetKurtosisCoefficient()`, `GetExcessKurtosisCoefficient()`
- `GetMoment(int n)`, `GetExpectedValue(Func<double, double>)`

**Conversion:**
- `GetProbabilityDensityFunction() -> PiecewiseAffineFunction`
- `GetCumulativeDensityFunction() -> PiecewiseAffineFunction`
- `GetSurvivorFunction() -> PiecewiseAffineFunction`
- `GetInverseDistributionFunction() -> PiecewiseAffineFunction`
- `CreatePdf() -> QuantizedHistogramPdf`

**Visualization:**
- `GetPdfMatlabCode() -> string`
- `GetCdfMatlabCode() -> string`
- `GetIdfMatlabCode() -> string`

**Bug Found:**
```csharp
// BUG #3: Lines 1078, 1104 - Wrong method (Min should be Max)
var domainFirstValue = sparseDictionary.Keys.Min();
var domainLastValue = sparseDictionary.Keys.Min();  // BUG: Should be Max()!
```

#### QuantizedHistogramBinData
**Type:** Float64 + ulong (record type)
**Purpose:** Data holder for quantized histogram bins
**Properties:**
- `Index: int`
- `MidValue, Width: double`
- `Height, HistogramSum: ulong`
- Computed: `NormalizedHeight` (as double)

**Methods:**
- Area/Height queries: `GetHeightBefore/After/Between(double...) -> ulong`
- Width queries: `GetWidthBefore/After/Between(double...) -> double`
- Normalized area: `GetNormalizedAreaBefore/After/Between(double...) -> double`

#### QuantizedHistogramPdf
**Type:** Float64 only
**Purpose:** Random generator from quantized histogram
**Inherits:** `Random` (overrides `NextDouble()`)

**Process:**
1. Compute inverse CDF as `PiecewiseAffineFunction`
2. Generate uniform random [0, 1]
3. Map through inverse CDF to get distributed value

#### SparseIrregularHistogram
**Type:** Float64 only
**Purpose:** Histogram with variable-width bins
**Storage:** `List<HistogramBinData>` - Unsorted, irregular bins

**Methods:**
- `AddBin(double midValue, double width, double height)`
- `GetBinsContaining(double domainValue) -> IEnumerable<HistogramBinData>`
- `GetArea() -> double`
- `GetAreaBefore/After/Between(double...) -> double`

**Note:** Simple, lightweight design for irregular binning.

#### SparseRegularHistogram
**Type:** Float64 only
**Purpose:** Normalized histogram with regular spacing
**Size:** 1510 lines (very large)
**Storage:** `SortedDictionary<int, double>` - Sparse bin storage

**Key Difference from QuantizedHistogram:**
- Uses `double` heights (probabilities) instead of `ulong` counts
- Heights automatically normalized to sum to 1.0
- Similar API but focused on probability distributions

**Static Factories:**
- `CreateEmpty(double, double, int binCount)`
- `CreateFromHistogram(SparseIrregularHistogram, int binCount)`
- `CreateFromHistogram(IReadOnlyDictionary<double, double>, int binCount)`
- `CreateFromRandomSamples(Random, double, double, int binCount, int sampleCount = 10M)`
- `CreateUniform(double, double, int binCount = 1)`
- `CreateNormal(double mean, double stdDev, int binCount, double zeroEpsilon)`
- `CreateExponential(double rate, int binCount, double zeroEpsilon)`

**Operators:** `+, -, *, /` (same as QuantizedHistogram)

**Similar API to QuantizedHistogram but with:**
- `double` heights instead of `ulong`
- `NormalizeHeights()` method
- Probability-focused operations

**Conversion:**
- All same function conversion methods as QuantizedHistogram
- Returns `PiecewiseAffineFunction` for PDF/CDF/Survivor/Inverse

---

### 3. Discrete Statistics

#### DiscreteProbabilityFunction (Abstract Base)
**Type:** Float64 only
**Purpose:** Base class for discrete probability functions
**Storage:** `SortedDictionary<int, double>` - Index to probability

**Properties:**
- `DomainFirstValue, DomainLastValue: double`
- `DomainSampleCount: int`
- `DomainResolution: double` - Spacing between samples
- `DomainSize, DomainMinValue, DomainMaxValue: double`
- `ValueProbabilityPairs: IEnumerable<Pair<double>>`

**Methods:**
- `abstract IsValid() -> bool`
- `GetDomainValue(int sampleIndex) -> double`
- `GetMatlabCode() -> string`

**Implements:** `IReadOnlyList<double>` (index access to probabilities)

#### DiscreteProbabilityMassFunction
**Type:** Float64 only
**Purpose:** Discrete PMF with standard distributions
**Size:** 760 lines

**Inherits:** `DiscreteProbabilityFunction`

**Static Factories:**
- `CreateFromHistogram(IReadOnlyDictionary<double, double>, int sampleCount)`
- `CreateFromHistogram(Random, double first, double last, int sampleCount, int samples = 10M)`
- `CreateUniform(double first, double last, int sampleCount)`
- `CreateBinomial(int trialCount, double successProb, double zeroEpsilon)`
- `CreatePoisson(double mean, double zeroEpsilon)`
- `CreateNormal(double mean, double stdDev, int sampleCount, double zeroEpsilon)`
- `CreateExponential(double rate, int sampleCount, double zeroEpsilon)`

**Operators:** `+, -, *, /` (scalar and PMF-to-PMF)

**Probability Queries:**
- `GetProbability(Func<double, bool> condition) -> double`
- `GetProbability(double maxValue) -> double` - P(X ≤ max)
- `GetProbability(double min, double max) -> double` - P(min ≤ X ≤ max)

**Domain Operations:**
- `ResetDomain(double first, double last)`
- `ShiftDomain(double delta)`
- `MapDomain(Func<double, double>, int sampleCount)`
- `MapDomain(DiscreteProbabilityMassFunction, Func<double, double, double>, int sampleCount)`
- `JoinDomain(DiscreteProbabilityMassFunction, int sampleCount)` - Merge two PMFs

**Arithmetic:**
- `Negative()`, `Inverse()`
- `Add(double)`, `Subtract(double)`, `Times(double)`, `Divide(double)`
- `Add(PMF)`, `Subtract(PMF)`, `Times(PMF)`, `Divide(PMF)`

**Statistics:** (Same as QuantizedHistogram)
- `GetMean()`, `GetVariance()`, `GetStandardDeviation()`, `GetRelativeStandardDeviation()`
- `GetSkewness()`, `GetSkewnessCoefficient()`
- `GetKurtosisCoefficient()`, `GetExcessKurtosisCoefficient()`
- `GetMoment(int n)`, `GetExpectedValue(Func<double, double>)`

**Conversion:**
- `GetInverseCdfArray(int sampleCount = 2049) -> IReadOnlyList<double>`
- `GetCdf() -> CumulativeDistributionFunction`

**Validation:**
```csharp
public override bool IsValid()
{
    return !DomainFirstValue.IsNaNOrInfinite() &&
           !DomainLastValue.IsNaNOrInfinite() &&
           (DomainFirstValue - DomainLastValue).Abs() > 1e-12 &&
           SampleProbabilityDictionary.Count >= 2 &&
           SampleProbabilityDictionary.Keys.First() == 0 &&
           SampleProbabilityDictionary.Keys.Last() == DomainSampleCount - 1 &&
           SampleProbabilityDictionary.Values.Sum().IsNearOne(1e-7) &&
           SampleProbabilityDictionary.All(p => p is { Key: >= 0, Value: > 0 });
}
```

**Bug in MapDomain:**
```csharp
// Line 492 - Should be multiplication, not addition!
var p = p1 + p2;  // BUG: Should be p1 * p2 for convolution!
```

#### PmfRandomGenerator
**Type:** Float64 only
**Purpose:** Random number generator from discrete PMF
**Inherits:** `Random` (overrides `NextDouble()`)

**Construction:**
- `Create(Random uniformGen, IReadOnlyList<double> inverseCdfArray)`

**Process:**
1. Generate uniform random r ∈ [0, 1]
2. Map r through inverse CDF array using linear interpolation
3. Return distributed value

---

## Precision & Numerical Issues

### 1. Hard-coded Tolerances
Multiple hard-coded epsilon values throughout:
- `1e-12` - Domain size validation (lines appear in multiple files)
- `1e-7` - Probability sum validation (DiscreteProbabilityMassFunction)
- `1e-17` - Near-one check (CumulativeDistributionFunction)
- `Float64Utils.ZeroEpsilon` - Default for trimming operations

**Issue:** These should be configurable or use a consistent epsilon policy.

### 2. Inverse CDF Computation
Both `QuantizedHistogram` and `SparseRegularHistogram` compute inverse CDF by:
1. Building piecewise affine function
2. Linear interpolation between breakpoints

**Potential Issue:** No adaptive sampling based on function curvature.

### 3. Histogram Arithmetic
Operations like `histogram1 + histogram2` use sparse dictionary multiplication:
```csharp
foreach (var (x1, p1) in hist1.StoredBinValueHeightPairs)
    foreach (var (x2, p2) in hist2.StoredBinValueHeightPairs)
    {
        var x = x1 + x2;
        var p = p1 * p2;  // Convolution
        // ...accumulate in sparse dictionary
    }
```

**Issue:** O(n²) complexity, no FFT-based convolution for large histograms.

### 4. Quantization Levels
`QuantizedHistogram` uses `ulong` (64-bit) for counts:
- `quantizationBits` parameter (default 32) controls resolution
- Max levels: `1ul << quantizationBits`

**Issue:** For `quantizationBits > 64`, code will overflow silently.

### 5. Distribution Sampling Accuracy
`CreateNormal`, `CreateExponential` use adaptive range computation:
```csharp
var halfSize = Math.Sqrt(-2 * variance * Math.Log(sqrt2Pi * stdDev * zeroEpsilon));
```

**Good:** Adapts range to avoid truncation error.

---

## API Differences Matrix

| Feature | Float64 Version | Generic Version |
|---------|----------------|-----------------|
| **Root Utilities** | ✅ CumulativeDistributionFunction | ❌ Missing |
| | ✅ RandomEuclideanVectorsComposer | ❌ Missing |
| | ✅ RandomUtils | ❌ Missing |
| **Continuous Histograms** | ✅ HistogramBinData | ❌ Missing |
| | ✅ PiecewiseAffineFunction | ❌ Missing |
| | ✅ ProbabilityDistributionFunction | ❌ Missing |
| | ✅ QuantizedHistogram (1549 lines) | ❌ Missing |
| | ✅ SparseRegularHistogram (1510 lines) | ❌ Missing |
| | ✅ SparseIrregularHistogram | ❌ Missing |
| **Discrete PMFs** | ✅ DiscreteProbabilityFunction | ❌ Missing |
| | ✅ DiscreteProbabilityMassFunction | ❌ Missing |
| | ✅ PmfRandomGenerator | ❌ Missing |
| **Operators** | ✅ +, -, *, / overloaded | ❌ Missing |
| **Distributions** | ✅ Normal, Exponential, Binomial, Poisson | ❌ Missing |
| **Statistics** | ✅ Mean, Variance, Moments, Kurtosis | ❌ Missing |
| **Visualization** | ✅ MATLAB code generation | ❌ Missing |

---

## Missing Features

### Generic Implementation
**Status:** Completely absent

**What Would Be Needed:**
1. `HistogramBinData<T>` - Generic bin data
2. `PiecewiseAffineFunction<T>` - Generic piecewise functions
3. `QuantizedHistogram<T>` - Generic histogram with `IScalarProcessor<T>`
4. `DiscreteProbabilityMassFunction<T>` - Generic PMF

**Challenges:**
- **Random number generation:** How to generate `T` values from uniform [0, 1]?
  - Requires `IScalarProcessor<T>.NextRandom()` or similar
- **Comparison operations:** Histograms need `x < y` tests
  - Requires `IComparable<T>` constraint
- **Mathematical functions:** `Math.Exp()`, `Math.Log()`, `Math.Sqrt()`
  - Requires `IScalarProcessor<T>` to provide these
  - Current processors (ERational, EDecimal) don't have transcendental functions
- **Quantization:** `ulong` counts are inherently integer-based
  - Generic version might need different approach

**Recommendation:** Statistics is inherently numerical and benefits from floating-point arithmetic. Generic version has limited practical value given current scalar processor capabilities.

---

## Critical Bugs Found

### Bug #1: CumulativeDistributionFunction.GetProbability (Line 67)
**Severity:** CRITICAL
**File:** `CumulativeDistributionFunction.cs`

```csharp
// WRONG:
if (value >= DomainMinValue) return 1d;

// CORRECT:
if (value >= DomainMaxValue) return 1d;
```

**Impact:** CDF always returns 1.0 for any value >= minimum (instead of maximum).

---

### Bug #2: CumulativeDistributionFunction.ProbabilityToValue (Line 129)
**Severity:** CRITICAL
**File:** `CumulativeDistributionFunction.cs`

```csharp
// WRONG:
var t = (p2 - p1) / (p2 - p1);  // Always equals 1!

// CORRECT:
var t = (probability - p1) / (p2 - p1);
```

**Impact:** Inverse CDF always returns exact endpoint, ignoring interpolation.

---

### Bug #3: QuantizedHistogram - Domain Range (Lines 1078, 1104)
**Severity:** HIGH
**File:** `QuantizedHistogram.cs`

```csharp
// WRONG:
var domainFirstValue = sparseDictionary.Keys.Min();
var domainLastValue = sparseDictionary.Keys.Min();  // BUG!

// CORRECT:
var domainFirstValue = sparseDictionary.Keys.Min();
var domainLastValue = sparseDictionary.Keys.Max();
```

**Impact:** Domain range collapses to single value, breaks histogram construction.

**Locations:** Two occurrences in `MapDomain` and `JoinDomain` methods.

---

### Bug #4: DiscreteProbabilityMassFunction.MapDomain (Line 492)
**Severity:** MEDIUM
**File:** `DiscreteProbabilityMassFunction.cs`

```csharp
// WRONG:
var p = p1 + p2;  // Addition instead of multiplication!

// CORRECT:
var p = p1 * p2;  // Convolution requires multiplication
```

**Impact:** Probability convolution is mathematically incorrect.

---

## Recommendations

### 1. Fix Critical Bugs Immediately
All 4 bugs should be fixed before any production use:
- CumulativeDistributionFunction: Lines 67, 129
- QuantizedHistogram: Lines 1078, 1104
- DiscreteProbabilityMassFunction: Line 492

### 2. Do NOT Create Generic Versions
**Rationale:**
- Statistics is inherently numerical (requires transcendental functions)
- Current generic scalar processors (ERational, EDecimal) lack `exp()`, `log()`, `sqrt()`
- Random number generation from generic types is non-trivial
- Float64 performance is critical for large histograms (millions of samples)
- Symbolic statistics makes little practical sense

**Recommendation:** Keep Statistics module Float64-only.

### 3. Numerical Improvements
- **Configurable tolerances:** Add `StatisticsConfig` class with epsilon settings
- **FFT convolution:** For large histograms, use FFT-based convolution
- **Adaptive sampling:** Improve inverse CDF computation with curvature-adaptive sampling
- **Overflow checking:** Add guards for `quantizationBits > 64`

### 4. Documentation
- Add XML doc comments explaining numerical methods
- Document precision limitations
- Add examples of creating and using distributions
- Explain quantized vs. normalized histograms

### 5. Testing
- **Unit tests:** Verify probability sums to 1.0
- **Numerical tests:** Check CDF/inverse CDF round-trip accuracy
- **Distribution tests:** Validate moments against known values
- **Edge cases:** Zero bins, single bin, very large histograms

### 6. Code Quality
- **Extract constants:** Hard-coded tolerances → named constants
- **Reduce duplication:** QuantizedHistogram and SparseRegularHistogram share 80% logic
  - Consider common base class or refactoring
- **Split large files:** 1500+ line files should be split into partial classes
  - `QuantizedHistogram.Core.cs`, `QuantizedHistogram.Operators.cs`, etc.

---

## Conclusion

The Statistics module is:
- ✅ **Well-architected** with clear separation of continuous vs. discrete
- ✅ **Feature-rich** with comprehensive distribution support
- ❌ **Buggy** with 4 critical/high severity bugs
- ✅ **Float64-only** by design (appropriate for this domain)
- ⚠️ **Needs testing** - No unit tests found
- ⚠️ **Needs documentation** - Minimal XML comments

**Priority Actions:**
1. **Fix the 4 bugs immediately**
2. **Add comprehensive unit tests**
3. **Document numerical methods and limitations**
4. **Do not attempt generic version** (not practical)

**Status:** Module is sophisticated but requires bug fixes and testing before production use. The Float64-only design is appropriate and should NOT be changed to generic.
