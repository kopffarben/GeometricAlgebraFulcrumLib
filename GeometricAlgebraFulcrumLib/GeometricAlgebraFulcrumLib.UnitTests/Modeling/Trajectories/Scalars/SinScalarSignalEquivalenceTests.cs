using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories.Scalars;

[TestFixture]
public class SinScalarSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void SinSignal_Finite_GetValue_AtZero_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Finite();
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var valueFloat64 = signalFloat64.GetValue(0.0);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.Zero);

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"sin(0) should be 0. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void SinSignal_Finite_GetValue_AtPiOverTwo_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Finite();
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var tValue = Math.PI / 2.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"sin(π/2) should be 1. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void SinSignal_Finite_GetValue_AtPi_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Finite();
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var tValue = Math.PI;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"sin(π) should be ~0. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void SinSignal_Finite_GetValue_AtMinusPiOverTwo_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Finite();
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var tValue = -Math.PI / 2.0;
        var valueFloat64 = signalFloat64.GetValue(tValue);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance),
            $"sin(-π/2) should be -1. Float64: {valueFloat64}, Generic: {valueGeneric.ScalarValue}");
    }

    [Test]
    public void SinSignal_Periodic_GetValue_AtZero_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Periodic();
        var signalGeneric = SinScalarSignal<double>.Periodic(ScalarProcessor);

        var valueFloat64 = signalFloat64.GetValue(0.0);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.Zero);

        Assert.That(valueGeneric.ScalarValue, Is.EqualTo(valueFloat64).Within(Tolerance));
    }

    [Test]
    public void SinSignal_GetDerivative1Value_AtZero_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Finite();
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var deriv1Float64 = signalFloat64.GetDerivative1Value(0.0);
        var deriv1Generic = signalGeneric.GetDerivative1Value(ScalarProcessor.Zero);

        Assert.That(deriv1Generic.ScalarValue, Is.EqualTo(deriv1Float64).Within(Tolerance),
            $"sin'(0) = cos(0) should be 1. Float64: {deriv1Float64}, Generic: {deriv1Generic.ScalarValue}");
    }

    [Test]
    public void SinSignal_GetDerivative1Value_AtPiOverTwo_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Finite();
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var tValue = Math.PI / 2.0;
        var deriv1Float64 = signalFloat64.GetDerivative1Value(tValue);
        var deriv1Generic = signalGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(deriv1Generic.ScalarValue, Is.EqualTo(deriv1Float64).Within(Tolerance),
            $"sin'(π/2) = cos(π/2) should be ~0. Float64: {deriv1Float64}, Generic: {deriv1Generic.ScalarValue}");
    }

    [Test]
    public void SinSignal_GetDerivative1Value_AtPi_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Finite();
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var tValue = Math.PI;
        var deriv1Float64 = signalFloat64.GetDerivative1Value(tValue);
        var deriv1Generic = signalGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(deriv1Generic.ScalarValue, Is.EqualTo(deriv1Float64).Within(Tolerance),
            $"sin'(π) = cos(π) should be -1. Float64: {deriv1Float64}, Generic: {deriv1Generic.ScalarValue}");
    }

    [Test]
    public void SinSignal_GetDerivative2Value_AtZero_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Finite();
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var deriv2Float64 = signalFloat64.GetDerivative2Value(0.0);
        var deriv2Generic = signalGeneric.GetDerivative2Value(ScalarProcessor.Zero);

        Assert.That(deriv2Generic.ScalarValue, Is.EqualTo(deriv2Float64).Within(Tolerance),
            $"sin''(0) = -sin(0) should be 0. Float64: {deriv2Float64}, Generic: {deriv2Generic.ScalarValue}");
    }

    [Test]
    public void SinSignal_GetDerivative2Value_AtPiOverTwo_ShouldMatchFloat64()
    {
        var signalFloat64 = Float64ScalarSinSignal.Finite();
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var tValue = Math.PI / 2.0;
        var deriv2Float64 = signalFloat64.GetDerivative2Value(tValue);
        var deriv2Generic = signalGeneric.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(tValue));

        Assert.That(deriv2Generic.ScalarValue, Is.EqualTo(deriv2Float64).Within(Tolerance),
            $"sin''(π/2) = -sin(π/2) should be -1. Float64: {deriv2Float64}, Generic: {deriv2Generic.ScalarValue}");
    }

    [Test]
    public void SinSignal_ToFiniteSignal_ShouldReturnFiniteInstance()
    {
        var signalGeneric = SinScalarSignal<double>.Periodic(ScalarProcessor);
        var finiteSignal = signalGeneric.ToFiniteSignal();

        Assert.That(finiteSignal.IsFinite, Is.True);
        Assert.That(finiteSignal.IsPeriodic, Is.False);
    }

    [Test]
    public void SinSignal_ToPeriodicSignal_ShouldReturnPeriodicInstance()
    {
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = signalGeneric.ToPeriodicSignal();

        Assert.That(periodicSignal.IsFinite, Is.False);
        Assert.That(periodicSignal.IsPeriodic, Is.True);
    }

    [Test]
    public void SinSignal_IsValid_ShouldReturnTrue()
    {
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        Assert.That(signalGeneric.IsValid(), Is.True);
    }

    [Test]
    public void SinSignal_TimeRange_ShouldBeSymmetricPi()
    {
        var signalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);

        var expectedMin = -Math.PI;
        var expectedMax = Math.PI;

        Assert.That(signalGeneric.MinTime.ScalarValue, Is.EqualTo(expectedMin).Within(Tolerance),
            $"MinTime should be -π. Got: {signalGeneric.MinTime.ScalarValue}");
        Assert.That(signalGeneric.MaxTime.ScalarValue, Is.EqualTo(expectedMax).Within(Tolerance),
            $"MaxTime should be π. Got: {signalGeneric.MaxTime.ScalarValue}");
    }
}
