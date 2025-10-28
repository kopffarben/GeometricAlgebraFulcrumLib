using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;

/// <summary>
/// A 3D path representing simple harmonic motion in each coordinate.
/// Each component follows: magnitude * cos(2π * harmonicFactor * (t + timeOffset))
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class SimpleHarmonicPath3D<T> :
    ParametricPath3D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicPath3D<T> FiniteSymmetric(IScalarProcessor<T> scalarProcessor, int harmonicFactor, LinVector3D<T> magnitudeVector)
    {
        return new SimpleHarmonicPath3D<T>(
            ScalarRange<T>.SymmetricPi(scalarProcessor),
            false,
            harmonicFactor,
            magnitudeVector,
            LinVector3D<T>.Create(
                scalarProcessor.Zero,
                scalarProcessor.ScalarFromNumber(1) / scalarProcessor.ScalarFromNumber(3),
                -scalarProcessor.ScalarFromNumber(1) / scalarProcessor.ScalarFromNumber(3)
            )
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicPath3D<T> PeriodicSymmetric(IScalarProcessor<T> scalarProcessor, int harmonicFactor, LinVector3D<T> magnitudeVector)
    {
        return new SimpleHarmonicPath3D<T>(
            ScalarRange<T>.SymmetricPi(scalarProcessor),
            true,
            harmonicFactor,
            magnitudeVector,
            LinVector3D<T>.Create(
                scalarProcessor.Zero,
                scalarProcessor.ScalarFromNumber(1) / scalarProcessor.ScalarFromNumber(3),
                -scalarProcessor.ScalarFromNumber(1) / scalarProcessor.ScalarFromNumber(3)
            )
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicPath3D<T> Finite(ScalarRange<T> timeRange, int harmonicFactor, LinVector3D<T> magnitude, LinVector3D<T> timeOffset)
    {
        return new SimpleHarmonicPath3D<T>(
            timeRange,
            false,
            harmonicFactor,
            magnitude,
            timeOffset
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicPath3D<T> Periodic(ScalarRange<T> timeRange, int harmonicFactor, LinVector3D<T> magnitude, LinVector3D<T> timeOffset)
    {
        return new SimpleHarmonicPath3D<T>(
            timeRange,
            true,
            harmonicFactor,
            magnitude,
            timeOffset
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicPath3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, int harmonicFactor, LinVector3D<T> magnitude, LinVector3D<T> timeOffset)
    {
        return new SimpleHarmonicPath3D<T>(
            timeRange,
            isPeriodic,
            harmonicFactor,
            magnitude,
            timeOffset
        );
    }


    private readonly IScalarProcessor<T> _scalarProcessor;

    public int HarmonicFactor { get; }

    public LinVector3D<T> Magnitude { get; }

    public LinVector3D<T> TimeOffset { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SimpleHarmonicPath3D(ScalarRange<T> timeRange, bool isPeriodic, int harmonicFactor, LinVector3D<T> magnitude, LinVector3D<T> timeOffset)
        : base(timeRange, isPeriodic)
    {
        if (!magnitude.IsValid())
            throw new ArgumentException("Magnitude must be valid", nameof(magnitude));

        if (!timeOffset.IsValid())
            throw new ArgumentException("Time offset must be valid", nameof(timeOffset));

        _scalarProcessor = magnitude.GetScalarProcessor();
        HarmonicFactor = harmonicFactor;
        Magnitude = magnitude;
        TimeOffset = timeOffset;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return Magnitude.IsValid() &&
               TimeOffset.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Scalar<T> ClampTime(Scalar<T> t)
    {
        if (IsPeriodic)
        {
            // For periodic paths, wrap time into range using ClampPeriodic logic
            // This maps time to [minTime, maxTime] inclusively
            return ClampPeriodic(t - TimeRange.MinValue, TimeRange.Length) + TimeRange.MinValue;
        }
        else
        {
            // For finite paths, clamp to range [minTime, maxTime]
            if ((t - TimeRange.MinValue).IsNegative())
                return TimeRange.MinValue;

            if ((t - TimeRange.MaxValue).IsPositive())
                return TimeRange.MaxValue;

            return t;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Scalar<T> ClampPeriodic(Scalar<T> value, Scalar<T> maxValue)
    {
        // Simplified periodic clamping to map value to [0, maxValue]
        // Uses iterative approach since Ceiling/Truncate not available for generic scalars

        // Handle negative values
        while (value.IsNegative())
            value = value + maxValue;

        // Handle values > maxValue
        while ((value - maxValue).IsPositive())
            value = value - maxValue;

        // 0 <= value <= maxValue
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new SimpleHarmonicPath3D<T>(
                TimeRange,
                false,
                HarmonicFactor,
                Magnitude,
                TimeOffset
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new SimpleHarmonicPath3D<T>(
                TimeRange,
                true,
                HarmonicFactor,
                Magnitude,
                TimeOffset
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        t = this.ClampTime(t);

        // Angular frequency: w = 2π * harmonicFactor
        var w = _scalarProcessor.PiTimes2 * _scalarProcessor.ScalarFromNumber(HarmonicFactor);

        // Position: magnitude * cos(w * (t + timeOffset))
        var angleX = w * (t + TimeOffset.X);
        var angleY = w * (t + TimeOffset.Y);
        var angleZ = w * (t + TimeOffset.Z);

        return LinVector3D<T>.Create(
            Magnitude.X * _scalarProcessor.Cos(angleX.ScalarValue),
            Magnitude.Y * _scalarProcessor.Cos(angleY.ScalarValue),
            Magnitude.Z * _scalarProcessor.Cos(angleZ.ScalarValue)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        t = this.ClampTime(t);

        // Angular frequency: w = 2π * harmonicFactor
        var w = _scalarProcessor.PiTimes2 * _scalarProcessor.ScalarFromNumber(HarmonicFactor);

        // First derivative: -magnitude * w * sin(w * (t + timeOffset))
        var angleX = w * (t + TimeOffset.X);
        var angleY = w * (t + TimeOffset.Y);
        var angleZ = w * (t + TimeOffset.Z);

        return LinVector3D<T>.Create(
            -Magnitude.X * w * _scalarProcessor.Sin(angleX.ScalarValue),
            -Magnitude.Y * w * _scalarProcessor.Sin(angleY.ScalarValue),
            -Magnitude.Z * w * _scalarProcessor.Sin(angleZ.ScalarValue)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        t = this.ClampTime(t);

        // Angular frequency: w = 2π * harmonicFactor
        var w = _scalarProcessor.PiTimes2 * _scalarProcessor.ScalarFromNumber(HarmonicFactor);
        var w2 = w * w;

        // Second derivative: -magnitude * w² * cos(w * (t + timeOffset))
        var angleX = w * (t + TimeOffset.X);
        var angleY = w * (t + TimeOffset.Y);
        var angleZ = w * (t + TimeOffset.Z);

        return LinVector3D<T>.Create(
            -Magnitude.X * w2 * _scalarProcessor.Cos(angleX.ScalarValue),
            -Magnitude.Y * w2 * _scalarProcessor.Cos(angleY.ScalarValue),
            -Magnitude.Z * w2 * _scalarProcessor.Cos(angleZ.ScalarValue)
        );
    }
}
