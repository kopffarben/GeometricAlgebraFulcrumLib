# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Required Tools

Always use:
- **serena** for semantic code retrieval and editing tools
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
- Both `Debug.Assert()` and `Assert.That()` are used together
- Test naming: `Test<FeatureName>` or descriptive names
- Test classes: `<Feature>Tests.cs`

Located in: `GeometricAlgebraFulcrumLib.UnitTests/`
- `Algebra/` - Core algebra tests
- `Geometry/` - Geometric operations tests
- `LinearMaps/` - Rotors, reflectors, outermorphisms tests
- `Processing/` - Basis blade and multivector storage tests

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

1. **Use specialized types when possible:** `XGaFloat64Processor` is faster than `XGaProcessor<double>`
2. **Reuse processors:** Processors cache zero/one/constants—don't recreate them
3. **Sparse storage:** Most real GA problems have sparse multivectors—use Uniform or Graded storage
4. **Composer pattern:** More efficient than creating multivectors term-by-term
5. **Guided Binary Traversal:** Automatically used for products—exploits sparsity
6. **Release builds for benchmarks:** Always benchmark in Release mode

## Common Pitfalls

1. **Don't hardcode scalar operations:** Use `processor.ScalarProcessor.Add(a, b)` not `a + b`
2. **Don't mutate multivectors:** They're immutable—use composers instead
3. **Don't ignore metric:** `XGaFloat64Processor.Create(2, 0)` ≠ `XGaFloat64Processor.Euclidean` for products
4. **Don't mix processors:** Multivectors from different processors are incompatible
5. **Don't forget grade:** `CreateKVectorComposer(grade)` requires consistent grade—mixing grades needs `CreateMultivectorComposer()`

## External Documentation

Complete documentation: https://kopffarben.github.io/GeometricAlgebraFulcrumLib/ (English and German)