using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for Quaternion Operations
/// Phase 3B - Core Modeling: Euclidean Geometry (10 tests)
/// Quaternions represent 3D rotations: q = w + xi + yj + zk
/// </summary>
[TestFixture]
public class LinFloat64QuaternionTests
{
    private const double Tolerance = 1e-10;

    #region Construction Tests (2 tests)

    [Test]
    public void Quaternion_Construction_ShouldHaveCorrectComponents()
    {
        // Arrange & Act
        var q = LinFloat64Quaternion.Create(2.0, 3.0, 4.0, 1.0);  // (x, y, z, w)

        // Assert
        Assert.That(q.Scalar.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Scalar (w) component should be 1");
        Assert.That(q.ScalarI.ScalarValue, Is.EqualTo(2.0).Within(Tolerance),
            "ScalarI (x) component should be 2");
        Assert.That(q.ScalarJ.ScalarValue, Is.EqualTo(3.0).Within(Tolerance),
            "ScalarJ (y) component should be 3");
        Assert.That(q.ScalarK.ScalarValue, Is.EqualTo(4.0).Within(Tolerance),
            "ScalarK (z) component should be 4");
    }

    [Test]
    public void Quaternion_Identity_ShouldHaveCorrectValues()
    {
        // Arrange
        var identity = LinFloat64Quaternion.Identity;

        // Assert
        Assert.That(identity.Scalar.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Identity scalar should be 1");
        Assert.That(identity.ScalarI.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Identity i component should be 0");
        Assert.That(identity.ScalarJ.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Identity j component should be 0");
        Assert.That(identity.ScalarK.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Identity k component should be 0");
        Assert.That(identity.IsIdentity(), Is.True, "Should recognize as identity");
    }

    #endregion

    #region Norm and Normalization (2 tests)

    [Test]
    public void Quaternion_Norm_ShouldCalculateCorrectly()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(2.0, 3.0, 4.0, 1.0);  // (x, y, z, w)

        // Act
        var norm = q.Norm();
        var normSquared = q.NormSquared();

        // Assert
        // ||q|| = sqrt(1² + 2² + 3² + 4²) = sqrt(1 + 4 + 9 + 16) = sqrt(30)
        Assert.That(normSquared.ScalarValue, Is.EqualTo(30.0).Within(Tolerance),
            "Norm squared should be 30");
        Assert.That(norm.ScalarValue, Is.EqualTo(Math.Sqrt(30.0)).Within(Tolerance),
            "Norm should be sqrt(30)");
    }

    [Test]
    public void Quaternion_Normalize_ShouldProduceUnitQuaternion()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(2.0, 3.0, 4.0, 1.0);  // (x, y, z, w)

        // Act
        var unitQ = q.Normalize();
        var norm = unitQ.Norm();

        // Assert
        Assert.That(norm.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Normalized quaternion should have norm 1");
        Assert.That(unitQ.IsNearNormalized(Tolerance), Is.True, "Should be recognized as normalized");

        // Direction should be preserved
        var originalNorm = q.Norm().ScalarValue;
        Assert.That(unitQ.Scalar.ScalarValue, Is.EqualTo(1.0 / originalNorm).Within(Tolerance),
            "Scalar component should be scaled");
    }

    #endregion

    #region Conjugate and Inverse (2 tests)

    [Test]
    public void Quaternion_Conjugate_ShouldNegateVectorPart()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(2.0, 3.0, 4.0, 1.0);  // (x, y, z, w)

        // Act
        var conjugate = q.Conjugate();

        // Assert
        // Conjugate: q* = w - xi - yj - zk
        Assert.That(conjugate.Scalar.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Conjugate keeps scalar part");
        Assert.That(conjugate.ScalarI.ScalarValue, Is.EqualTo(-2.0).Within(Tolerance),
            "Conjugate negates i component");
        Assert.That(conjugate.ScalarJ.ScalarValue, Is.EqualTo(-3.0).Within(Tolerance),
            "Conjugate negates j component");
        Assert.That(conjugate.ScalarK.ScalarValue, Is.EqualTo(-4.0).Within(Tolerance),
            "Conjugate negates k component");

        // q * q* should give norm squared
        var product = q * conjugate;
        Assert.That(product.Scalar.ScalarValue, Is.EqualTo(30.0).Within(Tolerance),
            "q * q* should equal norm squared");
        Assert.That(product.ScalarI.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "q * q* should have zero vector part");
    }

    [Test]
    public void Quaternion_Inverse_ShouldSatisfyIdentity()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(2.0, 3.0, 4.0, 1.0);  // (x, y, z, w)

        // Act
        var inverse = q.Inverse();
        var product1 = q * inverse;
        var product2 = inverse * q;

        // Assert
        // q * q^-1 = q^-1 * q = identity
        Assert.That(product1.IsNearIdentity(Tolerance), Is.True,
            "q * q^-1 should be identity");
        Assert.That(product2.IsNearIdentity(Tolerance), Is.True,
            "q^-1 * q should be identity");
    }

    #endregion

    #region Quaternion Multiplication (2 tests)

    [Test]
    public void Quaternion_Multiplication_ShouldBeNonCommutative()
    {
        // Arrange
        var q1 = LinFloat64Quaternion.Create(1.0, 0.0, 0.0, 1.0);  // (x, y, z, w)
        var q2 = LinFloat64Quaternion.Create(0.0, 1.0, 0.0, 1.0);  // (x, y, z, w)

        // Act
        var product12 = q1 * q2;
        var product21 = q2 * q1;

        // Assert
        // Quaternion multiplication is non-commutative: q1 * q2 ≠ q2 * q1
        var areEqual =
            Math.Abs(product12.Scalar.ScalarValue - product21.Scalar.ScalarValue) < Tolerance &&
            Math.Abs(product12.ScalarI.ScalarValue - product21.ScalarI.ScalarValue) < Tolerance &&
            Math.Abs(product12.ScalarJ.ScalarValue - product21.ScalarJ.ScalarValue) < Tolerance &&
            Math.Abs(product12.ScalarK.ScalarValue - product21.ScalarK.ScalarValue) < Tolerance;

        Assert.That(areEqual, Is.False,
            "Quaternion multiplication should be non-commutative for non-collinear quaternions");
    }

    [Test]
    public void Quaternion_IdentityMultiplication_ShouldPreserveQuaternion()
    {
        // Arrange
        var q = LinFloat64Quaternion.Create(3.0, 4.0, 5.0, 2.0);  // (x, y, z, w)
        var identity = LinFloat64Quaternion.Identity;

        // Act
        var product1 = q * identity;
        var product2 = identity * q;

        // Assert
        // Identity property: q * I = I * q = q
        Assert.That(product1.Scalar.ScalarValue, Is.EqualTo(q.Scalar.ScalarValue).Within(Tolerance),
            "q * I should equal q (scalar)");
        Assert.That(product1.ScalarI.ScalarValue, Is.EqualTo(q.ScalarI.ScalarValue).Within(Tolerance),
            "q * I should equal q (i)");
        Assert.That(product1.ScalarJ.ScalarValue, Is.EqualTo(q.ScalarJ.ScalarValue).Within(Tolerance),
            "q * I should equal q (j)");
        Assert.That(product1.ScalarK.ScalarValue, Is.EqualTo(q.ScalarK.ScalarValue).Within(Tolerance),
            "q * I should equal q (k)");

        Assert.That(product2.Scalar.ScalarValue, Is.EqualTo(q.Scalar.ScalarValue).Within(Tolerance),
            "I * q should equal q");
    }

    #endregion

    #region Rotation Tests (2 tests)

    [Test]
    public void Quaternion_FromAxisAngle_ShouldCreateCorrectRotation()
    {
        // Arrange
        var angle = LinFloat64PolarAngle.CreateFromDegrees(90); // 90 degree rotation
        var axis = LinFloat64Vector3D.E3; // Z-axis

        // Act
        var q = LinFloat64Quaternion.CreateFromAxisAngle(axis, angle);

        // Assert
        // For 90° rotation around Z: q = cos(45°) + sin(45°) * k
        var halfAngleRad = Math.PI / 4; // 45 degrees in radians
        Assert.That(q.Scalar.ScalarValue, Is.EqualTo(Math.Cos(halfAngleRad)).Within(Tolerance),
            "Scalar part should be cos(angle/2)");
        Assert.That(q.ScalarI.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "i component should be 0 for Z-axis rotation");
        Assert.That(q.ScalarJ.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "j component should be 0 for Z-axis rotation");
        Assert.That(Math.Abs(q.ScalarK.ScalarValue), Is.EqualTo(Math.Sin(halfAngleRad)).Within(Tolerance),
            "k component magnitude should be sin(angle/2)");

        // Rotation quaternion should be normalized
        Assert.That(q.IsNearNormalized(Tolerance), Is.True,
            "Rotation quaternion should be normalized");
    }

    [Test]
    public void Quaternion_RotateVector_ShouldRotateCorrectly()
    {
        // Arrange
        var angle = LinFloat64PolarAngle.CreateFromDegrees(90); // 90 degree rotation around Z
        var axis = LinFloat64Vector3D.E3; // Z-axis
        var q = LinFloat64Quaternion.CreateFromAxisAngle(axis, angle);

        var vector = LinFloat64Vector3D.E1; // Point along X-axis

        // Act
        // Rotate using: v' = q * v * q^-1 (as quaternions)
        var rotated = q.RotateVector(vector);

        // Assert
        // 90° rotation around Z should map X-axis to Y-axis
        Assert.That(rotated.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "X component should be 0 after 90° rotation");
        Assert.That(rotated.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Y component should be 1 after 90° rotation");
        Assert.That(rotated.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Z component should be 0 (rotation in XY plane)");

        // Rotation should preserve length
        Assert.That(rotated.VectorENorm().ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Rotation should preserve vector length");
    }

    #endregion
}
