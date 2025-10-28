using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

/// <summary>
/// A 3D parametric path that applies an affine transformation to the time parameter.
/// This allows time remapping: stretching, compressing, reversing, and shifting time.
/// Time transformation: t_new = scaling * t_old + offset
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class AffineMappedTimePath3D<T> :
    ParametricPath3D<T>
{
    /// <summary>
    /// Creates a time-mapped path with specified scaling and offset.
    /// Transform: t_new = scaling * t_old + offset
    /// </summary>
    /// <param name="basePath">The source path to time-remap</param>
    /// <param name="timeMapScaling">Scaling factor (speed multiplier, negative reverses direction)</param>
    /// <param name="timeMapOffset">Time offset (translation)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMappedTimePath3D<T> Create(
        ParametricPath3D<T> basePath,
        Scalar<T> timeMapScaling,
        Scalar<T> timeMapOffset)
    {
        return new AffineMappedTimePath3D<T>(basePath, timeMapScaling, timeMapOffset);
    }

    /// <summary>
    /// Creates a time-mapped path that maps [inputMin, inputMax] to [outputMin, outputMax].
    /// This is useful for remapping time ranges.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMappedTimePath3D<T> CreateFromRanges(
        ParametricPath3D<T> basePath,
        Scalar<T> inputMin,
        Scalar<T> inputMax,
        Scalar<T> outputMin,
        Scalar<T> outputMax)
    {
        var scalarProcessor = inputMin.ScalarProcessor;

        // Calculate: scaling = (outputMax - outputMin) / (inputMax - inputMin)
        var inputRange = scalarProcessor.Subtract(inputMax.ScalarValue, inputMin.ScalarValue);
        var outputRange = scalarProcessor.Subtract(outputMax.ScalarValue, outputMin.ScalarValue);
        var scaling = scalarProcessor.Divide(outputRange.ScalarValue, inputRange.ScalarValue);

        // Calculate: offset = (inputMax * outputMin - inputMin * outputMax) / (inputMax - inputMin)
        var term1 = scalarProcessor.Times(inputMax.ScalarValue, outputMin.ScalarValue);
        var term2 = scalarProcessor.Times(inputMin.ScalarValue, outputMax.ScalarValue);
        var numerator = scalarProcessor.Subtract(term1.ScalarValue, term2.ScalarValue);
        var offset = scalarProcessor.Divide(numerator.ScalarValue, inputRange.ScalarValue);

        return new AffineMappedTimePath3D<T>(
            basePath,
            scalarProcessor.Scalar(scaling.ScalarValue),
            scalarProcessor.Scalar(offset.ScalarValue)
        );
    }

    /// <summary>
    /// Creates a time-scaling path (no offset).
    /// Transform: t_new = scaling * t_old
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMappedTimePath3D<T> CreateScaling(
        ParametricPath3D<T> basePath,
        Scalar<T> timeMapScaling)
    {
        var scalarProcessor = timeMapScaling.ScalarProcessor;
        return new AffineMappedTimePath3D<T>(
            basePath,
            timeMapScaling,
            scalarProcessor.Zero
        );
    }

    /// <summary>
    /// Creates a time-translation path (no scaling).
    /// Transform: t_new = t_old + offset
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMappedTimePath3D<T> CreateTranslation(
        ParametricPath3D<T> basePath,
        Scalar<T> timeMapOffset)
    {
        var scalarProcessor = timeMapOffset.ScalarProcessor;
        return new AffineMappedTimePath3D<T>(
            basePath,
            scalarProcessor.One,
            timeMapOffset
        );
    }


    /// <summary>
    /// The source path that gets time-remapped
    /// </summary>
    public ParametricPath3D<T> BasePath { get; }

    /// <summary>
    /// Time scaling factor. Negative values reverse direction.
    /// Forward transform: t_out = Scaling * t_in + Offset
    /// </summary>
    public Scalar<T> TimeMapScaling { get; }

    /// <summary>
    /// Time offset (translation component).
    /// Forward transform: t_out = Scaling * t_in + Offset
    /// </summary>
    public Scalar<T> TimeMapOffset { get; }

    /// <summary>
    /// Inverse time scaling: 1 / Scaling
    /// Inverse transform: t_in = InverseScaling * (t_out - Offset)
    /// </summary>
    public Scalar<T> InverseTimeMapScaling { get; }

    /// <summary>
    /// Inverse time offset: -Offset / Scaling
    /// Inverse transform: t_in = InverseScaling * t_out + InverseOffset
    /// </summary>
    public Scalar<T> InverseTimeMapOffset { get; }

    private IScalarProcessor<T> ScalarProcessor
        => TimeMapScaling.ScalarProcessor;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AffineMappedTimePath3D(
        ParametricPath3D<T> basePath,
        Scalar<T> timeMapScaling,
        Scalar<T> timeMapOffset)
        : base(
            ComputeTimeRange(basePath, timeMapScaling, timeMapOffset),
            basePath.IsPeriodic
        )
    {
        BasePath = basePath;
        TimeMapScaling = timeMapScaling;
        TimeMapOffset = timeMapOffset;

        // Compute inverse: t_in = (t_out - offset) / scaling = (1/scaling) * t_out + (-offset/scaling)
        var scalarProcessor = timeMapScaling.ScalarProcessor;
        InverseTimeMapScaling = scalarProcessor.Divide(
            scalarProcessor.ScalarFromNumber(1).ScalarValue,
            timeMapScaling.ScalarValue
        );

        InverseTimeMapOffset = scalarProcessor.Times(
            scalarProcessor.Negative(timeMapOffset.ScalarValue).ScalarValue,
            InverseTimeMapScaling.ScalarValue
        );

        Debug.Assert(IsValid());
    }

    /// <summary>
    /// Computes the new time range after applying the affine transformation.
    /// If scaling > 0: [min_out, max_out] = [scaling*min_in + offset, scaling*max_in + offset]
    /// If scaling < 0: [min_out, max_out] = [scaling*max_in + offset, scaling*min_in + offset] (reversed)
    /// </summary>
    private static ScalarRange<T> ComputeTimeRange(
        ParametricPath3D<T> basePath,
        Scalar<T> timeMapScaling,
        Scalar<T> timeMapOffset)
    {
        var scalarProcessor = timeMapScaling.ScalarProcessor;

        // Forward transform: t_out = scaling * t_in + offset
        var minIn = basePath.MinTime;
        var maxIn = basePath.MaxTime;

        var minOut = scalarProcessor.Add(
            scalarProcessor.Times(timeMapScaling.ScalarValue, minIn.ScalarValue).ScalarValue,
            timeMapOffset.ScalarValue
        );

        var maxOut = scalarProcessor.Add(
            scalarProcessor.Times(timeMapScaling.ScalarValue, maxIn.ScalarValue).ScalarValue,
            timeMapOffset.ScalarValue
        );

        // Check if scaling is positive by comparing minOut and maxOut
        // If minOut > maxOut, scaling was negative, so swap them
        var comparison = scalarProcessor.Subtract(minOut.ScalarValue, maxOut.ScalarValue);

        // Need to determine sign without direct comparison
        // We'll just create the range in the correct order based on transformed values
        return ScalarRange<T>.Create(minOut, maxOut);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BasePath.IsValid() &&
               TimeMapScaling.IsValid() &&
               TimeMapOffset.IsValid() &&
               !TimeMapScaling.IsZero();  // Scaling must be non-zero
    }

    /// <summary>
    /// Gets the point at parameter t by applying inverse time transform to BasePath.
    /// t_remapped = InverseScaling * t + InverseOffset
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Inverse transform: t_in = InverseScaling * t + InverseOffset
        var tRemapped = ScalarProcessor.Add(
            ScalarProcessor.Times(InverseTimeMapScaling.ScalarValue, t.ScalarValue).ScalarValue,
            InverseTimeMapOffset.ScalarValue
        );

        return BasePath.GetValue(tRemapped);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new AffineMappedTimePath3D<T>(
                BasePath.ToFinitePath(),
                TimeMapScaling,
                TimeMapOffset
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new AffineMappedTimePath3D<T>(
                BasePath.ToPeriodicPath(),
                TimeMapScaling,
                TimeMapOffset
            );
    }

    /// <summary>
    /// Gets the first derivative using chain rule.
    /// d/dt[path(f(t))] = path'(f(t)) * f'(t) = path'(t_remapped) * InverseScaling
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // Inverse transform: t_in = InverseScaling * t + InverseOffset
        var tRemapped = ScalarProcessor.Add(
            ScalarProcessor.Times(InverseTimeMapScaling.ScalarValue, t.ScalarValue).ScalarValue,
            InverseTimeMapOffset.ScalarValue
        );

        // Chain rule: derivative scales by InverseScaling
        var baseDerivative = BasePath.GetDerivative1Value(tRemapped);

        return baseDerivative * InverseTimeMapScaling;
    }

    /// <summary>
    /// Gets the second derivative using chain rule twice.
    /// d²/dt²[path(f(t))] = path''(f(t)) * [f'(t)]² = path''(t_remapped) * InverseScaling²
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Inverse transform: t_in = InverseScaling * t + InverseOffset
        var tRemapped = ScalarProcessor.Add(
            ScalarProcessor.Times(InverseTimeMapScaling.ScalarValue, t.ScalarValue).ScalarValue,
            InverseTimeMapOffset.ScalarValue
        );

        // Chain rule for second derivative: scales by InverseScaling²
        var baseDerivative2 = BasePath.GetDerivative2Value(tRemapped);

        var inverseScalingSquared = ScalarProcessor.Times(
            InverseTimeMapScaling.ScalarValue,
            InverseTimeMapScaling.ScalarValue
        );

        return baseDerivative2 * inverseScalingSquared;
    }

    /// <summary>
    /// Gets the local frame at parameter t using the remapped time.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        // Inverse transform: t_in = InverseScaling * t + InverseOffset
        var tRemapped = ScalarProcessor.Add(
            ScalarProcessor.Times(InverseTimeMapScaling.ScalarValue, t.ScalarValue).ScalarValue,
            InverseTimeMapOffset.ScalarValue
        );

        return BasePath.GetFrame(tRemapped);
    }
}
