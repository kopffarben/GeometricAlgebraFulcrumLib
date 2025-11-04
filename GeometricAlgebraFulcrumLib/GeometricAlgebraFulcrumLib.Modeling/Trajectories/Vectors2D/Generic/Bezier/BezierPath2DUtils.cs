using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Bezier;

/// <summary>
/// Bernstein basis functions and DeCasteljau algorithm for 2D Bezier curves with generic scalar type T
/// </summary>
public static class BezierPath2DUtils
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
    public static LinVector2D<T> DeCasteljau<T>(this Scalar<T> t, LinVector2D<T> p0)
    {
        return p0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector2D<T> DeCasteljau<T>(this Scalar<T> t, LinVector2D<T> p0, LinVector2D<T> p1)
    {
        var processor = t.ScalarProcessor;
        var s = processor.One - t;

        return LinVector2D<T>.Create(
            s * p0.X + t * p1.X,
            s * p0.Y + t * p1.Y
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector2D<T> DeCasteljau<T>(this Scalar<T> t, LinVector2D<T> p0, LinVector2D<T> p1, LinVector2D<T> p2)
    {
        // De Casteljau's algorithm for quadratic Bezier curve
        // Not using Lerp to increase performance by avoiding allocations
        var processor = t.ScalarProcessor;
        var s = processor.One - t;

        // First stage: interpolate between consecutive control points
        var x0 = s * p0.X + t * p1.X;
        var y0 = s * p0.Y + t * p1.Y;

        var x1 = s * p1.X + t * p2.X;
        var y1 = s * p1.Y + t * p2.Y;

        // Second stage: interpolate between first-stage results
        return LinVector2D<T>.Create(
            s * x0 + t * x1,
            s * y0 + t * y1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector2D<T> DeCasteljau<T>(this Scalar<T> t, LinVector2D<T> p0, LinVector2D<T> p1, LinVector2D<T> p2, LinVector2D<T> p3)
    {
        // De Casteljau's algorithm for cubic Bezier curve
        // Not using Lerp to increase performance by avoiding allocations
        var processor = t.ScalarProcessor;
        var s = processor.One - t;

        // First stage: interpolate between consecutive control points
        var x0 = s * p0.X + t * p1.X;
        var y0 = s * p0.Y + t * p1.Y;

        var x1 = s * p1.X + t * p2.X;
        var y1 = s * p1.Y + t * p2.Y;

        var x2 = s * p2.X + t * p3.X;
        var y2 = s * p2.Y + t * p3.Y;

        // Second stage: interpolate between first-stage results
        x0 = s * x0 + t * x1;
        y0 = s * y0 + t * y1;

        x1 = s * x1 + t * x2;
        y1 = s * y1 + t * y2;

        // Third stage: final interpolation
        return LinVector2D<T>.Create(
            s * x0 + t * x1,
            s * y0 + t * y1
        );
    }

    #endregion

    #region DeCasteljau Array-Based (Arbitrary Degree)

    /// <summary>
    /// De Casteljau's algorithm for arbitrary-degree Bezier curves.
    /// Evaluates a Bezier curve defined by N control points at parameter t.
    /// </summary>
    /// <typeparam name="T">Scalar type</typeparam>
    /// <param name="t">Parameter value in [0,1]</param>
    /// <param name="controlPoints">Array of control points (N points define degree N-1 curve)</param>
    /// <returns>Point on the Bezier curve at parameter t</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector2D<T> DeCasteljau<T>(this Scalar<T> t, params LinVector2D<T>[] controlPoints)
    {
        var pointsCount = controlPoints.Length;

        if (pointsCount == 0)
            throw new ArgumentException("At least one control point is required", nameof(controlPoints));

        if (pointsCount == 1)
            return controlPoints[0];

        if (pointsCount == 2)
            return t.DeCasteljau(controlPoints[0], controlPoints[1]);

        if (pointsCount == 3)
            return t.DeCasteljau(controlPoints[0], controlPoints[1], controlPoints[2]);

        if (pointsCount == 4)
            return t.DeCasteljau(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);

        // For 5+ control points, use the general algorithm
        var processor = t.ScalarProcessor;
        var s = processor.One - t;
        pointsCount--;

        var xList = new Scalar<T>[pointsCount];
        var yList = new Scalar<T>[pointsCount];

        // Perform first stage of linear interpolation on given points
        for (var i = 0; i < pointsCount; i++)
        {
            var j = i + 1;

            xList[i] = s * controlPoints[i].X + t * controlPoints[j].X;
            yList[i] = s * controlPoints[i].Y + t * controlPoints[j].Y;
        }

        // Perform remaining stages of linear interpolation
        while (pointsCount > 2)
        {
            pointsCount--;

            for (var i = 0; i < pointsCount; i++)
            {
                var j = i + 1;

                xList[i] = s * xList[i] + t * xList[j];
                yList[i] = s * yList[i] + t * yList[j];
            }
        }

        // Only two points remain; interpolate them at t
        return LinVector2D<T>.Create(
            s * xList[0] + t * xList[1],
            s * yList[0] + t * yList[1]
        );
    }

    #endregion
}
