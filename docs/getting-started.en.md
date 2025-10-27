---
layout: default
title: "Getting Started"
lang: en
---

# Getting Started with GA-FuL

**[🇩🇪 Deutsche Version](getting-started.de.md)**

---

## Table of Contents

1. [Installation](#installation)
2. [System Requirements](#system-requirements)
3. [**Performance Considerations**](#performance-considerations) 🚀
4. [First Example](#first-example)
5. [Core Concepts](#core-concepts)
6. [Common Workflows](#common-workflows)
7. [Next Steps](#next-steps)

---

## Installation

### Prerequisites

- **.NET 8.0 SDK** or higher
- **Visual Studio 2022** or **JetBrains Rider** or **VS Code** with C# Extension
- Optional: **Wolfram Mathematica** (for symbolic computations)
- Optional: **MATLAB** (for MATLAB integration)

### Clone the Repository

```bash
git clone https://github.com/ga-explorer/GeometricAlgebraFulcrumLib.git
cd GeometricAlgebraFulcrumLib
```

### Build

```bash
cd GeometricAlgebraFulcrumLib
dotnet build GeometricAlgebraFulcrumLib.sln
```

### As NuGet Package (if available)

```bash
dotnet add package GeometricAlgebraFulcrumLib.Algebra
dotnet add package GeometricAlgebraFulcrumLib.Modeling
dotnet add package GeometricAlgebraFulcrumLib.MetaProgramming
```

---

## System Requirements

### Minimum Requirements

| Component | Requirement |
|-----------|-------------|
| **OS** | Windows 10/11, Linux, macOS |
| **.NET Version** | .NET 8.0+ |
| **RAM** | 4 GB (minimum), 8 GB (recommended) |
| **IDE** | Visual Studio 2022, Rider, VS Code |

### Dependencies

The main NuGet packages are already included in the project files:

- `MathNet.Numerics`: Numerical computations
- `AngouriMath`: Symbolic mathematics
- `PeterO.Numbers`: Arbitrary-precision numbers
- `OxyPlot`: Plotting and visualization
- See `.csproj` files for more

---

## Performance Considerations

### 🚀 Important: Generic<T> is Faster than Float64 Specialized

GA-FuL offers **two parallel implementations** for floating-point computations:

| Implementation | Example | Performance | When to Use |
|----------------|---------|-------------|-------------|
| **Generic<double>** | `XGaProcessor<double>` | **1.24-2.31x FASTER** | Production, performance-critical code |
| **Float64 Specialized** | `XGaFloat64Processor` | Baseline | Learning, prototyping, simpler API |

### Benchmark Results (October 2025)

| Operation | Float64 Specialized | Generic<double> | Speedup |
|-----------|---------------------|-----------------|---------|
| **High-level CGA** | 1.00x | **1.27x** | **27% faster** |
| **Norm Operations** | 1.00x | **1.74-2.31x** | **74-131% faster** |
| **Scalar Product** | 33% overhead | **14% overhead** | 19pp improvement |
| **Left Contraction** | 9% overhead | **5.2% overhead** | 3.8pp improvement |
| **Right Contraction** | 9% overhead | **6.0% overhead** | 3pp improvement |

**Memory**: Generic<T> uses **16-33% less memory** than Float64 Specialized.

### Why is Generic<T> Faster?

1. **JIT Devirtualization**: `typeof(T) == typeof(double)` compiles to direct CPU instructions
2. **Type-specific fast-paths**: Optimized code paths for `double` and `float`
3. **Better cache locality**: Struct-based scalars with inline data
4. **Reduced allocations**: 16-33% fewer allocations → less GC pressure

### Migration Guide

**Float64 Specialized (Simple):**
```csharp
var processor = XGaFloat64Processor.Euclidean;
var vector = processor.CreateVector(1, 2, 3);
```

**Generic<double> (Faster):**
```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
var vector = processor.CreateVector(1, 2, 3);
```

> **💡 Recommendation:** Use **Float64 Specialized** for learning and prototyping (simpler API), then switch to **Generic<double>** for production code requiring optimal performance.

> **📊 Complete Benchmarks:** See [GENERIC_VS_SPECIALIZED_PERFORMANCE.md](../GENERIC_VS_SPECIALIZED_PERFORMANCE.md) for detailed analysis.

---

## First Example

### Simple Vector Operations

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;

// 1. Create GA processor (3D Euclidean GA) - Modern simplified API
var processor = XGaFloat64Processor.Euclidean;

// 2. Create vectors
var v1 = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 0)
    .SetVectorTerm(2, 0)
    .GetVector();  // x-axis

var v2 = processor.CreateVectorComposer()
    .SetVectorTerm(0, 0)
    .SetVectorTerm(1, 1)
    .SetVectorTerm(2, 0)
    .GetVector();  // y-axis

// 4. Geometric product
var gp = v1.Gp(v2);  // = xy bivector

// 5. Outer product
var op = v1.Op(v2);  // = xy bivector

// 6. Inner product
var ip = v1.Lcp(v2);  // = 0 (orthogonal)

// 7. Output
Console.WriteLine($"Geometric Product: {gp}");
Console.WriteLine($"Outer Product: {op}");
Console.WriteLine($"Inner Product: {ip}");
```

**Output:**
```
Geometric Product: '1'<1,2>
Outer Product: '1'<1,2>
Inner Product: 0
```

> **⚡ Performance Note:** This example uses `XGaFloat64Processor` (Float64 Specialized) for simplicity. For **27% better performance**, use `XGaProcessor<double>` as shown in the [Performance Considerations](#performance-considerations) section above.

---

## Core Concepts

### 1. Scalar Processors

A **scalar processor** defines how scalars (numbers) are handled.

**Available Processors:**

```csharp
// Float64 (standard double)
var sp1 = ScalarProcessorOfFloat64.Instance;

// Float32 (standard float)
var sp2 = ScalarProcessorOfFloat32.Instance;

// Arbitrary Precision Decimal
var sp3 = ScalarProcessorOfDecimal.Instance;

// Rational Numbers (fractions)
var sp4 = ScalarProcessorOfRational.Instance;

// Complex Numbers
var sp5 = ScalarProcessorOfComplex.Instance;

// Symbolic (Mathematica)
var sp6 = ScalarProcessorOfMathematica.Instance;
```

**Example:**
```csharp
// With rational numbers
var scalarProc = ScalarProcessorOfRational.Instance;
var processor = XGaProcessor<Rational>.CreateEuclidean(scalarProc);

var composer = processor.CreateVectorComposer();
composer.SetVectorTerm(0, new Rational(1, 2));  // 1/2
composer.SetVectorTerm(1, new Rational(1, 3));  // 1/3
composer.SetVectorTerm(2, new Rational(1, 4));  // 1/4
var v = composer.GetVector();
```

---

### 2. GA Processors

A **GA processor** manages multivectors and GA operations.

**Types (Float64):**

```csharp
// Euclidean (all e_i^2 = +1) - Most common
var euclidean = XGaFloat64Processor.Euclidean;

// Conformal (includes 1 negative signature)
var conformal = XGaFloat64Processor.Conformal;

// Projective (includes 1 zero signature)
var projective = XGaFloat64Processor.Projective;

// Custom metric
var custom = XGaFloat64Processor.Create(
    negativeCount: 1,
    zeroCount: 0
);
```

**Types (Generic for non-Float64):**

```csharp
// Generic GA processor (for Rational, Symbolic, etc.)
var processor = XGaProcessor<T>.CreateEuclidean(scalarProcessor);

// Custom metric with generic type
var custom = XGaProcessor<T>.Create(
    scalarProcessor,
    negativeCount: 1,
    zeroCount: 0
);
```

**Common Metrics:**
```csharp
// 3D Euclidean: All positive signatures
var ga3d = XGaFloat64Processor.Euclidean;

// Spacetime (Minkowski): (3,1,0) - 3 positive, 1 negative
var spacetime = XGaFloat64Processor.Create(negativeCount: 1, zeroCount: 0);

// 5D Conformal: (4,1,0) - Used for 3D geometry
var cga = CGaFloat64GeometricSpace5D.Instance;  // Specialized CGA
```

> **⚡ Performance Tip:** All examples above use Float64 Specialized. For production code, prefer `XGaProcessor<double>` for **27% faster performance**. See [Performance Considerations](#performance-considerations).

---

### 3. Multivectors

**Multivectors** are the fundamental objects in GA.

**Creation:**

```csharp
// Scalars (Grade 0)
var scalar = processor.CreateScalar(5.0);

// Vectors (Grade 1)
var composer = processor.CreateVectorComposer();
composer.SetVectorTerm(0, 1);
composer.SetVectorTerm(1, 2);
composer.SetVectorTerm(2, 3);
var vector = composer.GetVector();

// Bivectors (Grade 2)
composer = processor.CreateBivectorComposer();
composer.SetBivectorTerm(0, 1, 1.0); // xy
composer.SetBivectorTerm(0, 2, 2.0); // xz
composer.SetBivectorTerm(1, 2, 3.0); // yz
var bivector = composer.GetBivector();

// General multivectors
composer = processor.CreateMultivectorComposer();
composer.SetTerm(0, 1.0);           // Scalar part
composer.SetTerm(1, 2.0);           // e_1
composer.SetTerm(2, 3.0);           // e_2
composer.SetBivectorTerm(0, 1, 4.0); // e_1 ∧ e_2
var mv = composer.GetMultivector();
```

---

### 4. GA Operations

**Basic Products:**

```csharp
var composer = processor.CreateVectorComposer();
composer.SetVectorTerm(0, 1);
composer.SetVectorTerm(1, 0);
composer.SetVectorTerm(2, 0);
var v1 = composer.GetVector();

composer = processor.CreateVectorComposer();
composer.SetVectorTerm(0, 0);
composer.SetVectorTerm(1, 1);
composer.SetVectorTerm(2, 0);
var v2 = composer.GetVector();

// Geometric product
var gp = v1.Gp(v2);

// Outer product (Wedge Product)
var op = v1.Op(v2);

// Inner product (Left Contraction)
var lcp = v1.Lcp(v2);

// Right Contraction
var rcp = v1.Rcp(v2);

// Scalar product
var sp = v1.Sp(v2);

// Fat dot product
var fdp = v1.Fdp(v2);
```

**Unary Operations:**

```csharp
// Create a multivector (example)
var composer = processor.CreateMultivectorComposer();
composer.SetTerm(0, 1.0);  // Scalar
composer.SetVectorTerm(0, 2.0);  // e_1
composer.SetBivectorTerm(0, 1, 3.0);  // e_1 ∧ e_2
var mv = composer.GetMultivector();

// Reverse
var rev = mv.Reverse();

// Grade Involution
var gi = mv.GradeInvolution();

// Clifford Conjugate
var cc = mv.CliffordConjugate();

// Dual
var dual = mv.Dual();

// Magnitude (Norm)
var norm = mv.Norm();
var normSquared = mv.NormSquared();

// Normalization
var normalized = mv.Normalize();
```

---

### 5. Basis Blades

**Basis blades** are the basis elements of GA.

```csharp
// Basis vectors
var e1 = processor.CreateBasisVector(0);  // e_1
var e2 = processor.CreateBasisVector(1);  // e_2
var e3 = processor.CreateBasisVector(2);  // e_3

// Basis bivectors
var e12 = processor.CreateBasisBivector(0, 1);  // e_1 ∧ e_2
var e23 = processor.CreateBasisBivector(1, 2);  // e_2 ∧ e_3

// General basis blades
var e123 = processor.CreateBasisBlade(0, 1, 2);  // e_1 ∧ e_2 ∧ e_3

// Products on basis blades
var product = e1.Gp(e2);  // e_1 * e_2
```

---

## Common Workflows

### Workflow 1: Numerical 3D Geometry

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa;

// Setup
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaFloat64GeometricSpace5D.Instance;

// Define points
var p1 = cga.Encode.IpnsRound.Point(0, 0, 0);
var p2 = cga.Encode.IpnsRound.Point(1, 0, 0);
var p3 = cga.Encode.IpnsRound.Point(0, 1, 0);

// Plane through three points
var plane = p1.Op(p2).Op(p3);

// Define sphere
var sphere = cga.Encode.IpnsRound.RealSphere(
    radius: 2,
    centerX: 1,
    centerY: 1,
    centerZ: 1
);

// Intersection of plane and sphere (yields circle)
var circle = plane.Op(sphere);

// Decode
var circleData = circle.Decode.OpnsRound.Element();
var center = circleData.CenterToVector3D();
var radius = circleData.RealRadius;

Console.WriteLine($"Circle Center: {center}");
Console.WriteLine($"Circle Radius: {radius}");
```

> **⚡ Performance Note:** This workflow uses `CGaFloat64GeometricSpace5D` (Float64 Specialized). For **27% faster** conformal geometry operations, consider migrating to Generic<T> CGA implementation. Contact the maintainers for Generic CGA documentation.

---

### Workflow 2: Symbolic Computations

```csharp
using GeometricAlgebraFulcrumLib.Mathematica;

// Mathematica scalar processor
var scalarProcessor = ScalarProcessorOfMathematica.Instance;
var processor = XGaProcessor<Expr>.CreateEuclidean(scalarProcessor);

// Symbolic parameters
var x = scalarProcessor.CreateSymbol("x");
var y = scalarProcessor.CreateSymbol("y");
var z = scalarProcessor.CreateSymbol("z");

// Vector with symbolic components
var composer = processor.CreateVectorComposer();
composer.SetVectorTerm(0, x);
composer.SetVectorTerm(1, y);
composer.SetVectorTerm(2, z);
var v = composer.GetVector();

// Computation
var normSquared = v.NormSquared();

// Output: normSquared is symbolic expression
// x^2 + y^2 + z^2
Console.WriteLine($"||v||^2 = {normSquared}");
```

---

### Workflow 3: Code Generation

```csharp
using GeometricAlgebraFulcrumLib.MetaProgramming;

// 1. Create meta context
var context = new MetaContext();

// 2. Define symbolic parameters
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");
var z = context.CreateParameter("z");

// 3. GA computations
var scalarProcessor = context.ScalarProcessor;
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);

var composer = processor.CreateVectorComposer();
composer.SetVectorTerm(0, x);
composer.SetVectorTerm(1, y);
composer.SetVectorTerm(2, z);
var v1 = composer.GetVector();

composer = processor.CreateVectorComposer();
composer.SetVectorTerm(0, 1);
composer.SetVectorTerm(1, 0);
composer.SetVectorTerm(2, 0);
var v2 = composer.GetVector();

var result = v1.Gp(v2);

// 4. Define output
context.SetOutputVariable("result", result);

// 5. Optimize
context.OptimizeContext();

// 6. Generate C# code
var codeComposer = new CSharpCodeComposer();
var code = codeComposer.GenerateCode(context);

// 7. Output
Console.WriteLine(code);
```

**Generated Code:**
```csharp
public static void ComputeGeometricProduct(
    double x, double y, double z,
    out double result_scalar,
    out double result_e1,
    out double result_e2,
    out double result_e3
) {
    result_scalar = x;
    result_e1 = 0;
    result_e2 = z;
    result_e3 = -y;
}
```

---

### Workflow 4: Rotations

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Rotation bivector (rotation plane)
var composer = processor.CreateBivectorComposer();
composer.SetBivectorTerm(0, 1, 1.0); // xy
composer.SetBivectorTerm(0, 2, 0.0); // xz
composer.SetBivectorTerm(1, 2, 0.0); // yz
var B = composer.GetBivector();

// Angle
var angle = Math.PI / 4;  // 45°

// Create rotor using exponential
var rotor = (-angle / 2 * B).Exp();

// Vector to rotate
composer = processor.CreateVectorComposer();
composer.SetVectorTerm(0, 1);
composer.SetVectorTerm(1, 0);
composer.SetVectorTerm(2, 0);
var v = composer.GetVector();

// Rotation: v' = R v R^†
var rotated = rotor.Gp(v).Gp(rotor.Reverse());

Console.WriteLine($"Original: {v}");
Console.WriteLine($"Rotated: {rotated}");
```

---

## Next Steps

### Further Documentation

1. **[Architecture](architecture.en.md)**: Understand the system design
2. **[Design Principles](design-principles.en.md)**: Learn the design philosophy
3. **[API Reference](api-reference.en.md)**: Detailed API documentation
4. **[Examples](examples.en.md)**: Comprehensive code examples

### Sample Projects

Check out the sample projects in the repository:

```
GeometricAlgebraFulcrumLib/
├── GeometricAlgebraFulcrumLib.Applications/
├── GeometricAlgebraFulcrumLib.Applications.Symbolic/
├── GeometricAlgebraFulcrumLib.Samples.Generations/
└── GeometricAlgebraFulcrumLib.UnitTests/
```

### Tutorials

1. **3D Geometry with CGA**
   - Point, line, plane, circle, sphere
   - Transformations
   - Intersections and projections

2. **Rotors and Versors**
   - Rotations
   - Reflections
   - Combined transformations

3. **Code Generation**
   - Optimization
   - Multi-language support
   - Template-based generation

4. **Symbolic Mathematics**
   - Mathematica integration
   - Simplifications
   - Derivatives

### Community and Support

- **GitHub Issues**: Bug reports and feature requests
- **Discussions**: Questions and discussions
- **Email**: ga.computing.eg@gmail.com

### Further Resources

**Books on Geometric Algebra:**
- *Geometric Algebra for Computer Science* - Dorst, Fontijne, Mann
- *Geometric Algebra for Physicists* - Doran, Lasenby
- *New Foundations for Classical Mechanics* - Hestenes

**Online Resources:**
- [bivector.net](https://bivector.net)
- [GA-FuL Paper on MDPI](https://www.mdpi.com/2227-7390/12/14/2272)

---

## Common Errors and Solutions

### Error 1: Scalar Type Mismatch

**Problem:**
```csharp
// Float64 processor
var proc1 = XGaFloat64Processor.Euclidean;
var v1 = proc1.CreateVectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();

// ERROR: Different scalar type (Float32)
var sp2 = ScalarProcessorOfFloat32.Instance;
var proc2 = XGaProcessor<float>.CreateEuclidean(sp2);
var v2 = proc2.CreateVectorComposer()
    .SetVectorTerm(0, 4f)
    .SetVectorTerm(1, 5f)
    .SetVectorTerm(2, 6f)
    .GetVector();

var result = v1.Gp(v2);  // Compilation error! Different types
```

**Solution:** Use consistent scalar types:
```csharp
var processor = XGaFloat64Processor.Euclidean;
var v1 = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();
var v2 = processor.CreateVectorComposer()
    .SetVectorTerm(0, 4)
    .SetVectorTerm(1, 5)
    .SetVectorTerm(2, 6)
    .GetVector();
var result = v1.Gp(v2);  // OK!
```

---

### Error 2: Dimension Awareness

**Note:** GA-FuL processors don't have a fixed dimension. You can create vectors of any dimension:

```csharp
var processor = XGaFloat64Processor.Euclidean;

// Create 3D vector
var v3d = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();

// Create 4D vector with the same processor
var v4d = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .SetVectorTerm(3, 4)
    .GetVector();

// Both work fine!
```

**Custom Metric Example:**
```csharp
// For non-Euclidean metrics (e.g., Minkowski spacetime)
var spacetime = XGaFloat64Processor.Create(
    negativeCount: 1,  // Time dimension
    zeroCount: 0
);
```

---

### Error 3: Forgetting to Normalize

**Problem:**
```csharp
var processor = XGaFloat64Processor.Euclidean;
var v = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();
// v is NOT normalized! Magnitude = sqrt(1² + 2² + 3²) = sqrt(14) ≈ 3.74
```

**Solution:**
```csharp
var v = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector()
    .Normalize();  // Now magnitude = 1

// Or separately:
var v = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();
v = v.DivideByNorm();  // Same as Normalize()
```

---

## Tips and Best Practices

1. **Use `var`**: Code becomes more readable
   ```csharp
   var processor = XGaFloat64Processor.Euclidean;
   // instead of
   XGaFloat64Processor processor = XGaFloat64Processor.Euclidean;
   ```

2. **Reuse processors**: Create processors only once
   ```csharp
   // Good: Reuse processor
   var processor = XGaFloat64Processor.Euclidean;
   var v1 = processor.CreateVectorComposer().SetVectorTerm(0, 1).GetVector();
   var v2 = processor.CreateVectorComposer().SetVectorTerm(1, 1).GetVector();

   // Bad: Create processor multiple times
   var v1 = XGaFloat64Processor.Euclidean.CreateVectorComposer().SetVectorTerm(0, 1).GetVector();
   var v2 = XGaFloat64Processor.Euclidean.CreateVectorComposer().SetVectorTerm(1, 1).GetVector();
   ```

3. **Reuse composers**: For multiple similar multivectors
   ```csharp
   var processor = XGaFloat64Processor.Euclidean;
   var composer = processor.CreateVectorComposer();

   // First vector
   composer.Clear();
   composer.SetVectorTerm(0, 1).SetVectorTerm(1, 0);
   var v1 = composer.GetVector();

   // Second vector (reuse composer)
   composer.Clear();
   composer.SetVectorTerm(0, 0).SetVectorTerm(1, 1);
   var v2 = composer.GetVector();
   ```

4. **Use method chaining for simple cases**
   ```csharp
   // Simple vector creation with chaining
   var v = processor.CreateVectorComposer()
       .SetVectorTerm(0, 1)
       .SetVectorTerm(1, 2)
       .SetVectorTerm(2, 3)
       .GetVector();
   ```

5. **Check your results**: Use `.ToString()` for debugging
   ```csharp
   Console.WriteLine($"Result: {result}");
   Console.WriteLine($"Norm: {result.Norm()}");
   ```

---

**Last Updated**: 2025-10-27 | **API**: Fully updated | **Performance**: Generic<T> 1.24-2.31x faster 🚀

**Note:** This guide uses Float64 Specialized API for simplicity. For production code, consider using Generic<double> for optimal performance. See [Performance Considerations](#performance-considerations).

[← Back to Main Documentation](README.en.md)
