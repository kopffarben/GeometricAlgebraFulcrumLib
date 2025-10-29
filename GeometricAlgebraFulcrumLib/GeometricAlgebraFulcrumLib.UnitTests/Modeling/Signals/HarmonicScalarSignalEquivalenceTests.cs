using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class HarmonicScalarSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestFiniteHarmonic_BasicParameters()
    {
        double frequencyHz = 1.0;
        double magnitude = 2.0;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarHarmonicSignal.Finite(Float64ScalarRange.SymmetricOne, frequencyHz, magnitude, timeOffset);
        var genericSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test at various points
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
    public void TestFiniteHarmonic_WithTimeOffset()
    {
        double frequencyHz = 1.0;
        double magnitude = 1.0;
        double timeOffset = 0.5;

        var float64Signal = Float64ScalarHarmonicSignal.Finite(Float64ScalarRange.SymmetricOne, frequencyHz, magnitude, timeOffset);
        var genericSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test at various points
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
    public void TestFiniteHarmonic_HigherFrequency()
    {
        double frequencyHz = 3.0;
        double magnitude = 1.5;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarHarmonicSignal.Finite(Float64ScalarRange.SymmetricOne, frequencyHz, magnitude, timeOffset);
        var genericSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test at various points
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
    public void TestFiniteHarmonic_BoundaryValues()
    {
        double frequencyHz = 1.0;
        double magnitude = 1.0;

        var float64Signal = Float64ScalarHarmonicSignal.Finite(Float64ScalarRange.SymmetricOne, frequencyHz, magnitude);
        var genericSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.Zero
        );

        // Test at t = -1
        var t1 = -1.0;
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

        // Test at t = 1
        var t3 = 1.0;
        var float64Value3 = float64Signal.GetValue(t3);
        var genericValue3 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t3)).ScalarValue;
        Assert.That(Math.Abs(float64Value3 - genericValue3), Is.LessThan(Tolerance),
            $"Mismatch at t={t3}: Float64={float64Value3}, Generic={genericValue3}");

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteHarmonic_Derivative1()
    {
        double frequencyHz = 1.0;
        double magnitude = 1.0;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarHarmonicSignal.Finite(Float64ScalarRange.SymmetricOne, frequencyHz, magnitude, timeOffset);
        var genericSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

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
    public void TestFiniteHarmonic_Derivative1WithHigherFrequency()
    {
        double frequencyHz = 2.0;
        double magnitude = 1.5;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarHarmonicSignal.Finite(Float64ScalarRange.SymmetricOne, frequencyHz, magnitude, timeOffset);
        var genericSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        // Test first derivative at various points
        for (double t = -1.0; t <= 1.0; t += 0.25)
        {
            var float64Deriv = float64Signal.GetDerivative1Value(t);
            var genericDeriv = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteHarmonic_Derivative2()
    {
        double frequencyHz = 1.0;
        double magnitude = 1.0;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarHarmonicSignal.Finite(Float64ScalarRange.SymmetricOne, frequencyHz, magnitude, timeOffset);
        var genericSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

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
    public void TestPeriodicHarmonic()
    {
        double frequencyHz = 1.0;
        double magnitude = 1.0;
        double timeOffset = 0.0;

        var float64Signal = Float64ScalarHarmonicSignal.Periodic(Float64ScalarRange.SymmetricOne, frequencyHz, magnitude, timeOffset);
        var genericSignal = HarmonicScalarSignal<double>.Periodic(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

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
        double frequencyHz = 1.0;
        double magnitude = 1.0;

        var finiteSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.Zero
        );
        var periodicSignal = finiteSignal.ToPeriodicSignal() as HarmonicScalarSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);
        Assert.That(periodicSignal.FrequencyHz.ScalarValue, Is.EqualTo(frequencyHz));
        Assert.That(periodicSignal.Magnitude.ScalarValue, Is.EqualTo(magnitude));

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicToFiniteConversion()
    {
        double frequencyHz = 2.0;
        double magnitude = 1.5;

        var periodicSignal = HarmonicScalarSignal<double>.Periodic(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.Zero
        );
        var finiteSignal = periodicSignal.ToFiniteSignal() as HarmonicScalarSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);
        Assert.That(finiteSignal.FrequencyHz.ScalarValue, Is.EqualTo(frequencyHz));
        Assert.That(finiteSignal.Magnitude.ScalarValue, Is.EqualTo(magnitude));

        Debug.Assert(true);
    }

    [Test]
    public void TestIsValid()
    {
        var finiteSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );
        var periodicSignal = HarmonicScalarSignal<double>.Periodic(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        Assert.That(finiteSignal.IsValid(), Is.True);
        Assert.That(periodicSignal.IsValid(), Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestTimeRangeProperties()
    {
        var signal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestPropertiesAccess()
    {
        double frequencyHz = 3.0;
        double magnitude = 2.5;
        double timeOffset = 0.5;

        var signal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        Assert.That(signal.FrequencyHz.ScalarValue, Is.EqualTo(frequencyHz));
        Assert.That(signal.Magnitude.ScalarValue, Is.EqualTo(magnitude));
        Assert.That(signal.TimeOffset.ScalarValue, Is.EqualTo(timeOffset));

        // Test Frequency property (should be 2π * FrequencyHz)
        var expectedFrequency = Math.Tau * frequencyHz;
        Assert.That(signal.Frequency.ScalarValue, Is.EqualTo(expectedFrequency).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestHarmonic_FrequencyRelationship()
    {
        // Test that Frequency = 2π * FrequencyHz
        double[] frequencyHzValues = { 0.5, 1.0, 2.0, 5.0 };

        foreach (var freqHz in frequencyHzValues)
        {
            var signal = HarmonicScalarSignal<double>.Finite(
                ScalarProcessor,
                ScalarProcessor.ScalarFromNumber(freqHz),
                ScalarProcessor.One,
                ScalarProcessor.Zero
            );

            var expectedFrequency = Math.Tau * freqHz; // Math.Tau = 2π
            Assert.That(signal.Frequency.ScalarValue, Is.EqualTo(expectedFrequency).Within(Tolerance),
                $"Frequency relationship broken for FrequencyHz={freqHz}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestHarmonic_PhaseShiftEffect()
    {
        double frequencyHz = 1.0;
        double magnitude = 1.0;
        double timeOffset1 = 0.0;
        double timeOffset2 = 0.25; // Quarter period offset

        var signal1 = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset1)
        );
        var signal2 = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(frequencyHz),
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset2)
        );

        // signal2 should be signal1 shifted in time by timeOffset2
        for (double t = -0.5; t <= 0.5; t += 0.1)
        {
            var value1 = signal1.GetValue(ScalarProcessor.ScalarFromNumber(t + timeOffset2)).ScalarValue;
            var value2 = signal2.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(value1 - value2), Is.LessThan(Tolerance),
                $"Phase shift effect broken at t={t}");
        }

        Debug.Assert(true);
    }
}
