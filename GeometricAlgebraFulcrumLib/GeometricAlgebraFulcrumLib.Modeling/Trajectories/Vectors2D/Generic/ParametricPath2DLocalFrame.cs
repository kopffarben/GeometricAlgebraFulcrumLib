using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic;

/// <summary>
/// Represents a local coordinate frame along a 2D parametric curve
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed record ParametricPath2DLocalFrame<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParametricPath2DLocalFrame<T> Create(Scalar<T> t, LinVector2D<T> point, LinVector2D<T> tangent)
    {
        return new ParametricPath2DLocalFrame<T>(
            t,
            point,
            tangent.ToUnitLinVector2D()
        );
    }


    /// <summary>
    /// The curve parameter value at the given curve point
    /// </summary>
    public Scalar<T> Time { get; }

    /// <summary>
    /// A point on the curve
    /// </summary>
    public LinVector2D<T> Point { get; }

    /// <summary>
    /// The tangent unit vector to the curve at the given curve point
    /// </summary>
    public LinVector2D<T> Tangent { get; }

    /// <summary>
    /// The normal unit vector to the curve at the given curve point (perpendicular to tangent)
    /// </summary>
    public LinVector2D<T> Normal { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ParametricPath2DLocalFrame(Scalar<T> t, LinVector2D<T> point, LinVector2D<T> tangent)
    {
        Time = t;
        Point = point;
        Tangent = tangent;
        Normal = Tangent.GetNormal();

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        var scalarProcessor = Time.ScalarProcessor;

        var isValid =
            Time.IsValid() &&
            Point.IsValid() &&
            Tangent.IsValid() &&
            Tangent.VectorENormSquared().IsNearEqual(scalarProcessor.One);

        return isValid;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParametricPath2DLocalFrame<T> TranslateBy(LinVector2D<T> translationVector)
    {
        Debug.Assert(translationVector.IsValid());

        return new ParametricPath2DLocalFrame<T>(
            Time,
            Point + translationVector,
            Tangent
        );
    }
}
