# Architecture & Design Patterns

This document describes the core architectural decisions, design patterns, and best practices for the GA-FUL Generic-First implementation.

---

## 🎯 Core Principles

### 1. Generic-First Strategy

**Principle:** All new code uses `Generic<T>` with `IScalarProcessor<T>`, Float64-specialized code is deprecated.

**Why:**
- **Eliminates duplication:** ~78,500 LOC of duplicate code removed
- **Better performance:** Generic<T> is **1.39-2.31x FASTER** than Float64!
- **Flexibility:** Works with double, float, symbolic, rational, exact types
- **Maintainability:** Single implementation for all scalar types

**Pattern:**
```csharp
// ✅ CORRECT: Generic-First
public class LinVector3D<T>
{
    private readonly IScalarProcessor<T> _scalarProcessor;
    public Scalar<T> X { get; }
    public Scalar<T> Y { get; }
    public Scalar<T> Z { get; }

    public LinVector3D<T> Add(LinVector3D<T> other)
    {
        return new LinVector3D<T>(
            _scalarProcessor,
            _scalarProcessor.Add(X, other.X),
            _scalarProcessor.Add(Y, other.Y),
            _scalarProcessor.Add(Z, other.Z)
        );
    }
}

// ❌ DEPRECATED: Float64-specialized
public class LinFloat64Vector3D
{
    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public LinFloat64Vector3D Add(LinFloat64Vector3D other)
    {
        return new LinFloat64Vector3D(X + other.X, Y + other.Y, Z + other.Z);
    }
}
```

---

### 2. Data-Oriented Programming (DOP)

**Principle:** Immutable data structures, pure functions, processor pattern.

**Key Concepts:**
- **Immutability:** All data structures are immutable (no setters)
- **Processors:** Stateless processors contain all operations
- **Composers:** Mutable builders for constructing immutable objects
- **Value semantics:** Pass by value (structs for scalars)

**Example:**
```csharp
// Immutable data structure
public sealed class LinVector3D<T>
{
    public Scalar<T> X { get; } // Read-only
    public Scalar<T> Y { get; }
    public Scalar<T> Z { get; }

    // Constructor only place where values are set
    public LinVector3D(IScalarProcessor<T> processor, Scalar<T> x, Scalar<T> y, Scalar<T> z)
    {
        ScalarProcessor = processor;
        X = x;
        Y = y;
        Z = z;
    }

    // Operations return NEW objects
    public LinVector3D<T> Add(LinVector3D<T> other) => new LinVector3D<T>(...);
}
```

---

### 3. Processor Pattern

**Principle:** `IScalarProcessor<T>` is the heart of the system - factory, metric holder, and computation engine combined.

**Responsibilities:**
- Create scalar values from numbers/strings
- Perform arithmetic operations (Add, Subtract, Times, Divide)
- Provide transcendental functions (Sin, Cos, Exp, Log, Sqrt, Power)
- Provide constants (Zero, One, Pi, E, etc.)
- Optional: Numerical operations (Differentiate, Integrate)

**Pattern:**
```csharp
public interface IScalarProcessor<T>
{
    // Factory methods
    Scalar<T> Scalar(T value);
    Scalar<T> ScalarFromNumber(int value);
    Scalar<T> ScalarFromText(string text);

    // Arithmetic
    Scalar<T> Add(Scalar<T> a, Scalar<T> b);
    Scalar<T> Subtract(Scalar<T> a, Scalar<T> b);
    Scalar<T> Times(Scalar<T> a, Scalar<T> b);
    Scalar<T> Divide(Scalar<T> a, Scalar<T> b);

    // Transcendental
    Scalar<T> Sin(Scalar<T> a);
    Scalar<T> Cos(Scalar<T> a);
    Scalar<T> Sqrt(Scalar<T> a);

    // Constants
    Scalar<T> ZeroValue { get; }
    Scalar<T> OneValue { get; }
    Scalar<T> Pi { get; }

    // Optional numerical operations
    INumericalOperations<T>? NumericalOperations { get; }
}
```

