# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Required Tools

Always use:
- mcp__serena__initial_instructions
- **serena** für die Codesuche, arbeite immer mit Symbolen, also im namespace und nicht im Filespace, editiere den c# code immer auf symbolebene, 
- benutze immer zuerst **serena** 
- **context7** for up-to-date documentation on third party code
- **sequential thinking** for any decision making

## Project Overview

GA-FuL (Geometric Algebra Fulcrum Library) is a unified C# library for geometric algebra computations with generic scalar abstraction. It unifies complex numbers, quaternions, vectors, and matrices under a single mathematical framework.

**Key Design Philosophy:** Data-Oriented Programming (DOP) with immutable data structures, generic scalar processors, and the Composer pattern for building multivectors.

## Essential Commands

### Building
```bash
# Build entire solution from GeometricAlgebraFulcrumLib directory
dotnet build GeometricAlgebraFulcrumLib.sln

# Build specific configuration
dotnet build GeometricAlgebraFulcrumLib.sln --configuration Release --arch x64
```

### Testing
```bash
# Run all tests
dotnet test GeometricAlgebraFulcrumLib.sln

# Run specific test class
dotnet test GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj --filter "FullyQualifiedName~BasisBladeTests"

# Run tests with verbose output
dotnet test GeometricAlgebraFulcrumLib.sln --verbosity normal
```

### Running Applications
```bash
# Run a specific application project
dotnet run --project GeometricAlgebraFulcrumLib.Applications/GeometricAlgebraFulcrumLib.Applications.csproj
```

### Benchmarking
```bash
# Always use Release configuration for benchmarks
dotnet run --project GeometricAlgebraFulcrumLib.Benchmarks/GeometricAlgebraFulcrumLib.Benchmarks.csproj --configuration Release
```

## Architecture: The Big Picture

### 1. Four-Layer Architecture

```
Applications Layer → Modeling Layer → Algebra Layer
                          ↓
                  MetaProgramming Layer
                          ↓
                   Utilities Layer
```

- **Algebra Layer**: Core GA operations, processors, multivectors, scalar abstraction
- **Modeling Layer**: CGA (Conformal), PGA (Projective), VGa (Vector), HGa (Hyperbolic) geometric algebras
- **MetaProgramming Layer**: Symbolic computation, code generation, optimization (CSE, constant folding)
- **Utilities Layer**: Data structures, text processing, code manipulation

### 2. The Processor Pattern (Critical to Understand)

**XGaProcessor\<T\>** is the heart of GA-FuL. It's not just a calculator—it's a factory, metric holder, and computation engine combined.

```csharp
// Processors are metric-aware and type-generic
var processor = XGaFloat64Processor.Euclidean;  // Euclidean metric, Float64 scalars
// or
var processor = XGaFloat64Processor.Create(p: 3, q: 1);  // Minkowski spacetime (3,1)
// or
var processor = XGaProcessor<T>.CreateEuclidean(scalarProcessor);  // Any scalar type T
```

**Key Insight:** The processor knows:
- The **metric signature** (p positive, q negative, r zero basis vectors)
- The **scalar type** and operations (via `IScalarProcessor<T>`)
- How to create composers, multivectors, and zero/one constants
- How to compute products efficiently (Gp, Op, Sp, Lcp, Rcp, etc.)

**Static Singletons:** Use pre-configured processors when possible:
- `XGaFloat64Processor.Euclidean` - Standard Euclidean GA
- `XGaFloat64Processor.Projective` - Projective GA
- `XGaFloat64Processor.Conformal` - Conformal GA (5D for 3D geometry)

### 3. Generic Scalar Abstraction

The library works with **any scalar type** through `IScalarProcessor<T>`:

- **Float64/Float32**: Native floating-point (fast)
- **ERational**: Exact rational arithmetic
- **EDecimal**: Arbitrary precision decimals
- **MetaExpression**: Symbolic expressions for code generation

**Critical Pattern:** Never hardcode `double` operations. Always use `processor.ScalarProcessor.Add(a, b)` or the scalar's methods.

### 4. Multivector Storage Hierarchy

Three storage strategies, each optimized for different use cases:

