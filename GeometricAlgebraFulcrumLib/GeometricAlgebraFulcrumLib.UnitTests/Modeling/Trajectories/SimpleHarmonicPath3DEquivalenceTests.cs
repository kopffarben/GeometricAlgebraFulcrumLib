using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class SimpleHarmonicPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    [Test]
    public void TestFiniteSymmetric_Properties()
    {
        const int harmonicFactor = 2;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessorOfFloat64.Instance,
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
            )
        );

        Assert.That(genericPath.HarmonicFactor, Is.EqualTo(float64Path.HarmonicFactor));
        Assert.That(genericPath.Magnitude.X.ScalarValue, Is.EqualTo(float64Path.Magnitude.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPath.Magnitude.Y.ScalarValue, Is.EqualTo(float64Path.Magnitude.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPath.Magnitude.Z.ScalarValue, Is.EqualTo(float64Path.Magnitude.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(genericPath.HarmonicFactor == float64Path.HarmonicFactor);
    }

    [Test]
    public void TestPeriodicSymmetric_Properties()
    {
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(2.0, 3.0, 4.0);

        var float64Path = Float64SimpleHarmonicPath3D.PeriodicSymmetric(harmonicFactor, magnitude);
        var genericPath = SimpleHarmonicPath3D<double>.PeriodicSymmetric(
            ScalarProcessorOfFloat64.Instance,
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(4.0)
            )
        );

        Assert.That(genericPath.IsPeriodic, Is.True);
        Assert.That(float64Path.IsPeriodic, Is.True);
        Assert.That(genericPath.HarmonicFactor, Is.EqualTo(float64Path.HarmonicFactor));

        Debug.Assert(genericPath.IsPeriodic && float64Path.IsPeriodic);
    }

    [Test]
    public void TestGetValue_StartPoint()
    {
        const double t = 0.0;
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessorOfFloat64.Instance,
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
            )
        );

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue - float64Point.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestGetValue_QuarterPeriod()
    {
        // For harmonic factor 1, quarter period is at t = π/2
        const double t = Math.PI / 2.0;
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);

        var float64Path = Float64SimpleHarmonicPath3D.PeriodicSymmetric(harmonicFactor, magnitude);
        var genericPath = SimpleHarmonicPath3D<double>.PeriodicSymmetric(
            ScalarProcessorOfFloat64.Instance,
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.One,
                ScalarProcessorOfFloat64.Instance.One,
                ScalarProcessorOfFloat64.Instance.One
            )
        );

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue - float64Point.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestGetValue_MultipleTimes()
    {
        const int harmonicFactor = 3;
        var magnitude = LinFloat64Vector3D.Create(2.0, 3.0, 1.5);
        var testTimes = new[] { -Math.PI, -Math.PI/2, 0.0, Math.PI/4, Math.PI/2, Math.PI };

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessorOfFloat64.Instance,
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.5)
            )
        );

        foreach (var t in testTimes)
        {
            var float64Point = float64Path.GetValue(t);
            var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

            Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance),
                $"Mismatch at t={t}, X component");
            Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance),
                $"Mismatch at t={t}, Y component");
            Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance),
                $"Mismatch at t={t}, Z component");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestGetDerivative1Value_StartPoint()
    {
        const double t = 0.0;
        const int harmonicFactor = 2;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessorOfFloat64.Instance,
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
            )
        );

        var float64Deriv = float64Path.GetDerivative1Value(t);
        var genericDeriv = genericPath.GetDerivative1Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericDeriv.X.ScalarValue, Is.EqualTo(float64Deriv.X.ScalarValue).Within(Tolerance));
        Assert.That(genericDeriv.Y.ScalarValue, Is.EqualTo(float64Deriv.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericDeriv.Z.ScalarValue, Is.EqualTo(float64Deriv.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericDeriv.X.ScalarValue - float64Deriv.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestGetDerivative1Value_MultipleTimes()
    {
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);
        var testTimes = new[] { 0.0, Math.PI/4, Math.PI/2, 3*Math.PI/4, Math.PI };

        var float64Path = Float64SimpleHarmonicPath3D.PeriodicSymmetric(harmonicFactor, magnitude);
        var genericPath = SimpleHarmonicPath3D<double>.PeriodicSymmetric(
            ScalarProcessorOfFloat64.Instance,
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.One,
                ScalarProcessorOfFloat64.Instance.One,
                ScalarProcessorOfFloat64.Instance.One
            )
        );

        foreach (var t in testTimes)
        {
            var float64Deriv = float64Path.GetDerivative1Value(t);
            var genericDeriv = genericPath.GetDerivative1Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

            Assert.That(genericDeriv.X.ScalarValue, Is.EqualTo(float64Deriv.X.ScalarValue).Within(Tolerance),
                $"Mismatch at t={t}, X derivative");
            Assert.That(genericDeriv.Y.ScalarValue, Is.EqualTo(float64Deriv.Y.ScalarValue).Within(Tolerance),
                $"Mismatch at t={t}, Y derivative");
            Assert.That(genericDeriv.Z.ScalarValue, Is.EqualTo(float64Deriv.Z.ScalarValue).Within(Tolerance),
                $"Mismatch at t={t}, Z derivative");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestGetDerivative2Value_StartPoint()
    {
        const double t = 0.0;
        const int harmonicFactor = 2;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessorOfFloat64.Instance,
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
            )
        );

        var float64Accel = float64Path.GetDerivative2Value(t);
        var genericAccel = genericPath.GetDerivative2Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericAccel.X.ScalarValue, Is.EqualTo(float64Accel.X.ScalarValue).Within(Tolerance));
        Assert.That(genericAccel.Y.ScalarValue, Is.EqualTo(float64Accel.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericAccel.Z.ScalarValue, Is.EqualTo(float64Accel.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericAccel.X.ScalarValue - float64Accel.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestGetDerivative2Value_MultipleTimes()
    {
        const int harmonicFactor = 3;
        var magnitude = LinFloat64Vector3D.Create(2.0, 1.0, 3.0);
        var testTimes = new[] { -Math.PI/2, 0.0, Math.PI/4, Math.PI/2 };

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessorOfFloat64.Instance,
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
            )
        );

        foreach (var t in testTimes)
        {
            var float64Accel = float64Path.GetDerivative2Value(t);
            var genericAccel = genericPath.GetDerivative2Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

            Assert.That(genericAccel.X.ScalarValue, Is.EqualTo(float64Accel.X.ScalarValue).Within(Tolerance),
                $"Mismatch at t={t}, X acceleration");
            Assert.That(genericAccel.Y.ScalarValue, Is.EqualTo(float64Accel.Y.ScalarValue).Within(Tolerance),
                $"Mismatch at t={t}, Y acceleration");
            Assert.That(genericAccel.Z.ScalarValue, Is.EqualTo(float64Accel.Z.ScalarValue).Within(Tolerance),
                $"Mismatch at t={t}, Z acceleration");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestIsValid()
    {
        var magnitude = LinVector3D<double>.Create(
            ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
            ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
            ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
        );

        var path = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessorOfFloat64.Instance,
            1,
            magnitude
        );

        Assert.That(path.IsValid(), Is.True);

        Debug.Assert(path.IsValid());
    }

    [Test]
    public void TestToFinitePath()
    {
        var magnitude = LinVector3D<double>.Create(
            ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
            ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
            ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
        );

        var periodicPath = SimpleHarmonicPath3D<double>.PeriodicSymmetric(
            ScalarProcessorOfFloat64.Instance,
            2,
            magnitude
        );

        var finitePath = periodicPath.ToFinitePath();

        Assert.That(finitePath.IsFinite, Is.True);
        Assert.That(finitePath.IsPeriodic, Is.False);

        Debug.Assert(finitePath.IsFinite);
    }

    [Test]
    public void TestToPeriodicPath()
    {
        var magnitude = LinVector3D<double>.Create(
            ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
            ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
            ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
        );

        var finitePath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessorOfFloat64.Instance,
            2,
            magnitude
        );

        var periodicPath = finitePath.ToPeriodicPath();

        Assert.That(periodicPath.IsPeriodic, Is.True);
        Assert.That(periodicPath.IsFinite, Is.False);

        Debug.Assert(periodicPath.IsPeriodic);
    }

    [Test]
    public void TestTimeOffset_EffectOnPhase()
    {
        const double t = 0.0;
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);
        var timeOffset = LinFloat64Vector3D.Create(0.0, 0.25, -0.25); // Phase shifts

        var float64Path = Float64SimpleHarmonicPath3D.Finite(harmonicFactor, magnitude, timeOffset);
        var genericPath = SimpleHarmonicPath3D<double>.Finite(
            ScalarRange<double>.SymmetricPi(ScalarProcessorOfFloat64.Instance),
            harmonicFactor,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.One,
                ScalarProcessorOfFloat64.Instance.One,
                ScalarProcessorOfFloat64.Instance.One
            ),
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.Zero,
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(0.25),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(-0.25)
            )
        );

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // X should be 1.0 (no phase shift)
        // Y should be cos(2π * 0.25) ≈ 0.0
        // Z should be cos(2π * -0.25) ≈ 0.0

        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue - float64Point.X.ScalarValue) < Tolerance);
    }
}
