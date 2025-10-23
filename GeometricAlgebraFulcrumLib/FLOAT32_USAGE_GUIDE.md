# Float32 Usage Guide

**Branch:** Feature/ScalarFloat32
**Status:** Production-Ready (97.9% of Float64 performance)
**Created:** 2025-10-23

## Overview

GeometricAlgebraFulcrumLib now supports `float` (32-bit) scalars alongside `double` (64-bit) scalars. The Float32 implementation provides nearly identical performance (97.9%) to Float64 while using 50% less memory, making it ideal for graphics, gaming, and GPU computing applications.

## Quick Start

### XGa (Extended Geometric Algebra)

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float32;

// Create a Float32 processor
var processor = XGaFloat32Processor.Euclidean;  // Euclidean metric
// or
var processor = XGaFloat32Processor.Conformal;  // Conformal metric
// or
var processor = XGaFloat32Processor.Create(negativeCount: 1, zeroCount: 0);  // Custom metric

// Create vectors using the Composer pattern
var v1 = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1f)
    .SetVectorTerm(1, 2f)
    .SetVectorTerm(2, 3f)
    .GetVector();

var v2 = processor.CreateVectorComposer()
    .SetVectorTerm(0, 4f)
    .SetVectorTerm(1, 5f)
    .SetVectorTerm(2, 6f)
    .GetVector();

// Perform geometric algebra operations
var outerProduct = v1.Op(v2);          // Bivector (grade 2)
var geometricProduct = v1.Gp(v2);      // Mixed multivector
var scalarProduct = v1.Sp(v2);         // Scalar
```

### CGA (Conformal Geometric Algebra)

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float32;

// Create CGA geometric spaces
var cga4D = CGaFloat32GeometricSpace.Space4D;  // 4D CGA for 2D geometry
var cga5D = CGaFloat32GeometricSpace.Space5D;  // 5D CGA for 3D geometry
var cgaCustom = CGaFloat32GeometricSpace.Create(vSpaceDimensions: 6);  // Custom

// Encode geometric objects (5D CGA example)
var point = cga5D.EncodeIpnsRound.Point(1f, 2f, 3f);
var sphere = cga5D.EncodeIpnsRound.Sphere(0f, 0f, 0f, 5f);  // Center at origin, radius 5
var plane = cga5D.EncodeOpns.Plane(0f, 0f, 1f, 2f);  // Normal (0,0,1), distance 2

// Perform CGA operations
var intersection = sphere.Op(plane);
```

### PGA (Projective Geometric Algebra)

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.PGa.Float32;

// Create PGA geometric spaces
var pga4D = PGaFloat32GeometricSpace.Space4D;  // 4D PGA for 3D Euclidean geometry
var pga5D = PGaFloat32GeometricSpace.Space5D;  // 5D PGA for 4D Euclidean geometry
var pgaCustom = PGaFloat32GeometricSpace.Create(vSpaceDimensions: 5);  // Custom

// PGA uses homogeneous coordinates for geometric objects
// Example: Working with 3D geometry in 4D PGA
var processor = pga4D.ProjectiveProcessor;
var point3D = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1f)   // x
    .SetVectorTerm(1, 2f)   // y
    .SetVectorTerm(2, 3f)   // z
    .SetVectorTerm(3, 1f)   // w (homogeneous coordinate)
    .GetVector();
```

## Hybrid API: Seamless Type Conversion

All encoder methods support **three overload types** for maximum flexibility:

```csharp
var cga = CGaFloat32GeometricSpace.Space4D;

// 1. Native float (T) - Most efficient
var v1 = cga.EncodeVGa.Vector(2f, 3f);

// 2. Double - Automatic conversion
var v2 = cga.EncodeVGa.Vector(2.0, 3.0);

