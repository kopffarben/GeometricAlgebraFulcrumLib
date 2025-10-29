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
public sealed class ScalarPlusSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestPlusSignal_TwoSignals_SinPlusCos()
    {
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64Cos = Float64ScalarCosSignal.FiniteInstance;
        var float64Plus = Float64ScalarPlusSignal.Finite(float64Sin, float64Cos);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericPlus = ScalarPlusSignal<double>.Finite(genericSin, genericCos);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Plus.GetValue(t);
            var genericValue = genericPlus.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");

            // Verify it's actually sin(t) + cos(t)
            var expected = Math.Sin(t) + Math.Cos(t);
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_ThreeSignals()
    {
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64Cos = Float64ScalarCosSignal.FiniteInstance;
        var float64Constant = Float64ScalarConstantOneSignal.FiniteInstance;
        var float64Plus = Float64ScalarPlusSignal.Finite(float64Sin, float64Cos, float64Constant);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericConstant = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var genericPlus = ScalarPlusSignal<double>.Finite(genericSin, genericCos, genericConstant);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 8)
        {
            var float64Value = float64Plus.GetValue(t);
            var genericValue = genericPlus.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");

            // Verify it's actually sin(t) + cos(t) + 1
            var expected = Math.Sin(t) + Math.Cos(t) + 1.0;
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_Derivative1()
    {
        // (sin(t) + cos(t))' = cos(t) - sin(t)
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64Cos = Float64ScalarCosSignal.FiniteInstance;
        var float64Plus = Float64ScalarPlusSignal.Finite(float64Sin, float64Cos);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericPlus = ScalarPlusSignal<double>.Finite(genericSin, genericCos);

        // Test derivatives at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Deriv = float64Plus.GetDerivative1Value(t);
            var genericDeriv = genericPlus.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative1 mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");

            // Verify derivative: d/dt(sin(t) + cos(t)) = cos(t) - sin(t)
            var expected = Math.Cos(t) - Math.Sin(t);
            Assert.That(Math.Abs(float64Deriv - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_Derivative2()
    {
        // (sin(t) + cos(t))'' = -sin(t) - cos(t)
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64Cos = Float64ScalarCosSignal.FiniteInstance;
        var float64Plus = Float64ScalarPlusSignal.Finite(float64Sin, float64Cos);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericPlus = ScalarPlusSignal<double>.Finite(genericSin, genericCos);

        // Test second derivatives at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Deriv = float64Plus.GetDerivative2Value(t);
            var genericDeriv = genericPlus.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative2 mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");

            // Verify second derivative: d²/dt²(sin(t) + cos(t)) = -sin(t) - cos(t)
            var expected = -Math.Sin(t) - Math.Cos(t);
            Assert.That(Math.Abs(float64Deriv - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_PeriodicSignal()
    {
        var float64Sin = Float64ScalarSinSignal.PeriodicInstance;
        var float64Cos = Float64ScalarCosSignal.PeriodicInstance;
        var float64Plus = Float64ScalarPlusSignal.Periodic(float64Sin, float64Cos);

        var genericSin = SinScalarSignal<double>.Periodic(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Periodic(ScalarProcessor);
        var genericPlus = ScalarPlusSignal<double>.Periodic(genericSin, genericCos);

        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Plus.GetValue(t);
            var genericValue = genericPlus.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_FiniteToPeriodicConversion()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var finiteSignal = ScalarPlusSignal<double>.Finite(genericSin, genericCos);
        var periodicSignal = finiteSignal.ToPeriodicSignal() as ScalarPlusSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);
        Assert.That(periodicSignal.Count, Is.EqualTo(2));

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_PeriodicToFiniteConversion()
    {
        var genericSin = SinScalarSignal<double>.Periodic(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Periodic(ScalarProcessor);
        var periodicSignal = ScalarPlusSignal<double>.Periodic(genericSin, genericCos);
        var finiteSignal = periodicSignal.ToFiniteSignal() as ScalarPlusSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);
        Assert.That(finiteSignal.Count, Is.EqualTo(2));

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_IsValid()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var signal = ScalarPlusSignal<double>.Finite(genericSin, genericCos);

        Assert.That(signal.IsValid(), Is.True);
        Assert.That(signal.Count, Is.EqualTo(2));

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_TimeRangeProperties()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var signal = ScalarPlusSignal<double>.Finite(genericSin, genericCos);

        // Time range should be the union (min of mins, max of maxs) of base signals
        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-Math.PI).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(Math.PI).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_Flattening()
    {
        // Test that nested PlusSignals are flattened
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericConstant = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);

        // Create nested: (sin + cos) + constant
        var innerPlus = ScalarPlusSignal<double>.Finite(genericSin, genericCos);
        var outerPlus = ScalarPlusSignal<double>.Finite(innerPlus, genericConstant);

        // Should be flattened to 3 base signals
        Assert.That(outerPlus.Count, Is.EqualTo(3));

        // Verify value at t=0
        var value = outerPlus.GetValue(ScalarProcessor.Zero).ScalarValue;
        var expected = Math.Sin(0) + Math.Cos(0) + 1.0; // 0 + 1 + 1 = 2
        Assert.That(Math.Abs(value - expected), Is.LessThan(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_EnumerableInterface()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var signal = ScalarPlusSignal<double>.Finite(genericSin, genericCos);

        // Test IReadOnlyList interface
        Assert.That(signal.Count, Is.EqualTo(2));
        Assert.That(signal[0], Is.EqualTo(genericSin));
        Assert.That(signal[1], Is.EqualTo(genericCos));

        // Test enumeration
        var count = 0;
        foreach (var s in signal)
        {
            Assert.That(s, Is.Not.Null);
            count++;
        }
        Assert.That(count, Is.EqualTo(2));

        Debug.Assert(true);
    }

    [Test]
    public void TestPlusSignal_WithDifferentTimeRanges()
    {
        // Create signals with different time ranges
        var signal1 = ConstantScalarSignal<double>.Finite(
            ScalarRange<double>.Create(ScalarProcessor.ScalarFromNumber(-2.0), ScalarProcessor.ScalarFromNumber(1.0)),
            ScalarProcessor.ScalarFromNumber(3.0)
        );
        var signal2 = ConstantScalarSignal<double>.Finite(
            ScalarRange<double>.Create(ScalarProcessor.ScalarFromNumber(-1.0), ScalarProcessor.ScalarFromNumber(2.0)),
            ScalarProcessor.ScalarFromNumber(5.0)
        );

        var plusSignal = ScalarPlusSignal<double>.Finite(signal1, signal2);

        // Time range should be union: min(-2, -1) = -2, max(1, 2) = 2
        Assert.That(plusSignal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-2.0).Within(Tolerance));
        Assert.That(plusSignal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));

        // Value at t=0 should be 3 + 5 = 8
        var value = plusSignal.GetValue(ScalarProcessor.Zero).ScalarValue;
        Assert.That(value, Is.EqualTo(8.0).Within(Tolerance));

        Debug.Assert(true);
    }
}
