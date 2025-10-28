using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;

/// <summary>
/// A 3D parametric path constructed from three independent scalar signals,
/// one for each coordinate (X, Y, Z).
/// This allows each component to vary independently along the path.
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class ScalarTripletPath3D<T> :
    ParametricPath3D<T>,
    ITriplet<ScalarSignal<T>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ScalarTripletPath3D<T> Finite(ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
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
    internal static ScalarTripletPath3D<T> Finite(ScalarRange<T> timeRange, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
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
    internal static ScalarTripletPath3D<T> Periodic(ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
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
    internal static ScalarTripletPath3D<T> Periodic(ScalarRange<T> timeRange, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
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
    internal static ScalarTripletPath3D<T> Create(bool isPeriodic, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
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
    internal static ScalarTripletPath3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
    {
        return new ScalarTripletPath3D<T>(
            timeRange,
            isPeriodic,
            item1,
            item2,
            item3
        );
    }


    /// <summary>
    /// Scalar signal for X component
    /// </summary>
    public ScalarSignal<T> Item1 { get; }

    /// <summary>
    /// Scalar signal for Y component
    /// </summary>
    public ScalarSignal<T> Item2 { get; }

    /// <summary>
    /// Scalar signal for Z component
    /// </summary>
    public ScalarSignal<T> Item3 { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarTripletPath3D(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> item1, ScalarSignal<T> item2, ScalarSignal<T> item3)
        : base(timeRange, isPeriodic)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Item1.IsValid() &&
               Item2.IsValid() &&
               Item3.IsValid();
    }

    /// <summary>
    /// Gets the 3D point at parameter t by evaluating each component signal.
    /// Result: (Item1(t), Item2(t), Item3(t))
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            Item1.GetValue(t),
            Item2.GetValue(t),
            Item3.GetValue(t)
        );
    }

    /// <summary>
    /// Gets the first derivative (velocity) at parameter t by evaluating
    /// the derivative of each component signal.
    /// Result: (Item1'(t), Item2'(t), Item3'(t))
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            Item1.GetDerivative1Value(t),
            Item2.GetDerivative1Value(t),
            Item3.GetDerivative1Value(t)
        );
    }

    /// <summary>
    /// Gets the second derivative (acceleration) at parameter t by evaluating
    /// the second derivative of each component signal.
    /// Result: (Item1''(t), Item2''(t), Item3''(t))
    /// </summary>
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
    /// Gets the three scalar component signals as a triplet.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Triplet<ScalarSignal<T>> GetScalarComponents()
    {
        return new Triplet<ScalarSignal<T>>(Item1, Item2, Item3);
    }
}
