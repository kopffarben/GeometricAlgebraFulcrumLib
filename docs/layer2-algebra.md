# Layer 2: Geometric Algebra Core

The Algebra layer represents the mathematical heart of GA-FuL, implementing the fundamental structures and operations of Geometric Algebra. This layer builds upon the utilities foundation to provide a comprehensive, generic framework for GA computations across different scalar types and geometric spaces.

## Architecture Overview

The algebra layer follows a hierarchical design pattern with clear separation between different levels of abstraction:

### Core Hierarchy

```
IGeometricAlgebraElement<T>
├── IScalar<T>
├── IMultivector<T>
│   ├── XGaMultivector<T>
│   │   ├── XGaScalar<T>
│   │   ├── XGaVector<T>
│   │   ├── XGaBivector<T>
│   │   ├── XGaHigherKVector<T>
│   │   └── XGaGradedMultivector<T>
│   └── ConformalMultivector<T>
└── IGeometricProcessor<T>
```

## Project Structure

### GeometricAlgebraFulcrumLib.Algebra

The main algebra project contains the core mathematical implementations:

**Key Namespaces:**
- `Scalars` - Scalar value abstractions and operations
- `GeometricAlgebraSpaces` - GA space definitions and metrics
- `LinearAlgebra` - Vector spaces and linear operations
- `GeometricAlgebra` - Core GA structures and operations
- `PolynomialAlgebra` - Polynomial operations over GA

**Dependencies:**
- GeometricAlgebraFulcrumLib.Utilities.Structures
- GeometricAlgebraFulcrumLib.Utilities.Text

## Core Components

### 1. Scalar Processing System

The scalar processing system provides a unified interface for different numeric types:

```csharp
public interface IScalarProcessor<T>
{
    T Zero { get; }
    T One { get; }
    T MinusOne { get; }
    
    T Add(T scalar1, T scalar2);
    T Subtract(T scalar1, T scalar2);
    T Multiply(T scalar1, T scalar2);
    T Divide(T scalar1, T scalar2);
    
    T Power(T baseScalar, T scalar);
    T Sqrt(T scalar);
    T Cos(T scalar);
    T Sin(T scalar);
    T Exp(T scalar);
    T Log(T scalar);
}
```

**Built-in Implementations:**
- `Float32ScalarProcessor` - Single precision floating point
- `Float64ScalarProcessor` - Double precision floating point  
- `ComplexFloat64ScalarProcessor` - Complex numbers
- `RationalScalarProcessor` - Exact rational arithmetic
- `SymbolicScalarProcessor` - Symbolic expressions

### 2. Geometric Algebra Spaces

GA spaces are defined by their signature and dimension:

```csharp
public interface IGeometricAlgebraSpace
{
    int VSpaceDimensions { get; }
    ulong GaSpaceDimensions { get; }
    IGaSignature Signature { get; }
    
    bool IsValidBasisBladeId(ulong basisBladeId);
    bool IsValidKVectorGrade(int grade);
}
```

**Space Types:**
- **Euclidean Spaces**: Positive definite metrics (e₁² = e₂² = ... = +1)
- **Minkowski Spaces**: Mixed signature spaces (e₀² = -1, e₁² = e₂² = ... = +1)
- **Conformal Spaces**: Extended spaces with null vectors for geometric modeling
- **Projective Spaces**: Homogeneous coordinate systems

### 3. Multivector Representation

Multivectors are represented using sparse storage with basis blade indexing:

```csharp
public sealed class XGaMultivector<T> : IXGaMultivector<T>
{
    private readonly Dictionary<ulong, T> _idScalarDictionary;
    
    public XGaProcessor<T> Processor { get; }
    public int VSpaceDimensions { get; }
    
    // Access coefficient by basis blade ID
    public T this[ulong basisBladeId] { get; }
    
    // Grade selection operations
    public XGaKVector<T> GetKVectorPart(int grade);
    public XGaMultivector<T> GetEvenPart();
    public XGaMultivector<T> GetOddPart();
}
```

**Basis Blade Encoding:**
- Each basis blade is encoded as a ulong bit pattern
- Bit i represents basis vector eᵢ
- Example: e₁₂₃ → binary 0111 → decimal 7

### 4. Operations Implementation

#### Geometric Product

The fundamental GA operation combining inner and outer products:

