using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Circles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Circles;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class XyCirclePath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    [Test]
    public void TestCreate_DefaultRotationCount()
    {
        // Arrange
        const double radius = 5.0;
        const int rotationCount = 1;

        // Act - Create Float64 version
        var float64Path = new Float64XyCirclePath3D(radius, rotationCount);

        // Act - Create Generic version
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var genericPath = XyCirclePath3D<double>.Create(scalarProcessor, radius, rotationCount);

        // Assert - Properties match
        Assert.That(genericPath.Radius.ScalarValue, Is.EqualTo(float64Path.Radius).Within(Tolerance),
            "Radius should match");
        Assert.That(genericPath.RotationCount, Is.EqualTo(float64Path.RotationCount),
            "RotationCount should match");
        Assert.That(genericPath.IsPeriodic, Is.EqualTo(float64Path.IsPeriodic),
            "IsPeriodic should match");

        Debug.Assert(genericPath.Radius.ScalarValue == float64Path.Radius);
    }

    [Test]
    public void TestCreate_MultipleRotations()
    {
        // Arrange
        const double radius = 3.0;
        const int rotationCount = 5;

        // Act
        var float64Path = new Float64XyCirclePath3D(radius, rotationCount);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius, rotationCount);

        // Assert
        Assert.That(genericPath.Radius.ScalarValue, Is.EqualTo(float64Path.Radius).Within(Tolerance));
        Assert.That(genericPath.RotationCount, Is.EqualTo(float64Path.RotationCount));

        Debug.Assert(genericPath.RotationCount == rotationCount);
    }

    [Test]
    public void TestGetLength_SingleRotation()
    {
        // Arrange
        const double radius = 2.5;
        const int rotationCount = 1;

        // Act
        var float64Path = new Float64XyCirclePath3D(radius, rotationCount);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius, rotationCount);

        var float64Length = float64Path.GetLength().ScalarValue;
        var genericLength = genericPath.GetLength().ScalarValue;

        // Assert - Length = 2π * radius for one rotation
        var expectedLength = 2.0 * Math.PI * radius;
        Assert.That(genericLength, Is.EqualTo(float64Length).Within(Tolerance),
            $"Generic length {genericLength} should match Float64 length {float64Length}");
        Assert.That(genericLength, Is.EqualTo(expectedLength).Within(Tolerance),
            $"Length should be 2π*radius = {expectedLength}");

        Debug.Assert(Math.Abs(genericLength - float64Length) < Tolerance);
    }

    [Test]
    public void TestGetLength_MultipleRotations()
    {
        // Arrange
        const double radius = 1.0;
        const int rotationCount = 3;

        // Act
        var float64Path = new Float64XyCirclePath3D(radius, rotationCount);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius, rotationCount);

        var float64Length = float64Path.GetLength().ScalarValue;
        var genericLength = genericPath.GetLength().ScalarValue;

        // Assert - Length = 2π * radius * rotationCount
        var expectedLength = 2.0 * Math.PI * radius * rotationCount;
        Assert.That(genericLength, Is.EqualTo(float64Length).Within(Tolerance),
            $"Lengths should match: Generic={genericLength}, Float64={float64Length}");
        Assert.That(genericLength, Is.EqualTo(expectedLength).Within(Tolerance),
            $"Length should be 2π*r*n = {expectedLength}");

        Debug.Assert(Math.Abs(genericLength - expectedLength) < Tolerance);
    }

    [Test]
    public void TestGetValue_StartPoint()
    {
        // Arrange
        const double radius = 4.0;
        const double t = 0.0;

        // Act
        var float64Path = new Float64XyCirclePath3D(radius);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // Assert - At t=0, point should be (radius, 0, 0)
        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance),
            $"X coordinates should match at t=0");
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance),
            $"Y coordinates should match at t=0");
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance),
            $"Z coordinates should match at t=0");

        // Expected: (radius, 0, 0)
        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(radius).Within(Tolerance),
            $"At t=0, x should be radius={radius}");
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            $"At t=0, y should be 0");
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            $"At t=0, z should be 0");

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue - radius) < Tolerance);
    }

    [Test]
    public void TestGetValue_QuarterCircle()
    {
        // Arrange
        const double radius = 3.0;
        const double t = 0.25; // Quarter rotation

        // Act
        var float64Path = new Float64XyCirclePath3D(radius);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // Assert
        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance),
            $"X coordinates should match at t={t}");
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance),
            $"Y coordinates should match at t={t}");
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance),
            $"Z coordinates should match at t={t}");

        // At t=0.25, angle = π/2, so point should be near (0, radius, 0)
        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            $"At t=0.25, x should be ~0");
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(radius).Within(Tolerance),
            $"At t=0.25, y should be radius={radius}");

        Debug.Assert(Math.Abs(genericPoint.Y.ScalarValue - radius) < Tolerance);
    }

    [Test]
    public void TestGetValue_HalfCircle()
    {
        // Arrange
        const double radius = 2.0;
        const double t = 0.5; // Half rotation

        // Act
        var float64Path = new Float64XyCirclePath3D(radius);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Point = float64Path.GetValue(t);
        var genericPoint = genericPath.GetValue(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // Assert - At t=0.5, angle = π, so point should be (-radius, 0, 0)
        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(float64Point.X.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(float64Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(float64Point.Z.ScalarValue).Within(Tolerance));

        Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(-radius).Within(Tolerance),
            $"At t=0.5, x should be -radius={-radius}");
        Assert.That(Math.Abs(genericPoint.Y.ScalarValue), Is.LessThan(Tolerance),
            $"At t=0.5, y should be ~0");

        Debug.Assert(Math.Abs(genericPoint.X.ScalarValue + radius) < Tolerance);
    }

    [Test]
    public void TestGetDerivative1Value_StartPoint()
    {
        // Arrange
        const double radius = 5.0;
        const double t = 0.0;

        // Act
        var float64Path = new Float64XyCirclePath3D(radius);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Deriv = float64Path.GetDerivative1Value(t);
        var genericDeriv = genericPath.GetDerivative1Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // Assert - First derivative (velocity) should match
        Assert.That(genericDeriv.X.ScalarValue, Is.EqualTo(float64Deriv.X.ScalarValue).Within(Tolerance),
            $"Derivative X should match at t=0");
        Assert.That(genericDeriv.Y.ScalarValue, Is.EqualTo(float64Deriv.Y.ScalarValue).Within(Tolerance),
            $"Derivative Y should match at t=0");
        Assert.That(genericDeriv.Z.ScalarValue, Is.EqualTo(float64Deriv.Z.ScalarValue).Within(Tolerance),
            $"Derivative Z should match at t=0");

        // At t=0, velocity should be tangent: (0, radius*2π, 0) for one rotation
        var expectedMagnitude = radius * 2.0 * Math.PI;
        Assert.That(Math.Abs(genericDeriv.X.ScalarValue), Is.LessThan(Tolerance),
            "At t=0, dx/dt should be ~0");
        Assert.That(genericDeriv.Y.ScalarValue, Is.EqualTo(expectedMagnitude).Within(Tolerance),
            $"At t=0, dy/dt should be radius*2π = {expectedMagnitude}");

        Debug.Assert(Math.Abs(genericDeriv.Y.ScalarValue - expectedMagnitude) < Tolerance);
    }

    [Test]
    public void TestGetDerivative2Value_StartPoint()
    {
        // Arrange
        const double radius = 3.0;
        const double t = 0.0;

        // Act
        var float64Path = new Float64XyCirclePath3D(radius);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Accel = float64Path.GetDerivative2Value(t);
        var genericAccel = genericPath.GetDerivative2Value(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // Assert - Second derivative (acceleration) should match
        Assert.That(genericAccel.X.ScalarValue, Is.EqualTo(float64Accel.X.ScalarValue).Within(Tolerance));
        Assert.That(genericAccel.Y.ScalarValue, Is.EqualTo(float64Accel.Y.ScalarValue).Within(Tolerance));
        Assert.That(genericAccel.Z.ScalarValue, Is.EqualTo(float64Accel.Z.ScalarValue).Within(Tolerance));

        // At t=0, acceleration points toward center: (-radius*(2π)², 0, 0)
        var expectedMagnitude = -radius * Math.Pow(2.0 * Math.PI, 2);
        Assert.That(genericAccel.X.ScalarValue, Is.EqualTo(expectedMagnitude).Within(Tolerance),
            $"At t=0, d²x/dt² should be -radius*(2π)² = {expectedMagnitude}");
        Assert.That(Math.Abs(genericAccel.Y.ScalarValue), Is.LessThan(Tolerance),
            "At t=0, d²y/dt² should be ~0");

        Debug.Assert(Math.Abs(genericAccel.X.ScalarValue - expectedMagnitude) < Tolerance);
    }

    [Test]
    public void TestTimeToLength_HalfWay()
    {
        // Arrange
        const double radius = 2.0;
        const double t = 0.5;

        // Act
        var float64Path = new Float64XyCirclePath3D(radius);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Length = float64Path.TimeToLength(t).ScalarValue;
        var genericLength = genericPath.TimeToLength(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t)).ScalarValue;

        // Assert - At t=0.5, length should be half the total circumference
        var expectedLength = Math.PI * radius; // Half of 2π*radius
        Assert.That(genericLength, Is.EqualTo(float64Length).Within(Tolerance),
            $"Lengths should match: Generic={genericLength}, Float64={float64Length}");
        Assert.That(genericLength, Is.EqualTo(expectedLength).Within(Tolerance),
            $"At t=0.5, length should be πr = {expectedLength}");

        Debug.Assert(Math.Abs(genericLength - expectedLength) < Tolerance);
    }

    [Test]
    public void TestLengthToTime_FullCircle()
    {
        // Arrange
        const double radius = 1.5;

        // Act
        var float64Path = new Float64XyCirclePath3D(radius);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var totalLength = 2.0 * Math.PI * radius;
        var float64Time = float64Path.LengthToTime(totalLength).ScalarValue;
        var genericTime = genericPath.LengthToTime(ScalarProcessorOfFloat64.Instance.ScalarFromValue(totalLength)).ScalarValue;

        // Assert - Full circumference should correspond to t=1.0
        Assert.That(genericTime, Is.EqualTo(float64Time).Within(Tolerance),
            $"Times should match: Generic={genericTime}, Float64={float64Time}");
        Assert.That(genericTime, Is.EqualTo(1.0).Within(Tolerance),
            $"Full length should map to t=1.0");

        Debug.Assert(Math.Abs(genericTime - 1.0) < Tolerance);
    }

    [Test]
    public void TestIsValid_ValidCircle()
    {
        // Arrange & Act
        var float64Path = new Float64XyCirclePath3D(5.0);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, 5.0);

        // Assert
        Assert.That(genericPath.IsValid(), Is.EqualTo(float64Path.IsValid()),
            "IsValid should match between implementations");
        Assert.That(genericPath.IsValid(), Is.True,
            "Circle with positive radius should be valid");

        Debug.Assert(genericPath.IsValid());
    }

    [Test]
    public void TestCenter_IsOrigin()
    {
        // Arrange & Act
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, 3.0);

        // Assert - XY circle is always centered at origin
        Assert.That(genericPath.Center.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Center X should be 0");
        Assert.That(genericPath.Center.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Center Y should be 0");
        Assert.That(genericPath.Center.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Center Z should be 0");

        Debug.Assert(genericPath.Center.IsZero());
    }

    [Test]
    public void TestUnitNormal_PointsInZ()
    {
        // Arrange & Act
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, 2.0);

        // Assert - XY circle has normal in +Z direction
        Assert.That(genericPath.UnitNormal.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Normal X should be 0");
        Assert.That(genericPath.UnitNormal.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Normal Y should be 0");
        Assert.That(genericPath.UnitNormal.Z.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Normal Z should be 1 (pointing up)");

        Debug.Assert(Math.Abs(genericPath.UnitNormal.Z.ScalarValue - 1.0) < Tolerance);
    }

    [Test]
    public void TestGetFrame_ReturnsValidFrame()
    {
        // Arrange
        const double radius = 4.0;
        const double t = 0.25;

        // Act
        var float64Path = new Float64XyCirclePath3D(radius);
        var genericPath = XyCirclePath3D<double>.Create(ScalarProcessorOfFloat64.Instance, radius);

        var float64Frame = float64Path.GetFrame(t);
        var genericFrame = genericPath.GetFrame(ScalarProcessorOfFloat64.Instance.ScalarFromValue(t));

        // Assert - Frame components should match
        Assert.That(genericFrame.TimeValue.ScalarValue, Is.EqualTo(float64Frame.TimeValue).Within(Tolerance),
            "Frame time values should match");

        // Tangent should be normalized
        var tangentNorm = genericFrame.Tangent.Norm().ScalarValue;
        Assert.That(tangentNorm, Is.EqualTo(1.0).Within(Tolerance),
            $"Tangent should be unit length, got {tangentNorm}");

        Debug.Assert(Math.Abs(tangentNorm - 1.0) < Tolerance);
    }
}
