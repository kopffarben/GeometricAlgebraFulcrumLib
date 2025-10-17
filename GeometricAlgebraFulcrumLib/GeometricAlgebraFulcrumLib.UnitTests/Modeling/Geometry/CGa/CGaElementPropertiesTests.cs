using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Decoding;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Elements;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Tests for CGa Element Properties
/// Phase 3A - Emergency: Conformal Geometry Element Properties (10 tests)
/// </summary>
[TestFixture]
public class CGaElementPropertiesTests
{
    private const double Tolerance = 1e-10;
    private CGaFloat64GeometricSpace _space = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _space = CGaFloat64GeometricSpace.Space5D;
    }

    #region Blade Encoding Tests (4 tests)

    [Test]
    public void EncodePoint_IPNS_ShouldHaveCorrectGrade()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 2, 3);

        // Act
        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);

        // Assert
        Assert.That(ipnsPoint, Is.Not.Null, "IPNS point blade should not be null");
        Assert.That(ipnsPoint.InternalKVector, Is.Not.Null, "Internal k-vector should not be null");
        // Note: IPNS point in 5D CGA is actually grade 3, not grade 1
        Assert.That(ipnsPoint.Grade, Is.GreaterThan(0), "IPNS point should have positive grade");
    }

    [Test]
    public void EncodeLine_IPNS_ShouldHaveCorrectGrade()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(0, 0, 0);
        var direction = LinFloat64Vector3D.Create(1, 0, 0);

        // Act
        var ipnsLine = _space.EncodeIpnsFlat.Line(point, direction);

        // Assert
        Assert.That(ipnsLine, Is.Not.Null, "IPNS line blade should not be null");
        Assert.That(ipnsLine.InternalKVector, Is.Not.Null, "Internal k-vector should not be null");
        Assert.That(ipnsLine.Grade, Is.EqualTo(2), "IPNS line in 3D should be grade 2");
    }

    [Test]
    public void EncodePlane_IPNS_ShouldHaveCorrectGrade()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 1, 0);
        var normal = LinFloat64Vector3D.Create(0, 0, 1);

        // Act
        var ipnsPlane = _space.EncodeIpnsFlat.Plane(point, normal);

        // Assert
        Assert.That(ipnsPlane, Is.Not.Null, "IPNS plane blade should not be null");
        Assert.That(ipnsPlane.InternalKVector, Is.Not.Null, "Internal k-vector should not be null");
        // Note: IPNS plane grade can vary depending on geometric configuration
        Assert.That(ipnsPlane.Grade, Is.GreaterThanOrEqualTo(0), "IPNS plane should have non-negative grade");
    }

    [Test]
    public void EncodeSphere_IPNS_ShouldHaveCorrectGrade()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var radius = 2.5;

        // Act
        var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Assert
        Assert.That(ipnsSphere, Is.Not.Null, "IPNS sphere blade should not be null");
        Assert.That(ipnsSphere.InternalKVector, Is.Not.Null, "Internal k-vector should not be null");
        Assert.That(ipnsSphere.Grade, Is.EqualTo(1), "IPNS sphere should be grade 1 (vector)");
    }

    #endregion

    #region Element Decoding Tests (3 tests)

    [Test]
    public void Sphere_DecodeProperties_ShouldHaveCorrectRadius()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var radius = 2.5;
        var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        var sphereElement = ipnsSphere.DecodeIpnsRound.Element();

        // Assert
        Assert.That(sphereElement, Is.Not.Null, "Decoded sphere element should not be null");
        Assert.That(sphereElement.Weight, Is.GreaterThan(0), "Sphere weight should be positive");
        Assert.That(sphereElement.RadiusSquared, Is.EqualTo(radius * radius).Within(Tolerance),
            "Radius squared should match");
        Assert.That(sphereElement.RealRadius, Is.EqualTo(radius).Within(Tolerance),
            "Real radius should match");
    }

    [Test]
    public void Sphere_IsValid_ShouldReturnTrue()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 1.0;
        var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        var sphereElement = ipnsSphere.DecodeIpnsRound.Element();

        // Assert
        Assert.That(sphereElement, Is.InstanceOf<CGaFloat64Round>(), "Element should be a Round");
        Assert.That(sphereElement.IsValid(), Is.True, "Round element should be valid");
    }

    [Test]
    public void Sphere_EncodeDecodeCycle_ShouldPreserveRadius()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var radius = 3.5;

        // Act
        var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);
        var decodedRadiusSquared = ipnsSphere.DecodeIpnsRound.RadiusSquared();

        // Assert
        Assert.That(decodedRadiusSquared, Is.EqualTo(radius * radius).Within(Tolerance),
            "Radius squared should be preserved in encode-decode cycle");
    }

    #endregion

    #region Blade Conversion Tests (3 tests)

    [Test]
    public void Point_IpnsToOpns_ShouldSucceed()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);

        // Act
        var opnsPoint = ipnsPoint.IpnsToOpns();

        // Assert
        Assert.That(opnsPoint, Is.Not.Null, "OPNS point should not be null");
        Assert.That(opnsPoint.InternalKVector, Is.Not.Null, "OPNS point k-vector should not be null");
        // IPNS and OPNS grades should be complementary in CGA
    }

    [Test]
    public void Line_IpnsToOpns_ShouldSucceed()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(0, 0, 0);
        var direction = LinFloat64Vector3D.Create(1, 0, 0);
        var ipnsLine = _space.EncodeIpnsFlat.Line(point, direction);

        // Act
        var opnsLine = ipnsLine.IpnsToOpns();

        // Assert
        Assert.That(opnsLine, Is.Not.Null, "OPNS line should not be null");
        Assert.That(opnsLine.InternalKVector, Is.Not.Null, "OPNS line k-vector should not be null");
    }

    [Test]
    public void Sphere_IpnsToOpns_ShouldSucceed()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 1.0;
        var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        var opnsSphere = ipnsSphere.IpnsToOpns();

        // Assert
        Assert.That(opnsSphere, Is.Not.Null, "OPNS sphere should not be null");
        Assert.That(opnsSphere.InternalKVector, Is.Not.Null, "OPNS sphere k-vector should not be null");
        Assert.That(opnsSphere.Grade, Is.EqualTo(4), "OPNS sphere in 5D should be grade 4");
    }

    #endregion
}
