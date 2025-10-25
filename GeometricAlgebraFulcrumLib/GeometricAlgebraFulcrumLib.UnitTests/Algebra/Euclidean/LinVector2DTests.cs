using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for 2D Euclidean Vector Operations
/// EXTENDED: Tests both Float64 AND Generic&lt;double&gt; implementations
/// Phase 2 - Validates API parity between Float64 and Generic&lt;double&gt;
/// </summary>
[TestFixture]
public class LinVector2DTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Helper Methods

    private object CreateVector(double x, double y, bool useGeneric)
    {
        if (useGeneric)
        {
            return LinVector2D<double>.Create(
                _scalarProcessor.ScalarFromNumber(x),
                _scalarProcessor.ScalarFromNumber(y)
            );
        }
        else
        {
            return LinFloat64Vector2D.Create(x, y);
        }
    }

    private object CreateFromPolar(double length, double degrees, bool useGeneric)
    {
        if (useGeneric)
        {
            var angle = LinPolarAngle<double>.CreateFromDegrees(
                _scalarProcessor.ScalarFromNumber(degrees)
            );
            return LinVector2D<double>.CreateFromPolar(
                _scalarProcessor.ScalarFromNumber(length),
                angle
            );
        }
        else
        {
            var angle = LinFloat64PolarAngle.CreateFromDegrees(degrees);
            return LinFloat64Vector2D.CreateFromPolar(length, angle);
        }
    }

    private double GetX(object vector)
    {
        return vector switch
        {
            LinFloat64Vector2D f64 => f64.X.ScalarValue,
            LinVector2D<double> gen => gen.X.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double GetY(object vector)
    {
        return vector switch
        {
            LinFloat64Vector2D f64 => f64.Y.ScalarValue,
            LinVector2D<double> gen => gen.Y.ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private object Add(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector2D f1, LinFloat64Vector2D f2) => f1 + f2,
            (LinVector2D<double> g1, LinVector2D<double> g2) => g1 + g2,
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object Subtract(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector2D f1, LinFloat64Vector2D f2) => f1 - f2,
            (LinVector2D<double> g1, LinVector2D<double> g2) => g1 - g2,
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object Multiply(object vector, double scalar)
    {
        return vector switch
        {
            LinFloat64Vector2D f64 => f64 * scalar,
            LinVector2D<double> gen => gen * _scalarProcessor.ScalarFromNumber(scalar),
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double VectorENorm(object vector)
    {
        return vector switch
        {
            LinFloat64Vector2D f64 => f64.VectorENorm(),
            LinVector2D<double> gen => gen.VectorENorm().ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double VectorENormSquared(object vector)
    {
        return vector switch
        {
            LinFloat64Vector2D f64 => f64.VectorENormSquared().ScalarValue,
            LinVector2D<double> gen => gen.VectorENormSquared().ScalarValue,
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private double VectorESp(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector2D f1, LinFloat64Vector2D f2) => f1.VectorESp(f2).ScalarValue,
            (LinVector2D<double> g1, LinVector2D<double> g2) => g1.VectorESp(g2).ScalarValue,
            _ => throw new ArgumentException("Mixed vector types")
        };
    }

    private object ToUnitVector(object vector)
    {
        return vector switch
        {
            LinFloat64Vector2D f64 => f64.ToUnitLinVector2D(),
            LinVector2D<double> gen => gen.ToUnitLinVector2D(), // Now identical API!
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private bool IsNearUnit(object vector)
    {
        return vector switch
        {
            LinFloat64Vector2D f64 => f64.IsNearUnit(),
            LinVector2D<double> gen => gen.IsNearUnit(), // Now identical API!
            _ => throw new ArgumentException($"Unexpected vector type: {vector.GetType()}")
        };
    }

    private bool IsNearOrthogonalTo(object v1, object v2)
    {
        return (v1, v2) switch
        {
            (LinFloat64Vector2D f1, LinFloat64Vector2D f2) => f1.IsNearOrthogonalTo(f2),
            (LinVector2D<double> g1, LinVector2D<double> g2) => g1.IsNearOrthogonalTo(g2), // Now identical API!
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
        var vector = CreateVector(3.0, 4.0, useGeneric);

        // Assert
        Assert.That(GetX(vector), Is.EqualTo(3.0).Within(Tolerance), "X component should be 3.0");
        Assert.That(GetY(vector), Is.EqualTo(4.0).Within(Tolerance), "Y component should be 4.0");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void CreateFromPolar_ShouldHaveCorrectCartesianComponents(bool useGeneric)
    {
        // Arrange & Act
        var vector = CreateFromPolar(5.0, 90, useGeneric);

        // Assert
        // 90 degrees: cos(90°) = 0, sin(90°) = 1
        Assert.That(GetX(vector), Is.EqualTo(0.0).Within(Tolerance), "X component should be 0 at 90°");
        Assert.That(GetY(vector), Is.EqualTo(5.0).Within(Tolerance), "Y component should be 5 at 90°");
    }

    #endregion

    #region Basic Operations (3 tests × 2 implementations = 6 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorAddition_ShouldAddComponents(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 2.0, useGeneric);
        var v2 = CreateVector(3.0, 4.0, useGeneric);

        // Act
        var result = Add(v1, v2);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(4.0).Within(Tolerance), "X: 1 + 3 = 4");
        Assert.That(GetY(result), Is.EqualTo(6.0).Within(Tolerance), "Y: 2 + 4 = 6");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorSubtraction_ShouldSubtractComponents(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(5.0, 7.0, useGeneric);
        var v2 = CreateVector(2.0, 3.0, useGeneric);

        // Act
        var result = Subtract(v1, v2);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(3.0).Within(Tolerance), "X: 5 - 2 = 3");
        Assert.That(GetY(result), Is.EqualTo(4.0).Within(Tolerance), "Y: 7 - 3 = 4");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorScalarMultiplication_ShouldScaleComponents(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(2.0, 3.0, useGeneric);
        var scalar = 4.0;

        // Act
        var result = Multiply(vector, scalar);

        // Assert
        Assert.That(GetX(result), Is.EqualTo(8.0).Within(Tolerance), "X: 2 * 4 = 8");
        Assert.That(GetY(result), Is.EqualTo(12.0).Within(Tolerance), "Y: 3 * 4 = 12");
    }

    #endregion

    #region Norm and Distance (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorNorm_ShouldCalculateEuclideanNorm(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(3.0, 4.0, useGeneric);

        // Act
        var norm = VectorENorm(vector);

        // Assert
        // ||v|| = sqrt(3^2 + 4^2) = sqrt(9 + 16) = sqrt(25) = 5
        Assert.That(norm, Is.EqualTo(5.0).Within(Tolerance), "Norm of (3,4) should be 5");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorNormSquared_ShouldCalculateSquaredNorm(bool useGeneric)
    {
        // Arrange
        var vector = CreateVector(3.0, 4.0, useGeneric);

        // Act
        var normSquared = VectorENormSquared(vector);

        // Assert
        // ||v||² = 3² + 4² = 9 + 16 = 25
        Assert.That(normSquared, Is.EqualTo(25.0).Within(Tolerance), "Norm squared of (3,4) should be 25");
    }

    #endregion

    #region Dot Product and Angle (2 tests × 2 implementations = 4 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void DotProduct_ShouldCalculateScalarProduct(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 2.0, useGeneric);
        var v2 = CreateVector(3.0, 4.0, useGeneric);

        // Act
        var dotProduct = VectorESp(v1, v2);

        // Assert
        // v1 · v2 = 1*3 + 2*4 = 3 + 8 = 11
        Assert.That(dotProduct, Is.EqualTo(11.0).Within(Tolerance), "Dot product should be 11");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void OrthogonalVectors_ShouldHaveZeroDotProduct(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 0.0, useGeneric);
        var v2 = CreateVector(0.0, 1.0, useGeneric);

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
        var vector = CreateVector(3.0, 4.0, useGeneric);

        // Act
        var unitVector = ToUnitVector(vector);
        var norm = VectorENorm(unitVector);

        // Assert
        Assert.That(norm, Is.EqualTo(1.0).Within(Tolerance), "Unit vector should have norm 1");
        Assert.That(IsNearUnit(unitVector), Is.True, "Vector should be near unit length");

        // Direction should be preserved: (3/5, 4/5)
        Assert.That(GetX(unitVector), Is.EqualTo(0.6).Within(Tolerance), "X component: 3/5 = 0.6");
        Assert.That(GetY(unitVector), Is.EqualTo(0.8).Within(Tolerance), "Y component: 4/5 = 0.8");
    }

    #endregion
}
