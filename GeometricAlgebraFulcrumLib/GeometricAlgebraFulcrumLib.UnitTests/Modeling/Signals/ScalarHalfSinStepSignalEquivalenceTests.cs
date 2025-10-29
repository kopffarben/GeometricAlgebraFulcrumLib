using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Normalized;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ScalarHalfSinStepSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestFiniteHalfSinStepSignal_BoundaryValues()
    {
        var float64Signal = Float64ScalarHalfSinStepSignal.FiniteInstance;
        var genericSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        // Test at t = -1 (left boundary, should be sin(-π/2) = -1)
        var t1 = -1.0;
        var float64Value1 = float64Signal.GetValue(t1);
        var genericValue1 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Value1 - genericValue1), Is.LessThan(Tolerance),
            $"Mismatch at t={t1}: Float64={float64Value1}, Generic={genericValue1}");
        Assert.That(float64Value1, Is.EqualTo(-1.0).Within(Tolerance));

        // Test at t = 0 (midpoint, should be sin(0) = 0)
        var t2 = 0.0;
        var float64Value2 = float64Signal.GetValue(t2);
        var genericValue2 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Value2 - genericValue2), Is.LessThan(Tolerance),
            $"Mismatch at t={t2}: Float64={float64Value2}, Generic={genericValue2}");
        Assert.That(Math.Abs(float64Value2), Is.LessThan(Tolerance)); // Should be ~0

        // Test at t = 1 (right boundary, should be sin(π/2) = 1)
        var t3 = 1.0;
        var float64Value3 = float64Signal.GetValue(t3);
        var genericValue3 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t3)).ScalarValue;
        Assert.That(Math.Abs(float64Value3 - genericValue3), Is.LessThan(Tolerance),
            $"Mismatch at t={t3}: Float64={float64Value3}, Generic={genericValue3}");
        Assert.That(float64Value3, Is.EqualTo(1.0).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteHalfSinStepSignal_NegativeRegion()
    {
        var float64Signal = Float64ScalarHalfSinStepSignal.FiniteInstance;
        var genericSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        // Test smooth transition in negative region
        for (double t = -0.9; t < 0.0; t += 0.1)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteHalfSinStepSignal_PositiveRegion()
    {
        var float64Signal = Float64ScalarHalfSinStepSignal.FiniteInstance;
        var genericSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        // Test smooth transition in positive region
        for (double t = 0.1; t <= 0.9; t += 0.1)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteHalfSinStepSignal_MonotonicallyIncreasing()
    {
        var genericSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        // Verify that the signal is monotonically increasing
        double previousValue = -2.0;
        for (double t = -0.99; t <= 0.99; t += 0.05)
        {
            var currentValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(currentValue, Is.GreaterThanOrEqualTo(previousValue),
                $"Signal is not monotonically increasing at t={t}");
            previousValue = currentValue;
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteHalfSinStepSignal_Derivative1()
    {
        var float64Signal = Float64ScalarHalfSinStepSignal.FiniteInstance;
        var genericSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        // Test first derivative at various points
        for (double t = -0.9; t <= 0.9; t += 0.2)
        {
            var float64Deriv = float64Signal.GetDerivative1Value(t);
            var genericDeriv = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteHalfSinStepSignal_Derivative1MaxValue()
    {
        var float64Signal = Float64ScalarHalfSinStepSignal.FiniteInstance;
        var genericSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        // Derivative should be maximum at t=0: (π/2) * cos(0) = π/2
        var t = 0.0;
        var float64Deriv = float64Signal.GetDerivative1Value(t);
        var genericDeriv = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;

        Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
            $"Derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
        Assert.That(float64Deriv, Is.EqualTo(Math.PI / 2).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteHalfSinStepSignal_Derivative1AtBoundaries()
    {
        var float64Signal = Float64ScalarHalfSinStepSignal.FiniteInstance;
        var genericSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        // Derivative at t=-1: (π/2) * cos(-π/2) = 0
        var t1 = -1.0;
        var float64Deriv1 = float64Signal.GetDerivative1Value(t1);
        var genericDeriv1 = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Deriv1), Is.LessThan(Tolerance));
        Assert.That(Math.Abs(float64Deriv1 - genericDeriv1), Is.LessThan(Tolerance));

        // Derivative at t=1: (π/2) * cos(π/2) = 0
        var t2 = 1.0;
        var float64Deriv2 = float64Signal.GetDerivative1Value(t2);
        var genericDeriv2 = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Deriv2), Is.LessThan(Tolerance));
        Assert.That(Math.Abs(float64Deriv2 - genericDeriv2), Is.LessThan(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteHalfSinStepSignal_Derivative2()
    {
        var float64Signal = Float64ScalarHalfSinStepSignal.FiniteInstance;
        var genericSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        // Test second derivative at various points
        for (double t = -0.9; t <= 0.9; t += 0.2)
        {
            var float64Deriv = float64Signal.GetDerivative2Value(t);
            var genericDeriv = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Second derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicHalfSinStepSignal()
    {
        var float64Signal = Float64ScalarHalfSinStepSignal.PeriodicInstance;
        var genericSignal = ScalarHalfSinStepSignal<double>.Periodic(ScalarProcessor);

        for (double t = -1.0; t <= 1.0; t += 0.2)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteToPeriodicConversion()
    {
        var finiteSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = finiteSignal.ToPeriodicSignal() as ScalarHalfSinStepSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicToFiniteConversion()
    {
        var periodicSignal = ScalarHalfSinStepSignal<double>.Periodic(ScalarProcessor);
        var finiteSignal = periodicSignal.ToFiniteSignal() as ScalarHalfSinStepSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestIsValid()
    {
        var finiteSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = ScalarHalfSinStepSignal<double>.Periodic(ScalarProcessor);

        Assert.That(finiteSignal.IsValid(), Is.True);
        Assert.That(periodicSignal.IsValid(), Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestOutOfRangeClamping()
    {
        var float64Signal = Float64ScalarHalfSinStepSignal.FiniteInstance;
        var genericSignal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        // Test below minimum
        var t1 = -2.0;
        var float64Value1 = float64Signal.GetValue(t1);
        var genericValue1 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Value1 - genericValue1), Is.LessThan(Tolerance),
            $"Clamping mismatch at t={t1}: Float64={float64Value1}, Generic={genericValue1}");
        Assert.That(float64Value1, Is.EqualTo(-1.0).Within(Tolerance));

        // Test above maximum
        var t2 = 2.0;
        var float64Value2 = float64Signal.GetValue(t2);
        var genericValue2 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Value2 - genericValue2), Is.LessThan(Tolerance),
            $"Clamping mismatch at t={t2}: Float64={float64Value2}, Generic={genericValue2}");
        Assert.That(float64Value2, Is.EqualTo(1.0).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestTimeRangeProperties()
    {
        var signal = ScalarHalfSinStepSignal<double>.Finite(ScalarProcessor);

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-1.0));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(1.0));

        Debug.Assert(true);
    }
}
