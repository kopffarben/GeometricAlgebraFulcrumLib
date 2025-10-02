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
3. [First Example](#first-example)
4. [Core Concepts](#core-concepts)
5. [Common Workflows](#common-workflows)
6. [Next Steps](#next-steps)

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

## First Example

### Simple Vector Operations

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

// 1. Select scalar processor (64-bit floats)
var scalarProcessor = ScalarProcessorOfFloat64.Instance;

// 2. Create GA processor (3D Euclidean GA)
var processor = XGaProcessor<double>.Create(scalarProcessor);

// 3. Create vectors
var v1 = processor.CreateVector(1, 0, 0);  // x-axis
var v2 = processor.CreateVector(0, 1, 0);  // y-axis

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
var processor = XGaProcessor<Rational>.Create(scalarProc);

var v = processor.CreateVector(
    new Rational(1, 2),  // 1/2
    new Rational(1, 3),  // 1/3
    new Rational(1, 4)   // 1/4
);
```

---

### 2. GA Processors

A **GA processor** manages multivectors and GA operations.

**Types:**

```csharp
// Generic GA processor (arbitrary metric)
var processor = XGaProcessor<T>.Create(scalarProcessor);

// Euclidean GA (all e_i^2 = +1)
var euclidean = XGaProcessor<T>.CreateEuclidean(scalarProcessor);

// Conformal GA
var conformal = XGaConformalSpace5D<T>.Create(scalarProcessor);

// Projective GA
var projective = XGaProjectiveSpace<T>.Create(scalarProcessor, dimension);
```

**With Metric:**
```csharp
// Metric (p, q, r)
// p: Number of +1 squares
// q: Number of -1 squares
// r: Number  0 squares

// 3D Euclidean: (3, 0, 0)
var ga3d = XGaProcessor<double>.Create(scalarProcessor, 3, 0, 0);

// Spacetime (Minkowski): (3, 1, 0)
var spacetime = XGaProcessor<double>.Create(scalarProcessor, 3, 1, 0);

// 5D Conformal: (4, 1, 0)
var cga5d = XGaProcessor<double>.Create(scalarProcessor, 4, 1, 0);
```

---

### 3. Multivectors

**Multivectors** are the fundamental objects in GA.

**Creation:**

```csharp
// Scalars (Grade 0)
var scalar = processor.CreateScalar(5.0);

// Vectors (Grade 1)
var vector = processor.CreateVector(1, 2, 3);

// Bivectors (Grade 2)
var bivector = processor.CreateBivector(
    xy: 1,  // e_1 ∧ e_2
    xz: 2,  // e_1 ∧ e_3
    yz: 3   // e_2 ∧ e_3
);

// General multivectors
var composer = processor.CreateComposer();
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
var v1 = processor.CreateVector(1, 0, 0);
var v2 = processor.CreateVector(0, 1, 0);

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
var mv = processor.CreateMultivector(...);

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
var cga = CGaGeometricSpace5D<double>.Create(scalarProcessor);

// Define points
var p1 = cga.EncodeIpnsRound.Point(0, 0, 0);
var p2 = cga.EncodeIpnsRound.Point(1, 0, 0);
var p3 = cga.EncodeIpnsRound.Point(0, 1, 0);

// Plane through three points
var plane = p1.Op(p2).Op(p3);

// Define sphere
var sphere = cga.EncodeIpnsRound.Sphere(
    centerX: 1,
    centerY: 1,
    centerZ: 1,
    radius: 2
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

---

### Workflow 2: Symbolic Computations

```csharp
using GeometricAlgebraFulcrumLib.Mathematica;

// Mathematica scalar processor
var scalarProcessor = ScalarProcessorOfMathematica.Instance;
var processor = XGaProcessor<Expr>.Create(scalarProcessor);

// Symbolic parameters
var x = scalarProcessor.CreateSymbol("x");
var y = scalarProcessor.CreateSymbol("y");
var z = scalarProcessor.CreateSymbol("z");

// Vector with symbolic components
var v = processor.CreateVector(x, y, z);

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
var processor = XGaProcessor<IMetaExpression>.Create(scalarProcessor);

var v1 = processor.CreateVector(x, y, z);
var v2 = processor.CreateVector(1, 0, 0);
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
var processor = XGaProcessor<double>.Create(scalarProcessor);

// Rotation bivector (rotation plane)
var B = processor.CreateBivector(
    xy: 1,  // Rotation in xy-plane
    xz: 0,
    yz: 0
);

// Angle
var angle = Math.PI / 4;  // 45°

// Rotor
var rotor = (-angle / 2 * B).Exp();

// Vector to rotate
var v = processor.CreateVector(1, 0, 0);

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
var sp1 = ScalarProcessorOfFloat64.Instance;
var proc = XGaProcessor<double>.Create(sp1);
var v = proc.CreateVector(1, 2, 3);

// ERROR: Wrong scalar type
var sp2 = ScalarProcessorOfFloat32.Instance;
var other = XGaProcessor<float>.Create(sp2);
var result = v.Gp(other.CreateVector(4, 5, 6));  // Compilation error!
```

**Solution:** Use consistent scalar types:
```csharp
var sp = ScalarProcessorOfFloat64.Instance;
var proc = XGaProcessor<double>.Create(sp);
var v1 = proc.CreateVector(1, 2, 3);
var v2 = proc.CreateVector(4, 5, 6);
var result = v1.Gp(v2);  // OK!
```

---

### Error 2: Wrong Metric

**Problem:**
```csharp
// 3D Euclidean
var proc = XGaProcessor<double>.CreateEuclidean(sp);

// Try to create 4D vector
var v = proc.CreateVector(1, 2, 3, 4);  // Works, but...
// ... processor expects 3D metric!
```

**Solution:** Define metric explicitly:
```csharp
// For 4D
var proc = XGaProcessor<double>.Create(sp, 4, 0, 0);
var v = proc.CreateVector(1, 2, 3, 4);  // OK!
```

---

### Error 3: Forgetting to Normalize

**Problem:**
```csharp
var v = processor.CreateVector(1, 2, 3);
// v is NOT normalized!
```

**Solution:**
```csharp
var v = processor.CreateVector(1, 2, 3).Normalize();
// or
var v = processor.CreateVector(1, 2, 3);
v = v.DivideByNorm();
```

---

## Tips and Best Practices

1. **Use `var`**: Code becomes more readable
   ```csharp
   var processor = XGaProcessor<double>.Create(...);
   // instead of
   XGaProcessor<double> processor = XGaProcessor<double>.Create(...);
   ```

2. **Reuse processors**: Create processors only once
   ```csharp
   // Good
   var processor = XGaProcessor<double>.CreateEuclidean(sp);
   var v1 = processor.CreateVector(...);
   var v2 = processor.CreateVector(...);

   // Bad
   var v1 = XGaProcessor<double>.CreateEuclidean(sp).CreateVector(...);
   var v2 = XGaProcessor<double>.CreateEuclidean(sp).CreateVector(...);
   ```

3. **Use composer for complex multivectors**
   ```csharp
   var composer = processor.CreateComposer();
   composer.SetTerm(...);
   composer.AddTerm(...);
   var mv = composer.GetMultivector();
   ```

4. **Check your results**: Use `.ToString()` for debugging
   ```csharp
   Console.WriteLine($"Result: {result}");
   ```

---

[← Back to Main Documentation](README.en.md)
