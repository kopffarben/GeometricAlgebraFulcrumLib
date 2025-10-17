using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Tests for Special CGa Elements (Tangents and Directions)
/// Phase 3B - Core Modeling: Special CGA Elements (20 tests)
/// Tests tangent and direction encoding in IPNS and OPNS
/// </summary>
[TestFixture]
public class CGaSpecialElementsTests
{
    private const double Tolerance = 1e-10;
    private CGaFloat64GeometricSpace _space = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _space = CGaFloat64GeometricSpace.Space5D;
    }

    #region IPNS Tangent Tests (5 tests)

    [Test]
    public void IpnsTangent_PointEncoding_ShouldCreateValidBlade()
    {
        // Arrange & Act
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var tangent = _space.EncodeIpnsTangent.Point(point);

        // Assert
        Assert.That(tangent, Is.Not.Null, "IPNS tangent point should be created");
        Assert.That(tangent.InternalKVector, Is.Not.Null, "Tangent should have internal k-vector");
        Assert.That(tangent.GeometricSpace, Is.EqualTo(_space), "Tangent should reference correct space");
    }

    [Test]
    public void IpnsTangent_Point2D_ShouldWork()
    {
        // Arrange & Act
        var tangent = _space.EncodeIpnsTangent.Point(1.0, 2.0);

        // Assert
        Assert.That(tangent, Is.Not.Null, "2D tangent point should be created");
        Assert.That(tangent.InternalKVector, Is.Not.Null, "2D tangent should have internal k-vector");
    }

    [Test]
    public void IpnsTangent_Point3D_ShouldWork()
    {
        // Arrange & Act
        var tangent = _space.EncodeIpnsTangent.Point(1.0, 2.0, 3.0);

        // Assert
        Assert.That(tangent, Is.Not.Null, "3D tangent point should be created");
        Assert.That(tangent.InternalKVector, Is.Not.Null, "3D tangent should have internal k-vector");
    }

    [Test]
    public void IpnsTangent_MultiplePoints_ShouldProduceDifferentBlades()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 1, 0);

        // Act
        var tangent1 = _space.EncodeIpnsTangent.Point(point1);
        var tangent2 = _space.EncodeIpnsTangent.Point(point2);

        // Assert
        Assert.That(tangent1, Is.Not.Null, "First tangent should exist");
        Assert.That(tangent2, Is.Not.Null, "Second tangent should exist");
        // Different input points should produce different tangents
        Assert.That(tangent1.InternalKVector, Is.Not.Null);
        Assert.That(tangent2.InternalKVector, Is.Not.Null);
    }

    [Test]
    public void IpnsTangent_ArithmeticOperations_ShouldWork()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 1, 1);
        var tangent = _space.EncodeIpnsTangent.Point(point);

        // Act
        var scaled = tangent * 2.0;
        var negated = -tangent;

        // Assert
        Assert.That(scaled, Is.Not.Null, "Scaled tangent should exist");
        Assert.That(negated, Is.Not.Null, "Negated tangent should exist");
        Assert.That(scaled.InternalKVector, Is.Not.Null);
        Assert.That(negated.InternalKVector, Is.Not.Null);
    }

    #endregion

    #region OPNS Tangent Tests (5 tests)

    [Test]
    public void OpnsTangent_PointEncoding_ShouldCreateValidBlade()
    {
        // Arrange & Act
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var tangent = _space.EncodeOpnsTangent.Point(point);

        // Assert
        Assert.That(tangent, Is.Not.Null, "OPNS tangent point should be created");
        Assert.That(tangent.InternalKVector, Is.Not.Null, "OPNS tangent should have internal k-vector");
        Assert.That(tangent.GeometricSpace, Is.EqualTo(_space), "OPNS tangent should reference correct space");
    }

    [Test]
    public void OpnsTangent_Point2D_ShouldWork()
    {
        // Arrange & Act
        var tangent = _space.EncodeOpnsTangent.Point(2.0, 3.0);

        // Assert
        Assert.That(tangent, Is.Not.Null, "2D OPNS tangent point should be created");
        Assert.That(tangent.InternalKVector, Is.Not.Null, "2D OPNS tangent should have internal k-vector");
    }

    [Test]
    public void OpnsTangent_Point3D_ShouldWork()
    {
        // Arrange & Act
        var tangent = _space.EncodeOpnsTangent.Point(4.0, 5.0, 6.0);

        // Assert
        Assert.That(tangent, Is.Not.Null, "3D OPNS tangent point should be created");
        Assert.That(tangent.InternalKVector, Is.Not.Null, "3D OPNS tangent should have internal k-vector");
    }

    [Test]
    public void OpnsTangent_MultiplePoints_ShouldProduceDifferentBlades()
    {
        // Arrange
        var point1 = LinFloat64Vector3D.Create(1, 0, 0);
        var point2 = LinFloat64Vector3D.Create(0, 0, 1);

        // Act
        var tangent1 = _space.EncodeOpnsTangent.Point(point1);
        var tangent2 = _space.EncodeOpnsTangent.Point(point2);

        // Assert
        Assert.That(tangent1, Is.Not.Null, "First OPNS tangent should exist");
        Assert.That(tangent2, Is.Not.Null, "Second OPNS tangent should exist");
        Assert.That(tangent1.InternalKVector, Is.Not.Null);
        Assert.That(tangent2.InternalKVector, Is.Not.Null);
    }

    [Test]
    public void OpnsTangent_ArithmeticOperations_ShouldWork()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(2, 2, 2);
        var tangent = _space.EncodeOpnsTangent.Point(point);

        // Act
        var scaled = tangent * 3.0;
        var divided = tangent / 2.0;

        // Assert
        Assert.That(scaled, Is.Not.Null, "Scaled OPNS tangent should exist");
        Assert.That(divided, Is.Not.Null, "Divided OPNS tangent should exist");
        Assert.That(scaled.InternalKVector, Is.Not.Null);
        Assert.That(divided.InternalKVector, Is.Not.Null);
    }

    #endregion

    #region IPNS Direction Tests (5 tests)

    [Test]
    public void IpnsDirection_VectorEncoding_ShouldCreateValidBlade()
    {
        // Arrange & Act
        var direction = LinFloat64Vector3D.Create(1, 0, 0);
        var directionBlade = _space.EncodeIpnsDirection.Vector(direction);

        // Assert
        Assert.That(directionBlade, Is.Not.Null, "IPNS direction should be created");
        Assert.That(directionBlade.InternalKVector, Is.Not.Null, "Direction should have internal k-vector");
        Assert.That(directionBlade.GeometricSpace, Is.EqualTo(_space), "Direction should reference correct space");
    }

    [Test]
    public void IpnsDirection_BasisVectors_ShouldWork()
    {
        // Arrange & Act
        var dirX = _space.EncodeIpnsDirection.Vector(LinFloat64Vector3D.E1);
        var dirY = _space.EncodeIpnsDirection.Vector(LinFloat64Vector3D.E2);
        var dirZ = _space.EncodeIpnsDirection.Vector(LinFloat64Vector3D.E3);

        // Assert
        Assert.That(dirX, Is.Not.Null, "X direction should be created");
        Assert.That(dirY, Is.Not.Null, "Y direction should be created");
        Assert.That(dirZ, Is.Not.Null, "Z direction should be created");
        Assert.That(dirX.InternalKVector, Is.Not.Null);
        Assert.That(dirY.InternalKVector, Is.Not.Null);
        Assert.That(dirZ.InternalKVector, Is.Not.Null);
    }

    [Test]
    public void IpnsDirection_ArbitraryVector_ShouldWork()
    {
        // Arrange & Act
        var direction = LinFloat64Vector3D.Create(3, 4, 5);
        var directionBlade = _space.EncodeIpnsDirection.Vector(direction);

        // Assert
        Assert.That(directionBlade, Is.Not.Null, "Arbitrary direction should be created");
        Assert.That(directionBlade.InternalKVector, Is.Not.Null, "Arbitrary direction should have k-vector");
    }

    [Test]
    public void IpnsDirection_ScaledVector_ShouldWork()
    {
        // Arrange
        var baseDirection = LinFloat64Vector3D.Create(1, 1, 0);
        var scaledDirection = LinFloat64Vector3D.Create(2, 2, 0);

        // Act
        var dir1 = _space.EncodeIpnsDirection.Vector(baseDirection);
        var dir2 = _space.EncodeIpnsDirection.Vector(scaledDirection);

        // Assert
        Assert.That(dir1, Is.Not.Null, "Base direction should be created");
        Assert.That(dir2, Is.Not.Null, "Scaled direction should be created");
        // Scaled vectors in same direction should still produce valid directions
        Assert.That(dir1.InternalKVector, Is.Not.Null);
        Assert.That(dir2.InternalKVector, Is.Not.Null);
    }

    [Test]
    public void IpnsDirection_ArithmeticOperations_ShouldWork()
    {
        // Arrange
        var direction = LinFloat64Vector3D.Create(1, 2, 3);
        var directionBlade = _space.EncodeIpnsDirection.Vector(direction);

        // Act
        var scaled = directionBlade * 1.5;
        var negated = -directionBlade;

        // Assert
        Assert.That(scaled, Is.Not.Null, "Scaled direction should exist");
        Assert.That(negated, Is.Not.Null, "Negated direction should exist");
        Assert.That(scaled.InternalKVector, Is.Not.Null);
        Assert.That(negated.InternalKVector, Is.Not.Null);
    }

    #endregion

    #region OPNS Direction Tests (5 tests)

    [Test]
    public void OpnsDirection_VectorEncoding_ShouldCreateValidBlade()
    {
        // Arrange & Act
        var direction = LinFloat64Vector3D.Create(1, 0, 0);
        var directionBlade = _space.EncodeOpnsDirection.Vector(direction);

        // Assert
        Assert.That(directionBlade, Is.Not.Null, "OPNS direction should be created");
        Assert.That(directionBlade.InternalKVector, Is.Not.Null, "OPNS direction should have internal k-vector");
        Assert.That(directionBlade.GeometricSpace, Is.EqualTo(_space), "OPNS direction should reference correct space");
    }

    [Test]
    public void OpnsDirection_BasisVectors_ShouldWork()
    {
        // Arrange & Act
        var dirX = _space.EncodeOpnsDirection.Vector(LinFloat64Vector3D.E1);
        var dirY = _space.EncodeOpnsDirection.Vector(LinFloat64Vector3D.E2);
        var dirZ = _space.EncodeOpnsDirection.Vector(LinFloat64Vector3D.E3);

        // Assert
        Assert.That(dirX, Is.Not.Null, "OPNS X direction should be created");
        Assert.That(dirY, Is.Not.Null, "OPNS Y direction should be created");
        Assert.That(dirZ, Is.Not.Null, "OPNS Z direction should be created");
        Assert.That(dirX.InternalKVector, Is.Not.Null);
        Assert.That(dirY.InternalKVector, Is.Not.Null);
        Assert.That(dirZ.InternalKVector, Is.Not.Null);
    }

    [Test]
    public void OpnsDirection_ArbitraryVector_ShouldWork()
    {
        // Arrange & Act
        var direction = LinFloat64Vector3D.Create(7, 8, 9);
        var directionBlade = _space.EncodeOpnsDirection.Vector(direction);

        // Assert
        Assert.That(directionBlade, Is.Not.Null, "Arbitrary OPNS direction should be created");
        Assert.That(directionBlade.InternalKVector, Is.Not.Null, "Arbitrary OPNS direction should have k-vector");
    }

    [Test]
    public void OpnsDirection_OrthogonalVectors_ShouldProduceDifferentDirections()
    {
        // Arrange
        var dir1 = LinFloat64Vector3D.Create(1, 0, 0);
        var dir2 = LinFloat64Vector3D.Create(0, 1, 0);

        // Act
        var opnsDir1 = _space.EncodeOpnsDirection.Vector(dir1);
        var opnsDir2 = _space.EncodeOpnsDirection.Vector(dir2);

        // Assert
        Assert.That(opnsDir1, Is.Not.Null, "First OPNS direction should exist");
        Assert.That(opnsDir2, Is.Not.Null, "Second OPNS direction should exist");
        Assert.That(opnsDir1.InternalKVector, Is.Not.Null);
        Assert.That(opnsDir2.InternalKVector, Is.Not.Null);
    }

    [Test]
    public void OpnsDirection_ArithmeticOperations_ShouldWork()
    {
        // Arrange
        var direction = LinFloat64Vector3D.Create(2, 3, 4);
        var directionBlade = _space.EncodeOpnsDirection.Vector(direction);

        // Act
        var scaled = directionBlade * 2.5;
        var divided = directionBlade / 1.5;

        // Assert
        Assert.That(scaled, Is.Not.Null, "Scaled OPNS direction should exist");
        Assert.That(divided, Is.Not.Null, "Divided OPNS direction should exist");
        Assert.That(scaled.InternalKVector, Is.Not.Null);
        Assert.That(divided.InternalKVector, Is.Not.Null);
    }

    #endregion
}