**Usage:**
```csharp
var processor = ScalarProcessorOfFloat64.Instance;
var x = processor.ScalarFromNumber(2.0);
var y = processor.ScalarFromNumber(3.0);
var result = processor.Add(x, y); // 5.0

// For Generic code
public class MyAlgorithm<T>
{
    private readonly IScalarProcessor<T> _processor;

    public MyAlgorithm(IScalarProcessor<T> processor)
    {
        _processor = processor;
    }

    public Scalar<T> Compute(Scalar<T> input)
    {
        var squared = _processor.Times(input, input);
        var doubled = _processor.Times(_processor.ScalarFromNumber(2), squared);
        return _processor.Add(doubled, _processor.OneValue);
    }
}
```

---

### 4. Composer Pattern

**Principle:** Use mutable builders to construct immutable objects efficiently.

**Why:**
- Immutable objects can't be modified after creation
- Building complex objects term-by-term would create many intermediate objects
- Composers provide efficient mutable construction, then create final immutable object

**Pattern:**
```csharp
// Building a vector the hard way (creates 3 intermediate objects)
var v1 = processor.Vector(1, 0, 0);
var v2 = processor.Vector(0, 2, 0);
var v3 = processor.Vector(0, 0, 3);
var result = v1.Add(v2).Add(v3); // Two intermediate objects!

// Building with composer (efficient)
var result = processor.CreateVectorComposer()
    .SetTerm(0, 1) // X component
    .SetTerm(1, 2) // Y component
    .SetTerm(2, 3) // Z component
    .GetVector();  // Single final object
```

**For Multivectors:**
```csharp
var mv = processor.CreateMultivectorComposer()
    .SetTerm(indexSet1, scalar1)      // Set coefficient
    .AddTerm(indexSet2, scalar2)      // Add to coefficient
    .AddGpTerms(mv1, mv2)             // Add geometric product terms
    .AddOpTerms(mv3, mv4)             // Add outer product terms
    .RemoveZeroTerms()                // Clean up
    .GetMultivector();                // Get immutable result
```

---

## ⚡ Performance Patterns

### Type-Specific Fast-Paths (CRITICAL!)

**Achievement:** This pattern made Generic<T> **1.39-2.31x FASTER** than Float64!

**Principle:** Use `typeof(T)` checks to bypass interface overhead for common types (double, float).

**Pattern:**
```csharp
public Scalar<T> VectorNorm<T>(LinVector3D<T> vector)
{
    var scalarProcessor = vector.ScalarProcessor;

    // Fast-path for double (JIT devirtualization)
    if (typeof(T) == typeof(double))
    {
        var x = (double)(object)vector.X.ScalarValue;
        var y = (double)(object)vector.Y.ScalarValue;
        var z = (double)(object)vector.Z.ScalarValue;
        var normSquared = x * x + y * y + z * z;
        var norm = Math.Sqrt(normSquared);
        return (Scalar<T>)(object)scalarProcessor.Scalar((T)(object)norm);
    }

    // Fast-path for float
    if (typeof(T) == typeof(float))
    {
        var x = (float)(object)vector.X.ScalarValue;
        var y = (float)(object)vector.Y.ScalarValue;
        var z = (float)(object)vector.Z.ScalarValue;
        var normSquared = x * x + y * y + z * z;
        var norm = MathF.Sqrt(normSquared);
        return (Scalar<T>)(object)scalarProcessor.Scalar((T)(object)norm);
    }

    // Generic fallback (for symbolic, rational, etc.)
    var xSquared = scalarProcessor.Times(vector.X, vector.X);
    var ySquared = scalarProcessor.Times(vector.Y, vector.Y);
    var zSquared = scalarProcessor.Times(vector.Z, vector.Z);
    var sumSquared = scalarProcessor.Add(
        scalarProcessor.Add(xSquared, ySquared),
        zSquared
    );
    return scalarProcessor.Sqrt(sumSquared);
}
```

**Why it works:**
1. **JIT Devirtualization:** `typeof(T) == typeof(double)` is compiled to a constant check
2. **Direct CPU operations:** `x * x` compiles to a single CPU instruction
3. **No interface calls:** Bypasses `IScalarProcessor<T>.Times()` overhead
4. **Same API:** External code doesn't know about fast-paths

