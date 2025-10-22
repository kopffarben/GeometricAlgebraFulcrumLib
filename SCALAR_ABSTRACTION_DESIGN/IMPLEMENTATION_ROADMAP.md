# GA-FUL Implementation Roadmap
## 4-Phasen Plan: 19-25 Wochen

**Teil von:** [SCALAR_ABSTRACTION_DESIGN.md](./SCALAR_ABSTRACTION_DESIGN.md)
**Version:** 3.0
**Datum:** 2025-01-22

---

## Inhaltsverzeichnis

1. [Roadmap-Übersicht](#roadmap-übersicht)
2. [Phase 0: Test-Baseline & Infrastructure (2-3 Wochen)](#phase-0-test-baseline--infrastructure-2-3-wochen)
3. [Phase 1: ScalarProcessorOfFloating<T> (1 Woche)](#phase-1-scalarprocessoroffloatingt-1-woche)
4. [Phase 2: CGa Generic API Extensions (4-6 Wochen)](#phase-2-cga-generic-api-extensions-4-6-wochen)
5. [Phase 3: CGa Float64 Wrapper Refactoring (9-11 Wochen)](#phase-3-cga-float64-wrapper-refactoring-9-11-wochen)
6. [Qualitätssicherung & Testing](#qualitätssicherung--testing)
7. [Risiko-Mitigation](#risiko-mitigation)

---

## Roadmap-Übersicht

### Zeitplan (FINAL KORRIGIERT)

```
🆕 PHASE 0 (2-3 Wochen): Test-Baseline & Infrastructure - CRITICAL!
│
├── Woche 0-1: Test-Infrastruktur aufbauen
│   ├── UnitTests .csproj erstellen + CI Integration
│   ├── 8 existierende PoC-Tests integrieren
│   └── Alle Code-Beispiele aus Docs als Tests
│
├── Woche 1-2: Baseline Regression-Tests schreiben
│   ├── 162 Regression-Tests für IST-Float64 CGa (MUSS 100% passen!)
│   ├── Performance Baseline messen
│   └── Float32 Workflow PoC validieren
│
└── Woche 2-3: Validation & Go/No-Go Decision
    ├── Symbolic Workflow PoC validieren
    ├── VGA Generic Gap analysieren
    └── GO-Decision oder Design-Revision

PHASE 1 (1 Woche): ScalarProcessorOfFloating<T>
│
└── Woche 3: Konsolidierung + 50 Unit Tests + Benchmarks

PHASE 2 (4-6 Wochen): CGa Generic API Extensions
│
├── Woche 4-5: Encoder API (~200 methods, ~400 overloads)
├── Woche 6: Decoder & Operations
├── Woche 7-8: Integration Tests (120 neue Tests)
└── Woche 9: Float32 & Symbolic Validation

PHASE 3 (9-11 Wochen): Float64 Wrapper (24k → 11-14k LOC!)
│
├── Woche 10-12: Encoder/Decoder Wrappers (83 files!)
├── Woche 13-15: Operations Wrappers
├── Woche 16-18: Elements Wrappers (9k LOC Complexity!)
├── Woche 19: Regressions-Testing (162 Tests MÜSSEN passen)
├── Woche 20: Performance Validation (<5% overhead)
└── Woche 21: Documentation & Release Prep

GESAMT: 19-25 Wochen (realistisch mit Buffer)
- Phase 0: 2-3 Wochen (NEU - CRITICAL!)
- Phase 1: 1 Woche
- Phase 2: 4-6 Wochen
- Phase 3: 9-11 Wochen (Elements: 9k LOC, Visualizer: 4.4k LOC)
- Buffer: 3-4 Wochen für Unvorhergesehenes
```

### Milestones (REVIDIERT)

| Milestone | Phase | Woche | Erfolgs-Kriterium |
|-----------|-------|-------|-------------------|
| **M0: Test-Baseline Established** | 0 | 2-3 | **162 Baseline-Tests existieren & passen** (IST: 8) |
| **M1: Float32 Processor Ready** | 1 | 3 | ScalarProcessorOfFloating<T> + 50 Tests + Benchmarks |
| **M2: CGa API Extended** | 2 | 9 | Hybrid API komplett, 120 Integration Tests pass |
| **M3: Float32 & Symbolic Validated** | 2 | 9 | Beide Workflows produktionsreif |
| **M4: Float64 Refactored** | 3 | 19 | **Alle 162 Baseline-Tests passen** nach Wrapper-Refactoring |
| **M5: Production Ready** | 3 | 21 | Performance ≤5% Overhead, Docs komplett, Release |

---

## Phase 1: ScalarProcessorOfFloating<T> (1 Woche)

### Ziele

1. `ScalarProcessorOfFloating<T>` für float, double, Half implementieren
2. `IScalarProcessor<T>.Scalar(T)` Methode hinzufügen
3. Performance validieren: ~90% von raw float
4. Unit Tests und Benchmarks

### Woche 1: ScalarProcessorOfFloating<T> Implementation

#### Task 1.1: Core Implementation

**Datei erstellen:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Floating/ScalarProcessorOfFloating.cs`

```csharp
using System.Numerics;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Floating;

/// <summary>
/// Unified scalar processor for IEEE 754 floating-point types.
/// Supports float, double, Half via IFloatingPointIeee754<T>.
/// </summary>
/// <typeparam name="T">Floating-point type (float, double, Half)</typeparam>
public sealed class ScalarProcessorOfFloating<T> : IScalarProcessor<T>
    where T : IFloatingPointIeee754<T>
{
    /// <summary>
    /// Singleton instance for performance (no allocations).
    /// </summary>
    public static ScalarProcessorOfFloating<T> Instance { get; } = new();

    private ScalarProcessorOfFloating() { }

    // Zero/One Properties
    public T ZeroValue => T.Zero;
    public T OneValue => T.One;
    public T MinusOneValue => -T.One;
    public T TwoValue => T.One + T.One;
    public T TenValue => T.CreateChecked(10);
    public T PiValue => T.Pi;
    public T ERValue => T.E;

    public Scalar<T> Zero => new Scalar<T>(this, T.Zero);
    public Scalar<T> One => new Scalar<T>(this, T.One);
    public Scalar<T> MinusOne => new Scalar<T>(this, -T.One);
    public Scalar<T> Two => new Scalar<T>(this, T.One + T.One);
    public Scalar<T> Ten => new Scalar<T>(this, T.CreateChecked(10));
    public Scalar<T> Pi => new Scalar<T>(this, T.Pi);
    public Scalar<T> PiTimes2 => new Scalar<T>(this, T.CreateChecked(2) * T.Pi);
    public Scalar<T> PiTimes4 => new Scalar<T>(this, T.CreateChecked(4) * T.Pi);
    public Scalar<T> PiOver2 => new Scalar<T>(this, T.Pi / T.CreateChecked(2));
    public Scalar<T> E => new Scalar<T>(this, T.E);
    public Scalar<T> DegreeToRadianFactor => new Scalar<T>(this, T.Pi / T.CreateChecked(180));
    public Scalar<T> RadianToDegreeFactor => new Scalar<T>(this, T.CreateChecked(180) / T.Pi);

    // NEW: Wrapping method
    public Scalar<T> Scalar(T value) => new Scalar<T>(this, value);

    // Arithmetic Operations
    public Scalar<T> Add(T a, T b) => new Scalar<T>(this, a + b);
    public Scalar<T> Subtract(T a, T b) => new Scalar<T>(this, a - b);
    public Scalar<T> Times(T a, T b) => new Scalar<T>(this, a * b);
    public Scalar<T> Divide(T a, T b) => new Scalar<T>(this, a / b);
    public Scalar<T> Negative(T a) => new Scalar<T>(this, -a);

    // Power & Root
    public Scalar<T> Square(T a) => new Scalar<T>(this, a * a);
    public Scalar<T> Cube(T a) => new Scalar<T>(this, a * a * a);
    public Scalar<T> Sqrt(T a) => new Scalar<T>(this, T.Sqrt(a));
    public Scalar<T> SqrtOfAbs(T a) => new Scalar<T>(this, T.Sqrt(T.Abs(a)));
    public Scalar<T> Exp(T a) => new Scalar<T>(this, T.Exp(a));
    public Scalar<T> Log(T a) => new Scalar<T>(this, T.Log(a));
    public Scalar<T> Log2(T a) => new Scalar<T>(this, T.Log2(a));
    public Scalar<T> Log10(T a) => new Scalar<T>(this, T.Log10(a));
    public Scalar<T> Power(T a, T b) => new Scalar<T>(this, T.Pow(a, b));

    // Trigonometric
    public Scalar<T> Cos(T a) => new Scalar<T>(this, T.Cos(a));
    public Scalar<T> Sin(T a) => new Scalar<T>(this, T.Sin(a));
    public Scalar<T> Tan(T a) => new Scalar<T>(this, T.Tan(a));
    public Scalar<T> Cosh(T a) => new Scalar<T>(this, T.Cosh(a));
    public Scalar<T> Sinh(T a) => new Scalar<T>(this, T.Sinh(a));
    public Scalar<T> Tanh(T a) => new Scalar<T>(this, T.Tanh(a));

    // Inverse Trigonometric
    public Scalar<T> ArcCos(T a) => new Scalar<T>(this, T.Acos(a));
    public Scalar<T> ArcSin(T a) => new Scalar<T>(this, T.Asin(a));
    public Scalar<T> ArcTan(T a) => new Scalar<T>(this, T.Atan(a));
    public Scalar<T> ArcTan2(T y, T x) => new Scalar<T>(this, T.Atan2(y, x));

    // Comparisons & Tests
    public bool IsZero(T a) => a == T.Zero;
    public bool IsZero(T a, bool nearZeroFlag) => nearZeroFlag ? T.Abs(a) < T.CreateChecked(1e-12) : a == T.Zero;
    public bool IsPositive(T a) => a > T.Zero;
    public bool IsNegative(T a) => a < T.Zero;
    public bool IsNotZero(T a) => a != T.Zero;
    public bool IsNotNearZero(T a) => T.Abs(a) >= T.CreateChecked(1e-12);

    // Value Conversions
    public T ValueFromNumber(int number) => T.CreateChecked(number);
    public T ValueFromNumber(uint number) => T.CreateChecked(number);
    public T ValueFromNumber(long number) => T.CreateChecked(number);
    public T ValueFromNumber(ulong number) => T.CreateChecked(number);
    public T ValueFromNumber(float number) => T.CreateChecked(number);
    public T ValueFromNumber(double number) => T.CreateChecked(number);
    public T ValueFromText(string text) => T.Parse(text, null);

    // Scalar Conversions
    public Scalar<T> ScalarFromNumber(int number) => new Scalar<T>(this, T.CreateChecked(number));
    public Scalar<T> ScalarFromNumber(uint number) => new Scalar<T>(this, T.CreateChecked(number));
    public Scalar<T> ScalarFromNumber(long number) => new Scalar<T>(this, T.CreateChecked(number));
    public Scalar<T> ScalarFromNumber(ulong number) => new Scalar<T>(this, T.CreateChecked(number));
    public Scalar<T> ScalarFromNumber(float number) => new Scalar<T>(this, T.CreateChecked(number));
    public Scalar<T> ScalarFromNumber(double number) => new Scalar<T>(this, T.CreateChecked(number));
    public Scalar<T> ScalarFromText(string text) => new Scalar<T>(this, T.Parse(text, null));
    public Scalar<T> ScalarFromValue(T value) => new Scalar<T>(this, value);

    // Utility
    public T Abs(T a) => T.Abs(a);
    public T GetScalarFromText(string text) => T.Parse(text, null);
    public T GetScalarFromNumber(int number) => T.CreateChecked(number);
    public T GetScalarFromNumber(double number) => T.CreateChecked(number);
    public T GetScalarFromRational(long numerator, long denominator)
        => T.CreateChecked(numerator) / T.CreateChecked(denominator);
    public T GetScalarFromRational(ulong numerator, ulong denominator)
        => T.CreateChecked(numerator) / T.CreateChecked(denominator);

    // ToString
    public string ToText(T a) => a.ToString() ?? string.Empty;
}
```

**Aufwand:** 2-3 Stunden

#### Task 1.2: IScalarProcessor Extension

**Datei modifizieren:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Generic/IScalarProcessor.cs`

```csharp
public interface IScalarProcessor<T>
{
    // Bestehende Methoden...

    // NEW: Wrapping method für Konsistenz
    Scalar<T> Scalar(T value);

    // Optional: Convenience
    Scalar<T> ScalarFromValue(T value);
}
```

**Alle bestehenden Implementierungen updaten:**
- `ScalarProcessorOfFloat64`
- `ScalarProcessorOfERational`
- `ScalarProcessorOfEDecimal`
- Etc. (alle 11 Implementierungen)

**Aufwand:** 2 Stunden

#### Task 1.3: Unit Tests

**Datei erstellen:** `GeometricAlgebraFulcrumLib.UnitTests/Algebra/ScalarProcessorOfFloatingTests.cs`

```csharp
using NUnit.Framework;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Floating;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

[TestFixture]
public class ScalarProcessorOfFloatingTests
{
    [TestFixture]
    public class Float32Tests
    {
        private ScalarProcessorOfFloating<float> _processor = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = ScalarProcessorOfFloating<float>.Instance;
        }

        [Test]
        public void ZeroOne_ShouldHaveCorrectValues()
        {
            Assert.That(_processor.ZeroValue, Is.EqualTo(0.0f));
            Assert.That(_processor.OneValue, Is.EqualTo(1.0f));
            Assert.That(_processor.Zero.ScalarValue, Is.EqualTo(0.0f));
            Assert.That(_processor.One.ScalarValue, Is.EqualTo(1.0f));
        }

        [Test]
        public void Add_ShouldReturnCorrectSum()
        {
            var result = _processor.Add(2.5f, 3.7f);
            Assert.That(result.ScalarValue, Is.EqualTo(6.2f).Within(1e-6f));
        }

        [Test]
        public void Multiply_ShouldReturnCorrectProduct()
        {
            var result = _processor.Times(2.5f, 4.0f);
            Assert.That(result.ScalarValue, Is.EqualTo(10.0f).Within(1e-6f));
        }

        [Test]
        public void Sqrt_ShouldReturnCorrectRoot()
        {
            var result = _processor.Sqrt(9.0f);
            Assert.That(result.ScalarValue, Is.EqualTo(3.0f).Within(1e-6f));
        }

        [Test]
        public void Sin_ShouldReturnCorrectValue()
        {
            var result = _processor.Sin(MathF.PI / 2.0f);
            Assert.That(result.ScalarValue, Is.EqualTo(1.0f).Within(1e-6f));
        }

        [Test]
        public void ScalarWrapping_ShouldWork()
        {
            var scalar = _processor.Scalar(5.0f);
            Assert.That(scalar.ScalarValue, Is.EqualTo(5.0f));
            Assert.That(scalar.ScalarProcessor, Is.EqualTo(_processor));
        }

        [Test]
        public void ValueFromNumber_ShouldConvertCorrectly()
        {
            Assert.That(_processor.ValueFromNumber(5), Is.EqualTo(5.0f));
            Assert.That(_processor.ValueFromNumber(5.5), Is.EqualTo(5.5f).Within(1e-6f));
            Assert.That(_processor.ValueFromNumber(5.5f), Is.EqualTo(5.5f));
        }
    }

    [TestFixture]
    public class Float64Tests
    {
        private ScalarProcessorOfFloating<double> _processor = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = ScalarProcessorOfFloating<double>.Instance;
        }

        [Test]
        public void ConsistencyWithFloat64Processor()
        {
            var oldProcessor = ScalarProcessorOfFloat64.Instance;
            var newProcessor = ScalarProcessorOfFloating<double>.Instance;

            // Verify identical behavior
            var a = 2.5;
            var b = 3.7;

            var oldResult = oldProcessor.Add(a, b).ScalarValue;
            var newResult = newProcessor.Add(a, b).ScalarValue;

            Assert.That(newResult, Is.EqualTo(oldResult));
        }

        [Test]
        public void PiValue_ShouldBeCorrect()
        {
            Assert.That(_processor.PiValue, Is.EqualTo(Math.PI).Within(1e-15));
            Assert.That(_processor.Pi.ScalarValue, Is.EqualTo(Math.PI).Within(1e-15));
        }
    }

    [TestFixture]
    public class HalfTests
    {
        private ScalarProcessorOfFloating<Half> _processor = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = ScalarProcessorOfFloating<Half>.Instance;
        }

        [Test]
        public void BasicOperations_ShouldWork()
        {
            var a = (Half)2.5;
            var b = (Half)3.5;

            var sum = _processor.Add(a, b);
            var product = _processor.Times(a, b);

            Assert.That((double)sum.ScalarValue, Is.EqualTo(6.0).Within(0.1)); // Half precision!
            Assert.That((double)product.ScalarValue, Is.EqualTo(8.75).Within(0.1));
        }
    }
}
```

**Aufwand:** 4 Stunden

**Gesamt Woche 1:** ~8-10 Stunden

### Woche 2: Testing & IScalarProcessor Extensions

#### Task 2.1: Erweiterte Tests

- **Comparison Tests:** Verify Float64 consistency
- **Edge Cases:** NaN, Infinity, Denormals
- **Transcendental Functions:** Sin, Cos, Exp, Log
- **Conversions:** int → float, double → float

**Aufwand:** 6 Stunden

#### Task 2.2: IScalarProcessor.Scalar(T) Rollout

Alle 11 Implementierungen updaten:

```csharp
// ScalarProcessorOfFloat64.cs
public Scalar<double> Scalar(double value) => new Scalar<double>(this, value);

// ScalarProcessorOfERational.cs
public Scalar<ERational> Scalar(ERational value) => new Scalar<ERational>(this, value);

// Etc...
```

**Aufwand:** 3 Stunden

#### Task 2.3: XGa Integration Tests

Verify ScalarProcessorOfFloating<float> works mit XGaProcessor:

```csharp
[Test]
public void XGaProcessor_WithFloat32_ShouldWork()
{
    var scalarProcessor = ScalarProcessorOfFloating<float>.Instance;
    var xgaProcessor = scalarProcessor.CreateEuclideanXGaProcessor();

    var v1 = xgaProcessor.Vector(1.0f, 2.0f, 3.0f);
    var v2 = xgaProcessor.Vector(4.0f, 5.0f, 6.0f);

    var dot = v1.Sp(v2);  // Scalar product

    Assert.That(dot.ScalarValue, Is.EqualTo(32.0f).Within(1e-5f));
}
```

**Aufwand:** 5 Stunden

**Gesamt Woche 2:** ~14 Stunden

### Woche 3: Performance Validation

#### Task 3.1: Benchmark Setup

**Datei erstellen:** `GeometricAlgebraFulcrumLib.Benchmarks/ScalarProcessorBenchmarks.cs`

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Floating;

namespace GeometricAlgebraFulcrumLib.Benchmarks;

[MemoryDiagnoser]
public class ScalarProcessorFloat32Benchmarks
{
    private const int Iterations = 100_000;
    private float[] _data = null!;
    private ScalarProcessorOfFloating<float> _processor = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = new float[Iterations];
        var random = new Random(42);
        for (int i = 0; i < Iterations; i++)
            _data[i] = (float)random.NextDouble() * 100.0f;

        _processor = ScalarProcessorOfFloating<float>.Instance;
    }

    [Benchmark(Baseline = true)]
    public float RawFloat_Add()
    {
        float sum = 0.0f;
        for (int i = 0; i < Iterations - 1; i++)
            sum += _data[i] + _data[i + 1];
        return sum;
    }

    [Benchmark]
    public float ScalarProcessor_Add()
    {
        float sum = 0.0f;
        for (int i = 0; i < Iterations - 1; i++)
        {
            var result = _processor.Add(_data[i], _data[i + 1]);
            sum += result.ScalarValue;
        }
        return sum;
    }

    [Benchmark]
    public float RawFloat_Multiply()
    {
        float product = 1.0f;
        for (int i = 0; i < Iterations - 1; i++)
            product *= _data[i] * _data[i + 1];
        return product;
    }

    [Benchmark]
    public float ScalarProcessor_Multiply()
    {
        float product = 1.0f;
        for (int i = 0; i < Iterations - 1; i++)
        {
            var result = _processor.Times(_data[i], _data[i + 1]);
            product *= result.ScalarValue;
        }
        return product;
    }

    [Benchmark]
    public float RawFloat_Sqrt()
    {
        float sum = 0.0f;
        for (int i = 0; i < Iterations; i++)
            sum += MathF.Sqrt(_data[i]);
        return sum;
    }

    [Benchmark]
    public float ScalarProcessor_Sqrt()
    {
        float sum = 0.0f;
        for (int i = 0; i < Iterations; i++)
        {
            var result = _processor.Sqrt(_data[i]);
            sum += result.ScalarValue;
        }
        return sum;
    }
}
```

**Aufwand:** 4 Stunden

#### Task 3.2: Performance Analysis

Run benchmarks und analysieren:

```bash
cd GeometricAlgebraFulcrumLib.Benchmarks
dotnet run -c Release --filter *ScalarProcessorFloat32*
```

**Erwartete Ergebnisse:**

| Operation | Raw Float | ScalarProcessor | Overhead | Target |
|-----------|-----------|-----------------|----------|--------|
| Add | 100 ns | ~105 ns | ~5% | ✅ <10% |
| Multiply | 100 ns | ~105 ns | ~5% | ✅ <10% |
| Sqrt | 500 ns | ~510 ns | ~2% | ✅ <10% |

**Success Criteria:** <10% Overhead → ~90%+ Performance ✅

**Aufwand:** 6 Stunden

#### Task 3.3: Documentation

- Performance-Ergebnisse dokumentieren
- README für ScalarProcessorOfFloating<T> erstellen
- API-Dokumentation (XML comments)

**Aufwand:** 4 Stunden

**Gesamt Woche 3:** ~14 Stunden

### Phase 1 Deliverables

✅ **ScalarProcessorOfFloating<T>** für float, double, Half
✅ **IScalarProcessor<T>.Scalar(T)** implementiert in allen Processoren
✅ **Unit Tests:** 100% pass
✅ **Performance:** ~90%+ von raw float (benchmarked)
✅ **Documentation:** README + XML comments

**Milestone M1 & M2 erreicht!**

---

## Phase 2: CGa Generic API Extensions (4-6 Wochen)

### Ziele

1. CGa Generic mit T + Scalar<T> + convenience Überladungen erweitern
2. Interne raw T Performance sicherstellen
3. Float32 Workflow validieren
4. Symbolischer Workflow validieren

### Woche 4: Code-Generation + Encoder Core

#### Task 4.1: T4 Template Setup

**Datei erstellen:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/Encoding/EncoderOverloads.tt`

```xml
<#@ template debug="false" hostspecific="false" language="C#" #>
<#@ output extension=".g.cs" #>
<#
    var scalarTypes = new[] {
        ("T", "value", false),
        ("Scalar<T>", "value.ScalarValue", true),
        ("IScalar<T>", "value.ScalarValue", true),
        ("double", "ScalarProcessor.ValueFromNumber(value)", true),
        ("float", "ScalarProcessor.ValueFromNumber(value)", true),
        ("int", "ScalarProcessor.ValueFromNumber(value)", true)
    };

    var methods = new[] {
        ("HyperSphere", new[] { "radiusSquared" }),
        ("Point", new[] { "x", "y", "z" }),
        ("Circle", new[] { "radiusSquared", "centerX", "centerY" }),
        ("Sphere", new[] { "radiusSquared", "centerX", "centerY", "centerZ" })
    };
#>
// <auto-generated>
// This file was generated by T4 template: EncoderOverloads.tt
// DO NOT EDIT MANUALLY!
// </auto-generated>

using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Encoding;

public partial class CGaIpnsRoundEncoder<T>
{
<#
    foreach (var (methodName, parameters) in methods)
    {
        foreach (var (type, unwrap, needsUnwrap) in scalarTypes)
        {
            if (type == "T") continue; // Core method in main file

            var paramList = string.Join(", ", parameters.Select(p => $"{type} {p}"));
            var coreArgs = string.Join(", ", parameters.Select(p =>
                needsUnwrap ? unwrap.Replace("value", p) : p
            ));
#>
    public CGaBlade<T> <#= methodName #>(<#= paramList #>)
    {
        return <#= methodName #>Core(<#= coreArgs #>);
    }

<#
        }
    }
#>
}
```

**Aufwand:** 6 Stunden

#### Task 4.2: Core Methods Refactoring

**Beispiel:** `CGaIpnsRoundEncoder.cs`

```csharp
public partial class CGaIpnsRoundEncoder<T> : CGaEncoderBase<T>
{
    // CORE METHOD: Raw T für Performance
    public CGaBlade<T> HyperSphere(T radiusSquared)
    {
        return GeometricSpace.Eo -
               ScalarProcessor.Times(0.5d, radiusSquared) * GeometricSpace.Ei;
    }

    private CGaBlade<T> HyperSphereCore(T radiusSquared, XGaVector<T> egaCenter)
    {
        var c = PointCore(egaCenter);
        // ✅ KORRIGIERT: ScalarFromNumber() statt direktem 0.5d
        var half = ScalarProcessor.ScalarFromNumber(0.5);
        return c - ScalarProcessor.Times(half.ScalarValue, radiusSquared) * GeometricSpace.Ei;
    }

    // Point Core (raw T)
    private CGaBlade<T> PointCore(T x, T y, T z)
    {
        var p = GeometricSpace.Encode.VGa.VectorAsXGaVectorCore(x, y, z);

        // Raw T arithmetic für Performance!
        var pNormSquared = ScalarProcessor.Add(
            ScalarProcessor.Times(x, x),
            ScalarProcessor.Add(
                ScalarProcessor.Times(y, y),
                ScalarProcessor.Times(z, z)
            )
        ).ScalarValue;  // Unwrap zu T!

        var kVector = GeometricSpace.EoVector +
                      p +
                      ScalarProcessor.Times(0.5d, pNormSquared) * GeometricSpace.EiVector;

        return new CGaBlade<T>(GeometricSpace, kVector);
    }

    private CGaBlade<T> PointCore(XGaVector<T> egaPoint)
    {
        var p = GeometricSpace.Encode.VGa.VectorAsXGaVector(egaPoint);
        var pNormSquared = egaPoint.NormSquared().ScalarValue;  // Unwrap!

        var kVector = GeometricSpace.EoVector +
                      p +
                      ScalarProcessor.Times(0.5d, pNormSquared) * GeometricSpace.EiVector;

        return new CGaBlade<T>(GeometricSpace, kVector);
    }

    // Public T overload
    public CGaBlade<T> Point(T x, T y, T z)
    {
        return PointCore(x, y, z);
    }

    // Overloads generiert durch T4 template
    // ... (siehe EncoderOverloads.g.cs)
}
```

**Dateien:** 4 Encoder-Klassen × ~15 Methoden = ~60 Core-Implementations

**Aufwand:** 16 Stunden (2 Tage)

**Gesamt Woche 4:** ~22 Stunden

### Woche 5: Decoding + Operations

#### Task 5.1: Decoding Refactoring

Analog zu Encoding:
- `CGaIpnsDecoder<T>`
- `CGaOpnsDecoder<T>`

**Aufwand:** 12 Stunden

#### Task 5.2: Operations Refactoring

- `CGaBladeOperations<T>.TranslateBy(...)` etc.
- Interne raw T Verwendung

**Aufwand:** 8 Stunden

#### Task 5.3: Code Generation

T4 Templates ausführen:

```bash
dotnet build /t:TransformTemplates
```

**Aufwand:** 2 Stunden

**Gesamt Woche 5:** ~22 Stunden

### Woche 6: Float32 & Symbolic Integration

#### Task 6.1: Float32 Workflow Tests

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

    [Test]
    public void Point_Float32_ShouldEncodeDecode()
    {
        // Encode
        var point = _cga.Encode.IpnsRound.Point(1.0f, 2.0f, 3.0f);

        // Decode
        var decoded = point.Decode.IpnsFlat.Position();

        Assert.That(decoded.X.ScalarValue, Is.EqualTo(1.0f).Within(1e-5f));
        Assert.That(decoded.Y.ScalarValue, Is.EqualTo(2.0f).Within(1e-5f));
        Assert.That(decoded.Z.ScalarValue, Is.EqualTo(3.0f).Within(1e-5f));
    }

    [Test]
    public void Circle_Float32_Operations()
    {
        var circle = _cga.Encode.IpnsRound.RealCircle(5.0f, 0.0f, 0.0f, 0.0f);
        var translated = circle.TranslateBy(1.0f, 2.0f, 3.0f);

        var newCenter = translated.Decode.IpnsRound.Center();
        Assert.That(newCenter.X.ScalarValue, Is.EqualTo(1.0f).Within(1e-4f));
    }
}
```

**Aufwand:** 8 Stunden

#### Task 6.2: Symbolic Workflow Tests

```csharp
[TestFixture]
public class CGaSymbolicWorkflowTests
{
    [Test]
    public void SymbolicCircle_ShouldOptimize()
    {
        var context = new MetaContext();
        var cga = CGaGeometricSpace5D<IMetaExpressionAtomic>.Create(context);

        var r = context.GetOrDefineParameterVariable("r");
        var x = context.GetOrDefineParameterVariable("x");
        var y = context.GetOrDefineParameterVariable("y");

        // Symbolische Operationen
        var circle = cga.Encode.IpnsRound.Circle(r, x, y);
        var doubled = 2 * circle;  // Operators!

        // Optimieren
        context.OptimizeContext();

        // Code-Gen
        var codeGen = new GaFuLMetaContextCodeComposer(context, "float");
        var code = codeGen.Generate();

        Assert.That(code, Does.Contain("float"));
        Assert.That(context.GetComputedVariables().Count(), Is.GreaterThan(0));
    }
}
```

**Aufwand:** 10 Stunden

#### Task 6.3: Performance Validation

CGa Float32 Operations benchmarken:

```csharp
[Benchmark]
public CGaBlade<float> Float32_EncodePoint()
{
    return _cgaFloat32.Encode.IpnsRound.Point(1.0f, 2.0f, 3.0f);
}

[Benchmark]
public CGaBlade<double> Float64_EncodePoint()
{
    return _cgaFloat64Generic.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
}
```

**Success:** Float32 ~90% von Float64 ✅

**Aufwand:** 6 Stunden

**Gesamt Woche 6:** ~24 Stunden

### Phase 2 Deliverables

✅ **CGa Generic API komplett erweitert** (~60 Core + ~300 Generated Overloads)
✅ **Float32 Workflow funktioniert**
✅ **Symbolischer Workflow funktioniert**
✅ **Performance ~90%** für Float32
✅ **Tests:** Integration Tests für beide Workflows

**Milestone M3 erreicht!**

---

## Phase 3: CGa Float64 Wrapper Refactoring (9-11 Wochen)

### Ziele

1. Float64 zu dünnem Wrapper über Generic<double> umbauen (24k → 11-14k LOC)
2. Public API 100% beibehalten (Zero Breaking Changes!)
3. Alle **162 Tests** müssen passen
4. Performance <5% Overhead

### Woche 7: Wrapper Implementation

#### Task 7.1: CGaFloat64GeometricSpace Refactoring

**Before:**
```csharp
public class CGaFloat64GeometricSpace5D
{
    // Eigene Implementation...
    public XGaFloat64Processor Processor { get; }
    // ...
}
```

**After:**
```csharp
public class CGaFloat64GeometricSpace5D
{
    // Interne Generic Space
    internal readonly CGaGeometricSpace5D<double> GenericSpace;

    // Singleton
    public static CGaFloat64GeometricSpace5D Instance { get; } = new();

    private CGaFloat64GeometricSpace5D()
    {
        var processor = ScalarProcessorOfFloating<double>.Instance;
        GenericSpace = CGaGeometricSpace5D<double>.Create(processor);
    }

    // Public Properties - delegieren
    public int VSpaceDimensions => GenericSpace.VSpaceDimensions;
    public bool Is5D => GenericSpace.Is5D;

    // Encode delegiert
    public CGaFloat64Encode Encode { get; }

    // Decode delegiert
    public CGaFloat64Decode Decode { get; }

    // Basis Blades delegieren
    public CGaFloat64Blade E1 => new CGaFloat64Blade(this, GenericSpace.E1);
    public CGaFloat64Blade E2 => new CGaFloat64Blade(this, GenericSpace.E2);
    public CGaFloat64Blade E3 => new CGaFloat64Blade(this, GenericSpace.E3);
    public CGaFloat64Blade Eo => new CGaFloat64Blade(this, GenericSpace.Eo);
    public CGaFloat64Blade Ei => new CGaFloat64Blade(this, GenericSpace.Ei);
    // ...
}
```

**Aufwand:** 8 Stunden

#### Task 7.2: Encoder/Decoder Wrapper

**Before:** Separate Implementation
**After:** Wrapper

```csharp
public class CGaFloat64IpnsRoundEncoder
{
    private readonly CGaFloat64GeometricSpace _geometricSpace;
    private readonly CGaIpnsRoundEncoder<double> _genericEncoder;

    internal CGaFloat64IpnsRoundEncoder(CGaFloat64GeometricSpace geometricSpace)
    {
        _geometricSpace = geometricSpace;
        _genericEncoder = geometricSpace.GenericSpace.Encode.IpnsRound;
    }

    // Public API IDENTISCH - delegiert
    public CGaFloat64Blade Point(double x, double y, double z)
    {
        var genericResult = _genericEncoder.Point(x, y, z);
        return new CGaFloat64Blade(_geometricSpace, genericResult.InternalKVector);
    }

    public CGaFloat64Blade Circle(double radiusSquared, double centerX, double centerY)
    {
        var genericResult = _genericEncoder.Circle(radiusSquared, centerX, centerY);
        return new CGaFloat64Blade(_geometricSpace, genericResult.InternalKVector);
    }

    // Alle Methoden analog...
}
```

**Dateien:** 4 Encoder + 4 Decoder Klassen

**Aufwand:** 16 Stunden (2 Tage)

**Gesamt Woche 7:** ~24 Stunden

### Woche 8: Regressions-Testing

#### Task 8.1: Alle CGa Tests ausführen

```bash
cd GeometricAlgebraFulcrumLib.UnitTests
dotnet test --filter "FullyQualifiedName~CGa" --verbosity normal
```

**Erwartung:** Alle **162 Tests** MÜSSEN passen! ✅ (korrigiert von 507)

**Bei Failures:**
- Debuggen und fixen
- Wrapper API anpassen
- Wiederhole Tests

**Aufwand:** 16 Stunden

#### Task 8.2: Applications Testing

**Datei:** `GeometricAlgebraFulcrumLib.Applications/Robotics/InverseKinematics6R.cs`

Manuell testen, dass Output identisch ist.

**Aufwand:** 4 Stunden

#### Task 8.3: Edge Cases

- NaN/Infinity Handling
- Zero Vectors
- Denormalized Numbers

**Aufwand:** 4 Stunden

**Gesamt Woche 8:** ~24 Stunden

### Woche 9: Performance & Documentation

#### Task 9.1: Performance Benchmarks

```csharp
[Benchmark(Baseline = true)]
public CGaFloat64Blade OldFloat64_Point()
{
    // Alte Implementation (baseline)
}

[Benchmark]
public CGaFloat64Blade NewFloat64Wrapper_Point()
{
    return _newCga.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
}
```

**Success Criteria:** <1% Overhead ✅

**Aufwand:** 8 Stunden

#### Task 9.2: Final Documentation

- [MIGRATION_GUIDE.md](./MIGRATION_GUIDE.md) vervollständigen
- API-Dokumentation updaten
- README für CGa Refactoring

**Aufwand:** 8 Stunden

#### Task 9.3: Release Preparation

- Change Log erstellen
- GitHub Release vorbereiten
- Announcement Draft

**Aufwand:** 4 Stunden

**Gesamt Woche 9:** ~20 Stunden

### Phase 3 Deliverables

✅ **Float64 ist dünner Wrapper** über Generic<double>
✅ **Alle 162 CGa Tests passen** (korrigiert von 507)
✅ **Performance <1% Overhead**
✅ **Documentation komplett**
✅ **Applications funktionieren**

**Milestone M4 & M5 erreicht!**

---

## Qualitätssicherung & Testing

### Test-Strategie pro Phase

| Phase | Test-Typ | Umfang | Success Criteria |
|-------|----------|--------|------------------|
| **1** | Unit Tests | ScalarProcessorOfFloating<T> | 100% pass |
| **1** | Performance | Float32 Benchmarks | ~90% von raw |
| **2** | Integration | Float32 + Symbolic Workflows | Funktioniert ✅ |
| **2** | Unit Tests | CGa Generic API | Neue Tests pass |
| **3** | Regression | Alle **162** CGa Tests (korrigiert von 507) | 100% pass |
| **3** | Performance | Float64 Wrapper | <1% overhead |
| **3** | Manual | Applications | Identische Results |

### Continuous Integration

**Nach jeder Phase:**
```bash
# Alle Tests
dotnet test GeometricAlgebraFulcrumLib.sln --verbosity normal

# CGa spezifisch
dotnet test --filter "FullyQualifiedName~CGa"

# Performance
cd GeometricAlgebraFulcrumLib.Benchmarks
dotnet run -c Release
```

---

## Risiko-Mitigation

### Identifizierte Risiken

| Risiko | Wahrscheinlichkeit | Impact | Mitigation |
|--------|-------------------|--------|------------|
| **Performance-Ziel verfehlt** (Float32 <90%) | LOW | HIGH | Benchmark early (Woche 3), Optimization-Loop |
| **162 Tests fail** (Float64 Wrapper - korrigiert von 507) | MEDIUM | HIGH | Incremental migration, Test-Driven |
| **API Breaking Changes** | LOW | CRITICAL | Public API dokumentieren, Contract Tests |
| **Symbolic Workflow breaks** | LOW | MEDIUM | MetaContext Tests früh (Woche 6) |
| **Scope Creep** | MEDIUM | MEDIUM | Striktes Scope-Management, nur CGa |

### Mitigation-Strategien

1. **Early Benchmarking:** Woche 3 (nicht erst Woche 9!)
2. **Incremental Testing:** Nach jedem Encoder/Decoder
3. **Contract Tests:** Public API signatures testen
4. **Rollback Plan:** Git Branches pro Phase, easy Rollback

---

## Zusammenfassung

### Timeline

| Phase | Wochen | Stunden (40h/Woche) | Deliverables |
|-------|--------|---------------------|--------------|
| **Phase 0** | 2-3 | ~80-120 | **Test-Baseline (162 Tests) + Infrastructure** |
| **Phase 1** | 1 | ~40 | ScalarProcessorOfFloating<T> + 50 Tests |
| **Phase 2** | 4-6 | ~160-240 | CGa Generic API + 120 Tests |
| **Phase 3** | 6-7 | ~240-280 | Float64 Wrapper (25k LOC!) + Validation |
| **Buffer** | 2-3 | ~80-120 | Unvorhergesehenes, Bugfixes |
| **GESAMT** | **15-20** | **~600-800h** | **Production Ready!** |

### Success Criteria

✅ **ScalarProcessorOfFloating<T>** funktioniert für float, double, Half
✅ **Performance:** Float32 ~90%, Float64 Wrapper <2% overhead
✅ **CGa Workflows:** Float32 GPU + Symbolic funktionieren
✅ **Tests:** Alle **162** CGa Tests passen
✅ **Backward Compatibility:** Float64 API 100% kompatibel
✅ **Documentation:** Komplett und aktuell

---

## Nächste Schritte

1. **Review dieser Roadmap** mit Stakeholdern
2. **Phase 1 starten:** ScalarProcessorOfFloating<T> Implementation
3. **Weiter lesen:**
   - [MIGRATION_GUIDE.md](./MIGRATION_GUIDE.md) - Für User-Perspektive
   - [TESTING_STRATEGY.md](./TESTING_STRATEGY.md) - Detaillierte Test-Pläne
   - [PERFORMANCE_ANALYSIS.md](./PERFORMANCE_ANALYSIS.md) - Benchmark-Details

---

[← Zurück zu SCALAR_ABSTRACTION_DESIGN.md](./SCALAR_ABSTRACTION_DESIGN.md)
