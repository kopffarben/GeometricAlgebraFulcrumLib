using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Bezier;

/// <summary>
/// A constant Bezier curve in 2D space (degree 0) defined by 1 control point
/// This is the trivial case where the curve is simply a fixed point in space
/// Parametric form: B(t) = P₁ for all t ∈ [0,1]
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class Bezier0Path2D<T> :
    ParametricPath2D<T>
{
    public static Bezier0Path2D<T> Create(IScalarProcessor<T> scalarProcessor, bool isPeriodic, LinVector2D<T> point1)
    {
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.Zero,
            scalarProcessor.One
        );

        return new Bezier0Path2D<T>(timeRange, isPeriodic, point1);
    }

    public static Bezier0Path2D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, LinVector2D<T> point1)
    {
        return new Bezier0Path2D<T>(timeRange, isPeriodic, point1);
    }


    public LinVector2D<T> Point1 { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Bezier0Path2D(ScalarRange<T> timeRange, bool isPeriodic, LinVector2D<T> point1)
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
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        // Degree 0 Bezier curve is constant: B(t) = P₁
        return Point1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new Bezier0Path2D<T>(TimeRange, false, Point1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new Bezier0Path2D<T>(TimeRange, true, Point1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        // First derivative of constant curve is zero (no velocity)
        return LinVector2D<T>.Zero(Point1.ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative of constant curve is zero (no acceleration)
        return LinVector2D<T>.Zero(Point1.ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        // For constant curve, tangent can be arbitrary unit vector
        // Using UnitSymmetric as 3D version does
        return ParametricPath2DLocalFrame<T>.Create(
            t,
            Point1,
            LinVector2D<T>.UnitSymmetric(Point1.ScalarProcessor)
        );
    }
}