**A. XGaUniformMultivector\<T\>** (Flat dictionary)
- Single `Dictionary<IndexSet, T>` of basis blade ID → scalar
- Best for: Sparse multivectors, meta-programming, symbolic computation
- Memory: Minimal for sparse data

**B. XGaGradedMultivector\<T\>** (Grade-organized)
- `Dictionary<int, XGaKVector<T>>` of grade → k-vector
- Best for: Operations needing grade separation (even/odd parts)
- Memory: Moderate, better cache locality

**C. RGaFloat64Multivector** (Dense arrays)
- Contiguous arrays for all 2^n basis blade coefficients
- Best for: Up to 64D, dense multivectors, maximum performance
- Memory: High (stores all coefficients including zeros)

**Type Specialization:**
- `XGaScalar<T>` (grade 0)
- `XGaVector<T>` (grade 1)
- `XGaBivector<T>` (grade 2)
- `XGaHigherKVector<T>` (grade ≥ 3)
- `XGaKVector<T>` (any single grade)

### 5. The Composer Pattern (Building Multivectors)

**Core Principle:** Multivectors are immutable. Use composers (mutable builders) to construct them.

```csharp
var result = processor
    .CreateMultivectorComposer()
    .SetTerm(indexSet1, scalar1)      // Set coefficient
    .AddTerm(indexSet2, scalar2)      // Add to coefficient
    .AddGpTerms(mv1, mv2)             // Add geometric product terms
    .AddOpTerms(mv3, mv4)             // Add outer product terms
    .GetMultivector();                 // Get immutable result
```

**Builder Types:**
- `CreateScalarComposer()` → `XGaScalar<T>`
- `CreateVectorComposer()` → `XGaVector<T>`
- `CreateBivectorComposer()` → `XGaBivector<T>`
- `CreateKVectorComposer(grade)` → `XGaKVector<T>`
- `CreateMultivectorComposer()` → Any multivector type

**Critical Pattern:** Operations like `mv1.Gp(mv2)` internally use composers to aggregate terms efficiently.

### 6. IndexSet: Basis Blade Encoding

`IndexSet` encodes which basis vectors are wedged together:
- Uses bitsets for \<64 dimensions (fast bit operations)
- Uses arrays for ≥64 dimensions (arbitrary size)
- Example: `e_1 ∧ e_3` → IndexSet {1, 3} → Bitset 0b1010

**Key Operations:**
- `id.ToUInt64()` - Get bitset representation
- `id.BasisBladeIdToGrade()` - Get grade (number of basis vectors)
- `id.BasisBladeIdToGradeIndex()` - Get (grade, index) pair

### 7. Geometric Algebra Products

All standard GA products are available on multivectors:

```csharp
var gp = mv1.Gp(mv2);   // Geometric product (full GA product)
var op = mv1.Op(mv2);   // Outer product (wedge)
var sp = mv1.Sp(mv2);   // Scalar product (inner product contraction)
var lcp = mv1.Lcp(mv2); // Left contraction
var rcp = mv1.Rcp(mv2); // Right contraction
```

**Implementation Detail:** Products use **Guided Binary Traversal (GBT)** for efficient sparse computation—skips zero terms automatically.

### 8. Code Generation Workflow (MetaProgramming Layer)

For generating optimized code from GA expressions:

```csharp
// 1. Create symbolic context
var context = new MetaContext();
var processor = XGaProcessor<IMetaExpressionAtomic>.CreateEuclidean(context);

// 2. Define symbolic parameters
var x = context.GetOrDefineParameterVariable("x");
var y = context.GetOrDefineParameterVariable("y");

// 3. Build symbolic GA expressions
var v1 = processor.CreateVector(x, y, 0);
var v2 = processor.CreateVector(1, 2, 3);
var result = v1.Gp(v2);  // Symbolic geometric product

// 4. Optimize and generate code
context.OptimizeContext();  // CSE, constant folding
var codeComposer = new GaFuLMetaContextCodeComposer(context, targetLanguage);
var generatedCode = codeComposer.Generate();
```

**Optimizations Applied:**
- Common Subexpression Elimination (CSE)
- Constant propagation
- Dead code elimination
- Algebraic simplification (via AngouriMath)

### 9. Conformal Geometric Algebra (CGA) Pattern

CGA is used extensively for 3D geometric modeling in the Modeling layer:

