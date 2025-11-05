# Numerical Operations Infrastructure - Dual-Backend Strategy

**Erstellt:** 2025-11-05
**Status:** 📋 PLANNED - Ready for Implementation
**Priorität:** P0 (CRITICAL - Blocks Phase 3 Module 6A/6B completion)
**Geschätzter Aufwand:** 1-2 Wochen

---

## 🎯 Executive Summary

**Problem:** Generic<T> Trajectory-Klassen (Module 6A/6B) benötigen numerische Differentiation für 100% API-Parität mit Float64, müssen aber für double, float UND symbolic types funktionieren.

**Lösung:** Dual-Backend Infrastructure mit `INumericalOperations<T>`:
- **Math.NET** für double/float (fast, numerical approximation)
- **AngouriMath** für symbolic types (exact, symbolic computation)

**Scope:** Nur 2 kritische Operationen benötigt:
- `Differentiate(func, point)` - Erste Ableitung
- `Differentiate2(func, point)` - Zweite Ableitung

**Performance-Garantie:** ≥95% von Float64 (proven pattern aus Phase 1 Optimizations)

---

## 📊 Situationsanalyse (Verifiziert via Codebase)

### Math.NET.Numerics Nutzung in GA-FUL

**Gesamtnutzung:** 20 Dateien

**Verwendungsmuster:**

1. **Numerische Differentiation** (KRITISCH - 14 Dateien)
   - `Differentiate.FirstDerivative(func, point)` - Erste Ableitung
   - `Differentiate.SecondDerivative(func, point)` - Zweite Ableitung
   - **Kontext**: Immer als FALLBACK wenn analytische Ableitungen nicht verfügbar
   - **Beispiel**: `Float64CatmullRomSplinePath2D/3D` bei Edge-Cases (t <= 0 or t >= 1)

2. **Interpolation** (5 Dateien)
   - Polynomial, Fourier, Akima, Barycentric Interpolators
   - **Status**: Nicht-kritisch für Phase 3 (separate Module)

3. **Random Number Generation** (1 Datei)
   - `RandomGeneratorUtils.cs`
   - **Status**: Nicht relevant für Generic<T>

**Kritische Erkenntnis**: Math.NET wird in Generic<T> NICHT für Matrix-Operationen verwendet!
- `XGaGramSchmidtFrame<T>` verwendet pure modified Gram-Schmidt Algorithmus
- KEINE QR/SVD/LU Decomposition in Generic

### AngouriMath Integration

**Status:** ✅ Bereits vollständig integriert

**Hauptklasse:** `ScalarProcessorOfAngouriMathEntity` (552 LOC)
- Implementiert `IScalarProcessor<Entity>`
- Unterstützt alle arithmetischen + transzendentalen Funktionen
- Verwendet nur in MetaProgramming Layer

**Capabilities:**
- ✅ Symbolic differentiation: `expr.Differentiate(variable)`
- ✅ Symbolic integration: `expr.Integrate(variable)`
- ✅ Symbolic simplification: `expr.Simplify()`
- ✅ Equation solving: `expr.Solve(variable)`

**Vorteil für Generic<T>**: Symbolic differentiation ist EXAKT, nicht numerisch approximiert!

### IScalarProcessor<T> Architecture

**Status:** ✅ Perfekt designed, benötigt nur Extension

**Bestehende API** (193 Zeilen):
- Alle arithmetischen Operationen (Add, Subtract, Times, Divide)
- Transzendentale Funktionen (Sin, Cos, Exp, Log, Sqrt, Power, etc.)
- Konstanten (Zero, One, Pi, E, DegreeToRadianFactor, etc.)

**Implementationen:**
- `ScalarProcessorOfFloat64` : IScalarProcessor<double> ✅
- `ScalarProcessorOfFloat32` : IScalarProcessor<float> ✅
- `ScalarProcessorOfAngouriMathEntity` : IScalarProcessor<Entity> ✅

**Was fehlt:** Numerische Differentiation/Integration

---

## 🏗️ Infrastructure Design

### Architecture Overview

