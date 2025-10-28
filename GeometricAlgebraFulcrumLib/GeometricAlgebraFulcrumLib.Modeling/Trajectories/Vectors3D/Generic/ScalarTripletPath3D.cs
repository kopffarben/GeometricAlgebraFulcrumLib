using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

/// <summary>
/// A 3D parametric path composed of three independent scalar signals for X, Y, Z components
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class ScalarTripletPath3D<T> :
    ParametricPath3D<T>,
    ITriplet<ScalarSignal<T>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTripletPath3D<T> Finite(ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
    {
        return new ScalarTripletPath3D<T>(
            item1.TimeRange.Intersect(item2.TimeRange),
            false,
            item1,
            item2,
            item3
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTripletPath3D<T> Finite(ScalarRange<T> timeRange, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
    {
        return new ScalarTripletPath3D<T>(
            timeRange,
            false,
            item1,
            item2,
            item3
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTripletPath3D<T> Periodic(ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
    {
        return new ScalarTripletPath3D<T>(
            item1.TimeRange.Intersect(item2.TimeRange),
            true,
            item1,
            item2,
            item3
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTripletPath3D<T> Periodic(ScalarRange<T> timeRange, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
    {
        return new ScalarTripletPath3D<T>(
            timeRange,
            true,
            item1,
            item2,
            item3
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTripletPath3D<T> Create(bool isPeriodic, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
    {
        return new ScalarTripletPath3D<T>(
            item1.TimeRange.Intersect(item2.TimeRange),
            isPeriodic,
            item1,
            item2,
            item3
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTripletPath3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
    {
        return new ScalarTripletPath3D<T>(
            timeRange,
            isPeriodic,
            item1,
            item2,
            item3
        );
    }


    public ScalarSignal<T> Item1 { get; }

    public ScalarSignal<T> Item2 { get; }

    public ScalarSignal<T> Item3 { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarTripletPath3D(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
        : base(timeRange, isPeriodic)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Item1.IsValid() &&
               Item2.IsValid() &&
               Item3.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            Item1.GetValue(t),
            Item2.GetValue(t),
            Item3.GetValue(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            Item1.GetDerivative1Value(t),
            Item2.GetDerivative1Value(t),
            Item3.GetDerivative1Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            Item1.GetDerivative2Value(t),
            Item2.GetDerivative2Value(t),
            Item3.GetDerivative2Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new ScalarTripletPath3D<T>(
                TimeRange,
                false,
                Item1,
                Item2,
                Item3
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new ScalarTripletPath3D<T>(
                TimeRange,
                true,
                Item1,
                Item2,
                Item3
            );
    }

    /// <summary>
    /// Get the three scalar signal components (X, Y, Z) of this path
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Triplet<ScalarSignal<T>> GetScalarComponents()
    {
        return new Triplet<ScalarSignal<T>>(Item1, Item2, Item3);
    }
}
