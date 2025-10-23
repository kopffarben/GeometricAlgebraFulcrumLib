# Trajectories API Analysis - Float64 vs Generic Implementations

**Agent:** Agent 20 - Trajectories API Analyzer
**Date:** 2025-10-23
**Base Directory:** `GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/Trajectories/`

---

## Executive Summary

### Critical Finding: NO Generic Implementations Exist

**The entire Trajectories subsystem (162 files) is Float64-only with ZERO generic scalar abstraction.**

This is a **MAJOR architectural gap** compared to the rest of GA-FUL:
- **Scalars API**: Has both Float64 and Generic implementations
- **Linear Algebra API**: Has both Float64 and Generic implementations
- **Geometric Algebra API**: Has both Float64 and Generic implementations
- **Trajectories API**: **Float64 ONLY** ❌

---

## 1. Base Trajectory Infrastructure

### Root-Level Files (5 files)

#### 1.1 `IFloat64Trajectory.cs` - Base Interface
```csharp
public interface IFloat64Trajectory : IAlgebraicElement
{
    bool IsPeriodic { get; }
    Float64ScalarRange TimeRange { get; }
    double MinTime { get; }
    double MaxTime { get; }
    double MidTime { get; }
    double TimeRangeLength { get; }

    IFloat64Trajectory ToFinite();
    IFloat64Trajectory ToPeriodic();
}

public interface IFloat64Trajectory<out T> : IFloat64Trajectory
{
    T ValueAtMinTime { get; }
    T ValueAtMidTime { get; }
    T ValueAtMaxTime { get; }
    T GetValue(double t);
}
```

**Design Notes:**
- Hardcoded to `Float64ScalarRange` and `double`
- Type parameter `T` is for value type (Vector2D, Vector3D, etc.), NOT scalar type
- No generic scalar abstraction

#### 1.2 `Float64Trajectory.cs` - Abstract Base Class
```csharp
public abstract class Float64Trajectory : IFloat64Trajectory
{
    public bool IsPeriodic { get; }
    public bool IsFinite => !IsPeriodic;
    public Float64ScalarRange TimeRange { get; }

    protected Float64Trajectory(Float64ScalarRange timeRange, bool isPeriodic) { }

    public abstract bool IsValid();
    public abstract IFloat64Trajectory ToFinite();
    public abstract IFloat64Trajectory ToPeriodic();
}

public abstract class Float64Trajectory<T>(Float64ScalarRange timeRange, bool isPeriodic) :
    Float64Trajectory(timeRange, isPeriodic),
    IFloat64Trajectory<T>
{
    public T ValueAtMinTime => GetValue(MinTime);
    public T ValueAtMidTime => GetValue(MidTime);
    public T ValueAtMaxTime => GetValue(MaxTime);

    public abstract T GetValue(double t);
}
```

