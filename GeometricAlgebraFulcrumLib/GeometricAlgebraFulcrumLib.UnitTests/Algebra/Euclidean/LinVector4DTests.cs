using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space4D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space4D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for 4D Euclidean Vector Operations
/// EXTENDED: Tests both Float64 AND Generic&lt;double&gt; implementations
/// Phase 2 - Validates API parity between Float64 and Generic&lt;double&gt;
/// </summary>
[TestFixture]
public class LinVector4DTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Helper Methods

    private object CreateVector(double x, double y, double z, double w, bool useGeneric)
    {
        if (useGeneric)
        {
            return LinVector4D<double>.Create(
                _scalarProcessor.ScalarFromNumber(x),
                _scalarProcessor.ScalarFromNumber(y),
                _scalarProcessor.ScalarFromNumber(z),
                _scalarProcessor.ScalarFromNumber(w)
            );
        }
        else
        {
            return LinFloat64Vector4D.Create(x, y, z, w);
        }
    }

    private object GetZero(bool useGeneric)
    {
        return useGeneric
            ? LinVector4D<double>.Zero(_scalarProcessor)
            : LinFloat64Vector4D.Zero;
    }

    private object GetE1(bool useGeneric)
    {
        return useGeneric
            ? LinVector4D<double>.E1(_scalarProcessor)
            : LinFloat64Vector4D.E1;
    }

    private object GetE2(bool useGeneric)
    {
        return useGeneric
            ? LinVector4D<double>.E2(_scalarProcessor)
            : LinFloat64Vector4D.E2;
    }

    private object GetE3(bool useGeneric)
    {
        return useGeneric
            ? LinVector4D<double>.E3(_scalarProcessor)
            : LinFloat64Vector4D.E3;
    }

    private object GetE4(bool useGeneric)
    {
        return useGeneric
            ? LinVector4D<double>.E4(_scalarProcessor)
            : LinFloat64Vector4D.E4;
    }

    private double GetX(object vector)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64.X.ScalarValue,
            LinVector4D<double> gen => gen.X.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double GetY(object vector)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64.Y.ScalarValue,
            LinVector4D<double> gen => gen.Y.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double GetZ(object vector)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64.Z.ScalarValue,
            LinVector4D<double> gen => gen.Z.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double GetW(object vector)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64.W.ScalarValue,
            LinVector4D<double> gen => gen.W.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private object Add(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector4D f1, LinFloat64Vector4D f2) => f1 + f2,
            (LinVector4D<double> g1, LinVector4D<double> g2) => g1 + g2,
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object Subtract(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector4D f1, LinFloat64Vector4D f2) => f1 - f2,
            (LinVector4D<double> g1, LinVector4D<double> g2) => g1 - g2,
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object Multiply(object vector, double scalar)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64 * scalar,
            LinVector4D<double> gen => gen * scalar,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private object Divide(object vector, double scalar)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64 / scalar,
            LinVector4D<double> gen => gen / scalar,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private object Negate(object vector)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => -f64,
            LinVector4D<double> gen => -gen,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double VectorENorm(object vector)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64.VectorENorm(),
            LinVector4D<double> gen => gen.VectorENorm().ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double VectorENormSquared(object vector)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64.VectorENormSquared(),
            LinVector4D<double> gen => gen.VectorENormSquared().ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double VectorESp(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector4D f1, LinFloat64Vector4D f2) => f1.VectorESp(f2).ScalarValue,
            (LinVector4D<double> g1, LinVector4D<double> g2) => g1.VectorESp(g2).ScalarValue,
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object ToUnitVector(object vector)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64.ToUnitLinVector4D(),
            LinVector4D<double> gen => gen.ToUnitLinVector4D(),
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private bool IsNearUnit(object vector)
    {
        return vector switch
        {
            LinFloat64Vector4D f64 => f64.IsNearUnit(),
            LinVector4D<double> gen => gen.IsNearUnit(),
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private bool IsNearOrthogonalTo(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector4D f1, LinFloat64Vector4D f2) => f1.IsNearOrthogonalTo(f2),
            (LinVector4D<double> g1, LinVector4D<double> g2) => g1.IsNearOrthogonalTo(g2),
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    #endregion

    #region Construction Tests (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void CreateVector_ShouldHaveCorrectComponents(bool useGeneric)
    {
        // Arrange & Act
        var vector = CreateVector(1.0, 2.0, 3.0, 4.0, useGeneric);

        // Assert
        Assert.That(GetX(vector), Is.EqualTo(1.0).Within(Tolerance), "X component should be 1.0");
        Assert.That(GetY(vector), Is.EqualTo(2.0).Within(Tolerance), "Y component should be 2.0");
        Assert.That(GetZ(vector), Is.EqualTo(3.0).Within(Tolerance), "Z component should be 3.0");
        Assert.That(GetW(vector), Is.EqualTo(4.0).Within(Tolerance), "W component should be 4.0");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void BasisVectors_ShouldBeOrthonormal(bool useGeneric)
    {
        // Arrange
        var e1 = GetE1(useGeneric);
        var e2 = GetE2(useGeneric);
        var e3 = GetE3(useGeneric);
        var e4 = GetE4(useGeneric);

        // Act & Assert - Check unit length
        Assert.That(VectorENorm(e1), Is.EqualTo(1.0).Within(Tolerance), "E1 should be unit length");
        Assert.That(VectorENorm(e2), Is.EqualTo(1.0).Within(Tolerance), "E2 should be unit length");
        Assert.That(VectorENorm(e3), Is.EqualTo(1.0).Within(Tolerance), "E3 should be unit length");
        Assert.That(VectorENorm(e4), Is.EqualTo(1.0).Within(Tolerance), "E4 should be unit length");

        // Check orthogonality
        Assert.That(VectorESp(e1, e2), Is.EqualTo(0.0).Within(Tolerance), "E1 and E2 are orthogonal");
        Assert.That(VectorESp(e1, e3), Is.EqualTo(0.0).Within(Tolerance), "E1 and E3 are orthogonal");
        Assert.That(VectorESp(e1, e4), Is.EqualTo(0.0).Within(Tolerance), "E1 and E4 are orthogonal");
        Assert.That(VectorESp(e2, e3), Is.EqualTo(0.0).Within(Tolerance), "E2 and E3 are orthogonal");
        Assert.That(VectorESp(e2, e4), Is.EqualTo(0.0).Within(Tolerance), "E2 and E4 are orthogonal");
        Assert.That(VectorESp(e3, e4), Is.EqualTo(0.0).Within(Tolerance), "E3 and E4 are orthogonal");
    }

    #endregion

    #region Basic Operations (4 tests × 2 implementations = 8 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorAddition_ShouldAddComponents(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 2.0, 3.0, 4.0, useGeneric);
        var v2 = CreateVector(5.0, 6.0, 7.0, 8.0, useGeneric);

        // Act
        var result = Add(v1, v2);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(6.0).Within(Tolerance), "X: 1 + 5 = 6");
        Assert.That(GetY(result), Is.EqualTo(8.0).Within(Tolerance), "Y: 2 + 6 = 8");
        Assert.That(GetZ(result), Is.EqualTo(10.0).Within(Tolerance), "Z: 3 + 7 = 10");
        Assert.That(GetW(result), Is.EqualTo(12.0).Within(Tolerance), "W: 4 + 8 = 12");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorSubtraction_ShouldSubtractComponents(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(10.0, 9.0, 8.0, 7.0, useGeneric);
        var v2 = CreateVector(1.0, 2.0, 3.0, 4.0, useGeneric);

        // Act
        var result = Subtract(v1, v2);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(9.0).Within(Tolerance), "X: 10 - 1 = 9");
        Assert.That(GetY(result), Is.EqualTo(7.0).Within(Tolerance), "Y: 9 - 2 = 7");
        Assert.That(GetZ(result), Is.EqualTo(5.0).Within(Tolerance), "Z: 8 - 3 = 5");
        Assert.That(GetW(result), Is.EqualTo(3.0).Within(Tolerance), "W: 7 - 4 = 3");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorScalarMultiplication_ShouldScaleComponents(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(1.0, 2.0, 3.0, 4.0, useGeneric);
        var scalar = 2.5;

        // Act
        var result = Multiply(vector, scalar);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(2.5).Within(Tolerance), "X: 1 * 2.5 = 2.5");
        Assert.That(GetY(result), Is.EqualTo(5.0).Within(Tolerance), "Y: 2 * 2.5 = 5.0");
        Assert.That(GetZ(result), Is.EqualTo(7.5).Within(Tolerance), "Z: 3 * 2.5 = 7.5");
        Assert.That(GetW(result), Is.EqualTo(10.0).Within(Tolerance), "W: 4 * 2.5 = 10.0");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorNegation_ShouldNegateComponents(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(1.0, -2.0, 3.0, -4.0, useGeneric);

        // Act
        var result = Negate(vector);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(-1.0).Within(Tolerance), "X should be negated");
        Assert.That(GetY(result), Is.EqualTo(2.0).Within(Tolerance), "Y should be negated");
        Assert.That(GetZ(result), Is.EqualTo(-3.0).Within(Tolerance), "Z should be negated");
        Assert.That(GetW(result), Is.EqualTo(4.0).Within(Tolerance), "W should be negated");
    }

    #endregion

    #region Norm and Distance (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorNorm_ShouldCalculateEuclideanNorm(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(2.0, 2.0, 1.0, 0.0, useGeneric);

        // Act
        var norm = VectorENorm(vector);

        // Assert
        // ||v|| = sqrt(2^2 + 2^2 + 1^2 + 0^2) = sqrt(4 + 4 + 1 + 0) = sqrt(9) = 3
        Assert.That(norm, Is.EqualTo(3.0).Within(Tolerance), "Norm of (2,2,1,0) should be 3");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorNormSquared_ShouldCalculateSquaredNorm(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(1.0, 2.0, 3.0, 4.0, useGeneric);

        // Act
        var normSquared = VectorENormSquared(vector);

        // Assert
        // ||v||² = 1² + 2² + 3² + 4² = 1 + 4 + 9 + 16 = 30
        Assert.That(normSquared, Is.EqualTo(30.0).Within(Tolerance), "Norm squared of (1,2,3,4) should be 30");
    }

    #endregion

    #region Dot Product and Angle (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void DotProduct_ShouldCalculateScalarProduct(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 2.0, 3.0, 4.0, useGeneric);
        var v2 = CreateVector(5.0, 6.0, 7.0, 8.0, useGeneric);

        // Act
        var dotProduct = VectorESp(v1, v2);

        // Assert
        // v1 · v2 = 1*5 + 2*6 + 3*7 + 4*8 = 5 + 12 + 21 + 32 = 70
        Assert.That(dotProduct, Is.EqualTo(70.0).Within(Tolerance), "Dot product should be 70");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void OrthogonalVectors_ShouldHaveZeroDotProduct(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 0.0, 0.0, 0.0, useGeneric);
        var v2 = CreateVector(0.0, 1.0, 0.0, 0.0, useGeneric);

        // Act
        var dotProduct = VectorESp(v1, v2);
        var isOrthogonal = IsNearOrthogonalTo(v1, v2);

        // Assert
        Assert.That(dotProduct, Is.EqualTo(0.0).Within(Tolerance), "Orthogonal vectors have zero dot product");
        Assert.That(isOrthogonal, Is.True, "Vectors should be orthogonal");
    }

    #endregion

    #region Normalization (1 test × 2 implementations = 2 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorNormalization_ShouldProduceUnitVector(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(2.0, 2.0, 1.0, 0.0, useGeneric);

        // Act
        var unitVector = ToUnitVector(vector);
        var norm = VectorENorm(unitVector);

        // Assert
        Assert.That(norm, Is.EqualTo(1.0).Within(Tolerance), "Unit vector should have norm 1");
        Assert.That(IsNearUnit(unitVector), Is.True, "Vector should be near unit length");

        // Direction should be preserved: (2/3, 2/3, 1/3, 0)
        Assert.That(GetX(unitVector), Is.EqualTo(2.0/3.0).Within(Tolerance), "X component: 2/3");
        Assert.That(GetY(unitVector), Is.EqualTo(2.0/3.0).Within(Tolerance), "Y component: 2/3");
        Assert.That(GetZ(unitVector), Is.EqualTo(1.0/3.0).Within(Tolerance), "Z component: 1/3");
        Assert.That(GetW(unitVector), Is.EqualTo(0.0).Within(Tolerance), "W component: 0");
    }

    #endregion

    #region Zero Vector (1 test × 2 implementations = 2 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void ZeroVector_ShouldHaveAllZeroComponents(bool useGeneric)
    {
        // Arrange & Act
        var zero = GetZero(useGeneric);

        // Assert
        Assert.That(GetX(zero), Is.EqualTo(0.0).Within(Tolerance), "Zero.X should be 0");
        Assert.That(GetY(zero), Is.EqualTo(0.0).Within(Tolerance), "Zero.Y should be 0");
        Assert.That(GetZ(zero), Is.EqualTo(0.0).Within(Tolerance), "Zero.Z should be 0");
        Assert.That(GetW(zero), Is.EqualTo(0.0).Within(Tolerance), "Zero.W should be 0");
    }

    #endregion

    #region Division (1 test × 2 implementations = 2 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorScalarDivision_ShouldDivideComponents(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(6.0, 9.0, 12.0, 15.0, useGeneric);
        var scalar = 3.0;

        // Act
        var result = Divide(vector, scalar);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(2.0).Within(Tolerance), "X: 6 / 3 = 2");
        Assert.That(GetY(result), Is.EqualTo(3.0).Within(Tolerance), "Y: 9 / 3 = 3");
        Assert.That(GetZ(result), Is.EqualTo(4.0).Within(Tolerance), "Z: 12 / 3 = 4");
        Assert.That(GetW(result), Is.EqualTo(5.0).Within(Tolerance), "W: 15 / 3 = 5");
    }

    #endregion
}
