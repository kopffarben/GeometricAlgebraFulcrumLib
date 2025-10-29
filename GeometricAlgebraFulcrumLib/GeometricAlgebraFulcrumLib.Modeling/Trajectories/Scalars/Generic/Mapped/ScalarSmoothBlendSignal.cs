using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

/// <summary>
/// Smoothly blends between two scalar signals using a smooth transition function
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
/// <remarks>
/// Smooth transition functions:
/// https://en.wikipedia.org/wiki/Non-analytic_smooth_function#Smooth_transition_functions
/// https://www.youtube.com/watch?v=vD5g8aVscUI
/// </remarks>
public sealed class ScalarSmoothBlendSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSmoothBlendSignal<T> Finite(Scalar<T> blendTimeMin, Scalar<T> blendTimeMax, ScalarSignal<T> baseSignal1, ScalarSignal<T> baseSignal2)
    {
        return new ScalarSmoothBlendSignal<T>(
            ScalarRange<T>.Create(blendTimeMin, blendTimeMax),
            false,
            baseSignal1,
            baseSignal2
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSmoothBlendSignal<T> Finite(ScalarRange<T> timeRange, ScalarSignal<T> baseSignal1, ScalarSignal<T> baseSignal2)
    {
        return new ScalarSmoothBlendSignal<T>(
            timeRange,
            false,
            baseSignal1,
            baseSignal2
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSmoothBlendSignal<T> Periodic(ScalarRange<T> timeRange, ScalarSignal<T> baseSignal1, ScalarSignal<T> baseSignal2)
    {
        return new ScalarSmoothBlendSignal<T>(
            timeRange,
            true,
            baseSignal1,
            baseSignal2
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSmoothBlendSignal<T> Periodic(Scalar<T> blendTimeMin, Scalar<T> blendTimeMax, ScalarSignal<T> baseSignal1, ScalarSignal<T> baseSignal2)
    {
        return new ScalarSmoothBlendSignal<T>(
            ScalarRange<T>.Create(blendTimeMin, blendTimeMax),
            true,
            baseSignal1,
            baseSignal2
        );
    }


    public ScalarSignal<T> BaseSignal1 { get; }

    public ScalarSignal<T> BaseSignal2 { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarSmoothBlendSignal(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> baseSignal1, ScalarSignal<T> baseSignal2)
        : base(timeRange, isPeriodic)
    {
        BaseSignal1 = baseSignal1;
        BaseSignal2 = baseSignal2;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseSignal1.IsValid() &&
               BaseSignal2.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new ScalarSmoothBlendSignal<T>(
                TimeRange,
                false,
                BaseSignal1,
                BaseSignal2
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ScalarSmoothBlendSignal<T>(
                TimeRange,
                true,
                BaseSignal1,
                BaseSignal2
            );
    }


    /// <summary>
    /// Smooth unit step function using sigmoid transition
    /// Maps [MinTime, MaxTime] → [0, 1] with smooth transition
    /// </summary>
    private Scalar<T> SmoothUnitStepFunction(Scalar<T> t)
    {
        var processor = ScalarProcessor;
        var one = processor.One.ToScalar();

        Debug.Assert(
            !t.IsLessThan(MinTime) && !MaxTime.IsLessThan(t)
        );

        // Normalize t to [0, 1] range
        var tNorm = (t - MinTime) / (MaxTime - MinTime);

        // Compute smooth transition: 1 / (1 + exp(1/t - 1/(1-t)))
        var s = one - tNorm; // s = 1 - t
        var invT = one / tNorm; // 1/t
        var invS = one / s; // 1/(1-t)
        var exponent = invT - invS; // 1/t - 1/(1-t)

        // y = 1 / (1 + exp(exponent))
        var expValue = processor.Exp(exponent.ScalarValue).ToScalar();
        var y = one / (one + expValue);

        return y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        var processor = ScalarProcessor;
        var one = processor.One.ToScalar();

        // Clamp time to valid range
        t = TimeRange.Clamp(t);

        // Get blend factor [0, 1]
        var x = SmoothUnitStepFunction(t);
        var y = one - x; // y = 1 - x

        // Blend: signal1 * (1-x) + signal2 * x
        var value1 = BaseSignal1.GetValue(t);
        var value2 = BaseSignal2.GetValue(t);

        return value1 * y + value2 * x;
    }
}
