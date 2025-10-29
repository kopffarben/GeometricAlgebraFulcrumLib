using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space1D;
using GeometricAlgebraFulcrumLib.Modeling.Signals;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

/// <summary>
/// Maps a base signal's time parameter through an affine transformation: baseSignal(affineMapInverse(t))
/// This allows time stretching, compression, and shifting of the signal.
/// </summary>
/// <typeparam name="T">The scalar type</typeparam>
public sealed class ScalarAffineMappedTimeSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarAffineMappedTimeSignal<T> Create(ScalarSignal<T> baseSignal, AffineMap1D<T> affineMap)
    {
        return new ScalarAffineMappedTimeSignal<T>(baseSignal, affineMap);
    }


    public ScalarSignal<T> BaseSignal { get; }

    public AffineMap1D<T> AffineMap { get; }

    public AffineMap1D<T> AffineMapInverse { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarAffineMappedTimeSignal(ScalarSignal<T> baseSignal, AffineMap1D<T> affineMap)
        : base(
            ComputeTimeRange(baseSignal, affineMap),
            baseSignal.IsPeriodic
        )
    {
        BaseSignal = baseSignal;
        AffineMap = affineMap;
        AffineMapInverse = affineMap.GetInverseAffineMap();

        Debug.Assert(IsValid());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ScalarRange<T> ComputeTimeRange(ScalarSignal<T> baseSignal, AffineMap1D<T> affineMap)
    {
        var processor = baseSignal.ScalarProcessor;

        // If scaling is positive, time order is preserved
        // If scaling is negative, time order is reversed
        if (processor.IsPositive(affineMap.Scaling.ScalarValue))
        {
            return ScalarRange<T>.Create(
                affineMap.MapPoint(baseSignal.MinTime),
                affineMap.MapPoint(baseSignal.MaxTime)
            );
        }
        else
        {
            return ScalarRange<T>.Create(
                affineMap.MapPoint(baseSignal.MaxTime),
                affineMap.MapPoint(baseSignal.MinTime)
            );
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseSignal.IsValid() &&
               AffineMap.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new ScalarAffineMappedTimeSignal<T>(
                BaseSignal.ToFiniteSignal(),
                AffineMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ScalarAffineMappedTimeSignal<T>(
                BaseSignal.ToPeriodicSignal(),
                AffineMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        return BaseSignal.GetValue(
            AffineMapInverse.MapPoint(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        var tMapped = AffineMapInverse.MapPoint(t);
        return ScalarProcessor.Times(
            AffineMapInverse.Scaling.ScalarValue,
            BaseSignal.GetDerivative1Value(tMapped).ScalarValue
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        var tMapped = AffineMapInverse.MapPoint(t);
        var scalingSquared = ScalarProcessor.Times(
            AffineMapInverse.Scaling.ScalarValue,
            AffineMapInverse.Scaling.ScalarValue
        );
        return ScalarProcessor.Times(
            scalingSquared.ScalarValue,
            BaseSignal.GetDerivative2Value(tMapped).ScalarValue
        );
    }
}
