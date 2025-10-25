using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for 3D Euclidean Vector Operations
/// EXTENDED: Tests both Float64 AND Generic&lt;double&gt; implementations
/// Phase 2 - Validates API parity between Float64 and Generic&lt;double&gt;
/// </summary>
[TestFixture]
public class LinVector3DTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Helper Methods

    private object CreateVector(double x, double y, double z, bool useGeneric)
    {
        if (useGeneric)
        {
            return LinVector3D<double>.Create(
                _scalarProcessor.ScalarFromNumber(x),
                _scalarProcessor.ScalarFromNumber(y),
                _scalarProcessor.ScalarFromNumber(z)
            );
        }
        else
        {
            return LinFloat64Vector3D.Create(x, y, z);
        }
    }

    private double GetX(object vector)
    {
        return vector switch
        {
            LinFloat64Vector3D f64 => f64.X.ScalarValue,
            LinVector3D<double> gen => gen.X.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double GetY(object vector)
    {
        return vector switch
        {
            LinFloat64Vector3D f64 => f64.Y.ScalarValue,
            LinVector3D<double> gen => gen.Y.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double GetZ(object vector)
    {
        return vector switch
        {
            LinFloat64Vector3D f64 => f64.Z.ScalarValue,
            LinVector3D<double> gen => gen.Z.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private object Add(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector3D f1, LinFloat64Vector3D f2) => f1 + f2,
            (LinVector3D<double> g1, LinVector3D<double> g2) => g1 + g2,
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object Subtract(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector3D f1, LinFloat64Vector3D f2) => f1 - f2,
            (LinVector3D<double> g1, LinVector3D<double> g2) => g1 - g2,
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object Multiply(object vector, double scalar)
    {
        return vector switch
        {
            LinFloat64Vector3D f64 => f64 * scalar,
            LinVector3D<double> gen => gen * _scalarProcessor.ScalarFromNumber(scalar),
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double VectorENorm(object vector)
    {
        return vector switch
        {
            LinFloat64Vector3D f64 => f64.VectorENorm().ScalarValue,
            LinVector3D<double> gen => gen.VectorENorm().ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double VectorESp(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector3D f1, LinFloat64Vector3D f2) => f1.VectorESp(f2).ScalarValue,
            (LinVector3D<double> g1, LinVector3D<double> g2) => g1.VectorESp(g2).ScalarValue,
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object VectorCross(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector3D f1, LinFloat64Vector3D f2) => f1.VectorCross(f2),
            (LinVector3D<double> g1, LinVector3D<double> g2) => g1.VectorCross(g2),
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object ToUnitVector(object vector)
    {
        return vector switch
        {
            LinFloat64Vector3D f64 => f64.ToUnitLinVector3D(),
            LinVector3D<double> gen => gen.ToUnitLinVector3D(), // Now identical API!
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private bool IsNearUnit(object vector)
    {
        return vector switch
        {
            LinFloat64Vector3D f64 => f64.IsNearUnit(),
            LinVector3D<double> gen => gen.IsNearUnit(),
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private object GetE1(bool useGeneric)
    {
        return useGeneric
            ? LinVector3D<double>.E1(_scalarProcessor)
            : LinFloat64Vector3D.E1;
    }

    private object GetE2(bool useGeneric)
    {
        return useGeneric
            ? LinVector3D<double>.E2(_scalarProcessor)
            : LinFloat64Vector3D.E2;
    }

    private object GetE3(bool useGeneric)
    {
        return useGeneric
            ? LinVector3D<double>.E3(_scalarProcessor)
            : LinFloat64Vector3D.E3;
    }

    #endregion

    #region Construction Tests (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void CreateVector_ShouldHaveCorrectComponents(bool useGeneric)
    {
        // Arrange & Act
        var vector = CreateVector(2.0, 3.0, 4.0, useGeneric);

        // Assert
        Assert.That(GetX(vector), Is.EqualTo(2.0).Within(Tolerance), "X component should be 2.0");
        Assert.That(GetY(vector), Is.EqualTo(3.0).Within(Tolerance), "Y component should be 3.0");
        Assert.That(GetZ(vector), Is.EqualTo(4.0).Within(Tolerance), "Z component should be 4.0");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void BasisVectors_ShouldBeOrthonormal(bool useGeneric)
    {
        // Arrange
        var e1 = GetE1(useGeneric);
        var e2 = GetE2(useGeneric);
        var e3 = GetE3(useGeneric);

        // Act & Assert - Check unit length
        Assert.That(VectorENorm(e1), Is.EqualTo(1.0).Within(Tolerance), "E1 should be unit length");
        Assert.That(VectorENorm(e2), Is.EqualTo(1.0).Within(Tolerance), "E2 should be unit length");
        Assert.That(VectorENorm(e3), Is.EqualTo(1.0).Within(Tolerance), "E3 should be unit length");

        // Check orthogonality
        Assert.That(VectorESp(e1, e2), Is.EqualTo(0.0).Within(Tolerance), "E1 and E2 are orthogonal");
        Assert.That(VectorESp(e1, e3), Is.EqualTo(0.0).Within(Tolerance), "E1 and E3 are orthogonal");
        Assert.That(VectorESp(e2, e3), Is.EqualTo(0.0).Within(Tolerance), "E2 and E3 are orthogonal");
    }

    #endregion

    #region Basic Operations (3 tests × 2 implementations = 6 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorAddition_ShouldAddComponents(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 2.0, 3.0, useGeneric);
        var v2 = CreateVector(4.0, 5.0, 6.0, useGeneric);

        // Act
        var result = Add(v1, v2);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(5.0).Within(Tolerance), "X: 1 + 4 = 5");
        Assert.That(GetY(result), Is.EqualTo(7.0).Within(Tolerance), "Y: 2 + 5 = 7");
        Assert.That(GetZ(result), Is.EqualTo(9.0).Within(Tolerance), "Z: 3 + 6 = 9");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorSubtraction_ShouldSubtractComponents(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(10.0, 8.0, 6.0, useGeneric);
        var v2 = CreateVector(3.0, 2.0, 1.0, useGeneric);

        // Act
        var result = Subtract(v1, v2);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(7.0).Within(Tolerance), "X: 10 - 3 = 7");
        Assert.That(GetY(result), Is.EqualTo(6.0).Within(Tolerance), "Y: 8 - 2 = 6");
        Assert.That(GetZ(result), Is.EqualTo(5.0).Within(Tolerance), "Z: 6 - 1 = 5");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorScalarMultiplication_ShouldScaleComponents(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(1.0, 2.0, 3.0, useGeneric);
        var scalar = 3.0;

        // Act
        var result = Multiply(vector, scalar);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(3.0).Within(Tolerance), "X: 1 * 3 = 3");
        Assert.That(GetY(result), Is.EqualTo(6.0).Within(Tolerance), "Y: 2 * 3 = 6");
        Assert.That(GetZ(result), Is.EqualTo(9.0).Within(Tolerance), "Z: 3 * 3 = 9");
    }

    #endregion

    #region Norm and Dot Product (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorNorm_ShouldCalculateEuclideanNorm(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(2.0, 3.0, 6.0, useGeneric);

        // Act
        var norm = VectorENorm(vector);

        // Assert
        // ||v|| = sqrt(2^2 + 3^2 + 6^2) = sqrt(4 + 9 + 36) = sqrt(49) = 7
        Assert.That(norm, Is.EqualTo(7.0).Within(Tolerance), "Norm of (2,3,6) should be 7");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void DotProduct_ShouldCalculateScalarProduct(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 2.0, 3.0, useGeneric);
        var v2 = CreateVector(4.0, 5.0, 6.0, useGeneric);

        // Act
        var dotProduct = VectorESp(v1, v2);

        // Assert
        // v1 · v2 = 1*4 + 2*5 + 3*6 = 4 + 10 + 18 = 32
        Assert.That(dotProduct, Is.EqualTo(32.0).Within(Tolerance), "Dot product should be 32");
    }

    #endregion

    #region Cross Product (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void CrossProduct_ShouldCalculateVectorProduct(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 0.0, 0.0, useGeneric);  // X-axis
        var v2 = CreateVector(0.0, 1.0, 0.0, useGeneric);  // Y-axis

        // Act
        var crossProduct = VectorCross(v1, v2);

        // Assert
        // X × Y = Z
        Assert.That(GetX(crossProduct), Is.EqualTo(0.0).Within(Tolerance), "X component should be 0");
        Assert.That(GetY(crossProduct), Is.EqualTo(0.0).Within(Tolerance), "Y component should be 0");
        Assert.That(GetZ(crossProduct), Is.EqualTo(1.0).Within(Tolerance), "Z component should be 1 (right-hand rule)");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void CrossProduct_ShouldBeOrthogonalToBothVectors(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 2.0, 3.0, useGeneric);
        var v2 = CreateVector(4.0, 5.0, 6.0, useGeneric);

        // Act
        var crossProduct = VectorCross(v1, v2);

        // Assert
        // Cross product should be orthogonal to both input vectors
        var dot1 = VectorESp(crossProduct, v1);
        var dot2 = VectorESp(crossProduct, v2);

        Assert.That(dot1, Is.EqualTo(0.0).Within(Tolerance),
            "Cross product should be orthogonal to v1");
        Assert.That(dot2, Is.EqualTo(0.0).Within(Tolerance),
            "Cross product should be orthogonal to v2");
    }

    #endregion

    #region Normalization (1 test × 2 implementations = 2 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorNormalization_ShouldProduceUnitVector(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(3.0, 4.0, 0.0, useGeneric);

        // Act
        var unitVector = ToUnitVector(vector);
        var norm = VectorENorm(unitVector);

        // Assert
        Assert.That(norm, Is.EqualTo(1.0).Within(Tolerance), "Unit vector should have norm 1");
        Assert.That(IsNearUnit(unitVector), Is.True, "Vector should be near unit length");

        // Direction should be preserved: (3/5, 4/5, 0)
        Assert.That(GetX(unitVector), Is.EqualTo(0.6).Within(Tolerance), "X component: 3/5 = 0.6");
        Assert.That(GetY(unitVector), Is.EqualTo(0.8).Within(Tolerance), "Y component: 4/5 = 0.8");
        Assert.That(GetZ(unitVector), Is.EqualTo(0.0).Within(Tolerance), "Z component: 0/5 = 0");
    }

    #endregion
}
