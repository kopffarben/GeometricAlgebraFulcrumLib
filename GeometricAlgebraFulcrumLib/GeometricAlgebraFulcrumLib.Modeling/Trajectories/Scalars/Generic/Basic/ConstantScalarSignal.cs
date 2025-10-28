using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;

/// <summary>
/// A constant scalar signal that returns the same value for all time parameters
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ConstantScalarSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantScalarSignal<T> Finite(IScalarProcessor<T> processor, Scalar<T> value)
    {
        return new ConstantScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            value
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantScalarSignal<T> Finite(ScalarRange<T> timeRange, Scalar<T> value)
    {
        return new ConstantScalarSignal<T>(
            timeRange,
            false,
            value
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantScalarSignal<T> Periodic(IScalarProcessor<T> processor, Scalar<T> value)
    {
        return new ConstantScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            true,
            value
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantScalarSignal<T> Periodic(ScalarRange<T> timeRange, Scalar<T> value)
    {
        return new ConstantScalarSignal<T>(
            timeRange,
            true,
            value
        );
    }


    public Scalar<T> Value { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ConstantScalarSignal(ScalarRange<T> timeRange, bool isPeriodic, Scalar<T> value)
        : base(timeRange, isPeriodic)
    {
        Value = value;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Value.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new ConstantScalarSignal<T>(TimeRange, false, Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ConstantScalarSignal<T>(TimeRange, true, Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        return Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Derivative of constant is always zero
        return ScalarProcessor.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative of constant is always zero
        return ScalarProcessor.Zero;
    }
}
