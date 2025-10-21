# GA-FuL Implementation Design - Part 2
## CGa Generic Migration Detailed Phases

**Continuation from IMPLEMENTATION_DESIGN_DOCUMENT.md**

---

## Phase 4.2: CGa Elements Generic (32h)

### Overview

Migrate the 17 files in `Elements/` directory to generic types.

**Key Components**:
- `CGaElement<T>` - Abstract base class
- `CGaRound<T>` - Circles, spheres
- `CGaFlat<T>` - Lines, planes
- `CGaTangent<T>` - Tangent elements
- `CGaDirection<T>` - Direction elements
- `CGaParametricElement<T>` - Time-varying elements

### Task 4.2.1: CGaElement<T> Base Class (12h)

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Generic/Elements/CGaElement.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Elements;

/// <summary>
/// Abstract base class for all CGA elements (generic version).
///
/// Key changes from Float64 version:
/// - double properties → Scalar<T> properties
/// - LinFloat64Vector methods → Generic conversion methods
/// - Visualizer property removed (Float64-only specialization)
/// </summary>
public abstract class CGaElement<T>
    where T : IScalarOps<T>
{
    // ===== Properties =====

    public CGaElementSpecs<T> Specs { get; }

    private Scalar<T> _weight;
    public Scalar<T> Weight
    {
        get => _weight;
        set
        {
            // Validate: weight must be ≥ 0
            if (!IsValidWeight(value.ScalarValue))
                throw new InvalidOperationException("Weight must be >= 0");
            _weight = value;
        }
    }

    public abstract CGaBlade<T> Position { get; }

    public CGaBlade<T> Direction { get; }

    /// <summary>
    /// Normal direction (orthogonal complement)
    /// </summary>
    public CGaBlade<T> NormalDirection
        => Direction.VGaNormal();  // TODO: Implement VGaNormal

    public abstract Scalar<T> RadiusSquared { get; set; }

    public Scalar<T> RealRadius
    {
        get
        {
            var radiusSq = RadiusSquared.ScalarValue;
            var absRadiusSq = T.Abs(radiusSq);
            return T.Sqrt(absRadiusSq).CreateScalar(ScalarProcessor);
        }
    }

    public Scalar<T> RealRadiusSquared
    {
        get
        {
            var radiusSq = RadiusSquared.ScalarValue;
            return T.Abs(radiusSq).CreateScalar(ScalarProcessor);
        }
    }

    // ===== Geometric Space References =====

    public CGaGeometricSpace<T> GeometricSpace
        => Specs.GeometricSpace;

    public IScalarProcessor<T> ScalarProcessor
        => GeometricSpace.ScalarProcessor;

    public int VSpaceDimensions
        => GeometricSpace.VSpaceDimensions;

    // ===== Element Type Properties =====

    public CGaElementKind Kind
        => Specs.Kind;

    public bool IsDirection => Kind == CGaElementKind.Direction;
    public bool IsTangent => Kind == CGaElementKind.Tangent;
    public bool IsFlat => Kind == CGaElementKind.Flat;
    public bool IsRound => Kind == CGaElementKind.Round;

    public bool IsPoint => Direction.Grade == 0;
    public bool IsLine => !IsRound && Direction.Grade == 1;
    public bool IsPlane => !IsRound && Direction.Grade == 2;
    public bool IsVolume => !IsRound && Direction.Grade == 3;

    public bool IsRoundPoint => IsRound && Direction.Grade == 0;
    public bool IsRoundCircle => IsRound && Direction.Grade == 2;
    public bool IsRoundSphere => IsRound && Direction.Grade == 3;

    // ===== Constructor =====

    protected CGaElement(
        CGaGeometricSpace<T> cgaGeometricSpace,
        CGaElementKind kind,
        Scalar<T> weight,
        CGaBlade<T> direction)
    {
        Debug.Assert(direction.IsVGaBlade());

        var directionNorm = direction.Norm();

        if (IsValidWeight(weight.ScalarValue) &&
            !T.IsZero(directionNorm.ScalarValue))
        {
            _weight = weight;
            Direction = direction.Divide(directionNorm.ScalarValue);
        }
        else
        {
            _weight = ScalarProcessor.Zero;
            Direction = cgaGeometricSpace.OneScalarBlade;
        }

        Specs = new CGaElementSpecs<T>(cgaGeometricSpace, kind);
    }

    // ===== Abstract Methods =====

    /// <summary>
    /// Validate element properties
    /// </summary>
    public abstract bool IsValid();

    /// <summary>
    /// Compare with another element
    /// </summary>
    public abstract bool IsSameElement(CGaElement<T> other, bool ignoreWeight = false);

    /// <summary>
    /// Encode as OPNS blade
    /// </summary>
    public abstract CGaBlade<T> EncodeOpnsBlade();

    /// <summary>
    /// Encode as IPNS blade
    /// </summary>
    public abstract CGaBlade<T> EncodeIpnsBlade();

    // ===== Helper Methods =====

    private static bool IsValidWeight(T weight)
    {
        // Check: weight is finite and >= 0
        // For symbolic, always return true (can't validate)
        if (T.Magnitude(weight) == 0.0)
            return true;  // Symbolic - can't check

        var zero = T.Zero;
        return weight >= zero;
    }

    /// <summary>
    /// Convert position to XGaVector (generic)
    /// </summary>
    public XGaVector<T> PositionToXGaVector()
    {
        return Position.InternalKVector.GetVectorPart();
    }

    // Note: LinFloat64Vector conversions moved to Float64-specific extensions
    // Generic version doesn't have Float64-specific types!
}
```

### Task 4.2.2: CGaRound<T> Implementation (8h)

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Generic/Elements/CGaRound.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Elements;

/// <summary>
/// Generic CGA round element (circles, spheres, point pairs).
///
/// Changes from Float64 version:
/// - All double properties → Scalar<T>
/// - LinFloat64Vector methods removed (moved to Float64 extensions)
/// - Rendering-specific methods removed (visualization is Float64-only)
/// </summary>
public class CGaRound<T> : CGaElement<T>
    where T : IScalarOps<T>
{
    // ===== Properties =====

    public override CGaBlade<T> Position { get; }

    public CGaBlade<T> Center => Position;

    private Scalar<T> _radiusSquared;
    public override Scalar<T> RadiusSquared
    {
        get => _radiusSquared;
        set
        {
            // Validate: must be finite
            if (!IsValidRadiusSquared(value.ScalarValue))
                throw new InvalidOperationException("RadiusSquared must be finite");
            _radiusSquared = value;
        }
    }

    // ===== Constructor =====

    internal CGaRound(
        CGaGeometricSpace<T> cgaGeometricSpace,
        Scalar<T> weight,
        Scalar<T> radiusSquared,
        CGaBlade<T> position,
        CGaBlade<T> direction)
        : base(cgaGeometricSpace, CGaElementKind.Round, weight, direction)
    {
        Position = position;

        // Normalize radius to zero if degenerate
        var isDegenerate =
            Direction.IsScalar ||
            T.IsZero(Weight.ScalarValue) ||
            T.IsZero(radiusSquared.ScalarValue);

        _radiusSquared = isDegenerate
            ? ScalarProcessor.Zero
            : radiusSquared;

        Debug.Assert(IsValid());
    }

    // ===== Validation =====

    public sealed override bool IsValid()
    {
        // Check all properties are valid
        if (!IsValidWeight(Weight.ScalarValue))
            return false;

        if (!Position.IsVGaVector())
            return false;

        if (!Direction.IsVGaBlade())
            return false;

        // Direction should be normalized
        var normSq = Direction.NormSquared().ScalarValue;
        var one = T.One;
        if (!T.IsNear(normSq, one))
            return false;

        if (!IsValidRadiusSquared(RadiusSquared.ScalarValue))
            return false;

        return true;
    }

    private static bool IsValidWeight(T weight)
    {
        var zero = T.Zero;
        return weight >= zero;
    }

    private static bool IsValidRadiusSquared(T radiusSquared)
    {
        // For symbolic, can't check - assume valid
        if (T.Magnitude(radiusSquared) == 0.0)
            return true;

        // For numeric, check not infinite
        // (NaN is checked via magnitude)
        return true;  // TODO: Add infinity check
    }

    // ===== Comparison =====

    public override bool IsSameElement(CGaElement<T> element2, bool ignoreWeight = false)
    {
        if (element2 is not CGaRound<T> round2)
            return false;

        if (!ignoreWeight && !T.IsNear(Weight.ScalarValue, round2.Weight.ScalarValue))
            return false;

        if (!T.IsNear(RadiusSquared.ScalarValue, round2.RadiusSquared.ScalarValue))
            return false;

        if (!Center.IsNearEqual(round2.Center))
            return false;

        if (!Direction.IsNearEqual(round2.Direction))
            return false;

        return true;
    }

    // ===== Encoding =====

    public override CGaBlade<T> EncodeOpnsBlade()
    {
        var eo = GeometricSpace.Eo;
        var ei = GeometricSpace.Ei;

        // OPNS encoding: weight * (eo + 0.5 * r² * ei) ∧ direction + translation
        var halfRadiusSq = RadiusSquared.ScalarValue / (T.One + T.One);

        var result = (eo + (halfRadiusSq * ei))
            .Op(Direction)
            .TranslateBy(Position);

        return Weight.ScalarValue * result;
    }

    public override CGaBlade<T> EncodeIpnsBlade()
    {
        var eo = GeometricSpace.Eo;
        var ei = GeometricSpace.Ei;

        // Determine direction sign based on dimensions
        var isEvenDimension = (VSpaceDimensions - 2) % 2 == 0;
        var direction = isEvenDimension ? Direction : -Direction;

        // Dual direction
        var directionDual = direction.VGaDual();

        // IPNS encoding
        var halfRadiusSq = RadiusSquared.ScalarValue / (T.One + T.One);

        var result = (eo - (halfRadiusSq * ei))
            .Op(directionDual)
            .TranslateBy(Position);

        return Weight.ScalarValue * result;
    }

    // ===== Factory Methods =====

    /// <summary>
    /// Create point (zero radius sphere)
    /// </summary>
    public static CGaRound<T> CreatePoint(
        CGaGeometricSpace<T> space,
        CGaBlade<T> position)
    {
        var weight = space.ScalarProcessor.One;
        var radiusSquared = space.ScalarProcessor.Zero;
        var direction = space.OneScalarBlade;

        return new CGaRound<T>(space, weight, radiusSquared, position, direction);
    }

    /// <summary>
    /// Create circle
    /// </summary>
    public static CGaRound<T> CreateCircle(
        CGaGeometricSpace<T> space,
        Scalar<T> radiusSquared,
        CGaBlade<T> center,
        CGaBlade<T> bivector)
    {
        Debug.Assert(bivector.Grade == 2);

        var weight = space.ScalarProcessor.One;

        return new CGaRound<T>(space, weight, radiusSquared, center, bivector);
    }

    /// <summary>
    /// Create sphere
    /// </summary>
    public static CGaRphere<T> CreateSphere(
        CGaGeometricSpace<T> space,
        Scalar<T> radiusSquared,
        CGaBlade<T> center)
    {
        var weight = space.ScalarProcessor.One;
        var direction = space.IeInv;  // Pseudo-scalar direction

        return new CGaRound<T>(space, weight, radiusSquared, center, direction);
    }
}
```

