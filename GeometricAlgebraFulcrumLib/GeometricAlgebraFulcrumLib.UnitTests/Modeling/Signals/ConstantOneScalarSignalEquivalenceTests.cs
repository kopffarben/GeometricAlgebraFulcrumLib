using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ConstantOneScalarSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private IScalarProcessor<double> ScalarProcessor { get; }
        = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestFiniteConstantOne_Value()
    {
        // Float64 version
        var signalFloat64 = Float64ScalarConstantOneSignal.FiniteInstance;

        // Generic version
        var signalGeneric = ConstantOneScalarSignal<double>.Finite(ScalarProcessor);

        // Test values at various times
        for (var t = -5.0; t <= 5.0; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance));
            Assert.That(valueFloat64, Is.EqualTo(1.0));
            Assert.That(valueGeneric, Is.EqualTo(1.0).Within(Tolerance));
        }
    }

    [Test]
    public void TestPeriodicConstantOne_Value()
    {
        // Float64 version
        var signalFloat64 = Float64ScalarConstantOneSignal.PeriodicInstance;

        // Generic version
        var signalGeneric = ConstantOneScalarSignal<double>.Periodic(ScalarProcessor);

        // Verify periodic flag
        Assert.That(signalGeneric.IsPeriodic, Is.True);
        Assert.That(signalFloat64.IsPeriodic, Is.True);

        // Test values at various times
        for (var t = -10.0; t <= 10.0; t += 1.0)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance));
            Assert.That(valueGeneric, Is.EqualTo(1.0).Within(Tolerance));
        }
    }

    [Test]
    public void TestDerivative1_AlwaysZero()
    {
        // Float64 version
        var signalFloat64 = Float64ScalarConstantOneSignal.FiniteInstance;

        // Generic version
        var signalGeneric = ConstantOneScalarSignal<double>.Finite(ScalarProcessor);

        // Test derivative at various times
        for (var t = -5.0; t <= 5.0; t += 1.0)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var derivFloat64 = signalFloat64.GetDerivative1Value(t);
            var derivGeneric = signalGeneric.GetDerivative1Value(tScalar).ScalarValue;

            Assert.That(derivGeneric, Is.EqualTo(derivFloat64).Within(Tolerance));
            Assert.That(derivFloat64, Is.EqualTo(0.0));
            Assert.That(derivGeneric, Is.EqualTo(0.0).Within(Tolerance));
        }
    }

    [Test]
    public void TestDerivative2_AlwaysZero()
    {
        // Float64 version
        var signalFloat64 = Float64ScalarConstantOneSignal.FiniteInstance;

        // Generic version
        var signalGeneric = ConstantOneScalarSignal<double>.Finite(ScalarProcessor);

        // Test second derivative at various times
        for (var t = -5.0; t <= 5.0; t += 1.0)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var derivFloat64 = signalFloat64.GetDerivative2Value(t);
            var derivGeneric = signalGeneric.GetDerivative2Value(tScalar).ScalarValue;

            Assert.That(derivGeneric, Is.EqualTo(derivFloat64).Within(Tolerance));
            Assert.That(derivFloat64, Is.EqualTo(0.0));
            Assert.That(derivGeneric, Is.EqualTo(0.0).Within(Tolerance));
        }
    }

    [Test]
    public void TestTimeRange()
    {
        var signalFloat64 = Float64ScalarConstantOneSignal.FiniteInstance;
        var signalGeneric = ConstantOneScalarSignal<double>.Finite(ScalarProcessor);

        // Verify time ranges match (should be SymmetricOne = [-1, 1])
        Assert.That(signalGeneric.MinTime.ScalarValue, Is.EqualTo(signalFloat64.MinTime).Within(Tolerance));
        Assert.That(signalGeneric.MaxTime.ScalarValue, Is.EqualTo(signalFloat64.MaxTime).Within(Tolerance));
    }

    [Test]
    public void TestIsValid()
    {
        var signalGeneric = ConstantOneScalarSignal<double>.Finite(ScalarProcessor);

        Assert.That(signalGeneric.IsValid(), Is.True);
    }

    [Test]
    public void TestToFiniteSignal()
    {
        var periodicGeneric = ConstantOneScalarSignal<double>.Periodic(ScalarProcessor);
        var finiteGeneric = periodicGeneric.ToFiniteSignal();

        Assert.That(finiteGeneric.IsFinite, Is.True);
        Assert.That(finiteGeneric.IsPeriodic, Is.False);

        // Values should match
        for (var t = -1.0; t <= 1.0; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var originalValue = periodicGeneric.GetValue(tScalar).ScalarValue;
            var finiteValue = finiteGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(finiteValue, Is.EqualTo(originalValue).Within(Tolerance));
            Assert.That(finiteValue, Is.EqualTo(1.0).Within(Tolerance));
        }
    }

    [Test]
    public void TestToPeriodicSignal()
    {
        var finiteGeneric = ConstantOneScalarSignal<double>.Finite(ScalarProcessor);
        var periodicGeneric = finiteGeneric.ToPeriodicSignal();

        Assert.That(periodicGeneric.IsFinite, Is.False);
        Assert.That(periodicGeneric.IsPeriodic, Is.True);

        // Values should match
        for (var t = -1.0; t <= 1.0; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var originalValue = finiteGeneric.GetValue(tScalar).ScalarValue;
            var periodicValue = periodicGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(periodicValue, Is.EqualTo(originalValue).Within(Tolerance));
            Assert.That(periodicValue, Is.EqualTo(1.0).Within(Tolerance));
        }
    }

    [Test]
    public void TestConstantBehavior_EdgeCases()
    {
        var signalGeneric = ConstantOneScalarSignal<double>.Finite(ScalarProcessor);

        // Test extreme values
        var extremeValues = new[] { -1000.0, -100.0, 0.0, 100.0, 1000.0 };

        foreach (var t in extremeValues)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var value = signalGeneric.GetValue(tScalar).ScalarValue;
            Assert.That(value, Is.EqualTo(1.0).Within(Tolerance),
                $"Value should be 1.0 at t={t}");
        }
    }
}
