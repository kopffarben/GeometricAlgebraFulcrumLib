using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;

/// <summary>
/// A simple harmonic motion path in 2D space
/// Each component oscillates as: magnitude * cos(w * (t + timeOffset))
/// where w = 2π * harmonicFactor
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class SimpleHarmonicPath2D<T> :
    ParametricPath2D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicPath2D<T> Create(IScalarProcessor<T> scalarProcessor, bool isPeriodic, int harmonicFactor, LinVector2D<T> magnitude, LinVector2D<T> timeOffset)
    {
        var timeRange = ScalarRange<T>.SymmetricPi(scalarProcessor);

        return new SimpleHarmonicPath2D<T>(timeRange, isPeriodic, harmonicFactor, magnitude, timeOffset);
    }


    public int HarmonicFactor { get; }

    public LinVector2D<T> Magnitude { get; }

    public LinVector2D<T> TimeOffset { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SimpleHarmonicPath2D(ScalarRange<T> timeRange, bool isPeriodic, int harmonicFactor, LinVector2D<T> magnitude, LinVector2D<T> timeOffset)
        : base(timeRange, isPeriodic)
    {
        //if (harmonicFactor < 1)
        //    throw new ArgumentOutOfRangeException(nameof(harmonicFactor));

        if (!magnitude.IsValid())
            throw new ArgumentException(nameof(magnitude));

        if (!timeOffset.IsValid())
            throw new ArgumentException(nameof(timeOffset));

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
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new SimpleHarmonicPath2D<T>(
                TimeRange,
                false,
                HarmonicFactor,
                Magnitude,
                TimeOffset
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new SimpleHarmonicPath2D<T>(
                TimeRange,
                true,
                HarmonicFactor,
                Magnitude,
                TimeOffset
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        var scalarProcessor = t.ScalarProcessor;
        var w = scalarProcessor.PiTimes2 * scalarProcessor.ScalarFromNumber(HarmonicFactor);

        // magnitude * cos(w * (t + timeOffset))
        var argX = (w * (t + TimeOffset.X)).ScalarValue;
        var argY = (w * (t + TimeOffset.Y)).ScalarValue;

        var cosX = scalarProcessor.Cos(argX);
        var cosY = scalarProcessor.Cos(argY);

        return LinVector2D<T>.Create(
            Magnitude.X * cosX,
            Magnitude.Y * cosY
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        var scalarProcessor = t.ScalarProcessor;
        var w = scalarProcessor.PiTimes2 * scalarProcessor.ScalarFromNumber(HarmonicFactor);

        // -magnitude * w * sin(w * (t + timeOffset))
        var argX = (w * (t + TimeOffset.X)).ScalarValue;
        var argY = (w * (t + TimeOffset.Y)).ScalarValue;

        var sinX = scalarProcessor.Sin(argX);
        var sinY = scalarProcessor.Sin(argY);

        return LinVector2D<T>.Create(
            -Magnitude.X * w * sinX,
            -Magnitude.Y * w * sinY
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        var scalarProcessor = t.ScalarProcessor;
        var w = scalarProcessor.PiTimes2 * scalarProcessor.ScalarFromNumber(HarmonicFactor);
        var w2 = w * w;

        // -magnitude * w² * cos(w * (t + timeOffset))
        var argX = (w * (t + TimeOffset.X)).ScalarValue;
        var argY = (w * (t + TimeOffset.Y)).ScalarValue;

        var cosX = scalarProcessor.Cos(argX);
        var cosY = scalarProcessor.Cos(argY);

        return LinVector2D<T>.Create(
            -Magnitude.X * w2 * cosX,
            -Magnitude.Y * w2 * cosY
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        // Get tangent vector from first derivative
        var tangent = GetDerivative1Value(t);
        var tangentNorm = tangent.Norm();

        var normalizedTangent = tangent.ScalarProcessor.IsZero(tangentNorm.ScalarValue)
            ? LinVector2D<T>.UnitSymmetric(tangent.ScalarProcessor)
            : tangent / tangentNorm;

        return ParametricPath2DLocalFrame<T>.Create(
            t,
            GetValue(t),
            normalizedTangent
        );
    }
}
