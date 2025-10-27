# Performance Benchmark Recommendations
**Date:** 2025-10-25
**Status:** Roadmap for Future Benchmarks

## Current Benchmark Coverage

### Existing Benchmarks ✅

1. **CgaFloat32PerformanceBenchmarks.cs** - Conformal GA with Float32
   - Point encoding (IPNS)
   - Basic CGA operations
   - Comparison: Generic<float> vs Generic<double> vs Float64 Specialized

2. **Generic vs Specialized Analysis** (GENERIC_VS_SPECIALIZED_PERFORMANCE.md)
   - Comprehensive comparison across scalar types
   - Memory allocation analysis
   - Performance ratios documented

### Current Findings Summary

| Scalar Type | Performance vs Float64 Spec | Memory vs Float64 Spec |
|-------------|----------------------------|------------------------|
| Generic<float> | **1.24x faster** (24% speedup) | **50% less memory** |
| Generic<double> | **1.27x faster** (27% speedup) | **33% less memory** |
| Float64 Specialized | Baseline (1.0x) | Baseline |

---

## Recommended New Benchmarks

### Priority 1: Core Operations (High Impact)

#### 1. Multivector Product Operations Benchmark
**File:** `XGaProductOperationsBenchmark.cs`

**Rationale:** Products (Gp, Op, Sp, Lcp, Rcp) are the most common operations in GA.

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class XGaProductOperationsBenchmark
{
    private XGaFloat64Processor _procFloat64 = null!;
    private XGaProcessor<float> _procFloat32 = null!;
    private XGaProcessor<double> _procDouble = null!;

    [Params(3, 5, 8)] // Vector space dimensions
    public int Dimensions;

    [GlobalSetup]
    public void Setup()
    {
        _procFloat64 = XGaFloat64Processor.Euclidean;
        _procFloat32 = XGaProcessor<float>.CreateEuclidean(ScalarProcessorOfFloat32.Instance);
        _procDouble = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    }

    [Benchmark(Baseline = true)]
    public void GeometricProduct_Float64Specialized()
    {
        var v1 = CreateRandomVector_Float64();
        var v2 = CreateRandomVector_Float64();
        var result = v1.Gp(v2);
    }

    [Benchmark]
    public void GeometricProduct_GenericFloat()
    {
        var v1 = CreateRandomVector_Float32();
        var v2 = CreateRandomVector_Float32();
        var result = v1.Gp(v2);
    }

    [Benchmark]
    public void GeometricProduct_GenericDouble()
    {
        var v1 = CreateRandomVector_Double();
        var v2 = CreateRandomVector_Double();
        var result = v1.Gp(v2);
    }

    // Similar for Op, Sp, Lcp, Rcp
}
```

**Expected Insights:**
- How do product operations scale with dimensions?
- Are there performance differences between product types?
- Does sparse vs dense multivector storage affect performance?

---

#### 2. Linear Maps Benchmark (Rotors, Reflectors)
**File:** `XGaLinearMapsBenchmark.cs`

**Rationale:** Rotors are critical for 3D graphics and animations.

```csharp
[Benchmark]
public void PureRotor_Creation_And_Application()
{
    var u1 = CreateRandomUnitVector();
    var u2 = CreateRandomUnitVector();
    var rotor = u1.CreatePureRotor(u2);
    var v = CreateRandomVector();
    var rotated = rotor.OmMap(v);  // Apply rotation
}
```

**Test Scenarios:**
- Rotor creation (2D, 3D, higher dimensions)
- Rotor application (OmMap)
- Rotor composition
- Reflector operations

**Expected Insights:**
- Float32 advantage for real-time rotation-heavy applications
- Memory impact of storing rotation sequences

---

#### 3. Norm and Normalization Benchmark
**File:** `XGaNormalizationBenchmark.cs`

**Rationale:** Normalization is frequent in graphics (unit vectors, unit quaternions).

```csharp
[Benchmark]
public void VectorNormalization_Float32()
{
    for (int i = 0; i < 1000; i++)
    {
        var v = CreateRandomVector_Float32();
        var normalized = v.DivideByENorm();
    }
}
```

**Test Scenarios:**
- Vector normalization (2D, 3D, 4D)
- Multivector normalization
- Batch normalization (1000 vectors)

---

### Priority 2: Domain-Specific Benchmarks (Medium Impact)

#### 4. CGa Geometric Operations Benchmark
**File:** `CgaGeometricOperationsBenchmark.cs`

**Expand existing CgaFloat32PerformanceBenchmarks.cs with:**

```csharp
[Benchmark]
public void CGa_SphereIntersection()
{
    var sphere1 = _cga.EncodeIpnsRound.Sphere(0, 0, 0, 5.0f);
    var sphere2 = _cga.EncodeIpnsRound.Sphere(3, 0, 0, 4.0f);
    var intersection = sphere1.Op(sphere2);  // Meet operation
}

