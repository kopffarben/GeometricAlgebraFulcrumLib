# API Reference

This document provides a reference for the key classes and interfaces in GA-FuL.

## Core Interfaces

### IScalarProcessor&lt;T&gt;

The fundamental interface for all scalar operations in GA-FuL.

```csharp
public interface IScalarProcessor<T>
{
    // Constants
    T ZeroValue { get; }
    T OneValue { get; }
    T MinusOneValue { get; }
    
    // Basic Operations
    T Add(T scalar1, T scalar2);
    T Multiply(T scalar1, T scalar2);
    T Divide(T scalar1, T scalar2);
    T Negative(T scalar);
    
    // Mathematical Functions
    T Cos(T scalar);
    T Sin(T scalar);
    T Sqrt(T scalar);
    T Power(T baseScalar, T scalar);
    
    // Validation
    bool IsZero(T scalar);
    bool IsValid(T scalar);
}
```

### XGaProcessor&lt;T&gt;

Main processor for Geometric Algebra operations.

```csharp
public abstract class XGaProcessor<T>
{
    // Factory Methods
    public static XGaEuclideanProcessor<T> CreateEuclidean(IScalarProcessor<T> scalarProcessor);
    public static XGaConformalProcessor<T> CreateConformal(IScalarProcessor<T> scalarProcessor);
    
    // Vector Creation
    public XGaVector<T> CreateVector(params T[] coordinates);
    public XGaVector<T> CreateParameterVector(params string[] coordinateNames);
    
    // Multivector Creation
    public XGaComposer<T> CreateComposer();
    public XGaMultivector<T> CreateZero();
    public XGaScalar<T> CreateScalar(T value);
}
```

### XGaMultivector&lt;T&gt;

Base class for all multivector types.

```csharp
public abstract class XGaMultivector<T>
{
    // Properties
    public XGaProcessor<T> Processor { get; }
    public int Grade { get; }
    public int Count { get; }
    public bool IsZero { get; }
    
    // Operations
    public XGaMultivector<T> Add(XGaMultivector<T> mv2);
    public XGaMultivector<T> Gp(XGaMultivector<T> mv2);    // Geometric Product
    public XGaMultivector<T> Op(XGaMultivector<T> mv2);    // Outer Product
    public XGaScalar<T> Sp(XGaMultivector<T> mv2);         // Scalar Product
    
    // Transformations
    public XGaMultivector<T> Reverse();
    public XGaMultivector<T> Conjugate();
    public XGaScalar<T> Norm();
    public XGaScalar<T> NormSquared();
}
```

## Scalar Processors

### ScalarProcessorOfFloat64

High-performance double precision scalar processor.

```csharp
public static class ScalarProcessorOfFloat64
{
    public static ScalarProcessorOfFloat64 Instance { get; }
    
    // Conversion Methods
    public double ScalarFromNumber(double value);
    public double ScalarFromText(string text);
    public double ScalarFromRational(int numerator, int denominator);
}
```

### ScalarProcessorOfComplex

Complex number scalar processor.

```csharp
public static class ScalarProcessorOfComplex
{
    public static ScalarProcessorOfComplex Instance { get; }
    
    // Complex Number Creation
    public Complex ScalarFromNumbers(double real, double imaginary);
    public Complex ScalarFromPolar(double magnitude, double phase);
}
```

## Multivector Hierarchy

### XGaScalar&lt;T&gt;

Grade-0 multivector (scalar).

```csharp
public sealed class XGaScalar<T> : XGaMultivector<T>
{
    public T ScalarValue { get; }
    
    // Conversion
    public static implicit operator T(XGaScalar<T> scalar);
}
```

### XGaVector&lt;T&gt;

Grade-1 multivector (vector).

```csharp
public sealed class XGaVector<T> : XGaMultivector<T>
{
    public T this[int index] { get; }
    public int VSpaceDimensions { get; }
    
    // Vector-specific Operations
    public XGaBivector<T> Op(XGaVector<T> vector2);
    public XGaVector<T> ReflectOn(XGaVector<T> mirror);
    public XGaVector<T> ProjectOn(XGaVector<T> subspace);
}
```