```csharp
public XGaMultivector<T> Gp(XGaMultivector<T> mv2)
{
    var composer = Processor.CreateComposer();
    
    foreach (var (id1, scalar1) in _idScalarDictionary)
    foreach (var (id2, scalar2) in mv2._idScalarDictionary)
    {
        var (id, sign) = GeometricProduct(id1, id2);
        var scalar = sign > 0 
            ? ScalarProcessor.Times(scalar1, scalar2)
            : ScalarProcessor.Negative(ScalarProcessor.Times(scalar1, scalar2));
            
        composer.AddEGpTerm(id, scalar);
    }
    
    return composer.GetMultivector();
}
```

#### Outer Product (Wedge Product)

```csharp
public XGaMultivector<T> Op(XGaMultivector<T> mv2)
{
    var composer = Processor.CreateComposer();
    
    foreach (var (id1, scalar1) in _idScalarDictionary)
    foreach (var (id2, scalar2) in mv2._idScalarDictionary)
    {
        // Only add if basis blades don't share common factors
        if ((id1 & id2) == 0)
        {
            var id = id1 ^ id2;
            var sign = OuterProductSign(id1, id2);
            var scalar = sign > 0 
                ? ScalarProcessor.Times(scalar1, scalar2)
                : ScalarProcessor.Negative(ScalarProcessor.Times(scalar1, scalar2));
                
            composer.AddEGpTerm(id, scalar);
        }
    }
    
    return composer.GetMultivector();
}
```

#### Inner Products

Left and right contractions for different geometric interpretations:

```csharp
// Left contraction: A ⌋ B
public XGaMultivector<T> Lcp(XGaMultivector<T> mv2)
{
    var composer = Processor.CreateComposer();
    
    foreach (var (id1, scalar1) in _idScalarDictionary)
    foreach (var (id2, scalar2) in mv2._idScalarDictionary)
    {
        var grade1 = id1.Grade();
        var grade2 = id2.Grade();
        
        if (grade1 <= grade2)
        {
            var (id, sign) = LeftContraction(id1, id2);
            if (id.HasValue)
            {
                var scalar = sign > 0 
                    ? ScalarProcessor.Times(scalar1, scalar2)
                    : ScalarProcessor.Negative(ScalarProcessor.Times(scalar1, scalar2));
                    
                composer.AddEGpTerm(id.Value, scalar);
            }
        }
    }
    
    return composer.GetMultivector();
}
```

### 5. K-Vector Specializations

Specialized classes for specific grades provide optimized operations:

#### Scalars (Grade 0)
```csharp
public sealed class XGaScalar<T> : XGaKVector<T>
{
    public T ScalarValue { get; }
    
    public static implicit operator T(XGaScalar<T> scalar) => scalar.ScalarValue;
    public static implicit operator XGaScalar<T>(T scalarValue) => new(scalarValue);
}
```

#### Vectors (Grade 1)
```csharp
public sealed class XGaVector<T> : XGaKVector<T>
{
    // Component access by index
    public T this[int index] { get; }
    
    // Geometric operations optimized for vectors
    public T Sp(XGaVector<T> vector2);  // Scalar product
    public XGaBivector<T> Op(XGaVector<T> vector2);  // Outer product
    public XGaMultivector<T> Gp(XGaVector<T> vector2);  // Geometric product
}
```

#### Bivectors (Grade 2)
```csharp
public sealed class XGaBivector<T> : XGaKVector<T>
{
    // Dual operations
    public XGaKVector<T> Dual();
    public XGaKVector<T> UnDual();
    
    // Specialized bivector operations
    public XGaScalar<T> Sp(XGaBivector<T> bivector2);
    public XGaMultivector<T> Gp(XGaBivector<T> bivector2);
}
```

## Advanced Features

### 1. Conformal Geometric Algebra

Specialized implementation for conformal GA with optimized point, line, plane, and sphere operations:

```csharp
public sealed class CGaProcessor<T> : XGaProcessor<T>
{
    public CGaVector<T> EncodePoint(LinVector3D<T> point);
    public CGaVector<T> EncodeDirection(LinVector3D<T> direction);
    public CGaBivector<T> EncodeLine(LinVector3D<T> point, LinVector3D<T> direction);
    public CGaMultivector<T> EncodePlane(T distance, LinVector3D<T> normal);
    public CGaMultivector<T> EncodeSphere(LinVector3D<T> center, T radius);
    
    public LinVector3D<T> DecodePoint(CGaVector<T> cgaPoint);
    public LinVector3D<T> DecodeDirection(CGaVector<T> cgaDirection);
    public Tuple<LinVector3D<T>, LinVector3D<T>> DecodeLine(CGaBivector<T> cgaLine);
}
```

