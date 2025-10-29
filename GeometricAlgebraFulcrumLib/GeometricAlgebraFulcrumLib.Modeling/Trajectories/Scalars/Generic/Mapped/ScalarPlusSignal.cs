using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

/// <summary>
/// A scalar signal that represents the sum of multiple scalar signals
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ScalarPlusSignal<T> :
    ScalarSignal<T>,
    IReadOnlyList<ScalarSignal<T>>
{
    private static void Add(ICollection<ScalarSignal<T>> baseSignals, ScalarSignal<T> signal)
    {
        if (signal is not ScalarPlusSignal<T> plusSignal)
        {
            baseSignals.Add(signal);
            return;
        }

        // Flatten nested PlusSignals
        foreach (var s in plusSignal)
            Add(baseSignals, s);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPlusSignal<T> Finite(ScalarSignal<T> signal1, ScalarSignal<T> signal2)
    {
        return Finite(new[] { signal1, signal2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPlusSignal<T> Finite(ScalarSignal<T> signal1, ScalarSignal<T> signal2, params ScalarSignal<T>[] signalList)
    {
        var signals = new List<ScalarSignal<T>>(signalList.Length + 2)
        {
            signal1,
            signal2
        };

        signals.AddRange(signalList);

        return Finite(signals);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPlusSignal<T> Finite(IEnumerable<ScalarSignal<T>> signalList)
    {
        var baseSignals = new List<ScalarSignal<T>>();

        foreach (var signal in signalList)
            Add(baseSignals, signal);

        return new ScalarPlusSignal<T>(false, baseSignals);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPlusSignal<T> Periodic(ScalarSignal<T> signal1, ScalarSignal<T> signal2)
    {
        return Periodic(new[] { signal1, signal2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPlusSignal<T> Periodic(ScalarSignal<T> signal1, ScalarSignal<T> signal2, params ScalarSignal<T>[] signalList)
    {
        var signals = new List<ScalarSignal<T>>(signalList.Length + 2)
        {
            signal1,
            signal2
        };

        signals.AddRange(signalList);

        return Periodic(signals);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarPlusSignal<T> Periodic(IEnumerable<ScalarSignal<T>> signalList)
    {
        var baseSignals = new List<ScalarSignal<T>>();

        foreach (var signal in signalList)
            Add(baseSignals, signal);

        return new ScalarPlusSignal<T>(true, baseSignals);
    }


    public IReadOnlyList<ScalarSignal<T>> BaseSignals { get; }

    public int Count
        => BaseSignals.Count;

    public ScalarSignal<T> this[int index]
        => BaseSignals[index];


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarPlusSignal(bool isPeriodic, IReadOnlyList<ScalarSignal<T>> baseSignals)
        : base(
            ScalarRange<T>.Create(
                ComputeMinTimeValue(baseSignals),
                ComputeMaxTimeValue(baseSignals)
            ),
            isPeriodic
        )
    {
        BaseSignals = baseSignals;

        Debug.Assert(IsValid());
    }

    private static Scalar<T> ComputeMinTimeValue(IReadOnlyList<ScalarSignal<T>> baseSignals)
    {
        if (baseSignals.Count == 0)
            throw new ArgumentException("Base signals list cannot be empty", nameof(baseSignals));

        var minValue = baseSignals[0].TimeRange.MinValue;
        var processor = baseSignals[0].ScalarProcessor;

        for (var i = 1; i < baseSignals.Count; i++)
        {
            var currentMin = baseSignals[i].TimeRange.MinValue;
            // Check if currentMin < minValue by computing currentMin - minValue
            var diff = currentMin - minValue;
            if (processor.Sign(diff.ScalarValue) < 0)
                minValue = currentMin;
        }

        return minValue;
    }

    private static Scalar<T> ComputeMaxTimeValue(IReadOnlyList<ScalarSignal<T>> baseSignals)
    {
        if (baseSignals.Count == 0)
            throw new ArgumentException("Base signals list cannot be empty", nameof(baseSignals));

        var maxValue = baseSignals[0].TimeRange.MaxValue;
        var processor = baseSignals[0].ScalarProcessor;

        for (var i = 1; i < baseSignals.Count; i++)
        {
            var currentMax = baseSignals[i].TimeRange.MaxValue;
            // Check if currentMax > maxValue by computing currentMax - maxValue
            var diff = currentMax - maxValue;
            if (processor.Sign(diff.ScalarValue) > 0)
                maxValue = currentMax;
        }

        return maxValue;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseSignals.Count >= 2 &&
               BaseSignals.All(s => s.IsValid()) &&
               TimeRange.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new ScalarPlusSignal<T>(
                false,
                BaseSignals
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ScalarPlusSignal<T>(
                true,
                BaseSignals
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        // Sum all signal values
        var sum = ScalarProcessor.Zero;
        foreach (var signal in BaseSignals)
        {
            sum = sum + signal.GetValue(t);
        }
        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Sum all signal derivatives (derivative of sum is sum of derivatives)
        var sum = ScalarProcessor.Zero;
        foreach (var signal in BaseSignals)
        {
            sum = sum + signal.GetDerivative1Value(t);
        }
        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Sum all signal second derivatives
        var sum = ScalarProcessor.Zero;
        foreach (var signal in BaseSignals)
        {
            sum = sum + signal.GetDerivative2Value(t);
        }
        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<ScalarSignal<T>> GetEnumerator()
    {
        return BaseSignals.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
