using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;

/// <summary>
/// Bernstein basis functions and DeCasteljau algorithm for Bezier curves with generic scalar type T
/// </summary>
public static class BezierPath3DUtils
{
    #region Bernstein Basis Functions - Degree 0

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_0<T>(this Scalar<T> t)
    {
        return t.ScalarProcessor.One;
    }

    #endregion

    #region Bernstein Basis Functions - Degree 1

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_0_1<T>(this Scalar<T> t)
    {
        return t.ScalarProcessor.One - t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_1_1<T>(this Scalar<T> t)
    {
        return t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Pair<Scalar<T>> BernsteinBasis_1<T>(this Scalar<T> t)
    {
        var processor = t.ScalarProcessor;
        return new Pair<Scalar<T>>(processor.One - t, t);
    }

    #endregion

    #region Bernstein Basis Functions - Degree 2

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_0_2<T>(this Scalar<T> t)
    {
        var s = t.ScalarProcessor.One - t;
        return s * s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_1_2<T>(this Scalar<T> t)
    {
        var processor = t.ScalarProcessor;
        var two = processor.ScalarFromNumber(2);
        return two * (processor.One - t) * t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_2_2<T>(this Scalar<T> t)
    {
        return t * t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Triplet<Scalar<T>> BernsteinBasis_2<T>(this Scalar<T> t)
    {
        var processor = t.ScalarProcessor;
        var s = processor.One - t;
        var two = processor.ScalarFromNumber(2);

        // B₀(t) = (1-t)², B₁(t) = 2(1-t)t, B₂(t) = t²
        // BUGFIX: Float64 version had "t * 3" instead of "t * t" for the third component
        return new Triplet<Scalar<T>>(s * s, two * s * t, t * t);
    }

    #endregion

    #region Bernstein Basis Functions - Degree 3

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_0_3<T>(this Scalar<T> t)
    {
        var s = t.ScalarProcessor.One - t;
        return s * s * s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_1_3<T>(this Scalar<T> t)
    {
        var processor = t.ScalarProcessor;
        var s = processor.One - t;
        var three = processor.ScalarFromNumber(3);

        return three * t * s * s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_2_3<T>(this Scalar<T> t)
    {
        var processor = t.ScalarProcessor;
        var s = processor.One - t;
        var three = processor.ScalarFromNumber(3);

        return three * t * t * s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> BernsteinBasis_3_3<T>(this Scalar<T> t)
    {
        return t * t * t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quad<Scalar<T>> BernsteinBasis_3<T>(this Scalar<T> t)
    {
        var processor = t.ScalarProcessor;
        var three = processor.ScalarFromNumber(3);

        var t2 = t * t;
        var t3 = t * t2;

        var s = processor.One - t;
        var s2 = s * s;
        var s3 = s * s2;

        // B₀(t) = (1-t)³, B₁(t) = 3(1-t)²t, B₂(t) = 3(1-t)t², B₃(t) = t³
        return new Quad<Scalar<T>>(s3, three * s2 * t, three * s * t2, t3);
    }

    #endregion

    #region DeCasteljau Algorithm

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T> DeCasteljau<T>(this Scalar<T> t, LinVector3D<T> p0)
    {
        return p0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T> DeCasteljau<T>(this Scalar<T> t, LinVector3D<T> p0, LinVector3D<T> p1)
    {
        var processor = t.ScalarProcessor;
        var s = processor.One - t;

        return LinVector3D<T>.Create(
            s * p0.X + t * p1.X,
            s * p0.Y + t * p1.Y,
            s * p0.Z + t * p1.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T> DeCasteljau<T>(this Scalar<T> t, LinVector3D<T> p0, LinVector3D<T> p1, LinVector3D<T> p2)
    {
        // De Casteljau's algorithm for quadratic Bezier curve
        // Not using Lerp to increase performance by avoiding allocations
        var processor = t.ScalarProcessor;
        var s = processor.One - t;

        // First stage: interpolate between consecutive control points
        var x0 = s * p0.X + t * p1.X;
        var y0 = s * p0.Y + t * p1.Y;
        var z0 = s * p0.Z + t * p1.Z;

        var x1 = s * p1.X + t * p2.X;
        var y1 = s * p1.Y + t * p2.Y;
        var z1 = s * p1.Z + t * p2.Z;

        // Second stage: interpolate between first-stage results
        return LinVector3D<T>.Create(
            s * x0 + t * x1,
            s * y0 + t * y1,
            s * z0 + t * z1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T> DeCasteljau<T>(this Scalar<T> t, LinVector3D<T> p0, LinVector3D<T> p1, LinVector3D<T> p2, LinVector3D<T> p3)
    {
        // De Casteljau's algorithm for cubic Bezier curve
        // Not using Lerp to increase performance by avoiding allocations
        var processor = t.ScalarProcessor;
        var s = processor.One - t;

        // First stage: interpolate between consecutive control points
        var x0 = s * p0.X + t * p1.X;
        var y0 = s * p0.Y + t * p1.Y;
        var z0 = s * p0.Z + t * p1.Z;

        var x1 = s * p1.X + t * p2.X;
        var y1 = s * p1.Y + t * p2.Y;
        var z1 = s * p1.Z + t * p2.Z;

        var x2 = s * p2.X + t * p3.X;
        var y2 = s * p2.Y + t * p3.Y;
        var z2 = s * p2.Z + t * p3.Z;

        // Second stage: interpolate between first-stage results
        x0 = s * x0 + t * x1;
        y0 = s * y0 + t * y1;
        z0 = s * z0 + t * z1;

        x1 = s * x1 + t * x2;
        y1 = s * y1 + t * y2;
        z1 = s * z1 + t * z2;

        // Third stage: final interpolation
        return LinVector3D<T>.Create(
            s * x0 + t * x1,
            s * y0 + t * y1,
            s * z0 + t * z1
        );
    }

    #endregion
}