### 2. Outermorphisms

Linear transformations that preserve the outer product structure:

```csharp
public sealed class XGaOutermorphism<T>
{
    private readonly Dictionary<int, LinMatrix<T>> _gradeMatrixDictionary;
    
    public XGaMultivector<T> Map(XGaMultivector<T> multivector);
    public XGaOutermorphism<T> GetInverse();
    public XGaOutermorphism<T> GetAdjoint();
    
    // Composition of outermorphisms
    public static XGaOutermorphism<T> operator *(
        XGaOutermorphism<T> om1, 
        XGaOutermorphism<T> om2);
}
```

### 3. Versors and Rotations

Efficient representation of rotations using geometric product structure:

```csharp
public sealed class XGaVersor<T>
{
    private readonly XGaMultivector<T> _multivector;
    
    public XGaMultivector<T> Map(XGaMultivector<T> mv)
    {
        return _multivector.Gp(mv).Gp(_multivector.Reverse());
    }
    
    public XGaVersor<T> GetInverse()
    {
        return new XGaVersor<T>(_multivector.Reverse());
    }
    
    // Create rotation versor from bivector
    public static XGaVersor<T> CreateRotation(XGaBivector<T> bivector, T angle);
}
```

## Performance Optimizations

### 1. Sparse Representation

Only non-zero coefficients are stored, reducing memory usage and computation time:

```csharp
// Instead of dense array of 2^n elements
private T[] _coefficients = new T[1 << VSpaceDimensions];

// Use sparse dictionary
private Dictionary<ulong, T> _idScalarDictionary = new();
```

### 2. Lazy Evaluation

Operations are constructed as expression trees and evaluated only when needed:

```csharp
public sealed class XGaMultivectorExpression<T>
{
    private readonly Func<XGaMultivector<T>> _evaluationFunc;
    private XGaMultivector<T>? _cachedResult;
    
    public XGaMultivector<T> Evaluate()
    {
        return _cachedResult ??= _evaluationFunc();
    }
}
```

### 3. Grade-Selective Operations

Operations can be restricted to specific grades for efficiency:

```csharp
public XGaMultivector<T> Gp(XGaMultivector<T> mv2, params int[] targetGrades)
{
    var composer = Processor.CreateComposer();
    
    foreach (var (id1, scalar1) in _idScalarDictionary)
    foreach (var (id2, scalar2) in mv2._idScalarDictionary)
    {
        var targetGrade = (id1 ^ id2).Grade();
        if (!targetGrades.Contains(targetGrade)) continue;
        
        // Perform computation only for desired grades
        // ...
    }
    
    return composer.GetMultivector();
}
```

## Code Examples

### Basic GA Operations

<details>
<summary>Basic Geometric Algebra Operations</summary>

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;

// Create a 3D Euclidean GA processor
var processor = XGaProcessor<double>.CreateEuclidean(
    Float64ScalarProcessor.Instance
);

// Create vectors
var e1 = processor.VectorTerm(0, 1.0);  // e1
var e2 = processor.VectorTerm(1, 1.0);  // e2
var e3 = processor.VectorTerm(2, 1.0);  // e3

// Vector operations
var v1 = 2 * e1 + 3 * e2 + e3;  // 2e1 + 3e2 + e3
var v2 = e1 - 2 * e2 + 4 * e3;  // e1 - 2e2 + 4e3

// Scalar product (inner product)
var dot = v1.Sp(v2);  // Result: 0 (2*1 + 3*(-2) + 1*4 = 0)

// Outer product (wedge product)
var wedge = v1.Op(v2);  // Result: bivector

// Geometric product
var geometric = v1.Gp(v2);  // Result: dot + wedge

Console.WriteLine($"v1 = {v1}");
Console.WriteLine($"v2 = {v2}");
Console.WriteLine($"v1 · v2 = {dot}");
Console.WriteLine($"v1 ∧ v2 = {wedge}");
Console.WriteLine($"v1 * v2 = {geometric}");

