using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Circles;

/// <summary>
/// A circular path in the ZX plane with generic scalar type T.
/// The circle is centered at origin with radius R.
/// Parameterized as: x = R*sin(t*2π*n), y = 0, z = R*cos(t*2π*n)
/// where n is the rotation count.
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ZxCirclePath3D<T> :
    AxisAlignedCirclePath3D<T>
{
    public static ZxCirclePath3D<T> Create(IScalarProcessor<T> scalarProcessor, T radius, int rotationCount = 1)
    {
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.ScalarFromValue(scalarProcessor.ZeroValue),
            scalarProcessor.ScalarFromValue(scalarProcessor.OneValue)
        );

        return new ZxCirclePath3D<T>(timeRange, scalarProcessor, radius, rotationCount);
    }

    public static ZxCirclePath3D<T> Create(ScalarRange<T> timeRange, IScalarProcessor<T> scalarProcessor, T radius, int rotationCount = 1)
    {
        return new ZxCirclePath3D<T>(timeRange, scalarProcessor, radius, rotationCount);
    }


    private readonly Scalar<T> _directionFactor;
    private readonly IScalarProcessor<T> _scalarProcessor;

    public override Scalar<T> Radius { get; }

    public override LinVector3D<T> Center { get; }

    public override LinVector3D<T> UnitNormal { get; }

    public override int RotationCount { get; }

    public bool ReverseDirection { get; init; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ZxCirclePath3D(ScalarRange<T> timeRange, IScalarProcessor<T> scalarProcessor, T radius, int rotationCount)
        : base(timeRange, true) // Circles are always periodic
    {
        if (rotationCount == 0 || rotationCount < -100 || rotationCount > 100)
            throw new ArgumentException("Rotation count must be non-zero and in range [-100, 100]", nameof(rotationCount));

        _scalarProcessor = scalarProcessor;
        RotationCount = rotationCount;
        Radius = scalarProcessor.ScalarFromValue(radius);

        // Direction factor = 2π * rotationCount (using PiTimes2 = Tau)
        var tau = scalarProcessor.PiTimes2;
        var rotCount = scalarProcessor.ScalarFromNumber(rotationCount);
        _directionFactor = tau * rotCount;

        Center = LinVector3D<T>.Zero(_scalarProcessor);
        UnitNormal = ReverseDirection
            ? LinVector3D<T>.NegativeE2(_scalarProcessor)
            : LinVector3D<T>.E2(_scalarProcessor);

        Debug.Assert(IsValid());
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
    public override bool IsValid()
    {
        return _scalarProcessor.IsValid(Radius.ScalarValue) &&
               Radius.IsPositive() &&
               RotationCount != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Position: (R*sin(angle), 0, R*cos(angle))
        // where angle = t * 2π * rotationCount
        var angle = t * _directionFactor;
        var cos = _scalarProcessor.Cos(angle.ScalarValue);
        var sin = _scalarProcessor.Sin(angle.ScalarValue);

        var x = Radius * sin;
        var z = Radius * cos;

        return LinVector3D<T>.Create(x, _scalarProcessor.Zero, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToFiniteArcLengthPath()
    {
        return new ZxCirclePath3D<T>(TimeRange, _scalarProcessor, Radius.ScalarValue, RotationCount)
        {
            ReverseDirection = ReverseDirection
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToPeriodicArcLengthPath()
    {
        // Already periodic
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // First derivative (velocity/tangent):
        // dx/dt =  R * 2π * n * cos(angle)
        // dy/dt = 0
        // dz/dt = -R * 2π * n * sin(angle)
        var angle = t * _directionFactor;
        var cos = _scalarProcessor.Cos(angle.ScalarValue);
        var sin = _scalarProcessor.Sin(angle.ScalarValue);

        var magnitude = Radius * _directionFactor;

        var x = magnitude * cos;
        var z = magnitude * (-sin);

        return LinVector3D<T>.Create(x, _scalarProcessor.Zero, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        var tClamped = ClampPeriodic(t, _scalarProcessor.One);
        var tangent = GetDerivative1Value(t).ToUnitLinVector3D();

        return ParametricPath3DLocalFrame<T>.Create(
            tClamped,
            GetValue(t),
            tangent
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative (acceleration, points toward center):
        // d²x/dt² = -R * (2π * n)² * sin(angle)
        // d²y/dt² = 0
        // d²z/dt² = -R * (2π * n)² * cos(angle)
        var angle = t * _directionFactor;
        var cos = _scalarProcessor.Cos(angle.ScalarValue);
        var sin = _scalarProcessor.Sin(angle.ScalarValue);

        var magnitudeSquared = _directionFactor * _directionFactor;
        var magnitude = Radius * magnitudeSquared;

        var x = magnitude * (-sin);
        var z = magnitude * (-cos);

        return LinVector3D<T>.Create(x, _scalarProcessor.Zero, z);
    }
}
