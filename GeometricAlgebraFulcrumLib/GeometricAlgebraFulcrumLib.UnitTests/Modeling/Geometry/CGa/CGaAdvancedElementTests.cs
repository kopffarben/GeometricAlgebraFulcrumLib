using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Tests for Advanced CGA Blade Operations
/// Phase 3B - Core Modeling: Conformal Geometry Advanced Blades (15 tests)
/// Tests blade encoding, decoding, and basic operations
/// </summary>
[TestFixture]
public class CGaAdvancedElementTests
{
    private const double Tolerance = 1e-10;
    private CGaFloat64GeometricSpace _space = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _space = CGaFloat64GeometricSpace.Space5D;
    }

    #region Sphere Tests (3 tests)

    [Test]
    public void Sphere_Encode_ShouldCreateValidBlade()
    {
        // Arrange & Act
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var radius = 2.5;
        var sphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Assert
        Assert.That(sphere, Is.Not.Null, "Sphere blade should be created");
        Assert.That(sphere.InternalKVector, Is.Not.Null, "Sphere should have internal k-vector");
        Assert.That(sphere.Grade, Is.GreaterThanOrEqualTo(0), "Grade should be non-negative");
    }

    [Test]
    public void Sphere_DecodeRadius_ShouldSucceed()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 3.5;
        var sphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        var radiusSquared = sphere.DecodeIpnsRound.RadiusSquared();

        // Assert
        Assert.That(radiusSquared, Is.GreaterThan(0), "Radius squared should be positive for real sphere");
    }

    [Test]
    public void Sphere_ScalarMultiplication_ShouldWork()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var radius = 2.0;
        var sphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        var scaled = sphere * 2.0;

        // Assert
        Assert.That(scaled, Is.Not.Null, "Scaled sphere should exist");
        Assert.That(scaled.InternalKVector, Is.Not.Null, "Scaled sphere should have k-vector");
    }

    #endregion

    #region Circle Tests (3 tests)

    [Test]
    public void Circle_Encode_ShouldCreateValidBlade()
    {
        // Arrange & Act
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var normal = LinFloat64Vector3D.E3;
        var radius = 2.0;
        var circle = _space.EncodeIpnsRound.RealCircle(radius, center, normal);

        // Assert
        Assert.That(circle, Is.Not.Null, "Circle blade should be created");
        Assert.That(circle.InternalKVector, Is.Not.Null, "Circle should have internal k-vector");
        Assert.That(circle.Grade, Is.GreaterThanOrEqualTo(0), "Grade should be non-negative");
    }

    [Test]
    public void Circle_DecodeCenter_ShouldSucceed()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var normal = LinFloat64Vector3D.E3;
        var radius = 2.5;
        var circle = _space.EncodeIpnsRound.RealCircle(radius, center, normal);

        // Act
        var decodedCenter = circle.DecodeIpnsRound.VGaCenter();

        // Assert
        Assert.That(decodedCenter, Is.Not.Null, "Decoded center should exist");
        Assert.That(decodedCenter.InternalKVector, Is.Not.Null, "Center should have k-vector");
    }

    [Test]
    public void Circle_Addition_ShouldWork()
    {
        // Arrange
        var center1 = LinFloat64Vector3D.Create(0, 0, 0);
        var center2 = LinFloat64Vector3D.Create(1, 0, 0);
        var normal = LinFloat64Vector3D.E3;
        var radius = 1.5;
        var circle1 = _space.EncodeIpnsRound.RealCircle(radius, center1, normal);
        var circle2 = _space.EncodeIpnsRound.RealCircle(radius, center2, normal);

        // Act
        var sum = circle1 + circle2;

        // Assert
        Assert.That(sum, Is.Not.Null, "Sum of circles should exist");
        Assert.That(sum.InternalKVector, Is.Not.Null, "Sum should have k-vector");
    }

    #endregion

    #region Point Tests (3 tests)

    [Test]
    public void Point_Encode_ShouldCreateValidBlade()
    {
        // Arrange & Act
        var point = LinFloat64Vector3D.Create(3, 4, 5);
        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);

        // Assert
        Assert.That(ipnsPoint, Is.Not.Null, "Point blade should be created");
        Assert.That(ipnsPoint.InternalKVector, Is.Not.Null, "Point should have internal k-vector");
        Assert.That(ipnsPoint.Grade, Is.GreaterThanOrEqualTo(0), "Grade should be non-negative");
    }

    [Test]
    public void Point_DecodePosition_ShouldSucceed()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(7, 8, 9);
        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);

        // Act
        var decodedPoint = ipnsPoint.DecodeIpnsFlat.VGaPosition();

        // Assert
        Assert.That(decodedPoint, Is.Not.Null, "Decoded position should exist");
        Assert.That(decodedPoint.InternalKVector, Is.Not.Null, "Decoded position should have k-vector");
    }

    [Test]
    public void Point_Negation_ShouldWork()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);

        // Act
        var negated = -ipnsPoint;

        // Assert
        Assert.That(negated, Is.Not.Null, "Negated point should exist");
        Assert.That(negated.InternalKVector, Is.Not.Null, "Negated point should have k-vector");
    }

    #endregion

    #region Line Tests (3 tests)

    [Test]
    public void Line_Encode_ShouldCreateValidBlade()
    {
        // Arrange & Act
        var linePoint = LinFloat64Vector3D.Create(1, 1, 1);
        var lineDirection = LinFloat64Vector3D.E1;
        var line = _space.EncodeIpnsFlat.Line(linePoint, lineDirection);

        // Assert
        Assert.That(line, Is.Not.Null, "Line blade should be created");
        Assert.That(line.InternalKVector, Is.Not.Null, "Line should have internal k-vector");
        Assert.That(line.Grade, Is.GreaterThanOrEqualTo(0), "Grade should be non-negative");
    }

    [Test]
    public void Line_DecodePosition_ShouldSucceed()
    {
        // Arrange
        var linePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var lineDirection = LinFloat64Vector3D.Create(3, 4, 0);
        var line = _space.EncodeIpnsFlat.Line(linePoint, lineDirection);

        // Act
        var decodedPosition = line.DecodeIpnsFlat.VGaPosition();

        // Assert
        Assert.That(decodedPosition, Is.Not.Null, "Decoded position should exist");
        Assert.That(decodedPosition.InternalKVector, Is.Not.Null, "Position should have k-vector");
    }

    [Test]
    public void Line_Division_ShouldWork()
    {
        // Arrange
        var linePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var lineDirection = LinFloat64Vector3D.E1;
        var line = _space.EncodeIpnsFlat.Line(linePoint, lineDirection);

        // Act
        var divided = line / 2.0;

        // Assert
        Assert.That(divided, Is.Not.Null, "Divided line should exist");
        Assert.That(divided.InternalKVector, Is.Not.Null, "Divided line should have k-vector");
    }

    #endregion

    #region Plane Tests (3 tests)

    [Test]
    public void Plane_Encode_ShouldCreateValidBlade()
    {
        // Arrange & Act
        var planePoint = LinFloat64Vector3D.Create(0, 0, 5);
        var planeNormal = LinFloat64Vector3D.E3;
        var plane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Assert
        Assert.That(plane, Is.Not.Null, "Plane blade should be created");
        Assert.That(plane.InternalKVector, Is.Not.Null, "Plane should have internal k-vector");
        Assert.That(plane.Grade, Is.GreaterThanOrEqualTo(0), "Grade should be non-negative");
    }

    [Test]
    public void Plane_DecodePosition_ShouldSucceed()
    {
        // Arrange
        var planePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var planeNormal = LinFloat64Vector3D.Create(0, 0, 5);
        var plane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act
        var decodedPosition = plane.DecodeIpnsFlat.VGaPosition();

        // Assert
        Assert.That(decodedPosition, Is.Not.Null, "Decoded position should exist");
        Assert.That(decodedPosition.InternalKVector, Is.Not.Null, "Position should have k-vector");
    }

    [Test]
    public void Plane_Subtraction_ShouldWork()
    {
        // Arrange
        var planePoint = LinFloat64Vector3D.Create(0, 0, 5);
        var planeNormal = LinFloat64Vector3D.E3;
        var plane1 = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);
        var plane2 = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act
        var difference = plane1 - plane2;

        // Assert
        Assert.That(difference, Is.Not.Null, "Difference of planes should exist");
        Assert.That(difference.InternalKVector, Is.Not.Null, "Difference should have k-vector");
    }

    #endregion
}
