using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;

/// <summary>
/// A triangle signal that ramps up from -1 to 1 and then down to -1.
/// The vertex (peak at value 1) occurs at VertexTime (range [-1, 1]).
/// </summary>
public sealed class ScalarTriangleSignal<T> :
    ScalarNormalizedSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTriangleSignal<T> FiniteSymmetric(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarTriangleSignal<T>(scalarProcessor, false, scalarProcessor.Zero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTriangleSignal<T> PeriodicSymmetric(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarTriangleSignal<T>(scalarProcessor, true, scalarProcessor.Zero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTriangleSignal<T> Finite(IScalarProcessor<T> scalarProcessor, Scalar<T> vertexTime)
    {
        return new ScalarTriangleSignal<T>(scalarProcessor, false, vertexTime);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarTriangleSignal<T> Periodic(IScalarProcessor<T> scalarProcessor, Scalar<T> vertexTime)
    {
        return new ScalarTriangleSignal<T>(scalarProcessor, true, vertexTime);
    }


    public Scalar<T> VertexTime { get; }

    public Scalar<T> VertexRelativeTime
        => (VertexTime + ScalarProcessor.One) / (ScalarProcessor.One + ScalarProcessor.One);

    public bool IsSymmetric
        => VertexTime == ScalarProcessor.Zero;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarTriangleSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic, Scalar<T> vertexTime)
        : base(scalarProcessor, isPeriodic)
    {
        if (vertexTime < TimeRange.MinValue || vertexTime > TimeRange.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(vertexTime),
                $"VertexTime must be in range [{TimeRange.MinValue}, {TimeRange.MaxValue}]");

        VertexTime = vertexTime;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        if (IsFinite)
            return this;

        if (IsSymmetric)
            return FiniteSymmetric(ScalarProcessor);

        return new ScalarTriangleSignal<T>(ScalarProcessor, false, VertexTime);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        if (IsPeriodic)
            return this;

        if (IsSymmetric)
            return PeriodicSymmetric(ScalarProcessor);

        return new ScalarTriangleSignal<T>(ScalarProcessor, true, VertexTime);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        var two = ScalarProcessor.One + ScalarProcessor.One;

        // Piecewise linear: ramp up to vertex, then ramp down
        if (clampedT <= VertexTime)
        {
            // Rising slope: from (-1, -1) to (VertexTime, 1)
            // Formula: 2 * (t + 1) / (VertexTime + 1) - 1
            return two * (clampedT + ScalarProcessor.One) / (VertexTime + ScalarProcessor.One) - ScalarProcessor.One;
        }
        else
        {
            // Falling slope: from (VertexTime, 1) to (1, -1)
            // Formula: 2 * (t - 1) / (VertexTime - 1) - 1
            return two * (clampedT - ScalarProcessor.One) / (VertexTime - ScalarProcessor.One) - ScalarProcessor.One;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        var two = ScalarProcessor.One + ScalarProcessor.One;

        // Constant slope before and after vertex
        if (clampedT <= VertexTime)
        {
            // Rising slope: 2 / (VertexTime + 1)
            return two / (VertexTime + ScalarProcessor.One);
        }
        else
        {
            // Falling slope: 2 / (VertexTime - 1)
            return two / (VertexTime - ScalarProcessor.One);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative is 0 (piecewise linear)
        return ScalarProcessor.Zero;
    }
}