// 3. IScalar<float> - For generic scenarios
var x = cga.ScalarProcessor.ScalarFromValue(2f);
var y = cga.ScalarProcessor.ScalarFromValue(3f);
var v3 = cga.EncodeVGa.Vector(x, y);
```

This hybrid approach allows:
- **Gradual migration** from Float64 to Float32 codebases
- **Mixed precision** workflows (e.g., `double` constants with `float` runtime data)
- **Library interop** with APIs using different numeric types

## Use Cases

### ✅ Recommended for Float32

- **Graphics Rendering** (Unity, Unreal Engine, DirectX, OpenGL)
- **Game Physics** (collision detection, rigid body dynamics)
- **GPU Computing** (CUDA, OpenCL, Compute Shaders)
- **Real-time Applications** (VR/AR, robotics, simulation)
- **Large-scale Datasets** (memory-bound computations)

### ⚠️ Use Float64 Instead

- **Scientific Computing** (high precision required)
- **Astronomical Calculations** (large number ranges)
- **Financial Calculations** (accuracy critical)
- **Long-term Numerical Integration** (error accumulation concerns)

## Performance Characteristics

| Metric | Float32 | Float64 | Ratio |
|--------|---------|---------|-------|
| **Average Performance** | - | - | **97.9%** |
| **Best Case (Reverse)** | 7.7 ns | 7.9 ns | **102%** 🚀 |
| **Worst Case (Sphere Encoding)** | 804.3 ns | 743.8 ns | **92.5%** |
| **Memory Usage** | - | - | **50%** less |
| **SIMD Throughput** | 8 values/256-bit | 4 values/256-bit | **2x** |

See [FLOAT32_PERFORMANCE_ANALYSIS.md](GeometricAlgebraFulcrumLib.Benchmarks/FLOAT32_PERFORMANCE_ANALYSIS.md) for detailed benchmarks.

## Architecture: Thin Wrapper Pattern

Float32 implementation uses **zero code duplication** via thin wrappers:

```csharp
// XGaFloat32Processor.cs - Static wrapper
public static class XGaFloat32Processor
{
    private static readonly IScalarProcessor<float> ScalarProcessor =
        ScalarProcessorOfFloat32.Instance;

    public static XGaProcessor<float> Euclidean
    {
        get => XGaProcessor<float>.CreateEuclidean(ScalarProcessor);
    }

    public static XGaProcessor<float> Create(int negativeCount, int zeroCount)
    {
        return XGaProcessor<float>.Create(ScalarProcessor, negativeCount, zeroCount);
    }
}
```

All business logic resides in the generic `XGaProcessor<T>` implementation. The wrapper merely provides:
1. **Convenience**: Pre-configured static properties
2. **Type Safety**: Explicit `float` semantics
3. **Discoverability**: IntelliSense-friendly API

## API Reference

### XGaFloat32Processor

**Static Properties:**
- `XGaProcessor<float> Euclidean` - All positive metric signatures
- `XGaConformalProcessor<float> Conformal` - 1 negative, rest positive
- `XGaProcessor<float> Projective` - 1 zero, rest positive

**Static Methods:**
- `XGaProcessor<float> Create(int negativeCount, int zeroCount)` - Custom metric signature
- `XGaProcessor<float> Create(int p, int q, int r = 0)` - p/q/r notation (p positive, q negative, r zero)

### CGaFloat32GeometricSpace

**Static Properties:**
- `CGaGeometricSpace4D<float> Space4D` - 4D CGA for 2D Euclidean geometry
- `CGaGeometricSpace5D<float> Space5D` - 5D CGA for 3D Euclidean geometry

**Static Methods:**
- `CGaGeometricSpace<float> Create(int vSpaceDimensions)` - Custom dimensions (≥ 4)

### PGaFloat32GeometricSpace

**Static Properties:**
- `PGaGeometricSpace3D<float> Space4D` - 4D PGA for 3D Euclidean geometry
- `PGaGeometricSpace4D<float> Space5D` - 5D PGA for 4D Euclidean geometry

**Static Methods:**
- `PGaGeometricSpace<float> Create(int vSpaceDimensions)` - Custom dimensions (≥ 3)

### Encoder Hybrid API

All encoder classes support `T`, `double`, and `IScalar<T>` overloads:

**IPNS/OPNS Round Encoders:**
- `Point(T x, T y)`, `Point(T x, T y, T z)`, ...
- `Sphere(T cx, T cy, T cz, T radius)`, ...
- `Circle(T cx, T cy, T radius)`, ...

**VGa/HGa Encoders:**
- `Vector(T x, T y)`, `Vector(T x, T y, T z)`, ...
- `Bivector(T xy)`, `Bivector(IScalar<T> xy, IScalar<T> xz, IScalar<T> yz)`, ...

**Direction/Tangent/Flat Encoders:**
- `Direction(LinVector2D<T> direction)`, ...
- `Tangent(T x, T y)`, ...
- `Line(T px, T py, T dx, T dy)`, ...

## Migration from Float64

Minimal code changes required:

```csharp
// Before (Float64)
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64;

