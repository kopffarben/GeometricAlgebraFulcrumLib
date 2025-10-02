---
layout: default
title: "Examples"
lang: en
---

# GA-FuL Usage Examples

**[🇩🇪 Deutsche Version](examples.de.md)**

---

## Table of Contents

1. [Basic GA Operations](#basic-ga-operations)
2. [3D Conformal Geometric Algebra](#3d-conformal-geometric-algebra)
3. [Rotations and Transformations](#rotations-and-transformations)
4. [Symbolic Computations](#symbolic-computations)
5. [Code Generation](#code-generation)
6. [Advanced Examples](#advanced-examples)

---

## Basic GA Operations

### Example 1: Vector Products in 3D

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

// Setup
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.Create(scalarProcessor, 3, 0, 0);

// Define two vectors
var v1 = processor.CreateVector(1, 0, 0);  // x-axis
var v2 = processor.CreateVector(0, 1, 0);  // y-axis

// Geometric product: v1 * v2 = v1·v2 + v1∧v2
var gp = v1.Gp(v2);
Console.WriteLine($"Geometric Product: {gp}");
// Output: '1'<1,2> (xy-bivector)

// Outer product (Wedge): v1 ∧ v2
var wedge = v1.Op(v2);
Console.WriteLine($"Outer Product: {wedge}");
// Output: '1'<1,2> (xy-bivector)

// Scalar product: v1·v2
var dot = v1.Sp(v2);
Console.WriteLine($"Scalar Product: {dot}");
// Output: 0 (orthogonal)

// Non-orthogonal vectors
var v3 = processor.CreateVector(1, 1, 0);
var dot2 = v1.Sp(v3);
Console.WriteLine($"Scalar Product v1·v3: {dot2}");
// Output: 1
```

---

### Example 2: Cross Product via Duality

The cross product in 3D can be elegantly expressed through GA:

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

var a = processor.CreateVector(1, 0, 0);
var b = processor.CreateVector(0, 1, 0);

// Cross product: a × b = -(a ∧ b) * I^{-1}
// where I is the pseudoscalar

// Method 1: Via bivector and dual
var bivector = a.Op(b);        // a ∧ b
var cross = bivector.Dual();   // Dualization

Console.WriteLine($"Cross Product a × b: {cross}");
// Output: '1'<3> (z-vector)

// Method 2: Direct
var cross2 = a.Cross(b);
Console.WriteLine($"Cross Product (direct): {cross2}");
```

---

### Example 3: Rotors and Rotations

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Define rotation plane (bivector)
var B = processor.CreateBivector(
    xy: 1,  // Rotation in xy-plane
    xz: 0,
    yz: 0
).Normalize();

// Rotation angle
var angle = Math.PI / 2;  // 90°

// Create rotor: R = exp(-θ/2 B)
var rotor = processor.CreateRotor(B, angle);

// Vector to rotate
var v = processor.CreateVector(1, 0, 0);

// Rotation: v' = R v R†
var vRotated = rotor.OmMap(v);

Console.WriteLine($"Original: {v}");
Console.WriteLine($"Rotated (90° in xy): {vRotated}");
// Output: (0, 1, 0) - now along y-axis
```

---

## 3D Conformal Geometric Algebra

### Example 4: Points, Lines, Planes

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaGeometricSpace5D<double>.Create(scalarProcessor);

// Encode 3D points in 5D CGA
var p1 = cga.EncodeIpnsRound.Point(0, 0, 0);
var p2 = cga.EncodeIpnsRound.Point(1, 0, 0);
var p3 = cga.EncodeIpnsRound.Point(0, 1, 0);

// Plane through three points: π = p1 ∧ p2 ∧ p3
var plane = p1.Op(p2).Op(p3);

Console.WriteLine($"Plane through three points: {plane}");

// Encode a line
var linePoint = cga.EncodeIpnsRound.Point(0, 0, 0);
var lineDirection = scalarProcessor.Vector3D(1, 1, 1);
var line = cga.EncodeOpnsFlat.Line(linePoint, lineDirection);

Console.WriteLine($"Line: {line}");

// Intersect line with plane
var intersection = line.Op(plane);

Console.WriteLine($"Intersection point: {intersection}");
```

---

### Example 5: Spheres and Circles

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaGeometricSpace5D<double>.Create(scalarProcessor);

// Define sphere
var sphere = cga.EncodeIpnsRound.Sphere(
    centerX: 0,
    centerY: 0,
    centerZ: 0,
    radius: 1
);

// Define plane (z = 0)
var p1 = cga.EncodeIpnsRound.Point(-1, -1, 0);
var p2 = cga.EncodeIpnsRound.Point(1, -1, 0);
var p3 = cga.EncodeIpnsRound.Point(0, 1, 0);
var plane = p1.Op(p2).Op(p3);

// Intersect sphere with plane → circle
var circle = sphere.Op(plane);

// Decode circle
var circleData = circle.Decode.OpnsRound.Element();

Console.WriteLine($"Circle Center: {circleData.CenterToVector3D()}");
Console.WriteLine($"Circle Radius: {circleData.RealRadius}");
Console.WriteLine($"Circle Normal: {circleData.NormalDirectionToVector3D()}");

// Output:
// Circle Center: (0, 0, 0)
// Circle Radius: 1
// Circle Normal: (0, 0, 1)
```

---

### Example 6: Transformations in CGA

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaGeometricSpace5D<double>.Create(scalarProcessor);

// Define a point
var point = cga.EncodeIpnsRound.Point(1, 0, 0);

// Translation
var translationVector = scalarProcessor.CreateVector3D(1, 2, 3);
var translator = cga.CreateTranslator(translationVector);
var translatedPoint = translator.OmMap(point);

Console.WriteLine($"Original: {point.DecodeIpnsRound.Point()}");
Console.WriteLine($"Translated: {translatedPoint.DecodeIpnsRound.Point()}");

// Rotation
var angle = Math.PI / 4;  // 45°
var bivector = processor.CreateBivector(xy: 1, xz: 0, yz: 0);
var rotator = cga.CreateRotor(bivector, angle);
var rotatedPoint = rotator.OmMap(point);

Console.WriteLine($"Rotated: {rotatedPoint.DecodeIpnsRound.Point()}");

// Scaling (Dilation)
var scale = 2.0;
var dilator = cga.CreateDilator(scale);
var scaledPoint = dilator.OmMap(point);

Console.WriteLine($"Scaled: {scaledPoint.DecodeIpnsRound.Point()}");

// Combined transformation
var combined = translator.Gp(rotator).Gp(dilator);
var transformedPoint = combined.OmMap(point);

Console.WriteLine($"Combined transform: {transformedPoint.DecodeIpnsRound.Point()}");
```

---

## Rotations and Transformations

### Example 7: Rotation Around Arbitrary Axis

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Rotation axis (must be normalized)
var axis = processor.CreateVector(1, 1, 1).Normalize();

// Rotation angle
var angle = Math.PI / 3;  // 60°

// Create rotor for rotation around axis
// R = exp(-θ/2 n)
// where n is the bivector normal to the rotation axis

// Method: Use Rodrigues formula in GA
var halfAngle = angle / 2;
var rotor = processor.CreateScalar(Math.Cos(halfAngle))
    .Add(axis.Dual().Times(-Math.Sin(halfAngle)));

// Vector to rotate
var v = processor.CreateVector(1, 0, 0);

// Rotation: v' = R v R†
var vRotated = rotor.Gp(v).Gp(rotor.Reverse());

Console.WriteLine($"Original: {v}");
Console.WriteLine($"Rotated around (1,1,1) by 60°: {vRotated}");
```

---

### Example 8: Reflection at Plane

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Normal of reflection plane (e.g. xy-plane, normal = z-axis)
var normal = processor.CreateVector(0, 0, 1);

// Vector to reflect
var v = processor.CreateVector(1, 1, 1);

// Reflection: v' = -n v n / (n·n)
var reflected = normal.Gp(v).Gp(normal).Negative().DivideByNorm();

Console.WriteLine($"Original: {v}");
Console.WriteLine($"Reflected at xy-plane: {reflected}");
// Output: (1, 1, -1)
```

---

## Symbolic Computations

### Example 9: Symbolic GA with Mathematica

```csharp
using GeometricAlgebraFulcrumLib.Mathematica;
using Wolfram.NETLink;

// Initialize Mathematica
var kernel = MathematicaInterface.DefaultKernel;
var scalarProcessor = ScalarProcessorOfMathematica.Instance;
var processor = XGaProcessor<Expr>.Create(scalarProcessor);

// Create symbolic parameters
var x = scalarProcessor.CreateSymbol("x");
var y = scalarProcessor.CreateSymbol("y");
var z = scalarProcessor.CreateSymbol("z");

// Vector with symbolic components
var v = processor.CreateVector(x, y, z);

// Compute norm^2
var normSq = v.NormSquared();

Console.WriteLine($"||v||^2 = {normSq}");
// Output: x^2 + y^2 + z^2

// Geometric product with itself
var vv = v.Gp(v);

Console.WriteLine($"v * v = {vv}");
// Output: x^2 + y^2 + z^2 (scalar)

// Derivative with respect to x
var dvdx = scalarProcessor.Differentiate(normSq, "x");

Console.WriteLine($"d(||v||^2)/dx = {dvdx}");
// Output: 2*x
```

---

### Example 10: Symbolic Simplification

```csharp
var scalarProcessor = ScalarProcessorOfMathematica.Instance;
var processor = XGaProcessor<Expr>.Create(scalarProcessor);

// Define symbolic vectors
var a = processor.CreateVector(
    scalarProcessor.CreateSymbol("a1"),
    scalarProcessor.CreateSymbol("a2"),
    scalarProcessor.CreateSymbol("a3")
);

var b = processor.CreateVector(
    scalarProcessor.CreateSymbol("b1"),
    scalarProcessor.CreateSymbol("b2"),
    scalarProcessor.CreateSymbol("b3")
);

// Compute (a + b) · (a + b)
var sum = a.Add(b);
var dotProduct = sum.Sp(sum);

// Simplify
var simplified = scalarProcessor.Simplify(dotProduct);

Console.WriteLine($"(a + b) · (a + b) = {simplified}");
// Output: a1^2 + 2*a1*b1 + b1^2 + a2^2 + 2*a2*b2 + b2^2 + a3^2 + 2*a3*b3 + b3^2
```

---

## Code Generation

### Example 11: C# Code Generation

```csharp
using GeometricAlgebraFulcrumLib.MetaProgramming;

// 1. Create meta context
var context = new MetaContext();
var scalarProcessor = context.ScalarProcessor;

// 2. Define input parameters
var x1 = context.CreateParameter("x1");
var x2 = context.CreateParameter("x2");
var x3 = context.CreateParameter("x3");

var y1 = context.CreateParameter("y1");
var y2 = context.CreateParameter("y2");
var y3 = context.CreateParameter("y3");

// 3. GA computations
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);

var v1 = processor.CreateVector(x1, x2, x3);
var v2 = processor.CreateVector(y1, y2, y3);

// Cross product
var cross = v1.Cross(v2);

// 4. Define output variables
var crossComponents = cross.GetVectorComponents();
context.SetOutputVariable("result_x", crossComponents[0]);
context.SetOutputVariable("result_y", crossComponents[1]);
context.SetOutputVariable("result_z", crossComponents[2]);

// 5. Optimize
context.OptimizeContext();

// 6. Generate C# code
var codeComposer = new CSharpCodeComposer();
codeComposer.SetMethodName("CrossProduct");
var code = codeComposer.GenerateCode(context);

Console.WriteLine(code);
```

**Generated Code:**
```csharp
public static void CrossProduct(
    double x1, double x2, double x3,
    double y1, double y2, double y3,
    out double result_x,
    out double result_y,
    out double result_z
)
{
    // temp variables
    var temp_0 = x2 * y3;
    var temp_1 = x3 * y2;
    var temp_2 = x1 * y3;
    var temp_3 = x3 * y1;
    var temp_4 = x1 * y2;
    var temp_5 = x2 * y1;

    // output variables
    result_x = temp_0 - temp_1;
    result_y = temp_3 - temp_2;
    result_z = temp_4 - temp_5;
}
```

---

### Example 12: C++ Code Generation

```csharp
var context = new MetaContext();
var scalarProcessor = context.ScalarProcessor;

// Parameters
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");

// Computation
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);
var v = processor.CreateVector(x, y, 0);
var normSq = v.NormSquared();

// Output
context.SetOutputVariable("result", normSq);
context.OptimizeContext();

// C++ code
var cppComposer = new CppCodeComposer();
cppComposer.SetFunctionName("ComputeNormSquared");
var code = cppComposer.GenerateCode(context);

Console.WriteLine(code);
```

**Generated C++ Code:**
```cpp
void ComputeNormSquared(double x, double y, double& result) {
    // temp variables
    double temp_0 = x * x;
    double temp_1 = y * y;

    // output
    result = temp_0 + temp_1;
}
```

---

## Advanced Examples

### Example 13: Projective Geometry

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.PGa;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var pga = XGaProjectiveSpace<double>.Create(scalarProcessor, 3);

// Points in projective coordinates
var p1 = pga.EncodePoint(1, 0, 0);
var p2 = pga.EncodePoint(0, 1, 0);

// Line through two points
var line = p1.Op(p2);

// Plane
var p3 = pga.EncodePoint(0, 0, 1);
var plane = p1.Op(p2).Op(p3);

Console.WriteLine($"Line: {line}");
Console.WriteLine($"Plane: {plane}");
```

---

### Example 14: Quaternions in GA

Quaternions can be represented as even-grade multivectors in 3D GA:

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Quaternion: q = a + b*i + c*j + d*k
// In GA: q = a + b*e23 + c*e31 + d*e12

var a = 1.0;
var b = 0.0;
var c = 0.0;
var d = 1.0;

var quaternion = processor.CreateComposer()
    .SetTerm(0, a)                    // Scalar
    .SetBivectorTerm(1, 2, b)         // e_yz (corresponds to i)
    .SetBivectorTerm(2, 0, c)         // e_zx (corresponds to j)
    .SetBivectorTerm(0, 1, d)         // e_xy (corresponds to k)
    .GetMultivector();

Console.WriteLine($"Quaternion in GA: {quaternion}");

// Quaternion multiplication = Geometric product
var q1 = quaternion;
var q2 = processor.CreateComposer()
    .SetTerm(0, 0.707)
    .SetBivectorTerm(0, 1, 0.707)
    .GetMultivector();

var product = q1.Gp(q2);

Console.WriteLine($"Quaternion Product: {product}");
```

---

### Example 15: Plücker Coordinates for Lines

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Line defined by two points
var p1 = processor.CreateVector(0, 0, 0);
var p2 = processor.CreateVector(1, 1, 0);

// Plücker coordinates: L = p1 ∧ p2
var lineDirection = p2.Subtract(p1);
var lineMoment = p1.Op(p2);

Console.WriteLine($"Line Direction: {lineDirection}");
Console.WriteLine($"Line Moment: {lineMoment}");

// Line bivector (6D representation)
var lineBivector = lineMoment.Add(lineDirection.Dual());

Console.WriteLine($"Line Bivector: {lineBivector}");
```

---

### Example 16: Interpolation with Rotors

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Define two rotors
var B1 = processor.CreateBivector(xy: 1, xz: 0, yz: 0).Normalize();
var rotor1 = processor.CreateRotor(B1, 0);  // Identity

var B2 = processor.CreateBivector(xy: 1, xz: 0, yz: 0).Normalize();
var rotor2 = processor.CreateRotor(B2, Math.PI / 2);  // 90°

// Interpolation (SLERP in GA)
var t = 0.5;  // 50% between rotor1 and rotor2

var logRotor = rotor2.Gp(rotor1.Reverse()).Log();
var interpolated = (logRotor.Times(t)).Exp().Gp(rotor1);

// Apply interpolated rotor
var v = processor.CreateVector(1, 0, 0);
var result = interpolated.OmMap(v);

Console.WriteLine($"Interpolated (t=0.5): {result}");
// Result: 45° rotation
```

---

### Example 17: Möbius Transformation in CGA

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaGeometricSpace5D<double>.Create(scalarProcessor);

// Point in 3D
var point = cga.EncodeIpnsRound.Point(1, 0, 0);

// Inversion sphere (origin, radius 1)
var sphere = cga.EncodeIpnsRound.Sphere(0, 0, 0, 1);

// Inversion at point
// p' = sphere * p * sphere / |sphere|^2
var inverted = sphere.Gp(point).Gp(sphere).DivideByNorm();

var invertedPoint = inverted.DecodeIpnsRound.Point();

Console.WriteLine($"Original: (1, 0, 0)");
Console.WriteLine($"Inverted: {invertedPoint}");
// Output: (1, 0, 0) - point lies on inversion sphere
```

---

### Example 18: Signal Processing with GA

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

// Create signal processor
var signalProcessor = ScalarProcessorOfSignal.Create(sampleRate: 44100);

// Generate signal vectors
var signal1 = signalProcessor.CreateSineWave(frequency: 440, duration: 1.0);
var signal2 = signalProcessor.CreateSineWave(frequency: 880, duration: 1.0);

var processor = XGaProcessor<Signal>.Create(signalProcessor);

var v1 = processor.CreateVector(signal1, signal2);

// GA operations on signals
var norm = v1.Norm();

Console.WriteLine($"Signal Norm: {norm}");
```

---

## Summary

These examples demonstrate the versatility of GA-FuL:

✓ **Basic GA Operations**: Products, duality
✓ **3D Geometry**: CGA for points, lines, planes, spheres
✓ **Transformations**: Rotations, reflections, translations
✓ **Symbolic Mathematics**: Mathematica integration
✓ **Code Generation**: Optimized code in C++, C#, etc.
✓ **Advanced Applications**: Quaternions, Plücker coordinates, signal processing

For more examples, see the projects in the repository:
- `GeometricAlgebraFulcrumLib.Applications`
- `GeometricAlgebraFulcrumLib.Samples.Generations`
- `GeometricAlgebraFulcrumLib.UnitTests`

---

[← Back to Main Documentation](README.en.md)