```
                    INumericalOperations<T>
                    (new interface)
                              ▲
                              │ implements
            ┌─────────────────┼─────────────────┐
            │                 │                 │
  MathNetNumerics         MathNetNumerics    AngouriMath
  Operations<double>      Operations<float>  NumericalOperations
  (new)                   (new)              (new)
            │                 │                 │
            ├─────────────────┼─────────────────┤
            │                 │                 │
    Math.NET.Numerics   Math.NET.Numerics   AngouriMath
    (existing lib)      (existing lib)      (existing lib)
            │                 │                 │
    Numerical (fast)    Numerical (fast)    Symbolic (exact)
```

### Extension to IScalarProcessor<T>

```csharp
public interface IScalarProcessor<T>
{
    // ... existing 60+ methods ...

    /// <summary>
    /// Numerical operations for this scalar type.
    /// Returns null if numerical operations not supported.
    /// </summary>
    INumericalOperations<T>? NumericalOperations { get; }
}
```

### INumericalOperations<T> Interface

```csharp
namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

/// <summary>
/// Provides numerical operations for scalar types.
/// Different implementations for numeric (double/float) vs symbolic types.
/// </summary>
public interface INumericalOperations<T>
{
    /// <summary>
    /// Reference to scalar processor
    /// </summary>
    IScalarProcessor<T> ScalarProcessor { get; }

    /// <summary>
    /// Compute first derivative of function at given point.
    ///
    /// For double/float: Uses Math.NET numerical differentiation
    ///   - Method: Central finite differences
    ///   - Accuracy: ~1e-8 (double), ~1e-4 (float)
    ///   - Performance: Fast (~100ns per call)
    ///
    /// For symbolic (Entity): Uses AngouriMath symbolic differentiation
    ///   - Method: Symbolic calculus rules
    ///   - Accuracy: Exact (no approximation)
    ///   - Performance: Slower (~1ms per call, but exact)
    /// </summary>
    /// <param name="function">Function to differentiate</param>
    /// <param name="point">Point at which to evaluate derivative</param>
    /// <returns>Derivative value at point</returns>
    Scalar<T> Differentiate(
        Func<Scalar<T>, Scalar<T>> function,
        Scalar<T> point
    );

    /// <summary>
    /// Compute second derivative of function at given point.
    /// Same backend selection as Differentiate().
    /// </summary>
    Scalar<T> Differentiate2(
        Func<Scalar<T>, Scalar<T>> function,
        Scalar<T> point
    );

    // --- OPTIONAL: For future Phase 3+ extensions ---

    /// <summary>
    /// Numerical integration over interval [a, b].
    /// Returns null if not implemented for this type.
    /// </summary>
    Scalar<T>? Integrate(
        Func<Scalar<T>, Scalar<T>> function,
        Scalar<T> a,
        Scalar<T> b
    );

    /// <summary>
    /// Find root of equation function(x) = 0.
    /// Returns null if not implemented for this type.
    /// </summary>
    Scalar<T>? FindRoot(
        Func<Scalar<T>, Scalar<T>> function,
        Scalar<T> initialGuess,
        Scalar<T>? tolerance = null
    );
}
```

---

## 🔧 Implementation Details

### 1. Math.NET Backend for double

**File:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Float64/MathNetNumericalOperationsOfFloat64.cs`

```csharp
using MathNet.Numerics;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;

/// <summary>
/// Math.NET-based numerical operations for double precision.
/// Uses central finite differences for derivatives.
/// </summary>
public sealed class MathNetNumericalOperationsOfFloat64 : INumericalOperations<double>
{
    public static MathNetNumericalOperationsOfFloat64 Instance { get; }
        = new(ScalarProcessorOfFloat64.Instance);

    public IScalarProcessor<double> ScalarProcessor { get; }

    private MathNetNumericalOperationsOfFloat64(IScalarProcessor<double> processor)
    {
        ScalarProcessor = processor ?? throw new ArgumentNullException(nameof(processor));
    }

    public Scalar<double> Differentiate(
        Func<Scalar<double>, Scalar<double>> function,
        Scalar<double> point)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(point);

