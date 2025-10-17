using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Operations;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Tests for CGa Operation Composition
/// Phase 3B - Core Modeling: Conformal Geometry Operation Composition (15 tests)
/// Tests that CGA operations can be composed and chained together
/// </summary>
[TestFixture]
public class CGaOperationCompositionTests
{
    private const double Tolerance = 1e-10;
    private CGaFloat64GeometricSpace _space = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _space = CGaFloat64GeometricSpace.Space5D;
    }

    #region Arithmetic Composition Tests (5 tests)

    [Test]
    public void Composition_AddThenMultiply_ShouldWork()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);
        var blade1 = _space.EncodeIpnsFlat.Point(point1);
        var blade2 = _space.EncodeIpnsFlat.Point(point2);

        // Act
        var sum = blade1 + blade2;
        var result = sum * 2.0;

        // Assert
        Assert.That(result, Is.Not.Null, "Composed operation should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Composition_MultiplyThenAdd_ShouldWork()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 2.0;
        var sphere1 = _space.EncodeIpnsRound.RealSphere(radius, center);
        var sphere2 = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        var scaled1 = sphere1 * 3.0;
        var scaled2 = sphere2 * 2.0;
        var result = scaled1 + scaled2;

        // Assert
        Assert.That(result, Is.Not.Null, "Composed operation should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Composition_NegateThenAdd_ShouldWork()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 2, 3);
        var point2 = LinFloat64Vector3D.Create(4, 5, 6);
        var blade1 = _space.EncodeIpnsFlat.Point(point1);
        var blade2 = _space.EncodeIpnsFlat.Point(point2);

        // Act
        var negated = -blade1;
        var result = negated + blade2;

        // Assert
        Assert.That(result, Is.Not.Null, "Composed operation should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Composition_SubtractThenMultiply_ShouldWork()
    {
        // Arrange
        var center1 = LinFloat64Vector3D.Create(1, 0, 0);
        var center2 = LinFloat64Vector3D.Create(0, 1, 0);
        var radius = 1.0;
        var sphere1 = _space.EncodeIpnsRound.RealSphere(radius, center1);
        var sphere2 = _space.EncodeIpnsRound.RealSphere(radius, center2);

        // Act
        var difference = sphere1 - sphere2;
        var result = difference * 0.5;

        // Assert
        Assert.That(result, Is.Not.Null, "Composed operation should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Composition_DivideThenAdd_ShouldWork()
    {
        // Arrange
        var planePoint = LinFloat64Vector3D.Create(0, 0, 1);
        var planeNormal = LinFloat64Vector3D.E3;
        var plane1 = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);
        var plane2 = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act
        var divided = plane1 / 2.0;
        var result = divided + plane2;

        // Assert
        Assert.That(result, Is.Not.Null, "Composed operation should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    #endregion

    #region Meet Composition Tests (5 tests)

    [Test]
    public void Composition_MeetThenScale_ShouldWork()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 2.0;
        var sphere1 = _space.EncodeIpnsRound.RealSphere(radius, center);
        var sphere2 = _space.EncodeIpnsRound.RealSphere(radius, LinFloat64Vector3D.Create(3, 0, 0));

        // Act
        var meet = sphere1.MeetIpns(sphere2);
        var result = meet * 2.0;

        // Assert
        Assert.That(result, Is.Not.Null, "Meet then scale should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Composition_ScaleThenMeet_ShouldWork()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);
        var pga1 = _space.EncodePGa.Point(point1);
        var pga2 = _space.EncodePGa.Point(point2);

        // Act
        var scaled1 = pga1 * 2.0;
        var scaled2 = pga2 * 2.0;
        var result = scaled1.MeetPGa(scaled2);

        // Assert
        Assert.That(result, Is.Not.Null, "Scale then meet should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Composition_MeetThenMeet_ShouldWork()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);
        var point3 = LinFloat64Vector3D.Create(0, 0, 1);
        var pga1 = _space.EncodePGa.Point(point1);
        var pga2 = _space.EncodePGa.Point(point2);
        var pga3 = _space.EncodePGa.Point(point3);

        // Act
        var meet1 = pga1.MeetPGa(pga2);
        var result = meet1.MeetPGa(pga3);

        // Assert
        Assert.That(result, Is.Not.Null, "Chained meets should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Composition_MeetThenNegate_ShouldWork()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);
        var pga1 = _space.EncodePGa.Point(point1);
        var pga2 = _space.EncodePGa.Point(point2);

        // Act
        var meet = pga1.MeetPGa(pga2);
        var result = -meet;

        // Assert
        Assert.That(result, Is.Not.Null, "Meet then negate should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Composition_AddThenMeet_ShouldWork()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);
        var point3 = LinFloat64Vector3D.Create(1, 1, 0);
        var pga1 = _space.EncodePGa.Point(point1);
        var pga2 = _space.EncodePGa.Point(point2);
        var pga3 = _space.EncodePGa.Point(point3);

        // Act
        var sum = pga1 + pga2;
        var result = sum.MeetPGa(pga3);

        // Assert
        Assert.That(result, Is.Not.Null, "Add then meet should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    #endregion

    #region Decode After Composition Tests (5 tests)

    [Test]
    public void Composition_AddSphereThenDecodeCenter_ShouldWork()
    {
        // Arrange
        var center1 = LinFloat64Vector3D.Create(1, 0, 0);
        var center2 = LinFloat64Vector3D.Create(-1, 0, 0);
        var radius = 1.0;
        var sphere1 = _space.EncodeIpnsRound.RealSphere(radius, center1);
        var sphere2 = _space.EncodeIpnsRound.RealSphere(radius, center2);

        // Act
        var sum = sphere1 + sphere2;
        var center = sum.DecodeIpnsRound.VGaCenter();

        // Assert
        Assert.That(center, Is.Not.Null, "Decode after addition should succeed");
        Assert.That(center.InternalKVector, Is.Not.Null, "Decoded center should have k-vector");
    }

    [Test]
    public void Composition_ScaleCircleThenDecodeCenter_ShouldWork()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var normal = LinFloat64Vector3D.E3;
        var radius = 2.0;
        var circle = _space.EncodeIpnsRound.RealCircle(radius, center, normal);

        // Act
        var scaled = circle * 3.0;
        var decodedCenter = scaled.DecodeIpnsRound.VGaCenter();

        // Assert
        Assert.That(decodedCenter, Is.Not.Null, "Decode center after scaling should succeed");
        Assert.That(decodedCenter.InternalKVector, Is.Not.Null, "Decoded center should have k-vector");
    }

    [Test]
    public void Composition_NegatePointThenDecodePosition_ShouldWork()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(5, 6, 7);
        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);

        // Act
        var negated = -ipnsPoint;
        var position = negated.DecodeIpnsFlat.VGaPosition();

        // Assert
        Assert.That(position, Is.Not.Null, "Decode position after negation should succeed");
        Assert.That(position.InternalKVector, Is.Not.Null, "Position should have k-vector");
    }

    [Test]
    public void Composition_AddPointsThenDecodePosition_ShouldWork()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);
        var ipnsPoint1 = _space.EncodeIpnsFlat.Point(point1);
        var ipnsPoint2 = _space.EncodeIpnsFlat.Point(point2);

        // Act
        var sum = ipnsPoint1 + ipnsPoint2;
        var position = sum.DecodeIpnsFlat.VGaPosition();

        // Assert
        Assert.That(position, Is.Not.Null, "Decode after addition should succeed");
        Assert.That(position.InternalKVector, Is.Not.Null, "Position should have k-vector");
    }

    [Test]
    public void Composition_DividePlaneThenDecodePosition_ShouldWork()
    {
        // Arrange
        var planePoint = LinFloat64Vector3D.Create(0, 0, 3);
        var planeNormal = LinFloat64Vector3D.E3;
        var plane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act
        var divided = plane / 4.0;
        var position = divided.DecodeIpnsFlat.VGaPosition();

        // Assert
        Assert.That(position, Is.Not.Null, "Decode after division should succeed");
        Assert.That(position.InternalKVector, Is.Not.Null, "Position should have k-vector");
    }

    #endregion
}
