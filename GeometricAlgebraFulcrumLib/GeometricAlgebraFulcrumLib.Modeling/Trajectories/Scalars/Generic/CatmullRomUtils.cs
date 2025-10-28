using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;

public static class CatmullRomUtils
{
    /// <summary>
    /// Compute Catmull-Rom spline value using the parametric formula.
    /// Based on Figure 3 from http://www.cemyuksel.com/research/catmullrom_param/catmullrom.pdf
    /// </summary>
    /// <param name="t">Parameter value to evaluate at</param>
    /// <param name="tQuad">Time values (t0, t1, t2, t3) for the 4 control points</param>
    /// <param name="pQuad">Position values (p0, p1, p2, p3) for the 4 control points</param>
    /// <returns>Interpolated value at parameter t (interpolation occurs from p1 to p2)</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> GetCatmullRomValue<T>(this Scalar<T> t, Quad<Scalar<T>> tQuad, Quad<Scalar<T>> pQuad)
    {
        var (t0, t1, t2, t3) = tQuad;
        var (p0, p1, p2, p3) = pQuad;
        var processor = t.ScalarProcessor;

        var tt0 = t - t0;
        var tt1 = t - t1;
        var tt2 = t - t2;
        var tt3 = t - t3;

        var t10 = t1 - t0;
        var t21 = t2 - t1;
        var t32 = t3 - t2;

        var p10 = (p1 * tt0 - p0 * tt1) / t10;
        var p21 = (p2 * tt1 - p1 * tt2) / t21;
        var p32 = (p3 * tt2 - p2 * tt3) / t32;

        var t210 = t2 - t0;
        var t321 = t3 - t1;

        var p210 = (p21 * tt0 - p10 * tt2) / t210;
        var p321 = (p32 * tt1 - p21 * tt3) / t321;

        var t3210 = t2 - t1;

        var p3210 = (p321 * tt1 - p210 * tt2) / t3210;

        return p3210;
    }

    /// <summary>
    /// Compute first derivative of Catmull-Rom spline.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> GetCatmullRomDerivativeValue<T>(this Scalar<T> t, Quad<Scalar<T>> tQuad, Quad<Scalar<T>> pQuad)
    {
        var (t0, t1, t2, t3) = tQuad;
        var (p0, p1, p2, p3) = pQuad;

        var tt0 = t - t0;
        var tt1 = t - t1;
        var tt2 = t - t2;
        var tt3 = t - t3;

        var t10 = t1 - t0;
        var t21 = t2 - t1;
        var t32 = t3 - t2;

        var p10 = (p1 * tt0 - p0 * tt1) / t10;
        var p21 = (p2 * tt1 - p1 * tt2) / t21;
        var p32 = (p3 * tt2 - p2 * tt3) / t32;

        var t210 = t2 - t0;
        var t321 = t3 - t1;

        var p210 = (p21 * tt0 - p10 * tt2) / t210;
        var p321 = (p32 * tt1 - p21 * tt3) / t321;

        var t3210 = t2 - t1;

        var dp10 = (p1 - p0) / t10;
        var dp21 = (p2 - p1) / t21;
        var dp32 = (p3 - p2) / t32;

        var dp210 = (p21 - p10 + dp21 * tt0 - dp10 * tt2) / t210;
        var dp321 = (p32 - p21 + dp32 * tt1 - dp21 * tt3) / t321;

        var dp3210 = (p321 - p210 + dp321 * tt1 - dp210 * tt2) / t3210;

        return dp3210;
    }

    /// <summary>
    /// Compute second derivative of Catmull-Rom spline.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Scalar<T> GetCatmullRomDerivative2Value<T>(this Scalar<T> t, Quad<Scalar<T>> tQuad, Quad<Scalar<T>> pQuad)
    {
        var (t0, t1, t2, t3) = tQuad;
        var (p0, p1, p2, p3) = pQuad;
        var processor = t.ScalarProcessor;

        var tt0 = t - t0;
        var tt1 = t - t1;
        var tt2 = t - t2;
        var tt3 = t - t3;

        var t10 = t1 - t0;
        var t21 = t2 - t1;
        var t32 = t3 - t2;

        var p10 = (p1 * tt0 - p0 * tt1) / t10;
        var p21 = (p2 * tt1 - p1 * tt2) / t21;
        var p32 = (p3 * tt2 - p2 * tt3) / t32;

        var t210 = t2 - t0;
        var t321 = t3 - t1;

        var t3210 = t2 - t1;

        var dp10 = (p1 - p0) / t10;
        var dp21 = (p2 - p1) / t21;
        var dp32 = (p3 - p2) / t32;

        var dp210 = (p21 - p10 + dp21 * tt0 - dp10 * tt2) / t210;
        var dp321 = (p32 - p21 + dp32 * tt1 - dp21 * tt3) / t321;

        var two = processor.ScalarFromNumber(2);

        var d2P210 = two * (dp21 - dp10) / t210;
        var d2P321 = two * (dp32 - dp21) / t321;

        var d2P3210 = (two * (dp321 - dp210) + d2P321 * tt1 - d2P210 * tt2) / t3210;

        return d2P3210;
    }