```csharp
var cga = CGaFloat64GeometricSpace5D.Instance;  // 5D CGA for 3D geometry

// Encode points as null vectors
var point = cga.Encode.IpnsRound.Point(x, y, z);

// Encode geometric objects
var sphere = cga.Encode.IpnsRound.Sphere(cx, cy, cz, radius);
var plane = cga.Encode.Opns.Plane(normal, distance);

// Perform intersections, transformations via GA operations
var intersection = sphere.Op(plane);
```

**IPNS vs OPNS:**
- IPNS (Inner Product Null Space): Objects as null vectors
- OPNS (Outer Product Null Space): Objects as blades

## Common Patterns and Conventions

### Pattern: Getting a Processor
```csharp
// Static singleton (preferred for Float64 + standard metrics)
var processor = XGaFloat64Processor.Euclidean;

// Custom metric
var processor = XGaFloat64Processor.Create(p: 3, q: 1);  // Minkowski (3,1)

// Generic scalar type
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
```

### Pattern: Creating Multivectors
```csharp
// From composer (preferred for multiple terms)
var v = processor.CreateVectorComposer()
    .SetVectorTerm(0, 2.0)  // 2*e_1
    .SetVectorTerm(1, 3.0)  // 3*e_2
    .GetVector();

// Direct factory methods (for simple cases)
var scalar = processor.Scalar(5.0);
var vector = processor.Vector(1, 2, 3);
var bivector = processor.Bivector(0, 1, 0.5);  // 0.5*(e_1∧e_2)
```

### Pattern: Extracting Multivector Components
```csharp
var scalar = multivector.GetScalarPart();
var vector = multivector.GetVectorPart();
var bivector = multivector.GetBivectorPart();
var kVector = multivector.GetKVectorPart(grade);

// Get specific term coefficient
var coeff = multivector.GetBasisBladeScalar(indexSet);
```

### Pattern: Testing Multivector Properties
```csharp
if (mv.IsZero()) { }
if (mv.IsScalar()) { }
if (mv.IsVector()) { }
if (mv.Grade == 2) { }  // Is bivector
if (mv.ContainsGrade(k)) { }
```

### Pattern: Working with Frames (Basis Sets)
```csharp
// Create orthonormal basis frame
var frame = processor.CreateBasisVectorFrame(dimensions);

// Create free frame (custom vectors)
var frame = processor.CreateFreeFrameOfBasis(vectorList);

// Extract frame vectors
var e1 = frame[0];  // First basis vector
var e2 = frame[1];  // Second basis vector
```

## Testing Conventions

- Test classes use `[TestFixture]` (NUnit)
- Test methods use `[Test]` attribute
- Both `Debug.Assert()` and `Assert.That()` are used together for debugging and testing
- Test naming: `Test<FeatureName>` or descriptive names
- Test classes: `<Feature>Tests.cs`

**Test Directory Structure:**
Located in: `GeometricAlgebraFulcrumLib.UnitTests/`
- `Algebra/` - Core algebra tests (133 tests, 100% passing)
- `LinearMaps/` - Rotors, reflectors, outermorphisms tests (121 tests, 100% passing)
- `AutoDiff/` - Automatic differentiation tests (69 tests, 100% passing)
- `Processing/` - Basis blade and multivector storage tests (19 tests)
- `Modeling/Geometry/CGa/` - Conformal GA tests (507 tests, 91% passing)
- `Modeling/Graphics/` - Graphics primitives, accelerators tests
- `Modeling/Signals/` - Signal processing tests
- `Utilities/` - BitManipulation, IndexSets, Combinations tests (295 tests, 99.7% passing)

**Test Statistics (as of 2025-10-17):**
- Total: 1153 tests
- Pass Rate: 97.92% (1129 passing)
- Failing: 0 (all critical bugs fixed!)
- Skipped: 24 (known library limitations or future work)

## Testing Best Practices & Learnings

Based on 1000+ tests and multiple bug fixes, these are critical learnings:

### 1. Floating-Point Comparisons
**NEVER use exact zero comparisons for floating-point arithmetic.**

