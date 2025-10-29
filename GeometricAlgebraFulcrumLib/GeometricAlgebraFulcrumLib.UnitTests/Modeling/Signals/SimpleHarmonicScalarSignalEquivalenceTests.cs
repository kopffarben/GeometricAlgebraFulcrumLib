using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class SimpleHarmonicScalarSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestFiniteSimpleHarmonic_BasicParameters()
    {
        int harmonicFactor = 1;
        double magnitude = 2.0;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var genericSignal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 8)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSimpleHarmonic_WithTimeOffset()
    {
        int harmonicFactor = 1;
        double magnitude = 1.0;
        double timeOffset = Math.PI / 4;

        var float64Signal = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var genericSignal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSimpleHarmonic_HigherHarmonic()
    {
        int harmonicFactor = 3;
        double magnitude = 1.5;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var genericSignal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 10)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSimpleHarmonic_BoundaryValues()
    {
        int harmonicFactor = 1;
        double magnitude = 1.0;

        var float64Signal = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude);
        var genericSignal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude)
        );

        // Test at t = -π
        var t1 = -Math.PI;
        var float64Value1 = float64Signal.GetValue(t1);
        var genericValue1 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Value1 - genericValue1), Is.LessThan(Tolerance),
            $"Mismatch at t={t1}: Float64={float64Value1}, Generic={genericValue1}");

        // Test at t = 0
        var t2 = 0.0;
        var float64Value2 = float64Signal.GetValue(t2);
        var genericValue2 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Value2 - genericValue2), Is.LessThan(Tolerance),
            $"Mismatch at t={t2}: Float64={float64Value2}, Generic={genericValue2}");
        Assert.That(float64Value2, Is.EqualTo(magnitude).Within(Tolerance)); // cos(0) = 1

        // Test at t = π
        var t3 = Math.PI;
        var float64Value3 = float64Signal.GetValue(t3);
        var genericValue3 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t3)).ScalarValue;
        Assert.That(Math.Abs(float64Value3 - genericValue3), Is.LessThan(Tolerance),
            $"Mismatch at t={t3}: Float64={float64Value3}, Generic={genericValue3}");

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSimpleHarmonic_Derivative1()
    {
        int harmonicFactor = 1;
        double magnitude = 1.0;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var genericSignal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test first derivative at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Deriv = float64Signal.GetDerivative1Value(t);
            var genericDeriv = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSimpleHarmonic_Derivative1WithHigherHarmonic()
    {
        int harmonicFactor = 2;
        double magnitude = 1.5;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var genericSignal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test first derivative at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 8)
        {
            var float64Deriv = float64Signal.GetDerivative1Value(t);
            var genericDeriv = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSimpleHarmonic_Derivative2()
    {
        int harmonicFactor = 1;
        double magnitude = 1.0;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var genericSignal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test second derivative at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Deriv = float64Signal.GetDerivative2Value(t);
            var genericDeriv = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Second derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicSimpleHarmonic()
    {
        int harmonicFactor = 1;
        double magnitude = 1.0;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarSimpleHarmonicSignal.Periodic(harmonicFactor, magnitude, timeOffset);
        var genericSignal = SimpleHarmonicScalarSignal<double>.Periodic(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
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
        int harmonicFactor = 1;
        double magnitude = 1.0;

        var finiteSignal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude)
        );
        var periodicSignal = finiteSignal.ToPeriodicSignal() as SimpleHarmonicScalarSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);
        Assert.That(periodicSignal.HarmonicFactor, Is.EqualTo(harmonicFactor));
        Assert.That(periodicSignal.Magnitude.ScalarValue, Is.EqualTo(magnitude));

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicToFiniteConversion()
    {
        int harmonicFactor = 2;
        double magnitude = 1.5;

        var periodicSignal = SimpleHarmonicScalarSignal<double>.Periodic(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude)
        );
        var finiteSignal = periodicSignal.ToFiniteSignal() as SimpleHarmonicScalarSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);
        Assert.That(finiteSignal.HarmonicFactor, Is.EqualTo(harmonicFactor));
        Assert.That(finiteSignal.Magnitude.ScalarValue, Is.EqualTo(magnitude));

        Debug.Assert(true);
    }

    [Test]
    public void TestIsValid()
    {
        var finiteSignal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            1,
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var periodicSignal = SimpleHarmonicScalarSignal<double>.Periodic(
            ScalarProcessor,
            1,
            ScalarProcessor.ScalarFromNumber(1.0)
        );

        Assert.That(finiteSignal.IsValid(), Is.True);
        Assert.That(periodicSignal.IsValid(), Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestTimeRangeProperties()
    {
        var signal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            1,
            ScalarProcessor.ScalarFromNumber(1.0)
        );

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-Math.PI).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(Math.PI).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestPropertiesAccess()
    {
        int harmonicFactor = 3;
        double magnitude = 2.5;
        double timeOffset = 0.5;

        var signal = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        Assert.That(signal.HarmonicFactor, Is.EqualTo(harmonicFactor));
        Assert.That(signal.Magnitude.ScalarValue, Is.EqualTo(magnitude));
        Assert.That(signal.TimeOffset.ScalarValue, Is.EqualTo(timeOffset));

        Debug.Assert(true);
    }
}
