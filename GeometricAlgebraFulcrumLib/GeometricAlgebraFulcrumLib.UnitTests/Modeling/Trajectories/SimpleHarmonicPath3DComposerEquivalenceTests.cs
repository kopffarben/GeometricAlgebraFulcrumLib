using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Composers;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Composers;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class SimpleHarmonicPath3DComposerEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void SimpleHarmonicPath3DComposer_SetHarmonic_WithScalarComponents_ShouldMatchFloat64()
    {
        const double magX = 1.0, magY = 2.0, magZ = 3.0;

        var composerFloat64 = Float64SimpleHarmonicPath3DComposer.Create()
            .SetHarmonic(1, magX, magY, magZ)
            .SetHarmonic(2, 0.5, 0.5, 0.5);  // Need at least 2 harmonics for PlusPath3D

        var composerGeneric = SimpleHarmonicPath3DComposer<double>.Create(ScalarProcessor)
            .SetHarmonic(1, magX, magY, magZ)
            .SetHarmonic(2, 0.5, 0.5, 0.5);

        var pathFloat64 = composerFloat64.GetSignal(false);
        var pathGeneric = composerGeneric.GetSignal(false);

        const double t = 0.0;
        var pointFloat64 = pathFloat64.GetValue(t);
        var pointGeneric = pathGeneric.GetValue(ScalarProcessor.ScalarFromValue(t));

        Assert.That(pointGeneric.X.ScalarValue, Is.EqualTo(pointFloat64.X.ScalarValue).Within(Tolerance),
            $"X component should match at t={t}");
        Assert.That(pointGeneric.Y.ScalarValue, Is.EqualTo(pointFloat64.Y.ScalarValue).Within(Tolerance),
            $"Y component should match at t={t}");
        Assert.That(pointGeneric.Z.ScalarValue, Is.EqualTo(pointFloat64.Z.ScalarValue).Within(Tolerance),
            $"Z component should match at t={t}");

        Debug.Assert(Math.Abs(pointGeneric.X.ScalarValue - pointFloat64.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void SimpleHarmonicPath3DComposer_SetHarmonic_WithVector_ShouldMatchFloat64()
    {
        var magnitude1 = LinFloat64Vector3D.Create(1.5, 2.5, 3.5);
        var magnitude2 = LinFloat64Vector3D.Create(0.5, 0.5, 0.5);

        var composerFloat64 = Float64SimpleHarmonicPath3DComposer.Create()
            .SetHarmonic(1, magnitude1)
            .SetHarmonic(2, magnitude2);  // Need at least 2 harmonics

        var composerGeneric = SimpleHarmonicPath3DComposer<double>.Create(ScalarProcessor)
            .SetHarmonic(
                1,
                LinVector3D<double>.Create(
                    ScalarProcessor.ScalarFromValue(1.5),
                    ScalarProcessor.ScalarFromValue(2.5),
                    ScalarProcessor.ScalarFromValue(3.5)
                )
            )
            .SetHarmonic(
                2,
                LinVector3D<double>.Create(
                    ScalarProcessor.ScalarFromValue(0.5),
                    ScalarProcessor.ScalarFromValue(0.5),
                    ScalarProcessor.ScalarFromValue(0.5)
                )
            );

        var pathFloat64 = composerFloat64.GetSignal(true);
        var pathGeneric = composerGeneric.GetSignal(true);

        const double t = Math.PI / 4;
        var pointFloat64 = pathFloat64.GetValue(t);
        var pointGeneric = pathGeneric.GetValue(ScalarProcessor.ScalarFromValue(t));

        Assert.That(pointGeneric.X.ScalarValue, Is.EqualTo(pointFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(pointGeneric.Y.ScalarValue, Is.EqualTo(pointFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(pointGeneric.Z.ScalarValue, Is.EqualTo(pointFloat64.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(pointGeneric.X.ScalarValue - pointFloat64.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void SimpleHarmonicPath3DComposer_MultipleHarmonics_ShouldMatchFloat64()
    {
        var composerFloat64 = Float64SimpleHarmonicPath3DComposer.Create()
            .SetHarmonic(1, 1.0, 0.0, 0.0)  // Fundamental frequency, X component
            .SetHarmonic(2, 0.0, 0.5, 0.0)  // Second harmonic, Y component
            .SetHarmonic(3, 0.0, 0.0, 0.25); // Third harmonic, Z component

        var composerGeneric = SimpleHarmonicPath3DComposer<double>.Create(ScalarProcessor)
            .SetHarmonic(1, 1.0, 0.0, 0.0)
            .SetHarmonic(2, 0.0, 0.5, 0.0)
            .SetHarmonic(3, 0.0, 0.0, 0.25);

        var pathFloat64 = composerFloat64.GetSignal(false);
        var pathGeneric = composerGeneric.GetSignal(false);

        var testTimes = new[] { -Math.PI, -Math.PI / 2, 0.0, Math.PI / 4, Math.PI / 2, Math.PI };

        foreach (var t in testTimes)
        {
            var pointFloat64 = pathFloat64.GetValue(t);
            var pointGeneric = pathGeneric.GetValue(ScalarProcessor.ScalarFromValue(t));

            Assert.That(pointGeneric.X.ScalarValue, Is.EqualTo(pointFloat64.X.ScalarValue).Within(Tolerance),
                $"X component mismatch at t={t}");
            Assert.That(pointGeneric.Y.ScalarValue, Is.EqualTo(pointFloat64.Y.ScalarValue).Within(Tolerance),
                $"Y component mismatch at t={t}");
            Assert.That(pointGeneric.Z.ScalarValue, Is.EqualTo(pointFloat64.Z.ScalarValue).Within(Tolerance),
                $"Z component mismatch at t={t}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void SimpleHarmonicPath3DComposer_Clear_ThenAddNewHarmonics_ShouldMatchFloat64()
    {
        // Test that Clear() removes all harmonics and we can start fresh
        var composerFloat64 = Float64SimpleHarmonicPath3DComposer.Create()
            .SetHarmonic(1, 1.0, 0.0, 0.0)
            .SetHarmonic(2, 0.0, 1.0, 0.0)
            .Clear()
            .SetHarmonic(3, 0.0, 0.0, 1.0)
            .SetHarmonic(4, 0.5, 0.5, 0.5);

        var composerGeneric = SimpleHarmonicPath3DComposer<double>.Create(ScalarProcessor)
            .SetHarmonic(1, 1.0, 0.0, 0.0)
            .SetHarmonic(2, 0.0, 1.0, 0.0)
            .Clear()
            .SetHarmonic(3, 0.0, 0.0, 1.0)
            .SetHarmonic(4, 0.5, 0.5, 0.5);

        var pathFloat64 = composerFloat64.GetSignal(false);
        var pathGeneric = composerGeneric.GetSignal(false);

        const double t = 0.0;
        var pointFloat64 = pathFloat64.GetValue(t);
        var pointGeneric = pathGeneric.GetValue(ScalarProcessor.ScalarFromValue(t));

        Assert.That(pointGeneric.X.ScalarValue, Is.EqualTo(pointFloat64.X.ScalarValue).Within(Tolerance),
            "After Clear(), only harmonics 3 and 4 should be active");
        Assert.That(pointGeneric.Y.ScalarValue, Is.EqualTo(pointFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(pointGeneric.Z.ScalarValue, Is.EqualTo(pointFloat64.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(pointGeneric.X.ScalarValue - pointFloat64.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void SimpleHarmonicPath3DComposer_RemoveHarmonic_ShouldMatchFloat64()
    {
        var composerFloat64 = Float64SimpleHarmonicPath3DComposer.Create()
            .SetHarmonic(1, 1.0, 0.0, 0.0)
            .SetHarmonic(2, 0.0, 1.0, 0.0)
            .SetHarmonic(3, 0.0, 0.0, 1.0)
            .RemoveHarmonic(2);  // Remove second harmonic

        var composerGeneric = SimpleHarmonicPath3DComposer<double>.Create(ScalarProcessor)
            .SetHarmonic(1, 1.0, 0.0, 0.0)
            .SetHarmonic(2, 0.0, 1.0, 0.0)
            .SetHarmonic(3, 0.0, 0.0, 1.0)
            .RemoveHarmonic(2);

        var pathFloat64 = composerFloat64.GetSignal(false);
        var pathGeneric = composerGeneric.GetSignal(false);

        const double t = 0.0;
        var pointFloat64 = pathFloat64.GetValue(t);
        var pointGeneric = pathGeneric.GetValue(ScalarProcessor.ScalarFromValue(t));

        Assert.That(pointGeneric.X.ScalarValue, Is.EqualTo(pointFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(pointGeneric.Y.ScalarValue, Is.EqualTo(pointFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(pointGeneric.Z.ScalarValue, Is.EqualTo(pointFloat64.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(pointGeneric.Y.ScalarValue) < Tolerance); // Y should be zero (harmonic 2 removed)
    }

    [Test]
    public void SimpleHarmonicPath3DComposer_GetSignal_Periodic_ShouldMatchFloat64()
    {
        var composerFloat64 = Float64SimpleHarmonicPath3DComposer.Create()
            .SetHarmonic(1, 1.0, 1.0, 1.0)
            .SetHarmonic(2, 0.5, 0.5, 0.5);  // Need at least 2 harmonics

        var composerGeneric = SimpleHarmonicPath3DComposer<double>.Create(ScalarProcessor)
            .SetHarmonic(1, 1.0, 1.0, 1.0)
            .SetHarmonic(2, 0.5, 0.5, 0.5);

        var pathFloat64 = composerFloat64.GetSignal(true);  // Periodic
        var pathGeneric = composerGeneric.GetSignal(true);

        Assert.That(pathGeneric.IsPeriodic, Is.True, "Generic path should be periodic");
        Assert.That(pathFloat64.IsPeriodic, Is.True, "Float64 path should be periodic");

        const double t = Math.PI;
        var pointFloat64 = pathFloat64.GetValue(t);
        var pointGeneric = pathGeneric.GetValue(ScalarProcessor.ScalarFromValue(t));

        Assert.That(pointGeneric.X.ScalarValue, Is.EqualTo(pointFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(pointGeneric.Y.ScalarValue, Is.EqualTo(pointFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(pointGeneric.Z.ScalarValue, Is.EqualTo(pointFloat64.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(pathGeneric.IsPeriodic);
    }

    [Test]
    public void SimpleHarmonicPath3DComposer_GetSignal_Finite_ShouldMatchFloat64()
    {
        var composerFloat64 = Float64SimpleHarmonicPath3DComposer.Create()
            .SetHarmonic(1, 2.0, 2.0, 2.0)
            .SetHarmonic(2, 1.0, 1.0, 1.0);  // Need at least 2 harmonics

        var composerGeneric = SimpleHarmonicPath3DComposer<double>.Create(ScalarProcessor)
            .SetHarmonic(1, 2.0, 2.0, 2.0)
            .SetHarmonic(2, 1.0, 1.0, 1.0);

        var pathFloat64 = composerFloat64.GetSignal(false);  // Finite
        var pathGeneric = composerGeneric.GetSignal(false);

        Assert.That(pathGeneric.IsFinite, Is.True, "Generic path should be finite");
        Assert.That(pathFloat64.IsFinite, Is.True, "Float64 path should be finite");

        const double t = -Math.PI / 2;
        var pointFloat64 = pathFloat64.GetValue(t);
        var pointGeneric = pathGeneric.GetValue(ScalarProcessor.ScalarFromValue(t));

        Assert.That(pointGeneric.X.ScalarValue, Is.EqualTo(pointFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(pointGeneric.Y.ScalarValue, Is.EqualTo(pointFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(pointGeneric.Z.ScalarValue, Is.EqualTo(pointFloat64.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(pathGeneric.IsFinite);
    }

    [Test]
    public void SimpleHarmonicPath3DComposer_GetDerivative1Value_ShouldMatchFloat64()
    {
        var composerFloat64 = Float64SimpleHarmonicPath3DComposer.Create()
            .SetHarmonic(1, 1.0, 0.0, 0.0)
            .SetHarmonic(2, 0.0, 1.0, 0.0);

        var composerGeneric = SimpleHarmonicPath3DComposer<double>.Create(ScalarProcessor)
            .SetHarmonic(1, 1.0, 0.0, 0.0)
            .SetHarmonic(2, 0.0, 1.0, 0.0);

        var pathFloat64 = composerFloat64.GetSignal(false);
        var pathGeneric = composerGeneric.GetSignal(false);

        const double t = Math.PI / 4;
        var derivFloat64 = pathFloat64.GetDerivative1Value(t);
        var derivGeneric = pathGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromValue(t));

        Assert.That(derivGeneric.X.ScalarValue, Is.EqualTo(derivFloat64.X.ScalarValue).Within(Tolerance),
            "X derivative should match");
        Assert.That(derivGeneric.Y.ScalarValue, Is.EqualTo(derivFloat64.Y.ScalarValue).Within(Tolerance),
            "Y derivative should match");
        Assert.That(derivGeneric.Z.ScalarValue, Is.EqualTo(derivFloat64.Z.ScalarValue).Within(Tolerance),
            "Z derivative should match");

        Debug.Assert(Math.Abs(derivGeneric.X.ScalarValue - derivFloat64.X.ScalarValue) < Tolerance);
    }
}