[Benchmark]
public void CGa_ReflectionInPlane()
{
    var point = _cga.EncodeIpnsRound.Point(1, 2, 3);
    var plane = _cga.EncodeOpnsFlat.Plane(0, 0, 1, 5);  // z=5 plane
    var reflected = plane.ReflectIpnsRoundInOpnsFlat(point);
}
```

**Test Scenarios:**
- Point-sphere distance
- Sphere-sphere intersection
- Plane reflections
- Circle construction

---

#### 5. PGa Transformations Benchmark
**File:** `PgaTransformationsBenchmark.cs`

**Rationale:** PGA is excellent for computer graphics transformations.

```csharp
[Benchmark]
public void PGA_ComposedTransformation()
{
    var translator = CreateTranslation(1, 2, 3);
    var rotator = CreateRotation(axis, angle);
    var composed = translator.Gp(rotator);
    var point = CreatePoint(0, 0, 0);
    var transformed = composed.OmMap(point);
}
```

---

### Priority 3: Edge Cases & Scalability (Lower Priority)

#### 6. High-Dimensional GA Benchmark
**File:** `XGaHighDimensionalBenchmark.cs`

**Rationale:** Test scaling behavior.

```csharp
[Params(8, 16, 32, 64)]
public int Dimensions;

[Benchmark]
public void HighDimensional_OuterProduct()
{
    var v1 = CreateSparseVector(Dimensions, sparsity: 0.1);
    var v2 = CreateSparseVector(Dimensions, sparsity: 0.1);
    var result = v1.Op(v2);
}
```

**Expected Insights:**
- When does sparse storage become critical?
- How does performance degrade with dimensions?

---

#### 7. Sparse vs Dense Storage Benchmark
**File:** `XGaStorageComparisonBenchmark.cs`

**Rationale:** Understand storage strategy performance trade-offs.

```csharp
[Benchmark]
public void DenseMultivector_Operations()
{
    // Use RGaFloat64Multivector (dense storage)
}

[Benchmark]
public void SparseMultivector_Operations()
{
    // Use XGaUniformMultivector (sparse storage)
}
```

---

## Benchmark Infrastructure Improvements

### 1. Automated Benchmark Runner

Create `RunAllBenchmarks.ps1`:

```powershell
# Run all benchmarks and generate reports
dotnet run --project GeometricAlgebraFulcrumLib.Benchmarks `
    --configuration Release `
    --filter "*" `
    --exporters json markdown html

# Aggregate results
python aggregate_benchmark_results.py
```

### 2. Benchmark Comparison Tool

Create tool to compare benchmark runs:

```bash
compare-benchmarks before.json after.json --output=comparison.md
```

### 3. CI/CD Integration

Add performance regression detection:

```yaml
# .github/workflows/benchmark.yml
name: Performance Benchmarks

on:
  pull_request:
    paths:
      - 'GeometricAlgebraFulcrumLib.Algebra/**'

jobs:
  benchmark:
    runs-on: ubuntu-latest
    steps:
      - name: Run Benchmarks
        run: dotnet run --project Benchmarks --configuration Release

      - name: Compare with Baseline
        run: compare-benchmarks baseline.json current.json

      - name: Comment PR if Regression
        if: regression_detected
        uses: actions/github-script@v6
        with:
          script: |
            github.rest.issues.createComment({
              body: '⚠️ Performance regression detected!'
            })
```

---

## Benchmark Best Practices

### 1. Always Use Release Mode

```bash
dotnet run --project Benchmarks --configuration Release
```

### 2. Disable CPU Frequency Scaling

