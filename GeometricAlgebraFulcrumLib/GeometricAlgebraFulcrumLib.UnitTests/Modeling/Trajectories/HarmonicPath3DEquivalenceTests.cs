using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class HarmonicPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void HarmonicScalarSignal_GetValue_ShouldReturnCorrectValues()
    {
        // Create a harmonic signal: f(t) = 2 * cos(2*pi * 1 * (t + 0))
        // Frequency = 1 Hz, Magnitude = 2, TimeOffset = 0
        var signal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,      // 1 Hz
            ScalarProcessor.Two,       // magnitude 2
            ScalarProcessor.Zero       // no time offset
        );

        // Test at several time points
        // At t=0: 2*cos(0) = 2
        var value0 = signal.GetValue(ScalarProcessor.Zero).ScalarValue;
        Assert.That(value0, Is.EqualTo(2.0).Within(Tolerance), "Value at t=0");

        // At t=0.25: 2*cos(2*pi*0.25) = 2*cos(pi/2) = 0
        var value025 = signal.GetValue(ScalarProcessor.ScalarFromNumber(0.25)).ScalarValue;
        Assert.That(value025, Is.EqualTo(0.0).Within(Tolerance), "Value at t=0.25");

        // At t=0.5: 2*cos(2*pi*0.5) = 2*cos(pi) = -2
        var value05 = signal.GetValue(ScalarProcessor.ScalarFromNumber(0.5)).ScalarValue;
        Assert.That(value05, Is.EqualTo(-2.0).Within(Tolerance), "Value at t=0.5");

        // At t=1.0: 2*cos(2*pi*1.0) = 2*cos(2*pi) = 2
        var value1 = signal.GetValue(ScalarProcessor.One).ScalarValue;
        Assert.That(value1, Is.EqualTo(2.0).Within(Tolerance), "Value at t=1.0");
    }

    [Test]
    public void HarmonicScalarSignal_WithTimeOffset_GetValue_ShouldReturnCorrectValues()
    {
        // Create a harmonic signal with time offset: f(t) = 1 * cos(2*pi * 1 * (t + 0.25))
        // This shifts the wave by 0.25 periods
        var signal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,                      // 1 Hz
            ScalarProcessor.One,                      // magnitude 1
            ScalarProcessor.ScalarFromNumber(0.25)   // time offset 0.25
        );

        // At t=0: cos(2*pi*0.25) = cos(pi/2) = 0
        var value0 = signal.GetValue(ScalarProcessor.Zero).ScalarValue;
        Assert.That(value0, Is.EqualTo(0.0).Within(Tolerance), "Value at t=0 with offset");

        // At t=0.25: cos(2*pi*0.5) = cos(pi) = -1
        var value025 = signal.GetValue(ScalarProcessor.ScalarFromNumber(0.25)).ScalarValue;
        Assert.That(value025, Is.EqualTo(-1.0).Within(Tolerance), "Value at t=0.25 with offset");
    }

    [Test]
    public void HarmonicScalarSignal_GetDerivative1_ShouldReturnCorrectValues()
    {
        // f(t) = Magnitude * cos(w * (t + offset))
        // f'(t) = -Magnitude * w * sin(w * (t + offset))
        // For w = 2*pi, Magnitude = 1, offset = 0:
        // f'(t) = -2*pi * sin(2*pi * t)

        var signal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,   // 1 Hz -> w = 2*pi
            ScalarProcessor.One,   // magnitude 1
            ScalarProcessor.Zero   // no offset
        );

        // At t=0: -2*pi * sin(0) = 0
        var deriv0 = signal.GetDerivative1Value(ScalarProcessor.Zero).ScalarValue;
        Assert.That(deriv0, Is.EqualTo(0.0).Within(Tolerance), "Derivative at t=0");

        // At t=0.25: -2*pi * sin(pi/2) = -2*pi
        var deriv025 = signal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.25)).ScalarValue;
        Assert.That(deriv025, Is.EqualTo(-2.0 * Math.PI).Within(Tolerance), "Derivative at t=0.25");

        // At t=0.5: -2*pi * sin(pi) = 0
        var deriv05 = signal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5)).ScalarValue;
        Assert.That(deriv05, Is.EqualTo(0.0).Within(Tolerance), "Derivative at t=0.5");
    }

    [Test]
    public void HarmonicScalarSignal_GetDerivative2_ShouldReturnCorrectValues()
    {
        // f''(t) = -Magnitude * w^2 * cos(w * (t + offset))
        // For w = 2*pi, Magnitude = 1, offset = 0:
        // f''(t) = -4*pi^2 * cos(2*pi * t)

        var signal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,   // 1 Hz
            ScalarProcessor.One,   // magnitude 1
            ScalarProcessor.Zero   // no offset
        );

        var w = 2.0 * Math.PI;

        // At t=0: -4*pi^2 * cos(0) = -4*pi^2
        var deriv20 = signal.GetDerivative2Value(ScalarProcessor.Zero).ScalarValue;
        Assert.That(deriv20, Is.EqualTo(-w * w).Within(Tolerance), "Second derivative at t=0");

        // At t=0.25: -4*pi^2 * cos(pi/2) = 0
        var deriv2025 = signal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(0.25)).ScalarValue;
        Assert.That(deriv2025, Is.EqualTo(0.0).Within(Tolerance), "Second derivative at t=0.25");

        // At t=0.5: -4*pi^2 * cos(pi) = 4*pi^2
        var deriv205 = signal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(0.5)).ScalarValue;
        Assert.That(deriv205, Is.EqualTo(w * w).Within(Tolerance), "Second derivative at t=0.5");
    }

    [Test]
    public void HarmonicScalarSignal_ToPeriodicSignal_ShouldReturnPeriodicVersion()
    {
        var signal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        Assert.That(signal.IsFinite, Is.True);
        Assert.That(signal.IsPeriodic, Is.False);

        var periodicSignal = (HarmonicScalarSignal<double>)signal.ToPeriodicSignal();

        Assert.That(periodicSignal.IsFinite, Is.False);
        Assert.That(periodicSignal.IsPeriodic, Is.True);

        // Values should still match
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var value1 = signal.GetValue(t).ScalarValue;
        var value2 = periodicSignal.GetValue(t).ScalarValue;
        Assert.That(value2, Is.EqualTo(value1).Within(Tolerance));
    }

    [Test]
    public void HarmonicScalarSignal_ToFiniteSignal_WhenFinite_ShouldReturnSelf()
    {
        var signal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var finiteSignal = signal.ToFiniteSignal();

        Assert.That(finiteSignal, Is.SameAs(signal));
    }

    [Test]
    public void HarmonicPath3D_GetValue_ShouldReturnCorrectValues()
    {
        // Create a circular path in XY plane
        // X(t) = cos(2*pi * t)
        // Y(t) = sin(2*pi * t) = -cos(2*pi * (t + 0.25))
        // Z(t) = 0

        var xSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,   // 1 Hz
            ScalarProcessor.One,   // magnitude 1
            ScalarProcessor.Zero   // no offset
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,                      // 1 Hz
            ScalarProcessor.MinusOne,                 // magnitude -1 (to get sin from cos)
            ScalarProcessor.ScalarFromNumber(0.25)   // offset by 0.25 period
        );

        var zSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.Zero,  // zero magnitude
            ScalarProcessor.Zero
        );

        var path = HarmonicPath3D<double>.Create(xSignal, ySignal, zSignal);

        // At t=0: (1, 0, 0)
        var value0 = path.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0");
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0");
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z at t=0");

        // At t=0.25: (0, 1, 0)
        var value025 = path.GetValue(ScalarProcessor.ScalarFromNumber(0.25));
        Assert.That(value025.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.25");
        Assert.That(value025.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=0.25");
        Assert.That(value025.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z at t=0.25");

        // At t=0.5: (-1, 0, 0)
        var value05 = path.GetValue(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(value05.X.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "X at t=0.5");
        Assert.That(value05.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0.5");
        Assert.That(value05.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z at t=0.5");
    }

    [Test]
    public void HarmonicPath3D_GetDerivatives_ShouldReturnCorrectValues()
    {
        var xSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.Zero,
            ScalarProcessor.Zero
        );

        var zSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.Zero,
            ScalarProcessor.Zero
        );

        var path = HarmonicPath3D<double>.Create(xSignal, ySignal, zSignal);

        var t = ScalarProcessor.ScalarFromNumber(0.25);

        // Test first derivative
        var deriv1 = path.GetDerivative1Value(t);
        var expectedDeriv1 = xSignal.GetDerivative1Value(t).ScalarValue;
        Assert.That(deriv1.X.ScalarValue, Is.EqualTo(expectedDeriv1).Within(Tolerance));

        // Test second derivative
        var deriv2 = path.GetDerivative2Value(t);
        var expectedDeriv2 = xSignal.GetDerivative2Value(t).ScalarValue;
        Assert.That(deriv2.X.ScalarValue, Is.EqualTo(expectedDeriv2).Within(Tolerance));
    }

    [Test]
    public void HarmonicPath3D_ToFinitePath_WhenFinite_ShouldReturnSelf()
    {
        var xSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var zSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var path = HarmonicPath3D<double>.Create(xSignal, ySignal, zSignal);

        var finitePath = path.ToFinitePath();

        Assert.That(finitePath, Is.SameAs(path));
        Assert.That(path.IsFinite, Is.True);
        Assert.That(path.IsPeriodic, Is.False);
    }

    [Test]
    public void HarmonicPath3D_ToPeriodicPath_WhenFinite_ShouldReturnNewInstance()
    {
        var xSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var zSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var path = HarmonicPath3D<double>.Create(xSignal, ySignal, zSignal);

        var periodicPath = path.ToPeriodicPath();

        Assert.That(periodicPath, Is.Not.SameAs(path));
        Assert.That(periodicPath.IsPeriodic, Is.True);
        Assert.That(periodicPath.IsFinite, Is.False);

        // Values should still match
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var value1 = path.GetValue(t);
        var value2 = periodicPath.GetValue(t);

        Assert.That(value2.X.ScalarValue, Is.EqualTo(value1.X.ScalarValue).Within(Tolerance));
        Assert.That(value2.Y.ScalarValue, Is.EqualTo(value1.Y.ScalarValue).Within(Tolerance));
        Assert.That(value2.Z.ScalarValue, Is.EqualTo(value1.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void HarmonicPath3D_IsValid_ShouldReturnTrue()
    {
        var xSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var zSignal = HarmonicScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var path = HarmonicPath3D<double>.Create(xSignal, ySignal, zSignal);

        Assert.That(path.IsValid(), Is.True);
    }
}
