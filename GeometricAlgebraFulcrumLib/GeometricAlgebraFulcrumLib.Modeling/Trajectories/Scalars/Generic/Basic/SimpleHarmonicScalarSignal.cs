using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;

/// <summary>
/// A simple harmonic scalar signal with formula: Magnitude * Cos(2π * HarmonicFactor * (t + TimeOffset))
/// Uses integer harmonic factor instead of frequency.
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class SimpleHarmonicScalarSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicScalarSignal<T> Finite(
        IScalarProcessor<T> scalarProcessor,
        int harmonicFactor,
        Scalar<T> magnitude,
        Scalar<T> timeOffset)
    {
        return new SimpleHarmonicScalarSignal<T>(
            scalarProcessor,
            false,
            harmonicFactor,
            magnitude,
            timeOffset
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicScalarSignal<T> Finite(
        IScalarProcessor<T> scalarProcessor,
        int harmonicFactor,
        Scalar<T> magnitude)
    {
        return new SimpleHarmonicScalarSignal<T>(
            scalarProcessor,
            false,
            harmonicFactor,
            magnitude,
            scalarProcessor.Zero
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicScalarSignal<T> Periodic(
        IScalarProcessor<T> scalarProcessor,
        int harmonicFactor,
        Scalar<T> magnitude,
        Scalar<T> timeOffset)
    {
        return new SimpleHarmonicScalarSignal<T>(
            scalarProcessor,
            true,
            harmonicFactor,
            magnitude,
            timeOffset
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicScalarSignal<T> Periodic(
        IScalarProcessor<T> scalarProcessor,
        int harmonicFactor,
        Scalar<T> magnitude)
    {
        return new SimpleHarmonicScalarSignal<T>(
            scalarProcessor,
            true,
            harmonicFactor,
            magnitude,
            scalarProcessor.Zero
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicScalarSignal<T> Create(
        IScalarProcessor<T> scalarProcessor,
        bool isPeriodic,
        int harmonicFactor,
        Scalar<T> magnitude,
        Scalar<T> timeOffset)
    {
        return new SimpleHarmonicScalarSignal<T>(
            scalarProcessor,
            isPeriodic,
            harmonicFactor,
            magnitude,
            timeOffset
        );
    }


    public int HarmonicFactor { get; }

    public Scalar<T> Magnitude { get; }

    public Scalar<T> TimeOffset { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SimpleHarmonicScalarSignal(
        IScalarProcessor<T> scalarProcessor,
        bool isPeriodic,
        int harmonicFactor,
        Scalar<T> magnitude,
        Scalar<T> timeOffset)
        : base(ScalarRange<T>.SymmetricPi(scalarProcessor), isPeriodic)
    {
        HarmonicFactor = harmonicFactor;
        Magnitude = magnitude;
        TimeOffset = timeOffset;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new SimpleHarmonicScalarSignal<T>(
                ScalarProcessor,
                false,
                HarmonicFactor,
                Magnitude,
                TimeOffset
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new SimpleHarmonicScalarSignal<T>(
                ScalarProcessor,
                true,
                HarmonicFactor,
                Magnitude,
                TimeOffset
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        var w = ScalarProcessor.PiTimes2 * ScalarProcessor.ScalarFromNumber(HarmonicFactor);
        var angle = w * (t + TimeOffset);
        return Magnitude * ScalarProcessor.Cos(angle.ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        var w = ScalarProcessor.PiTimes2 * ScalarProcessor.ScalarFromNumber(HarmonicFactor);
        var angle = w * (t + TimeOffset);

        return ScalarProcessor.Negative((Magnitude * w * ScalarProcessor.Sin(angle.ScalarValue)).ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        var w = ScalarProcessor.PiTimes2 * ScalarProcessor.ScalarFromNumber(HarmonicFactor);
        var w2 = w * w;
        var angle = w * (t + TimeOffset);

        return ScalarProcessor.Negative((Magnitude * w2 * ScalarProcessor.Cos(angle.ScalarValue)).ScalarValue);
    }
}