        // Convert Scalar<double> function to raw double function for Math.NET
        double RawFunction(double x)
        {
            var scalarX = ScalarProcessor.Scalar(x);
            var result = function(scalarX);
            return result.ScalarValue;
        }

        // Use Math.NET central finite differences
        var derivativeValue = Differentiate.FirstDerivative(
            RawFunction,
            point.ScalarValue
        );

        return ScalarProcessor.Scalar(derivativeValue);
    }

    public Scalar<double> Differentiate2(
        Func<Scalar<double>, Scalar<double>> function,
        Scalar<double> point)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(point);

        double RawFunction(double x)
        {
            var scalarX = ScalarProcessor.Scalar(x);
            var result = function(scalarX);
            return result.ScalarValue;
        }

        var derivativeValue = Differentiate.SecondDerivative(
            RawFunction,
            point.ScalarValue
        );

        return ScalarProcessor.Scalar(derivativeValue);
    }

    public Scalar<double>? Integrate(
        Func<Scalar<double>, Scalar<double>> function,
        Scalar<double> a,
        Scalar<double> b)
    {
        // TODO Phase 3: Implement using Math.NET.Numerics.Integration
        return null;
    }

    public Scalar<double>? FindRoot(
        Func<Scalar<double>, Scalar<double>> function,
        Scalar<double> initialGuess,
        Scalar<double>? tolerance = null)
    {
        // TODO Phase 3: Implement using Math.NET.Numerics.RootFinding
        return null;
    }
}
```

**Integration with ScalarProcessorOfFloat64:**

```csharp
// File: ScalarProcessorOfFloat64.cs

public sealed class ScalarProcessorOfFloat64 : IScalarProcessor<double>
{
    // ... existing code ...

    public INumericalOperations<double> NumericalOperations
        => MathNetNumericalOperationsOfFloat64.Instance;
}
```

### 2. Math.NET Backend for float

**File:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Float32/MathNetNumericalOperationsOfFloat32.cs`

```csharp
namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32;

/// <summary>
/// Math.NET-based numerical operations for single precision.
/// Internally uses double precision for accuracy, then converts back to float.
/// </summary>
public sealed class MathNetNumericalOperationsOfFloat32 : INumericalOperations<float>
{
    public static MathNetNumericalOperationsOfFloat32 Instance { get; }
        = new(ScalarProcessorOfFloat32.Instance);

    public IScalarProcessor<float> ScalarProcessor { get; }

    private MathNetNumericalOperationsOfFloat32(IScalarProcessor<float> processor)
    {
        ScalarProcessor = processor ?? throw new ArgumentNullException(nameof(processor));
    }

    public Scalar<float> Differentiate(
        Func<Scalar<float>, Scalar<float>> function,
        Scalar<float> point)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(point);

        // Convert to double for better accuracy, then back to float
        double RawFunction(double x)
        {
            var scalarX = ScalarProcessor.Scalar((float)x);
            var result = function(scalarX);
            return result.ScalarValue;
        }

        var derivativeValue = (float)Differentiate.FirstDerivative(
            RawFunction,
            point.ScalarValue
        );

        return ScalarProcessor.Scalar(derivativeValue);
    }

    public Scalar<float> Differentiate2(
        Func<Scalar<float>, Scalar<float>> function,
        Scalar<float> point)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(point);

        double RawFunction(double x)
        {
            var scalarX = ScalarProcessor.Scalar((float)x);
            var result = function(scalarX);
            return result.ScalarValue;
        }

        var derivativeValue = (float)Differentiate.SecondDerivative(
            RawFunction,
            point.ScalarValue
        );

        return ScalarProcessor.Scalar(derivativeValue);
    }

    public Scalar<float>? Integrate(
        Func<Scalar<float>, Scalar<float>> function,
        Scalar<float> a,
        Scalar<float> b)
    {
        return null; // TODO Phase 3
    }

    public Scalar<float>? FindRoot(
        Func<Scalar<float>, Scalar<float>> function,
        Scalar<float> initialGuess,
        Scalar<float>? tolerance = null)
    {
        return null; // TODO Phase 3
    }
}
```

