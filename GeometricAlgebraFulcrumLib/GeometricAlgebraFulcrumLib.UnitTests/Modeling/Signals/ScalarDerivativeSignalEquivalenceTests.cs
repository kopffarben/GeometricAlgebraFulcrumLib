using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ScalarDerivativeSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestDerivativeSignal_SinBase()
    {
        // Derivative of sin(t) is cos(t)
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64Derivative = Float64ScalarDerivativeSignal.Create(float64Sin);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericDerivative = ScalarDerivativeSignal<double>.Create(genericSin);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Derivative.GetValue(t);
            var genericValue = genericDerivative.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");

            // Verify it's actually cos(t)
            var expected = Math.Cos(t);
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_CosBase()
    {
        // Derivative of cos(t) is -sin(t)
        var float64Cos = Float64ScalarCosSignal.FiniteInstance;
        var float64Derivative = Float64ScalarDerivativeSignal.Create(float64Cos);

        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericDerivative = ScalarDerivativeSignal<double>.Create(genericCos);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Derivative.GetValue(t);
            var genericValue = genericDerivative.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");

            // Verify it's actually -sin(t)
            var expected = -Math.Sin(t);
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_SecondDerivative_Sin()
    {
        // Second derivative of sin(t) is -sin(t)
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64Derivative = Float64ScalarDerivativeSignal.Create(float64Sin);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericDerivative = ScalarDerivativeSignal<double>.Create(genericSin);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Deriv = float64Derivative.GetDerivative1Value(t);
            var genericDeriv = genericDerivative.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative1 mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");

            // Verify second derivative: d²/dt²(sin(t)) = -sin(t)
            var expected = -Math.Sin(t);
            Assert.That(Math.Abs(float64Deriv - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_SecondDerivative_Cos()
    {
        // Second derivative of cos(t) is -cos(t)
        var float64Cos = Float64ScalarCosSignal.FiniteInstance;
        var float64Derivative = Float64ScalarDerivativeSignal.Create(float64Cos);

        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericDerivative = ScalarDerivativeSignal<double>.Create(genericCos);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Deriv = float64Derivative.GetDerivative1Value(t);
            var genericDeriv = genericDerivative.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative1 mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");

            // Verify second derivative: d²/dt²(cos(t)) = -cos(t)
            var expected = -Math.Cos(t);
            Assert.That(Math.Abs(float64Deriv - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_ChainedDerivatives()
    {
        // Test derivative of derivative: d²/dt²(sin(t)) = d/dt(cos(t)) = -sin(t)
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var firstDerivative = ScalarDerivativeSignal<double>.Create(genericSin);
        var secondDerivative = ScalarDerivativeSignal<double>.Create(firstDerivative);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            // First derivative should be cos(t)
            var firstDerivValue = firstDerivative.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(firstDerivValue - Math.Cos(t)), Is.LessThan(Tolerance));

            // Second derivative should be -sin(t)
            var secondDerivValue = secondDerivative.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            var expected = -Math.Sin(t);
            Assert.That(Math.Abs(secondDerivValue - expected), Is.LessThan(Tolerance),
                $"Second derivative mismatch at t={t}: Got={secondDerivValue}, Expected={expected}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_ConstantBase()
    {
        // Derivative of constant is zero
        var genericConstant = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.ScalarFromNumber(5.0));
        var derivative = ScalarDerivativeSignal<double>.Create(genericConstant);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var value = derivative.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(value), Is.LessThan(Tolerance),
                $"Derivative of constant should be zero at t={t}, got {value}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_PeriodicSignal()
    {
        var float64Sin = Float64ScalarSinSignal.PeriodicInstance;
        var float64Derivative = Float64ScalarDerivativeSignal.Create(float64Sin);

        var genericSin = SinScalarSignal<double>.Periodic(ScalarProcessor);
        var genericDerivative = ScalarDerivativeSignal<double>.Create(genericSin);

        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Derivative.GetValue(t);
            var genericValue = genericDerivative.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_FiniteToPeriodicConversion()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var finiteSignal = ScalarDerivativeSignal<double>.Create(genericSin);
        var periodicSignal = finiteSignal.ToPeriodicSignal() as ScalarDerivativeSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);
        Assert.That(periodicSignal.BaseSignal, Is.Not.Null);

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_PeriodicToFiniteConversion()
    {
        var genericSin = SinScalarSignal<double>.Periodic(ScalarProcessor);
        var periodicSignal = ScalarDerivativeSignal<double>.Create(genericSin);
        var finiteSignal = periodicSignal.ToFiniteSignal() as ScalarDerivativeSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);
        Assert.That(finiteSignal.BaseSignal, Is.Not.Null);

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_IsValid()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal = ScalarDerivativeSignal<double>.Create(genericSin);

        Assert.That(signal.IsValid(), Is.True);
        Assert.That(signal.BaseSignal, Is.Not.Null);

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_TimeRangeProperties()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal = ScalarDerivativeSignal<double>.Create(genericSin);

        // Time range should match base signal
        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-Math.PI).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(Math.PI).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_BaseSignalProperty()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal = ScalarDerivativeSignal<double>.Create(genericSin);

        // Verify BaseSignal property is accessible
        Assert.That(signal.BaseSignal, Is.EqualTo(genericSin));
        Assert.That(signal.BaseSignal, Is.Not.Null);

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_PreservesPeriodicity()
    {
        // Test that derivative preserves periodicity flag
        var periodicSin = SinScalarSignal<double>.Periodic(ScalarProcessor);
        var periodicDerivative = ScalarDerivativeSignal<double>.Create(periodicSin);
        Assert.That(periodicDerivative.IsPeriodic, Is.True);

        var finiteSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var finiteDerivative = ScalarDerivativeSignal<double>.Create(finiteSin);
        Assert.That(finiteDerivative.IsFinite, Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestDerivativeSignal_PlusSignalDerivative()
    {
        // Test derivative of (sin + cos)
        // d/dt(sin(t) + cos(t)) = cos(t) - sin(t)
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var plusSignal = ScalarPlusSignal<double>.Finite(genericSin, genericCos);
        var derivative = ScalarDerivativeSignal<double>.Create(plusSignal);

        // Derivative should be cos(t) - sin(t)
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var derivValue = derivative.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            var expected = Math.Cos(t) - Math.Sin(t);
            Assert.That(Math.Abs(derivValue - expected), Is.LessThan(Tolerance),
                $"Derivative mismatch at t={t}: Got={derivValue}, Expected={expected}");
        }

        Debug.Assert(true);
    }
}
