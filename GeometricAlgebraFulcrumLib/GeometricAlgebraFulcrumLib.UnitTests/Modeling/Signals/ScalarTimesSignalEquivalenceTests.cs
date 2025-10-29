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
public sealed class ScalarTimesSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestTimesSignal_TwoSignals_SinTimesCos()
    {
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64Cos = Float64ScalarCosSignal.FiniteInstance;
        var float64Times = Float64ScalarTimesSignal.Finite(float64Sin, float64Cos);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericTimes = ScalarTimesSignal<double>.Finite(genericSin, genericCos);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Times.GetValue(t);
            var genericValue = genericTimes.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");

            // Verify it's actually sin(t) * cos(t)
            var expected = Math.Sin(t) * Math.Cos(t);
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_ThreeSignals()
    {
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64Cos = Float64ScalarCosSignal.FiniteInstance;
        var float64Constant = Float64ScalarConstantZeroSignal.FiniteInstance;
        // Note: Using ConstantOne would make product always include the same factor
        // Using a different constant
        var float64Two = Float64ScalarComputedSignal.Finite(Float64ScalarRange.SymmetricPi, t => 2.0);
        var float64Times = Float64ScalarTimesSignal.Finite(float64Sin, float64Cos, float64Two);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericTwo = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.ScalarFromNumber(2.0));
        var genericTimes = ScalarTimesSignal<double>.Finite(genericSin, genericCos, genericTwo);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 8)
        {
            var float64Value = float64Times.GetValue(t);
            var genericValue = genericTimes.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");

            // Verify it's actually sin(t) * cos(t) * 2
            var expected = Math.Sin(t) * Math.Cos(t) * 2.0;
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_WithConstantZero()
    {
        // Product with zero should always be zero
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64Zero = Float64ScalarConstantZeroSignal.FiniteInstance;
        var float64Times = Float64ScalarTimesSignal.Finite(float64Sin, float64Zero);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericZero = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.Zero);
        var genericTimes = ScalarTimesSignal<double>.Finite(genericSin, genericZero);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Times.GetValue(t);
            var genericValue = genericTimes.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
            Assert.That(Math.Abs(float64Value), Is.LessThan(Tolerance)); // Should be zero
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_WithConstantOne()
    {
        // Product with one should be identity
        var float64Sin = Float64ScalarSinSignal.FiniteInstance;
        var float64One = Float64ScalarConstantOneSignal.FiniteInstance;
        var float64Times = Float64ScalarTimesSignal.Finite(float64Sin, float64One);

        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericOne = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var genericTimes = ScalarTimesSignal<double>.Finite(genericSin, genericOne);

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Times.GetValue(t);
            var genericValue = genericTimes.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            var sinValue = Math.Sin(t);

            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
            Assert.That(Math.Abs(float64Value - sinValue), Is.LessThan(Tolerance)); // Should equal sin(t)
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_PeriodicSignal()
    {
        var float64Sin = Float64ScalarSinSignal.PeriodicInstance;
        var float64Cos = Float64ScalarCosSignal.PeriodicInstance;
        var float64Times = Float64ScalarTimesSignal.Periodic(float64Sin, float64Cos);

        var genericSin = SinScalarSignal<double>.Periodic(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Periodic(ScalarProcessor);
        var genericTimes = ScalarTimesSignal<double>.Periodic(genericSin, genericCos);

        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Times.GetValue(t);
            var genericValue = genericTimes.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_FiniteToPeriodicConversion()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var finiteSignal = ScalarTimesSignal<double>.Finite(genericSin, genericCos);
        var periodicSignal = finiteSignal.ToPeriodicSignal() as ScalarTimesSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);
        Assert.That(periodicSignal.Count, Is.EqualTo(2));

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_PeriodicToFiniteConversion()
    {
        var genericSin = SinScalarSignal<double>.Periodic(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Periodic(ScalarProcessor);
        var periodicSignal = ScalarTimesSignal<double>.Periodic(genericSin, genericCos);
        var finiteSignal = periodicSignal.ToFiniteSignal() as ScalarTimesSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);
        Assert.That(finiteSignal.Count, Is.EqualTo(2));

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_IsValid()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var signal = ScalarTimesSignal<double>.Finite(genericSin, genericCos);

        Assert.That(signal.IsValid(), Is.True);
        Assert.That(signal.Count, Is.EqualTo(2));

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_TimeRangeProperties()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var signal = ScalarTimesSignal<double>.Finite(genericSin, genericCos);

        // Time range should be the union (min of mins, max of maxs) of base signals
        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-Math.PI).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(Math.PI).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_Flattening()
    {
        // Test that nested TimesSignals are flattened
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericTwo = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.ScalarFromNumber(2.0));

        // Create nested: (sin * cos) * 2
        var innerTimes = ScalarTimesSignal<double>.Finite(genericSin, genericCos);
        var outerTimes = ScalarTimesSignal<double>.Finite(innerTimes, genericTwo);

        // Should be flattened to 3 base signals
        Assert.That(outerTimes.Count, Is.EqualTo(3));

        // Verify value at t=π/4 (where sin and cos are equal)
        var t = Math.PI / 4;
        var value = outerTimes.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
        var expected = Math.Sin(t) * Math.Cos(t) * 2.0;
        Assert.That(Math.Abs(value - expected), Is.LessThan(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestTimesSignal_EnumerableInterface()
    {
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var signal = ScalarTimesSignal<double>.Finite(genericSin, genericCos);

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
    public void TestTimesSignal_TrigonometricIdentity()
    {
        // Test: sin(t) * cos(t) = 0.5 * sin(2t) at specific points
        var genericSin = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericCos = CosScalarSignal<double>.Finite(ScalarProcessor);
        var timesSignal = ScalarTimesSignal<double>.Finite(genericSin, genericCos);

        for (double t = 0; t <= Math.PI; t += Math.PI / 8)
        {
            var productValue = timesSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            var identityValue = 0.5 * Math.Sin(2 * t);
            Assert.That(Math.Abs(productValue - identityValue), Is.LessThan(Tolerance),
                $"Trigonometric identity broken at t={t}");
        }

        Debug.Assert(true);
    }
}
