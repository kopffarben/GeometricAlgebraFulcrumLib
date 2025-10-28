using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Basis;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Circles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Circles;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class CirclePath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    [Test]
    public void TestCreate_Properties_XYPlane()
    {
        const double radius = 5.0;
        const int rotationCount = 1;

        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var normal = LinFloat64Vector3D.E3;  // +Z normal (XY plane)

        var float64Path = new Float64CirclePath3D(center, normal, radius, rotationCount);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
            ),
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(0.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(0.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0)
            ),
            radius,
            rotationCount
        );

        Assert.That(genericPath.Radius.ScalarValue, Is.EqualTo(float64Path.Radius).Within(Tolerance));
        Assert.That(genericPath.RotationCount, Is.EqualTo(float64Path.RotationCount));

        Assert.That(genericPath.Center.X.ScalarValue, Is.EqualTo(center.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPath.Center.Y.ScalarValue, Is.EqualTo(center.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPath.Center.Z.ScalarValue, Is.EqualTo(center.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(genericPath.Radius.ScalarValue == float64Path.Radius);
    }

    [Test]
    public void TestCreate_Properties_YZPlane()
    {
        const double radius = 3.0;

        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var normal = LinFloat64Vector3D.E1;  // +X normal (YZ plane)

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.E1(ScalarProcessorOfFloat64.Instance),
            radius
        );

        Assert.That(genericPath.Radius.ScalarValue, Is.EqualTo(float64Path.Radius).Within(Tolerance));
        Assert.That(genericPath.UnitNormal.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPath.UnitNormal.X.ScalarValue - 1.0) < Tolerance);
    }

    [Test]
    public void TestCreate_Properties_ZXPlane()
    {
        const double radius = 4.0;

        var center = LinFloat64Vector3D.Create(1, 1, 1);
        var normal = LinFloat64Vector3D.E2;  // +Y normal (ZX plane)

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0)
            ),
            LinVector3D<double>.E2(ScalarProcessorOfFloat64.Instance),
            radius
        );

        Assert.That(genericPath.Radius.ScalarValue, Is.EqualTo(float64Path.Radius).Within(Tolerance));
        Assert.That(genericPath.UnitNormal.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPath.UnitNormal.Y.ScalarValue - 1.0) < Tolerance);
    }

    [Test]
    public void TestGetLength()
    {
        const double radius = 2.5;

        var center = LinFloat64Vector3D.Zero;
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var float64Length = float64Path.GetLength().ScalarValue;
        var genericLength = genericPath.GetLength().ScalarValue;

        Assert.That(genericLength, Is.EqualTo(float64Length).Within(Tolerance));
        Assert.That(genericLength, Is.EqualTo(2 * Math.PI * radius).Within(Tolerance));

        Debug.Assert(Math.Abs(genericLength - float64Length) < Tolerance);
    }

    [Test]
    public void TestGetValue_StartPoint()
    {
        const double radius = 4.0;
        const double t = 0.0;

        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(1.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(2.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(3.0)
            ),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue - float64Point.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestGetValue_QuarterCircle()
    {
        const double radius = 3.0;
        const double t = 0.25;

        var center = LinFloat64Vector3D.Zero;
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue - float64Point.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestGetValue_HalfCircle()
    {
        const double radius = 2.0;
        const double t = 0.5;

        var center = LinFloat64Vector3D.Create(0, 0, 5);
        var normal = LinFloat64Vector3D.E2;  // +Y normal

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(0.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(0.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(5.0)
            ),
            LinVector3D<double>.E2(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPoint.Z.ScalarValue - float64Point.Z.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestGetDerivative1Value()
    {
        const double radius = 5.0;
        const double t = 0.0;

        var center = LinFloat64Vector3D.Zero;
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var float64Deriv = float64Path.GetDerivative1Value(t);
        var genericDeriv = genericPath.GetDerivative1Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericDeriv.X.ScalarValue, Is.EqualTo(float64Deriv.X.ScalarValue).Within(Tolerance));
        Assert.That(genericDeriv.Y.ScalarValue, Is.EqualTo(float64Deriv.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericDeriv.Z.ScalarValue, Is.EqualTo(float64Deriv.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericDeriv.X.ScalarValue - float64Deriv.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestGetDerivative2Value()
    {
        const double radius = 3.0;
        const double t = 0.0;

        var center = LinFloat64Vector3D.Zero;
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var float64Accel = float64Path.GetDerivative2Value(t);
        var genericAccel = genericPath.GetDerivative2Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericAccel.X.ScalarValue, Is.EqualTo(float64Accel.X.ScalarValue).Within(Tolerance));
        Assert.That(genericAccel.Y.ScalarValue, Is.EqualTo(float64Accel.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericAccel.Z.ScalarValue, Is.EqualTo(float64Accel.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericAccel.X.ScalarValue - float64Accel.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestTimeToLength()
    {
        const double radius = 2.0;
        const double t = 0.5;

        var center = LinFloat64Vector3D.Zero;
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var float64Length = float64Path.TimeToLength(t).ScalarValue;
        var genericLength = genericPath.TimeToLength(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t)).ScalarValue;

        Assert.That(genericLength, Is.EqualTo(float64Length).Within(Tolerance));

        Debug.Assert(Math.Abs(genericLength - float64Length) < Tolerance);
    }

    [Test]
    public void TestLengthToTime()
    {
        const double radius = 1.5;

        var center = LinFloat64Vector3D.Zero;
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var totalLength = 2.0 * Math.PI * radius;
        var float64Time = float64Path.LengthToTime(totalLength).ScalarValue;
        var genericTime = genericPath.LengthToTime(ScalarProcessorOfFloat64.Instance.ScalarFromValue(totalLength)).ScalarValue;

        Assert.That(genericTime, Is.EqualTo(float64Time).Within(Tolerance));
        Assert.That(genericTime, Is.EqualTo(1.0).Within(Tolerance));

        Debug.Assert(Math.Abs(genericTime - 1.0) < Tolerance);
    }

    [Test]
    public void TestGetFrame()
    {
        const double radius = 3.0;
        const double t = 0.25;

        var center = LinFloat64Vector3D.Zero;
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var float64Frame = float64Path.GetFrame(t);
        var genericFrame = genericPath.GetFrame(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericFrame.TimeValue.ScalarValue, Is.EqualTo(t).Within(Tolerance));
        Assert.That(genericFrame.Point.X.ScalarValue, Is.EqualTo(float64Frame.Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericFrame.Point.Y.ScalarValue, Is.EqualTo(float64Frame.Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericFrame.Tangent.X.ScalarValue, Is.EqualTo(float64Frame.Tangent.X.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericFrame.TimeValue.ScalarValue - t) < Tolerance);
    }

    [Test]
    public void TestArbitraryNormal_Diagonal()
    {
        const double radius = 2.0;
        const double t = 0.0;

        // Diagonal unit normal (not axis-aligned)
        var normalVector = LinFloat64Vector3D.Create(1, 1, 1).ToUnitLinVector3D();
        var center = LinFloat64Vector3D.Zero;

        var float64Path = new Float64CirclePath3D(center, normalVector, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(normalVector.X.ScalarValue),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(normalVector.Y.ScalarValue),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(normalVector.Z.ScalarValue)
            ),
            radius
        );

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue - float64Point.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestCenter_NotAtOrigin()
    {
        const double radius = 3.0;
        const double t = 0.0;

        var center = LinFloat64Vector3D.Create(10, 20, 30);
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Create(
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(10.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(20.0),
                ScalarProcessorOfFloat64.Instance.ScalarFromValue(30.0)
            ),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius
        );

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // Point should be offset from origin by center
        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue - float64Point.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void TestMultipleRotations()
    {
        const double radius = 2.0;
        const int rotationCount = 3;

        var center = LinFloat64Vector3D.Zero;
        var normal = LinFloat64Vector3D.E3;

        var float64Path = new Float64CirclePath3D(center, normal, radius, rotationCount);
        var genericPath = CirclePath3D<double>.Create(
            ScalarProcessorOfFloat64.Instance,
            LinVector3D<double>.Zero(ScalarProcessorOfFloat64.Instance),
            LinVector3D<double>.E3(ScalarProcessorOfFloat64.Instance),
            radius,
            rotationCount
        );

        var float64Length = float64Path.GetLength().ScalarValue;
        var genericLength = genericPath.GetLength().ScalarValue;

        Assert.That(genericLength, Is.EqualTo(float64Length).Within(Tolerance));
        Assert.That(genericLength, Is.EqualTo(2 * Math.PI * radius * rotationCount).Within(Tolerance));

        Debug.Assert(Math.Abs(genericLength - float64Length) < Tolerance);
    }
}