### 3. AngouriMath Backend for Symbolic

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Context/Processors/AngouriMathNumericalOperations.cs`

```csharp
using AngouriMath;

namespace GeometricAlgebraFulcrumLib.MetaProgramming.Context.Processors;

/// <summary>
/// AngouriMath-based operations for symbolic computation.
/// Provides EXACT symbolic differentiation/integration (not numerical approximation).
/// </summary>
public sealed class AngouriMathNumericalOperations : INumericalOperations<Entity>
{
    public IScalarProcessor<Entity> ScalarProcessor { get; }

    private readonly ScalarProcessorOfAngouriMathEntity _processor;

    internal AngouriMathNumericalOperations(ScalarProcessorOfAngouriMathEntity processor)
    {
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        ScalarProcessor = processor;
    }

    public Scalar<Entity> Differentiate(
        Func<Scalar<Entity>, Scalar<Entity>> function,
        Scalar<Entity> point)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(point);

        // Strategy: Create symbolic variable, evaluate function symbolically,
        // differentiate symbolically, then substitute point

        var variable = Entity.Variable("__diff_var__");
        var variableScalar = ScalarProcessor.Scalar(variable);

        // Evaluate function symbolically
        var functionResult = function(variableScalar);
        var expr = functionResult.ScalarValue;

        // Symbolic differentiation (EXACT!)
        var derivative = expr.Differentiate(variable);

        // Substitute the evaluation point
        var result = derivative.Substitute(variable, point.ScalarValue);

        // Simplify the result
        result = result.Simplify();

        return ScalarProcessor.Scalar(result);
    }

    public Scalar<Entity> Differentiate2(
        Func<Scalar<Entity>, Scalar<Entity>> function,
        Scalar<Entity> point)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(point);

        var variable = Entity.Variable("__diff_var__");
        var variableScalar = ScalarProcessor.Scalar(variable);

        var functionResult = function(variableScalar);
        var expr = functionResult.ScalarValue;

        // Second derivative = differentiate twice
        var derivative2 = expr
            .Differentiate(variable)
            .Differentiate(variable);

        var result = derivative2
            .Substitute(variable, point.ScalarValue)
            .Simplify();

        return ScalarProcessor.Scalar(result);
    }

    public Scalar<Entity>? Integrate(
        Func<Scalar<Entity>, Scalar<Entity>> function,
        Scalar<Entity> a,
        Scalar<Entity> b)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        // AngouriMath supports symbolic integration!
        var variable = Entity.Variable("__int_var__");
        var variableScalar = ScalarProcessor.Scalar(variable);

        var functionResult = function(variableScalar);
        var expr = functionResult.ScalarValue;

        // Indefinite integral
        var integral = expr.Integrate(variable);

        // Definite integral: F(b) - F(a)
        var resultB = integral.Substitute(variable, b.ScalarValue);
        var resultA = integral.Substitute(variable, a.ScalarValue);
        var result = (resultB - resultA).Simplify();

        return ScalarProcessor.Scalar(result);
    }

    public Scalar<Entity>? FindRoot(
        Func<Scalar<Entity>, Scalar<Entity>> function,
        Scalar<Entity> initialGuess,
        Scalar<Entity>? tolerance = null)
    {
        // AngouriMath supports symbolic equation solving
        // For now return null - complex implementation depending on use case
        // TODO Phase 3: Implement symbolic root finding
        return null;
    }
}
```

**Integration with ScalarProcessorOfAngouriMathEntity:**

```csharp
// File: ScalarProcessorOfAngouriMathEntity.cs

public sealed class ScalarProcessorOfAngouriMathEntity : IScalarProcessor<Entity>
{
    // ... existing code ...

    private INumericalOperations<Entity>? _numericalOperations;

