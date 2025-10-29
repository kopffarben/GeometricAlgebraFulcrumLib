using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;

/// <summary>
/// A half-sine step signal that smoothly transitions from -1 to 1.
/// Uses sin(π/2 * t) formula for smooth interpolation.
/// </summary>
public sealed class ScalarHalfSinStepSignal<T> :
    ScalarNormalizedSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarHalfSinStepSignal<T> Finite(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarHalfSinStepSignal<T>(scalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarHalfSinStepSignal<T> Periodic(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarHalfSinStepSignal<T>(scalarProcessor, true);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarHalfSinStepSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
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

        // s = π/2
        var two = ScalarProcessor.One + ScalarProcessor.One;
        var s = ScalarProcessor.Pi / two;

        // sin(π/2 * t)
        return (s * clampedT).Sin();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        // s = π/2
        var two = ScalarProcessor.One + ScalarProcessor.One;
        var s = ScalarProcessor.Pi / two;

        // (π/2) * cos(π/2 * t)
        return s * (s * clampedT).Cos();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        // s = π/2, s2 = -(π/2)²
        var two = ScalarProcessor.One + ScalarProcessor.One;
        var s = ScalarProcessor.Pi / two;
        var s2 = -(s * s);

        // -(π/2)² * sin(π/2 * t)
        return s2 * (s * clampedT).Sin();
    }
}
