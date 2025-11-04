using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;

/// <summary>
/// A 2D parametric path that applies affine transformations to another path.
/// Affine transformations include translation, rotation, scaling, shearing, and their combinations.
/// Points and vectors are transformed differently: points include translation, vectors do not.
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class AffineMappedPath2D<T> :
    ParametricPath2D<T>
{
    /// <summary>
    /// Creates an affine mapped path with separate point and vector transformations.
    /// </summary>
    /// <param name="basePath">The source path to transform</param>
    /// <param name="pointMap">Function that transforms points (includes translation)</param>
    /// <param name="vectorMap">Function that transforms vectors (no translation, for derivatives)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMappedPath2D<T> Create(
        ParametricPath2D<T> basePath,
        Func<LinVector2D<T>, LinVector2D<T>> pointMap,
        Func<LinVector2D<T>, LinVector2D<T>> vectorMap)
    {
        return new AffineMappedPath2D<T>(basePath, pointMap, vectorMap);
    }

    /// <summary>
    /// Creates an affine mapped path using the same transformation for both points and vectors.
    /// Use this for pure linear transformations (rotation, scaling) without translation.
    /// </summary>
    /// <param name="basePath">The source path to transform</param>
    /// <param name="linearMap">Function that transforms both points and vectors linearly</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMappedPath2D<T> CreateLinear(
        ParametricPath2D<T> basePath,
        Func<LinVector2D<T>, LinVector2D<T>> linearMap)
    {
        return new AffineMappedPath2D<T>(basePath, linearMap, linearMap);
    }


    /// <summary>
    /// The source path that gets transformed
    /// </summary>
    public ParametricPath2D<T> BasePath { get; }

    /// <summary>
    /// Transformation function for points (position).
    /// Includes translation component of the affine transformation.
    /// </summary>
    public Func<LinVector2D<T>, LinVector2D<T>> PointMap { get; }

    /// <summary>
    /// Transformation function for vectors (velocity, acceleration).
    /// Excludes translation component - only linear part of affine transformation.
    /// </summary>
    public Func<LinVector2D<T>, LinVector2D<T>> VectorMap { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AffineMappedPath2D(
        ParametricPath2D<T> basePath,
        Func<LinVector2D<T>, LinVector2D<T>> pointMap,
        Func<LinVector2D<T>, LinVector2D<T>> vectorMap)
        : base(basePath.TimeRange, basePath.IsPeriodic)
    {
        BasePath = basePath;
        PointMap = pointMap;
        VectorMap = vectorMap;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BasePath.IsValid();
    }

    /// <summary>
    /// Gets the transformed point at parameter t.
    /// Applies PointMap (with translation) to the base path value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        return PointMap(BasePath.GetValue(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new AffineMappedPath2D<T>(
                BasePath.ToFinitePath(),
                PointMap,
                VectorMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new AffineMappedPath2D<T>(
                BasePath.ToPeriodicPath(),
                PointMap,
                VectorMap
            );
    }

    /// <summary>
    /// Gets the transformed velocity (first derivative) at parameter t.
    /// Applies VectorMap (without translation) to the base path derivative.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        return VectorMap(BasePath.GetDerivative1Value(t));
    }

    /// <summary>
    /// Gets the transformed acceleration (second derivative) at parameter t.
    /// Applies VectorMap (without translation) to the base path second derivative.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        return VectorMap(BasePath.GetDerivative2Value(t));
    }

    /// <summary>
    /// Gets the transformed local frame at parameter t.
    /// Transforms both the position (with PointMap) and tangent vector (with VectorMap).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        var frame = BasePath.GetFrame(t);

        return ParametricPath2DLocalFrame<T>.Create(
            t,
            PointMap(frame.Point),
            VectorMap(frame.Tangent)
        );
    }
}