**Where to use:**
- ✅ Performance-critical inner loops
- ✅ Norm/Distance computations
- ✅ Scalar products
- ✅ Any operation called millions of times

**Where NOT to use:**
- ❌ Rarely-called code (not worth complexity)
- ❌ When code clarity is more important
- ❌ When you need behavior to be identical across all types

---

### Local Accumulator Pattern

**Achievement:** Reduced overhead from 33% to 14% in Scalar Product (Sp) optimization.

**Principle:** Accumulate results in local variables/dictionaries before creating final object.

**Pattern:**
```csharp
// ❌ SLOW: Creates intermediate objects
public Scalar<T> DotProduct<T>(IEnumerable<(Scalar<T>, Scalar<T>)> terms)
{
    var result = scalarProcessor.ZeroValue;
    foreach (var (a, b) in terms)
    {
        var product = scalarProcessor.Times(a, b);
        result = scalarProcessor.Add(result, product); // New object each iteration!
    }
    return result;
}

// ✅ FAST: Local accumulator (for double/float)
public Scalar<T> DotProduct<T>(IEnumerable<(Scalar<T>, Scalar<T>)> terms)
{
    if (typeof(T) == typeof(double))
    {
        var accumulator = 0.0; // Local variable!
        foreach (var (a, b) in terms)
        {
            var aVal = (double)(object)a.ScalarValue;
            var bVal = (double)(object)b.ScalarValue;
            accumulator += aVal * bVal; // Direct CPU operation
        }
        return (Scalar<T>)(object)scalarProcessor.Scalar((T)(object)accumulator);
    }

    // Generic fallback...
}
```

**For Multivectors (Dictionary accumulator):**
```csharp
public XGaMultivector<T> AddScalarProduct<T>(
    XGaKVector<T> kVector1,
    XGaKVector<T> kVector2)
{
    if (typeof(T) == typeof(double))
    {
        var accumulator = new Dictionary<IndexSet, double>(); // Local dictionary!

        foreach (var term1 in kVector1.Terms)
        {
            foreach (var term2 in kVector2.Terms)
            {
                var id = term1.Id.Gp(term2.Id);
                var value = (double)(object)term1.Scalar.ScalarValue *
                           (double)(object)term2.Scalar.ScalarValue;

                if (accumulator.ContainsKey(id))
                    accumulator[id] += value;
                else
                    accumulator[id] = value;
            }
        }

        // Convert accumulator to multivector once
        return CreateMultivectorFromDict(accumulator);
    }

    // Generic fallback...
}
```

---

### Lambda-Free Iteration

**Achievement:** 10% performance improvement by eliminating LINQ closures.

**Principle:** Avoid LINQ methods in performance-critical code; use direct iteration instead.

**Pattern:**
```csharp
// ❌ SLOW: LINQ creates closures
var sum = terms.Sum(term => term.Scalar.ScalarValue);

// ✅ FAST: Direct iteration
var sum = 0.0;
foreach (var term in terms)
{
    sum += term.Scalar.ScalarValue;
}

// ❌ SLOW: LINQ with complex lambda
var filteredTerms = terms
    .Where(term => Math.Abs(term.Scalar.ScalarValue) > tolerance)
    .Select(term => term.Scalar)
    .ToList();

// ✅ FAST: Manual filtering
var filteredTerms = new List<Scalar<T>>();
foreach (var term in terms)
{
    if (Math.Abs((double)(object)term.Scalar.ScalarValue) > tolerance)
    {
        filteredTerms.Add(term.Scalar);
    }
}
```

---

## 🧪 Testing Strategies

### Equivalence Test Pattern (CRITICAL!)

**Principle:** Every Generic<T> class must have equivalence tests proving Generic<double> ≡ Float64.

**Why:**
- **API Verification:** Proves Generic<T> behaves identically to Float64
- **Regression Prevention:** Catches any behavioral differences
- **Confidence:** 100% pass rate = production-ready

