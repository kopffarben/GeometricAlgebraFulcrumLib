using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

/// <summary>
/// Arc-length parameterized wrapper around any <see cref="ParametricPath3D{T}"/> using adaptive sampling.
/// Matches the behavior of <see cref="GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Mapped.Float64AdaptiveArcLengthPath3D"/>.
/// </summary>
public sealed class AdaptiveArcLengthPath3D<T> :
    ArcLengthPath3D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptiveArcLengthPath3D<T> Create(ParametricPath3D<T> basePath)
    {
        var options = CreateDefaultSamplingOptions(basePath.TimeRange.ScalarProcessor);
        return new AdaptiveArcLengthPath3D<T>(basePath, options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptiveArcLengthPath3D<T> Create(ParametricPath3D<T> basePath, AdaptivePath3DSamplingOptions<T> samplingOptions)
    {
        return new AdaptiveArcLengthPath3D<T>(basePath, samplingOptions);
    }

    private static AdaptivePath3DSamplingOptions<T> CreateDefaultSamplingOptions(IScalarProcessor<T> scalarProcessor)
    {
        var defaultAngle = LinDirectedAngle<T>.CreateFromDegrees(scalarProcessor, 5);
        return new AdaptivePath3DSamplingOptions<T>(scalarProcessor, defaultAngle, 3, 16);
    }

    private readonly AdaptivePath3D<T> _adaptiveCurve;
    private readonly Scalar<T> _adaptiveCurveLength;

    public ParametricPath3D<T> BasePath { get; }

    public AdaptivePath3DSamplingOptions<T> SamplingOptions { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AdaptiveArcLengthPath3D(ParametricPath3D<T> basePath, AdaptivePath3DSamplingOptions<T> samplingOptions)
        : base(basePath.TimeRange, basePath.IsPeriodic)
    {
        BasePath = basePath;
        SamplingOptions = samplingOptions;

        _adaptiveCurve = basePath.CreateAdaptiveCurve3D(samplingOptions);
        _adaptiveCurveLength = _adaptiveCurve.GetLength();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BasePath.IsValid() &&
               _adaptiveCurve.IsValid() &&
               _adaptiveCurveLength.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToFiniteArcLengthPath()
    {
        return IsFinite
            ? this
            : new AdaptiveArcLengthPath3D<T>(
                BasePath.ToFinitePath(),
                SamplingOptions
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToPeriodicArcLengthPath()
    {
        return IsPeriodic
            ? this
            : new AdaptiveArcLengthPath3D<T>(
                BasePath.ToPeriodicPath(),
                SamplingOptions
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        return BasePath.GetValue(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        return BasePath.GetDerivative1Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        return BasePath.GetDerivative2Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        return BasePath.GetFrame(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetLength()
    {
        return _adaptiveCurveLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> TimeToLength(Scalar<T> t)
    {
        return _adaptiveCurve.TimeToLength(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> LengthToTime(Scalar<T> length)
    {
        var lengthClamped = ClampLength(length);
        return _adaptiveCurve.LengthToTime(lengthClamped);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Scalar<T> ClampLength(Scalar<T> value)
    {
        var sp = value.ScalarProcessor;

        if (value.IsNegative())
            return sp.Zero;

        if ((value - _adaptiveCurveLength).IsPositive())
            return _adaptiveCurveLength;

        return value;
    }
}