```csharp
// ❌ WRONG - Will fail due to rounding errors
Assert.That(result.IsZero);

// ✅ CORRECT - Use tolerance-based comparison
const double tolerance = 1e-12;
Assert.That(result.IsNearZero(tolerance),
    $"Expected near-zero, got {result.Norm().ScalarValue}");
```

**Why:** Different multivector storage implementations (Uniform, Graded, Dense) compute operations in different orders, accumulating different rounding errors. Typical differences: 1e-13 to 1e-15.

**Where this matters:**
- MultivectorStoragesTests - comparing different storage types
- Product operations (Gp, Cp, Acp, etc.)
- Self-operations like Gp(Reverse())

### 2. Random Number Generator Isolation
**Always isolate random generator state between tests to avoid test-order dependencies.**

```csharp
// ❌ WRONG - Shared state causes flakiness
public class MyTests
{
    private static Random _random = new Random(42);  // Shared!

    [Test]
    public void Test1()
    {
        var value = _random.Next();  // Depends on previous test order!
    }
}

// ✅ CORRECT - Fresh state per test
[Test]
public void TestWithIsolatedRandom()
{
    var random = new Random(42);  // Fresh seed
    var value = random.Next();    // Predictable!
}
```

**Why:** Test execution order can vary (parallel execution, test selection, etc.). Tests must be independent.

**Fixed bugs:** BasisBladeTests.TestOddGradeInvolution failed when run after certain tests due to shared random state.

### 3. API Correctness - IndexSet Creation
**Critical bug found:** `BasisVectorIndexToId()` vs `BasisBivectorIndexToId()`

```csharp
// ❌ WRONG - GetBivector bug (CRITICAL!)
public XGaFloat64Bivector GetBivector(int index)
{
    return Processor.BivectorTerm(
        index.BasisVectorIndexToId(),  // Creates single index - WRONG!
        GetScalarValue()
    );
}

// ✅ CORRECT - Fixed version
public XGaFloat64Bivector GetBivector(int index)
{
    return Processor.BivectorTerm(
        index.BasisBivectorIndexToId(),  // Creates pair of indices
        GetScalarValue()
    );
}
```

**Impact:** This bug blocked 13 tests in MultivectorStoragesTests. A bivector requires TWO basis vector indices (e.g., e₁∧e₂), not one!

### 4. Product Implementation Patterns
**Commutator and Anti-Commutator products have simple, direct formulas.**

```csharp
// ✅ CORRECT Implementation Pattern
// Commutator Product: [A,B] = (AB - BA) / 2
public static XGaFloat64Multivector Cp(this XGaFloat64Multivector mv1, XGaFloat64Multivector mv2)
{
    return mv1.Gp(mv2)
        .Subtract(mv2.Gp(mv1))
        .Divide(2d);
}

// Anti-Commutator Product: {A,B} = (AB + BA) / 2
public static XGaFloat64Multivector Acp(this XGaFloat64Multivector mv1, XGaFloat64Multivector mv2)
{
    return mv1.Gp(mv2)
        .Add(mv2.Gp(mv1))
        .Divide(2d);
}
```

**Lesson:** These products are NOT inner/outer product variations. They're simple algebraic combinations of the geometric product.

### 5. Grade Involution Logic
**Sign patterns for grade involution were reversed.**

```csharp
// ❌ WRONG - Reversed logic
grade % 2 == 0 ? scalar : -scalar  // Even grades negated - WRONG!

// ✅ CORRECT - Odd grades get negated
grade % 2 == 0 ? scalar : scalar.Negative()  // Grade 1,3,5,... negated
```

**Mathematical rule:** Grade involution negates odd-grade terms (vectors, trivectors, etc.), not even-grade terms.

### 6. Known Edge Cases & Limitations

#### CreatePureRotor with Antiparallel Vectors
**Limitation:** `vector.CreatePureRotor(targetVector)` fails when vectors are nearly antiparallel (angle ≈ 180°).

```csharp
// ⚠️ KNOWN ISSUE - May throw DebugAssertException
var u1 = random.GetVector().DivideByENorm();
var u2 = random.GetVector().DivideByENorm();
var rotor = u1.CreatePureRotor(u2);  // Fails if u1 ≈ -u2

// ✅ WORKAROUND - Check for antiparallel case
var cosAngle = u1.ESp(u2);
if (Math.Abs(cosAngle + 1.0) < 1e-10)
{
    // Vectors are antiparallel - handle specially or skip
    return;
}
var rotor = u1.CreatePureRotor(u2);
```