**Pattern:**
```csharp
[TestFixture]
public class LinVector3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> _scalarProcessor;

    [SetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void VectorAddition_ShouldProduceIdenticalResults()
    {
        // Arrange
        var v1F64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var v2F64 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);

        var v1Gen = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(2.0),
            _scalarProcessor.ScalarFromNumber(3.0)
        );
        var v2Gen = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(4.0),
            _scalarProcessor.ScalarFromNumber(5.0),
            _scalarProcessor.ScalarFromNumber(6.0)
        );

        // Act
        var resultF64 = v1F64.Add(v2F64);
        var resultGen = v1Gen.Add(v2Gen);

        // Assert
        Assert.That(resultGen.X.ScalarValue, Is.EqualTo(resultF64.X).Within(Tolerance));
        Assert.That(resultGen.Y.ScalarValue, Is.EqualTo(resultF64.Y).Within(Tolerance));
        Assert.That(resultGen.Z.ScalarValue, Is.EqualTo(resultF64.Z).Within(Tolerance));
    }
}
```

**Requirements:**
- **10+ tests minimum** per Generic<T> class
- **100% pass rate** required before commit
- **Tolerance-based comparisons** (never exact equality for floating-point)
- **All public methods** must be tested

---

### Tolerance-Based Floating-Point Comparisons

**Principle:** NEVER use exact equality for floating-point arithmetic.

**Why:**
- Different computation orders accumulate different rounding errors
- Typical differences: 1e-13 to 1e-15 for equivalent operations
- Different storage types (Uniform, Graded, Dense) compute differently

**Pattern:**
```csharp
// ❌ WRONG - Will fail due to rounding errors
Assert.That(result.IsZero);
Assert.AreEqual(expected, actual);

// ✅ CORRECT - Use tolerance
const double tolerance = 1e-12;
Assert.That(result.Norm().ScalarValue, Is.LessThan(tolerance));
Assert.That(actual, Is.EqualTo(expected).Within(tolerance));

// ✅ CORRECT - Use helper method
Assert.That(result.IsNearZero(tolerance));
```

**Recommended Tolerances:**
- **Double precision:** 1e-12 (conservative) to 1e-14 (aggressive)
- **Float precision:** 1e-5 (conservative) to 1e-6 (aggressive)
- **Symbolic types:** Exact equality (no rounding errors)

---

### Test Isolation (Random Number Generators)

**Principle:** Always isolate random generator state between tests.

**Why:**
- Test execution order can vary (parallel execution, test selection)
- Tests must be independent
- Shared RNG state causes flaky tests

**Pattern:**
```csharp
// ❌ WRONG - Shared state
public class MyTests
{
    private static Random _random = new Random(42);  // Shared!

    [Test]
    public void Test1()
    {
        var value = _random.Next();  // Depends on previous test order!
    }
}

// ✅ CORRECT - Isolated state
[Test]
public void TestWithIsolatedRandom()
{
    var random = new Random(42);  // Fresh seed per test
    var value = random.Next();    // Predictable!
}
```

---

## 🏗️ Infrastructure Design

### INumericalOperations<T> - Dual Backend

**Purpose:** Enable numerical differentiation/integration for Generic<T> classes.

**Architecture:**
```
INumericalOperations<T> (interface)
    ├── MathNetNumericalOperationsOfFloat64 (Math.NET backend)
    ├── MathNetNumericalOperationsOfFloat32 (Math.NET backend)
    └── AngouriMathNumericalOperations (AngouriMath backend)
```

**Why Dual Backend:**
- **Math.NET:** Fast numerical approximation (~100ns) for double/float
- **AngouriMath:** EXACT symbolic computation (~1ms) for symbolic types
- **Flexibility:** Each scalar type gets the best backend for its needs

**Usage:**
```csharp
public LinVector3D<T> GetDerivative(T t)
{
    var ops = _scalarProcessor.NumericalOperations;

    if (ops == null)
        throw new NotSupportedException(
            $"Numerical operations not supported for type {typeof(T).Name}");

    return LinVector3D<T>.Create(
        ops.Differentiate(GetX, t),
        ops.Differentiate(GetY, t),
        ops.Differentiate(GetZ, t)
    );
}
```

**Full Specification:** [docs/specifications/NUMERICAL_OPERATIONS.md](../../docs/specifications/NUMERICAL_OPERATIONS.md)

---

## 📐 Code Conventions

### Naming Conventions