### Task 4.2.3: Remaining Element Types (12h)

**Similar implementations for**:
- `CGaFlat<T>` (lines, planes)
- `CGaTangent<T>` (tangent elements)
- `CGaDirection<T>` (directions)
- `CGaParametricElement<T>` (time-varying)

**Files to create**:
- `CGaFlat.cs`
- `CGaTangent.cs`
- `CGaDirection.cs`
- `CGaParametricElement.cs`
- `CGaElementSpecs.cs`
- `CGaElementKind.cs` (enum)
- `CGaElementEncoding.cs` (enum)

**Composer utils** (7 files):
- `CGaRoundComposerUtils.cs`
- `CGaFlatComposerUtils.cs`
- `CGaTangentComposerUtils.cs`
- `CGaDirectionComposerUtils.cs`
- `CGaParametricRoundComposerUtils.cs`
- `CGaParametricFlatComposerUtils.cs`
- `CGaParametricTangentComposerUtils.cs`

---

## Phase 4.3: CGa Encoding Generic (28h)

### Overview

Migrate 14 encoder files to generic.

**Key Challenge**: Methods currently accept `double` parameters, need to accept `T` with backward-compatible overloads.

### Task 4.3.1: Generic Encoder Base (4h)

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Generic/Encoding/CGaEncoderBase.cs`

```csharp
namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Encoding;

