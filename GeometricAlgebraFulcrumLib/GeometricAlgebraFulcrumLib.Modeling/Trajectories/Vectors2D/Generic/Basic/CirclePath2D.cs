using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;

/// <summary>
/// A circular arc path in 2D with generic scalar type T.
/// The circle is parameterized by angle from 0 to 2π (one complete rotation).
/// </summary>
/// <remarks>
/// NOTE: This implementation CORRECTLY translates by Center, fixing a bug in Float64CirclePath2D
/// which returns positions relative to origin without adding Center.
/// </remarks>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class CirclePath2D<T> :
    ArcLengthPath2D<T>
{
    private readonly IScalarProcessor<T> _scalarProcessor;
    private readonly Scalar<T> _directionFactor;

    public bool ReverseDirection { get; }

    public Scalar<T> Radius { get; }

    public LinVector2D<T> Center { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CirclePath2D<T> Create(IScalarProcessor<T> scalarProcessor, LinVector2D<T> center, T radius, bool reverseDirection = false)
    {
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.Zero,
            scalarProcessor.One
        );

        return new CirclePath2D<T>(timeRange, scalarProcessor, center, radius, reverseDirection);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CirclePath2D<T> Create(ScalarRange<T> timeRange, IScalarProcessor<T> scalarProcessor, LinVector2D<T> center, T radius, bool reverseDirection = false)
    {
        return new CirclePath2D<T>(timeRange, scalarProcessor, center, radius, reverseDirection);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CirclePath2D(ScalarRange<T> timeRange, IScalarProcessor<T> scalarProcessor, LinVector2D<T> center, T radius, bool reverseDirection)
        : base(timeRange, true) // Circles are always periodic
    {
        if (scalarProcessor.IsNegative(radius))
            throw new ArgumentException("Radius must be non-negative", nameof(radius));

        _scalarProcessor = scalarProcessor;
        ReverseDirection = reverseDirection;
        Radius = scalarProcessor.ScalarFromValue(radius);
        Center = center;

        // Direction factor: 2π for counter-clockwise, -2π for clockwise
        _directionFactor = reverseDirection
            ? scalarProcessor.PiTimes2.Negative()
            : scalarProcessor.PiTimes2;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return _scalarProcessor.IsValid(Radius.ScalarValue) &&
               !Radius.IsNegative() &&
               Center.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        // Parametric circle: (cos(θ), sin(θ)) * radius + center
        // where θ = t * directionFactor (0 to 2π)
        var angle = t * _directionFactor;
        var cosAngle = _scalarProcessor.Cos(angle.ScalarValue);
        var sinAngle = _scalarProcessor.Sin(angle.ScalarValue);

        var x = Radius * cosAngle;
        var y = Radius * sinAngle;

        var point = LinVector2D<T>.Create(x, y);

        return point + Center;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        // Derivative: d/dt[(cos(θ), sin(θ)) * r] where θ = t * directionFactor
        // = (-sin(θ), cos(θ)) * r * directionFactor
        var angle = t * _directionFactor;
        var cosAngle = _scalarProcessor.Cos(angle.ScalarValue);
        var sinAngle = _scalarProcessor.Sin(angle.ScalarValue);

        var magnitude = Radius * _directionFactor;
        var x = magnitude * (-sinAngle);
        var y = magnitude * cosAngle;

        return LinVector2D<T>.Create(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative: d²/dt²[(cos(θ), sin(θ)) * r]
        // = (-cos(θ), -sin(θ)) * r * directionFactor²
        var angle = t * _directionFactor;
        var cosAngle = _scalarProcessor.Cos(angle.ScalarValue);
        var sinAngle = _scalarProcessor.Sin(angle.ScalarValue);

        var magnitude = Radius * _directionFactor * _directionFactor;
        var x = magnitude * (-cosAngle);
        var y = magnitude * (-sinAngle);

        return LinVector2D<T>.Create(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        var angle = t * _directionFactor;
        var cosAngle = _scalarProcessor.Cos(angle.ScalarValue);
        var sinAngle = _scalarProcessor.Sin(angle.ScalarValue);

        // Point on circle
        var x = Radius * cosAngle;
        var y = Radius * sinAngle;
        var point = LinVector2D<T>.Create(x, y) + Center;

        // Unit tangent (derivative normalized)
        // For a circle, tangent is perpendicular to radius: (-sin, cos) or (sin, -cos)
        Scalar<T> tangentX, tangentY;
        if (ReverseDirection)
        {
            tangentX = sinAngle;
            tangentY = -cosAngle;
        }
        else
        {
            tangentX = -sinAngle;
            tangentY = cosAngle;
        }

        var tangent = LinVector2D<T>.Create(tangentX, tangentY);

        return ParametricPath2DLocalFrame<T>.Create(
            t,
            point,
            tangent
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetLength()
    {
        // Circumference = 2πr
        return _scalarProcessor.PiTimes2 * Radius;
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
        if (value.IsNegative())
            return _scalarProcessor.Zero;

        if ((value - maxValue).IsPositive())
            return maxValue;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath2D<T> ToFiniteArcLengthPath()
    {
        // Circles are inherently periodic, but we can return a non-periodic version
        // by changing the isPeriodic flag (though the geometric shape remains circular)
        return IsPeriodic
            ? new CirclePath2D<T>(TimeRange, _scalarProcessor, Center, Radius.ScalarValue, ReverseDirection)
            : this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath2D<T> ToPeriodicArcLengthPath()
    {
        // Already periodic
        return this;
    }
}
