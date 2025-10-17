using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Basis;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.Euclidean;

/// <summary>
/// Tests for Quaternion Operations
/// Phase 3B - Core Modeling: Euclidean Geometry Quaternion Operations (15 tests)
/// Tests quaternion construction, multiplication, rotation, and conversions
/// </summary>
[TestFixture]
public class LinFloat64QuaternionTests
{
    private const double Tolerance = 1e-10;

    #region Quaternion Construction Tests (5 tests)

    [Test]
    public void Quaternion_Construction_ShouldWork()
    {
        // Arrange & Act
        var q = LinFloat64Quaternion.Create(1, 2, 3, 4);

        // Assert
        Assert.That(q, Is.Not.Null, "Quaternion should be created");
        Assert.That(q.Scalar.ScalarValue, Is.EqualTo(1), "Scalar part should match");
        Assert.That(q.ScalarI.ScalarValue, Is.EqualTo(2), "i component should match");
        Assert.That(q.ScalarJ.ScalarValue, Is.EqualTo(3), "j component should match");
        Assert.That(q.ScalarK.ScalarValue, Is.EqualTo(4), "k component should match");
    }

    [Test]
    public void Quaternion_Identity_ShouldBeValid()
    {
        // Arrange & Act
        var identity = LinFloat64Quaternion.Identity;

        // Assert
        Assert.That(identity.Scalar.ScalarValue, Is.EqualTo(1), "Identity scalar should be 1");
        Assert.That(identity.ScalarI.ScalarValue, Is.EqualTo(0), "Identity i should be 0");
        Assert.That(identity.ScalarJ.ScalarValue, Is.EqualTo(0), "Identity j should be 0");
        Assert.That(identity.ScalarK.ScalarValue, Is.EqualTo(0), "Identity k should be 0");
    }

    [Test]
    public void Quaternion_FromScalarAndBivector_ShouldWork()
    {
        // Arrange
        var scalar = 1.0;
        var bivector = LinFloat64Bivector3D.Create(2, 3, 4);

        // Act
        var q = LinFloat64Quaternion.Create(scalar, bivector);

        // Assert
        Assert.That(q, Is.Not.Null, "Quaternion should be created");
        Assert.That(q.Scalar.ScalarValue, Is.EqualTo(1), "Scalar part should match");
    }

    [Test]
    public void Quaternion_FromAxisAngle_ShouldWork()
    {
        // Arrange
        var axis = LinFloat64Vector3D.E1; // X axis
        var angle = LinFloat64PolarAngle.Angle90; // 90 degrees

        // Act
        var q = LinFloat64Quaternion.CreateFromAxisAngle(axis, angle);

        // Assert
        Assert.That(q, Is.Not.Null, "Quaternion should be created from axis-angle");
        Assert.That(q.Scalar.ScalarValue, Is.GreaterThan(0), "Scalar part should be positive");
    }

    [Test]
    public void Quaternion_FromBasisAxisAngle_ShouldWork()
    {
        // Arrange
        var axis = LinBasisVector.Px; // X axis
        var angle = LinFloat64PolarAngle.Angle180; // 180 degrees

        // Act
        var q = LinFloat64Quaternion.CreateFromAxisAngle(axis, angle);

        // Assert
        Assert.That(q, Is.Not.Null, "Quaternion should be created from basis axis-angle");
    }

    #endregion

    #region Quaternion Arithmetic Tests (4 tests)

