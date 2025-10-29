using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Normalized;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ScalarSharpStepSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestFiniteSharpStepSignal_BoundaryValues()
    {
        // Float64 version
        var float64Signal = Float64ScalarSharpStepSignal.FiniteInstance;

        // Generic version
        var genericSignal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);

        // Test at t = -1 (left boundary)
        var t1 = -1.0;
        var float64Value1 = float64Signal.GetValue(t1);
        var genericValue1 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Value1 - genericValue1), Is.LessThan(Tolerance),
            $"Mismatch at t={t1}: Float64={float64Value1}, Generic={genericValue1}");

        // Test at t = 0 (discontinuity)
        var t2 = 0.0;
        var float64Value2 = float64Signal.GetValue(t2);
        var genericValue2 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Value2 - genericValue2), Is.LessThan(Tolerance),
            $"Mismatch at t={t2}: Float64={float64Value2}, Generic={genericValue2}");

        // Test at t = 1 (right boundary)
        var t3 = 1.0;
        var float64Value3 = float64Signal.GetValue(t3);
        var genericValue3 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t3)).ScalarValue;
        Assert.That(Math.Abs(float64Value3 - genericValue3), Is.LessThan(Tolerance),
            $"Mismatch at t={t3}: Float64={float64Value3}, Generic={genericValue3}");

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSharpStepSignal_NegativeRegion()
    {
        var float64Signal = Float64ScalarSharpStepSignal.FiniteInstance;
        var genericSignal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);

        for (double t = -1.0; t < 0.0; t += 0.1)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSharpStepSignal_PositiveRegion()
    {
        var float64Signal = Float64ScalarSharpStepSignal.FiniteInstance;
        var genericSignal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);

        for (double t = 0.1; t <= 1.0; t += 0.1)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSharpStepSignal_Derivative1()
    {
        var float64Signal = Float64ScalarSharpStepSignal.FiniteInstance;
        var genericSignal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);

        // Test first derivative at various points
        for (double t = -1.0; t <= 1.0; t += 0.2)
        {
            var float64Deriv = float64Signal.GetDerivative1Value(t);
            var genericDeriv = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSharpStepSignal_Derivative2()
    {
        var float64Signal = Float64ScalarSharpStepSignal.FiniteInstance;
        var genericSignal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);

        // Test second derivative at various points
        for (double t = -1.0; t <= 1.0; t += 0.2)
        {
            var float64Deriv = float64Signal.GetDerivative2Value(t);
            var genericDeriv = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Second derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicSharpStepSignal_BoundaryValues()
    {
        var float64Signal = Float64ScalarSharpStepSignal.PeriodicInstance;
        var genericSignal = ScalarSharpStepSignal<double>.Periodic(ScalarProcessor);

        var testPoints = new[] { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (var t in testPoints)
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
        var finiteSignal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = finiteSignal.ToPeriodicSignal() as ScalarSharpStepSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicToFiniteConversion()
    {
        var periodicSignal = ScalarSharpStepSignal<double>.Periodic(ScalarProcessor);
        var finiteSignal = periodicSignal.ToFiniteSignal() as ScalarSharpStepSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestIsValid()
    {
        var finiteSignal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = ScalarSharpStepSignal<double>.Periodic(ScalarProcessor);

        Assert.That(finiteSignal.IsValid(), Is.True);
        Assert.That(periodicSignal.IsValid(), Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestOutOfRangeClamping_BelowMinimum()
    {
        var float64Signal = Float64ScalarSharpStepSignal.FiniteInstance;
        var genericSignal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);

        var t = -2.0;
        var float64Value = float64Signal.GetValue(t);
        var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
        Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
            $"Clamping mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");

        Debug.Assert(true);
    }

    [Test]
    public void TestOutOfRangeClamping_AboveMaximum()
    {
        var float64Signal = Float64ScalarSharpStepSignal.FiniteInstance;
        var genericSignal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);

        var t = 2.0;
        var float64Value = float64Signal.GetValue(t);
        var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
        Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
            $"Clamping mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");

        Debug.Assert(true);
    }

    [Test]
    public void TestTimeRangeProperties()
    {
        var signal = ScalarSharpStepSignal<double>.Finite(ScalarProcessor);

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-1.0));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(1.0));

        Debug.Assert(true);
    }
}