### XGaBivector&lt;T&gt;

Grade-2 multivector (bivector).

```csharp
public sealed class XGaBivector<T> : XGaMultivector<T>
{
    public T this[int index1, int index2] { get; }
    
    // Bivector-specific Operations
    public XGaRotor<T> Exp();              // Exponential → Rotor
    public XGaVector<T> GetDualVector();   // Hodge dual in 3D
}
```

## Conformal Geometric Algebra

### CGaFloat64GeometricSpace5D

5D CGA space for 3D geometry.

```csharp
public class CGaFloat64GeometricSpace5D
{
    public static CGaFloat64GeometricSpace5D Create(IScalarProcessor<double> processor);
    
    // Point Encoding
    public XGaVector<double> EncodeOpnsRoundPoint(double x, double y, double z);
    public XGaVector<double> EncodeIpnsRoundPoint(double x, double y, double z);
    
    // Sphere Encoding  
    public XGaMultivector<double> EncodeOpnsRoundSphere(double cx, double cy, double cz, double radius);
    public XGaMultivector<double> EncodeIpnsRoundSphere(double cx, double cy, double cz, double radius);
    
    // Plane Encoding
    public XGaMultivector<double> EncodeOpnsFlatPlane(double a, double b, double c, double d);
    public XGaMultivector<double> EncodeIpnsFlatPlane(double a, double b, double c, double d);
    
    // Line Encoding
    public XGaMultivector<double> EncodeOpnsFlatLine(Vector3D<double> point, Vector3D<double> direction);
    
    // Decoding
    public CGaFloat64Element DecodeOpnsRoundElement(XGaMultivector<double> cgaMv);
    public CGaFloat64Element DecodeIpnsRoundElement(XGaMultivector<double> cgaMv);
}
```

### CGaFloat64Element

Decoded CGA geometric element.

```csharp
public class CGaFloat64Element
{
    public CGaElementKind Kind { get; }              // Point, Line, Plane, Circle, Sphere
    public CGaElementSpecs Specs { get; }            // Round/Flat, Opns/Ipns
    
    // Properties (when applicable)
    public Vector3D<double> Center { get; }
    public double Radius { get; }
    public Vector3D<double> Normal { get; }
    public Vector3D<double> Direction { get; }
    
    // Validation
    public bool IsValid { get; }
    public bool IsRound { get; }
    public bool IsFlat { get; }
}
```

## MetaProgramming

### MetaContext

Context for symbolic expression building and code generation.

```csharp
public class MetaContext
{
    public MetaContextOptions ContextOptions { get; set; }
    public bool MergeExpressions { get; set; }
    
    // Parameter Creation
    public IMetaExpression CreateParameter(string name);
    public IMetaExpression CreateLiteral(double value);
    public IMetaExpression CreateSymbol(string name, IMetaExpression expr);
    
    // GA Processor Creation
    public XGaProcessor<IMetaExpression> CreateEuclideanXGaProcessor();
    public XGaProcessor<IMetaExpression> CreateConformalXGaProcessor();
    
    // Optimization and Code Generation
    public void OptimizeContext();
    public void SetComputedExternalNamesByOrder(Func<int, string> nameGenerator);
    public CSharpCodeComposer CreateCSharpCodeComposer();
    public CppCodeComposer CreateCppCodeComposer();
    public PythonCodeComposer CreatePythonCodeComposer();
}
```

### IMetaExpression

Interface for symbolic expressions.

```csharp
public interface IMetaExpression
{
    // Properties
    bool IsZero { get; }
    bool IsConstant { get; }
    string ExpressionText { get; }
    
    // Arithmetic Operations
    IMetaExpression Add(IMetaExpression expr2);
    IMetaExpression Multiply(IMetaExpression expr2);
    IMetaExpression Divide(IMetaExpression expr2);
    IMetaExpression Negative();
    
    // Mathematical Functions
    IMetaExpression Sin();
    IMetaExpression Cos();
    IMetaExpression Sqrt();
    IMetaExpression Power(IMetaExpression exponent);
    
    // Output Management
    void SetAsOutput(string outputName);
    void SetExternalName(string externalName);
}
```

