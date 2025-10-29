using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Composers;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Composers;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class SimpleHarmonicScalarSignalComposerEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private IScalarProcessor<double> ScalarProcessor { get; }
        = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestSingleHarmonic()
    {
        // Float64 version
        var composerFloat64 = Float64ScalarSimpleHarmonicSignalComposer.Create();
        composerFloat64.SetHarmonic(1, 1.0, 0.0);
        var signalFloat64 = composerFloat64.GetSignal(false);

        // Generic version
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(1.0), ScalarProcessor.Scalar(0.0));
        var signalGeneric = composerGeneric.GetSignal(false);

        // Test values at various times
        for (var t = -1.0; t <= 1.0; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestMultipleHarmonics()
    {
        // Float64 version - fundamental + 2nd + 3rd harmonics
        var composerFloat64 = Float64ScalarSimpleHarmonicSignalComposer.Create();
        composerFloat64.SetHarmonic(1, 1.0);
        composerFloat64.SetHarmonic(2, 0.5);
        composerFloat64.SetHarmonic(3, 0.25);
        var signalFloat64 = composerFloat64.GetSignal(false);

        // Generic version
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(1.0));
        composerGeneric.SetHarmonic(2, ScalarProcessor.Scalar(0.5));
        composerGeneric.SetHarmonic(3, ScalarProcessor.Scalar(0.25));
        var signalGeneric = composerGeneric.GetSignal(false);

        // Test values at various times
        for (var t = -1.0; t <= 1.0; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestWithTimeShift()
    {
        // Float64 version - fundamental with time shift
        var composerFloat64 = Float64ScalarSimpleHarmonicSignalComposer.Create();
        composerFloat64.SetHarmonic(1, 1.0, 0.25);
        var signalFloat64 = composerFloat64.GetSignal(false);

        // Generic version
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(1.0), ScalarProcessor.Scalar(0.25));
        var signalGeneric = composerGeneric.GetSignal(false);

        // Test values at various times
        for (var t = -1.0; t <= 1.0; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestPeriodicSignal()
    {
        // Float64 version
        var composerFloat64 = Float64ScalarSimpleHarmonicSignalComposer.Create();
        composerFloat64.SetHarmonic(1, 1.0);
        composerFloat64.SetHarmonic(2, 0.5);
        var signalFloat64 = composerFloat64.GetSignal(true);

        // Generic version
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(1.0));
        composerGeneric.SetHarmonic(2, ScalarProcessor.Scalar(0.5));
        var signalGeneric = composerGeneric.GetSignal(true);

        // Verify periodic flag
        Assert.That(signalGeneric.IsPeriodic, Is.True);
        Assert.That(signalFloat64.IsPeriodic, Is.True);

        // Test values
        for (var t = -2.0; t <= 2.0; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestClearMethod()
    {
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(1.0));
        composerGeneric.SetHarmonic(2, ScalarProcessor.Scalar(0.5));

        composerGeneric.Clear();

        var signalGeneric = composerGeneric.GetSignal(false);
        var valueGeneric = signalGeneric.GetValue(ScalarProcessor.Scalar(0.5)).ScalarValue;

        // After clear, signal should be zero (sum of no terms)
        Assert.That(valueGeneric, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void TestRemoveHarmonic()
    {
        // Float64 version
        var composerFloat64 = Float64ScalarSimpleHarmonicSignalComposer.Create();
        composerFloat64.SetHarmonic(1, 1.0);
        composerFloat64.SetHarmonic(2, 0.5);
        composerFloat64.RemoveHarmonic(2);
        var signalFloat64 = composerFloat64.GetSignal(false);

        // Generic version
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(1.0));
        composerGeneric.SetHarmonic(2, ScalarProcessor.Scalar(0.5));
        composerGeneric.RemoveHarmonic(2);
        var signalGeneric = composerGeneric.GetSignal(false);

        // Should only have harmonic 1 remaining
        for (var t = -1.0; t <= 1.0; t += 0.3)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestReplaceHarmonic()
    {
        // Float64 version - set same harmonic twice (should replace)
        var composerFloat64 = Float64ScalarSimpleHarmonicSignalComposer.Create();
        composerFloat64.SetHarmonic(1, 1.0);
        composerFloat64.SetHarmonic(1, 2.0);  // Replace with new magnitude
        var signalFloat64 = composerFloat64.GetSignal(false);

        // Generic version
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(1.0));
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(2.0));  // Replace
        var signalGeneric = composerGeneric.GetSignal(false);

        // Should have magnitude 2.0, not 3.0 (sum would be 3.0 if both were kept)
        for (var t = -1.0; t <= 1.0; t += 0.3)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestIsValid()
    {
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(1.0));
        composerGeneric.SetHarmonic(2, ScalarProcessor.Scalar(0.5));

        Assert.That(composerGeneric.IsValid(), Is.True);
    }

    [Test]
    public void TestHigherHarmonics()
    {
        // Float64 version - test with higher harmonic factors
        var composerFloat64 = Float64ScalarSimpleHarmonicSignalComposer.Create();
        composerFloat64.SetHarmonic(5, 0.8);
        composerFloat64.SetHarmonic(7, 0.6);
        var signalFloat64 = composerFloat64.GetSignal(false);

        // Generic version
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(5, ScalarProcessor.Scalar(0.8));
        composerGeneric.SetHarmonic(7, ScalarProcessor.Scalar(0.6));
        var signalGeneric = composerGeneric.GetSignal(false);

        // Test values
        for (var t = 0.0; t <= 1.0; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestDerivatives()
    {
        // Float64 version
        var composerFloat64 = Float64ScalarSimpleHarmonicSignalComposer.Create();
        composerFloat64.SetHarmonic(1, 1.0);
        composerFloat64.SetHarmonic(2, 0.5);
        var signalFloat64 = composerFloat64.GetSignal(false);

        // Generic version
        var composerGeneric = SimpleHarmonicScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.SetHarmonic(1, ScalarProcessor.Scalar(1.0));
        composerGeneric.SetHarmonic(2, ScalarProcessor.Scalar(0.5));
        var signalGeneric = composerGeneric.GetSignal(false);

        // Test first derivative
        for (var t = -1.0; t <= 1.0; t += 0.3)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var derivFloat64 = signalFloat64.GetDerivative1Value(t);
            var derivGeneric = signalGeneric.GetDerivative1Value(tScalar).ScalarValue;

            Assert.That(derivGeneric, Is.EqualTo(derivFloat64).Within(Tolerance),
                $"Derivative mismatch at t={t}");
        }
    }
}
