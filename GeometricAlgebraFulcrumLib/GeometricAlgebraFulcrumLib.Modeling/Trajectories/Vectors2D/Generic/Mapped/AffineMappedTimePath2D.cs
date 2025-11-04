using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space1D;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;

/// <summary>
/// A 2D path with an affine time transformation applied.
/// Transforms the time parameter: t' = scaling * t + offset
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class AffineMappedTimePath2D<T> :
    ParametricPath2D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMappedTimePath2D<T> Create(ParametricPath2D<T> basePath, AffineMap1D<T> timeMap)
    {
        return new AffineMappedTimePath2D<T>(basePath, timeMap);
    }


    public ParametricPath2D<T> BasePath { get; }

    /// <summary>
    /// This function takes a time value in the new range and maps it to the base curve's time range.
    /// Transformation: t' = scaling * t + offset
    /// </summary>
    public AffineMap1D<T> TimeMap { get; }

    /// <summary>
    /// The inverse time map for efficient evaluation.
    /// Maps from the new time range back to the base curve's time range.
    /// </summary>
    public AffineMap1D<T> TimeMapInverse { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AffineMappedTimePath2D(ParametricPath2D<T> basePath, AffineMap1D<T> timeMap)
        : base(
            timeMap.Scaling.IsPositive()
                ? ScalarRange<T>.Create(
                    timeMap.MapPoint(basePath.TimeRange.MinValue),
                    timeMap.MapPoint(basePath.TimeRange.MaxValue)
                )
                : ScalarRange<T>.Create(
                    timeMap.MapPoint(basePath.TimeRange.MaxValue),
                    timeMap.MapPoint(basePath.TimeRange.MinValue)
                ),
            basePath.IsPeriodic
        )
    {
        BasePath = basePath;
        TimeMap = timeMap;
        TimeMapInverse = timeMap.GetInverseAffineMap();

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BasePath.IsValid() &&
               TimeMap.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        return BasePath.GetValue(
            TimeMapInverse.MapPoint(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : Create(BasePath.ToFinitePath(), TimeMap);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : Create(BasePath.ToPeriodicPath(), TimeMap);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        // Chain rule: d/dt[f(g(t))] = f'(g(t)) * g'(t)
        // where g(t) = TimeMapInverse(t), so g'(t) = TimeMapInverse.Scaling
        return TimeMapInverse.Scaling * BasePath.GetDerivative1Value(TimeMapInverse.MapPoint(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative: d²/dt²[f(g(t))] = f''(g(t)) * (g'(t))²
        // where g(t) = TimeMapInverse(t), so g'(t) = TimeMapInverse.Scaling
        var scalingSquared = TimeMapInverse.Scaling * TimeMapInverse.Scaling;
        return scalingSquared * BasePath.GetDerivative2Value(TimeMapInverse.MapPoint(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        return BasePath.GetFrame(
            TimeMapInverse.MapPoint(t)
        );
    }
}
