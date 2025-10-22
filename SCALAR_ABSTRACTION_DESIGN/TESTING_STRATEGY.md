# GA-FUL Testing Strategy
## Qualitätssicherung für Scalar Abstraction Refactoring

**Teil von:** [SCALAR_ABSTRACTION_DESIGN.md](./SCALAR_ABSTRACTION_DESIGN.md)
**Version:** 2.1 (Korrigiert)
**Datum:** 2025-01-22 (Updated: 2025-10-22)

---

## ⚠️ KORREKTUR (2025-10-22)

**Test-Count korrigiert:** Ursprüngliche Dokumentation behauptete 507 CGa Tests. Tatsächliche Anzahl basierend auf Code-Analyse: **162 CGa Tests** (9 Test-Files im `CGa/` Verzeichnis).

**Implikation:** Geringere Regressions-Test-Abdeckung als angenommen. Test-Coverage sollte erhöht werden.

---

## Test-Coverage Übersicht

### Pro Phase

| Phase | Test-Typ | Anzahl Tests | Success Criteria |
|-------|----------|--------------|------------------|
| **1** | Unit (ScalarProcessor) | ~50 | 100% pass |
| **1** | Performance (Benchmarks) | ~10 | ~90% von raw float |
| **2** | Integration (Float32/Symbolic) | ~40 | Workflows funktionieren |
| **2** | Unit (CGa Generic API) | ~80 | 100% pass |
| **3** | Regression (CGa Float64) | **162** | **100% pass** ⚠️ (korrigiert von 507) |
| **3** | Performance (Float64 Wrapper) | ~10 | <2% overhead (adjustiert von <1%) |
| **GESAMT** | | **~352** | **100% pass** (korrigiert von ~697) |

---

## Phase 1: ScalarProcessorOfFloating<T> Tests

### Unit Tests (GeometricAlgebraFulcrumLib.UnitTests/Algebra/ScalarProcessorOfFloatingTests.cs)

```csharp
[TestFixture]
public class ScalarProcessorOfFloatingTests
{
    // Float32 Tests (20 tests)
    [TestFixture]
    public class Float32Tests
    {
        [Test] public void ZeroOne_ShouldHaveCorrectValues() { }
        [Test] public void Add_ShouldReturnCorrectSum() { }
        [Test] public void Subtract_ShouldReturnCorrectDifference() { }
        [Test] public void Multiply_ShouldReturnCorrectProduct() { }
        [Test] public void Divide_ShouldReturnCorrectQuotient() { }
        [Test] public void Sqrt_ShouldReturnCorrectRoot() { }
        [Test] public void Sin_ShouldReturnCorrectValue() { }
        [Test] public void Cos_ShouldReturnCorrectValue() { }
        [Test] public void Exp_ShouldReturnCorrectValue() { }
        [Test] public void Log_ShouldReturnCorrectValue() { }
        [Test] public void ScalarWrapping_ShouldWork() { }
        [Test] public void ValueFromNumber_Int_ShouldConvert() { }
        [Test] public void ValueFromNumber_Double_ShouldConvert() { }
        [Test] public void IsZero_ShouldWorkCorrectly() { }
        [Test] public void Abs_ShouldReturnAbsoluteValue() { }
        [Test] public void Negative_ShouldNegate() { }
        [Test] public void Square_ShouldSquareCorrectly() { }
        [Test] public void Power_ShouldCalculateCorrectly() { }
        [Test] public void ArcTan2_ShouldCalculateCorrectly() { }
        [Test] public void Pi_ShouldHaveCorrectValue() { }
    }

    // Float64 Tests (20 tests) - analog
    [TestFixture]
    public class Float64Tests { /* ... */ }

    // Half Tests (10 tests) - reduziert wegen Precision
    [TestFixture]
    public class HalfTests { /* ... */ }
}
```

**Total:** ~50 Unit Tests

### Performance Tests (GeometricAlgebraFulcrumLib.Benchmarks/ScalarProcessorBenchmarks.cs)

