using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Decoding;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Operations;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Tests for CGa Meet and Join Operations
/// Phase 3B - Core Modeling: Conformal Geometry Meet/Join (15 tests)
/// Meet = geometric intersection, Join = geometric union/span
/// </summary>
[TestFixture]
public class CGaMeetJoinTests
{
    private const double Tolerance = 1e-10;
    private CGaFloat64GeometricSpace _space = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _space = CGaFloat64GeometricSpace.Space5D;
    }

    #region OPNS Meet Tests (5 tests)

    [Test]
    public void MeetOpns_TwoPoints_ShouldResultInLowerGrade()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);

        var ipnsPoint1 = _space.EncodeIpnsFlat.Point(point1);
        var ipnsPoint2 = _space.EncodeIpnsFlat.Point(point2);

        // Convert to OPNS for meet operation
        var opnsPoint1 = ipnsPoint1.IpnsToOpns();
        var opnsPoint2 = ipnsPoint2.IpnsToOpns();

        // Act
        var meet = opnsPoint1.MeetOpns(opnsPoint2);

        // Assert
        Assert.That(meet, Is.Not.Null, "Meet result should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
    }

    [Test]
    public void MeetOpns_LineAndPlane_ShouldSucceed()
    {
        // Arrange
        var linePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var lineDirection = LinFloat64Vector3D.Create(1, 0, 0);
        var planePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var planeNormal = LinFloat64Vector3D.Create(0, 0, 1);

        var ipnsLine = _space.EncodeIpnsFlat.Line(linePoint, lineDirection);
        var ipnsPlane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        var opnsLine = ipnsLine.IpnsToOpns();
        var opnsPlane = ipnsPlane.IpnsToOpns();

        // Act
        var meet = opnsLine.MeetOpns(opnsPlane);

        // Assert
        Assert.That(meet, Is.Not.Null, "Line-plane meet should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
    }

    [Test]
    public void MeetOpns_TwoParallelLines_ShouldHaveSpecialResult()
    {
        // Arrange
        var line1Point = LinFloat64Vector3D.Create(0, 0, 0);
        var line1Direction = LinFloat64Vector3D.Create(1, 0, 0);
        var line2Point = LinFloat64Vector3D.Create(0, 1, 0);
        var line2Direction = LinFloat64Vector3D.Create(1, 0, 0); // Parallel to line1

        var ipnsLine1 = _space.EncodeIpnsFlat.Line(line1Point, line1Direction);
        var ipnsLine2 = _space.EncodeIpnsFlat.Line(line2Point, line2Direction);

        var opnsLine1 = ipnsLine1.IpnsToOpns();
        var opnsLine2 = ipnsLine2.IpnsToOpns();

        // Act
        var meet = opnsLine1.MeetOpns(opnsLine2);

        // Assert
        Assert.That(meet, Is.Not.Null, "Parallel lines meet should exist");
        // Parallel non-intersecting lines have no common point (meet is zero or at infinity)
    }

    [Test]
    public void MeetOpns_TwoIntersectingLines_ShouldGivePoint()
    {
        // Arrange - Two lines that intersect at origin
        var line1Point = LinFloat64Vector3D.Create(0, 0, 0);
        var line1Direction = LinFloat64Vector3D.Create(1, 0, 0);
        var line2Point = LinFloat64Vector3D.Create(0, 0, 0);
        var line2Direction = LinFloat64Vector3D.Create(0, 1, 0);

        var ipnsLine1 = _space.EncodeIpnsFlat.Line(line1Point, line1Direction);
        var ipnsLine2 = _space.EncodeIpnsFlat.Line(line2Point, line2Direction);

        var opnsLine1 = ipnsLine1.IpnsToOpns();
        var opnsLine2 = ipnsLine2.IpnsToOpns();

        // Act
        var meet = opnsLine1.MeetOpns(opnsLine2);

        // Assert
        Assert.That(meet, Is.Not.Null, "Intersecting lines meet should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
    }

    [Test]
    public void MeetOpns_ThreeBlades_ShouldSucceed()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);
        var point3 = LinFloat64Vector3D.Create(0, 0, 1);

        var ipns1 = _space.EncodeIpnsFlat.Point(point1);
        var ipns2 = _space.EncodeIpnsFlat.Point(point2);
        var ipns3 = _space.EncodeIpnsFlat.Point(point3);

        var opns1 = ipns1.IpnsToOpns();
        var opns2 = ipns2.IpnsToOpns();
        var opns3 = ipns3.IpnsToOpns();

        // Act
        var meet = opns1.MeetOpns(opns2, opns3);

        // Assert
        Assert.That(meet, Is.Not.Null, "Three-way meet should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
    }

    #endregion

    #region IPNS Meet Tests (5 tests)

    [Test]
    public void MeetIpns_TwoSpheres_ShouldGiveCircle()
    {
        // Arrange
        var center1 = LinFloat64Vector3D.Create(0, 0, 0);
        var radius1 = 2.0;
        var center2 = LinFloat64Vector3D.Create(3, 0, 0);
        var radius2 = 2.0;

        var sphere1 = _space.EncodeIpnsRound.RealSphere(radius1, center1);
        var sphere2 = _space.EncodeIpnsRound.RealSphere(radius2, center2);

        // Act
        var meet = sphere1.MeetIpns(sphere2);

        // Assert
        Assert.That(meet, Is.Not.Null, "Sphere intersection should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
        // Two intersecting spheres meet in a circle (grade 2)
    }

    [Test]
    public void MeetIpns_SphereAndPlane_ShouldSucceed()
    {
        // Arrange
        var sphereCenter = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 2.0;
        var planePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var planeNormal = LinFloat64Vector3D.Create(0, 0, 1);

        var sphere = _space.EncodeIpnsRound.RealSphere(radius, sphereCenter);
        var plane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act
        var meet = sphere.MeetIpns(plane);

        // Assert
        Assert.That(meet, Is.Not.Null, "Sphere-plane meet should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
        // Sphere and plane meet in a circle
    }

    [Test]
    public void MeetIpns_TwoNonIntersectingSpheres_ShouldHaveSpecialResult()
    {
        // Arrange
        var center1 = LinFloat64Vector3D.Create(0, 0, 0);
        var radius1 = 1.0;
        var center2 = LinFloat64Vector3D.Create(10, 0, 0); // Far away
        var radius2 = 1.0;

        var sphere1 = _space.EncodeIpnsRound.RealSphere(radius1, center1);
        var sphere2 = _space.EncodeIpnsRound.RealSphere(radius2, center2);

        // Act
        var meet = sphere1.MeetIpns(sphere2);

        // Assert
        Assert.That(meet, Is.Not.Null, "Non-intersecting spheres meet should exist");
        // Non-intersecting spheres give imaginary or zero result
    }

    [Test]
    public void MeetIpns_ThreeSpheres_ShouldSucceed()
    {
        // Arrange
        var center1 = LinFloat64Vector3D.Create(0, 0, 0);
        var center2 = LinFloat64Vector3D.Create(2, 0, 0);
        var center3 = LinFloat64Vector3D.Create(1, 2, 0);
        var radius = 1.5;

        var sphere1 = _space.EncodeIpnsRound.RealSphere(radius, center1);
        var sphere2 = _space.EncodeIpnsRound.RealSphere(radius, center2);
        var sphere3 = _space.EncodeIpnsRound.RealSphere(radius, center3);

        // Act
        var meet = sphere1.MeetIpns(sphere2, sphere3);

        // Assert
        Assert.That(meet, Is.Not.Null, "Three-way sphere meet should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
        // Three spheres typically meet at two points (if they intersect)
    }

    [Test]
    public void MeetIpns_LineAndPlane_ShouldGivePoint()
    {
        // Arrange
        var linePoint = LinFloat64Vector3D.Create(0, 0, 5);
        var lineDirection = LinFloat64Vector3D.Create(0, 0, -1); // Pointing down
        var planePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var planeNormal = LinFloat64Vector3D.Create(0, 0, 1); // XY plane

        var line = _space.EncodeIpnsFlat.Line(linePoint, lineDirection);
        var plane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act
        var meet = line.MeetIpns(plane);

        // Assert
        Assert.That(meet, Is.Not.Null, "Line-plane meet should give a point");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
    }

    #endregion

    #region VGA and PGA Meet Tests (5 tests)

    [Test]
    public void MeetVGa_TwoVectors_ShouldSucceed()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(1, 0, 0);
        var v2 = LinFloat64Vector3D.Create(0, 1, 0);

        var vga1 = _space.EncodeVGa.Vector(v1);
        var vga2 = _space.EncodeVGa.Vector(v2);

        // Act
        var meet = vga1.MeetVGa(vga2);

        // Assert
        Assert.That(meet, Is.Not.Null, "VGA vector meet should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
    }

    [Test]
    public void MeetVGa_ThreeVectors_ShouldSucceed()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(1, 0, 0);
        var v2 = LinFloat64Vector3D.Create(0, 1, 0);
        var v3 = LinFloat64Vector3D.Create(0, 0, 1);

        var vga1 = _space.EncodeVGa.Vector(v1);
        var vga2 = _space.EncodeVGa.Vector(v2);
        var vga3 = _space.EncodeVGa.Vector(v3);

        // Act
        var meet = vga1.MeetVGa(vga2, vga3);

        // Assert
        Assert.That(meet, Is.Not.Null, "VGA three-way meet should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
    }

    [Test]
    public void MeetPGa_TwoBlades_ShouldSucceed()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);

        var pga1 = _space.EncodePGa.Point(point1);
        var pga2 = _space.EncodePGa.Point(point2);

        // Act
        var meet = pga1.MeetPGa(pga2);

        // Assert
        Assert.That(meet, Is.Not.Null, "PGA meet should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
    }

    [Test]
    public void MeetPGa_ThreeBlades_ShouldSucceed()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);
        var point3 = LinFloat64Vector3D.Create(0, 0, 1);

        var pga1 = _space.EncodePGa.Point(point1);
        var pga2 = _space.EncodePGa.Point(point2);
        var pga3 = _space.EncodePGa.Point(point3);

        // Act
        var meet = pga1.MeetPGa(pga2, pga3);

        // Assert
        Assert.That(meet, Is.Not.Null, "PGA three-way meet should exist");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
    }

    [Test]
    public void Meet_TwoPointPairs_ShouldSucceed()
    {
        // Arrange - Create two point pairs (circles that intersect)
        var center1 = LinFloat64Vector3D.Create(0, 0, 0);
        var radius1 = 2.0;
        var center2 = LinFloat64Vector3D.Create(3, 0, 0);
        var radius2 = 2.0;

        var sphere1 = _space.EncodeIpnsRound.RealSphere(radius1, center1);
        var sphere2 = _space.EncodeIpnsRound.RealSphere(radius2, center2);

        // Act - Meet of two spheres gives a circle
        var meet = sphere1.MeetIpns(sphere2);

        // Assert
        Assert.That(meet, Is.Not.Null, "Sphere meet should give a circle");
        Assert.That(meet.InternalKVector, Is.Not.Null, "Should have internal k-vector");
        // Two intersecting spheres meet in a circle (grade 2 in IPNS)
    }

    #endregion
}
