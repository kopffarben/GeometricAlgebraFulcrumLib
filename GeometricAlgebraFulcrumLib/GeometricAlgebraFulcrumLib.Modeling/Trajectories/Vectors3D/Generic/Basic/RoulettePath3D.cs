using System;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;

/// <summary>
/// Generic implementation of a roulette path: a generator point on a moving curve is rolled along a fixed curve.
/// The path parameter represents travelled arc length.
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class RoulettePath3D<T> :
    ParametricPath3D<T>
{
    private readonly IScalarProcessor<T> _scalarProcessor;

    public ArcLengthPath3D<T> FixedCurve { get; }

    public ArcLengthPath3D<T> MovingCurve { get; }

    public LinVector3D<T> GeneratorPoint { get; }

    public Scalar<T> ParameterValueMax
        => TimeRange.MaxValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RoulettePath3D(
        bool isPeriodic,
        ArcLengthPath3D<T> fixedCurve,
        ArcLengthPath3D<T> movingCurve,
        Scalar<T> parameterValueMax,
        LinVector3D<T>? generatorPoint = null)
        : base(
            ScalarRange<T>.Create(
                parameterValueMax.ScalarProcessor.Zero,
                parameterValueMax
            ),
            isPeriodic)
    {
        _scalarProcessor = parameterValueMax.ScalarProcessor;
        FixedCurve = fixedCurve ?? throw new ArgumentNullException(nameof(fixedCurve));
        MovingCurve = movingCurve ?? throw new ArgumentNullException(nameof(movingCurve));
        GeneratorPoint = generatorPoint ?? MovingCurve.GetValue(MovingCurve.TimeRange.MinValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RoulettePath3D<T> Create(
        ArcLengthPath3D<T> fixedCurve,
        ArcLengthPath3D<T> movingCurve,
        Scalar<T>? parameterValueMax = null,
        bool isPeriodic = false,
        LinVector3D<T>? generatorPoint = null)
    {
        var maxValue = parameterValueMax ?? movingCurve.GetLength();
        return new RoulettePath3D<T>(
            isPeriodic,
            fixedCurve,
            movingCurve,
            maxValue,
            generatorPoint
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return FixedCurve.IsValid() &&
               MovingCurve.IsValid() &&
               GeneratorPoint.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsPeriodic
            ? new RoulettePath3D<T>(false, FixedCurve, MovingCurve, ParameterValueMax, GeneratorPoint)
            : this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsFinite
            ? new RoulettePath3D<T>(true, FixedCurve, MovingCurve, ParameterValueMax, GeneratorPoint)
            : this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> parameterValue)
    {
        return ComputeValue(parameterValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> parameterValue)
    {
        var numericalOperations = _scalarProcessor.NumericalOperations
            ?? throw new InvalidOperationException(
                "RoulettePath3D<T> requires ScalarProcessor.NumericalOperations to approximate derivatives.");

        Scalar<T> DifferentiateComponent(Func<Scalar<T>, Scalar<T>> component)
            => numericalOperations.Differentiate(component, parameterValue);

        var dx = DifferentiateComponent(t => ComputeValue(t).X);
        var dy = DifferentiateComponent(t => ComputeValue(t).Y);
        var dz = DifferentiateComponent(t => ComputeValue(t).Z);

        return new LinVector3D<T>(new Triplet<Scalar<T>>(dx, dy, dz));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> parameterValue)
    {
        var numericalOperations = _scalarProcessor.NumericalOperations
            ?? throw new InvalidOperationException(
                "RoulettePath3D<T> requires ScalarProcessor.NumericalOperations to approximate derivatives.");

        Scalar<T> DifferentiateComponent(Func<Scalar<T>, Scalar<T>> component)
            => numericalOperations.Differentiate2(component, parameterValue);

        var ddx = DifferentiateComponent(t => ComputeValue(t).X);
        var ddy = DifferentiateComponent(t => ComputeValue(t).Y);
        var ddz = DifferentiateComponent(t => ComputeValue(t).Z);

        return new LinVector3D<T>(new Triplet<Scalar<T>>(ddx, ddy, ddz));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LinVector3D<T> ComputeValue(Scalar<T> parameterValue)
    {
        var clamped = TimeRange.Clamp(parameterValue);

        var movingTime = MovingCurve.LengthToTime(clamped);
        var movingFrame = MovingCurve.GetFrame(movingTime);

        var fixedTime = FixedCurve.LengthToTime(clamped);
        var fixedFrame = FixedCurve.GetFrame(fixedTime);

        var relativeVector = GeneratorPoint - movingFrame.Point;

        var rotatedVector =
            RotateVectorBetweenFrames(relativeVector, movingFrame, fixedFrame);

        return fixedFrame.Point + rotatedVector;
    }

    private static LinVector3D<T> RotateVectorBetweenFrames(
        LinVector3D<T> vector,
        ParametricPath3DLocalFrame<T> sourceFrame,
        ParametricPath3DLocalFrame<T> targetFrame)
    {
        var c1 = sourceFrame.Tangent.Sp(vector);
        var c2 = sourceFrame.Normal1.Sp(vector);
        var c3 = sourceFrame.Normal2.Sp(vector);

        return targetFrame.Tangent * c1 +
               targetFrame.Normal1 * c2 +
               targetFrame.Normal2 * c3;
    }
}
