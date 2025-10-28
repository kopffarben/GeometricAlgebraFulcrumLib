using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories.Scalars;

[TestFixture]
public class SimpleHarmonicScalarSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void SimpleHarmonicSignal_Finite_GetValue_AtZero_HarmonicFactor1_ShouldMatchFloat64()
    {
        var harmonicFactor = 1;
        var magnitude = 2.0;
        var timeOffset = 0.0;

        var signalFloat64 = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        var valueFloat64 = signalFloat64.GetValue(0.0);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.Zero);

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"Value at t=0 should match. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void SimpleHarmonicSignal_Finite_GetValue_AtPiOverTwo_HarmonicFactor2_ShouldMatchFloat64()
    {
        var harmonicFactor = 2;
        var magnitude = 1.5;
        var timeOffset = 0.0;

        var signalFloat64 = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        var tValue = Math.PI / 2.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"Value at t=π/2 should match. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void SimpleHarmonicSignal_Finite_GetValue_WithTimeOffset_ShouldMatchFloat64()
    {
        var harmonicFactor = 1;
        var magnitude = 1.0;
        var timeOffset = 0.5;

        var signalFloat64 = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        var tValue = 1.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"Value with timeOffset should match. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void SimpleHarmonicSignal_Periodic_GetValue_AtZero_ShouldMatchFloat64()
    {
        var harmonicFactor = 3;
        var magnitude = 2.5;

        var signalFloat64 = Float64ScalarSimpleHarmonicSignal.Periodic(harmonicFactor, magnitude);
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Periodic(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude)
        );

        var valueFloat64 = signalFloat64.GetValue(0.0);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.Zero);

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance));
    }

    [Test]
    public void SimpleHarmonicSignal_GetDerivative1Value_AtZero_ShouldMatchFloat64()
    {
        var harmonicFactor = 1;
        var magnitude = 2.0;
        var timeOffset = 0.0;

        var signalFloat64 = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        var deriv1Float64 = signalFloat64.GetDerivative1Value(0.0);
        var deriv1Generic = signalGeneric.GetDerivative1Value(ScalarProcessor.Zero);

        Assert.That(deriv1Generic.ScalarValue, Is.EqualTo(deriv1Float64).Within(Tolerance),
            $"Derivative1 at t=0 should match. Float64: {deriv1Float64}, Generic: {deriv1Generic.ScalarValue}");
    }

    [Test]
    public void SimpleHarmonicSignal_GetDerivative1Value_AtPiOverTwo_HarmonicFactor2_ShouldMatchFloat64()
    {
        var harmonicFactor = 2;
        var magnitude = 1.5;
        var timeOffset = 0.0;

        var signalFloat64 = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        var tValue = Math.PI / 2.0;
        var deriv1Float64 = signalFloat64.GetDerivative1Value(tValue);
        var deriv1Generic = signalGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(deriv1Generic.ScalarValue, Is.EqualTo(deriv1Float64).Within(Tolerance),
            $"Derivative1 at t=π/2 should match. Float64: {deriv1Float64}, Generic: {deriv1Generic.ScalarValue}");
    }

    [Test]
    public void SimpleHarmonicSignal_GetDerivative2Value_AtZero_ShouldMatchFloat64()
    {
        var harmonicFactor = 1;
        var magnitude = 2.0;
        var timeOffset = 0.0;

        var signalFloat64 = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        var deriv2Float64 = signalFloat64.GetDerivative2Value(0.0);
        var deriv2Generic = signalGeneric.GetDerivative2Value(ScalarProcessor.Zero);

        Assert.That(deriv2Generic.ScalarValue, Is.EqualTo(deriv2Float64).Within(Tolerance),
            $"Derivative2 at t=0 should match. Float64: {deriv2Float64}, Generic: {deriv2Generic.ScalarValue}");
    }

    [Test]
    public void SimpleHarmonicSignal_GetDerivative2Value_AtPi_HarmonicFactor3_ShouldMatchFloat64()
    {
        var harmonicFactor = 3;
        var magnitude = 1.0;
        var timeOffset = 0.0;

        var signalFloat64 = Float64ScalarSimpleHarmonicSignal.Finite(harmonicFactor, magnitude, timeOffset);
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        var tValue = Math.PI;
        var deriv2Float64 = signalFloat64.GetDerivative2Value(tValue);
        var deriv2Generic = signalGeneric.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(deriv2Generic.ScalarValue, Is.EqualTo(deriv2Float64).Within(Tolerance),
            $"Derivative2 at t=π should match. Float64: {deriv2Float64}, Generic: {deriv2Generic.ScalarValue}");
    }

    [Test]
    public void SimpleHarmonicSignal_ToFiniteSignal_ShouldReturnFiniteInstance()
    {
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Periodic(
            ScalarProcessor,
            2,
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var finiteSignal = signalGeneric.ToFiniteSignal();

        Assert.That(finiteSignal.IsFinite, Is.True);
        Assert.That(finiteSignal.IsPeriodic, Is.False);
    }

    [Test]
    public void SimpleHarmonicSignal_ToPeriodicSignal_ShouldReturnPeriodicInstance()
    {
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            2,
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var periodicSignal = signalGeneric.ToPeriodicSignal();

        Assert.That(periodicSignal.IsFinite, Is.False);
        Assert.That(periodicSignal.IsPeriodic, Is.True);
    }

    [Test]
    public void SimpleHarmonicSignal_IsValid_ShouldReturnTrue()
    {
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            1,
            ScalarProcessor.ScalarFromNumber(2.0),
            ScalarProcessor.ScalarFromNumber(0.5)
        );

        Assert.That(signalGeneric.IsValid(), Is.True);
    }

    [Test]
    public void SimpleHarmonicSignal_TimeRange_ShouldBeSymmetricPi()
    {
        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            1,
            ScalarProcessor.ScalarFromNumber(1.0)
        );

        var expectedMin = -Math.PI;
        var expectedMax = Math.PI;

        Assert.That(signalGeneric.MinTime.ScalarValue, Is.EqualTo(expectedMin).Within(Tolerance),
            $"MinTime should be -π. Got: {signalGeneric.MinTime.ScalarValue}");
        Assert.That(signalGeneric.MaxTime.ScalarValue, Is.EqualTo(expectedMax).Within(Tolerance),
            $"MaxTime should be π. Got: {signalGeneric.MaxTime.ScalarValue}");
    }

    [Test]
    public void SimpleHarmonicSignal_Properties_ShouldMatchConstructorValues()
    {
        var harmonicFactor = 5;
        var magnitude = 3.0;
        var timeOffset = 0.25;

        var signalGeneric = SimpleHarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            harmonicFactor,
            ScalarProcessor.ScalarFromNumber(magnitude),
            ScalarProcessor.ScalarFromNumber(timeOffset)
        );

        Assert.That(signalGeneric.HarmonicFactor, Is.EqualTo(harmonicFactor));
        Assert.That(signalGeneric.Magnitude.ScalarValue, Is.EqualTo(magnitude).Within(Tolerance));
        Assert.That(signalGeneric.TimeOffset.ScalarValue, Is.EqualTo(timeOffset).Within(Tolerance));
    }
}