public abstract class CGaEncoderBase<T>
    where T : IScalarOps<T>
{
    public CGaGeometricSpace<T> GeometricSpace { get; }

    public IScalarProcessor<T> ScalarProcessor
        => GeometricSpace.ScalarProcessor;

    protected CGaEncoderBase(CGaGeometricSpace<T> geometricSpace)
    {
        GeometricSpace = geometricSpace;
    }
}
```

### Task 4.3.2: IpnsRound Encoder Generic (8h)

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Generic/Encoding/CGaIpnsRoundEncoder.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Encoding;

/// <summary>
/// Generic IPNS Round encoder.
///
/// Key changes:
/// - All parameters now T (not double)
/// - Convenience overloads for double (backward compat)
/// - LinFloat64Vector removed (generic version uses XGaVector<T>)
/// </summary>
public class CGaIpnsRoundEncoder<T> : CGaEncoderBase<T>
    where T : IScalarOps<T>
{
    internal CGaIpnsRoundEncoder(CGaGeometricSpace<T> geometricSpace)
        : base(geometricSpace)
    {
    }

    // ===== Generic Methods (Primary) =====

    /// <summary>
    /// Encode point from generic coordinates
    /// </summary>
    public CGaBlade<T> Point(T x, T y, T z)
    {
        Debug.Assert(GeometricSpace.Is5D);

        var position = GeometricSpace.EncodeVGaVector(x, y, z);
        return Point(position);
    }

    /// <summary>
    /// Encode point from XGaVector
    /// </summary>
    public CGaBlade<T> Point(XGaVector<T> egaPoint)
    {
        // CGA encoding: P = p + 0.5 * |p|² * e∞ + e₀
        var eo = GeometricSpace.Eo;
        var ei = GeometricSpace.Ei;

        var normSquared = egaPoint.NormSquared().ScalarValue;
        var half = T.One / (T.One + T.One);
        var halfNormSq = half * normSquared;

        return GeometricSpace.ConformalProcessor
            .CreateVectorComposer()
            .SetVector(eo.InternalVector)
            .AddVector(egaPoint)
            .SetVectorTerm(
                GeometricSpace.VSpaceDimensions - 1,
                halfNormSq
            )
            .GetVector()
            .CreateCGaBlade(GeometricSpace);
    }

    /// <summary>
    /// Encode circle from generic parameters
    /// </summary>
    public CGaBlade<T> Circle(T radiusSquared, T centerX, T centerY)
    {
        Debug.Assert(GeometricSpace.Is4D);

        var center = GeometricSpace.EncodeVGaVector(centerX, centerY);
        return HyperSphere(radiusSquared, center);
    }

    /// <summary>
    /// Encode sphere from generic parameters
    /// </summary>
    public CGaBlade<T> Sphere(T cx, T cy, T cz, T radiusSquared)
    {
        Debug.Assert(GeometricSpace.Is5D);

        var center = GeometricSpace.EncodeVGaVector(cx, cy, cz);
        return HyperSphere(radiusSquared, center);
    }

    /// <summary>
    /// Encode hyper-sphere (generic dimensions)
    /// </summary>
    public CGaBlade<T> HyperSphere(T radiusSquared, XGaVector<T> egaCenter)
    {
        var eo = GeometricSpace.Eo;
        var ei = GeometricSpace.Ei;

        // IPNS sphere: s = C - 0.5 * r² * e∞
        // where C = point encoding of center
        var centerPoint = Point(egaCenter);
        var half = T.One / (T.One + T.One);
        var halfRadiusSq = half * radiusSquared;

        return centerPoint - (halfRadiusSq * ei);
    }

    // ===== Backward Compatible Overloads (for double) =====

    /// <summary>
    /// Encode point from double coordinates (backward compat)
    /// </summary>
    public CGaBlade<T> Point(double x, double y, double z)
    {
        var xT = ScalarProcessor.ScalarFromNumber(x).ScalarValue;
        var yT = ScalarProcessor.ScalarFromNumber(y).ScalarValue;
        var zT = ScalarProcessor.ScalarFromNumber(z).ScalarValue;

        return Point(xT, yT, zT);
    }

    /// <summary>
    /// Encode circle from double parameters (backward compat)
    /// </summary>
    public CGaBlade<T> Circle(double radiusSquared, double centerX, double centerY)
    {
        var rT = ScalarProcessor.ScalarFromNumber(radiusSquared).ScalarValue;
        var xT = ScalarProcessor.ScalarFromNumber(centerX).ScalarValue;
        var yT = ScalarProcessor.ScalarFromNumber(centerY).ScalarValue;

        return Circle(rT, xT, yT);
    }

    /// <summary>
    /// Encode sphere from double parameters (backward compat)
    /// </summary>
    public CGaBlade<T> Sphere(double cx, double cy, double cz, double radiusSquared)
    {
        var xT = ScalarProcessor.ScalarFromNumber(cx).ScalarValue;
        var yT = ScalarProcessor.ScalarFromNumber(cy).ScalarValue;
        var zT = ScalarProcessor.ScalarFromNumber(cz).ScalarValue;
        var rT = ScalarProcessor.ScalarFromNumber(radiusSquared).ScalarValue;

        return Sphere(xT, yT, zT, rT);
    }

    // ... implement RealCircle, ImaginaryCircle, etc.
    // All with BOTH generic T and double overloads
}
```

### Task 4.3.3: Remaining Encoders (16h)

