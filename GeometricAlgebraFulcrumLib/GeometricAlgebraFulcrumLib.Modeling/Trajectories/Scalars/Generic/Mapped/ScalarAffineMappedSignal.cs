using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space1D;
using GeometricAlgebraFulcrumLib.Modeling.Signals;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

/// <summary>
/// Maps a base signal's values through an affine transformation: f(x) = scaling * x + offset
/// </summary>
/// <typeparam name="T">The scalar type</typeparam>
public sealed class ScalarAffineMappedSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarAffineMappedSignal<T> Create(ScalarSignal<T> baseSignal, AffineMap1D<T> affineMap)
    {
        return new ScalarAffineMappedSignal<T>(baseSignal, affineMap);
    }


    public ScalarSignal<T> BaseSignal { get; }

    public AffineMap1D<T> AffineMap { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarAffineMappedSignal(ScalarSignal<T> baseSignal, AffineMap1D<T> affineMap)
        : base(baseSignal.TimeRange, baseSignal.IsPeriodic)
    {
        BaseSignal = baseSignal;
        AffineMap = affineMap;

        Debug.Assert(IsValid());
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
            : new ScalarAffineMappedSignal<T>(
                BaseSignal.ToFiniteSignal(),
                AffineMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ScalarAffineMappedSignal<T>(
                BaseSignal.ToPeriodicSignal(),
                AffineMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        return AffineMap.MapPoint(
            BaseSignal.GetValue(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        return ScalarProcessor.Times(
            AffineMap.Scaling.ScalarValue,
            BaseSignal.GetDerivative1Value(t).ScalarValue
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        return ScalarProcessor.Times(
            AffineMap.Scaling.ScalarValue,
            BaseSignal.GetDerivative2Value(t).ScalarValue
        );
    }
}
