using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class SphericalPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void SphericalPath3D_ConstantRadiusZeroAngles_ShouldReturnPointOnPositiveXAxis()
    {
        // r = 1, theta = 0, phi = 0
        // => x = 1*cos(0)*cos(0) = 1, y = 1*cos(0)*sin(0) = 0, z = 1*sin(0) = 0
        // Point at (1, 0, 0)

        var rCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        var thetaCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var phiCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var path = SphericalPath3D<double>.Finite(
            ScalarRange<double>.SymmetricOne(ScalarProcessor),
            rCurve,
            thetaCurve,
            phiCurve
        );

        var value = path.GetValue(ScalarProcessor.Zero);

        Assert.That(value.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X coordinate");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y coordinate");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z coordinate");
    }

    [Test]
    public void SphericalPath3D_ThetaPiOver2PhiZero_ShouldReturnPointOnPositiveZAxis()
    {
        // r = 1, theta = pi/2, phi = 0
        // => x = 1*cos(pi/2)*cos(0) = 0, y = 1*cos(pi/2)*sin(0) = 0, z = 1*sin(pi/2) = 1
        // Point at (0, 0, 1)

        var rCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        var thetaCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.PiOver2
        );

        var phiCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var path = SphericalPath3D<double>.Finite(
            ScalarRange<double>.SymmetricOne(ScalarProcessor),
            rCurve,
            thetaCurve,
            phiCurve
        );

        var value = path.GetValue(ScalarProcessor.Zero);

        Assert.That(value.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X coordinate");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y coordinate");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Z coordinate");
    }

    [Test]
    public void SphericalPath3D_ThetaZeroPhiPiOver2_ShouldReturnPointOnPositiveYAxis()
    {
        // r = 1, theta = 0, phi = pi/2
        // => x = 1*cos(0)*cos(pi/2) = 0, y = 1*cos(0)*sin(pi/2) = 1, z = 1*sin(0) = 0
        // Point at (0, 1, 0)

        var rCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        var thetaCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var phiCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.PiOver2
        );

        var path = SphericalPath3D<double>.Finite(
            ScalarRange<double>.SymmetricOne(ScalarProcessor),
            rCurve,
            thetaCurve,
            phiCurve
        );

        var value = path.GetValue(ScalarProcessor.Zero);

        Assert.That(value.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X coordinate");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y coordinate");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z coordinate");
    }

    [Test]
    public void SphericalPath3D_ConstantCoordinates_DerivativesShouldBeZero()
    {
        // When all coordinates are constant, all derivatives should be zero

        var rCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        var thetaCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.PiOver2
        );

        var phiCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var path = SphericalPath3D<double>.Finite(
            ScalarRange<double>.SymmetricOne(ScalarProcessor),
            rCurve,
            thetaCurve,
            phiCurve
        );

        var deriv1 = path.GetDerivative1Value(ScalarProcessor.Zero);

        Assert.That(deriv1.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "First derivative X");
        Assert.That(deriv1.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "First derivative Y");
        Assert.That(deriv1.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "First derivative Z");

        var deriv2 = path.GetDerivative2Value(ScalarProcessor.Zero);

        Assert.That(deriv2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative X");
        Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Y");
        Assert.That(deriv2.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Z");
    }

    [Test]
    public void SphericalPath3D_LinearRadius_ShouldMoveRadially()
    {
        // r(t) = t, theta = pi/4, phi = pi/4
        // Movement along a fixed direction from origin

        Func<Scalar<double>, Scalar<double>> rFunc = t => t;
        var rCurve = ComputedScalarSignal<double>.Finite(ScalarProcessor, rFunc);

        var thetaCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(Math.PI / 4)
        );

        var phiCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(Math.PI / 4)
        );

        var path = SphericalPath3D<double>.Finite(
            ScalarRange<double>.SymmetricOne(ScalarProcessor),
            rCurve,
            thetaCurve,
            phiCurve
        );

        // At t=0: r=0 => (0,0,0)
        var value0 = path.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        // At t=1: r=1, theta=pi/4, phi=pi/4
        var value1 = path.GetValue(ScalarProcessor.One);
        var expectedX = Math.Cos(Math.PI / 4) * Math.Cos(Math.PI / 4);
        var expectedY = Math.Cos(Math.PI / 4) * Math.Sin(Math.PI / 4);
        var expectedZ = Math.Sin(Math.PI / 4);

        Assert.That(value1.X.ScalarValue, Is.EqualTo(expectedX).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(expectedY).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(expectedZ).Within(Tolerance));
    }

    [Test]
    public void SphericalPath3D_RotatingPhi_ShouldCircleInXYPlane()
    {
        // r = 1, theta = 0 (xy-plane), phi(t) = 2*pi*t
        // Circular path in xy-plane

        var rCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        var thetaCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        Func<Scalar<double>, Scalar<double>> phiFunc = t => ScalarProcessor.PiTimes2 * t;
        var phiCurve = ComputedScalarSignal<double>.Finite(ScalarProcessor, phiFunc);

        var path = SphericalPath3D<double>.Finite(
            ScalarRange<double>.SymmetricOne(ScalarProcessor),
            rCurve,
            thetaCurve,
            phiCurve
        );

        // At t=0: phi=0 => (1, 0, 0)
        var value0 = path.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        // At t=0.25: phi=pi/2 => (0, 1, 0)
        var value025 = path.GetValue(ScalarProcessor.ScalarFromNumber(0.25));
        Assert.That(value025.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value025.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(value025.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        // At t=0.5: phi=pi => (-1, 0, 0)
        var value05 = path.GetValue(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(value05.X.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance));
        Assert.That(value05.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value05.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void SphericalPath3D_IsValid_ShouldReturnTrue()
    {
        var rCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        var thetaCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var phiCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var path = SphericalPath3D<double>.Finite(
            ScalarRange<double>.SymmetricOne(ScalarProcessor),
            rCurve,
            thetaCurve,
            phiCurve
        );

        Assert.That(path.IsValid(), Is.True);
    }

    [Test]
    public void SphericalPath3D_ToFinitePath_WhenFinite_ShouldReturnSelf()
    {
        var rCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        var thetaCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var phiCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var path = SphericalPath3D<double>.Finite(
            ScalarRange<double>.SymmetricOne(ScalarProcessor),
            rCurve,
            thetaCurve,
            phiCurve
        );

        var finitePath = path.ToFinitePath();

        Assert.That(finitePath, Is.SameAs(path));
        Assert.That(path.IsFinite, Is.True);
        Assert.That(path.IsPeriodic, Is.False);
    }

    [Test]
    public void SphericalPath3D_ToPeriodicPath_WhenFinite_ShouldReturnNewInstance()
    {
        var rCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.One
        );

        var thetaCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var phiCurve = ConstantScalarSignal<double>.Finite(
            ScalarProcessor,
            ScalarProcessor.Zero
        );

        var path = SphericalPath3D<double>.Finite(
            ScalarRange<double>.SymmetricOne(ScalarProcessor),
            rCurve,
            thetaCurve,
            phiCurve
        );

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
}