**Implement generic versions of** (13 files):
1. `CGaEncoder.cs` - Main encoder facade
2. `CGaOpnsRoundEncoder.cs`
3. `CGaIpnsFlatEncoder.cs`
4. `CGaOpnsFlatEncoder.cs`
5. `CGaIpnsTangentEncoder.cs`
6. `CGaOpnsTangentEncoder.cs`
7. `CGaIpnsDirectionEncoder.cs`
8. `CGaOpnsDirectionEncoder.cs`
9. `CGaVGaEncoder.cs` - Vector GA encoding
10. `CGaPGaEncoder.cs` - Projective GA encoding
11. `CGaHGaEncoder.cs` - Hyperbolic GA encoding
12. `CGaEncoderUtils.cs` - Utility methods
13. `CGaEncoderBase.cs`

**Pattern for all**:
- Primary methods accept generic `T` parameters
- Overloads accept `double` for backward compatibility
- LinFloat64Vector removed, use XGaVector<T>
- All return `CGaBlade<T>`

---

## Phase 4.4: CGa Decoding Generic (28h)

### Overview

Migrate 11 decoder files to generic.

**Key Challenge**: Decoders extract scalar values - need to return `Scalar<T>` not `double`.

### Task 4.4.1: Generic Decoder Base (4h)

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Generic/Decoding/CGaBladeDecoderBase.cs`

```csharp
namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Decoding;

public abstract class CGaBladeDecoderBase<T>
    where T : IScalarOps<T>
{
    public CGaBlade<T> Blade { get; }

    public CGaGeometricSpace<T> GeometricSpace
        => Blade.ConformalProcessor.GeometricSpace;

    public IScalarProcessor<T> ScalarProcessor
        => GeometricSpace.ScalarProcessor;

    protected CGaBladeDecoderBase(CGaBlade<T> blade)
    {
        Blade = blade;
    }
}
```

### Task 4.4.2: IpnsRound Decoder Generic (8h)

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Generic/Decoding/CGaIpnsRoundBladeDecoder.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Elements;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Decoding;

/// <summary>
/// Generic IPNS Round decoder.
///
/// Key changes:
/// - All returned scalars are Scalar<T> (not double)
/// - LinFloat64Vector removed
/// - Returns CGaRound<T> (not CGaFloat64Round)
/// </summary>
public class CGaIpnsRoundBladeDecoder<T> : CGaBladeDecoderBase<T>
    where T : IScalarOps<T>
{
    internal CGaIpnsRoundBladeDecoder(CGaBlade<T> blade)
        : base(blade)
    {
    }

    /// <summary>
    /// Decode as Round element
    /// </summary>
    public CGaRound<T> DecodeRound()
    {
        // Extract components from IPNS blade
        var weight = GetWeight();
        var radiusSquared = GetRadiusSquared();
        var center = GetCenter();
        var direction = GetDirection();

        return new CGaRound<T>(
            GeometricSpace,
            weight,
            radiusSquared,
            center,
            direction
        );
    }

    /// <summary>
    /// Get weight (generic scalar)
    /// </summary>
    public Scalar<T> GetWeight()
    {
        // Weight is the norm of the blade
        return Blade.Norm();
    }

    /// <summary>
    /// Get squared radius (generic scalar)
    /// </summary>
    public Scalar<T> GetRadiusSquared()
    {
        var eo = GeometricSpace.Eo;
        var ei = GeometricSpace.Ei;

        // r² = -2 * (blade · e₀) / (blade · e∞)
        var bladeEo = Blade.Lcp(eo);
        var bladeEi = Blade.Lcp(ei);

        var numerator = bladeEo.Sp(eo);  // Scalar<T>
        var denominator = bladeEi.Sp(ei);  // Scalar<T>

        var two = T.One + T.One;
        var minusTwo = -two;

        var radiusSquared = (minusTwo * numerator.ScalarValue) / denominator.ScalarValue;

        return radiusSquared.CreateScalar(ScalarProcessor);
    }

    /// <summary>
    /// Get center position
    /// </summary>
    public CGaBlade<T> GetCenter()
    {
        var ei = GeometricSpace.Ei;

        // Center = (blade · e∞) / (blade · e∞ · e∞)
        var bladeEi = Blade.Lcp(ei);
        var norm = bladeEi.Sp(ei);

        return bladeEi.Divide(norm.ScalarValue);
    }

    /// <summary>
    /// Get direction blade
    /// </summary>
    public CGaBlade<T> GetDirection()
    {
        var center = GetCenter();

        // Direction = blade - center contribution
        // (complex calculation involving dual, etc.)

        // ... implementation details

        return direction.Normalize();
    }

    /// <summary>
    /// Check if blade represents a point
    /// </summary>
    public bool IsPoint()
    {
        var radiusSquared = GetRadiusSquared();
        return T.IsZero(radiusSquared.ScalarValue);
    }

    /// <summary>
    /// Check if blade represents a circle
    /// </summary>
    public bool IsCircle()
    {
        return Blade.Grade == 3 && GeometricSpace.Is4D;
    }

    /// <summary>
    /// Check if blade represents a sphere
    /// </summary>
    public bool IsSphere()
    {
        return Blade.Grade == 4 && GeometricSpace.Is5D;
    }
}
```

### Task 4.4.3: Remaining Decoders (16h)

**Implement generic versions of** (10 files):
1. `CGaBladeDecoder.cs` - Main decoder facade
2. `CGaOpnsRoundBladeDecoder.cs`
3. `CGaIpnsFlatBladeDecoder.cs`
4. `CGaOpnsFlatBladeDecoder.cs`
5. `CGaIpnsTangentBladeDecoder.cs`
6. `CGaOpnsTangentBladeDecoder.cs`
7. `CGaIpnsDirectionBladeDecoder.cs`
8. `CGaOpnsDirectionBladeDecoder.cs`
9. `CGaVGaDirectionBladeDecoder.cs`
10. `CGaPGaFlatBladeDecoder.cs`
11. `CGaDecoderUtils.cs`

---

## Phase 4.5: CGa Operations Generic (20h)

### Overview

Migrate 7 operation utility files to generic.

**Files**:
1. `CGaRotationUtils.cs` - Rotations
2. `CGaTranslationUtils.cs` - Translations
3. `CGaScalingUtils.cs` - Scalings
4. `CGaReflectionUtils.cs` - Reflections
5. `CGaProjectionUtils.cs` - Projections
6. `CGaMeetUtils.cs` - Meet operations
7. `CGaMappingUtils.cs` - General mappings