```bash
# Linux
sudo cpupower frequency-set --governor performance

# Windows
powercfg /setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c
```

### 3. Close Background Applications

Minimize interference from other processes.

### 4. Run Multiple Iterations

```csharp
[SimpleJob(warmupCount: 5, targetCount: 20, invocationCount: 1000)]
```

### 5. Use Memory Diagnoser

```csharp
[MemoryDiagnoser]
public class MyBenchmark { }
```

---

## Expected Benchmark Results (Predictions)

Based on current findings, we expect:

| Operation | Generic<float> Advantage |
|-----------|--------------------------|
| Geometric Product (3D) | 20-25% faster |
| Outer Product (3D) | 25-30% faster |
| Normalization | 15-20% faster |
| Rotor Application | 25-30% faster |
| CGA Intersections | 20-25% faster |
| High-Dimensional (64D) | 30-40% faster (more cache-friendly) |

---

## Summary & Priorities

### Immediate Actions (Do First)

1. ⏸️ **BLOCKED**: XGa benchmarks require deeper API research
   - **Issue**: XGa Generic<T> API differs significantly from Float64 Specialized
   - **Examples**: `CreatePureRotor`, `VectorENorm`, `GetComposerCombine` have different signatures
   - **Next Steps**: Study existing BilinearProductsBenchmarks.cs, research generic XGa API patterns

2. ✅ **COMPLETED**: CGa benchmarks work well (see `CgaFloat32PerformanceBenchmarks.cs`)
   - Successfully demonstrates Generic<float> vs Float64 Specialized comparison
   - Validated 24% performance improvement for Generic<float>

### Recommended Alternative Approach

Since low-level XGa benchmarks are blocked on API research, focus on **higher-level benchmarks** using CGa/PGa:

3. ⏳ Expand `CgaGeometricOperationsBenchmark.cs` - Add more geometric operations
4. ⏳ Create `PgaTransformationsBenchmark.cs` - Projective transformations

### Long-Term (Future work)

5. 📋 High-dimensional benchmarks
6. 📋 Storage comparison benchmarks
7. 📋 CI/CD integration

---

## ✅ API Research Findings (2025-10-26) - RESOLVED

### XGa Generic<T> API Complexity

Initial attempt to create XGa benchmarks comparing Generic<float> vs Float64 Specialized revealed significant API differences:

#### Issues Discovered

1. **Missing Extension Methods**
   - `XGaProcessor<T>.CreateVector(params)` doesn't exist
   - Must use `CreateVectorComposer().SetVectorTerm(...).GetVector()` pattern

2. **Rotor API Differences**
   - `CreatePureRotor()` expects `LinBasisVector` not `XGaVector<T>` for generic types
   - `GetComposerCombine()` method signature differs between Float64 and Generic<T>

3. **Normalization API Differences**
   - `VectorENorm()` and `VectorENormSquared()` extension methods not available for Generic<T>
   - Alternative approaches needed

#### Resolution (2025-10-26)

✅ **Successfully implemented XGaNormalizationBenchmark** using correct Generic<T> API patterns:
- Used `CreateVectorComposer()` pattern for vector creation
- Used `ENorm()` and `ENormSquared()` methods (not extension methods)
- 21 benchmarks implemented and executed successfully

---

## 🚨 CRITICAL FINDING: XGa Performance Contradiction (2025-10-26)

**Status:** ⚠️ **Phase 2 Strategy Requires Re-evaluation**

### Performance Contradiction Discovered

XGa (low-level) benchmarks **contradict** CGa (high-level) performance findings:

| Level | Float64 Spec | Generic<float> | Generic<double> | Conclusion |
|-------|--------------|----------------|-----------------|------------|
| **CGa (High-Level)** | Baseline | **1.24x faster** ✅ | **1.27x faster** ✅ | Generic wins! |
| **XGa (Low-Level)** | Baseline | **1.85x slower** ⚠️ | **1.88x slower** ⚠️ | Float64 wins! |

### Detailed XGa Performance Results

**Worst Cases (Low-Level Operations):**
- Vector Norm 3D: Generic **1.85x slower** (75.0ns vs 40.6ns)
- Vector Norm 4D: Generic **2.11x slower** (85.9ns vs 40.7ns)
- Multivector Norm: Generic **2.62x slower** (236.0ns vs 90.3ns) ⚠️ **WORST!**

