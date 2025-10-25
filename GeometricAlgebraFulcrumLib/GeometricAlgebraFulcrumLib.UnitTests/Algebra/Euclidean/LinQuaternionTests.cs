using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for Quaternion Operations
/// EXTENDED: Tests both Float64 AND Generic&lt;double&gt; implementations
/// Phase 2 - Validates API parity between Float64 and Generic&lt;double&gt;
/// Quaternions represent 3D rotations: q = w + xi + yj + zk
/// </summary>
[TestFixture]
public class LinQuaternionTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Helper Methods

    private object CreateQuaternion(double x, double y, double z, double w, bool useGeneric)
    {
        if (useGeneric)
        {
            return LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(x),
                _scalarProcessor.ScalarFromNumber(y),
                _scalarProcessor.ScalarFromNumber(z),
                _scalarProcessor.ScalarFromNumber(w)
            );
        }
        else
        {
            return LinFloat64Quaternion.Create(x, y, z, w);
        }
    }

    private object GetIdentity(bool useGeneric)
    {
        return useGeneric
            ? LinQuaternion<double>.Identity(_scalarProcessor)
            : LinFloat64Quaternion.Identity;
    }

    private object CreateFromAxisAngle(object axis, double degrees, bool useGeneric)
    {
        if (useGeneric)
        {
            var angle = LinPolarAngle<double>.CreateFromDegrees(
                _scalarProcessor.ScalarFromNumber(degrees)
            );
            return LinQuaternion<double>.CreateFromAxisAngle((LinVector3D<double>)axis, angle);
        }
        else
        {
            var angle = LinFloat64PolarAngle.CreateFromDegrees(degrees);
            return LinFloat64Quaternion.CreateFromAxisAngle((LinFloat64Vector3D)axis, angle);
        }
    }

    private double GetScalar(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.Scalar.ScalarValue,
            LinQuaternion<double> gen => gen.Scalar.ScalarValue,
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private double GetScalarI(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.ScalarI.ScalarValue,
            LinQuaternion<double> gen => gen.ScalarI.ScalarValue,
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private double GetScalarJ(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.ScalarJ.ScalarValue,
            LinQuaternion<double> gen => gen.ScalarJ.ScalarValue,
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private double GetScalarK(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.ScalarK.ScalarValue,
            LinQuaternion<double> gen => gen.ScalarK.ScalarValue,
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private bool IsIdentity(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.IsIdentity(),
            LinQuaternion<double> gen => gen.IsIdentity(),
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private double Norm(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.Norm().ScalarValue,
            LinQuaternion<double> gen => gen.Norm().ScalarValue,
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private double NormSquared(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.NormSquared().ScalarValue,
            LinQuaternion<double> gen => gen.NormSquared().ScalarValue,
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private object Normalize(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.Normalize(),
            LinQuaternion<double> gen => gen.Normalize(),
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private bool IsNearNormalized(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.IsNearNormalized(Tolerance),
            LinQuaternion<double> gen => gen.IsNearNormalized(),
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private object Conjugate(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.Conjugate(),
            LinQuaternion<double> gen => gen.Conjugate(),
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private object Inverse(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.Inverse(),
            LinQuaternion<double> gen => gen.Inverse(),
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private object Multiply(object q1, object q2)
    {
        return (q1, q2) switch
        {
            (LinFloat64Quaternion f1, LinFloat64Quaternion f2) => f1 * f2,
            (LinQuaternion<double> g1, LinQuaternion<double> g2) => g1 * g2,
            _ => throw new ArgumentException("Mixed quaternion types")
        };
    }

    private bool IsNearIdentity(object q)
    {
        return q switch
        {
            LinFloat64Quaternion f64 => f64.IsNearIdentity(Tolerance),
            LinQuaternion<double> gen => gen.IsNearIdentity(),
            _ => throw new ArgumentException($"Unexpected quaternion type: {q.GetType()}")
        };
    }

    private object GetE3Axis(bool useGeneric)
    {
        return useGeneric
            ? LinVector3D<double>.E3(_scalarProcessor)
            : LinFloat64Vector3D.E3;
    }

    private object GetE1Vector(bool useGeneric)
    {
        return useGeneric
            ? LinVector3D<double>.E1(_scalarProcessor)
            : LinFloat64Vector3D.E1;
    }

    private object RotateVector(object q, object v)
    {
        return (q, v) switch
        {
            (LinFloat64Quaternion fq, LinFloat64Vector3D fv) => fq.RotateVector(fv),
            (LinQuaternion<double> gq, LinVector3D<double> gv) => gq.RotateVector(gv),
            _ => throw new ArgumentException("Mixed types")
        };
    }

    private double GetVectorX(object v)
    {
        return v switch
        {
            LinFloat64Vector3D f64 => f64.X.ScalarValue,
            LinVector3D<double> gen => gen.X.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {v.GetType()}")
        };
    }

    private double GetVectorY(object v)
    {
        return v switch
        {
            LinFloat64Vector3D f64 => f64.Y.ScalarValue,
            LinVector3D<double> gen => gen.Y.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {v.GetType()}")
        };
    }

    private double GetVectorZ(object v)
    {
        return v switch
        {
            LinFloat64Vector3D f64 => f64.Z.ScalarValue,
            LinVector3D<double> gen => gen.Z.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {v.GetType()}")
        };
    }

    private double VectorNorm(object v)
    {
        return v switch
        {
            LinFloat64Vector3D f64 => f64.VectorENorm().ScalarValue,
            LinVector3D<double> gen => gen.VectorENorm().ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {v.GetType()}")
        };
    }

    #endregion

    #region Construction Tests (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Construction_ShouldHaveCorrectComponents(bool useGeneric)
    {
        // Arrange & Act
        var q = CreateQuaternion(2.0, 3.0, 4.0, 1.0, useGeneric);  // (x, y, z, w)

        // Assert
        Assert.That(GetScalar(q), Is.EqualTo(1.0).Within(Tolerance),
            "Scalar (w) component should be 1");
        Assert.That(GetScalarI(q), Is.EqualTo(2.0).Within(Tolerance),
            "ScalarI (x) component should be 2");
        Assert.That(GetScalarJ(q), Is.EqualTo(3.0).Within(Tolerance),
            "ScalarJ (y) component should be 3");
        Assert.That(GetScalarK(q), Is.EqualTo(4.0).Within(Tolerance),
            "ScalarK (z) component should be 4");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Identity_ShouldHaveCorrectValues(bool useGeneric)
    {
        // Arrange
        var identity = GetIdentity(useGeneric);

        // Assert
        Assert.That(GetScalar(identity), Is.EqualTo(1.0).Within(Tolerance),
            "Identity scalar should be 1");
        Assert.That(GetScalarI(identity), Is.EqualTo(0.0).Within(Tolerance),
            "Identity i component should be 0");
        Assert.That(GetScalarJ(identity), Is.EqualTo(0.0).Within(Tolerance),
            "Identity j component should be 0");
        Assert.That(GetScalarK(identity), Is.EqualTo(0.0).Within(Tolerance),
            "Identity k component should be 0");
        Assert.That(IsIdentity(identity), Is.True, "Should recognize as identity");
    }

    #endregion

    #region Norm and Normalization (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Norm_ShouldCalculateCorrectly(bool useGeneric)
    {
        // Arrange
        var q = CreateQuaternion(2.0, 3.0, 4.0, 1.0, useGeneric);  // (x, y, z, w)

        // Act
        var norm = Norm(q);
        var normSquared = NormSquared(q);

        // Assert
        // ||q|| = sqrt(1² + 2² + 3² + 4²) = sqrt(1 + 4 + 9 + 16) = sqrt(30)
        Assert.That(normSquared, Is.EqualTo(30.0).Within(Tolerance),
            "Norm squared should be 30");
        Assert.That(norm, Is.EqualTo(Math.Sqrt(30.0)).Within(Tolerance),
            "Norm should be sqrt(30)");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Normalize_ShouldProduceUnitQuaternion(bool useGeneric)
    {
        // Arrange
        var q = CreateQuaternion(2.0, 3.0, 4.0, 1.0, useGeneric);  // (x, y, z, w)

        // Act
        var unitQ = Normalize(q);
        var norm = Norm(unitQ);

        // Assert
        Assert.That(norm, Is.EqualTo(1.0).Within(Tolerance),
            "Normalized quaternion should have norm 1");
        Assert.That(IsNearNormalized(unitQ), Is.True, "Should be recognized as normalized");

        // Direction should be preserved
        var originalNorm = Norm(q);
        Assert.That(GetScalar(unitQ), Is.EqualTo(1.0 / originalNorm).Within(Tolerance),
            "Scalar component should be scaled");
    }

    #endregion

    #region Conjugate and Inverse (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Conjugate_ShouldNegateVectorPart(bool useGeneric)
    {
        // Arrange
        var q = CreateQuaternion(2.0, 3.0, 4.0, 1.0, useGeneric);  // (x, y, z, w)

        // Act
        var conjugate = Conjugate(q);

        // Assert
        // Conjugate: q* = w - xi - yj - zk
        Assert.That(GetScalar(conjugate), Is.EqualTo(1.0).Within(Tolerance),
            "Conjugate keeps scalar part");
        Assert.That(GetScalarI(conjugate), Is.EqualTo(-2.0).Within(Tolerance),
            "Conjugate negates i component");
        Assert.That(GetScalarJ(conjugate), Is.EqualTo(-3.0).Within(Tolerance),
            "Conjugate negates j component");
        Assert.That(GetScalarK(conjugate), Is.EqualTo(-4.0).Within(Tolerance),
            "Conjugate negates k component");

        // q * q* should give norm squared
        var product = Multiply(q, conjugate);
        Assert.That(GetScalar(product), Is.EqualTo(30.0).Within(Tolerance),
            "q * q* should equal norm squared");
        Assert.That(GetScalarI(product), Is.EqualTo(0.0).Within(Tolerance),
            "q * q* should have zero vector part");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Inverse_ShouldSatisfyIdentity(bool useGeneric)
    {
        // Arrange
        var q = CreateQuaternion(2.0, 3.0, 4.0, 1.0, useGeneric);  // (x, y, z, w)

        // Act
        var inverse = Inverse(q);
        var product1 = Multiply(q, inverse);
        var product2 = Multiply(inverse, q);

        // Assert
        // q * q^-1 = q^-1 * q = identity
        Assert.That(IsNearIdentity(product1), Is.True,
            "q * q^-1 should be identity");
        Assert.That(IsNearIdentity(product2), Is.True,
            "q^-1 * q should be identity");
    }

    #endregion

    #region Quaternion Multiplication (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Multiplication_ShouldBeNonCommutative(bool useGeneric)
    {
        // Arrange
        var q1 = CreateQuaternion(1.0, 0.0, 0.0, 1.0, useGeneric);  // (x, y, z, w)
        var q2 = CreateQuaternion(0.0, 1.0, 0.0, 1.0, useGeneric);  // (x, y, z, w)

        // Act
        var product12 = Multiply(q1, q2);
        var product21 = Multiply(q2, q1);

        // Assert
        // Quaternion multiplication is non-commutative: q1 * q2 ≠ q2 * q1
        var areEqual =
            Math.Abs(GetScalar(product12) - GetScalar(product21)) < Tolerance &&
            Math.Abs(GetScalarI(product12) - GetScalarI(product21)) < Tolerance &&
            Math.Abs(GetScalarJ(product12) - GetScalarJ(product21)) < Tolerance &&
            Math.Abs(GetScalarK(product12) - GetScalarK(product21)) < Tolerance;

        Assert.That(areEqual, Is.False,
            "Quaternion multiplication should be non-commutative for non-collinear quaternions");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_IdentityMultiplication_ShouldPreserveQuaternion(bool useGeneric)
    {
        // Arrange
        var q = CreateQuaternion(3.0, 4.0, 5.0, 2.0, useGeneric);  // (x, y, z, w)
        var identity = GetIdentity(useGeneric);

        // Act
        var product1 = Multiply(q, identity);
        var product2 = Multiply(identity, q);

        // Assert
        // Identity property: q * I = I * q = q
        Assert.That(GetScalar(product1), Is.EqualTo(GetScalar(q)).Within(Tolerance),
            "q * I should equal q (scalar)");
        Assert.That(GetScalarI(product1), Is.EqualTo(GetScalarI(q)).Within(Tolerance),
            "q * I should equal q (i)");
        Assert.That(GetScalarJ(product1), Is.EqualTo(GetScalarJ(q)).Within(Tolerance),
            "q * I should equal q (j)");
        Assert.That(GetScalarK(product1), Is.EqualTo(GetScalarK(q)).Within(Tolerance),
            "q * I should equal q (k)");

        Assert.That(GetScalar(product2), Is.EqualTo(GetScalar(q)).Within(Tolerance),
            "I * q should equal q");
    }

    #endregion

    #region Rotation Tests (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_FromAxisAngle_ShouldCreateCorrectRotation(bool useGeneric)
    {
        // Arrange
        var axis = GetE3Axis(useGeneric); // Z-axis

        // Act
        var q = CreateFromAxisAngle(axis, 90, useGeneric); // 90 degree rotation

        // Assert
        // For 90° rotation around Z: q = cos(45°) + sin(45°) * k
        var halfAngleRad = Math.PI / 4; // 45 degrees in radians
        Assert.That(GetScalar(q), Is.EqualTo(Math.Cos(halfAngleRad)).Within(Tolerance),
            "Scalar part should be cos(angle/2)");
        Assert.That(GetScalarI(q), Is.EqualTo(0.0).Within(Tolerance),
            "i component should be 0 for Z-axis rotation");
        Assert.That(GetScalarJ(q), Is.EqualTo(0.0).Within(Tolerance),
            "j component should be 0 for Z-axis rotation");
        Assert.That(Math.Abs(GetScalarK(q)), Is.EqualTo(Math.Sin(halfAngleRad)).Within(Tolerance),
            "k component magnitude should be sin(angle/2)");

        // Rotation quaternion should be normalized
        Assert.That(IsNearNormalized(q), Is.True,
            "Rotation quaternion should be normalized");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_RotateVector_ShouldRotateCorrectly(bool useGeneric)
    {
        // Arrange
        var axis = GetE3Axis(useGeneric); // Z-axis
        var q = CreateFromAxisAngle(axis, 90, useGeneric); // 90 degree rotation around Z

        var vector = GetE1Vector(useGeneric); // Point along X-axis

        // Act
        // Rotate using: v' = q * v * q^-1 (as quaternions)
        var rotated = RotateVector(q, vector);

        // Assert
        // 90° rotation around Z should map X-axis to Y-axis
        Assert.That(GetVectorX(rotated), Is.EqualTo(0.0).Within(Tolerance),
            "X component should be 0 after 90° rotation");
        Assert.That(GetVectorY(rotated), Is.EqualTo(1.0).Within(Tolerance),
            "Y component should be 1 after 90° rotation");
        Assert.That(GetVectorZ(rotated), Is.EqualTo(0.0).Within(Tolerance),
            "Z component should be 0 (rotation in XY plane)");

        // Rotation should preserve length
        Assert.That(VectorNorm(rotated), Is.EqualTo(1.0).Within(Tolerance),
            "Rotation should preserve vector length");
    }

    #endregion
}
