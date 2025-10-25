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

1. ✅ Create `XGaProductOperationsBenchmark.cs` - Most common operations
2. ✅ Expand `CgaGeometricOperationsBenchmark.cs` - Important for 3D graphics

### Short-Term (Within 1-2 weeks)

3. ⏳ Create `XGaLinearMapsBenchmark.cs` - Rotors/reflectors
4. ⏳ Create `XGaNormalizationBenchmark.cs` - Frequent operation

### Long-Term (Future work)

5. 📋 High-dimensional benchmarks
6. 📋 Storage comparison benchmarks
7. 📋 CI/CD integration

---

**Generated:** 2025-10-25
**Author:** Claude Code
**Context:** Performance Benchmark Roadmap