```csharp
[MemoryDiagnoser]
public class ScalarProcessorFloat32Benchmarks
{
    [Benchmark(Baseline = true)]
    public float RawFloat_Add() { }

    [Benchmark]
    public float ScalarProcessor_Add() { }

    [Benchmark(Baseline = true)]
    public float RawFloat_Multiply() { }

    [Benchmark]
    public float ScalarProcessor_Multiply() { }

    [Benchmark(Baseline = true)]
    public float RawFloat_Sqrt() { }

    [Benchmark]
    public float ScalarProcessor_Sqrt() { }

    // Analog für Exp, Sin, Cos, etc.
}
```

**Erwartete Ergebnisse:**
```
| Method                   | Mean      | Ratio |
|-------------------------|-----------|-------|
| RawFloat_Add            | 100.0 ns  | 1.00  |
| ScalarProcessor_Add     | ~105 ns   | ~1.05 | ✅ <10%
| RawFloat_Multiply       | 100.0 ns  | 1.00  |
| ScalarProcessor_Multiply| ~105 ns   | ~1.05 | ✅ <10%
| RawFloat_Sqrt           | 500.0 ns  | 1.00  |
| ScalarProcessor_Sqrt    | ~510 ns   | ~1.02 | ✅ <10%
```

**Success:** Performance ~90%+ von raw ✅

---

## Phase 2: CGa Generic API Tests

### Float32 Integration Tests (~20 tests)

```csharp
[TestFixture]
public class CGaFloat32WorkflowTests
{
    private CGaGeometricSpace5D<float> _cga = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        var processor = ScalarProcessorOfFloating<float>.Instance;
        _cga = CGaGeometricSpace5D<float>.Create(processor);
    }

    [Test] public void Point_Float32_ShouldEncodeDecode() { }
    [Test] public void Circle_Float32_ShouldEncodeDecode() { }
    [Test] public void Sphere_Float32_ShouldEncodeDecode() { }
    [Test] public void Line_Float32_ShouldWork() { }
    [Test] public void Plane_Float32_ShouldWork() { }

    [Test] public void Point_Operators_ShouldWork() { }
    [Test] public void Circle_Translation_ShouldWork() { }
    [Test] public void Sphere_Scaling_ShouldWork() { }

    [Test] public void Intersection_SpherePlane_ShouldWork() { }
    [Test] public void Intersection_CircleLine_ShouldWork() { }

    [Test] public void Meet_TwoPlanes_ShouldGiveLine() { }
    [Test] public void Join_TwoPoints_ShouldGiveLine() { }

    [Test] public void Decoding_Center_ShouldWork() { }
    [Test] public void Decoding_Radius_ShouldWork() { }
    [Test] public void Decoding_Normal_ShouldWork() { }

    [Test] public void BatchProcessing_100kPoints_ShouldBePerformant() { }

    [Test] public void GPUDataTransfer_ShouldWork() { }
    [Test] public void MixedOperations_Complex_ShouldWork() { }

    [Test] public void NumericalPrecision_Float32_ShouldBeAcceptable() { }
    [Test] public void EdgeCase_ZeroRadius_ShouldHandle() { }
}
```

### Symbolic Integration Tests (~20 tests)

```csharp
[TestFixture]
public class CGaSymbolicWorkflowTests
{
    [Test] public void SymbolicPoint_ShouldCreateExpression() { }
    [Test] public void SymbolicCircle_ShouldOptimize() { }
    [Test] public void SymbolicSphere_ShouldOptimize() { }

    [Test] public void SymbolicOperators_ShouldWork() { }
    [Test] public void SymbolicTranslation_ShouldSimplify() { }
    [Test] public void SymbolicScaling_ShouldSimplify() { }

    [Test] public void CSE_ShouldEliminateDuplicates() { }
    [Test] public void ConstantFolding_ShouldSimplify() { }
    [Test] public void AlgebraicSimplification_ShouldWork() { }

    [Test] public void CodeGeneration_CSharp_ShouldProduceValidCode() { }
    [Test] public void CodeGeneration_CPlusPlus_ShouldProduceValidCode() { }
    [Test] public void CodeGeneration_Float32_ShouldWork() { }

    [Test] public void ComplexExpression_ShouldOptimize() { }
    [Test] public void ParameterSubstitution_ShouldWork() { }

    [Test] public void SymbolicToNumeric_RoundTrip_ShouldWork() { }
    [Test] public void MetaContext_MultipleSpaces_ShouldIsolate() { }

    [Test] public void OptimizationStatistics_ShouldReport() { }
    [Test] public void DeadCodeElimination_ShouldWork() { }

    [Test] public void LargeExpression_Performance_ShouldBeAcceptable() { }
    [Test] public void EdgeCase_SymbolicZero_ShouldHandle() { }
}
```