### Example: CGaRotationUtils<T>

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Generic/Operations/CGaRotationUtils.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Operations;

/// <summary>
/// Generic CGA rotation utilities
/// </summary>
public static class CGaRotationUtils<T>
    where T : IScalarOps<T>
{
    /// <summary>
    /// Create rotation rotor from angle and bivector
    /// </summary>
    public static CGaBlade<T> CreateRotor(
        CGaGeometricSpace<T> space,
        T angle,
        CGaBlade<T> bivector)
    {
        var halfAngle = angle / (T.One + T.One);
        var cosHalf = T.Cos(halfAngle);
        var sinHalf = T.Sin(halfAngle);

        var bivectorNorm = bivector.Norm().ScalarValue;
        var bivectorUnit = bivector.Divide(bivectorNorm);

        return space.CreateScalar(cosHalf) - (sinHalf * bivectorUnit);
    }

    /// <summary>
    /// Apply rotation to blade
    /// </summary>
    public static CGaBlade<T> Rotate(
        CGaBlade<T> blade,
        CGaBlade<T> rotor)
    {
        var rotorReverse = rotor.Reverse();
        return rotor.Gp(blade).Gp(rotorReverse);
    }

    // ... more rotation utilities
}
```

---

## Phase 4.6: CGa Interpolation Generic (24h)

### Overview

Migrate 13 Lerp (linear interpolation) utility files.

**Pattern**: All methods take parameter `t` which could be `T` (symbolic time) or `double`.

### Example Implementation

```csharp
public static class CGaLerpRoundUtils<T>
    where T : IScalarOps<T>
{
    /// <summary>
    /// Linear interpolation between two round elements
    /// </summary>
    public static CGaRound<T> LerpRound(
        CGaRound<T> round1,
        CGaRound<T> round2,
        T t)  // Interpolation parameter (0 to 1)
    {
        // Lerp center
        var center1 = round1.Center.InternalVector;
        var center2 = round2.Center.InternalVector;
        var centerLerp = center1 + (t * (center2 - center1));

        // Lerp radius
        var r1 = round1.RadiusSquared.ScalarValue;
        var r2 = round2.RadiusSquared.ScalarValue;
        var rLerp = r1 + (t * (r2 - r1));

        // Lerp direction (slerp for better results)
        var directionLerp = SlerpBlades(round1.Direction, round2.Direction, t);

        var space = round1.GeometricSpace;
        return new CGaRound<T>(
            space,
            space.ScalarProcessor.One,
            rLerp.CreateScalar(space.ScalarProcessor),
            centerLerp.CreateCGaBlade(space),
            directionLerp
        );
    }

    private static CGaBlade<T> SlerpBlades(
        CGaBlade<T> blade1,
        CGaBlade<T> blade2,
        T t)
    {
        // Spherical linear interpolation
        // ... implementation
    }
}
```

---

## Phase 4.7: CGa Versors Generic (16h)

### Overview

Migrate 3 versor files to generic.

**Files**:
1. `CGaVersor.cs`
2. `ICGaParametricVersor.cs`
3. `CGaVersorComposerUtils.cs`

---

## Phase 4.8: Visualizer Integration (8h)

### Overview

**Key Decision**: Visualizer stays Float64-only!

**Solution**: Conversion layer

### Float64 Specialization

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Float64/CGaFloat64GeometricSpace5D.cs`

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Visualizer;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;

/// <summary>
/// Float64-specific specialization with Visualizer.
/// Inherits from generic CGaGeometricSpace5D<double>.
/// </summary>
public sealed class CGaFloat64GeometricSpace5D : CGaGeometricSpace5D<FloatingScalar<double>>
{
    // Singleton pattern preserved for backward compatibility
    public static CGaFloat64GeometricSpace5D Instance { get; } = new();

    // Float64-only Visualizer
    public CGaFloat64Visualizer Visualizer { get; }

    private CGaFloat64GeometricSpace5D()
        : base(FloatingScalarProcessor<double>.Instance)
    {
        Visualizer = new CGaFloat64Visualizer(this);
    }
}
```

### Conversion Extensions

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Generic/Extensions/CGaConversionExtensions.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Extensions;

/// <summary>
/// Conversion extensions for visualization
/// </summary>
public static class CGaConversionExtensions
{
    /// <summary>
    /// Convert generic CGaBlade to Float64 for visualization
    /// </summary>
    public static CGaBlade<FloatingScalar<double>> ToFloat64<T>(this CGaBlade<T> blade)
        where T : IScalarOps<T>
    {
        var scalarProcessor = FloatingScalarProcessor<double>.Instance;
        var processor = CGaProcessor<FloatingScalar<double>>.Create(scalarProcessor);

        // Convert all scalar coefficients to Float64
        var float64KVector = blade.InternalKVector.MapScalars(
            scalar => new FloatingScalar<double>(T.Magnitude(scalar))
        );

        return new CGaBlade<FloatingScalar<double>>(processor, float64KVector);
    }

    /// <summary>
    /// Convert Float32 to Float64 (simple widening)
    /// </summary>
    public static CGaBlade<FloatingScalar<double>> ToFloat64(
        this CGaBlade<FloatingScalar<float>> blade)
    {
        var scalarProcessor = FloatingScalarProcessor<double>.Instance;
        var processor = CGaProcessor<FloatingScalar<double>>.Create(scalarProcessor);

        var float64KVector = blade.InternalKVector.MapScalars(
            scalar => new FloatingScalar<double>((double)scalar.Value)
        );

        return new CGaBlade<FloatingScalar<double>>(processor, float64KVector);
    }

    /// <summary>
    /// Convert element to Float64
    /// </summary>
    public static CGaElement<FloatingScalar<double>> ToFloat64<T>(
        this CGaElement<T> element)
        where T : IScalarOps<T>
    {
        // Convert all components to Float64
        // Return appropriate element type
        // ... implementation
    }
}
```

### Usage Pattern