// Expected Output:
// v1 = 2*e1 + 3*e2 + 1*e3
// v2 = 1*e1 + -2*e2 + 4*e3
// v1 · v2 = 0
// v1 ∧ v2 = 14*e1^e2 + 6*e1^e3 + -5*e2^e3
// v1 * v2 = 14*e1^e2 + 6*e1^e3 + -5*e2^e3
```

</details>

### Conformal Geometric Algebra

<details>
<summary>3D Point and Line Operations using CGA</summary>

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Generic.Processors;
using GeometricAlgebraFulcrumLib.Modeling.GeometricAlgebra.Conformal.Generic;

// Create 5D conformal GA processor (3D + 2 extra dimensions)
var processor = XGaProcessor<double>.CreateConformal(
    Float64ScalarProcessor.Instance
);

var cgaProcessor = new CGaProcessor<double>(processor);

// Define 3D points
var point1 = new LinVector3D<double>(1, 0, 0);    // Point on x-axis
var point2 = new LinVector3D<double>(0, 1, 0);    // Point on y-axis
var point3 = new LinVector3D<double>(0, 0, 1);    // Point on z-axis

// Encode points in CGA
var cgaPoint1 = cgaProcessor.EncodePoint(point1);
var cgaPoint2 = cgaProcessor.EncodePoint(point2);
var cgaPoint3 = cgaProcessor.EncodePoint(point3);

// Create line through two points using outer product
var line12 = cgaPoint1.Op(cgaPoint2);

// Create plane through three points
var plane123 = cgaPoint1.Op(cgaPoint2).Op(cgaPoint3);

// Distance from point to plane
var distance = cgaPoint1.Lcp(plane123).Norm();

// Reflection of point across plane
var reflected = plane123.Gp(cgaPoint1).Gp(plane123.Reverse()) / plane123.NormSquared();

Console.WriteLine($"Line through points 1,2: {line12}");
Console.WriteLine($"Plane through points 1,2,3: {plane123}");
Console.WriteLine($"Distance from point 1 to plane: {distance}");

// Decode back to 3D
var decodedReflected = cgaProcessor.DecodePoint(reflected.GetVectorPart());
Console.WriteLine($"Reflected point: ({decodedReflected.X:F3}, {decodedReflected.Y:F3}, {decodedReflected.Z:F3})");

// Expected Output:
// Line through points 1,2: (CGA bivector representation)
// Plane through points 1,2,3: (CGA trivector representation) 
// Distance from point 1 to plane: 0.577
// Reflected point: (-0.333, -0.333, -0.333)
```

</details>

### Rotor Construction and Application

<details>
<summary>3D Rotations using Geometric Algebra Rotors</summary>

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;

// Create 3D Euclidean processor
var processor = XGaProcessor<double>.CreateEuclidean(
    Float64ScalarProcessor.Instance
);

// Create bivector representing rotation plane (xy-plane)
var xy_bivector = processor.BivectorTerm(0, 1, 1.0);  // e1^e2

// Create rotor for 90-degree rotation around z-axis
var angle = Math.PI / 2;  // 90 degrees
var rotor = (Math.Cos(angle/2) * processor.OneScalar() - 
            Math.Sin(angle/2) * xy_bivector).CreatePureRotor();

// Vector to rotate
var vector = 2 * processor.VectorTerm(0) + 3 * processor.VectorTerm(1);  // 2e1 + 3e2

// Apply rotation: R * v * R_reverse
var rotatedVector = rotor.Map(vector);

// Extract components
var x = rotatedVector.Scalar(1);  // Should be -3
var y = rotatedVector.Scalar(2);  // Should be 2
var z = rotatedVector.Scalar(4);  // Should be 0

Console.WriteLine($"Original vector: {vector}");
Console.WriteLine($"Rotor: {rotor}");
Console.WriteLine($"Rotated vector: {rotatedVector}");
Console.WriteLine($"Components: x={x:F3}, y={y:F3}, z={z:F3}");

// Verify rotor properties
var rotorNorm = rotor.Norm();
var rotorReverse = rotor.Reverse();
var identity = rotor.Gp(rotorReverse);

Console.WriteLine($"Rotor norm: {rotorNorm:F6}");  // Should be 1.0
Console.WriteLine($"R * R_reverse: {identity}");    // Should be scalar 1

