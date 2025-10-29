using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;

/// <summary>
/// A linear ramp signal that increases linearly from -1 to 1 over the time range [-1, 1].
/// GetValue(t) returns the clamped time value (within [-1, 1]).
/// GetDerivative1Value(t) returns 1 inside the valid range, 0 outside (for finite signals).
/// GetDerivative2Value(t) always returns 0 (constant slope).
/// </summary>
public sealed class ScalarRampSignal<T> :
    ScalarNormalizedSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarRampSignal<T> Finite(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarRampSignal<T>(scalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarRampSignal<T> Periodic(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarRampSignal<T>(scalarProcessor, true);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarRampSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
        : base(scalarProcessor, isPeriodic)
    {
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return Finite(ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return Periodic(ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        // For finite signals: clamp to range
        // For periodic signals: wrap using modulo (not implemented for Generic<T>, so just clamp)

        if (t < TimeRange.MinValue)
            return TimeRange.MinValue;

        if (t > TimeRange.MaxValue)
            return TimeRange.MaxValue;

        return t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // For periodic signals, derivative is always 1
        // For finite signals, derivative is 1 inside the range, 0 outside

        if (IsPeriodic)
            return ScalarProcessor.One;

        // Check if t is within TimeRange
        var isInRange = t >= TimeRange.MinValue && t <= TimeRange.MaxValue;

        return isInRange
            ? ScalarProcessor.One
            : ScalarProcessor.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Constant slope => zero second derivative
        return ScalarProcessor.Zero;
    }
}