    public INumericalOperations<Entity> NumericalOperations
        => _numericalOperations ??= new AngouriMathNumericalOperations(this);
}
```

---

## 🎯 Usage Examples

### Example 1: Generic Trajectory with Numerical Differentiation

**CatmullRomSplinePath2D<T>.GetDerivative1Value():**

```csharp
public override LinVector2D<T> GetDerivative1Value(T t)
{
    var processor = ScalarProcessor;
    var tScalar = processor.Scalar(t);

    // Check if outside main range (edge case)
    if (processor.IsLessThanOrEqualTo(t, TimeRange.MinValue) ||
        processor.IsGreaterThanOrEqualTo(t, TimeRange.MaxValue))
    {
        // Use numerical differentiation as fallback
        var ops = processor.NumericalOperations;

        if (ops == null)
            throw new NotSupportedException(
                $"Numerical differentiation not supported for scalar type {typeof(T).Name}"
            );

        // Differentiate component-wise
        var dx = ops.Differentiate(
            tScalar => GetPoint(tScalar.ScalarValue).X,
            tScalar
        );

        var dy = ops.Differentiate(
            tScalar => GetPoint(tScalar.ScalarValue).Y,
            tScalar
        );

        return LinVector2D<T>.Create(dx.ScalarValue, dy.ScalarValue);
    }

    // Use analytical formula for main range (preferred!)
    var (index1, index2) = GetKnotIndexContaining(t);
    var tQuad = _knotList.GetItemQuad(index1 - 1);
    var xQuad = _pointList.GetTupleXQuad(index1 - 1);
    var yQuad = _pointList.GetTupleYQuad(index1 - 1);

    var x = CatmullRomUtils.GetCatmullRomDerivativeValue(tScalar, tQuad, xQuad);
    var y = CatmullRomUtils.GetCatmullRomDerivativeValue(tScalar, tQuad, yQuad);

    return LinVector2D<T>.Create(x, y);
}
```

### Example 2: Performance for double/float

```csharp
// For Generic<double>:
var processor = ScalarProcessorOfFloat64.Instance;
var points = new[] { /* 10 points */ };
var curve = new CatmullRomSplinePath2D<double>(processor, points);

// At edge case (t = 0), uses numerical differentiation
var derivative = curve.GetDerivative1Value(0.0);
// Internally: MathNet.Numerics.Differentiate.FirstDerivative()
// Performance: ~100ns per call
// Accuracy: ~1e-8

// At main range (0 < t < 1), uses analytical formula
var derivative2 = curve.GetDerivative1Value(0.5);
// Internally: CatmullRom analytical derivative
// Performance: ~50ns per call
// Accuracy: Machine epsilon (exact)
```

### Example 3: Symbolic Differentiation

```csharp
// For Generic<Entity> (symbolic):
var processor = ScalarProcessorOfAngouriMathEntity.Instance;
var t = Entity.Variable("t");

// Create symbolic points
var p0 = LinVector2D.Create(processor.Scalar(0), processor.Scalar(0));
var p1 = LinVector2D.Create(processor.Scalar("a"), processor.Scalar("b"));
var p2 = LinVector2D.Create(processor.Scalar("c"), processor.Scalar("d"));
var points = new[] { p0, p1, p2 };

var curve = new CatmullRomSplinePath2D<Entity>(processor, points);

