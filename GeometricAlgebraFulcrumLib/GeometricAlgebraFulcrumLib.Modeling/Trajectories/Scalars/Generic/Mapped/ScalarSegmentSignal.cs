using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space1D;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

/// <summary>
/// Creates a segment of a signal - restricts a base signal to a specific time range.
/// The signal value is clamped to the segment's time range.
/// </summary>
/// <typeparam name="T">The scalar type</typeparam>
public sealed class ScalarSegmentSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSegmentSignal<T> Finite(ScalarRange<T> timeRange, ScalarSignal<T> baseSignal)
    {
        return new ScalarSegmentSignal<T>(timeRange, false, baseSignal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSegmentSignal<T> Finite(Scalar<T> timeMin, Scalar<T> timeMax, ScalarSignal<T> baseSignal)
    {
        // Ensure timeMin <= timeMax
        if (timeMin.IsLessThanOrEqualTo(timeMax))
        {
            return new ScalarSegmentSignal<T>(
                ScalarRange<T>.Create(timeMin, timeMax),
                false,
                baseSignal
            );
        }

        // If timeMin > timeMax, swap them and map the time range
        return new ScalarSegmentSignal<T>(
            ScalarRange<T>.Create(timeMax, timeMin),
            false,
            FlipTimeRange(baseSignal, timeMax, timeMin)
        );
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSegmentSignal<T> Periodic(ScalarRange<T> timeRange, ScalarSignal<T> baseSignal)
    {
        return new ScalarSegmentSignal<T>(timeRange, true, baseSignal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSegmentSignal<T> Periodic(Scalar<T> timeMin, Scalar<T> timeMax, ScalarSignal<T> baseSignal)
    {
        // Ensure timeMin <= timeMax
        if (timeMin.IsLessThanOrEqualTo(timeMax))
        {
            return new ScalarSegmentSignal<T>(
                ScalarRange<T>.Create(timeMin, timeMax),
                true,
                baseSignal
            );
        }

        // If timeMin > timeMax, swap them and map the time range
        return new ScalarSegmentSignal<T>(
            ScalarRange<T>.Create(timeMax, timeMin),
            true,
            FlipTimeRange(baseSignal, timeMax, timeMin)
        );
    }

    /// <summary>
    /// Helper method to flip/reverse the time range of a signal.
    /// Maps [time1, time2] to [time2, time1].
    /// </summary>
    private static ScalarSignal<T> FlipTimeRange(ScalarSignal<T> baseSignal, Scalar<T> time1, Scalar<T> time2)
    {
        // Create an affine map that flips the time range: [time1, time2] → [time2, time1]
        var affineMap = AffineMap1D<T>.CreateFromRanges(
            baseSignal.ScalarProcessor,
            time1,
            time2,
            time2,
            time1
        );

        return ScalarAffineMappedTimeSignal<T>.Create(baseSignal, affineMap);
    }


    public ScalarSignal<T> BaseSignal { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarSegmentSignal(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> baseSignal)
        : base(timeRange, isPeriodic)
    {
        BaseSignal = baseSignal;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseSignal.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new ScalarSegmentSignal<T>(
                TimeRange,
                false,
                BaseSignal
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ScalarSegmentSignal<T>(
                TimeRange,
                true,
                BaseSignal
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        return BaseSignal.GetValue(
            TimeRange.Clamp(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        return BaseSignal.GetDerivative1Value(
            TimeRange.Clamp(t)
        );
    }
}
