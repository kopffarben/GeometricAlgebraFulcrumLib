using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

/// <summary>
/// Maps a trajectory of type TValue to a scalar signal via a mapping function
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
/// <typeparam name="TValue">Value type of the base trajectory</typeparam>
public sealed class ScalarMappedTrajectorySignal<T, TValue> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarMappedTrajectorySignal<T, TValue> Create(Trajectory<T, TValue> baseTrajectory, Func<TValue, Scalar<T>> valueMap)
    {
        return new ScalarMappedTrajectorySignal<T, TValue>(baseTrajectory, valueMap);
    }


    public Trajectory<T, TValue> BaseTrajectory { get; }

    public Func<TValue, Scalar<T>> ValueMap { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarMappedTrajectorySignal(Trajectory<T, TValue> baseTrajectory, Func<TValue, Scalar<T>> valueMap)
        : base(baseTrajectory.TimeRange, baseTrajectory.IsPeriodic)
    {
        BaseTrajectory = baseTrajectory;
        ValueMap = valueMap;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseTrajectory.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new ScalarMappedTrajectorySignal<T, TValue>(
                (Trajectory<T, TValue>)BaseTrajectory.ToFinite(),
                ValueMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ScalarMappedTrajectorySignal<T, TValue>(
                (Trajectory<T, TValue>)BaseTrajectory.ToPeriodic(),
                ValueMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        t = TimeRange.Clamp(t);

        var value = ValueMap(
            BaseTrajectory.GetValue(t)
        );

        Debug.Assert(value.IsValid());

        return value;
    }

}
