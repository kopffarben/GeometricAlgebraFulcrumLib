using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Normalized;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories.Scalars;

[TestFixture]
public class ScalarRampSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void ScalarRampSignal_Finite_GetValue_AtMinusOne_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        var tValue = -1.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"Value at t=-1 should match. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void ScalarRampSignal_Finite_GetValue_AtZero_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        var tValue = 0.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"Value at t=0 should match. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void ScalarRampSignal_Finite_GetValue_AtPlusOne_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        var tValue = 1.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"Value at t=1 should match. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void ScalarRampSignal_Finite_GetValue_BelowRange_ShouldClampToMinusOne()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        var tValue = -2.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"Value at t=-2 should clamp to -1. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance),
            "Value below range should be clamped to -1");
    }

    [Test]
    public void ScalarRampSignal_Finite_GetValue_AboveRange_ShouldClampToPlusOne()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        var tValue = 2.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"Value at t=2 should clamp to 1. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Value above range should be clamped to 1");
    }

    [Test]
    public void ScalarRampSignal_Finite_GetDerivative1Value_InsideRange_ShouldBeOne()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        var tValue = 0.0;
        var derivativeFloat64 = signalFloat64.GetDerivative1Value(tValue);
        var derivativeGeneric = signalGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(derivativeGeneric.ScalarValue, Is.EqualTo(derivativeFloat64).Within(Tolerance),
            $"Derivative1 at t=0 should match. Float64: {derivativeFloat64}, Generic: {derivativeGeneric.ScalarValue}");
        Assert.That(derivativeGeneric.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Derivative inside range should be 1");
    }

    [Test]
    public void ScalarRampSignal_Finite_GetDerivative1Value_OutsideRange_ShouldBeZero()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        var tValue = 2.0;
        var derivativeFloat64 = signalFloat64.GetDerivative1Value(tValue);
        var derivativeGeneric = signalGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(derivativeGeneric.ScalarValue, Is.EqualTo(derivativeFloat64).Within(Tolerance),
            $"Derivative1 at t=2 should match. Float64: {derivativeFloat64}, Generic: {derivativeGeneric.ScalarValue}");
        Assert.That(derivativeGeneric.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Derivative outside range should be 0");
    }

    [Test]
    public void ScalarRampSignal_Finite_GetDerivative2Value_Always_ShouldBeZero()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        var tValue = 0.5;
        var derivativeFloat64 = signalFloat64.GetDerivative2Value(tValue);
        var derivativeGeneric = signalGeneric.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(derivativeGeneric.ScalarValue, Is.EqualTo(derivativeFloat64).Within(Tolerance),
            $"Derivative2 should match. Float64: {derivativeFloat64}, Generic: {derivativeGeneric.ScalarValue}");
        Assert.That(derivativeGeneric.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Second derivative should always be 0 (constant slope)");
    }

    [Test]
    public void ScalarRampSignal_Periodic_GetValue_AtZero_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarRampSignal.PeriodicInstance;
        var signalGeneric = ScalarRampSignal<double>.Periodic(ScalarProcessor);

        var tValue = 0.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"Periodic value at t=0 should match. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void ScalarRampSignal_Periodic_GetDerivative1Value_Always_ShouldBeOne()
    {
        var signalFloat64 = Float64ScalarRampSignal.PeriodicInstance;
        var signalGeneric = ScalarRampSignal<double>.Periodic(ScalarProcessor);

        var tValue = 5.0; // Outside original range
        var derivativeFloat64 = signalFloat64.GetDerivative1Value(tValue);
        var derivativeGeneric = signalGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(derivativeGeneric.ScalarValue, Is.EqualTo(derivativeFloat64).Within(Tolerance),
            $"Periodic derivative1 should match. Float64: {derivativeFloat64}, Generic: {derivativeGeneric.ScalarValue}");
        Assert.That(derivativeGeneric.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Periodic derivative should always be 1");
    }

    [Test]
    public void ScalarRampSignal_IsValid_ShouldAlwaysBeTrue()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        Assert.That(signalGeneric.IsValid(), Is.EqualTo(signalFloat64.IsValid()),
            "IsValid should match Float64");
        Assert.That(signalGeneric.IsValid(), Is.True,
            "ScalarRampSignal should always be valid");
    }

    [Test]
    public void ScalarRampSignal_ToFiniteSignal_ShouldReturnFiniteInstance()
    {
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);
        var finiteSignal = signalGeneric.ToFiniteSignal();

        Assert.That(finiteSignal.IsPeriodic, Is.False,
            "ToFiniteSignal should return a finite signal");
    }

    [Test]
    public void ScalarRampSignal_ToPeriodicSignal_ShouldReturnPeriodicInstance()
    {
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = signalGeneric.ToPeriodicSignal();

        Assert.That(periodicSignal.IsPeriodic, Is.True,
            "ToPeriodicSignal should return a periodic signal");
    }

    [Test]
    public void ScalarRampSignal_TimeRange_ShouldBeSymmetricOne()
    {
        var signalFloat64 = Float64ScalarRampSignal.FiniteInstance;
        var signalGeneric = ScalarRampSignal<double>.Finite(ScalarProcessor);

        Assert.That(signalGeneric.TimeRange.MinValue.ScalarValue,
            Is.EqualTo(signalFloat64.TimeRange.MinValue).Within(Tolerance),
            "TimeRange.MinValue should match Float64");
        Assert.That(signalGeneric.TimeRange.MaxValue.ScalarValue,
            Is.EqualTo(signalFloat64.TimeRange.MaxValue).Within(Tolerance),
            "TimeRange.MaxValue should match Float64");
        Assert.That(signalGeneric.TimeRange.MinValue.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance),
            "TimeRange should start at -1");
        Assert.That(signalGeneric.TimeRange.MaxValue.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "TimeRange should end at 1");
    }
}
