using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ConstantScalarSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestConstantZero_FiniteSignal()
    {
        var float64Signal = Float64ScalarConstantZeroSignal.FiniteInstance;
        var genericSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.Zero);

        // Test at various points
        for (double t = -1.0; t <= 1.0; t += 0.25)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
            Assert.That(float64Value, Is.EqualTo(0.0).Within(Tolerance)); // Always zero
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestConstantZero_Derivatives()
    {
        var float64Signal = Float64ScalarConstantZeroSignal.FiniteInstance;
        var genericSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.Zero);

        // Test derivatives at various points
        for (double t = -1.0; t <= 1.0; t += 0.5)
        {
            var float64Deriv1 = float64Signal.GetDerivative1Value(t);
            var genericDeriv1 = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv1 - genericDeriv1), Is.LessThan(Tolerance),
                $"Derivative1 mismatch at t={t}: Float64={float64Deriv1}, Generic={genericDeriv1}");
            Assert.That(float64Deriv1, Is.EqualTo(0.0).Within(Tolerance)); // Derivative of constant is zero

            var float64Deriv2 = float64Signal.GetDerivative2Value(t);
            var genericDeriv2 = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv2 - genericDeriv2), Is.LessThan(Tolerance),
                $"Derivative2 mismatch at t={t}: Float64={float64Deriv2}, Generic={genericDeriv2}");
            Assert.That(float64Deriv2, Is.EqualTo(0.0).Within(Tolerance)); // Second derivative of constant is zero
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestConstantOne_FiniteSignal()
    {
        var float64Signal = Float64ScalarConstantOneSignal.FiniteInstance;
        var genericSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);

        // Test at various points
        for (double t = -1.0; t <= 1.0; t += 0.25)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
            Assert.That(float64Value, Is.EqualTo(1.0).Within(Tolerance)); // Always one
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestConstantOne_Derivatives()
    {
        var float64Signal = Float64ScalarConstantOneSignal.FiniteInstance;
        var genericSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);

        // Test derivatives at various points
        for (double t = -1.0; t <= 1.0; t += 0.5)
        {
            var float64Deriv1 = float64Signal.GetDerivative1Value(t);
            var genericDeriv1 = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv1 - genericDeriv1), Is.LessThan(Tolerance),
                $"Derivative1 mismatch at t={t}: Float64={float64Deriv1}, Generic={genericDeriv1}");
            Assert.That(float64Deriv1, Is.EqualTo(0.0).Within(Tolerance)); // Derivative of constant is zero

            var float64Deriv2 = float64Signal.GetDerivative2Value(t);
            var genericDeriv2 = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv2 - genericDeriv2), Is.LessThan(Tolerance),
                $"Derivative2 mismatch at t={t}: Float64={float64Deriv2}, Generic={genericDeriv2}");
            Assert.That(float64Deriv2, Is.EqualTo(0.0).Within(Tolerance)); // Second derivative of constant is zero
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestConstantArbitrary_VariousValues()
    {
        double[] testValues = { 0.0, 1.0, -1.0, 2.5, -3.7, Math.PI, Math.E };

        foreach (var constantValue in testValues)
        {
            var genericSignal = ConstantScalarSignal<double>.Finite(
                ScalarProcessor,
                ScalarProcessor.ScalarFromNumber(constantValue)
            );

            // Test at multiple time points
            for (double t = -1.0; t <= 1.0; t += 0.5)
            {
                var value = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
                Assert.That(value, Is.EqualTo(constantValue).Within(Tolerance),
                    $"Constant signal should return {constantValue} at t={t}, got {value}");
            }
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestConstant_PeriodicSignal()
    {
        var float64Signal = Float64ScalarConstantZeroSignal.PeriodicInstance;
        var genericSignal = ConstantScalarSignal<double>.Periodic(ScalarProcessor, ScalarProcessor.Zero);

        for (double t = -1.0; t <= 1.0; t += 0.25)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestConstant_FiniteToPeriodicConversion()
    {
        var constantValue = 5.0;
        var finiteSignal = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(constantValue)
        );
        var periodicSignal = finiteSignal.ToPeriodicSignal() as ConstantScalarSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);
        Assert.That(periodicSignal.Value.ScalarValue, Is.EqualTo(constantValue));

        Debug.Assert(true);
    }

    [Test]
    public void TestConstant_PeriodicToFiniteConversion()
    {
        var constantValue = 7.5;
        var periodicSignal = ConstantScalarSignal<double>.Periodic(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(constantValue)
        );
        var finiteSignal = periodicSignal.ToFiniteSignal() as ConstantScalarSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);
        Assert.That(finiteSignal.Value.ScalarValue, Is.EqualTo(constantValue));

        Debug.Assert(true);
    }

    [Test]
    public void TestConstant_IsValid()
    {
        var signal1 = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.Zero);
        var signal2 = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var signal3 = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.ScalarFromNumber(Math.PI));

        Assert.That(signal1.IsValid(), Is.True);
        Assert.That(signal2.IsValid(), Is.True);
        Assert.That(signal3.IsValid(), Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestConstant_TimeRangeProperties()
    {
        var signal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestConstant_CustomTimeRange()
    {
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.ScalarFromNumber(-5.0),
            ScalarProcessor.ScalarFromNumber(10.0)
        );
        var signal = ConstantScalarSignal<double>.Finite(timeRange, ScalarProcessor.ScalarFromNumber(3.14));

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-5.0).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));

        // Value should be constant across entire range
        var valueAtMin = signal.GetValue(signal.TimeRange.MinValue).ScalarValue;
        var valueAtMax = signal.GetValue(signal.TimeRange.MaxValue).ScalarValue;
        Assert.That(valueAtMin, Is.EqualTo(3.14).Within(Tolerance));
        Assert.That(valueAtMax, Is.EqualTo(3.14).Within(Tolerance));

        Debug.Assert(true);
    }
}