**Best Cases (Higher-Level/Batch Operations):**
- Batch Normalize 1000x: Generic **1.15x slower** (381.1µs vs 331.6µs)
- Normalize + Dot Product: Generic **1.14x slower** (881.9ns vs 772.3ns)

### Key Insight: Generic<float> vs Generic<double>

⚠️ **NO performance advantage for Generic<float> at XGa level** - both perform identically (within 1-2%)!

This contradicts the Float32 advantage seen at CGa level.

### Hypothesis: Why Does Performance Differ by Abstraction Level?

**XGa (Low-Level):**
- Direct scalar operations (`ENorm().ScalarValue`)
- Minimal abstraction overhead
- Float64 likely uses SIMD/AVX2 intrinsics
- Generic<T> suffers from `IScalarProcessor<T>` indirection

**CGa (High-Level):**
- Complex geometric algebra operations
- Multiple XGa calls combined
- JIT has more room to optimize across boundaries
- Generic<T> benefits from devirtualization at higher level

### Impact on Phase 2 (Thin Wrapper Migration)

**Original Assumption (based on CGa):**
> "Generic is 1.27x faster → Thin wrapper will improve performance"

**New Reality (based on XGa):**
> "Generic is 1.15-2.62x SLOWER at XGa level → Thin wrapper will DEGRADE performance"

### Recommendations

1. ❌ **DO NOT migrate XGa Float64 to Thin Wrapper** (yet)
   - Keep Float64 Specialized for XGa Core operations
   - Performance regression would be unacceptable (1.15-2.62x slower)

2. ✅ **CGa/PGa Thin Wrapper is still valid**
   - High-level operations show Generic advantage
   - CGa benchmarks: Generic 1.24-1.27x faster

3. 🔬 **Further Investigation Required BEFORE Phase 2:**
   - Profile XGa Float64 vs Generic<double> to find exact bottlenecks
   - Check if Float64 uses SIMD/AVX2 intrinsics (grep for Vector256, AVX2)
   - Measure `IScalarProcessor<T>` call overhead
   - Analyze memory access patterns (cache misses?)
   - Test with aggressive inlining attributes

4. 📋 **Potential Optimizations (Future Work):**
   - Add SIMD paths for Generic<float> and Generic<double>
   - Implement specialized XGaScalarProcessor<T> with fast-paths
   - Use aggressive inlining on Generic methods
   - Consider struct-based scalar processors for devirtualization

### Action Items

**Immediate (Before Phase 2):**
- [ ] Run profiler on XGa Generic vs Float64 - Find exact bottleneck
- [ ] Search Float64 XGa source for SIMD usage
- [ ] Benchmark `IScalarProcessor<T>` call overhead
- [ ] Test aggressive inlining hints (`[MethodImpl(MethodImplOptions.AggressiveInlining)]`)

**Short-Term:**
- [ ] Re-evaluate Phase 2 strategy - XGa may need to stay Float64 Specialized
- [ ] Document performance trade-offs in roadmap
- [ ] Consider hybrid approach:
  - XGa Core: Keep Float64 Specialized (performance-critical)
  - CGa/PGa: Migrate to Generic (already faster)
  - ComplexAlgebra/VGA: Evaluate case-by-case

**Long-Term:**
- [ ] Implement SIMD-optimized Generic paths
- [ ] Benchmark on different hardware (AMD vs Intel, ARM64)
- [ ] Profile real-world workloads

### References

**Benchmark Report:** `XGA_NORMALIZATION_BENCHMARK_RESULTS.md` (comprehensive 300+ line analysis)
**Roadmap Updates:** `NEXT_STEPS_ROADMAP.md`, `DEDUPLICATION_TASKS.md`

---

## Recommendations (Updated 2025-10-26)

**Previous Recommendations:**
- ~~Use CGa/PGa for benchmarks~~ ✅ DONE
- ~~Study existing benchmarks~~ ✅ DONE
- ~~API documentation needed~~ ⏳ IN PROGRESS

---

**Generated:** 2025-10-25 (Updated: 2025-10-26)
**Author:** Claude Code
**Context:** Performance Benchmark Roadmap