var processor = XGaFloat64Processor.Euclidean;
var v = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1.0)
    .GetVector();

// After (Float32)
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float32;

var processor = XGaFloat32Processor.Euclidean;  // Changed namespace + class
var v = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1f)  // Changed literal suffix
    .GetVector();
```

**Migration Checklist:**
1. Change `using` statement: `Float64` → `Float32`
2. Change processor class: `XGaFloat64Processor` → `XGaFloat32Processor`
3. Change space classes:
   - `CGaFloat64GeometricSpace` → `CGaFloat32GeometricSpace`
   - `PGaFloat64GeometricSpace` → `PGaFloat32GeometricSpace` (if used)
4. Update numeric literals: `1.0` → `1f` (optional, works with `double` too via hybrid API)
5. **Note:** Linear Maps (`XGaPureRotor`, `XGaReflector`, etc.) are already generic - just use with `XGaFloat32Processor`

## Testing

**Smoke Tests:** `GeometricAlgebraFulcrumLib.UnitTests/Algebra/Scalars/Float32SmokeTests.cs`
**Performance Benchmarks:** `GeometricAlgebraFulcrumLib.Benchmarks/Scalars/CgaFloat32PerformanceBenchmarks.cs`

Run smoke tests:
```bash
dotnet test GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj \
    --filter "FullyQualifiedName~Float32SmokeTests"
```

Run performance benchmarks:
```bash
cd GeometricAlgebraFulcrumLib.Benchmarks/bin/Release/net8.0
./GeometricAlgebraFulcrumLib.Benchmarks.exe --filter "*CgaFloat32*"
```

## Known Issues

### Fixed in This Branch

**CGaVGaEncoder.cs:46** - Incorrect dimension assertion for `VectorAsXGaVector(T x, T y)`
- **Was:** `Debug.Assert(GeometricSpace.Is5D);`
- **Now:** `Debug.Assert(GeometricSpace.Is4D);`
- **Impact:** 2D vectors in 4D CGA now work correctly with Float32

## Future Work

**Completed in Phase 2:**
- ✅ PGa (Projective Geometric Algebra) Float32 wrapper - `PGaFloat32GeometricSpace`
- ✅ Linear Maps - Already fully generic (`XGaPureRotor<T>`, `XGaReflector<T>`, etc.) - **No wrapper needed**

**Phase 3 (Next Steps):**
- GPU interop examples (ILGPU, ComputeSharp)

**Potential Enhancements:**
- Mixed-precision workflows (Float32 storage + Float64 computation)
- SIMD intrinsics optimization
- Half-precision (Float16) support for mobile/embedded

## References

- **Performance Analysis:** [FLOAT32_PERFORMANCE_ANALYSIS.md](GeometricAlgebraFulcrumLib.Benchmarks/FLOAT32_PERFORMANCE_ANALYSIS.md)
- **Main Documentation:** https://kopffarben.github.io/GeometricAlgebraFulcrumLib/
- **Generic Math (.NET):** https://learn.microsoft.com/en-us/dotnet/standard/generics/math

---

**Questions? Issues?** Open an issue at https://github.com/ga-explorer/GeometricAlgebraFulcrumLib/issues
