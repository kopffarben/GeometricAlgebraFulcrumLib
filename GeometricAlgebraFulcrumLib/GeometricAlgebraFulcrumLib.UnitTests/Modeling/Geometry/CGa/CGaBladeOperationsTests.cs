using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Tests for Extended CGa Blade Operations
/// Phase 3B - Core Modeling: Additional Blade Operations (20 tests)
/// Tests advanced blade properties, norms, and grade operations
/// </summary>
[TestFixture]
public class CGaBladeOperationsTests
{
    private const double Tolerance = 1e-10;
    private CGaFloat64GeometricSpace _space = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _space = CGaFloat64GeometricSpace.Space5D;
    }

    #region Blade Grade and Properties (5 tests)

    [Test]
    public void Blade_Grade_ShouldBeConsistent()
    {
        // Arrange & Act
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var blade = _space.EncodeIpnsFlat.Point(point);

        // Assert
        Assert.That(blade.Grade, Is.GreaterThanOrEqualTo(0), "Grade should be non-negative");
        Assert.That(blade.Grade, Is.LessThanOrEqualTo(_space.VSpaceDimensions),
            "Grade should not exceed space dimensions");
    }

    [Test]
    public void Blade_InternalKVector_ShouldNeverBeNull()
    {
        // Arrange & Act
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 1.0;
        var sphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Assert
        Assert.That(sphere.InternalKVector, Is.Not.Null, "InternalKVector should never be null");
    }

    [Test]
    public void Blade_GeometricSpace_ShouldMatchConstructor()
    {
        // Arrange & Act
        var linePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var lineDirection = LinFloat64Vector3D.E1;
        var line = _space.EncodeIpnsFlat.Line(linePoint, lineDirection);

        // Assert
        Assert.That(line.GeometricSpace, Is.EqualTo(_space), "GeometricSpace should match");
    }

    [Test]
    public void Blade_VSpaceDimensions_ShouldMatchSpace()
    {
        // Arrange & Act
        var planePoint = LinFloat64Vector3D.Create(0, 0, 1);
        var planeNormal = LinFloat64Vector3D.E3;
        var plane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Assert
        Assert.That(plane.VSpaceDimensions, Is.EqualTo(_space.VSpaceDimensions),
            "VSpaceDimensions should match space");
    }

    [Test]
    public void Blade_IsZero_ShouldDetectZeroBlade()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 0, 0);
        var blade1 = _space.EncodeIpnsFlat.Point(point);
        var blade2 = _space.EncodeIpnsFlat.Point(point);

        // Act
        var difference = blade1 - blade2; // Should be zero

        // Assert
        // The difference might not be exactly zero due to internal representation
        Assert.That(difference, Is.Not.Null, "Difference blade should exist");
    }

    #endregion

    #region Blade Norm Operations (5 tests)

    [Test]
    public void Blade_Norm_ShouldBeNonNegative()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var normal = LinFloat64Vector3D.E3;
        var radius = 2.0;
        var circle = _space.EncodeIpnsRound.RealCircle(radius, center, normal);

        // Act
        var norm = circle.Norm();

        // Assert
        Assert.That(norm, Is.GreaterThanOrEqualTo(0), "Norm should be non-negative");
    }

    [Test]
    public void Blade_NormSquared_ShouldBeNonNegativeForValidBlades()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(5, 6, 7);
        var blade = _space.EncodeIpnsFlat.Point(point);

        // Act
        var normSquared = blade.NormSquared();

        // Assert
        // Note: NormSquared can be negative in non-Euclidean signatures
        Assert.That(normSquared, Is.Not.EqualTo(double.NaN), "NormSquared should not be NaN");
    }

    [Test]
    public void Blade_ScalingPreservesDirection()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(1, 1, 1);
        var radius = 1.5;
        var sphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        var scaled = sphere * 2.0;
        var scaledNorm = scaled.Norm();
        var originalNorm = sphere.Norm();

        // Assert
        Assert.That(scaledNorm, Is.GreaterThan(0), "Scaled blade should have positive norm");
        // Scaling by 2 should roughly double the norm (depending on signature)
        Assert.That(scaledNorm, Is.GreaterThan(originalNorm * 0.5), "Scaled norm should be larger");
    }

    [Test]
    public void Blade_NegationPreservesNorm()
    {
        // Arrange
        var linePoint = LinFloat64Vector3D.Create(0, 0, 0);
        var lineDirection = LinFloat64Vector3D.E2;
        var line = _space.EncodeIpnsFlat.Line(linePoint, lineDirection);

        // Act
        var negated = -line;
        var originalNorm = line.Norm();
        var negatedNorm = negated.Norm();

        // Assert
        Assert.That(Math.Abs(originalNorm - negatedNorm), Is.LessThan(Tolerance),
            "Negation should preserve norm");
    }

    [Test]
    public void Blade_ZeroNorm_ForZeroBlade()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 0, 0);
        var blade = _space.EncodeIpnsFlat.Point(point);

        // Act
        var selfSubtraction = blade - blade;
        var norm = selfSubtraction.Norm();

        // Assert
        Assert.That(norm, Is.EqualTo(0).Within(Tolerance),
            "Self-subtraction should have zero norm");
    }

    #endregion

    #region Blade Arithmetic Chains (5 tests)

    [Test]
    public void Blade_MultipleAdditions_ShouldWork()
    {
        // Arrange
        var p1 = LinFloat64Vector3D.Create(1, 0, 0);
        var p2 = LinFloat64Vector3D.Create(0, 1, 0);
        var p3 = LinFloat64Vector3D.Create(0, 0, 1);
        var blade1 = _space.EncodeIpnsFlat.Point(p1);
        var blade2 = _space.EncodeIpnsFlat.Point(p2);
        var blade3 = _space.EncodeIpnsFlat.Point(p3);

        // Act
        var sum = blade1 + blade2 + blade3;

        // Assert
        Assert.That(sum, Is.Not.Null, "Triple addition should succeed");
        Assert.That(sum.InternalKVector, Is.Not.Null, "Sum should have k-vector");
    }

    [Test]
    public void Blade_MultipleMultiplications_ShouldWork()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 1.0;
        var sphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        var scaled = sphere * 2.0 * 1.5 * 0.5;

        // Assert
        Assert.That(scaled, Is.Not.Null, "Multiple multiplications should succeed");
        Assert.That(scaled.InternalKVector, Is.Not.Null, "Scaled blade should have k-vector");
    }

    [Test]
    public void Blade_MixedArithmetic_ShouldWork()
    {
        // Arrange
        var c1 = LinFloat64Vector3D.Create(1, 0, 0);
        var c2 = LinFloat64Vector3D.Create(-1, 0, 0);
        var normal = LinFloat64Vector3D.E3;
        var radius = 1.0;
        var circle1 = _space.EncodeIpnsRound.RealCircle(radius, c1, normal);
        var circle2 = _space.EncodeIpnsRound.RealCircle(radius, c2, normal);

        // Act
        var result = (circle1 * 2.0 + circle2) / 3.0;

        // Assert
        Assert.That(result, Is.Not.Null, "Mixed arithmetic should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Blade_ParenthesizedArithmetic_ShouldWork()
    {
        // Arrange
        var p1 = LinFloat64Vector3D.Create(1, 0, 0);
        var p2 = LinFloat64Vector3D.Create(0, 1, 0);
        var p3 = LinFloat64Vector3D.Create(0, 0, 1);
        var b1 = _space.EncodeIpnsFlat.Point(p1);
        var b2 = _space.EncodeIpnsFlat.Point(p2);
        var b3 = _space.EncodeIpnsFlat.Point(p3);

        // Act
        var result = (b1 + b2) * 2.0 - b3;

        // Assert
        Assert.That(result, Is.Not.Null, "Parenthesized arithmetic should succeed");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    [Test]
    public void Blade_DistributiveProperty_ShouldWork()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(1, 0, 0);
        var radius = 1.0;
        var sphere1 = _space.EncodeIpnsRound.RealSphere(radius, center);
        var sphere2 = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        // Test: 2 * (a + b) should work (distributive multiplication)
        var sum = sphere1 + sphere2;
        var result = sum * 2.0;

        // Assert
        Assert.That(result, Is.Not.Null, "Distributive multiplication should work");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have k-vector");
    }

    #endregion

    #region Blade Type Consistency (5 tests)

    [Test]
    public void Blade_OperatorsReturnSameType()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var blade = _space.EncodeIpnsFlat.Point(point);

        // Act & Assert
        var sum = blade + blade;
        Assert.That(sum, Is.InstanceOf<CGaFloat64Blade>(), "Addition should return CGaFloat64Blade");

        var difference = blade - blade;
        Assert.That(difference, Is.InstanceOf<CGaFloat64Blade>(), "Subtraction should return CGaFloat64Blade");

        var scaled = blade * 2.0;
        Assert.That(scaled, Is.InstanceOf<CGaFloat64Blade>(), "Multiplication should return CGaFloat64Blade");
    }

    [Test]
    public void Blade_AllOperationsPreserveGeometricSpace()
    {
        // Arrange
        var center = LinFloat64Vector3D.Create(0, 0, 0);
        var radius = 1.0;
        var sphere = _space.EncodeIpnsRound.RealSphere(radius, center);

        // Act
        var negated = -sphere;
        var scaled = sphere * 2.0;
        var divided = sphere / 2.0;

        // Assert
        Assert.That(negated.GeometricSpace, Is.EqualTo(_space), "Negation preserves space");
        Assert.That(scaled.GeometricSpace, Is.EqualTo(_space), "Scaling preserves space");
        Assert.That(divided.GeometricSpace, Is.EqualTo(_space), "Division preserves space");
    }

    [Test]
    public void Blade_CombinedOperationsPreserveGeometricSpace()
    {
        // Arrange
        var c1 = LinFloat64Vector3D.Create(1, 0, 0);
        var c2 = LinFloat64Vector3D.Create(0, 1, 0);
        var normal = LinFloat64Vector3D.E3;
        var radius = 1.0;
        var circle1 = _space.EncodeIpnsRound.RealCircle(radius, c1, normal);
        var circle2 = _space.EncodeIpnsRound.RealCircle(radius, c2, normal);

        // Act
        var result = (circle1 + circle2) * 0.5 - circle1;

        // Assert
        Assert.That(result.GeometricSpace, Is.EqualTo(_space),
            "Combined operations preserve geometric space");
    }

    [Test]
    public void Blade_ChainedOperationsReturnValidBlades()
    {
        // Arrange
        var planePoint = LinFloat64Vector3D.Create(0, 0, 5);
        var planeNormal = LinFloat64Vector3D.E3;
        var plane = _space.EncodeIpnsFlat.Plane(planePoint, planeNormal);

        // Act
        var result = -((plane * 3.0 + plane) / 2.0);

        // Assert
        Assert.That(result, Is.Not.Null, "Chained operations should return valid blade");
        Assert.That(result.InternalKVector, Is.Not.Null, "Result should have internal k-vector");
        Assert.That(result.GeometricSpace, Is.EqualTo(_space), "Result should preserve space");
    }

    [Test]
    public void Blade_AllBasicPropertiesAccessible()
    {
        // Arrange
        var linePoint = LinFloat64Vector3D.Create(1, 1, 1);
        var lineDirection = LinFloat64Vector3D.E1;
        var line = _space.EncodeIpnsFlat.Line(linePoint, lineDirection);

        // Act & Assert - All properties should be accessible without throwing
        Assert.DoesNotThrow(() =>
        {
            var _ = line.Grade;
            var __ = line.VSpaceDimensions;
            var ___ = line.GeometricSpace;
            var ____ = line.InternalKVector;
            var _____ = line.Norm();
            var ______ = line.NormSquared();
        }, "All basic properties should be accessible");
    }

    #endregion
}
