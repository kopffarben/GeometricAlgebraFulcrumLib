using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;

/// <summary>
/// A smooth step signal that smoothly transitions from -1 to 1.
/// Uses a sigmoid-like function for smooth interpolation.
/// Formula: 2 / (1 + exp(4*t / (t² - 1))) - 1
/// </summary>
public sealed class ScalarSmoothStepSignal<T> :
    ScalarNormalizedSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSmoothStepSignal<T> Finite(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarSmoothStepSignal<T>(scalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSmoothStepSignal<T> Periodic(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarSmoothStepSignal<T>(scalarProcessor, true);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarSmoothStepSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        // Boundary cases
        if (clampedT <= TimeRange.MinValue)
            return ScalarProcessor.MinusOne;

        if (clampedT >= TimeRange.MaxValue)
            return ScalarProcessor.One;

        // Smooth transition: 2 / (1 + exp(4*t / (t² - 1))) - 1
        var tSquared = clampedT * clampedT;
        var tSquaredMinusOne = tSquared - ScalarProcessor.One;
        var two = ScalarProcessor.One + ScalarProcessor.One;
        var four = two * two;
        var exponent = four * clampedT / tSquaredMinusOne;
        var expValue = exponent.Exp();
        var result = two / (ScalarProcessor.One + expValue) - ScalarProcessor.One;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        // Boundary cases: derivative is 0 at boundaries
        if (clampedT <= TimeRange.MinValue || clampedT >= TimeRange.MaxValue)
            return ScalarProcessor.Zero;

        // Derivative formula:
        // a = t² + 1
        // b = 1 / (t² - 1)
        // c = 1 / cosh(2*t*b)
        // result = 2 * a * b² * c²

        var tSquared = clampedT * clampedT;
        var a = tSquared + ScalarProcessor.One;

        var tSquaredMinusOne = tSquared - ScalarProcessor.One;
        var b = ScalarProcessor.One / tSquaredMinusOne;

        var two = ScalarProcessor.One + ScalarProcessor.One;
        var twoTB = two * clampedT * b;
        var c = ScalarProcessor.One / twoTB.Cosh();

        // result = 2 * a * b² * c²
        var result = two * a * b * b * c * c;

        // Handle potential NaN (replace with 0)
        return result.IsValid() ? result : ScalarProcessor.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        // Clamp time to [-1, 1] range
        var clampedT = t < TimeRange.MinValue ? TimeRange.MinValue :
                       t > TimeRange.MaxValue ? TimeRange.MaxValue : t;

        // Boundary cases: derivative is 0 at boundaries
        if (clampedT <= TimeRange.MinValue || clampedT >= TimeRange.MaxValue)
            return ScalarProcessor.Zero;

        // Second derivative formula (complex):
        // a = t² + 1
        // b = 1 / (t² - 1)
        // c = 2*t*b
        // t1 = 3*t - 2*t³ - t⁵ + 2*a²*tanh(c)
        // result = 4 * t1 * b⁴ / cosh(c)²

        var tSquared = clampedT * clampedT;
        var tCubed = tSquared * clampedT;
        var tToThe5 = tCubed * tSquared;

        var a = tSquared + ScalarProcessor.One;
        var aSquared = a * a;

        var tSquaredMinusOne = tSquared - ScalarProcessor.One;
        var b = ScalarProcessor.One / tSquaredMinusOne;

        var two = ScalarProcessor.One + ScalarProcessor.One;
        var c = two * clampedT * b;

        var tanhC = c.Tanh();

        // t1 = 3*t - 2*t³ - t⁵ + 2*a²*tanh(c)
        var three = two + ScalarProcessor.One;
        var t1 = three * clampedT - two * tCubed - tToThe5 + two * aSquared * tanhC;

        // b⁴
        var bToThe4 = b * b * b * b;

        // cosh(c)²
        var coshC = c.Cosh();
        var coshCSquared = coshC * coshC;

        // result = 4 * t1 * b⁴ / cosh(c)²
        var four = two * two;
        var result = four * t1 * bToThe4 / coshCSquared;

        // Handle potential NaN (replace with 0)
        return result.IsValid() ? result : ScalarProcessor.Zero;
    }
}
