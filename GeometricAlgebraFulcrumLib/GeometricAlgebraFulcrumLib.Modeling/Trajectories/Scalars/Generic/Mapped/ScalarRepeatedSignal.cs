using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Signals;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

/// <summary>
/// Repeats a base signal a specified number of times sequentially.
/// The time range of the repeated signal is Count * BaseSignal.TimeRangeLength.
/// </summary>
/// <typeparam name="T">The scalar type</typeparam>
public sealed class ScalarRepeatedSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarRepeatedSignal<T> Create(ScalarSignal<T> baseSignal, int count)
    {
        return new ScalarRepeatedSignal<T>(
            baseSignal,
            count
        );
    }


    public ScalarSignal<T> BaseSignal { get; }

    public int Count { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarRepeatedSignal(ScalarSignal<T> baseSignal, int count)
        : base(
            ScalarRange<T>.Create(
                baseSignal.MinTime,
                baseSignal.ScalarProcessor.Add(
                    baseSignal.MinTime,
                    baseSignal.ScalarProcessor.Times(
                        baseSignal.TimeRangeLength,
                        baseSignal.ScalarProcessor.ScalarFromNumber(count)
                    )
                )
            ),
            false
        )
    {
        if (count < 1)
            throw new InvalidOperationException("Count must be >= 1");

        BaseSignal = baseSignal;
        Count = count;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseSignal.IsValid() &&
               Count >= 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return BaseSignal.ToPeriodicSignal();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        var processor = ScalarProcessor;
        
        // First: Clamp t to this signal's time range (non-periodic finite signal)
        var clampedT = ClampToFiniteRange(t);
        
        // Second: Clamp to base signal's time range with periodic wrapping
        var mappedT = ClampToBaseSignalPeriodic(clampedT);
        
        return BaseSignal.GetValue(mappedT);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Scalar<T> ClampToFiniteRange(Scalar<T> t)
    {
        if (t.IsLessThan(MinTime.ScalarValue))
            return MinTime;
        if (t.IsMoreThan(MaxTime.ScalarValue))
            return MaxTime;
        
        return t;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Scalar<T> ClampToBaseSignalPeriodic(Scalar<T> t)
    {
        var processor = ScalarProcessor;
        var baseLength = BaseSignal.TimeRangeLength.ScalarValue;
        var baseMin = BaseSignal.MinTime.ScalarValue;
        
        // Calculate offset from our MinTime (which equals BaseSignal.MinTime)
        var offset = processor.Subtract(t.ScalarValue, MinTime.ScalarValue);
        
        // Periodic clamping: wrap using modulo
        // mappedT = baseMin + (offset mod baseLength)
        var normalized = processor.Divide(offset.ScalarValue, baseLength);
        var normalizedFloat = processor.ToFloat64(normalized.ScalarValue);
        var fracPart = normalizedFloat - Math.Floor(normalizedFloat);
        var wrappedOffset = processor.Times(processor.ScalarFromNumber(fracPart).ScalarValue, baseLength);
        var mappedT = processor.Add(baseMin, wrappedOffset.ScalarValue);
        
        return processor.Scalar(mappedT.ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Same clamping logic as GetValue
        var clampedT = ClampToFiniteRange(t);
        var mappedT = ClampToBaseSignalPeriodic(clampedT);
        
        return BaseSignal.GetDerivative1Value(mappedT);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Same clamping logic as GetValue
        var clampedT = ClampToFiniteRange(t);
        var mappedT = ClampToBaseSignalPeriodic(clampedT);
        
        return BaseSignal.GetDerivative2Value(mappedT);
    }
}
