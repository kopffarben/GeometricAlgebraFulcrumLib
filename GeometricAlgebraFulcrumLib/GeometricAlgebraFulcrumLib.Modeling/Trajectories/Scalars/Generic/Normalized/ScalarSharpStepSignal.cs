using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;

/// <summary>
/// A sharp step signal that jumps from -1 to 1 at t=0.
/// GetValue(t < 0) = -1, GetValue(t > 0) = 1, GetValue(t = 0) = 0.
/// GetDerivative1Value(t) = 0 everywhere (undefined at t=0, but we return 0).
/// GetDerivative2Value(t) = 0 everywhere.
/// </summary>
public sealed class ScalarSharpStepSignal<T> :
    ScalarNormalizedSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSharpStepSignal<T> Finite(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarSharpStepSignal<T>(scalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSharpStepSignal<T> Periodic(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarSharpStepSignal<T>(scalarProcessor, true);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarSharpStepSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
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
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        // Sharp step: < 0 => -1, > 0 => 1, = 0 => 0
        if (clampedT < ScalarProcessor.Zero)
            return ScalarProcessor.MinusOne;

        if (clampedT > ScalarProcessor.Zero)
            return ScalarProcessor.One;

        return ScalarProcessor.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Derivative is 0 everywhere (undefined at t=0, but we return 0)
        return ScalarProcessor.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative is also 0
        return ScalarProcessor.Zero;
    }
}
