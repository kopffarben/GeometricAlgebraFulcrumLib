using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;

/// <summary>
/// A constant 2D trajectory that returns the same position for all time values
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ConstantPath2D<T> :
    ParametricPath2D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantPath2D<T> Create(IScalarProcessor<T> scalarProcessor, LinVector2D<T> point)
    {
        var timeRange = ScalarRange<T>.Create(
            -scalarProcessor.One,
            scalarProcessor.One
        );

        return new ConstantPath2D<T>(
            timeRange,
            false,
            point,
            LinVector2D<T>.Zero(scalarProcessor)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantPath2D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, LinVector2D<T> point)
    {
        return new ConstantPath2D<T>(
            timeRange,
            isPeriodic,
            point,
            LinVector2D<T>.Zero(point.ScalarProcessor)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantPath2D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, LinVector2D<T> point, LinVector2D<T> tangent)
    {
        return new ConstantPath2D<T>(
            timeRange,
            isPeriodic,
            point,
            tangent
        );
    }


    public LinVector2D<T> Point { get; }

    public LinVector2D<T> Tangent { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ConstantPath2D(ScalarRange<T> timeRange, bool isPeriodic, LinVector2D<T> point, LinVector2D<T> tangent)
        : base(timeRange, isPeriodic)
    {
        Point = point;
        Tangent = tangent;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Point.IsValid() &&
               Tangent.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new ConstantPath2D<T>(TimeRange, false, Point, Tangent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new ConstantPath2D<T>(TimeRange, true, Point, Tangent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        return Point;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        return Tangent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        return LinVector2D<T>.Zero(Point.ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        return ParametricPath2DLocalFrame<T>.Create(
            t,
            Point,
            Tangent
        );
    }
}