## Visualization

### GrBabylonJsScene

Babylon.js scene composer for 3D visualization.

```csharp
public class GrBabylonJsScene
{
    // Camera Management
    public GrBabylonJsCamera AddArcRotateCamera(string name, double alpha, double beta, double radius, Vector3D target);
    public GrBabylonJsFreeCamera AddFreeCamera(string name, Vector3D position);
    
    // Lighting
    public GrBabylonJsLight AddHemisphericLight(string name, Vector3D direction, Color color, double intensity = 1.0);
    public GrBabylonJsLight AddDirectionalLight(string name, Vector3D direction, Color color, double intensity = 1.0);
    
    // Materials
    public GrBabylonJsMaterial AddStandardMaterial(string name);
    public GrBabylonJsMaterial AddPBRMaterial(string name);
    
    // Geometry Creation
    public GrBabylonJsMesh AddSphere(string name, double diameter, int segments = 16);
    public GrBabylonJsMesh AddBox(string name, double size);
    public GrBabylonJsMesh AddCylinder(string name, double height, double topDiameter, double bottomDiameter, int segments = 16);
    public GrBabylonJsMesh AddPlane(string name, double size);
    
    // Advanced Geometry
    public GrBabylonJsMesh AddArrow(string name, Vector3D origin, Vector3D direction, double thickness, Color color);
    public GrBabylonJsMesh AddCurve(string name, Vector3D[] points, Color color, double thickness);
    
    // Animation
    public GrBabylonJsAnimation CreateAnimation(string name, string property, int frameRate, Animation.LoopMode loopMode);
    public void AddAnimation(GrBabylonJsObject target, GrBabylonJsAnimation animation);
}
```

## Utility Classes

### IndexSetUtils

Utility methods for index set operations.

```csharp
public static class IndexSetUtils
{
    public static IIndexSet CreateFromIndices(params int[] indices);
    public static IIndexSet CreateEmpty();
    public static IIndexSet CreateRange(int minIndex, int maxIndex);
    
    // Set Operations
    public static IIndexSet Union(IIndexSet set1, IIndexSet set2);
    public static IIndexSet Intersection(IIndexSet set1, IIndexSet set2);
    public static IIndexSet SymmetricExcept(IIndexSet set1, IIndexSet set2);
}
```

### LinearTextComposer

Text composition with automatic indentation.

```csharp
public class LinearTextComposer
{
    // Text Building
    public LinearTextComposer AppendLine(string text = "");
    public LinearTextComposer Append(string text);
    
    // Indentation Management
    public LinearTextComposer IncreaseIndentation();
    public LinearTextComposer DecreaseIndentation();
    public LinearTextComposer SetIndentation(int level);
    
    // Output
    public override string ToString();
    public void SaveToFile(string filePath);
}
```

## Extension Methods

### Common Extensions

```csharp
// Scalar Extensions
public static T Add<T>(this T scalar1, T scalar2) where T : IScalarProcessor<T>;
public static T Multiply<T>(this T scalar1, T scalar2) where T : IScalarProcessor<T>;

// Multivector Extensions  
public static XGaMultivector<T> DivideByNorm<T>(this XGaMultivector<T> mv);
public static bool IsNearZero<T>(this XGaMultivector<T> mv, T epsilon);

// Vector Extensions
public static double DegreesToRadians(this double degrees);
public static double RadiansToDegrees(this double radians);

// Color Extensions
public static Color ToBabylonJsStandardMaterial(this Color color, string name);
public static string ToHexString(this Color color);
```

## Performance Considerations

### Memory Management
- Use `XGaComposer<T>` for efficient multivector construction
- Prefer immutable operations over mutable state
- Use appropriate index set implementations for your use case

### Optimization
- Enable optimization in `MetaContext` for code generation
- Use specialized scalar processors (`Float64` vs. generic) when possible  
- Consider GPU acceleration for large-scale parallel operations

### Threading
- Most operations are thread-safe for reading
- Use separate processor instances for concurrent modifications
- Consider parallel algorithms for large datasets

---

**[← Previous: Usage Examples](usage-examples.md) | [Next: Contributing →](contributing.md)**