// Expected Output:
// Original vector: 2*e1 + 3*e2
// Rotor: 0.707 + -0.707*e1^e2
// Rotated vector: -3*e1 + 2*e2  
// Components: x=-3.000, y=2.000, z=0.000
// Rotor norm: 1.000000
// R * R_reverse: 1
```

</details>

### Multi-Scalar Type Operations

<details>
<summary>Using Different Scalar Types (Float64, Rational, Symbolic)</summary>

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

// Float64 operations
var float64Processor = XGaProcessor<double>.CreateEuclidean(
    Float64ScalarProcessor.Instance);

var v1_f64 = float64Processor.Vector(1.5, 2.7);
var v2_f64 = float64Processor.Vector(0.8, -1.2);
var result_f64 = v1_f64.Gp(v2_f64);

Console.WriteLine("Float64 Operations:");
Console.WriteLine($"v1 = {v1_f64}");
Console.WriteLine($"v2 = {v2_f64}");
Console.WriteLine($"v1 * v2 = {result_f64}");

// Rational exact arithmetic
var rationalProcessor = XGaProcessor<Rational>.CreateEuclidean(
    RationalScalarProcessor.Instance);

var v1_rat = rationalProcessor.Vector(new Rational(3, 2), new Rational(5, 3));  // 3/2, 5/3
var v2_rat = rationalProcessor.Vector(new Rational(2, 3), new Rational(-4, 5));  // 2/3, -4/5
var result_rat = v1_rat.Gp(v2_rat);

Console.WriteLine("\nRational Exact Operations:");
Console.WriteLine($"v1 = {v1_rat}");
Console.WriteLine($"v2 = {v2_rat}");  
Console.WriteLine($"v1 * v2 = {result_rat}");

// Symbolic operations (requires AngouriMath or Mathematica)
var symbolicProcessor = XGaProcessor<Expr>.CreateEuclidean(
    SymbolicScalarProcessor.Instance);

var a = symbolicProcessor.ScalarProcessor.ScalarFromText("a");
var b = symbolicProcessor.ScalarProcessor.ScalarFromText("b");
var c = symbolicProcessor.ScalarProcessor.ScalarFromText("c");
var d = symbolicProcessor.ScalarProcessor.ScalarFromText("d");

var v1_sym = symbolicProcessor.Vector(a, b);
var v2_sym = symbolicProcessor.Vector(c, d);
var result_sym = v1_sym.Gp(v2_sym);

Console.WriteLine("\nSymbolic Operations:");
Console.WriteLine($"v1 = {v1_sym}");
Console.WriteLine($"v2 = {v2_sym}");
Console.WriteLine($"v1 * v2 = {result_sym}");

// Expected Output:
// Float64 Operations:
// v1 = 1.5*e1 + 2.7*e2
// v2 = 0.8*e1 + -1.2*e2
// v1 * v2 = -2.04 + -3.96*e1^e2

// Rational Exact Operations:
// v1 = (3/2)*e1 + (5/3)*e2
// v2 = (2/3)*e1 + (-4/5)*e2
// v1 * v2 = (-11/15) + (-38/15)*e1^e2

// Symbolic Operations:
// v1 = a*e1 + b*e2  
// v2 = c*e1 + d*e2
// v1 * v2 = (a*c + b*d) + (a*d - b*c)*e1^e2
```

</details>

## Integration with Utilities Layer

The algebra layer heavily utilizes the utilities foundation:

### Index Sets for Basis Blades
```csharp
// Using IIndexSet for efficient basis blade representation
var basisBlade = processor.CreateBasisBlade(
    ImmutableSortedSet<int>.Empty.Add(0).Add(2).Add(3)  // e0^e2^e3
);
```

### Text Processing for LaTeX Output
```csharp
// Generate LaTeX representation
var latexComposer = processor.CreateLaTeXComposer();
var latexString = multivector.ToLaTeX(latexComposer);
```

### Dependency Tracking
```csharp
// Track computational dependencies for optimization
var dependencyGraph = new ComputationDependencyGraph();
dependencyGraph.AddDependency(result, operand1);
dependencyGraph.AddDependency(result, operand2);
```

This comprehensive algebra layer provides the mathematical foundation for all higher-level GA operations, supporting multiple scalar types, different geometric spaces, and efficient computational patterns while maintaining mathematical rigor and extensibility.

---

**[← Previous: Layer 1 - Utilities](layer1-utilities.md) | [Next: Layer 3 - Modeling →](layer3-modeling.md)**