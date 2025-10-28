using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;

/// <summary>
/// A cubic Bezier curve in 3D space (degree 3) defined by 4 control points
/// Parametric form: B(t) = (1-t)³P₁ + 3(1-t)²tP₂ + 3(1-t)t²P₃ + t³P₄ for t ∈ [0,1]
/// This is the most commonly used Bezier curve for complex smooth curves
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class Bezier3Path3D<T> :
    ParametricPath3D<T>
{
    public static Bezier3Path3D<T> Create(IScalarProcessor<T> scalarProcessor, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2, LinVector3D<T> point3, LinVector3D<T> point4)
    {
        var timeRange = ScalarRange<T>.Create(
            scalarProcessor.Zero,
            scalarProcessor.One
        );

        return new Bezier3Path3D<T>(timeRange, isPeriodic, point1, point2, point3, point4);
    }

    public static Bezier3Path3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2, LinVector3D<T> point3, LinVector3D<T> point4)
    {
        return new Bezier3Path3D<T>(timeRange, isPeriodic, point1, point2, point3, point4);
    }


    public LinVector3D<T> Point1 { get; }

    public LinVector3D<T> Point2 { get; }

    public LinVector3D<T> Point3 { get; }

    public LinVector3D<T> Point4 { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Bezier3Path3D(ScalarRange<T> timeRange, bool isPeriodic, LinVector3D<T> point1, LinVector3D<T> point2, LinVector3D<T> point3, LinVector3D<T> point4)
        : base(timeRange, isPeriodic)
    {
        Point1 = point1;
        Point2 = point2;
        Point3 = point3;
        Point4 = point4;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Point1.IsValid() &&
               Point2.IsValid() &&
               Point3.IsValid() &&
               Point4.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bezier2Path3D<T> GetDerivativeCurve()
    {
        // Derivative of cubic Bezier curve is a quadratic Bezier (degree 2)
        // with control points: 3(P₂-P₁), 3(P₃-P₂), 3(P₄-P₃)
        var processor = Point1.ScalarProcessor;
        var three = processor.ScalarFromNumber(3);

        return Bezier2Path3D<T>.Create(
            TimeRange,
            IsPeriodic,
            three * (Point2 - Point1),
            three * (Point3 - Point2),
            three * (Point4 - Point3)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Cubic Bezier curve using Bernstein basis functions
        // B(t) = B₀(t)P₁ + B₁(t)P₂ + B₂(t)P₃ + B₃(t)P₄
        // where B₀(t) = (1-t)³, B₁(t) = 3(1-t)²t, B₂(t) = 3(1-t)t², B₃(t) = t³
        var (b0, b1, b2, b3) = t.BernsteinBasis_3();

        return LinVector3D<T>.Create(
            b0 * Point1.X + b1 * Point2.X + b2 * Point3.X + b3 * Point4.X,
            b0 * Point1.Y + b1 * Point2.Y + b2 * Point3.Y + b3 * Point4.Y,
            b0 * Point1.Z + b1 * Point2.Z + b2 * Point3.Z + b3 * Point4.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new Bezier3Path3D<T>(TimeRange, false, Point1, Point2, Point3, Point4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new Bezier3Path3D<T>(TimeRange, true, Point1, Point2, Point3, Point4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // First derivative of cubic Bezier curve
        // B'(t) = 3(1-t)²(P₂-P₁) + 6(1-t)t(P₃-P₂) + 3t²(P₄-P₃)
        var processor = t.ScalarProcessor;
        var s = processor.One - t;
        var three = processor.ScalarFromNumber(3);
        var six = processor.ScalarFromNumber(6);

        var p1 = three * s * s;
        var p2 = six * t * s;
        var p3 = three * t * t;

        var d1 = Point2 - Point1;
        var d2 = Point3 - Point2;
        var d3 = Point4 - Point3;

        return LinVector3D<T>.Create(
            p1 * d1.X + p2 * d2.X + p3 * d3.X,
            p1 * d1.Y + p2 * d2.Y + p3 * d3.Y,
            p1 * d1.Z + p2 * d2.Z + p3 * d3.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative of cubic Bezier curve
        // Can be computed by taking derivative of the derivative curve (Bezier2)
        // which results in a Bezier1 (linear) curve
        var derivative2Curve = GetDerivativeCurve().GetDerivativeCurve();

        return derivative2Curve.GetValue(t);
    }
}
