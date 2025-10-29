using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;

/// <summary>
/// A sharp rectangle signal with discontinuous jumps.
/// Returns 1 for |t| < 0.5, -1 for |t| > 0.5, and 0 at boundaries.
/// </summary>
public sealed class ScalarSharpRectangleSignal<T> :
    ScalarNormalizedSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSharpRectangleSignal<T> Finite(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarSharpRectangleSignal<T>(scalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSharpRectangleSignal<T> Periodic(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarSharpRectangleSignal<T>(scalarProcessor, true);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarSharpRectangleSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
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

        // Half = 0.5
        var two = ScalarProcessor.One + ScalarProcessor.One;
        var half = ScalarProcessor.One / two;

        // Rectangle: -1 for |t| > 0.5, 1 for |t| < 0.5, 0 for t = ±0.5
        if (clampedT < -half || clampedT > half)
            return ScalarProcessor.MinusOne;

        if (clampedT > -half && clampedT < half)
            return ScalarProcessor.One;

        return ScalarProcessor.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Derivative is 0 everywhere (discontinuous)
        return ScalarProcessor.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative is 0 everywhere
        return ScalarProcessor.Zero;
    }
}