**Classes:**
- **Generic:** `LinVector3D<T>`, `XGaMultivector<T>`, `ComplexNumber<T>`
- **Float64 (deprecated):** `LinFloat64Vector3D`, `XGaFloat64Multivector`
- **Processors:** `XGaProcessor<T>`, `ScalarProcessorOfFloat64`

**Methods:**
- **Operations:** Verb-based - `Add()`, `Subtract()`, `GetValue()`, `SetTerm()`
- **Queries:** Adjective/Boolean - `IsZero()`, `IsNearZero()`, `ContainsGrade()`
- **Factories:** Noun-based - `Create()`, `CreateVectorComposer()`, `Scalar()`

**Properties:**
- **PascalCase:** `ScalarProcessor`, `X`, `Y`, `Z`, `TimeRange`
- **Descriptive:** `MinValue`, `MaxValue`, `IsPeriodic`, `IsConstant`

### File Organization

**One class per file:**
```
GeometricAlgebraFulcrumLib/
├── Algebra/
│   ├── Scalars/
│   │   ├── Generic/
│   │   │   ├── IScalarProcessor.cs
│   │   │   ├── Scalar.cs
│   │   │   └── INumericalOperations.cs
│   │   ├── Float64/
│   │   │   ├── ScalarProcessorOfFloat64.cs
│   │   │   └── MathNetNumericalOperationsOfFloat64.cs
│   │   └── Float32/
│   │       └── ScalarProcessorOfFloat32.cs
│   └── LinearAlgebra/
│       └── Vectors/
│           ├── LinVector2D.cs
│           ├── LinVector3D.cs
│           └── LinVector4D.cs
└── Modeling/
    └── Trajectories/
        ├── Trajectory.cs
        ├── ITrajectory.cs
        ├── Vectors3D/
        │   └── Generic/
        │       ├── ParametricPath3D.cs
        │       └── ConstantPath3D.cs
        └── Vectors2D/
            └── Generic/
                └── ParametricPath2D.cs
```

---

## 🎓 Best Practices

### When to Use Type-Specific Fast-Paths

**✅ DO use when:**
- Operation is called millions of times (inner loops)
- Performance is critical (benchmarks show significant impact)
- Operation is purely computational (no side effects)

**❌ DON'T use when:**
- Code is rarely called
- Clarity/maintainability more important than speed
- Need identical behavior across all types (no approximation differences)

### When to Use Composers

**✅ DO use when:**
- Building objects with multiple terms
- Accumulating results from multiple operations
- Performance-critical construction

**❌ DON'T use when:**
- Creating simple objects with 1-3 values (use constructor/factory)
- One-time construction in non-critical code

### When to Write Equivalence Tests

**✅ ALWAYS for:**
- Every new Generic<T> class
- Every public method with non-trivial logic
- Any operation that replaces Float64 specialized code

**❌ Can skip for:**
- Private helper methods (test via public API)
- Trivial getters/setters
- Infrastructure classes (test via usage)

---

## 📚 References

### Related Documentation
- [STATUS.md](STATUS.md) - Current project status
- [ROADMAP.md](ROADMAP.md) - Future plans and milestones
- [README.md](README.md) - Project overview
- [docs/specifications/NUMERICAL_OPERATIONS.md](../../docs/specifications/NUMERICAL_OPERATIONS.md) - Detailed technical spec

### Key Files
- [DEVELOPMENT_GUIDE.md](../docs/guides/DEVELOPMENT_GUIDE.md) - Condensed architectural overview for agents
- [ISSUES_TO_FIX.md](../docs/status/ISSUES_TO_FIX.md) - Known issues tracking
- [PERFORMANCE_BENCHMARK_RECOMMENDATIONS.md](../docs/performance/PERFORMANCE_BENCHMARK_RECOMMENDATIONS.md) - Benchmarking guide

### External Resources
- GA-FUL Documentation: https://kopffarben.github.io/GeometricAlgebraFulcrumLib/
- Math.NET Numerics: https://numerics.mathdotnet.com/
- AngouriMath: https://am.angouri.org/

---

**Last Updated:** 2025-11-11
**Maintained By:** GA-FUL Development Team
**Version:** 1.0
