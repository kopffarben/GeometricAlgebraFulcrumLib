using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using Float64Signal = GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Float64ScalarSignal;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public class ScalarSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region ConstantScalarSignal Tests

    [Test]
    public void ConstantScalarSignal_Zero_Finite_GetValue_ShouldMatch()
    {
        var signalFloat64 = Float64Signal.FiniteZero();
        var signalGeneric = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        // Test at several time points
        var t1Float64 = signalFloat64.GetValue(0.0);
        var t1Generic = signalGeneric.GetValue(ScalarProcessor.Zero).ScalarValue;
        Assert.That(t1Generic, Is.EqualTo(t1Float64).Within(Tolerance));

        var t2Float64 = signalFloat64.GetValue(0.5);
        var t2Generic = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.5)).ScalarValue;
        Assert.That(t2Generic, Is.EqualTo(t2Float64).Within(Tolerance));

        var t3Float64 = signalFloat64.GetValue(1.0);
        var t3Generic = signalGeneric.GetValue(ScalarProcessor.One).ScalarValue;
        Assert.That(t3Generic, Is.EqualTo(t3Float64).Within(Tolerance));

        // All should be zero
        Assert.That(t1Generic, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(t2Generic, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(t3Generic, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void ConstantScalarSignal_Zero_GetDerivatives_ShouldBeZero()
    {
        var signalFloat64 = Float64Signal.FiniteZero();
        var signalGeneric = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var t = ScalarProcessor.ScalarFromNumber(0.5);

        var deriv1Float64 = signalFloat64.GetDerivative1Value(0.5);
        var deriv1Generic = signalGeneric.GetDerivative1Value(t).ScalarValue;
        Assert.That(deriv1Generic, Is.EqualTo(deriv1Float64).Within(Tolerance));
        Assert.That(deriv1Generic, Is.EqualTo(0.0).Within(Tolerance));

        var deriv2Float64 = signalFloat64.GetDerivative2Value(0.5);
        var deriv2Generic = signalGeneric.GetDerivative2Value(t).ScalarValue;
        Assert.That(deriv2Generic, Is.EqualTo(deriv2Float64).Within(Tolerance));
        Assert.That(deriv2Generic, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void ConstantScalarSignal_One_Finite_GetValue_ShouldMatch()
    {
        var signalFloat64 = Float64Signal.FiniteOne();
        var signalGeneric = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        // Test at several time points
        var t1Float64 = signalFloat64.GetValue(0.0);
        var t1Generic = signalGeneric.GetValue(ScalarProcessor.Zero).ScalarValue;
        Assert.That(t1Generic, Is.EqualTo(t1Float64).Within(Tolerance));

        var t2Float64 = signalFloat64.GetValue(0.5);
        var t2Generic = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.5)).ScalarValue;
        Assert.That(t2Generic, Is.EqualTo(t2Float64).Within(Tolerance));

        var t3Float64 = signalFloat64.GetValue(1.0);
        var t3Generic = signalGeneric.GetValue(ScalarProcessor.One).ScalarValue;
        Assert.That(t3Generic, Is.EqualTo(t3Float64).Within(Tolerance));

        // All should be one
        Assert.That(t1Generic, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(t2Generic, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(t3Generic, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void ConstantScalarSignal_One_GetDerivatives_ShouldBeZero()
    {
        var signalFloat64 = Float64Signal.FiniteOne();
        var signalGeneric = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        var t = ScalarProcessor.ScalarFromNumber(0.5);

        var deriv1Float64 = signalFloat64.GetDerivative1Value(0.5);
        var deriv1Generic = signalGeneric.GetDerivative1Value(t).ScalarValue;
        Assert.That(deriv1Generic, Is.EqualTo(deriv1Float64).Within(Tolerance));
        Assert.That(deriv1Generic, Is.EqualTo(0.0).Within(Tolerance));

        var deriv2Float64 = signalFloat64.GetDerivative2Value(0.5);
        var deriv2Generic = signalGeneric.GetDerivative2Value(t).ScalarValue;
        Assert.That(deriv2Generic, Is.EqualTo(deriv2Float64).Within(Tolerance));
        Assert.That(deriv2Generic, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void ConstantScalarSignal_CustomValue_GetValue_ShouldMatch()
    {
        const double value = 3.14159;
        var signalFloat64 = Float64Signal.FiniteConstant(value);
        var signalGeneric = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(value)
        );

        // Test at several time points
        var t1Float64 = signalFloat64.GetValue(0.0);
        var t1Generic = signalGeneric.GetValue(ScalarProcessor.Zero).ScalarValue;
        Assert.That(t1Generic, Is.EqualTo(t1Float64).Within(Tolerance));

        var t2Float64 = signalFloat64.GetValue(0.5);
        var t2Generic = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.5)).ScalarValue;
        Assert.That(t2Generic, Is.EqualTo(t2Float64).Within(Tolerance));

        // All should be the custom value
        Assert.That(t1Generic, Is.EqualTo(value).Within(Tolerance));
        Assert.That(t2Generic, Is.EqualTo(value).Within(Tolerance));
    }

    [Test]
    public void ConstantScalarSignal_ToPeriodicSignal_ShouldReturnPeriodicVersion()
    {
        var signalGeneric = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.5)
        );

        Assert.That(signalGeneric.IsFinite, Is.True);
        Assert.That(signalGeneric.IsPeriodic, Is.False);

        var periodicSignal = signalGeneric.ToPeriodicSignal();

        Assert.That(periodicSignal.IsFinite, Is.False);
        Assert.That(periodicSignal.IsPeriodic, Is.True);

        // Values should still match
        var value1 = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.5)).ScalarValue;
        var value2 = periodicSignal.GetValue(ScalarProcessor.ScalarFromNumber(0.5)).ScalarValue;
        Assert.That(value2, Is.EqualTo(value1).Within(Tolerance));
    }

    [Test]
    public void ConstantScalarSignal_ToFiniteSignal_ShouldReturnSelf()
    {
        var signalGeneric = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.5)
        );

        var finiteSignal = signalGeneric.ToFiniteSignal();

        Assert.That(finiteSignal, Is.SameAs(signalGeneric));
    }

    #endregion

    #region ComputedScalarSignal Tests

    [Test]
    public void ComputedScalarSignal_Linear_GetValue_ShouldMatch()
    {
        // Create a linear signal: f(t) = 2*t + 1
        Func<double, double> funcFloat64 = t => 2.0 * t + 1.0;
        var signalFloat64 = Float64Signal.FiniteComputed(
            GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64.Float64ScalarRange.SymmetricOne,
            funcFloat64
        );

        Func<Scalar<double>, Scalar<double>> funcGeneric = t =>
            ScalarProcessor.ScalarFromNumber(2.0) * t + ScalarProcessor.One;
        var signalGeneric = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            funcGeneric
        );

        // Test at several points
        var testPoints = new[] { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (var t in testPoints)
        {
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Mismatch at t={t}");
        }
    }

    [Test]
    public void ComputedScalarSignal_Quadratic_GetValue_ShouldMatch()
    {
        // Create a quadratic signal: f(t) = t^2
        Func<double, double> funcFloat64 = t => t * t;
        var signalFloat64 = Float64Signal.FiniteComputed(
            GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64.Float64ScalarRange.SymmetricOne,
            funcFloat64
        );

        Func<Scalar<double>, Scalar<double>> funcGeneric = t => t * t;
        var signalGeneric = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            funcGeneric
        );

        // Test at several points
        var testPoints = new[] { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (var t in testPoints)
        {
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Mismatch at t={t}");
        }
    }

    [Test]
    public void ComputedScalarSignal_WithDerivative_ShouldMatch()
    {
        // Create signal: f(t) = t^2, f'(t) = 2*t
        Func<double, double> funcFloat64 = t => t * t;
        Func<double, double> derivFloat64 = t => 2.0 * t;
        var signalFloat64 = Float64Signal.FiniteComputed(
            GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64.Float64ScalarRange.SymmetricOne,
            funcFloat64,
            derivFloat64
        );

        Func<Scalar<double>, Scalar<double>> funcGeneric = t => t * t;
        Func<Scalar<double>, Scalar<double>> derivGeneric = t =>
            ScalarProcessor.ScalarFromNumber(2.0) * t;
        var signalGeneric = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            funcGeneric,
            derivGeneric
        );

        // Test derivatives at several points
        var testPoints = new[] { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (var t in testPoints)
        {
            var deriv1Float64 = signalFloat64.GetDerivative1Value(t);
            var deriv1Generic = signalGeneric.GetDerivative1Value(
                ScalarProcessor.ScalarFromNumber(t)
            ).ScalarValue;

            Assert.That(deriv1Generic, Is.EqualTo(deriv1Float64).Within(Tolerance),
                $"Derivative mismatch at t={t}");
        }
    }

    [Test]
    public void ComputedScalarSignal_WithBothDerivatives_ShouldMatch()
    {
        // Create signal: f(t) = t^3, f'(t) = 3*t^2, f''(t) = 6*t
        Func<double, double> funcFloat64 = t => t * t * t;
        Func<double, double> deriv1Float64Func = t => 3.0 * t * t;
        Func<double, double> deriv2Float64Func = t => 6.0 * t;
        var signalFloat64 = Float64Signal.FiniteComputed(
            GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64.Float64ScalarRange.SymmetricOne,
            funcFloat64,
            deriv1Float64Func,
            deriv2Float64Func
        );

        Func<Scalar<double>, Scalar<double>> funcGeneric = t => t * t * t;
        Func<Scalar<double>, Scalar<double>> deriv1Generic = t =>
            ScalarProcessor.ScalarFromNumber(3.0) * t * t;
        Func<Scalar<double>, Scalar<double>> deriv2Generic = t =>
            ScalarProcessor.ScalarFromNumber(6.0) * t;
        var signalGeneric = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            funcGeneric,
            deriv1Generic,
            deriv2Generic
        );

        // Test both derivatives at several points
        var testPoints = new[] { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (var t in testPoints)
        {
            var deriv1Float64Val = signalFloat64.GetDerivative1Value(t);
            var deriv1GenericVal = signalGeneric.GetDerivative1Value(
                ScalarProcessor.ScalarFromNumber(t)
            ).ScalarValue;

            Assert.That(deriv1GenericVal, Is.EqualTo(deriv1Float64Val).Within(Tolerance),
                $"First derivative mismatch at t={t}");

            var deriv2Float64Val = signalFloat64.GetDerivative2Value(t);
            var deriv2GenericVal = signalGeneric.GetDerivative2Value(
                ScalarProcessor.ScalarFromNumber(t)
            ).ScalarValue;

            Assert.That(deriv2GenericVal, Is.EqualTo(deriv2Float64Val).Within(Tolerance),
                $"Second derivative mismatch at t={t}");
        }
    }

    [Test]
    public void ComputedScalarSignal_ToPeriodicSignal_ShouldReturnPeriodicVersion()
    {
        Func<Scalar<double>, Scalar<double>> func = t => t * t;
        var signalGeneric = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            func
        );

        Assert.That(signalGeneric.IsFinite, Is.True);
        Assert.That(signalGeneric.IsPeriodic, Is.False);

        var periodicSignal = signalGeneric.ToPeriodicSignal();

        Assert.That(periodicSignal.IsFinite, Is.False);
        Assert.That(periodicSignal.IsPeriodic, Is.True);

        // Values should still match
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var value1 = signalGeneric.GetValue(t).ScalarValue;
        var value2 = periodicSignal.GetValue(t).ScalarValue;
        Assert.That(value2, Is.EqualTo(value1).Within(Tolerance));
    }

    [Test]
    public void ComputedScalarSignal_WithoutDerivatives_ShouldThrowNotSupported()
    {
        Func<Scalar<double>, Scalar<double>> func = t => t * t;
        var signalGeneric = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            func
        );

        var t = ScalarProcessor.ScalarFromNumber(0.5);

        // Should throw NotSupportedException when derivatives not provided
        Assert.Throws<NotSupportedException>(() => signalGeneric.GetDerivative1Value(t));
        Assert.Throws<NotSupportedException>(() => signalGeneric.GetDerivative2Value(t));
    }

    [Test]
    public void ComputedScalarSignal_IsValid_ShouldReturnTrue()
    {
        Func<Scalar<double>, Scalar<double>> func = t => t * t;
        var signalGeneric = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            func
        );

        Assert.That(signalGeneric.IsValid(), Is.True);
    }

    #endregion
}