```csharp
// Develop with Float32
var cga32 = new CGaGeometricSpace5D<FloatingScalar<float>>(...);
var sphere = cga32.Encode.IpnsRound.Sphere(0.0f, 0.0f, 0.0f, 5.0f);

// Convert to Float64 for visualization
var sphere64 = sphere.ToFloat64();

// Visualize
var cgaVis = CGaFloat64GeometricSpace5D.Instance;
cgaVis.Visualizer.DrawSphere(sphere64.DecodeIpnsRound());
```

---

## 7. Testing Strategy

### 7.1 Unit Test Structure

```
GeometricAlgebraFulcrumLib.UnitTests/
├── Algebra/Scalars/
│   ├── IScalarOpsTests.cs (50 tests)
│   ├── FloatingScalarTests.cs (100 tests)
│   └── MetaExpressionScalarOpsTests.cs (30 tests)
├── Integration/
│   ├── UnifiedWorkflowTests.cs (20 tests)
│   ├── Float32WorkflowTests.cs (30 tests)
│   ├── SymbolicWorkflowTests.cs (40 tests)
│   └── CodeGenerationTests.cs (30 tests)
├── Modeling/CGa/Generic/
│   ├── CGaBladeTests.cs (80 tests)
│   ├── CGaElementTests.cs (150 tests)
│   ├── CGaEncodingTests.cs (180 tests)
│   ├── CGaDecodingTests.cs (150 tests)
│   └── CGaOperationsTests.cs (100 tests)
└── Performance/
    ├── FloatingScalarBenchmarks.cs
    ├── GenericAlgorithmBenchmarks.cs
    └── CGaPerformanceBenchmarks.cs
```

### 7.2 Test Categories

**Category 1: Correctness** (P0)
- All operations produce mathematically correct results
- Float32 and Float64 produce consistent results (within precision)
- Symbolic AST structure is correct

**Category 2: Performance** (P1)
- FloatingScalar<float> ≥ 99% native float performance
- FloatingScalar<double> ≥ 99% native double performance
- Generic algorithms ≤ 2% overhead

**Category 3: Compatibility** (P1)
- Backward compatibility with existing Float64 code
- Type aliases work correctly
- Migration path is smooth

**Category 4: Edge Cases** (P2)
- Zero divisions handled
- NaN, Infinity handled
- Empty/degenerate geometric objects

### 7.3 Performance Benchmarks

**Target Metrics**:
```
| Operation | Native | FloatingScalar | Generic | Overhead |
|-----------|--------|----------------|---------|----------|
| Addition | 1.0 ns | 1.01 ns | 1.02 ns | 1-2% ✅ |
| Sqrt | 3.2 ns | 3.25 ns | 3.30 ns | 1-3% ✅ |
| Sin | 15 ns | 15.2 ns | 15.5 ns | 1-3% ✅ |
| GA Rotor | 450 ns | 455 ns | 465 ns | 1-3% ✅ |
| CGA Sphere | 850 ns | 860 ns | 880 ns | 1-4% ✅ |
```

---

## 8. Migration Guide

### 8.1 Migration for Existing Code

**Scenario 1: Code using XGa (already generic)**
```csharp
// BEFORE (works unchanged):
var processor = XGaFloat64Processor.Euclidean;
var vector = processor.Vector(1.0, 2.0, 3.0);
var result = vector.Gp(vector.Reverse());

// AFTER (also works, alternative):
var processor = XGaProcessorFactory.CreateFloat64Euclidean();
var vector = processor.Vector(1.0, 2.0, 3.0);
var result = vector.Gp(vector.Reverse());

// NEW (Float32):
var processor = XGaProcessorFactory.CreateFloat32Euclidean();
var vector = processor.Vector(1.0f, 2.0f, 3.0f);
var result = vector.Gp(vector.Reverse());
```
**Migration effort**: ZERO (backward compatible)

**Scenario 2: Code using CGa Float64**
```csharp
// BEFORE:
var cga = CGaFloat64GeometricSpace5D.Instance;
var point = cga.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
double radius = point.Decode().RadiusSquared;

// AFTER (unchanged - works via aliases):
var cga = CGaFloat64GeometricSpace5D.Instance;
var point = cga.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
double radius = point.Decode().RadiusSquared.ScalarValue;  // ← ADD .ScalarValue

// NEW (Float32):
var cga = new CGaGeometricSpace5D<FloatingScalar<float>>(...);
var point = cga.Encode.IpnsRound.Point(1.0f, 2.0f, 3.0f);
float radius = point.Decode().RadiusSquared.ScalarValue.Value;
```
**Migration effort**: 2-4 hours (add `.ScalarValue` where needed)

### 8.2 Common Migration Patterns

**Pattern 1: Scalar Property Access**
```csharp
// BEFORE:
double weight = element.Weight;

// AFTER:
double weight = element.Weight.ScalarValue;
// OR (if using generic):
var weight = element.Weight.ScalarValue;  // Type is T
```

**Pattern 2: Indexer Access**
```csharp
// BEFORE:
double coeff = blade[0];

// AFTER:
double coeff = blade[0].ScalarValue;
// OR (generic):
var coeff = blade[0].ScalarValue;  // Type is T
```

**Pattern 3: Encoder Parameters**
```csharp
// BEFORE:
var circle = cga.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);

// AFTER (unchanged - overloads preserved):
var circle = cga.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);

// NEW (generic):
var circle = cga.Encode.IpnsRound.Circle(r, x, y);  // T parameters
```

---

## 9. Implementation Checklist

### Phase 1: IScalarOps (24h) ✅ or ❌

#### Foundation
- [ ] Create `IScalarOps.cs` interface (4h)
  - [ ] Define all 30+ operations
  - [ ] XML documentation complete
  - [ ] Usage examples in comments

#### Symbolic Adapter
- [ ] Create `MetaExpressionScalarOps.cs` (8h)
  - [ ] Implement all IScalarOps members
  - [ ] Verify AST building works
  - [ ] Handle context threading

#### Testing
- [ ] Create `IScalarOpsTests.cs` (8h)
  - [ ] 50+ unit tests
  - [ ] Test with FloatingScalar
  - [ ] Test with MetaExpression
  - [ ] Test symbolic AST building

