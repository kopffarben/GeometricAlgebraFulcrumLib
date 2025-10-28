using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;

/// <summary>
/// A linear Bezier curve in 3D space (degree 1) defined by 2 control points
/// Parametric form: B(t) = (1-t)P₁ + tP₂ for t ∈ [0,1]
/// This is equivalent to linear interpolation (Lerp) between two points
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class Bezier1Path3D<T> :
    ParametricPath3D<T>
{
    public static Bezier1Path3D<T> Create(IScalarProcessor<T> scalarProcessor, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2)
    {
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.Zero,
            scalarProcessor.One
        );

        return new Bezier1Path3D<T>(timeRange, isPeriodic, point1, point2);
    }

    public static Bezier1Path3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2)
    {
        return new Bezier1Path3D<T>(timeRange, isPeriodic, point1, point2);
    }


    public LinVector3D<T> Point1 { get; }

    public LinVector3D<T> Point2 { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Bezier1Path3D(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2)
        : base(timeRange, isPeriodic)
    {
        Point1 = point1;
        Point2 = point2;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Point1.IsValid() &&
               Point2.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bezier0Path3D<T> GetDerivativeCurve()
    {
        // Derivative of linear Bezier curve is a constant (degree 0 Bezier)
        return Bezier0Path3D<T>.Create(TimeRange, IsPeriodic, Point2 - Point1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Linear Bezier curve using Bernstein basis functions
        // B(t) = B₀(t)P₁ + B₁(t)P₂
        // where B₀(t) = (1-t), B₁(t) = t
        var (b0, b1) = t.BernsteinBasis_1();

        return LinVector3D<T>.Create(
            b0 * Point1.X + b1 * Point2.X,
            b0 * Point1.Y + b1 * Point2.Y,
            b0 * Point1.Z + b1 * Point2.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new Bezier1Path3D<T>(TimeRange, false, Point1, Point2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new Bezier1Path3D<T>(TimeRange, true, Point1, Point2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // First derivative of linear Bezier curve is constant (velocity is constant)
        // B'(t) = P₂ - P₁
        return Point2 - Point1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative of linear Bezier curve is zero (no acceleration)
        return LinVector3D<T>.Zero(Point1.ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        // For linear curve, tangent is constant: normalized direction vector
        var tangent = GetDerivative1Value(t);
        var tangentNorm = tangent.Norm();

        var normalizedTangent = tangent.ScalarProcessor.IsZero(tangentNorm.ScalarValue)
            ? LinVector3D<T>.UnitSymmetric(tangent.ScalarProcessor)
            : tangent / tangentNorm;

        return ParametricPath3DLocalFrame<T>.Create(
            t,
            GetValue(t),
            normalizedTangent
        );
    }
}
