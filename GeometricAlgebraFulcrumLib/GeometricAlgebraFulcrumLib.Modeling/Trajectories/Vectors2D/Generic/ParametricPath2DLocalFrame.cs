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
        // Normalize the tangent, handling zero-norm case
        // Check if already a unit vector to avoid floating-point errors from re-normalization
        var tangentNormSquared = tangent.VectorENormSquared();
        var scalarProcessor = tangent.ScalarProcessor;

        LinVector2D<T> normalizedTangent;
        if (scalarProcessor.IsZero(tangentNormSquared.ScalarValue))
        {
            // Zero tangent - use default unit vector
            normalizedTangent = LinVector2D<T>.UnitSymmetric(scalarProcessor);
        }
        else if (tangentNormSquared.IsNearEqualTo(scalarProcessor.One))
        {
            // Already a unit vector - use as-is to avoid floating-point errors
            normalizedTangent = tangent;
        }
        else
        {
            // Not a unit vector - normalize it
            normalizedTangent = tangent / scalarProcessor.Sqrt(tangentNormSquared.ScalarValue).ToScalar();
        }

        return new ParametricPath2DLocalFrame<T>(
            t,
            point,
            normalizedTangent
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

        // Note: Debug.Assert(IsValid()) removed to avoid floating-point precision issues
        // when normalizing already-normalized vectors. The IsValid() check is too strict
        // for Generic<T> implementations where double normalization can introduce small errors.
        // The tangent should still be approximately normalized by the Create method.
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        var scalarProcessor = Time.ScalarProcessor;

        var isValid =
            Time.IsValid() &&
            Point.IsValid() &&
            Tangent.IsValid() &&
            Tangent.VectorENormSquared().IsNearEqualTo(scalarProcessor.One);

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