// Derivative is EXACT symbolic expression!
var derivative = curve.GetDerivative1Value(t.ToFloat64().ToRational().ToEntity());
// Result: Symbolic expression in terms of t, a, b, c, d
// Example: dx/dt = 3*a*t^2 - 2*b*t + c
// NO numerical approximation!
```

---

## ⚠️ Potential Problems & Solutions

### Problem 1: Performance Overhead for double/float

**Risk:** Interface calls + function wrapping could impact performance.

**Solution:** Type-specific fast-paths (PROVEN in Phase 1):

```csharp
// In performance-critical code:
public LinVector2D<T> GetDerivative1Value(T t)
{
    // Fast-path for double (JIT devirtualization)
    if (typeof(T) == typeof(double))
    {
        var tDouble = (double)(object)t;
        var dx = Differentiate.FirstDerivative(
            x => GetPointXDouble(x),
            tDouble
        );
        var dy = Differentiate.FirstDerivative(
            x => GetPointYDouble(x),
            tDouble
        );
        return (LinVector2D<T>)(object)LinFloat64Vector2D.Create(dx, dy);
    }

    // Fast-path for float
    if (typeof(T) == typeof(float))
    {
        var tFloat = (float)(object)t;
        var dx = (float)Differentiate.FirstDerivative(
            x => GetPointXDouble(x),
            tFloat
        );
        var dy = (float)Differentiate.FirstDerivative(
            x => GetPointYDouble(x),
            tFloat
        );
        return (LinVector2D<T>)(object)LinFloat32Vector2D.Create(dx, dy);
    }

    // Generic fallback
    var processor = ScalarProcessor;
    var ops = processor.NumericalOperations;
    // ...
}
```

**Evidence:** Phase 1 optimizations showed 1.27-2.31x SPEEDUPS with this pattern!

### Problem 2: Not All Scalar Types Support Numerical Operations

**Risk:** What if `processor.NumericalOperations` returns null?

**Solution:** Fail-fast with clear error message:

```csharp
var ops = processor.NumericalOperations;

if (ops == null)
    throw new NotSupportedException(
        $"Numerical differentiation not supported for scalar type {typeof(T).Name}. " +
        $"Analytical derivatives must be provided for this type."
    );
```

**Future Enhancement (Phase 3+):** Implement finite-difference fallback for all types:

```csharp
public interface IScalarProcessor<T>
{
    // Always available (finite-difference approximation)
    Scalar<T> DifferentiateFiniteDifference(Func<T, T> function, T point, T? stepSize = null);
}
```

### Problem 3: AngouriMath Symbolic Evaluation Performance

**Risk:** Symbolic differentiation might be slow.

**Analysis:**
- Symbolic computation is **inherently** slower than numerical (~1ms vs ~100ns)
- BUT: Symbolic gives EXACT results, not approximations
- Used rarely in performance-critical paths (mostly in meta-programming/code generation)

**Solution:** ✅ No problem - this is expected and acceptable behavior.

### Problem 4: Complex Function Signatures

**Risk:** `Func<Scalar<T>, Scalar<T>>` might be awkward to use.

**Solution:** Provide convenience overloads:

```csharp
public interface INumericalOperations<T>
{
    // High-level API (type-safe, preferred)
    Scalar<T> Differentiate(Func<Scalar<T>, Scalar<T>> function, Scalar<T> point);

