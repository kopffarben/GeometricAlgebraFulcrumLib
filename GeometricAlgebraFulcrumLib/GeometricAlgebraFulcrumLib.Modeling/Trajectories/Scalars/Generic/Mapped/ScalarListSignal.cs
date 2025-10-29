using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

/// <summary>
/// Concatenates multiple scalar signals into a single list signal.
/// Automatically offsets signals so they're consecutive in time.
/// </summary>
/// <typeparam name="T">The scalar type</typeparam>
public sealed class ScalarListSignal<T> :
    ScalarSignal<T>,
    IReadOnlyList<ScalarSignal<T>>
{
    private static void Add(List<ScalarSignal<T>> baseSignals, ScalarSignal<T> scalar)
    {
        if (scalar is not ScalarListSignal<T> scalarList)
        {
            if (baseSignals.Count == 0)
            {
                baseSignals.Add(scalar);
                return;
            }

            var timeMax = baseSignals[^1].MaxTime;

            baseSignals.Add(
                OffsetTimeMinTo(scalar, timeMax)
            );

            return;
        }

        // Flatten nested list signals
        foreach (var s in scalarList)
            Add(baseSignals, s);
    }

    private static ScalarSignal<T> OffsetTimeMinTo(ScalarSignal<T> signal, Scalar<T> outTimeMin)
    {
        var processor = signal.ScalarProcessor;
        var offset = processor.Subtract(outTimeMin.ScalarValue, signal.MinTime.ScalarValue);

        // Create affine map for time offset
        var affineMap = Geometry.AffineMaps.Space1D.AffineMap1D<T>.CreateTranslate(
            processor,
            processor.Scalar(offset.ScalarValue)
        );

        return ScalarAffineMappedTimeSignal<T>.Create(signal, affineMap);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarListSignal<T> Finite(ScalarSignal<T> scalar1, ScalarSignal<T> scalar2)
    {
        return Finite(new[] { scalar1, scalar2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarListSignal<T> Finite(ScalarSignal<T> scalar1, ScalarSignal<T> scalar2, params ScalarSignal<T>[] scalarList)
    {
        var scalars = new List<ScalarSignal<T>>(scalarList.Length + 2)
        {
            scalar1,
            scalar2
        };

        scalars.AddRange(scalarList);

        return Finite(scalars);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarListSignal<T> Finite(IEnumerable<ScalarSignal<T>> scalarList)
    {
        var baseSignals = new List<ScalarSignal<T>>();

        foreach (var scalar in scalarList)
            Add(baseSignals, scalar);

        return new ScalarListSignal<T>(false, baseSignals);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarListSignal<T> Periodic(ScalarSignal<T> scalar1, ScalarSignal<T> scalar2)
    {
        return Periodic(new[] { scalar1, scalar2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarListSignal<T> Periodic(ScalarSignal<T> scalar1, ScalarSignal<T> scalar2, params ScalarSignal<T>[] scalarList)
    {
        var scalars = new List<ScalarSignal<T>>(scalarList.Length + 2)
        {
            scalar1,
            scalar2
        };

        scalars.AddRange(scalarList);

        return Periodic(scalars);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarListSignal<T> Periodic(IEnumerable<ScalarSignal<T>> scalarList)
    {
        var baseSignals = new List<ScalarSignal<T>>();

        foreach (var scalar in scalarList)
            Add(baseSignals, scalar);

        return new ScalarListSignal<T>(true, baseSignals);
    }


    public IReadOnlyList<ScalarSignal<T>> BaseSignals { get; }

    public int Count
        => BaseSignals.Count;

    public ScalarSignal<T> this[int index]
        => BaseSignals[index];


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarListSignal(bool isPeriodic, IReadOnlyList<ScalarSignal<T>> scalarList)
        : base(
            ScalarRange<T>.Create(
                scalarList[0].MinTime,
                scalarList[^1].MaxTime
            ),
            isPeriodic
        )
    {
        BaseSignals = scalarList;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseSignals.Count >= 2 &&
               BaseSignals.All(s => s.IsValid());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new ScalarListSignal<T>(
                false,
                BaseSignals
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ScalarListSignal<T>(
                true,
                BaseSignals
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        // Clamp time based on whether signal is periodic
        t = TimeRange.Clamp(t);

        // Find first signal that contains this time
        return BaseSignals.First(
            scalar => !t.IsLessThan(scalar.MinTime) && !scalar.MaxTime.IsLessThan(t)
        ).GetValue(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Clamp time based on whether signal is periodic
        t = TimeRange.Clamp(t);

        // Find first signal that contains this time
        return BaseSignals.First(
            scalar => !t.IsLessThan(scalar.MinTime) && !scalar.MaxTime.IsLessThan(t)
        ).GetDerivative1Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Clamp time based on whether signal is periodic
        t = TimeRange.Clamp(t);

        // Find first signal that contains this time
        return BaseSignals.First(
            scalar => !t.IsLessThan(scalar.MinTime) && !scalar.MaxTime.IsLessThan(t)
        ).GetDerivative2Value(t);
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
