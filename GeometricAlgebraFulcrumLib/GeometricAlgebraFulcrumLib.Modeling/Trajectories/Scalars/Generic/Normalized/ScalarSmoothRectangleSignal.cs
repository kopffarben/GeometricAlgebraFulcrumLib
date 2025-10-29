using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;

/// <summary>
/// A smooth rectangle signal with continuous transitions.
/// Uses exponential functions for smooth interpolation between -1 and 1.
/// Formula: 1 - 2 / (1 + exp(1/t - 1/(1-t)))
/// </summary>
public sealed class ScalarSmoothRectangleSignal<T> :
    ScalarNormalizedSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSmoothRectangleSignal<T> Finite(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarSmoothRectangleSignal<T>(scalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSmoothRectangleSignal<T> Periodic(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarSmoothRectangleSignal<T>(scalarProcessor, true);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarSmoothRectangleSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
        : base(scalarProcessor, isPeriodic)
    {
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return Finite(ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return Periodic(ScalarProcessor);
    }

    public override Scalar<T> GetValue(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        // Boundary cases (approaching ±1)
        var almostOne = ScalarProcessor.One - ScalarProcessor.One / ScalarProcessor.ScalarFromNumber(1e9);
        var almostMinusOne = -almostOne;

        if (clampedT <= almostMinusOne || clampedT >= almostOne)
            return ScalarProcessor.MinusOne;

        // Near zero case
        var nearZero = ScalarProcessor.One / ScalarProcessor.ScalarFromNumber(1e9);
        if (clampedT > -nearZero && clampedT < nearZero)
            return ScalarProcessor.One;

        // Apply absolute value (work with positive t)
        var tAbs = clampedT < ScalarProcessor.Zero ? -clampedT : clampedT;

        // Formula: 1 - 2 / (1 + exp(1/t - 1/(1-t)))
        var two = ScalarProcessor.One + ScalarProcessor.One;
        var oneMinusT = ScalarProcessor.One - tAbs;
        var exponent = ScalarProcessor.One / tAbs - ScalarProcessor.One / oneMinusT;
        var expValue = exponent.Exp();
        var result = ScalarProcessor.One - two / (ScalarProcessor.One + expValue);

        // Handle potential NaN
        return result.IsValid() ? result : ScalarProcessor.Zero;
    }

    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        // Boundary cases
        var almostOne = ScalarProcessor.One - ScalarProcessor.One / ScalarProcessor.ScalarFromNumber(1e9);
        var almostMinusOne = -almostOne;

        if (clampedT <= almostMinusOne || clampedT >= almostOne)
            return ScalarProcessor.Zero;

        // Near zero case
        var nearZero = ScalarProcessor.One / ScalarProcessor.ScalarFromNumber(1e9);
        if (clampedT > -nearZero && clampedT < nearZero)
            return ScalarProcessor.Zero;

        // Determine sign
        var two = ScalarProcessor.One + ScalarProcessor.One;
        var s = -two;
        var tAbs = clampedT;

        if (clampedT < ScalarProcessor.Zero)
        {
            tAbs = -clampedT;
            s = two;
        }

        // Compute exp values
        var e1 = (ScalarProcessor.One / tAbs).Exp();
        var e2 = (ScalarProcessor.One / (ScalarProcessor.One - tAbs)).Exp();

        // Formula: s * e1 * e2 * (1 - 2*t + 2*t²) / [t * (1-t) * (e1+e2)]²
        var tSquared = tAbs * tAbs;
        var numerator = s * e1 * e2 * (ScalarProcessor.One - two * tAbs + two * tSquared);
        var temp = tAbs * (ScalarProcessor.One - tAbs) * (e1 + e2);
        var denominator = temp * temp; // Square the whole expression
        var value = numerator / denominator;

        // Handle potential NaN
        return value.IsValid() ? value : ScalarProcessor.Zero;
    }

    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        // Boundary cases
        var almostOne = ScalarProcessor.One - ScalarProcessor.One / ScalarProcessor.ScalarFromNumber(1e9);
        var almostMinusOne = -almostOne;

        if (clampedT <= almostMinusOne || clampedT >= almostOne)
            return ScalarProcessor.Zero;

        // Near zero case
        var nearZero = ScalarProcessor.One / ScalarProcessor.ScalarFromNumber(1e9);
        if (clampedT > -nearZero && clampedT < nearZero)
            return ScalarProcessor.Zero;

        // Apply absolute value
        var tAbs = clampedT < ScalarProcessor.Zero ? -clampedT : clampedT;

        // Compute exp values
        var e1 = (ScalarProcessor.One / tAbs).Exp();
        var e2 = (ScalarProcessor.One / (ScalarProcessor.One - tAbs)).Exp();

        // Complex formula from Float64 version:
        // (2 * e1 * e2 * (e2 * (1 - 2*t + 4*t³ - 6*t⁴ + 4*t⁵) + e1 * (-1 + 2*(-1+t)*t*(-3+2*t)*(1+(-1+t)*t)))) /
        // ((e1 + e2)³ * (-1+t)⁴ * t⁴)

        var two = ScalarProcessor.One + ScalarProcessor.One;
        var three = two + ScalarProcessor.One;
        var four = two * two;
        var six = three * two;

        var t2 = tAbs * tAbs;
        var t3 = t2 * tAbs;
        var t4 = t2 * t2;
        var t5 = t4 * tAbs;

        var minusOnePlusT = tAbs - ScalarProcessor.One; // (-1 + t)
        var minusOnePlusT_t = minusOnePlusT * tAbs; // (-1+t)*t
        var minusThreePlusTwoT = -three + two * tAbs; // (-3 + 2*t)
        var onePlusMinusOnePlusT_t = ScalarProcessor.One + minusOnePlusT_t; // (1 + (-1+t)*t)

        var term1 = e2 * (ScalarProcessor.One - two * tAbs + four * t3 - six * t4 + four * t5);
        var term2 = e1 * (-ScalarProcessor.One + two * minusOnePlusT_t * minusThreePlusTwoT * onePlusMinusOnePlusT_t);

        var e1PlusE2 = e1 + e2;
        var e1PlusE2_cubed = e1PlusE2 * e1PlusE2 * e1PlusE2;
        var minusOnePlusT_4th = minusOnePlusT * minusOnePlusT * minusOnePlusT * minusOnePlusT;
        var t_4th = t4;

        var numerator = two * e1 * e2 * (term1 + term2);
        var denominator = e1PlusE2_cubed * minusOnePlusT_4th * t_4th;

        var value = numerator / denominator;

        // Handle potential NaN
        return value.IsValid() ? value : ScalarProcessor.Zero;
    }
}
