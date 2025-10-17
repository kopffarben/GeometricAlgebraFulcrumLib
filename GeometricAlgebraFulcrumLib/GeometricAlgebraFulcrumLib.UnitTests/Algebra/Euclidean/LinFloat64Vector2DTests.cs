using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for 2D Euclidean Vector Operations
/// Phase 3B - Core Modeling: Euclidean Geometry (10 tests)
/// </summary>
[TestFixture]
public class LinFloat64Vector2DTests
{
    private const double Tolerance = 1e-10;

    #region Construction Tests (2 tests)

    [Test]
    public void CreateVector_ShouldHaveCorrectComponents()
    {
        // Arrange & Act
        var vector = LinFloat64Vector2D.Create(3.0, 4.0);

        // Assert
        Assert.That(vector.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "X component should be 3.0");
        Assert.That(vector.Y.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Y component should be 4.0");
    }

    [Test]
    public void CreateFromPolar_ShouldHaveCorrectCartesianComponents()
    {
        // Arrange
        var angle = LinFloat64PolarAngle.CreateFromDegrees(90);
        var length = 5.0;

        // Act
        var vector = LinFloat64Vector2D.CreateFromPolar(length, angle);

        // Assert
        // 90 degrees: cos(90°) = 0, sin(90°) = 1
        Assert.That(vector.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X component should be 0 at 90°");
        Assert.That(vector.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Y component should be 5 at 90°");
    }

    #endregion

    #region Basic Operations (3 tests)

    [Test]
    public void VectorAddition_ShouldAddComponents()
    {
        // Arrange
        var v1 = LinFloat64Vector2D.Create(1.0, 2.0);
        var v2 = LinFloat64Vector2D.Create(3.0, 4.0);

        // Act
        var result = v1 + v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "X: 1 + 3 = 4");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "Y: 2 + 4 = 6");
    }

    [Test]
    public void VectorSubtraction_ShouldSubtractComponents()
    {
        // Arrange
        var v1 = LinFloat64Vector2D.Create(5.0, 7.0);
        var v2 = LinFloat64Vector2D.Create(2.0, 3.0);

        // Act
        var result = v1 - v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "X: 5 - 2 = 3");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Y: 7 - 3 = 4");
    }

    [Test]
    public void VectorScalarMultiplication_ShouldScaleComponents()
    {
        // Arrange
        var vector = LinFloat64Vector2D.Create(2.0, 3.0);
        var scalar = 4.0;

        // Act
        var result = vector * scalar;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(8.0).Within(Tolerance), "X: 2 * 4 = 8");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(12.0).Within(Tolerance), "Y: 3 * 4 = 12");
    }

    #endregion

    #region Norm and Distance (2 tests)

    [Test]
    public void VectorNorm_ShouldCalculateEuclideanNorm()
    {
        // Arrange
        var vector = LinFloat64Vector2D.Create(3.0, 4.0);

        // Act
        var norm = vector.VectorENorm();

        // Assert
        // ||v|| = sqrt(3^2 + 4^2) = sqrt(9 + 16) = sqrt(25) = 5
        Assert.That(norm, Is.EqualTo(5.0).Within(Tolerance), "Norm of (3,4) should be 5");
    }

    [Test]
    public void VectorNormSquared_ShouldCalculateSquaredNorm()
    {
        // Arrange
        var vector = LinFloat64Vector2D.Create(3.0, 4.0);

        // Act
        var normSquared = vector.VectorENormSquared();

        // Assert
        // ||v||² = 3² + 4² = 9 + 16 = 25
        // VectorENormSquared returns Float64Scalar
        Assert.That(normSquared.ScalarValue, Is.EqualTo(25.0).Within(Tolerance), "Norm squared of (3,4) should be 25");
    }

    #endregion

    #region Dot Product and Angle (2 tests)

    [Test]
    public void DotProduct_ShouldCalculateScalarProduct()
    {
        // Arrange
        var v1 = LinFloat64Vector2D.Create(1.0, 2.0);
        var v2 = LinFloat64Vector2D.Create(3.0, 4.0);

        // Act
        var dotProduct = v1.VectorESp(v2);

        // Assert
        // v1 · v2 = 1*3 + 2*4 = 3 + 8 = 11
        Assert.That(dotProduct.ScalarValue, Is.EqualTo(11.0).Within(Tolerance), "Dot product should be 11");
    }

    [Test]
    public void OrthogonalVectors_ShouldHaveZeroDotProduct()
    {
        // Arrange
        var v1 = LinFloat64Vector2D.Create(1.0, 0.0);
        var v2 = LinFloat64Vector2D.Create(0.0, 1.0);

        // Act
        var dotProduct = v1.VectorESp(v2);
        var isOrthogonal = v1.IsNearOrthogonalTo(v2);

        // Assert
        Assert.That(dotProduct.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Orthogonal vectors have zero dot product");
        Assert.That(isOrthogonal, Is.True, "Vectors should be orthogonal");
    }

    #endregion

    #region Normalization (1 test)

    [Test]
    public void VectorNormalization_ShouldProduceUnitVector()
    {
        // Arrange
        var vector = LinFloat64Vector2D.Create(3.0, 4.0);

        // Act
        var unitVector = vector.ToUnitLinVector2D();
        var norm = unitVector.VectorENorm();

        // Assert
        Assert.That(norm, Is.EqualTo(1.0).Within(Tolerance), "Unit vector should have norm 1");
        Assert.That(unitVector.IsNearUnit(), Is.True, "Vector should be near unit length");

        // Direction should be preserved: (3/5, 4/5)
        Assert.That(unitVector.X.ScalarValue, Is.EqualTo(0.6).Within(Tolerance), "X component: 3/5 = 0.6");
        Assert.That(unitVector.Y.ScalarValue, Is.EqualTo(0.8).Within(Tolerance), "Y component: 4/5 = 0.8");
    }

    #endregion
}
