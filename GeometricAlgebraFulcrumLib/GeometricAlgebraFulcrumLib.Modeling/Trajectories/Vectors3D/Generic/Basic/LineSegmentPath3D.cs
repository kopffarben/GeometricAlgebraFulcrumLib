using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;

/// <summary>
/// A straight line segment between two 3D points with generic scalar type T
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class LineSegmentPath3D<T> :
    ArcLengthPath3D<T>
{
    public static LineSegmentPath3D<T> Create(bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2)
    {
        var scalarProcessor = point1.ScalarProcessor;
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.Zero,
            scalarProcessor.One
        );

        return new LineSegmentPath3D<T>(timeRange, isPeriodic, point1, point2);
    }

    public static LineSegmentPath3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2)
    {
        return new LineSegmentPath3D<T>(timeRange, isPeriodic, point1, point2);
    }


    public LinVector3D<T> Point1 { get; }

    public LinVector3D<T> Point2 { get; }

    public LinVector3D<T> Direction { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LineSegmentPath3D(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2)
        : base(timeRange, isPeriodic)
    {
        Point1 = point1;
        Point2 = point2;
        Direction = Point2 - Point1;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetLength()
    {
        return Direction.Norm();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> TimeToLength(Scalar<T> t)
    {
        var scalarProcessor = t.ScalarProcessor;
        var tClamped = ClampPeriodic(t, scalarProcessor.One);

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
        // Simple modulo for periodic clamping
        // For non-periodic, this just ensures value is in [0, maxValue]
        var scalarProcessor = value.ScalarProcessor;

        if (value < scalarProcessor.Zero)
            return scalarProcessor.Zero;

        if (value > maxValue)
            return maxValue;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Point1.IsValid() &&
               Point2.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Normalize t from TimeRange to [0,1]
        var scalarProcessor = t.ScalarProcessor;
        var tNormalized = (t - TimeRange.MinValue) / (TimeRange.MaxValue - TimeRange.MinValue);
        var oneMinusT = scalarProcessor.One - tNormalized;

        return oneMinusT * Point1 + tNormalized * Point2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToFiniteArcLengthPath()
    {
        return IsPeriodic
            ? new LineSegmentPath3D<T>(TimeRange, false, Point1, Point2)
            : this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToPeriodicArcLengthPath()
    {
        return IsFinite
            ? new LineSegmentPath3D<T>(TimeRange, true, Point1, Point2)
            : this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // Derivative of linear interpolation is constant: Point2 - Point1
        return Direction;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        var scalarProcessor = t.ScalarProcessor;
        var tClamped = ClampPeriodic(t, scalarProcessor.One);

        return ParametricPath3DLocalFrame<T>.Create(
            tClamped,
            GetValue(t),
            Direction.ToUnitLinVector3D()
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative of linear path is zero (no acceleration)
        return LinVector3D<T>.Zero(Point1.ScalarProcessor);
    }
}