    // Low-level API (convenience for simple cases)
    T DifferentiateValue(Func<T, T> function, T point)
    {
        return Differentiate(
            s => ScalarProcessor.Scalar(function(s.ScalarValue)),
            ScalarProcessor.Scalar(point)
        ).ScalarValue;
    }
}
```

---

## 📋 Implementation Roadmap

### Phase 1: Foundation (Week 1)

- [x] **Analysis Complete** (2025-11-05)
  - [x] Math.NET usage patterns verified
  - [x] AngouriMath integration verified
  - [x] IScalarProcessor<T> architecture reviewed
  - [x] Dual-backend strategy designed

- [ ] **Interface & Core Classes** (2-3 days)
  - [ ] Create `INumericalOperations<T>` interface
  - [ ] Implement `MathNetNumericalOperationsOfFloat64`
  - [ ] Implement `MathNetNumericalOperationsOfFloat32`
  - [ ] Implement `AngouriMathNumericalOperations`
  - [ ] Update `IScalarProcessor<T>` with `NumericalOperations` property
  - [ ] Update processor implementations (Float64, Float32, AngouriMathEntity)

- [ ] **Testing** (2 days)
  - [ ] Unit tests for `MathNetNumericalOperationsOfFloat64` (10+ tests)
  - [ ] Unit tests for `MathNetNumericalOperationsOfFloat32` (10+ tests)
  - [ ] Unit tests for `AngouriMathNumericalOperations` (10+ tests)
  - [ ] Equivalence tests: Generic<double> vs Float64 numerical diff
  - [ ] Symbolic accuracy tests: Verify exact symbolic derivatives

### Phase 2: Integration (Week 2)

- [ ] **Trajectory Classes Update** (3-4 days)
  - [ ] Update `CatmullRomSplinePath2D<T>` edge case handling
  - [ ] Update `CatmullRomSplinePath3D<T>` edge case handling
  - [ ] Update `ComputedPath2D<T>` (uses numerical diff by default)
  - [ ] Update `ComputedPath3D<T>` (uses numerical diff by default)
  - [ ] Update all other trajectory classes as needed (Module 6A/6B)

- [ ] **Integration Testing** (1-2 days)
  - [ ] Equivalence tests for updated trajectory classes
  - [ ] Performance regression tests (verify ≥95% of Float64)
  - [ ] Symbolic computation tests (verify exact results)
  - [ ] Edge case tests (t=0, t=1 boundaries)

- [ ] **Documentation** (1 day)
  - [ ] Update XML comments for all new classes
  - [ ] Update DEDUPLICATION_ROADMAP.md
  - [ ] Update PHASE_3_DEDUPLICATION_TASKS.md
  - [ ] Document usage examples

### Phase 3: Optional Extensions (Future)

- [ ] **Integration Support**
  - [ ] Implement `Integrate()` for double (Math.NET)
  - [ ] Implement `Integrate()` for float (Math.NET)
  - [ ] Implement `Integrate()` for Entity (AngouriMath)
  - [ ] Tests for numerical integration

- [ ] **Root Finding Support**
  - [ ] Implement `FindRoot()` for double (Math.NET)
  - [ ] Implement `FindRoot()` for float (Math.NET)
  - [ ] Implement `FindRoot()` for Entity (AngouriMath)
  - [ ] Tests for root finding

- [ ] **Additional Scalar Types**
  - [ ] Implement for Complex<T> (if needed)
  - [ ] Implement for Rational (if needed)
  - [ ] Finite-difference fallback for all types

---

## 📊 Success Metrics

| Metric | Target | Verification Method |
|--------|--------|---------------------|
| **API Parity** | 100% | All Float64 trajectory features work in Generic<double> |
| **Performance (double)** | ≥95% | Benchmark: CatmullRomSpline edge case differentiation |
| **Performance (float)** | ≥95% | Benchmark: CatmullRomSpline edge case differentiation |
| **Symbolic Accuracy** | 100% exact | Symbolic tests: Compare symbolic result vs analytical |
| **Test Coverage** | 100% | All INumericalOperations implementations have 10+ tests |
| **Zero Regressions** | 100% | All existing 1153 tests still pass |
| **Integration** | 100% | All Module 6A/6B classes use INumericalOperations<T> |

---

## 🎯 Conclusion

### Key Insights

1. **Minimal Scope**: Only 2 operations needed (Differentiate, Differentiate2)
2. **Proven Architecture**: IScalarProcessor<T> is perfectly designed for extension
3. **Proven Performance**: Phase 1 showed type-specific fast-paths work excellently
4. **Better than Float64**: Symbolic types get EXACT derivatives, not approximations
5. **Existing Infrastructure**: Math.NET and AngouriMath already integrated

### Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Performance regression | Low | High | Type-specific fast-paths (proven) |
| API complexity | Low | Medium | Clear documentation + examples |
| Symbolic evaluation bugs | Medium | Low | Comprehensive test coverage |
| Integration issues | Low | Medium | Incremental rollout per class |

**Overall Risk:** ✅ LOW - This is a well-understood, proven pattern.

### Recommendation

**GO FOR IMPLEMENTATION** - All prerequisites are met:
- ✅ Architecture is sound
- ✅ Performance pattern is proven
- ✅ Libraries are integrated
- ✅ Scope is minimal
- ✅ Benefits are clear

**Next Step:** Begin Phase 1 implementation (Week 1) - Create interfaces and core implementations.

---

**Document Status:** ✅ Complete and ready for implementation
**Created by:** Claude Code Analysis Session (2025-11-05)
**Approved for:** Phase 3 Module 6A/6B Prerequisites
