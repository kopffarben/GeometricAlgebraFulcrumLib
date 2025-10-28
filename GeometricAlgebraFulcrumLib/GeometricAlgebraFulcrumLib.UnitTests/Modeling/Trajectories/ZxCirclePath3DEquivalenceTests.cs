using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Circles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Circles;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class ZxCirclePath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    [Test]
    public void TestZxCreate_Properties()
    {
        const double radius = 5.0;
        const int rotationCount = 1;

        var float64Path = new Float64ZxCirclePath3D(radius, rotationCount);
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius, rotationCount);

        Assert.That(genericPath.Radius.ScalarValue, Is.EqualTo(float64Path.Radius).Within(Tolerance));
        Assert.That(genericPath.RotationCount, Is.EqualTo(float64Path.RotationCount));

        Debug.Assert(genericPath.Radius.ScalarValue == float64Path.Radius);
    }

    [Test]
    public void TestZxGetLength()
    {
        const double radius = 2.5;

        var float64Path = new Float64ZxCirclePath3D(radius);
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Length = float64Path.GetLength().ScalarValue;
        var genericLength = genericPath.GetLength().ScalarValue;

        Assert.That(genericLength, Is.EqualTo(float64Length).Within(Tolerance));

        Debug.Assert(Math.Abs(genericLength - float64Length) < Tolerance);
    }

    [Test]
    public void TestZxGetValue_StartPoint()
    {
        const double radius = 4.0;
        const double t = 0.0;

        var float64Path = new Float64ZxCirclePath3D(radius);
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // At t=0 in ZX plane: (0, 0, radius)
        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Assert.That(Math.Abs(genericPoint.X.ScalarValue), Is.LessThan(Tolerance), "X should be ~0");
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y should be 0");
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(radius).Within(Tolerance), "Z should be radius");

        Debug.Assert(Math.Abs(genericPoint.Z.ScalarValue - radius) < Tolerance);
    }

    [Test]
    public void TestZxGetValue_QuarterCircle()
    {
        const double radius = 3.0;
        const double t = 0.25;

        var float64Path = new Float64ZxCirclePath3D(radius);
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // At t=0.25 in ZX plane: (radius, 0, ~0)
        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(radius).Within(Tolerance), "X should be radius");
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y should be 0");
        Assert.That(Math.Abs(genericPoint.Z.ScalarValue), Is.LessThan(Tolerance), "Z should be ~0");

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue - radius) < Tolerance);
    }

    [Test]
    public void TestZxGetDerivative1Value()
    {
        const double radius = 5.0;
        const double t = 0.0;

        var float64Path = new Float64ZxCirclePath3D(radius);
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Deriv = float64Path.GetDerivative1Value(t);
        var genericDeriv = genericPath.GetDerivative1Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericDeriv.X.ScalarValue, Is.EqualTo(float64Deriv.X.ScalarValue).Within(Tolerance));
        Assert.That(genericDeriv.Y.ScalarValue, Is.EqualTo(float64Deriv.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericDeriv.Z.ScalarValue, Is.EqualTo(float64Deriv.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericDeriv.X.ScalarValue - float64Deriv.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestZxGetDerivative2Value()
    {
        const double radius = 3.0;
        const double t = 0.0;

        var float64Path = new Float64ZxCirclePath3D(radius);
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Accel = float64Path.GetDerivative2Value(t);
        var genericAccel = genericPath.GetDerivative2Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericAccel.X.ScalarValue, Is.EqualTo(float64Accel.X.ScalarValue).Within(Tolerance));
        Assert.That(genericAccel.Y.ScalarValue, Is.EqualTo(float64Accel.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericAccel.Z.ScalarValue, Is.EqualTo(float64Accel.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericAccel.X.ScalarValue - float64Accel.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestZxCenter_IsOrigin()
    {
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, 3.0);

        Assert.That(genericPath.Center.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(genericPath.Center.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(genericPath.Center.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        Debug.Assert(genericPath.Center.IsZero());
    }

    [Test]
    public void TestZxUnitNormal_PointsInY()
    {
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, 2.0);

        // ZX circle has normal in +Y direction
        Assert.That(genericPath.UnitNormal.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Normal X should be 0");
        Assert.That(genericPath.UnitNormal.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Normal Y should be 1");
        Assert.That(genericPath.UnitNormal.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Normal Z should be 0");

        Debug.Assert(Math.Abs(genericPath.UnitNormal.Y.ScalarValue - 1.0) < Tolerance);
    }

    [Test]
    public void TestZxTimeToLength()
    {
        const double radius = 2.0;
        const double t = 0.5;

        var float64Path = new Float64ZxCirclePath3D(radius);
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Length = float64Path.TimeToLength(t).ScalarValue;
        var genericLength = genericPath.TimeToLength(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t)).ScalarValue;

        Assert.That(genericLength, Is.EqualTo(float64Length).Within(Tolerance));

        Debug.Assert(Math.Abs(genericLength - float64Length) < Tolerance);
    }

    [Test]
    public void TestZxLengthToTime()
    {
        const double radius = 1.5;

        var float64Path = new Float64ZxCirclePath3D(radius);
        var genericPath = ZxCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var totalLength = 2.0 * Math.PI * radius;
        var float64Time = float64Path.LengthToTime(totalLength).ScalarValue;
        var genericTime = genericPath.LengthToTime(ScalarProcessorOfFloat64.Instance.ScalarFromValue(totalLength)).ScalarValue;

        Assert.That(genericTime, Is.EqualTo(float64Time).Within(Tolerance));
        Assert.That(genericTime, Is.EqualTo(1.0).Within(Tolerance));

        Debug.Assert(Math.Abs(genericTime - 1.0) < Tolerance);
    }
}