#### Documentation
- [ ] Create `IScalarOps_Guide.md` (4h)
  - [ ] Basic usage examples
  - [ ] Numeric vs symbolic patterns
  - [ ] Best practices

---

### Phase 2: FloatingScalar (20h) ✅ or ❌

#### Implementation
- [ ] Create `FloatingScalar.cs` (8h)
  - [ ] All IScalarOps operations
  - [ ] AggressiveInlining on all methods
  - [ ] Implicit conversions

- [ ] Create `FloatingScalarProcessor.cs` (4h)
  - [ ] IScalarProcessor implementation
  - [ ] Singleton pattern
  - [ ] All operations delegated

#### Testing
- [ ] Create `FloatingScalarTests.cs` (4h)
  - [ ] 100+ unit tests
  - [ ] Float32 tests
  - [ ] Float64 tests
  - [ ] Edge cases

#### Performance
- [ ] Create `FloatingScalarBenchmarks.cs` (4h)
  - [ ] Benchmark vs native
  - [ ] Verify ≥99% performance
  - [ ] Zero allocations verified

---

### Phase 3: XGa Integration (24h) ✅ or ❌

#### Factory Methods
- [ ] Create `XGaProcessorFactory.cs` (4h)
  - [ ] Float32 factories
  - [ ] Float64 factories
  - [ ] Symbolic factories

#### Auditing
- [ ] Audit `XGaProcessor<T>` (8h)
  - [ ] Check for Float64 hardcoding
  - [ ] Fix any issues
  - [ ] Test with FloatingScalar<float>
  - [ ] Test with IMetaExpressionAtomic

#### Integration Testing
- [ ] Create `UnifiedWorkflowTests.cs` (8h)
  - [ ] Float32 workflow test
  - [ ] Float64 workflow test
  - [ ] Symbolic workflow test
  - [ ] Consistency tests

#### Documentation
- [ ] Create "Unified Workflow Guide" (4h)
  - [ ] Complete examples
  - [ ] All three workflows documented

---

### Phase 4.1: CGa Foundation (24h) ✅ or ❌

#### Core Types
- [ ] Create `CGaBlade.cs` (12h)
  - [ ] Generic implementation
  - [ ] All operations
  - [ ] XML docs

- [ ] Create `CGaProcessor.cs` (4h)
  - [ ] Generic processor
  - [ ] Factory methods

- [ ] Create `CGaGeometricSpace.cs` (4h)
  - [ ] Generic base class
  - [ ] Metric handling

#### Compatibility
- [ ] Create backward compat aliases (2h)
  - [ ] CGaFloat64Blade alias
  - [ ] CGaFloat64Processor alias

#### Testing
- [ ] Create `CGaBladeTests.cs` (2h)
  - [ ] 80+ tests
  - [ ] Operations tests

---

### Phase 4.2: CGa Elements (32h) ✅ or ❌

#### Base Class
- [ ] Create `CGaElement.cs` (12h)
  - [ ] Generic base class
  - [ ] All abstract methods
  - [ ] Property conversions (double → Scalar<T>)

#### Element Types
- [ ] Create `CGaRound.cs` (8h)
- [ ] Create `CGaFlat.cs` (4h)
- [ ] Create `CGaTangent.cs` (4h)
- [ ] Create `CGaDirection.cs` (2h)
- [ ] Create `CGaParametricElement.cs` (2h)

---

### Phase 4.3: CGa Encoding (28h) ✅ or ❌

#### Encoders
- [ ] Create `CGaIpnsRoundEncoder.cs` (8h)
  - [ ] Generic methods
  - [ ] Double overloads
- [ ] Create `CGaOpnsRoundEncoder.cs` (4h)
- [ ] Create `CGaIpnsFlatEncoder.cs` (4h)
- [ ] Create `CGaOpnsFlatEncoder.cs` (4h)
- [ ] Create remaining 9 encoders (8h)

---

### Phase 4.4: CGa Decoding (28h) ✅ or ❌

#### Decoders
- [ ] Create `CGaIpnsRoundBladeDecoder.cs` (8h)
  - [ ] Return Scalar<T> not double
- [ ] Create `CGaOpnsRoundBladeDecoder.cs` (4h)
- [ ] Create `CGaIpnsFlatBladeDecoder.cs` (4h)
- [ ] Create remaining 8 decoders (12h)

---

### Phase 4.5: CGa Operations (20h) ✅ or ❌

#### Operations
- [ ] Create `CGaRotationUtils.cs` (4h)
- [ ] Create `CGaTranslationUtils.cs` (3h)
- [ ] Create `CGaScalingUtils.cs` (3h)
- [ ] Create `CGaReflectionUtils.cs` (3h)
- [ ] Create `CGaProjectionUtils.cs` (3h)
- [ ] Create `CGaMeetUtils.cs` (2h)
- [ ] Create `CGaMappingUtils.cs` (2h)

---

### Phase 4.6: CGa Interpolation (24h) ✅ or ❌

#### Lerp Utilities
- [ ] Create 13 Lerp utility files (24h)
  - Each file: ~2h implementation + tests

---

### Phase 4.7: CGa Versors (16h) ✅ or ❌

#### Versors
- [ ] Create `CGaVersor.cs` (8h)
- [ ] Create `ICGaParametricVersor.cs` (4h)
- [ ] Create `CGaVersorComposerUtils.cs` (4h)

---

### Phase 4.8: Visualizer Integration (8h) ✅ or ❌

#### Conversion Layer
- [ ] Create `CGaConversionExtensions.cs` (4h)
  - [ ] ToFloat64 for all types
  - [ ] Specialized conversions

#### Float64 Specialization
- [ ] Update `CGaFloat64GeometricSpace5D.cs` (2h)
  - [ ] Inherit from generic
  - [ ] Add Visualizer property

#### Documentation
- [ ] Document visualization pattern (2h)
  - [ ] Conversion workflow
  - [ ] Examples

---

### Final Integration (20h) ✅ or ❌

#### End-to-End Testing
- [ ] Create complete workflow tests (8h)
  - [ ] Float32 → Symbolic → GLSL
  - [ ] CGa operations end-to-end
  - [ ] Performance validated