    /// <summary>
    /// Compute Catmull-Rom spline value for LinVector3D.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T> GetCatmullRomValue<T>(this Scalar<T> t, Quad<Scalar<T>> tQuad, Quad<LinVector3D<T>> pQuad)
    {
        var (t0, t1, t2, t3) = tQuad;
        var (p0, p1, p2, p3) = pQuad;

        var tt0 = t - t0;
        var tt1 = t - t1;
        var tt2 = t - t2;
        var tt3 = t - t3;

        var t10 = t1 - t0;
        var t21 = t2 - t1;
        var t32 = t3 - t2;

        var p10 = (p1 * tt0 - p0 * tt1) / t10;
        var p21 = (p2 * tt1 - p1 * tt2) / t21;
        var p32 = (p3 * tt2 - p2 * tt3) / t32;

        var t210 = t2 - t0;
        var t321 = t3 - t1;

        var p210 = (p21 * tt0 - p10 * tt2) / t210;
        var p321 = (p32 * tt1 - p21 * tt3) / t321;

        var t3210 = t2 - t1;

        var p3210 = (p321 * tt1 - p210 * tt2) / t3210;

        return p3210;
    }

    /// <summary>
    /// Compute first derivative of Catmull-Rom spline for LinVector3D.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T> GetCatmullRomDerivativeValue<T>(this Scalar<T> t, Quad<Scalar<T>> tQuad, Quad<LinVector3D<T>> pQuad)
    {
        var (t0, t1, t2, t3) = tQuad;
        var (p0, p1, p2, p3) = pQuad;

        var tt0 = t - t0;
        var tt1 = t - t1;
        var tt2 = t - t2;
        var tt3 = t - t3;

        var t10 = t1 - t0;
        var t21 = t2 - t1;
        var t32 = t3 - t2;

        var p10 = (p1 * tt0 - p0 * tt1) / t10;
        var p21 = (p2 * tt1 - p1 * tt2) / t21;
        var p32 = (p3 * tt2 - p2 * tt3) / t32;

        var t210 = t2 - t0;
        var t321 = t3 - t1;

        var p210 = (p21 * tt0 - p10 * tt2) / t210;
        var p321 = (p32 * tt1 - p21 * tt3) / t321;

        var t3210 = t2 - t1;

        var dp10 = (p1 - p0) / t10;
        var dp21 = (p2 - p1) / t21;
        var dp32 = (p3 - p2) / t32;

        var dp210 = (p21 - p10 + dp21 * tt0 - dp10 * tt2) / t210;
        var dp321 = (p32 - p21 + dp32 * tt1 - dp21 * tt3) / t321;

        var dp3210 = (p321 - p210 + dp321 * tt1 - dp210 * tt2) / t3210;

        return dp3210;
    }

    /// <summary>
    /// Compute second derivative of Catmull-Rom spline for LinVector3D.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T> GetCatmullRomDerivative2Value<T>(this Scalar<T> t, Quad<Scalar<T>> tQuad, Quad<LinVector3D<T>> pQuad)
    {
        var (t0, t1, t2, t3) = tQuad;
        var (p0, p1, p2, p3) = pQuad;
        var processor = t.ScalarProcessor;

        var tt0 = t - t0;
        var tt1 = t - t1;
        var tt2 = t - t2;
        var tt3 = t - t3;

        var t10 = t1 - t0;
        var t21 = t2 - t1;
        var t32 = t3 - t2;

        var p10 = (p1 * tt0 - p0 * tt1) / t10;
        var p21 = (p2 * tt1 - p1 * tt2) / t21;
        var p32 = (p3 * tt2 - p2 * tt3) / t32;

        var t210 = t2 - t0;
        var t321 = t3 - t1;

        var t3210 = t2 - t1;

        var dp10 = (p1 - p0) / t10;
        var dp21 = (p2 - p1) / t21;
        var dp32 = (p3 - p2) / t32;

        var dp210 = (p21 - p10 + dp21 * tt0 - dp10 * tt2) / t210;
        var dp321 = (p32 - p21 + dp32 * tt1 - dp21 * tt3) / t321;

        var two = processor.ScalarFromNumber(2);

        var d2P210 = two * (dp21 - dp10) / t210;
        var d2P321 = two * (dp32 - dp21) / t321;

        var d2P3210 = (two * (dp321 - dp210) + d2P321 * tt1 - d2P210 * tt2) / t3210;

        return d2P3210;
    }
}