**Why:** The library's `GetNormalVector()` method creates a circular dependency when finding a perpendicular vector to antiparallel vectors.

**Affected:** Rotation tests are flaky when using random vectors without checking angles.

### 7. BitManipulation Edge Cases
**Critical bug found:** `GetNthSetBitPosition` returned relative positions instead of absolute.

```csharp
// Example of the bug (NOW FIXED):
ulong bitPattern = 0b1010;  // Bits at positions 1 and 3
// WRONG result: GetNthSetBitPosition(bitPattern, 1) returned 2 (relative)
// CORRECT result: Should return 3 (absolute position)
```

**Lesson:** Always test with sparse bit patterns where position != index.

### 8. Testing Strategy Insights

**Test Coverage by Priority:**
1. **Core Algebra** (P0): Product operations, unary operations - 100% must pass
2. **LinearMaps** (P1): Rotors, reflectors - Critical for geometric transformations
3. **Storage Consistency** (P1): All storage types must produce identical results
4. **Edge Cases** (P2): Empty sets, antiparallel vectors, boundary conditions
5. **CGa/Modeling** (P3): Domain-specific - acceptable to have some known limitations

**Best Testing Patterns:**
- Use `Debug.Assert()` + `Assert.That()` together (catches bugs in development + CI)
- Test with multiple random seeds to catch flaky tests
- Compare results across different storage types for consistency
- Always test edge cases: dimension=0, grade=0, empty multivectors
- Use descriptive assertion messages with actual values

### 9. Documentation References
For comprehensive issue tracking and test coverage details:
- `ISSUES_TO_FIX.md` - All known issues with priority levels (0 failing tests!)
- `TODO_TEST_COVERAGE.md` - Complete test coverage plan and statistics
- `DOCUMENTATION_INDEX.md` - Central documentation registry
- `UnitTests/KNOWN_ISSUES.md` - Known library bugs and workarounds

## Key Files and Locations

**Core Algebra:**
- `GeometricAlgebra/Float64/Processors/XGaFloat64Processor.cs` - Main processor
- `GeometricAlgebra/Extended/Float64/Multivectors/XGaFloat64Multivector*.cs` - Multivector implementations
- `GeometricAlgebra/Extended/Generic/Multivectors/Composers/` - Composer pattern implementations

**Modeling:**
- `Modeling/Geometry/CGa/Float64/` - Conformal GA for 3D geometry
- `Modeling/Geometry/PGa/Float64/` - Projective GA

**MetaProgramming:**
- `MetaProgramming/Context/MetaContext.cs` - Symbolic computation context
- `MetaProgramming/Composers/` - Code generation composers

**Utilities:**
- `Utilities.Structures/IndexSets/` - IndexSet implementations
- `Utilities.Structures/Combinations/` - Combinatorial utilities

## Performance Considerations

**🚀 MAJOR BREAKTHROUGH (2025-10-27): Generic implementations are SIGNIFICANTLY FASTER than specialized code!**

After Phase 1 Quick Win Optimizations, `XGaProcessor<T>` with Generic<double> and Generic<float> **dramatically outperform** Float64 Specialized implementations across ALL abstraction levels:

**Low-Level (XGa Core) Performance:**
- **Vector Norm (3D)**: Generic<double> **1.74x faster** than Float64 (20.9ns vs 36.4ns)
- **Vector Norm² (3D)**: Generic<double> **2.31x faster** than Float64 (16.0ns vs 37.0ns)
- **Multivector Norm**: Generic<double> **1.39x faster** than Float64 (63.9ns vs 88.7ns)

**High-Level (CGa) Performance:**
- **Generic<double>**: **1.27x faster** than Float64 Specialized (27% speedup)
- **Generic<float>**: **1.24x faster** than Float64 Specialized (24% speedup)
- **Memory**: Generic uses **16-33% less memory**

See [PERFORMANCE_BENCHMARK_RECOMMENDATIONS.md](PERFORMANCE_BENCHMARK_RECOMMENDATIONS.md) and [GENERIC_VS_SPECIALIZED_PERFORMANCE.md](GENERIC_VS_SPECIALIZED_PERFORMANCE.md) for detailed analysis.

