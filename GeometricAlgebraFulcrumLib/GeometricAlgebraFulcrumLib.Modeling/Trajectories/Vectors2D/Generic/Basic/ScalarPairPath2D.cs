using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;

/// <summary>
/// A 2D parametric path composed of two scalar signals for X and Y components
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ScalarPairPath2D<T> :
    ParametricPath2D<T>,
    IPair<ScalarSignal<T>>
{
    #region Static Factory Methods - Finite

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPairPath2D<T> Finite(ScalarSignal<T> item1, ScalarSignal<T> item2)
    {
        return new ScalarPairPath2D<T>(
            item1.TimeRange.Intersect(item2.TimeRange),
            false,
            item1,
            item2
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPairPath2D<T> Finite(ScalarRange<T> timeRange, ScalarSignal<T> item1, ScalarSignal<T> item2)
    {
        return new ScalarPairPath2D<T>(
            timeRange,
            false,
            item1,
            item2
        );
    }

    #endregion

    #region Static Factory Methods - Periodic

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPairPath2D<T> Periodic(ScalarSignal<T> item1, ScalarSignal<T> item2)
    {
        return new ScalarPairPath2D<T>(
            item1.TimeRange.Intersect(item2.TimeRange),
            true,
            item1,
            item2
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPairPath2D<T> Periodic(ScalarRange<T> timeRange, ScalarSignal<T> item1, ScalarSignal<T> item2)
    {
        return new ScalarPairPath2D<T>(
            timeRange,
            true,
            item1,
            item2
        );
    }

    #endregion

    #region Static Factory Methods - General

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPairPath2D<T> Create(bool isPeriodic, ScalarSignal<T> item1, ScalarSignal<T> item2)
    {
        return new ScalarPairPath2D<T>(
            item1.TimeRange.Intersect(item2.TimeRange),
            isPeriodic,
            item1,
            item2
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPairPath2D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> item1, ScalarSignal<T> item2)
    {
        return new ScalarPairPath2D<T>(
            timeRange,
            isPeriodic,
            item1,
            item2
        );
    }

    #endregion


    public ScalarSignal<T> Item1 { get; }

    public ScalarSignal<T> Item2 { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarPairPath2D(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> item1, ScalarSignal<T> item2)
        : base(timeRange, isPeriodic)
    {
        Item1 = item1;
        Item2 = item2;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Item1.IsValid() &&
               Item2.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new ScalarPairPath2D<T>(
                TimeRange,
                false,
                Item1.ToFiniteSignal(),
                Item2.ToFiniteSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new ScalarPairPath2D<T>(
                TimeRange,
                true,
                Item1.ToPeriodicSignal(),
                Item2.ToPeriodicSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        return LinVector2D<T>.Create(
            Item1.GetValue(t),
            Item2.GetValue(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        return LinVector2D<T>.Create(
            Item1.GetDerivative1Value(t),
            Item2.GetDerivative1Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        return LinVector2D<T>.Create(
            Item1.GetDerivative2Value(t),
            Item2.GetDerivative2Value(t)
        );
    }

    /// <summary>
    /// Override GetScalarComponents to return the scalar signals directly
    /// instead of creating computed signals that wrap the path methods.
    /// This is more efficient for ScalarPairPath2D.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Pair<ScalarSignal<T>> GetScalarComponents()
    {
        return new Pair<ScalarSignal<T>>(Item1, Item2);
    }
}
