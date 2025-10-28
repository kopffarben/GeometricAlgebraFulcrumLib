using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;

/// <summary>
/// A harmonic (sinusoidal) scalar signal with formula: Magnitude * Cos(Frequency * (t + TimeOffset))
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class HarmonicScalarSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HarmonicScalarSignal<T> Finite(
        ScalarRange<T> timeRange,
        Scalar<T> frequencyHz,
        Scalar<T> magnitude,
        Scalar<T> timeOffset)
    {
        return new HarmonicScalarSignal<T>(
            timeRange,
            false,
            frequencyHz,
            magnitude,
            timeOffset
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HarmonicScalarSignal<T> Finite(
        IScalarProcessor<T> processor,
        Scalar<T> frequencyHz,
        Scalar<T> magnitude,
        Scalar<T> timeOffset)
    {
        return new HarmonicScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            frequencyHz,
            magnitude,
            timeOffset
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HarmonicScalarSignal<T> Periodic(
        ScalarRange<T> timeRange,
        Scalar<T> frequencyHz,
        Scalar<T> magnitude,
        Scalar<T> timeOffset)
    {
        return new HarmonicScalarSignal<T>(
            timeRange,
            true,
            frequencyHz,
            magnitude,
            timeOffset
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HarmonicScalarSignal<T> Periodic(
        IScalarProcessor<T> processor,
        Scalar<T> frequencyHz,
        Scalar<T> magnitude,
        Scalar<T> timeOffset)
    {
        return new HarmonicScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            true,
            frequencyHz,
            magnitude,
            timeOffset
        );
    }


    public Scalar<T> FrequencyHz { get; }

    public Scalar<T> Frequency
        => ScalarProcessor.PiTimes2 * FrequencyHz;

    public Scalar<T> Magnitude { get; }

    public Scalar<T> TimeOffset { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private HarmonicScalarSignal(
        ScalarRange<T> timeRange,
        bool isPeriodic,
        Scalar<T> frequencyHz,
        Scalar<T> magnitude,
        Scalar<T> timeOffset)
        : base(timeRange, isPeriodic)
    {
        FrequencyHz = frequencyHz;
        Magnitude = magnitude;
        TimeOffset = timeOffset;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        var angle = Frequency * (t + TimeOffset);
        return Magnitude * ScalarProcessor.Cos(angle.ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new HarmonicScalarSignal<T>(
                TimeRange,
                false,
                FrequencyHz,
                Magnitude,
                TimeOffset
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new HarmonicScalarSignal<T>(
                TimeRange,
                true,
                FrequencyHz,
                Magnitude,
                TimeOffset
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        var w = Frequency;
        var angle = w * (t + TimeOffset);

        return ScalarProcessor.Negative((Magnitude * w * ScalarProcessor.Sin(angle.ScalarValue)).ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        var w = Frequency;
        var angle = w * (t + TimeOffset);

        return ScalarProcessor.Negative((Magnitude * w * w * ScalarProcessor.Cos(angle.ScalarValue)).ScalarValue);
    }
}