#### Documentation
- [ ] Complete migration guide (6h)
- [ ] API documentation (4h)
- [ ] Examples repository (2h)

---

## 10. Appendix: Complete Code Examples

### Example 1: Complete Rotor Algorithm

```csharp
/// <summary>
/// Complete generic rotor rotation algorithm.
/// Works with Float32, Float64, Symbolic, any IScalarOps type.
/// </summary>
public static class RotorAlgorithms
{
    public static XGaVector<T> RotateVector<T>(
        XGaProcessor<T> processor,
        XGaVector<T> vector,
        T angle) where T : IScalarOps<T>
    {
        // Compute half angle
        var two = T.One + T.One;
        var halfAngle = angle / two;

        // Trigonometry
        var cosHalf = T.Cos(halfAngle);
        var sinHalf = T.Sin(halfAngle);

        // Build rotor in e₁₂ plane: R = cos(θ/2) - sin(θ/2) e₁₂
        var rotor = processor.CreateMultivectorComposer()
            .SetTerm(0, cosHalf)         // Scalar part
            .SetTerm(3, -sinHalf)        // e₁₂ bivector part
            .GetMultivector();

        // Apply rotation: v' = R v R†
        var rotorReverse = rotor.Reverse();
        var rotated = rotor.Gp(vector).Gp(rotorReverse);

        return rotated.GetVectorPart();
    }

    // Usage example
    public static void DemonstrationFloat32()
    {
        var processor = XGaProcessorFactory.CreateFloat32Euclidean();
        var vector = processor.Vector(1.0f, 0.0f, 0.0f);
        var angle = new FloatingScalar<float>(MathF.PI / 4.0f);

        var result = RotateVector(processor, vector, angle);

        // Result: (0.707, 0.707, 0) - 45 degree rotation
    }

    public static void DemonstrationSymbolic()
    {
        var context = new MetaContext();
        var processor = XGaProcessorFactory.CreateSymbolicEuclidean(context);

        var vx = context["vx"];
        var vy = context["vy"];
        var vz = context["vz"];
        var angle = context["angle"];

        var vector = processor.Vector(vx, vy, vz);
        var result = RotateVector(processor, vector, angle);

        context.GetOrDefineOutputVariable("rx", result[0].ScalarValue);
        context.GetOrDefineOutputVariable("ry", result[1].ScalarValue);
        context.GetOrDefineOutputVariable("rz", result[2].ScalarValue);

        context.OptimizeContext();
        var glslCode = GenerateGLSL(context);

        // Result: Optimized GLSL shader function
    }
}
```

### Example 2: Complete CGa Sphere Intersection

```csharp
/// <summary>
/// Complete sphere-plane intersection using generic CGA
/// </summary>
public static class CGaIntersections
{
    public static CGaBlade<T> SphereP laneIntersection<T>(
        CGaGeometricSpace5D<T> cga,
        T sphereCx, T sphereCy, T sphereCz, T sphereRadius,
        T planeNx, T planeNy, T planeNz, T planeDistance)
        where T : IScalarOps<T>
    {
        // Encode sphere (IPNS)
        var sphere = cga.Encode.IpnsRound.Sphere(
            sphereCx, sphereCy, sphereCz, sphereRadius
        );

        // Encode plane (IPNS)
        var plane = cga.Encode.IpnsFlat.Plane(
            planeNx, planeNy, planeNz, planeDistance
        );

        // Intersection via outer product
        var intersection = sphere.Op(plane);

        // Result is a circle (or point pair if tangent)
        return intersection;
    }

    // Usage: Float32 development
    public static void DemoFloat32()
    {
        var cga = new CGaGeometricSpace5D<FloatingScalar<float>>(...);

        var intersection = SphereP laneIntersection(
            cga,
            0.0f, 0.0f, 0.0f, 5.0f,  // Sphere at origin, radius 5
            0.0f, 0.0f, 1.0f, 2.0f   // Horizontal plane at z=2
        );

        var circle = intersection.DecodeIpnsRound();

        // Visualize: convert to Float64
        var circle64 = circle.ToFloat64();
        CGaFloat64GeometricSpace5D.Instance.Visualizer.DrawCircle(circle64);
    }

    // Usage: Symbolic code generation
    public static void DemoSymbolic()
    {
        var context = new MetaContext();
        var cga = new CGaGeometricSpace5D<IMetaExpressionAtomic>(context);

        var cx = context["sphereCenterX"];
        var cy = context["sphereCenterY"];
        var cz = context["sphereCenterZ"];
        var r = context["sphereRadius"];
        var nx = context["planeNormalX"];
        var ny = context["planeNormalY"];
        var nz = context["planeNormalZ"];
        var d = context["planeDistance"];

        var intersection = SphereP laneIntersection(
            cga, cx, cy, cz, r, nx, ny, nz, d
        );

        var circle = intersection.DecodeIpnsRound();

        context.GetOrDefineOutputVariable("circleCenterX", circle.Center[0]);
        context.GetOrDefineOutputVariable("circleCenterY", circle.Center[1]);
        context.GetOrDefineOutputVariable("circleCenterZ", circle.Center[2]);
        context.GetOrDefineOutputVariable("circleRadius", circle.RealRadius.ScalarValue);

        context.OptimizeContext();
        var glslCode = GenerateGLSL(context);

        // Result: Optimized shader for sphere-plane intersection
    }
}
```

---

## Summary

This design document provides COMPLETE, IMPLEMENTATION-READY specifications for:

1. **Phase 1-3**: IScalarOps + FloatingScalar + XGa Integration (68h)
2. **Phase 4**: CGa Generic Migration (200h)
   - 4.1: Foundation (24h)
   - 4.2: Elements (32h)
   - 4.3: Encoding (28h)
   - 4.4: Decoding (28h)
   - 4.5: Operations (20h)
   - 4.6: Interpolation (24h)
   - 4.7: Versors (16h)
   - 4.8: Visualizer (8h)
3. **Testing & Documentation** (20h)

**Total**: 260 hours = 13 weeks @ 20h/week

**Next Step**: Start with Phase 1 (IScalarOps Foundation) - most critical, blocks everything else!
