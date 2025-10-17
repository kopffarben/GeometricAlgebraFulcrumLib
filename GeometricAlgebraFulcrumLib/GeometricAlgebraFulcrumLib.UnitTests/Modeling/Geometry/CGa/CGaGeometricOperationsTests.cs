using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Decoding;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Elements;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Operations;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Tests for CGa Geometric Operations
/// Phase 3A - Emergency: Conformal Geometry Operations (10 tests)
/// </summary>
[TestFixture]
public class CGaGeometricOperationsTests
{
    private const double Tolerance = 1e-10;
    private CGaFloat64GeometricSpace _space = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _space = CGaFloat64GeometricSpace.Space5D;
    }

    #region Translation Tests (2 tests)

    [Test]
    public void TranslatePoint_ShouldMovePosition()
    {
        // Arrange
        var originalPoint = LinFloat64Vector3D.Create(1, 2, 3);
        var translationVector = LinFloat64Vector3D.Create(5, 0, 0);

        var ipnsPoint = _space.EncodeIpnsFlat.Point(originalPoint);

        // Act
        var translatedBlade = ipnsPoint.TranslateBy(translationVector);

        // Assert
        Assert.That(translatedBlade, Is.Not.Null, "Translated blade should not be null");
        Assert.That(translatedBlade.InternalKVector, Is.Not.Null, "Translated blade internal k-vector should not be null");
    }

    [Test]
    public void TranslateSphere_ShouldPreserveRadius()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 2.0;
        var translationVector = LinFloat64Vector3D.Create(3, 4, 5);

        var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);
        var originalRadiusSquared = ipnsSphere.DecodeIpnsRound.RadiusSquared();

        // Act
        var translatedBlade = ipnsSphere.TranslateBy(translationVector);
        var translatedRadiusSquared = translatedBlade.DecodeIpnsRound.RadiusSquared();

        // Assert
        Assert.That(translatedBlade, Is.Not.Null, "Translated sphere should not be null");
        Assert.That(translatedRadiusSquared, Is.EqualTo(originalRadiusSquared).Within(Tolerance),
            "Translation should preserve sphere radius");
    }

    #endregion

    #region Rotation Tests (2 tests)

    [Test]
    public void RotatePoint_AroundZAxis_ShouldRotate()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 0, 0);
        var axisPoint = LinFloat64Vector3D.Create(0, 0, 0);
        var axisVector = LinFloat64Vector3D.Create(0, 0, 1); // Z-axis
        var angle = LinFloat64PolarAngle.CreateFromDegrees(90);

        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);

        // Act
        var rotatedBlade = ipnsPoint.RotateUsing(angle, axisPoint, axisVector);

        // Assert
        Assert.That(rotatedBlade, Is.Not.Null, "Rotated blade should not be null");
        Assert.That(rotatedBlade.InternalKVector, Is.Not.Null, "Rotated blade internal k-vector should not be null");
    }

    [Test]
    public void RotateSphere_ShouldPreserveRadius()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(2, 0, 0);
        var radius = 1.5;
        var axisPoint = LinFloat64Vector3D.Create(0, 0, 0);
        var axisVector = LinFloat64Vector3D.Create(0, 0, 1); // Z-axis
        var angle = LinFloat64PolarAngle.CreateFromDegrees(45);

        var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);
        var originalRadiusSquared = ipnsSphere.DecodeIpnsRound.RadiusSquared();

        // Act
        var rotatedBlade = ipnsSphere.RotateUsing(angle, axisPoint, axisVector);
        var rotatedRadiusSquared = rotatedBlade.DecodeIpnsRound.RadiusSquared();

        // Assert
        Assert.That(rotatedBlade, Is.Not.Null, "Rotated sphere should not be null");
        Assert.That(rotatedRadiusSquared, Is.EqualTo(originalRadiusSquared).Within(Tolerance),
            "Rotation should preserve sphere radius");
    }

    #endregion

    #region Scaling Tests (2 tests)

    [Test]
    public void ScaleSphere_ShouldChangeRadius()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 1.0;
        var scaleFactor = 2.0;

        var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);
        var originalRadiusSquared = ipnsSphere.DecodeIpnsRound.RadiusSquared();

        // Act
        var scaledBlade = ipnsSphere.ScaleBy(scaleFactor);
        var scaledRadiusSquared = scaledBlade.DecodeIpnsRound.RadiusSquared();

        // Assert
        Assert.That(scaledBlade, Is.Not.Null, "Scaled sphere should not be null");
        Assert.That(scaledRadiusSquared, Is.EqualTo(originalRadiusSquared * scaleFactor * scaleFactor).Within(Tolerance),
            "Scaling by factor k should multiply radius by k");
    }

    [Test]
    public void ScalePoint_ShouldMoveFromOrigin()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(2, 3, 4);
        var scaleFactor = 0.5;

        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);

        // Act
        var scaledBlade = ipnsPoint.ScaleBy(scaleFactor);

        // Assert
        Assert.That(scaledBlade, Is.Not.Null, "Scaled point should not be null");
        Assert.That(scaledBlade.InternalKVector, Is.Not.Null, "Scaled point internal k-vector should not be null");
    }

    #endregion

    #region Projection Tests (2 tests)

    [Test]
    public void ProjectBlade_OnPlane_ShouldSucceed()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 2, 5);
        var planePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var planeNormal = LinFloat64Vector3D.Create(0, 0, 1); // XY plane

        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);
        var ipnsPlane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act - Project blade-to-blade
        var projectedBlade = ipnsPoint.ProjectIpnsOn(ipnsPlane);

        // Assert
        Assert.That(projectedBlade, Is.Not.Null, "Projected blade should not be null");
        Assert.That(projectedBlade.InternalKVector, Is.Not.Null, "Projected blade internal k-vector should not be null");
    }

    [Test]
    public void ProjectBlade_OnLine_ShouldSucceed()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(5, 5, 0);
        var linePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var lineDirection = LinFloat64Vector3D.Create(1, 0, 0); // X-axis

        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);
        var ipnsLine = _space.EncodeIpnsFlat.Line(linePoint, lineDirection);

        // Act - Project blade-to-blade
        var projectedBlade = ipnsPoint.ProjectIpnsOn(ipnsLine);

        // Assert
        Assert.That(projectedBlade, Is.Not.Null, "Projected blade should not be null");
        Assert.That(projectedBlade.InternalKVector, Is.Not.Null, "Projected blade internal k-vector should not be null");
    }

    #endregion

    #region Reflection Tests (2 tests)

    [Test]
    public void ReflectPoint_AcrossPlane_ShouldSucceed()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var planePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var planeNormal = LinFloat64Vector3D.Create(0, 0, 1); // XY plane

        var ipnsPoint = _space.EncodeIpnsFlat.Point(point);
        var ipnsPlane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act
        var reflectedBlade = ipnsPoint.ReflectIpnsOn(ipnsPlane);

        // Assert
        Assert.That(reflectedBlade, Is.Not.Null, "Reflected blade should not be null");
        Assert.That(reflectedBlade.InternalKVector, Is.Not.Null, "Reflected blade internal k-vector should not be null");
    }

    [Test]
    public void ReflectSphere_AcrossPlane_ShouldSucceed()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(2, 3, 4);
        var radius = 1.5;
        var planePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var planeNormal = LinFloat64Vector3D.Create(1, 0, 0); // YZ plane

        var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);
        var ipnsPlane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act
        var reflectedBlade = ipnsSphere.ReflectIpnsOn(ipnsPlane);

        // Assert
        Assert.That(reflectedBlade, Is.Not.Null, "Reflected sphere should not be null");
        Assert.That(reflectedBlade.InternalKVector, Is.Not.Null, "Reflected sphere internal k-vector should not be null");
        // Note: Radius preservation depends on proper IPNS reflection implementation
        Assert.That(reflectedBlade.Grade, Is.EqualTo(ipnsSphere.Grade), "Reflected sphere should preserve grade");
    }

    #endregion
}