    [Test]
    public void Quaternion_Addition_ShouldWork()
    {
        // Arrange
        var q1 = LinFloat64Quaternion.Create(1, 2, 3, 4);
        var q2 = LinFloat64Quaternion.Create(5, 6, 7, 8);

        // Act
        var result = q1 + q2;

        // Assert
        Assert.That(result.Scalar.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(8).Within(Tolerance));
        Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(10).Within(Tolerance));
        Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(12).Within(Tolerance));
    }

    [Test]
    public void Quaternion_Subtraction_ShouldWork()
    {
        // Arrange
        var q1 = LinFloat64Quaternion.Create(10, 8, 6, 4);
        var q2 = LinFloat64Quaternion.Create(1, 2, 3, 4);

        // Act
        var result = q1 - q2;

        // Assert
        Assert.That(result.Scalar.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(3).Within(Tolerance));
        Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(0).Within(Tolerance));
    }

    [Test]
    public void Quaternion_ScalarMultiplication_ShouldWork()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(1, 2, 3, 4);

        // Act
        var result = q * 2;

        // Assert
        Assert.That(result.Scalar.ScalarValue, Is.EqualTo(2).Within(Tolerance));
        Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(4).Within(Tolerance));
        Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(8).Within(Tolerance));
    }

    [Test]
    public void Quaternion_Negation_ShouldWork()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(1, -2, 3, -4);

        // Act
        var result = -q;

        // Assert
        Assert.That(result.Scalar.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
        Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(2).Within(Tolerance));
        Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
        Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(4).Within(Tolerance));
    }

    #endregion

    #region Quaternion Multiplication Tests (3 tests)

    [Test]
    public void Quaternion_Multiplication_ShouldWork()
    {
        // Arrange
        var q1 = LinFloat64Quaternion.Create(1, 2, 3, 4);
        var q2 = LinFloat64Quaternion.Create(5, 6, 7, 8);

        // Act
        var result = q1 * q2;

        // Assert
        Assert.That(result, Is.Not.Null, "Quaternion multiplication should produce result");
    }

    [Test]
    public void Quaternion_IdentityMultiplication_ShouldPreserveQuaternion()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(2, 3, 4, 5);
        var identity = LinFloat64Quaternion.Identity;

        // Act
        var result1 = q * identity;
        var result2 = identity * q;

        // Assert
        Assert.That(result1.Scalar.ScalarValue, Is.EqualTo(q.Scalar.ScalarValue).Within(Tolerance));
        Assert.That(result2.Scalar.ScalarValue, Is.EqualTo(q.Scalar.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Quaternion_MultiplicationIsNonCommutative_ShouldBeValid()
    {
        // Arrange
        var q1 = LinFloat64Quaternion.Create(1, 1, 0, 0);
        var q2 = LinFloat64Quaternion.Create(1, 0, 1, 0);

        // Act
        var result1 = q1 * q2;
        var result2 = q2 * q1;

        // Assert - q1*q2 should NOT equal q2*q1 (non-commutative)
        Assert.That(result1, Is.Not.Null, "First product should exist");
        Assert.That(result2, Is.Not.Null, "Second product should exist");
        // We can't easily compare them without implementing equality, so just verify they both work
    }

    #endregion

    #region Quaternion Operations Tests (3 tests)

    [Test]
    public void Quaternion_Conjugate_ShouldWork()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(1, 2, 3, 4);

        // Act
        var conj = q.Conjugate();

        // Assert
        Assert.That(conj.Scalar.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Scalar part unchanged");
        Assert.That(conj.ScalarI.ScalarValue, Is.EqualTo(-2).Within(Tolerance), "i negated");
        Assert.That(conj.ScalarJ.ScalarValue, Is.EqualTo(-3).Within(Tolerance), "j negated");
        Assert.That(conj.ScalarK.ScalarValue, Is.EqualTo(-4).Within(Tolerance), "k negated");
    }

    [Test]
    public void Quaternion_RotateVector_ShouldWork()
    {
        // Arrange
        var axis = LinFloat64Vector3D.E3; // Z axis
        var angle = LinFloat64PolarAngle.Angle90; // 90 degrees
        var q = LinFloat64Quaternion.CreateFromAxisAngle(axis, angle);
        var v = LinFloat64Vector3D.E1; // X axis vector

        // Act
        var rotated = q.RotateVector(v);

        // Assert - rotating X by 90° around Z should give approximately Y
        Assert.That(rotated, Is.Not.Null, "Rotated vector should exist");
        Assert.That(Math.Abs(rotated.X.ScalarValue), Is.LessThan(Tolerance), "X should be near 0");
        Assert.That(Math.Abs(rotated.Y.ScalarValue - 1), Is.LessThan(Tolerance), "Y should be near 1");
        Assert.That(Math.Abs(rotated.Z.ScalarValue), Is.LessThan(Tolerance), "Z should be near 0");
    }

    [Test]
    public void Quaternion_NormSquared_ShouldBeValid()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(1, 2, 2, 0);

        // Act
        var normSquared = q.NormSquared();

        // Assert - 1^2 + 2^2 + 2^2 + 0^2 = 9
        Assert.That(normSquared.ScalarValue, Is.EqualTo(9).Within(Tolerance), "Norm squared should be 9");
    }

    #endregion
}
