using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;

/// <summary>
/// A straight line segment between two 2D points with generic scalar type T
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class LineSegmentPath2D<T> :
    ArcLengthPath2D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LineSegmentPath2D<T> Create(bool isPeriodic, LinVector2D<T> point1, LinVector2D<T> point2)
    {
        var scalarProcessor = point1.ScalarProcessor;
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.Zero,
            scalarProcessor.One
        );

        return new LineSegmentPath2D<T>(timeRange, isPeriodic, point1, point2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LineSegmentPath2D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, LinVector2D<T> point1, LinVector2D<T> point2)
    {
        return new LineSegmentPath2D<T>(timeRange, isPeriodic, point1, point2);
    }


    public LinVector2D<T> Point1 { get; }

    public LinVector2D<T> Point2 { get; }

    public LinVector2D<T> Direction { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LineSegmentPath2D(ScalarRange<T> timeRange, bool isPeriodic, LinVector2D<T> point1, LinVector2D<T> point2)
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
        // Simple clamping for periodic values
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
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        // Linear interpolation: (1-t) * Point1 + t * Point2
        var scalarProcessor = t.ScalarProcessor;
        var oneMinusT = scalarProcessor.One - t;

        return oneMinusT * Point1 + t * Point2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath2D<T> ToFiniteArcLengthPath()
    {
        return IsPeriodic
            ? new LineSegmentPath2D<T>(TimeRange, false, Point1, Point2)
            : this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath2D<T> ToPeriodicArcLengthPath()
    {
        return IsFinite
            ? new LineSegmentPath2D<T>(TimeRange, true, Point1, Point2)
            : this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        // Derivative of linear interpolation is constant: Point2 - Point1
        return Direction;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative of linear path is zero (no acceleration)
        return LinVector2D<T>.Zero(Point1.ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        var scalarProcessor = t.ScalarProcessor;
        var tClamped = ClampPeriodic(t, scalarProcessor.One);

        return ParametricPath2DLocalFrame<T>.Create(
            tClamped,
            GetValue(t),
            Direction.ToUnitLinVector2D()
        );
    }
}
