using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;

/// <summary>
/// A constant Bezier curve in 3D space (degree 0) defined by 1 control point
/// This is the trivial case where the curve is simply a fixed point in space
/// Parametric form: B(t) = P₁ for all t ∈ [0,1]
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class Bezier0Path3D<T> :
    ParametricPath3D<T>
{
    public static Bezier0Path3D<T> Create(IScalarProcessor<T> scalarProcessor, bool isPeriodic, LinVector3D<T> point1)
    {
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.Zero,
            scalarProcessor.One
        );

        return new Bezier0Path3D<T>(timeRange, isPeriodic, point1);
    }

    public static Bezier0Path3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1)
    {
        return new Bezier0Path3D<T>(timeRange, isPeriodic, point1);
    }


    public LinVector3D<T> Point1 { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Bezier0Path3D(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1)
        : base(timeRange, isPeriodic)
    {
        Point1 = point1;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Point1.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Degree 0 Bezier curve is constant: B(t) = P₁
        return Point1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new Bezier0Path3D<T>(TimeRange, false, Point1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new Bezier0Path3D<T>(TimeRange, true, Point1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // First derivative of constant curve is zero (no velocity)
        return LinVector3D<T>.Zero(Point1.ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative of constant curve is zero (no acceleration)
        return LinVector3D<T>.Zero(Point1.ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        // For constant curve, tangent can be arbitrary unit vector
        // Using UnitSymmetric as Float64 version does
        return ParametricPath3DLocalFrame<T>.Create(
            t,
            Point1,
            LinVector3D<T>.UnitSymmetric(Point1.ScalarProcessor)
        );
    }
}
