using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;

/// <summary>
/// A quadratic Bezier curve in 3D space defined by 3 control points
/// Parametric form: B(t) = (1-t)²P₁ + 2(1-t)tP₂ + t²P₃ for t ∈ [0,1]
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class Bezier2Path3D<T> :
    ParametricPath3D<T>
{
    public static Bezier2Path3D<T> Create(IScalarProcessor<T> scalarProcessor, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2, LinVector3D<T> point3)
    {
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.Zero,
            scalarProcessor.One
        );

        return new Bezier2Path3D<T>(timeRange, isPeriodic, point1, point2, point3);
    }

    public static Bezier2Path3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2, LinVector3D<T> point3)
    {
        return new Bezier2Path3D<T>(timeRange, isPeriodic, point1, point2, point3);
    }


    public LinVector3D<T> Point1 { get; }

    public LinVector3D<T> Point2 { get; }

    public LinVector3D<T> Point3 { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Bezier2Path3D(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2, LinVector3D<T> point3)
        : base(timeRange, isPeriodic)
    {
        Point1 = point1;
        Point2 = point2;
        Point3 = point3;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Point1.IsValid() &&
               Point2.IsValid() &&
               Point3.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bezier1Path3D<T> GetDerivativeCurve()
    {
        // Derivative of quadratic Bezier curve is a linear Bezier (degree 1)
        // with control points: 2(P₂-P₁), 2(P₃-P₂)
        var processor = Point1.ScalarProcessor;
        var two = processor.ScalarFromNumber(2);

        return Bezier1Path3D<T>.Create(
            TimeRange,
            IsPeriodic,
            two * (Point2 - Point1),
            two * (Point3 - Point2)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Quadratic Bezier curve using Bernstein basis functions
        // B(t) = B₀(t)P₁ + B₁(t)P₂ + B₂(t)P₃
        // where B₀(t) = (1-t)², B₁(t) = 2(1-t)t, B₂(t) = t²
        var (b0, b1, b2) = t.BernsteinBasis_2();

        return LinVector3D<T>.Create(
            b0 * Point1.X + b1 * Point2.X + b2 * Point3.X,
            b0 * Point1.Y + b1 * Point2.Y + b2 * Point3.Y,
            b0 * Point1.Z + b1 * Point2.Z + b2 * Point3.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new Bezier2Path3D<T>(TimeRange, false, Point1, Point2, Point3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new Bezier2Path3D<T>(TimeRange, true, Point1, Point2, Point3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // First derivative of quadratic Bezier curve
        // B'(t) = 2(1-t)(P₂-P₁) + 2t(P₃-P₂)
        // Simplifies to: B'(t) = 2[(1-t)(P₂-P₁) + t(P₃-P₂)]
        var processor = t.ScalarProcessor;
        var s = processor.One - t;
        var two = processor.ScalarFromNumber(2);

        var d1 = Point2 - Point1;
        var d2 = Point3 - Point2;

        return LinVector3D<T>.Create(
            two * (s * d1.X + t * d2.X),
            two * (s * d1.Y + t * d2.Y),
            two * (s * d1.Z + t * d2.Z)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative of quadratic Bezier curve
        // B''(t) = 2(P₃ - 2P₂ + P₁)
        // This is constant for quadratic Bezier curves
        var processor = t.ScalarProcessor;
        var two = processor.ScalarFromNumber(2);

        var d = Point3 - two * Point2 + Point1;

        return two * d;
    }
}