**Performance Best Practices:**

1. **✅ ALWAYS prefer generic implementations:** Generic<T> is now the FASTEST option at all levels
2. **✅ Use Float32 for graphics/gaming:** `XGaFloat32Processor` provides 1.24x speedup with 50% memory savings
3. **Reuse processors:** Processors cache zero/one/constants—don't recreate them
4. **Sparse storage:** Most real GA problems have sparse multivectors—use Uniform or Graded storage
5. **Composer pattern:** More efficient than creating multivectors term-by-term
6. **Guided Binary Traversal:** Automatically used for products—exploits sparsity
7. **Release builds for benchmarks:** Always benchmark in Release mode

**Why Generic is so much faster (Phase 1 Optimizations):**
- **Type-specific fast-paths**: `typeof(T)` checks bypass interface overhead for double/float
- **Lambda-free iteration**: Direct iteration eliminates closure overhead
- **JIT Devirtualization**: Generic interface calls compile to direct CPU instructions
- **Better cache locality**: Struct-based scalars with inline data
- **Modern patterns**: Span<T>, value semantics, aggressive inlining
- **Reduced allocations**: 16-33% fewer allocations → less GC pressure

**Recent Optimizations (2025-10-27):**
- **Scalar Product (Sp) Phase 1**: K-Vector Sp optimized with type-specific fast-paths + local accumulator pattern
  - Conformal Sp overhead reduced: 33% → 14% (19 percentage point improvement)
  - Implementation: `ScalarComposerOperations.cs` lines 186-342
- **Architectural Lesson from Phase 2B**: Attempted graded multivector Sp optimization caused 30% regression
  - Root cause: Bypassed efficient grade-based dispatcher architecture
  - Correctly reverted to preserve structural optimization
  - **Key Insight**: Respect architectural patterns - grade-based decomposition is a performance feature, not just organization
  - Details: See [SP_OPTIMIZATION_ANALYSIS.md](SP_OPTIMIZATION_ANALYSIS.md)
- **Left/Right Contraction (Lcp/Rcp) Phase 2D**: Successfully applied Phase 1 pattern to contraction products
  - Lcp overhead reduced: 9% → 5.2% (3.8 percentage point improvement)
  - Rcp overhead reduced: ~9% → 6.0% (bonus - same method optimized)
  - Both operations now in "Excellent" category (<10% overhead)
  - Implementation: `ProductGp.cs` lines 289-379 (`AddEuclideanProductTerms` with type-specific fast-paths)
  - **Success Factor**: Optimized LOW-LEVEL method without bypassing architectural patterns (learned from Phase 2B)
  - Details: See [LCP_OPTIMIZATION_ANALYSIS.md](LCP_OPTIMIZATION_ANALYSIS.md)

## Common Pitfalls

### General Programming Pitfalls
1. **Don't hardcode scalar operations:** Use `processor.ScalarProcessor.Add(a, b)` not `a + b`
2. **Don't mutate multivectors:** They're immutable—use composers instead
3. **Don't ignore metric:** `XGaFloat64Processor.Create(2, 0)` ≠ `XGaFloat64Processor.Euclidean` for products
4. **Don't mix processors:** Multivectors from different processors are incompatible
5. **Don't forget grade:** `CreateKVectorComposer(grade)` requires consistent grade—mixing grades needs `CreateMultivectorComposer()`

### Testing & Numerical Pitfalls
6. **Don't use exact zero comparisons:** Always use `IsNearZero(tolerance)` instead of `IsZero` for floating-point results
7. **Don't share random state:** Create fresh `Random` instances per test with explicit seeds
8. **Don't use wrong IndexSet APIs:** `BasisVectorIndexToId()` creates vectors; `BasisBivectorIndexToId()` creates bivectors
9. **Don't assume antiparallel vectors work:** Check angles before calling `CreatePureRotor()` with random vectors
10. **Don't test implementation details:** Test mathematical properties (e.g., `rotor * vector * rotor.Reverse()` preserves norm) not internal state

## External Documentation

Complete documentation: https://kopffarben.github.io/GeometricAlgebraFulcrumLib/ (English and German)