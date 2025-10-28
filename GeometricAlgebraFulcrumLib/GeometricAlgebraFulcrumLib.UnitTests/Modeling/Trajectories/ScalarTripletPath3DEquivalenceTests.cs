using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class ScalarTripletPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void ScalarTripletPath3D_WithConstantSignals_GetValue_ShouldReturnCorrectValues()
    {
        // Create constant signals for X, Y, Z
        var xSignal = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var ySignal = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.0)
        );
        var zSignal = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(3.0)
        );
        var path = ScalarTripletPath3D<double>.Finite(xSignal, ySignal, zSignal);

        // Test at several time points
        var testPoints = new[] { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (var t in testPoints)
        {
            var value = path.GetValue(ScalarProcessor.ScalarFromNumber(t));

            // Should be constant (1, 2, 3)
            Assert.That(value.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
                $"X should be 1.0 at t={t}");
            Assert.That(value.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance),
                $"Y should be 2.0 at t={t}");
            Assert.That(value.Z.ScalarValue, Is.EqualTo(3.0).Within(Tolerance),
                $"Z should be 3.0 at t={t}");
        }
    }

    [Test]
    public void ScalarTripletPath3D_WithLinearSignals_GetValue_ShouldReturnCorrectValues()
    {
        // Create linear signals: X(t) = t, Y(t) = 2*t, Z(t) = 3*t
        Func<Scalar<double>, Scalar<double>> xFunc = t => t;
        Func<Scalar<double>, Scalar<double>> yFunc = t => ScalarProcessor.ScalarFromNumber(2.0) * t;
        Func<Scalar<double>, Scalar<double>> zFunc = t => ScalarProcessor.ScalarFromNumber(3.0) * t;

        var xSignal = ComputedScalarSignal<double>.Finite(ScalarProcessor, xFunc);
        var ySignal = ComputedScalarSignal<double>.Finite(ScalarProcessor, yFunc);
        var zSignal = ComputedScalarSignal<double>.Finite(ScalarProcessor, zFunc);
        var path = ScalarTripletPath3D<double>.Finite(xSignal, ySignal, zSignal);

        // Test at several time points
        var testPoints = new[] { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (var t in testPoints)
        {
            var value = path.GetValue(ScalarProcessor.ScalarFromNumber(t));

            // Check expected values: X(t) = t, Y(t) = 2*t, Z(t) = 3*t
            Assert.That(value.X.ScalarValue, Is.EqualTo(t).Within(Tolerance),
                $"X should be {t} at t={t}");
            Assert.That(value.Y.ScalarValue, Is.EqualTo(2.0 * t).Within(Tolerance),
                $"Y should be {2.0 * t} at t={t}");
            Assert.That(value.Z.ScalarValue, Is.EqualTo(3.0 * t).Within(Tolerance),
                $"Z should be {3.0 * t} at t={t}");
        }
    }

    [Test]
    public void ScalarTripletPath3D_GetDerivatives_ShouldReturnCorrectValues()
    {
        // Create signals with known derivatives
        // X(t) = t^2, X'(t) = 2*t, X''(t) = 2
        // Y(t) = t^3, Y'(t) = 3*t^2, Y''(t) = 6*t
        // Z(t) = t, Z'(t) = 1, Z''(t) = 0

        Func<Scalar<double>, Scalar<double>> xFunc = t => t * t;
        Func<Scalar<double>, Scalar<double>> xDeriv1 = t => ScalarProcessor.ScalarFromNumber(2.0) * t;
        Func<Scalar<double>, Scalar<double>> xDeriv2 = t => ScalarProcessor.ScalarFromNumber(2.0);

        Func<Scalar<double>, Scalar<double>> yFunc = t => t * t * t;
        Func<Scalar<double>, Scalar<double>> yDeriv1 = t => ScalarProcessor.ScalarFromNumber(3.0) * t * t;
        Func<Scalar<double>, Scalar<double>> yDeriv2 = t => ScalarProcessor.ScalarFromNumber(6.0) * t;

        Func<Scalar<double>, Scalar<double>> zFunc = t => t;
        Func<Scalar<double>, Scalar<double>> zDeriv1 = t => ScalarProcessor.One;
        Func<Scalar<double>, Scalar<double>> zDeriv2 = t => ScalarProcessor.Zero;

        var xSignal = ComputedScalarSignal<double>.Finite(ScalarProcessor, xFunc, xDeriv1, xDeriv2);
        var ySignal = ComputedScalarSignal<double>.Finite(ScalarProcessor, yFunc, yDeriv1, yDeriv2);
        var zSignal = ComputedScalarSignal<double>.Finite(ScalarProcessor, zFunc, zDeriv1, zDeriv2);
        var path = ScalarTripletPath3D<double>.Finite(xSignal, ySignal, zSignal);

        // Test derivatives at several points
        var testPoints = new[] { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (var t in testPoints)
        {
            var tScalar = ScalarProcessor.ScalarFromNumber(t);

            // First derivative
            var deriv1 = path.GetDerivative1Value(tScalar);
            Assert.That(deriv1.X.ScalarValue, Is.EqualTo(2.0 * t).Within(Tolerance),
                $"Deriv1 X should be {2.0 * t} at t={t}");
            Assert.That(deriv1.Y.ScalarValue, Is.EqualTo(3.0 * t * t).Within(Tolerance),
                $"Deriv1 Y should be {3.0 * t * t} at t={t}");
            Assert.That(deriv1.Z.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
                $"Deriv1 Z should be 1.0 at t={t}");

            // Second derivative
            var deriv2 = path.GetDerivative2Value(tScalar);
            Assert.That(deriv2.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance),
                $"Deriv2 X should be 2.0 at t={t}");
            Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(6.0 * t).Within(Tolerance),
                $"Deriv2 Y should be {6.0 * t} at t={t}");
            Assert.That(deriv2.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
                $"Deriv2 Z should be 0.0 at t={t}");
        }
    }

    [Test]
    public void ScalarTripletPath3D_GetScalarComponents_ShouldReturnOriginalSignals()
    {
        var xSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.ScalarFromNumber(1.0));
        var ySignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.ScalarFromNumber(2.0));
        var zSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.ScalarFromNumber(3.0));
        var path = ScalarTripletPath3D<double>.Finite(xSignal, ySignal, zSignal);

        var components = path.GetScalarComponents();

        Assert.That(components.Item1, Is.SameAs(xSignal));
        Assert.That(components.Item2, Is.SameAs(ySignal));
        Assert.That(components.Item3, Is.SameAs(zSignal));
    }

    [Test]
    public void ScalarTripletPath3D_ToFinitePath_WhenFinite_ShouldReturnSelf()
    {
        var xSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var ySignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var zSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var path = ScalarTripletPath3D<double>.Finite(xSignal, ySignal, zSignal);

        var finitePath = path.ToFinitePath();

        Assert.That(finitePath, Is.SameAs(path));
        Assert.That(path.IsFinite, Is.True);
        Assert.That(path.IsPeriodic, Is.False);
    }

    [Test]
    public void ScalarTripletPath3D_ToPeriodicPath_WhenPeriodic_ShouldReturnSelf()
    {
        var xSignal = ConstantScalarSignal<double>.Periodic(ScalarProcessor, ScalarProcessor.One);
        var ySignal = ConstantScalarSignal<double>.Periodic(ScalarProcessor, ScalarProcessor.One);
        var zSignal = ConstantScalarSignal<double>.Periodic(ScalarProcessor, ScalarProcessor.One);
        var path = ScalarTripletPath3D<double>.Periodic(xSignal, ySignal, zSignal);

        var periodicPath = path.ToPeriodicPath();

        Assert.That(periodicPath, Is.SameAs(path));
        Assert.That(path.IsPeriodic, Is.True);
        Assert.That(path.IsFinite, Is.False);
    }

    [Test]
    public void ScalarTripletPath3D_ToPeriodicPath_WhenFinite_ShouldReturnNewInstance()
    {
        var xSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var ySignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var zSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var path = ScalarTripletPath3D<double>.Finite(xSignal, ySignal, zSignal);

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
    public void ScalarTripletPath3D_ToFinitePath_WhenPeriodic_ShouldReturnNewInstance()
    {
        var xSignal = ConstantScalarSignal<double>.Periodic(ScalarProcessor, ScalarProcessor.One);
        var ySignal = ConstantScalarSignal<double>.Periodic(ScalarProcessor, ScalarProcessor.One);
        var zSignal = ConstantScalarSignal<double>.Periodic(ScalarProcessor, ScalarProcessor.One);
        var path = ScalarTripletPath3D<double>.Periodic(xSignal, ySignal, zSignal);

        var finitePath = path.ToFinitePath();

        Assert.That(finitePath, Is.Not.SameAs(path));
        Assert.That(finitePath.IsFinite, Is.True);
        Assert.That(finitePath.IsPeriodic, Is.False);
    }

    [Test]
    public void ScalarTripletPath3D_IsValid_ShouldReturnTrue()
    {
        var xSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var ySignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var zSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var path = ScalarTripletPath3D<double>.Finite(xSignal, ySignal, zSignal);

        Assert.That(path.IsValid(), Is.True);
    }

    [Test]
    public void ScalarTripletPath3D_FactoryMethods_ShouldAllWork()
    {
        var xSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var ySignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var zSignal = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var timeRange = ScalarRange<double>.SymmetricOne(ScalarProcessor);

        // Test Finite (2 variants)
        var path1 = ScalarTripletPath3D<double>.Finite(xSignal, ySignal, zSignal);
        Assert.That(path1.IsFinite, Is.True);

        var path2 = ScalarTripletPath3D<double>.Finite(timeRange, xSignal, ySignal, zSignal);
        Assert.That(path2.IsFinite, Is.True);

        // Test Periodic (2 variants)
        var path3 = ScalarTripletPath3D<double>.Periodic(xSignal, ySignal, zSignal);
        Assert.That(path3.IsPeriodic, Is.True);

        var path4 = ScalarTripletPath3D<double>.Periodic(timeRange, xSignal, ySignal, zSignal);
        Assert.That(path4.IsPeriodic, Is.True);

        // Test Create (2 variants)
        var path5 = ScalarTripletPath3D<double>.Create(false, xSignal, ySignal, zSignal);
        Assert.That(path5.IsFinite, Is.True);

        var path6 = ScalarTripletPath3D<double>.Create(timeRange, true, xSignal, ySignal, zSignal);
        Assert.That(path6.IsPeriodic, Is.True);
    }
}
