using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Matrices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Circles;

/// <summary>
/// A circular path in 3D space with arbitrary orientation.
/// The circle is defined by a center point, unit normal vector, radius, and rotation count.
/// Internally uses an axis-aligned base circle rotated to match the desired orientation.
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class CirclePath3D<T> :
    ArcLengthPath3D<T>
{
    private readonly AxisAlignedCirclePath3D<T> _baseCircle;
    private readonly SquareMatrix3<T> _baseCircleRotation;
    private readonly IScalarProcessor<T> _scalarProcessor;

    public Scalar<T> Radius { get; }

    public LinVector3D<T> Center { get; }

    public LinVector3D<T> UnitNormal { get; }

    public int RotationCount { get; }


    public static CirclePath3D<T> Create(IScalarProcessor<T> scalarProcessor, ILinVector3D<T> center, ILinVector3D<T> unitNormal, T radius, int rotationCount = 1)
    {
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.ScalarFromValue(scalarProcessor.ZeroValue),
            scalarProcessor.ScalarFromValue(scalarProcessor.OneValue)
        );

        return new CirclePath3D<T>(timeRange, scalarProcessor, center, unitNormal, radius, rotationCount);
    }

    public static CirclePath3D<T> Create(ScalarRange<T> timeRange, IScalarProcessor<T> scalarProcessor, ILinVector3D<T> center, ILinVector3D<T> unitNormal, T radius, int rotationCount = 1)
    {
        return new CirclePath3D<T>(timeRange, scalarProcessor, center, unitNormal, radius, rotationCount);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CirclePath3D(ScalarRange<T> timeRange, IScalarProcessor<T> scalarProcessor, ILinVector3D<T> center, ILinVector3D<T> unitNormal, T radius, int rotationCount)
        : base(timeRange, true) // Circles are always periodic
    {
        if (scalarProcessor.IsNegative(radius))
            throw new ArgumentException("Radius must be non-negative", nameof(radius));

        if (rotationCount == 0 || rotationCount < -100 || rotationCount > 100)
            throw new ArgumentException("Rotation count must be non-zero and in range [-100, 100]", nameof(rotationCount));

        _scalarProcessor = scalarProcessor;
        RotationCount = Math.Abs(rotationCount);
        Center = LinVector3D<T>.Create(center.X, center.Y, center.Z);
        UnitNormal = LinVector3D<T>.Create(unitNormal.X, unitNormal.Y, unitNormal.Z);
        Radius = scalarProcessor.ScalarFromValue(radius);

        // Find the nearest basis axis to the unit normal
        var axis = unitNormal.SelectNearestBasisVector();

        // If the axis is negative, reverse the rotation direction
        if (axis.IsNegative)
            rotationCount = -rotationCount;

        // Create an axis-aligned base circle in the plane perpendicular to the nearest axis
        if (axis.IsXAxis())
            _baseCircle = YzCirclePath3D<T>.Create(timeRange, scalarProcessor, radius, rotationCount);
        else if (axis.IsYAxis())
            _baseCircle = ZxCirclePath3D<T>.Create(timeRange, scalarProcessor, radius, rotationCount);
        else
            _baseCircle = XyCirclePath3D<T>.Create(timeRange, scalarProcessor, radius, rotationCount);

        // Create rotation matrix from the base axis to the desired normal
        _baseCircleRotation = SquareMatrix3<T>.CreateAxisToVectorRotationMatrix3D(axis, unitNormal);

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return _scalarProcessor.IsValid(Radius.ScalarValue) &&
               Radius.IsPositive() &&
               Center.IsValid() &&
               UnitNormal.IsValid() &&
               UnitNormal.IsNearUnitVector() &&
               RotationCount != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Transform base circle point: Rotate and translate
        return _baseCircleRotation * _baseCircle.GetValue(t) + Center;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // Transform base circle tangent: Only rotate (no translation for vectors)
        return _baseCircleRotation * _baseCircle.GetDerivative1Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Transform base circle acceleration: Only rotate (no translation for vectors)
        return _baseCircleRotation * _baseCircle.GetDerivative2Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        var frame = _baseCircle.GetFrame(t);

        return ParametricPath3DLocalFrame<T>.Create(
            frame.TimeValue,
            _baseCircleRotation * frame.Point + Center,
            _baseCircleRotation * frame.Tangent
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetLength()
    {
        // Length = |2π * Radius * RotationCount|
        var circumference = _scalarProcessor.PiTimes2 * Radius;
        var totalLength = circumference * _scalarProcessor.ScalarFromNumber(RotationCount);

        return totalLength.Abs();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> TimeToLength(Scalar<T> t)
    {
        var tClamped = ClampPeriodic(t, _scalarProcessor.One);
        return tClamped * GetLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> LengthToTime(Scalar<T> length)
    {
        var curveLength = GetLength();
        var lengthClamped = ClampPeriodic(length, curveLength);

        return lengthClamped / curveLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Scalar<T> ClampPeriodic(Scalar<T> value, Scalar<T> maxValue)
    {
        // Simple clamping for periodic values
        // Ensures value is in [0, maxValue]
        if (value.IsNegative())
            return _scalarProcessor.Zero;

        if ((value - maxValue).IsPositive())
            return maxValue;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToFiniteArcLengthPath()
    {
        return new CirclePath3D<T>(TimeRange, _scalarProcessor, Center, UnitNormal, Radius.ScalarValue, Math.Abs(RotationCount));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToPeriodicArcLengthPath()
    {
        // Already periodic
        return this;
    }
}
