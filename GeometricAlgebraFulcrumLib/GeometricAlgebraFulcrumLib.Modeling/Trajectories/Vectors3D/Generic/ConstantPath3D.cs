using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Float64;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

public sealed class ConstantPath3D<T> :
    ParametricPath3D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantPath3D<T> Finite(IScalarProcessor<T> processor, LinVector3D<T> point)
    {
        return new ConstantPath3D<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            point,
            LinVector3D<T>.Zero(processor)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantPath3D<T> Finite(ScalarRange<T> timeRange, LinVector3D<T> point)
    {
        return new ConstantPath3D<T>(
            timeRange,
            false,
            point,
            LinVector3D<T>.Zero(timeRange.MinValue.ScalarProcessor)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantPath3D<T> Finite(IScalarProcessor<T> processor, Scalar<T> pointX, Scalar<T> pointY, Scalar<T> pointZ)
    {
        return new ConstantPath3D<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            LinVector3D<T>.Create(pointX, pointY, pointZ),
            LinVector3D<T>.Zero(processor)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantPath3D<T> Finite(IScalarProcessor<T> processor, LinVector3D<T> point, LinVector3D<T> tangent)
    {
        return new ConstantPath3D<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            point,
            tangent
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantPath3D<T> Finite(ScalarRange<T> timeRange, LinVector3D<T> point, LinVector3D<T> tangent)
    {
        return new ConstantPath3D<T>(
            timeRange,
            false,
            point,
            tangent
        );
    }


    public LinVector3D<T> Point { get; }

    public LinVector3D<T> Tangent { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ConstantPath3D(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point, LinVector3D<T> tangent)
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
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new ConstantPath3D<T>(TimeRange, false, Point, Tangent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new ConstantPath3D<T>(TimeRange, true, Point, Tangent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        return Point;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        return Tangent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        return LinVector3D<T>.Zero(Point.X.ScalarProcessor);
    }
}