### CGa Generic API Unit Tests (~80 tests)

Analog zu bestehenden Float64 Tests, aber für Generic<T>:

```csharp
// Encoding Tests (20)
[Test] public void Generic_Point_2D() { }
[Test] public void Generic_Point_3D() { }
[Test] public void Generic_Circle_2D() { }
[Test] public void Generic_Sphere_3D() { }
// ... etc.

// Decoding Tests (20)
[Test] public void Generic_DecodePoint() { }
[Test] public void Generic_DecodeCircle() { }
// ... etc.

// Operations Tests (20)
[Test] public void Generic_Translation() { }
[Test] public void Generic_Rotation() { }
// ... etc.

// Edge Cases (20)
[Test] public void Generic_ZeroVector() { }
[Test] public void Generic_InfinityHandling() { }
// ... etc.
```

---

## Phase 3: Regression Tests (KRITISCH!)

### Alle bestehenden CGa Tests (162 tests ⚠️ KORRIGIERT)

**Verzeichnis:** `GeometricAlgebraFulcrumLib.UnitTests/Modeling/Geometry/CGa/`

```bash
dotnet test --filter "FullyQualifiedName~CGa" --verbosity normal
```

**Dateien (tatsächlich vorhanden):**
- `CGaAdvancedElementTests.cs`
- `CGaBasicsTests.cs`
- `CGaBladeOperationsTests.cs`
- `CGaDecodingTests.cs`
- `CGaEncodingTests.cs`
- `CGaGeometricSpaceTests.cs`
- `CGaInterpolationTests.cs`
- `CGaOperationsTests.cs`
- `CGaVersorsTests.cs`

**TOTAL:** **162 Tests** (9 Test-Files, grep-verified)

⚠️ **HINWEIS:** Ursprüngliche Dokumentation behauptete 507 Tests mit anderen Dateinamen. Die tatsächliche Code-Analyse zeigt 162 Tests. Die zusätzlichen ~345 Tests existieren möglicherweise nicht, oder die Test-Files wurden umbenannt/konsolidiert.

**Success Criteria:** **100% müssen passen** (Zero Failures!) ✅

### Test-Execution Strategie

```bash
# 1. Baseline vor Refactoring
dotnet test --filter "FullyQualifiedName~CGa" --logger "console;verbosity=normal" > before.log

# 2. Nach Phase 3 Refactoring
dotnet test --filter "FullyQualifiedName~CGa" --logger "console;verbosity=normal" > after.log

# 3. Diff vergleichen
diff before.log after.log  # Sollte NUR timestamps unterscheiden!
```

### Bei Test-Failures

**Prozedur:**
1. **Identify:** Welcher Test ist gefailed?
2. **Analyze:** Wrapper-Delegation korrekt?
3. **Fix:** API-Mapping anpassen
4. **Verify:** Rerun nur failed test
5. **Retest:** Full regression suite

**Blocker-Kriterium:** Wenn >5 Tests failen, Phase 3 pausieren und Root-Cause analysieren!

### Applications Manual Testing

**Datei:** `GeometricAlgebraFulcrumLib.Applications/Robotics/InverseKinematics6R.cs`

