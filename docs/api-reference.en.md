---
layout: default
title: "API Reference"
lang: en
---

# GA-FuL API Reference

**[🇩🇪 Deutsche Version](api-reference.de.md)**

## Table of Contents

1. [Overview](#overview)
2. [Scalar Processors](#scalar-processors)
3. [GA Processors](#ga-processors)
4. [Multivectors](#multivectors)
5. [Basis Blades](#basis-blades)
6. [Conformal GA](#conformal-ga)
7. [Metaprogramming](#metaprogramming)
8. [Utilities](#utilities)

---

## Overview

This API reference provides an overview of the most important classes and methods in GA-FuL. For complete details, see the source code and IntelliSense documentation.

---

## Scalar Processors

### `IScalarProcessor<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.Scalars`

**Description:** Interface for scalar operations

**Important Methods:**

```csharp
public interface IScalarProcessor<T>
{
    // Arithmetic
    T Add(T a, T b);
    T Subtract(T a, T b);
    T Times(T a, T b);
    T Divide(T a, T b);
    T Negative(T a);
    T Power(T a, T b);

    // Trigonometry
    T Sin(T a);
    T Cos(T a);
    T Tan(T a);
    T ArcSin(T a);
    T ArcCos(T a);
    T ArcTan(T a);
    T ArcTan2(T y, T x);

    // Exponential and Logarithm
    T Exp(T a);
    T Log(T a);
    T Log10(T a);
    T Sqrt(T a);

    // Comparisons
    bool IsZero(T a);
    bool IsOne(T a);
    bool IsMinusOne(T a);
    bool IsPositive(T a);
    bool IsNegative(T a);

    // Constants
    T Zero { get; }
    T One { get; }
    T MinusOne { get; }
}
```

---

### `ScalarProcessorOfFloat64`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.Scalars`

**Description:** Scalar processor for 64-bit floating point numbers

**Usage:**

```csharp
// Singleton instance
var sp = ScalarProcessorOfFloat64.Instance;

// Operations
var result = sp.Add(1.0, 2.0);  // 3.0
var sin = sp.Sin(Math.PI / 2);   // 1.0
```

**Other Processors:**
- `ScalarProcessorOfFloat32`: 32-bit floats
- `ScalarProcessorOfDecimal`: Decimal
- `ScalarProcessorOfRational`: Rational numbers
- `ScalarProcessorOfComplex`: Complex numbers
- `ScalarProcessorOfMathematica`: Symbolic expressions

---

### `Scalar<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.Scalars`

**Description:** Thin wrapper for simpler scalar operations

**Properties:**

```csharp
public class Scalar<T>
{
    public IScalarProcessor<T> Processor { get; }
    public T Value { get; }
}
```

**Operators:**

```csharp
var sp = ScalarProcessorOfFloat64.Instance;
var a = new Scalar<double>(sp, 2.0);
var b = new Scalar<double>(sp, 3.0);

// Arithmetic operators
var sum = a + b;         // 5.0
var diff = a - b;        // -1.0
var product = a * b;     // 6.0
var quotient = a / b;    // 0.666...

// Comparisons
bool eq = a == b;        // false
bool lt = a < b;         // true
```

---

## GA Processors

### `XGaFloat64Processor` (Simplified Float64 API)

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors`

**Description:** Simplified Float64-specific GA processor (recommended for most users)

**Construction:**

```csharp
// Euclidean (all +1) - Most common
var processor = XGaFloat64Processor.Euclidean;

// Conformal (1 negative signature)
var conformal = XGaFloat64Processor.Conformal;

// Projective (1 zero signature)
var projective = XGaFloat64Processor.Projective;

// Custom metric
var custom = XGaFloat64Processor.Create(
    negativeCount: 1,  // Number of -1 squares
    zeroCount: 0       // Number of  0 squares
);
```

---

### `XGaProcessor<T>` (Generic API)

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended`

**Description:** Generic GA processor for custom scalar types (Rational, Symbolic, etc.)

**Construction:**

```csharp
// For non-Float64 types (e.g., Rational, Symbolic)
var scalarProcessor = ScalarProcessorOfRational.Instance;
var processor = XGaProcessor<Rational>.CreateEuclidean(scalarProcessor);
```

**Important Properties:**

```csharp
public class XGaProcessor<T>
{
    public IScalarProcessor<T> ScalarProcessor { get; }
    public XGaMetric Metric { get; }
    public int VSpaceDimensions { get; }
    public int GaSpaceDimensions { get; }  // 2^VSpaceDimensions
}
```

**Factory Methods:**

```csharp
// Scalars
XGaScalar<T> CreateScalar(T value);

// Vectors (using composer)
var vector = processor.CreateVectorComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, z)
    .GetVector();

// Basis vectors
XGaVector<T> CreateBasisVector(int index);

// Bivectors (using composer)
var bivector = processor.CreateBivectorComposer()
    .SetBivectorTerm(0, 1, xy)
    .SetBivectorTerm(0, 2, xz)
    .SetBivectorTerm(1, 2, yz)
    .GetBivector();

// Composer
XGaMultivectorComposer<T> CreateComposer();
```

---

## Multivectors

### `XGaMultivector<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended`

**Description:** Generic multivector

**Properties:**

```csharp
public class XGaMultivector<T>
{
    public XGaProcessor<T> Processor { get; }
    public IReadOnlyDictionary<int, XGaKVector<T>> KVectors { get; }

    // Grades
    public IEnumerable<int> Grades { get; }
    public int MinGrade { get; }
    public int MaxGrade { get; }

    // Tests
    public bool IsZero { get; }
    public bool IsScalar { get; }
    public bool IsVector { get; }
    public bool IsBivector { get; }
    public bool IsKVector { get; }
}
```

**Products:**

```csharp
// Geometric Product
XGaMultivector<T> Gp(XGaMultivector<T> other);

// Outer Product (Outer/Wedge)
XGaMultivector<T> Op(XGaMultivector<T> other);

// Left Contraction
XGaMultivector<T> Lcp(XGaMultivector<T> other);

// Right Contraction
XGaMultivector<T> Rcp(XGaMultivector<T> other);

// Scalar Product
Scalar<T> Sp(XGaMultivector<T> other);

// Fat Dot Product
XGaMultivector<T> Fdp(XGaMultivector<T> other);

// Hestenes Inner Product
XGaMultivector<T> Hip(XGaMultivector<T> other);

// Commutator Product
XGaMultivector<T> Cp(XGaMultivector<T> other);

// Anti-Commutator Product
XGaMultivector<T> Acp(XGaMultivector<T> other);
```

**Unary Operations:**

```csharp
// Reverse
XGaMultivector<T> Reverse();

// Grade Involution
XGaMultivector<T> GradeInvolution();

// Clifford Conjugate
XGaMultivector<T> CliffordConjugate();

// Dual
XGaMultivector<T> Dual();
XGaMultivector<T> UnDual();

// Negation
XGaMultivector<T> Negative();

// Magnitude
Scalar<T> Norm();
Scalar<T> NormSquared();

// Normalization
XGaMultivector<T> Normalize();
XGaMultivector<T> DivideByNorm();

// Inverse
XGaMultivector<T> Inverse();

// Exponential and Logarithm
XGaMultivector<T> Exp();
XGaMultivector<T> Log();
```

**Grade Extraction:**

```csharp
// Specific grade
XGaKVector<T> GetKVectorPart(int grade);

// Scalar part
XGaScalar<T> GetScalarPart();

// Vector part
XGaVector<T> GetVectorPart();

// Bivector part
XGaBivector<T> GetBivectorPart();

// Only even/odd grades
XGaMultivector<T> GetEvenPart();
XGaMultivector<T> GetOddPart();
```

**Arithmetic:**

```csharp
// Addition
XGaMultivector<T> Add(XGaMultivector<T> other);

// Subtraction
XGaMultivector<T> Subtract(XGaMultivector<T> other);

// Scalar multiplication
XGaMultivector<T> Times(T scalar);
XGaMultivector<T> Divide(T scalar);

// Operators
var sum = mv1 + mv2;
var diff = mv1 - mv2;
var scaled = 2.0 * mv1;
```

---

### `XGaMultivectorComposer<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Composers`

**Description:** Builder for multivectors

**Usage:**

```csharp
var composer = processor.CreateMultivectorComposer();

// Add terms
composer.SetTerm(indexSet, scalar);
composer.AddTerm(indexSet, scalar);

// Add basis blades
composer.SetBasisBladeScalar(id, scalar);
composer.AddBasisBladeScalar(id, scalar);

// Special terms
composer.SetScalarTerm(scalar);
composer.SetVectorTerm(index, scalar);
composer.SetBivectorTerm(index1, index2, scalar);

// Create multivector
var multivector = composer.GetMultivector();

// Or specific type
var scalar = composer.GetScalar();
var vector = composer.GetVector();
var bivector = composer.GetBivector();
var kVector = composer.GetKVector(grade);
```

---

## Basis Blades

### `XGaBasisBlade`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Basis`

**Description:** Represents a basis blade

**Properties:**

```csharp
public class XGaBasisBlade
{
    public IIndexSet IndexSet { get; }
    public int Grade { get; }
    public ulong Id { get; }  // For grade < 64
}
```

**Construction:**

```csharp
// Via processor
var e1 = processor.CreateBasisVector(0);
var e2 = processor.CreateBasisVector(1);
var e12 = processor.CreateBasisBivector(0, 1);

// Direct
var blade = new XGaBasisBlade(indexSet);
```

**Operations:**

```csharp
// Products
XGaSignedBasisBlade Gp(XGaBasisBlade other, XGaMetric metric);
XGaSignedBasisBlade Op(XGaBasisBlade other);
XGaSignedBasisBlade Lcp(XGaBasisBlade other, XGaMetric metric);

// Unary operations
XGaBasisBlade Reverse();
XGaBasisBlade GradeInvolution();
```

---

### `IIndexSet`

**Namespace:** `GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets`

**Description:** Interface for index sets

**Methods:**

```csharp
public interface IIndexSet
{
    int Count { get; }
    bool Contains(int index);
    IEnumerable<int> GetIndices();
    bool IsSubsetOf(IIndexSet other);
    bool Overlaps(IIndexSet other);

    IIndexSet Union(IIndexSet other);
    IIndexSet Intersect(IIndexSet other);
    IIndexSet Except(IIndexSet other);
}
```

---

## Conformal GA

### `CGaGeometricSpace5D<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa`

**Description:** 5D Conformal GA for 3D geometry

**Construction:**

```csharp
// For Float64 (recommended)
var cga = CGaFloat64GeometricSpace5D.Instance;

// For generic types
var cga = CGaGeometricSpace5D<T>.Create(scalarProcessor);
```

**Encoding:**

```csharp
// IPNS Round Objects (Inner Product Null Space)
var point = cga.Encode.IpnsRound.Point(x, y, z);
var sphere = cga.Encode.IpnsRound.RealSphere(radius, cx, cy, cz);
var pointPair = cga.Encode.IpnsRound.PointPair(p1, p2);
var circle = cga.Encode.IpnsRound.Circle(center, radius, bivector);

// OPNS Flat Objects (Outer Product Null Space)
var line = cga.Encode.OpnsFlat.Line(point, direction);
var plane = cga.Encode.OpnsFlat.Plane(point, bivector);
var flatPoint = cga.Encode.OpnsFlat.Point(x, y, z);

// IPNS Flat Objects
var ipnsLine = cga.Encode.IpnsFlat.Line(point, direction);
var ipnsPlane = cga.Encode.IpnsFlat.Plane(point, normal);
```

**Decoding:**

```csharp
// Decode IPNS Round
var decoded = blade.DecodeIpnsRound.Element();

// Extract components
var center = decoded.CenterToVector3D();
var radius = decoded.RealRadius;
var direction = decoded.DirectionToVector3D();
var normal = decoded.NormalDirectionToVector3D();
var bivector = decoded.DirectionToBivector3D();

// Decode OPNS Flat
var decodedFlat = blade.DecodeOpnsFlat.Element();
var point = decodedFlat.PositionToVector3D();
var direction = decodedFlat.DirectionToVector3D();
```

**Transformations:**

```csharp
// Translation
var translator = cga.CreateTranslator(translationVector);
var translated = translator.OmMap(object);

// Rotation
var rotor = cga.CreateRotor(bivector, angle);
var rotated = rotor.OmMap(object);

// Dilation (Scaling)
var dilator = cga.CreateDilator(scale);
var scaled = dilator.OmMap(object);

// Inversion
var inverter = cga.CreateInverter(sphere);
var inverted = inverter.OmMap(object);

// Combined transformation
var combined = translator.Gp(rotor).Gp(dilator);
var transformed = combined.OmMap(object);
```

**Geometric Operations:**

```csharp
// Intersection
var intersection = object1.Op(object2);

// Projection
var projected = object1.ProjectOn(object2);

// Reflection
var reflected = object.ReflectOn(plane);

// Dualization
var dual = object.Dual();
```

---

## Metaprogramming

### `MetaContext`

**Namespace:** `GeometricAlgebraFulcrumLib.MetaProgramming.Context`

**Description:** Context for code generation

**Usage:**

```csharp
// 1. Create context
var context = new MetaContext();

// 2. Define parameters
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");

// 3. Constants
var pi = context.CreateConstant("pi", Math.PI);

// 4. Computations
var scalarProcessor = context.ScalarProcessor;
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);
var v = processor.CreateVectorComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, 0)
    .GetVector();
var result = v.NormSquared();

// 5. Define output
context.SetOutputVariable("result", result);

// 6. Optimize
context.OptimizeContext();

// 7. Generate code
var codeComposer = new CSharpCodeComposer();
var code = codeComposer.GenerateCode(context);
```

**Optimizations:**

```csharp
// Constant propagation
context.EnableConstantPropagation();

// Common Subexpression Elimination
context.EnableCommonSubexpressionElimination();

// Symbolic simplification (requires CAS)
context.EnableSymbolicSimplification();

// Genetic programming
context.EnableGeneticOptimization(
    populationSize: 100,
    generations: 50
);

// All optimizations
context.OptimizeContext();
```

---

### `IMetaExpression`

**Namespace:** `GeometricAlgebraFulcrumLib.MetaProgramming.Expressions`

**Description:** Interface for meta-expressions

**Hierarchy:**

```csharp
IMetaExpression
├── IMetaExpressionAtomic
│   ├── MetaExpressionNumber        // Constant
│   └── MetaExpressionVariable      // Variable/Parameter
└── IMetaExpressionComposite
    ├── MetaExpressionNegative      // -a
    ├── MetaExpressionAdd           // a + b
    ├── MetaExpressionSubtract      // a - b
    ├── MetaExpressionTimes         // a * b
    ├── MetaExpressionDivide        // a / b
    ├── MetaExpressionPower         // a^b
    └── MetaExpressionFunction      // sin(a), cos(a), etc.
```

---

### Code Composers

**Namespace:** `GeometricAlgebraFulcrumLib.MetaProgramming.Composers`

**Available Composers:**

```csharp
// C#
var csharpComposer = new CSharpCodeComposer();

// C++
var cppComposer = new CppCodeComposer();

// JavaScript
var jsComposer = new JavaScriptCodeComposer();

// Python
var pythonComposer = new PythonCodeComposer();

// MATLAB
var matlabComposer = new MatlabCodeComposer();
```

**Configuration:**

```csharp
composer.SetMethodName("ComputeResult");
composer.SetClassName("MyClass");
composer.SetNamespace("MyNamespace");

composer.SetIndentation("    ");  // 4 Spaces
composer.SetLineEnding("\n");

var code = composer.GenerateCode(context);
```

---

## Utilities

### Linear Algebra Utilities

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra`

**Vectors (Float64):**

```csharp
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space4D;

// 2D vector
var v2d = LinFloat64Vector2D.Create(x, y);

// 3D vector
var v3d = LinFloat64Vector3D.Create(x, y, z);

// 4D vector
var v4d = LinFloat64Vector4D.Create(x, y, z, w);

// n-D vector (generic Float64)
var vnd = Float64Vector.Create(component1, component2, component3, ...);
```

**Vectors (Generic):**

```csharp
// For non-Float64 types (Rational, Symbolic, etc.)
var v2d = scalarProcessor.CreateVector2D(x, y);
var v3d = scalarProcessor.CreateVector3D(x, y, z);
var v4d = scalarProcessor.CreateVector4D(x, y, z, w);
```

**Matrices:**

```csharp
// Create matrix
var matrix = scalarProcessor.CreateMatrix(rows, columns);

// Matrix operations
var product = matrix1.Multiply(matrix2);
var transpose = matrix.Transpose();
var inverse = matrix.Inverse();
var determinant = matrix.Determinant();
```

---

### Text Utilities

**Namespace:** `GeometricAlgebraFulcrumLib.Utilities.Text`

**Linear Text Composer:**

```csharp
var composer = new LinearTextComposer();

composer.AppendLine("Hello");
composer.AppendLine("World");
composer.AppendAtNewLine("New line");

var text = composer.ToString();
```

**Parametric Text Composer:**

```csharp
var template = "Hello #name#, you are #age# years old.";
var composer = new ParametricTextComposer("#", "#", template);

composer["name"] = "Alice";
composer["age"] = "30";

var result = composer.GenerateText();
// "Hello Alice, you are 30 years old."
```

---

## Summary

This API reference provides an overview of the most important classes and methods. For:

- **Complete API details**: See IntelliSense and XML comments in the code
- **Usage examples**: See [examples.md](examples.en.md)
- **Architecture**: See [architecture.md](architecture.en.md)
- **Getting Started**: See [getting-started.md](getting-started.en.md)

> **💡 Performance Tip:** When choosing between `XGaFloat64Processor` and `XGaProcessor<double>`, prefer Generic<T> for **27% faster** performance. See [Getting Started Guide](getting-started.en.md#performance-considerations) for details.

---

**Last Updated**: 2025-10-27 | **API**: Complete | **Performance**: Generic<T> 1.24-2.31x faster 🚀

[← Back to Main Documentation](README.en.md)