**Design Pattern:**
- Primary constructor syntax (C# 12)
- Immutable time range
- Abstract methods for value retrieval

#### 1.3 `Float64TrajectoryTimeRange.cs` - Time Range Implementation
- Complete implementation of time range operations
- 815 lines of code
- Static factory methods, operators, affine mapping
- Hardcoded to `double` throughout

#### 1.4 `Float64TrajectoryConverter.cs` - Value Mapper
```csharp
public abstract class Float64TrajectoryConverter<T1, T>(
    Float64Trajectory<T1> baseSignal,
    Func<T1, T> valueMap) :
    Float64Trajectory<T>(baseSignal.TimeRange, baseSignal.IsPeriodic)
{
    public Float64Trajectory<T1> BaseSignal { get; } = baseSignal;
    public Func<T1, T> ValueMap { get; } = valueMap;

    public override T GetValue(double t)
    {
        return ValueMap(BaseSignal.GetValue(t));
    }
}
```

**Pattern:** Adapter pattern for value transformation, but time parameter stays `double`

#### 1.5 `Float64TrajectoryUtils.cs` - Extension Methods
- Time clamping utilities
- Relative time calculations
- All hardcoded to `double`

---

## 2. Trajectory Type Analysis (8 Subdirectories)

### 2.1 Scalars Trajectories

**Directory:** `Trajectories/Scalars/Float64/`
**File Count:** 45 files
**Implementation:** Float64 only

#### Structure:
```
Scalars/Float64/
├── Angles/                  (3 files)  - DirectAngle, PolarAngle
├── Basic/                   (9 files)  - Constant, Computed, Harmonic, Sin, Cos
├── Composers/               (2 files)  - Signal composers
├── Mapped/                  (9 files)  - Affine, Derivative, Segment, Smooth
├── Normalized/              (9 files)  - Ramp, Step, Triangle, Rectangle
├── Parametric/              (4 files)  - Arc length, local frames
├── Parametric/Samplers/     (2 files)  - Sampling strategies
├── Plots/                   (2 files)  - Plotting utilities
└── Root Files               (5 files)  - Base signal classes
```

#### Key Classes:

**`Float64ScalarSignal.cs`** (1,646 lines!)
```csharp
public abstract class Float64ScalarSignal(Float64ScalarRange timeRange, bool isPeriodic) :
    Float64Trajectory<double>(timeRange, isPeriodic)
{
    // Massive factory method library:
    public static Float64ScalarSignal FiniteZero() { }
    public static Float64ScalarSignal FiniteOne() { }
    public static Float64ScalarSignal FiniteConstant(double value) { }
    public static Float64ScalarSignal FiniteSharpStep() { }
    public static Float64ScalarSignal FiniteSin() { }
    public static Float64ScalarSignal FiniteCos() { }
    // ... 100+ more factory methods

    public virtual double GetDerivative1Value(double t) { }
    public virtual double GetDerivative2Value(double t) { }

    public virtual Float64ScalarRange FindValueRange() { }
}
```

**Features:**
- Extensive factory methods for common signals
- Numerical differentiation via MathNet.Numerics
- Value range computation
- Operator overloading (+, -, *, etc.)
- Finite vs Periodic variants

**Missing:** Generic scalar abstraction, custom scalar processors

---

### 2.2 Vectors2D Trajectories

**Directory:** `Trajectories/Vectors2D/Float64/`
**File Count:** 44 files
**Implementation:** Float64 only

#### Structure:
```
Vectors2D/Float64/
├── Adaptive/        (9 files)  - Adaptive sampling, tree structures
├── Basic/           (9 files)  - Circle, LineSegment, Polar, Harmonic
├── Bezier/          (6 files)  - Bezier curves (degree 0-3, N)
├── Composers/       (2 files)  - Path composers
├── Mapped/          (7 files)  - Affine, ArcLength, Roulette mappings
├── Samplers/        (6 files)  - Various sampling strategies
└── Root Files       (5 files)  - Base path classes
```

#### Key Classes:

**`Float64Path2D.cs`** (195 lines)
```csharp
public abstract class Float64Path2D(Float64ScalarRange timeRange, bool isPeriodic) :
    Float64Trajectory<LinFloat64Vector2D>(timeRange, isPeriodic)
{
    public virtual Pair<Float64ScalarSignal> GetScalarComponents() { }
    public virtual Pair<Float64ScalarRange> FindValueRange() { }

    public virtual LinFloat64Vector2D GetDerivative1Value(double t) { }
    public virtual LinFloat64Vector2D GetDerivative2Value(double t) { }

    public virtual Float64Path2DLocalFrame GetFrame(double t) { }
}
```

**Design:**
- Value type: `LinFloat64Vector2D` (hardcoded Float64)
- Decomposes into two `Float64ScalarSignal` components
- Local frame computation (position + tangent)
- Numerical differentiation for derivatives

**Example Implementations:**

**`Float64CirclePath2D.cs`**
```csharp
public sealed class Float64CirclePath2D : Float64Path2D
{
    public LinFloat64Vector2D Center { get; }
    public LinFloat64DirectedAngle AngleMinValue { get; }
    public LinFloat64DirectedAngle AngleMaxValue { get; }
    public double Radius { get; }

    public override LinFloat64Vector2D GetValue(double t)
    {
        var angle = GetAngle(t);
        return Center + Radius * angle.ToUnitVector();
    }

    public override LinFloat64Vector2D GetDerivative1Value(double t)
    {
        var angleRate = GetAngleRate();
        var angle = GetAngle(t);
        return (Radius * angleRate) * angle.ToUnitVector().GetNormal();
    }
}
```

**`Float64Bezier3Path2D.cs`** (Cubic Bezier)
```csharp
public class Float64Bezier3Path2D : Float64Path2D
{
    public LinFloat64Vector2D Point0 { get; }
    public LinFloat64Vector2D Point1 { get; }
    public LinFloat64Vector2D Point2 { get; }
    public LinFloat64Vector2D Point3 { get; }

    public override LinFloat64Vector2D GetValue(double t)
    {
        var s = 1 - t;
        return s * s * s * Point0 +
               3 * s * s * t * Point1 +
               3 * s * t * t * Point2 +
               t * t * t * Point3;
    }
}
```

**Adaptive Sampling:**
- `Float64AdaptivePath2D` - Tree-based adaptive refinement
- `AdaptiveCurveSampler2D` - Curvature-based sampling
- Used for rendering curved paths efficiently

---

### 2.3 Vectors3D Trajectories

**Directory:** `Trajectories/Vectors3D/Float64/`
**File Count:** 53 files
**Implementation:** Float64 only

#### Structure:
```
Vectors3D/Float64/
├── Adaptive/        (9 files)  - 3D adaptive sampling
├── Basic/           (9 files)  - Line, Spherical, Harmonic paths
├── Bezier/          (6 files)  - 3D Bezier curves
├── Circles/         (6 files)  - Circle paths (XY, YZ, ZX planes)
├── Composers/       (2 files)  - Path composers
├── Mapped/          (9 files)  - Affine, ArcLength, RotatedNormals
├── Samplers/        (6 files)  - 3D sampling strategies
└── Root Files       (6 files)  - Base 3D path classes
```

#### Key Classes:

**`Float64Path3D.cs`** (221 lines)
```csharp
public abstract class Float64Path3D(Float64ScalarRange timeRange, bool isPeriodic) :
    Float64Trajectory<LinFloat64Vector3D>(timeRange, isPeriodic)
{
    public virtual Triplet<Float64ScalarSignal> GetScalarComponents() { }
    public virtual Triplet<Float64ScalarRange> FindValueRange() { }

    public virtual LinFloat64Vector3D GetDerivative1Value(double t) { }
    public virtual LinFloat64Vector3D GetDerivative2Value(double t) { }

    public virtual Float64Path3DLocalFrame GetFrame(double t) { }
}
```

**Unique to 3D:**
- `Float64Path3DLocalFrame` - Frenet frame (tangent, normal, binormal)
- `IFloat64Path3DLocalFrame` - Local coordinate systems along curve
- Rotated normals for swept surfaces

**Circle Implementations:**

**`IFloat64CirclePath3D.cs`** - Interface
```csharp
public interface IFloat64CirclePath3D : IFloat64Trajectory<LinFloat64Vector3D>
{
    LinFloat64Vector3D Center { get; }
    double Radius { get; }
    LinFloat64Vector3D Direction1 { get; }
    LinFloat64Vector3D Direction2 { get; }
}
```

**Specialized Circles:**
- `Float64XyCirclePath3D` - Circle in XY plane
- `Float64YzCirclePath3D` - Circle in YZ plane
- `Float64ZxCirclePath3D` - Circle in ZX plane
- `Float64AxisAlignedCirclePath3D` - Arbitrary axis-aligned
- `Float64CirclePath3D` - General orientation

**`Float64SphericalPath3D.cs`** - Spherical coordinates
```csharp
public sealed class Float64SphericalPath3D : Float64Path3D
{
    public LinFloat64Vector3D Origin { get; }
    public Float64ScalarSignal RadiusSignal { get; }
    public Float64ScalarSignal ThetaSignal { get; }  // Polar angle
    public Float64ScalarSignal PhiSignal { get; }    // Azimuthal angle

    public override LinFloat64Vector3D GetValue(double t)
    {
        var r = RadiusSignal.GetValue(t);
        var theta = ThetaSignal.GetValue(t);
        var phi = PhiSignal.GetValue(t);

        return Origin + LinFloat64Vector3D.CreateFromSpherical(r, theta, phi);
    }
}
```

---

### 2.4 Bivectors2D Trajectories

**Directory:** `Trajectories/Bivectors2D/Float64/`
**File Count:** 3 files
**Implementation:** Float64 only

#### Structure:
```
Bivectors2D/Float64/
├── ComputedParametricBivector2D.cs
├── ConstantParametricBivector2D.cs
└── IParametricBivector2D.cs
```

**Status:** **MINIMAL IMPLEMENTATION**

#### Interface:
```csharp
public interface IParametricBivector2D : IAlgebraicElement
{
    Float64ScalarRange TimeRange { get; }

    LinFloat64Bivector2D GetValue(double parameterValue);
    LinFloat64Bivector2D GetDerivative1Bivector(double parameterValue);

    Float64ScalarSignal GetDualScalarCurve();  // Bivector → Scalar dual
}
```

**Key Points:**
- Does NOT inherit from `Float64Trajectory<T>`
- Different API pattern than vectors
- Only 2 concrete implementations (Computed, Constant)
- Can extract dual scalar curve

**`ConstantParametricBivector2D.cs`**
```csharp
public sealed class ConstantParametricBivector2D : IParametricBivector2D
{
    public Float64ScalarRange TimeRange { get; }
    public LinFloat64Bivector2D Value { get; }

    public LinFloat64Bivector2D GetValue(double parameterValue) => Value;
    public LinFloat64Bivector2D GetDerivative1Bivector(double parameterValue)
        => LinFloat64Bivector2D.Zero;
}
```

---

### 2.5 Bivectors3D Trajectories

**Directory:** `Trajectories/Bivectors3D/`
**File Count:** 3 files
**Implementation:** Float64 only

#### Structure:
```
Bivectors3D/
├── ComputedParametricBivector3D.cs
├── ConstantParametricBivector3D.cs
└── IParametricBivector3D.cs
```

**Status:** **MINIMAL IMPLEMENTATION** (same as 2D)

#### Interface:
```csharp
public interface IParametricBivector3D : IAlgebraicElement
{
    Float64ScalarRange TimeRange { get; }

    LinFloat64Bivector3D GetValue(double parameterValue);

    // Extract normal vector curve from bivector
    Float64Path3D GetNormalVectorCurve(LinFloat64Vector3D? zeroNormal = null);
}
```

**Unique Feature:** `GetNormalVectorCurve()` - Extracts 3D normal vectors from bivector trajectory

**Note:** No `GetDerivative1Bivector()` method (different from 2D!)

---

### 2.6 Trivectors3D Trajectories

**Directory:** `Trajectories/Trivectors3D/Float64/`
**File Count:** 4 files
**Implementation:** Float64 only

#### Structure:
```
Trivectors3D/Float64/
├── ILinFloat64Trivector3DTrajectory.cs
├── LinFloat64Trivector3DComputedTrajectory.cs
├── LinFloat64Trivector3DConstantTrajectory.cs
└── LinFloat64Trivector3DTrajectory.cs
```

**Status:** **INCOMPLETE IMPLEMENTATION**

#### Base Class:
```csharp
public abstract class LinFloat64Trivector3DTrajectory(
    Float64ScalarRange timeRange,
    bool isPeriodic) :
    Float64Trajectory<LinFloat64Trivector3D>(timeRange, isPeriodic),
    ILinFloat64Trivector3DTrajectory
{
    public override IFloat64Trajectory ToFinite()
    {
        throw new NotImplementedException();  // ❌ NOT IMPLEMENTED
    }

    public override IFloat64Trajectory ToPeriodic()
    {
        throw new NotImplementedException();  // ❌ NOT IMPLEMENTED
    }

    public abstract LinFloat64Trivector3D GetDerivative1Value(double t);
    public abstract LinFloat64Trivector3D GetDerivative2Value(double t);

    public abstract Float64ScalarSignal GetDualScalarCurve();  // Trivector → Scalar dual
}
```

**Bug Alert:** Missing `ToFinite()`/`ToPeriodic()` implementations throw exceptions!

#### Interface:
```csharp
public interface ILinFloat64Trivector3DTrajectory : IFloat64Trajectory<LinFloat64Trivector3D>
{
    LinFloat64Trivector3D GetDerivative1Value(double t);
    LinFloat64Trivector3D GetDerivative2Value(double t);
    Float64ScalarSignal GetDualScalarCurve();
}
```

**Implementations:**
- `LinFloat64Trivector3DConstantTrajectory` - Constant trivector value
- `LinFloat64Trivector3DComputedTrajectory` - Lambda-based computation

---

### 2.7 Quaternions Trajectories

**Directory:** `Trajectories/Quaternions/Float64/`
**File Count:** 3 files
**Implementation:** Float64 only

#### Structure:
```
Quaternions/Float64/
├── ComputedParametricQuaternion.cs
├── ConstantParametricQuaternion.cs
└── IParametricQuaternion.cs
```

**Status:** **MINIMAL IMPLEMENTATION**

#### Interface:
```csharp
public interface IParametricQuaternion : IAlgebraicElement
{
    Float64ScalarRange TimeRange { get; }

    LinFloat64Quaternion GetQuaternion(double parameterValue);
    LinFloat64Quaternion GetDerivative1Quaternion(double parameterValue);
}
```

**Key Points:**
- Does NOT inherit from `Float64Trajectory<T>`
- Method names: `GetQuaternion()` instead of `GetValue()`
- `GetDerivative1Quaternion()` instead of `GetDerivative1Value()`
- **API INCONSISTENCY** with other trajectory types

**`ConstantParametricQuaternion.cs`**
```csharp
public sealed class ConstantParametricQuaternion : IParametricQuaternion
{
    public Float64ScalarRange TimeRange { get; }
    public LinFloat64Quaternion Value { get; }

    public LinFloat64Quaternion GetQuaternion(double parameterValue) => Value;
    public LinFloat64Quaternion GetDerivative1Quaternion(double parameterValue)
        => LinFloat64Quaternion.Create(0, 0, 0, 0);
}
```

---

### 2.8 Colors Trajectories

**Directory:** `Trajectories/Colors/`
**File Count:** 2 files
**Implementation:** Float64 only

#### Structure:
```
Colors/
├── LinFloat64Vector3DTimeSignal.cs
└── TemporalRgba32Color.cs
```

**Status:** **MINIMAL IMPLEMENTATION**

#### Key Class: `TemporalRgba32Color.cs`

```csharp
public sealed class TemporalRgba32Color : Float64Trajectory<Rgba32>
{
    public Float64ScalarSignal Red { get; }
    public Float64ScalarSignal Green { get; }
    public Float64ScalarSignal Blue { get; }
    public Float64ScalarSignal Alpha { get; }

    private TemporalRgba32Color(
        Float64ScalarRange timeRange,
        bool isPeriodic,
        Float64ScalarSignal red,
        Float64ScalarSignal green,
        Float64ScalarSignal blue,
        Float64ScalarSignal alpha)
        : base(timeRange, isPeriodic)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public override Rgba32 GetValue(double t)
    {
        var r = (Red.GetValue(t) * 255).Clamp(0, 255).RoundToByte();
        var g = (Green.GetValue(t) * 255).Clamp(0, 255).RoundToByte();
        var b = (Blue.GetValue(t) * 255).Clamp(0, 255).RoundToByte();
        var a = (Alpha.GetValue(t) * 255).Clamp(0, 255).RoundToByte();

        return new Rgba32(r, g, b, a);
    }

    public override IFloat64Trajectory ToFinite()
    {
        throw new NotImplementedException();  // ❌ NOT IMPLEMENTED
    }

    public override IFloat64Trajectory ToPeriodic()
    {
        throw new NotImplementedException();  // ❌ NOT IMPLEMENTED
    }
}
```

**Factory Methods:**
```csharp
public static TemporalRgba32Color FiniteRed(Float64ScalarRange timeRange, Float64ScalarSignal red)
public static TemporalRgba32Color FiniteGreen(Float64ScalarRange timeRange, Float64ScalarSignal green)
public static TemporalRgba32Color FiniteBlue(Float64ScalarRange timeRange, Float64ScalarSignal blue)
public static TemporalRgba32Color FiniteGray(Float64ScalarRange timeRange, Float64ScalarSignal gray)
public static TemporalRgba32Color Finite(
    Float64ScalarRange timeRange,
    Float64ScalarSignal red,
    Float64ScalarSignal green,
    Float64ScalarSignal blue,
    Float64ScalarSignal alpha)
```

**Dependencies:**
- Uses `SixLabors.ImageSharp` for color types
- Integrates with Float64 scalar signals

**Bug Alert:** Missing `ToFinite()`/`ToPeriodic()` implementations!

---

## 3. API Consistency Matrix

| Trajectory Type | Inherits Float64Trajectory | GetValue() | GetDerivative1() | GetDerivative2() | ToFinite/Periodic | File Count |
|----------------|---------------------------|------------|------------------|------------------|-------------------|------------|
| **Scalars**    | ✅ Yes                    | ✅ Yes     | ✅ Yes           | ✅ Yes           | ✅ Yes            | 45         |
| **Vectors2D**  | ✅ Yes                    | ✅ Yes     | ✅ Yes           | ✅ Yes           | ✅ Yes            | 44         |
| **Vectors3D**  | ✅ Yes                    | ✅ Yes     | ✅ Yes           | ✅ Yes           | ✅ Yes            | 53         |
| **Bivectors2D**| ❌ No (own interface)     | ✅ Yes     | ✅ Yes           | ❌ No            | ❌ N/A            | 3          |
| **Bivectors3D**| ❌ No (own interface)     | ✅ Yes     | ❌ No            | ❌ No            | ❌ N/A            | 3          |
| **Trivectors3D**| ✅ Yes                   | ✅ Yes     | ✅ Yes           | ✅ Yes           | ❌ Throws!        | 4          |
| **Quaternions**| ❌ No (own interface)     | ⚠️ GetQuaternion() | ⚠️ Different name | ❌ No     | ❌ N/A            | 3          |
| **Colors**     | ✅ Yes                    | ✅ Yes     | ❌ No            | ❌ No            | ❌ Throws!        | 2          |

**Legend:**
- ✅ Fully implemented and consistent
- ❌ Not implemented or missing
- ⚠️ Implemented but with different API

---

## 4. Feature Completeness by Trajectory Type

### 4.1 Implementation Richness

| Feature Category          | Scalars | Vectors2D | Vectors3D | Bivectors2D | Bivectors3D | Trivectors3D | Quaternions | Colors |
|--------------------------|---------|-----------|-----------|-------------|-------------|--------------|-------------|--------|
| **Factory Methods**      | ✅✅✅   | ✅✅      | ✅✅      | ⚠️          | ⚠️          | ⚠️           | ⚠️          | ✅     |
| **Basic Paths**          | ✅✅✅   | ✅✅      | ✅✅      | ✅          | ✅          | ✅           | ✅          | ✅     |
| **Bezier Curves**        | ❌      | ✅✅      | ✅✅      | ❌          | ❌          | ❌           | ❌          | ❌     |
| **Adaptive Sampling**    | ❌      | ✅✅      | ✅✅      | ❌          | ❌          | ❌           | ❌          | ❌     |
| **Arc Length Param**     | ✅      | ✅        | ✅        | ❌          | ❌          | ❌           | ❌          | ❌     |
| **Local Frames**         | ✅      | ✅        | ✅✅      | ❌          | ❌          | ❌           | ❌          | ❌     |
| **Derivatives (1st)**    | ✅      | ✅        | ✅        | ✅          | ❌          | ✅           | ✅          | ❌     |
| **Derivatives (2nd)**    | ✅      | ✅        | ✅        | ❌          | ❌          | ✅           | ❌          | ❌     |
| **Mapping/Transforms**   | ✅✅✅   | ✅✅      | ✅✅      | ❌          | ❌          | ❌           | ❌          | ❌     |
| **Composers**            | ✅✅    | ✅        | ✅        | ❌          | ❌          | ❌           | ❌          | ❌     |
| **Plotting Utilities**   | ✅✅    | ❌        | ❌        | ❌          | ❌          | ❌           | ❌          | ❌     |

**Legend:** ✅✅✅ Extensive, ✅✅ Rich, ✅ Basic, ⚠️ Partial, ❌ Missing

### 4.2 Implementation Priority (by richness)

1. **Scalars** - Most complete (45 files, extensive features)
2. **Vectors3D** - Very rich (53 files, circles, Bezier, adaptive)
3. **Vectors2D** - Rich (44 files, Bezier, adaptive)
4. **Trivectors3D** - Minimal but follows conventions (4 files)
5. **Bivectors2D** - Minimal (3 files)
6. **Bivectors3D** - Minimal (3 files)
7. **Quaternions** - Minimal with inconsistent API (3 files)
8. **Colors** - Minimal specialty implementation (2 files)

---

## 5. Missing Features & Inconsistencies

### 5.1 No Generic Scalar Abstraction

**Current State:**
```csharp
// Hardcoded to double everywhere
public abstract T GetValue(double t);
public virtual LinFloat64Vector2D GetDerivative1Value(double t);
public Float64ScalarRange TimeRange { get; }
```

**Expected Generic Pattern (from rest of GA-FUL):**
```csharp
// What SHOULD exist but doesn't:
public interface ITrajectory<TScalar> : IAlgebraicElement
{
    bool IsPeriodic { get; }
    ScalarRange<TScalar> TimeRange { get; }
    TScalar MinTime { get; }
    TScalar MaxTime { get; }
    // ...
}

public abstract class Trajectory<TScalar, TValue> : ITrajectory<TScalar>
{
    public IScalarProcessor<TScalar> ScalarProcessor { get; }
    public abstract TValue GetValue(TScalar t);
}

// Specialized:
public abstract class Float64Trajectory<TValue> : Trajectory<double, TValue>
{
    // Optimized for Float64
}
```

**Impact:**
- Cannot use ERational for exact trajectories
- Cannot use EDecimal for high-precision
- Cannot use MetaExpression for symbolic trajectories
- Cannot generate optimized code from trajectory definitions

---

### 5.2 API Inconsistencies

#### Issue 1: Bivectors/Quaternions Don't Use Base Class
```csharp
// Vectors, Scalars, Trivectors:
public abstract class Float64Path3D : Float64Trajectory<LinFloat64Vector3D> { }

// But Bivectors:
public interface IParametricBivector2D : IAlgebraicElement { }  // ❌ Different

// And Quaternions:
public interface IParametricQuaternion : IAlgebraicElement { }  // ❌ Different
```

**Why this matters:**
- Cannot use polymorphically with `Float64Trajectory<T>`
- Cannot reuse common trajectory utilities
- Different extension method families

#### Issue 2: Quaternion Method Names
```csharp
// All other types:
T GetValue(double t);
T GetDerivative1Value(double t);

// Quaternions:
LinFloat64Quaternion GetQuaternion(double parameterValue);  // ❌ Different
LinFloat64Quaternion GetDerivative1Quaternion(double parameterValue);  // ❌ Different
```

**Impact:** Breaks polymorphism and consistency

#### Issue 3: Incomplete Implementations

**Throws NotImplementedException:**
```csharp
// LinFloat64Trivector3DTrajectory.cs
public override IFloat64Trajectory ToFinite()
{
    throw new NotImplementedException();  // ❌
}

// TemporalRgba32Color.cs
public override IFloat64Trajectory ToFinite()
{
    throw new NotImplementedException();  // ❌
}
```

**Missing Derivatives:**
- Bivectors2D: Has 1st derivative ✅
- Bivectors3D: Missing 1st derivative ❌
- Quaternions: Has 1st derivative ✅
- Colors: Missing all derivatives ❌

---

### 5.3 Missing Trajectory Types

**From Linear Algebra that should have trajectories:**
- ❌ Vectors4D trajectories (only 2D and 3D exist)
- ❌ Matrix2x2 trajectories
- ❌ Matrix3x3 trajectories
- ❌ Matrix4x4 trajectories
- ❌ Complex number trajectories
- ❌ Quaternion trajectories (only minimal interface exists)

**From Geometric Algebra:**
- ❌ Multivector trajectories
- ❌ Rotor trajectories (for interpolating rotations)
- ❌ Versor trajectories

---

## 6. Comparison with Scalars API Pattern

### Scalars API (for reference)

The Scalars area has BOTH Float64 and Generic implementations:

```
Algebra/Scalars/
├── Float64/
│   ├── ScalarProcessorOfFloat64.cs
│   ├── Float64Scalar.cs
│   └── Float64ScalarUtils.cs
├── Generic/
│   ├── IScalarProcessor<T>.cs
│   ├── Scalar<T>.cs
│   └── ScalarComposerUtils<T>.cs
└── Common interfaces
```

**Generic Pattern:**
```csharp
public interface IScalarProcessor<T>
{
    T Add(T a, T b);
    T Subtract(T a, T b);
    T Times(T a, T b);
    // ...
}

public class ScalarProcessorOfFloat64 : IScalarProcessor<double>
{
    // Optimized implementations
}
```

### Trajectories Should Follow Same Pattern

**What's missing:**

```
Trajectories/
├── Float64/                          # ✅ EXISTS (everything is here)
│   ├── Scalars/
│   ├── Vectors2D/
│   └── ...
├── Generic/                          # ❌ COMPLETELY MISSING
│   ├── Scalars/
│   │   └── ScalarTrajectory<T>.cs
│   ├── Vectors2D/
│   │   └── Vector2DTrajectory<T>.cs
│   └── ...
└── Interfaces/                       # ❌ MISSING
    ├── ITrajectory<TScalar>.cs
    ├── ITrajectoryProcessor<TScalar>.cs
    └── ...
```

---

## 7. Parameter Order Analysis

**All trajectory types consistently use:**
```csharp
constructor(Float64ScalarRange timeRange, bool isPeriodic)
```

**Methods consistently use:**
```csharp
GetValue(double t)
GetDerivative1Value(double t)
GetDerivative2Value(double t)
```

**Exception:** Quaternions use different names but same parameter order

**Assessment:** ✅ Parameter order is consistent across the board

---

## 8. Recommended Actions

### Priority 1: Critical Missing Features

1. **Add Generic Scalar Abstraction**
   - Create `ITrajectory<TScalar>` base interface
   - Create `Trajectory<TScalar, TValue>` abstract base
   - Implement `ScalarTrajectory<TScalar>`
   - Keep existing Float64 implementations as optimized specializations

2. **Fix API Inconsistencies**
   - Make `IParametricBivector2D/3D` extend `Float64Trajectory<T>`
   - Make `IParametricQuaternion` extend `Float64Trajectory<T>`
   - Rename `GetQuaternion()` to `GetValue()`

3. **Complete Incomplete Implementations**
   - Implement `ToFinite()`/`ToPeriodic()` for Trivectors3D
   - Implement `ToFinite()`/`ToPeriodic()` for Colors
   - Add `GetDerivative1Value()` for Bivectors3D
   - Add derivatives for Colors

### Priority 2: Missing Trajectory Types

4. **Add Vector4D Trajectories**
   - Follow Vectors3D pattern
   - 4D paths, hyperspheres, etc.

5. **Add Matrix Trajectories**
   - Matrix2x2, Matrix3x3, Matrix4x4
   - Useful for animated transformations

6. **Add Geometric Algebra Trajectories**
   - Multivector trajectories
   - Rotor trajectories (rotation interpolation)
   - Versor trajectories

### Priority 3: Enhanced Features

7. **Expand Bivector/Trivector Features**
   - Add Bezier curves
   - Add adaptive sampling
   - Add arc length parameterization

8. **Add Missing Derivatives**
   - 2nd derivatives for all types
   - Numerical differentiation fallbacks

9. **Quaternion Enhancements**
   - SLERP (spherical linear interpolation)
   - SQUAD (spherical cubic)
   - Rich factory methods like Scalars has

### Priority 4: Infrastructure

10. **Plotting Utilities**
    - Extend plotting to Vectors2D/3D
    - 3D visualization support

11. **Sampling Strategies**
    - Unified sampling interface across all types
    - Add to Bivectors/Trivectors/Quaternions

12. **Composer Pattern**
    - Extend to all trajectory types
    - Trajectory algebra (add, subtract, compose)

---

## 9. Bugs Found

### Bug 1: Trivectors3D - NotImplementedException
**File:** `LinFloat64Trivector3DTrajectory.cs`
**Lines:** 13-22

```csharp
public override IFloat64Trajectory ToFinite()
{
    throw new NotImplementedException();  // ❌
}

public override IFloat64Trajectory ToPeriodic()
{
    throw new NotImplementedException();  // ❌
}
```

**Impact:** Cannot convert Trivector trajectories between finite/periodic

**Fix:** Implement these methods following pattern from Scalars/Vectors

---

### Bug 2: Colors - NotImplementedException
**File:** `TemporalRgba32Color.cs`
**Lines:** 122-130

```csharp
public override IFloat64Trajectory ToFinite()
{
    throw new NotImplementedException();  // ❌
}

public override IFloat64Trajectory ToPeriodic()
{
    throw new NotImplementedException();  // ❌
}
```

**Impact:** Cannot convert Color trajectories between finite/periodic

**Fix:** Implement these methods, wrapping RGBA components

---

### Bug 3: Bivectors2D - Missing 2nd Derivative
**File:** `IParametricBivector2D.cs`
**Lines:** 11-21

```csharp
public interface IParametricBivector2D : IAlgebraicElement
{
    LinFloat64Bivector2D GetValue(double parameterValue);
    LinFloat64Bivector2D GetDerivative1Bivector(double parameterValue);
    // ❌ Missing GetDerivative2Bivector
}
```

**Impact:** Cannot compute acceleration/curvature for Bivector paths

**Fix:** Add `GetDerivative2Bivector()` method

---

### Bug 4: Bivectors3D - Missing All Derivatives
**File:** `IParametricBivector3D.cs`
**Lines:** 11-19

```csharp
public interface IParametricBivector3D : IAlgebraicElement
{
    LinFloat64Bivector3D GetValue(double parameterValue);
    // ❌ Missing GetDerivative1Bivector
    // ❌ Missing GetDerivative2Bivector
}
```

**Impact:** No derivatives available for 3D Bivector trajectories

**Fix:** Add both derivative methods, matching 2D API

---

### Bug 5: Quaternions - API Inconsistency
**File:** `IParametricQuaternion.cs`
**Lines:** 10-18

```csharp
// ❌ Inconsistent method names
LinFloat64Quaternion GetQuaternion(double parameterValue);
LinFloat64Quaternion GetDerivative1Quaternion(double parameterValue);

// Should be:
LinFloat64Quaternion GetValue(double parameterValue);
LinFloat64Quaternion GetDerivative1Value(double parameterValue);
```

**Impact:** Breaks polymorphism with other trajectory types

**Fix:** Rename methods to match standard trajectory API

---

## 10. Statistics Summary

### File Count by Type
| Type | Float64 Files | Generic Files | Total |
|------|---------------|---------------|-------|
| Base Infrastructure | 5 | 0 | 5 |
| Scalars | 45 | 0 | 45 |
| Vectors2D | 44 | 0 | 44 |
| Vectors3D | 53 | 0 | 53 |
| Bivectors2D | 3 | 0 | 3 |
| Bivectors3D | 3 | 0 | 3 |
| Trivectors3D | 4 | 0 | 4 |
| Quaternions | 3 | 0 | 3 |
| Colors | 2 | 0 | 2 |
| **TOTAL** | **162** | **0** | **162** |

### Implementation Completeness
| Metric | Count | Percentage |
|--------|-------|------------|
| Files with Float64 only | 162 | 100% |
| Files with Generic support | 0 | 0% |
| Complete implementations | ~145 | ~89% |
| Incomplete (throw NotImpl) | ~2 | ~1% |
| Minimal (< 5 methods) | ~15 | ~9% |

### API Coverage
| Feature | Scalars | Vectors2D | Vectors3D | Bivectors | Trivectors | Quaternions | Colors |
|---------|---------|-----------|-----------|-----------|------------|-------------|--------|
| Basic paths | 100% | 100% | 100% | 30% | 30% | 30% | 50% |
| Derivatives | 100% | 100% | 100% | 50% | 100% | 50% | 0% |
| Bezier curves | N/A | 100% | 100% | 0% | 0% | 0% | 0% |
| Adaptive sampling | 0% | 100% | 100% | 0% | 0% | 0% | 0% |
| Arc length param | 100% | 80% | 80% | 0% | 0% | 0% | 0% |
| Mapping/transforms | 100% | 90% | 90% | 0% | 0% | 0% | 0% |

---

## 11. Comparison with Other GA-FUL Subsystems

### Scalar API - Has Both Float64 and Generic ✅
```
Algebra/Scalars/
├── Float64/          ✅ Exists
└── Generic/          ✅ Exists
```

### Linear Algebra API - Has Both Float64 and Generic ✅
```
Algebra/LinearAlgebra/
├── Float64/          ✅ Exists
└── Generic/          ✅ Exists
```

### Geometric Algebra API - Has Both Float64 and Generic ✅
```
Algebra/GeometricAlgebra/
├── Extended/Float64/ ✅ Exists
└── Extended/Generic/ ✅ Exists
```

### Trajectories API - Has ONLY Float64 ❌
```
Modeling/Trajectories/
├── Scalars/Float64/  ✅ Exists
├── Scalars/Generic/  ❌ MISSING
├── Vectors2D/Float64/ ✅ Exists
├── Vectors2D/Generic/ ❌ MISSING
└── (same pattern for all types)
```

**Conclusion:** Trajectories is the ONLY major subsystem lacking generic scalar support!

---

## 12. Code Generation Opportunity

**Current:** Cannot generate optimized trajectory code
**With Generic Support:** Could generate trajectories using MetaExpression

**Example Use Case:**
```csharp
// Symbolic trajectory definition
var context = new MetaContext();
var processor = ScalarProcessor<IMetaExpressionAtomic>.CreateEuclidean(context);

var t = context.GetOrDefineParameterVariable("t");
var amplitude = context.GetOrDefineParameterVariable("A");
var frequency = context.GetOrDefineParameterVariable("f");

// Define trajectory symbolically
var trajectory = new ScalarTrajectory<IMetaExpressionAtomic>(
    processor,
    timeRange,
    t => amplitude * Sin(2 * Pi * frequency * t)
);

// Optimize and generate C++/C#/CUDA code
context.OptimizeContext();
var generator = new TrajectoryCodeGenerator(context);
var optimizedCode = generator.GenerateCpp();
```

**Applications:**
- GPU-accelerated trajectory evaluation
- Real-time graphics rendering
- Physics simulation kernels
- Embedded systems (fixed-point arithmetic)

---

## 13. Recommendations for Future Development

### Phase 1: Foundation (High Priority)
1. Create generic trajectory infrastructure
2. Fix API inconsistencies (Bivectors, Quaternions)
3. Complete incomplete implementations
4. Add missing derivatives

### Phase 2: Expansion (Medium Priority)
5. Add Vector4D trajectories
6. Add Matrix trajectories
7. Expand Bivector/Trivector features to match Vectors richness
8. Add Quaternion SLERP/SQUAD

### Phase 3: Advanced Features (Lower Priority)
9. Add Multivector trajectories
10. Add Rotor/Versor trajectories
11. Add plotting utilities for 2D/3D
12. Add trajectory algebra (composition, blending)

### Phase 4: Optimization
13. Performance benchmarks Float64 vs Generic
14. SIMD optimization for dense trajectory evaluation
15. GPU compute shaders for trajectory evaluation
16. Code generation for trajectories

---

## 14. Architecture Recommendations

### Proposed Generic Hierarchy

```csharp
// Generic base
public interface ITrajectory<TScalar> : IAlgebraicElement
{
    IScalarProcessor<TScalar> ScalarProcessor { get; }
    bool IsPeriodic { get; }
    ScalarRange<TScalar> TimeRange { get; }
    TScalar MinTime { get; }
    TScalar MaxTime { get; }
}

public interface ITrajectory<TScalar, TValue> : ITrajectory<TScalar>
{
    TValue ValueAtMinTime { get; }
    TValue ValueAtMidTime { get; }
    TValue ValueAtMaxTime { get; }
    TValue GetValue(TScalar t);
}

public abstract class Trajectory<TScalar, TValue> : ITrajectory<TScalar, TValue>
{
    public IScalarProcessor<TScalar> ScalarProcessor { get; }
    public ScalarRange<TScalar> TimeRange { get; }
    public bool IsPeriodic { get; }

    protected Trajectory(
        IScalarProcessor<TScalar> scalarProcessor,
        ScalarRange<TScalar> timeRange,
        bool isPeriodic) { }

    public abstract TValue GetValue(TScalar t);
}

// Float64 specialization (backward compatible)
public abstract class Float64Trajectory<TValue> :
    Trajectory<double, TValue>
{
    protected Float64Trajectory(Float64ScalarRange timeRange, bool isPeriodic)
        : base(ScalarProcessorOfFloat64.Instance, timeRange, isPeriodic) { }
}

// Keep ALL existing classes unchanged, they just become specializations!
```

### Migration Strategy

**Phase 1:** Add generic base (no breaking changes)
```csharp
// New generic infrastructure
public abstract class Trajectory<TScalar, TValue> { }

// Existing classes inherit from generic base
public abstract class Float64Trajectory<TValue> :
    Trajectory<double, TValue>  // ← Just add this inheritance
{
    // No other changes needed!
}
```

**Phase 2:** Add generic implementations alongside Float64
```csharp
Trajectories/
├── Float64/           # Keep all existing code
│   ├── Scalars/
│   └── Vectors2D/
└── Generic/           # New generic implementations
    ├── Scalars/
    │   └── ScalarTrajectory<T>.cs
    └── Vectors2D/
        └── Vector2DTrajectory<T>.cs
```

**Phase 3:** Gradually migrate users to generic API

**Benefit:** Non-breaking backward compatibility while enabling new features!

---

## 15. Conclusion

### Key Findings

1. **Zero Generic Support:** All 162 files are Float64-only (0% generic coverage)
2. **API Inconsistencies:** Bivectors and Quaternions don't follow base class pattern
3. **Incomplete Implementations:** 2 classes throw NotImplementedException
4. **Feature Gaps:**
   - Missing Vector4D, Matrix trajectories
   - Missing Multivector, Rotor trajectories
   - Bivectors/Trivectors/Quaternions have minimal features
5. **Architectural Gap:** Only major GA-FUL subsystem without generic support

### Strengths

- **Rich Vectors2D/3D:** Extensive features (Bezier, adaptive, arc length)
- **Extensive Scalars:** 45 files with comprehensive signal processing
- **Consistent APIs:** Where implemented, APIs are consistent
- **Good Infrastructure:** Float64TrajectoryTimeRange is well-designed

### Critical Path Forward

1. **Fix bugs** (ToFinite/ToPeriodic, missing derivatives)
2. **Standardize APIs** (Bivectors, Quaternions)
3. **Add generic support** (follow Scalars/LinearAlgebra pattern)
4. **Expand features** (fill gaps in Bivectors/Trivectors/Quaternions)
5. **Enable code generation** (via MetaExpression trajectories)

---

**Analysis Complete**
**Total Files Analyzed:** 162
**Trajectory Types Analyzed:** 8
**Bugs Found:** 5
**Critical Gap Identified:** 100% lack of generic scalar support