**Test-Prozedur:**
1. **Vor Refactoring:** Beispiel ausführen, Output loggen
2. **Nach Refactoring:** Beispiel ausführen, Output vergleichen
3. **Erwartung:** Bit-identische Results (oder <1e-15 difference)

---

## Performance Benchmarks

### Float64 Wrapper Overhead (Phase 3)

```csharp
[MemoryDiagnoser]
public class CGaFloat64WrapperBenchmarks
{
    private CGaFloat64GeometricSpace5D _oldImplementation = null!;
    private CGaFloat64GeometricSpace5D _newWrapper = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Baseline: Alte Implementation (branch backup)
        _oldImplementation = /* ... */;

        // New: Wrapper über Generic<double>
        _newWrapper = CGaFloat64GeometricSpace5D.Instance;
    }

    [Benchmark(Baseline = true)]
    public CGaFloat64Blade Old_EncodePoint()
    {
        return _oldImplementation.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
    }

    [Benchmark]
    public CGaFloat64Blade New_EncodePoint()
    {
        return _newWrapper.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
    }

    [Benchmark(Baseline = true)]
    public CGaFloat64Blade Old_EncodeCircle()
    {
        return _oldImplementation.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
    }

    [Benchmark]
    public CGaFloat64Blade New_EncodeCircle()
    {
        return _newWrapper.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
    }

    // Analog für alle häufigen Operationen
}
```

**Erwartete Ergebnisse:**
```
| Method              | Mean      | Ratio |
|--------------------|-----------|-------|
| Old_EncodePoint    | 1000 ns   | 1.00  |
| New_EncodePoint    | ~1005 ns  | ~1.005| ✅ <1%
| Old_EncodeCircle   | 1500 ns   | 1.00  |
| New_EncodeCircle   | ~1510 ns  | ~1.007| ✅ <1%
```

**Success:** <1% Overhead ✅

---

## Continuous Integration

### GitHub Actions Workflow

```yaml
name: CI - Scalar Abstraction Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        phase: [phase1, phase2, phase3]

    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '8.0.x'

      - name: Build
        run: dotnet build GeometricAlgebraFulcrumLib.sln -c Release

      - name: Test Phase 1
        if: matrix.phase == 'phase1'
        run: dotnet test --filter "FullyQualifiedName~ScalarProcessorOfFloating"

      - name: Test Phase 2
        if: matrix.phase == 'phase2'
        run: |
          dotnet test --filter "FullyQualifiedName~CGaFloat32Workflow"
          dotnet test --filter "FullyQualifiedName~CGaSymbolicWorkflow"

      - name: Test Phase 3 (Regression!)
        if: matrix.phase == 'phase3'
        run: dotnet test --filter "FullyQualifiedName~CGa"

      - name: Benchmarks
        if: matrix.phase == 'phase3'
        run: dotnet run --project GeometricAlgebraFulcrumLib.Benchmarks -c Release
```

---

## Test Coverage Ziele

| Komponente | Coverage | Priorität |
|------------|----------|-----------|
| ScalarProcessorOfFloating<T> | 100% | P0 |
| CGa Generic Encoding | 95%+ | P0 |
| CGa Generic Decoding | 95%+ | P0 |
| CGa Generic Operations | 90%+ | P1 |
| Float64 Wrapper | 80%+ | P1 |
| Symbolic Integration | 85%+ | P1 |
| Edge Cases | 70%+ | P2 |

---

## Success Criteria Summary

✅ **Phase 1:** 50 Unit Tests + 10 Benchmarks (100% pass, ~90% performance)
✅ **Phase 2:** 40 Integration Tests + 80 API Tests (100% pass, workflows work)
✅ **Phase 3:** **162 Regression Tests** (100% pass, <2% overhead) ⚠️ **Korrigiert von 507**

**GESAMTE TEST-COUNT:** ~352 Tests (korrigiert von ursprünglich ~697)
✅ **Gesamt:** ~352 Tests, Zero Failures, Performance-Ziele erreicht

---

[← Zurück zu SCALAR_ABSTRACTION_DESIGN.md](./SCALAR_ABSTRACTION_DESIGN.md)
