using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for 3D Euclidean Vector Operations
/// Phase 3B - Core Modeling: Euclidean Geometry (10 tests)
/// </summary>
[TestFixture]
public class LinFloat64Vector3DTests
{
    private const double Tolerance = 1e-10;

    #region Construction Tests (2 tests)

    [Test]
    public void CreateVector_ShouldHaveCorrectComponents()
    {
        // Arrange & Act
        var vector = LinFloat64Vector3D.Create(2.0, 3.0, 4.0);

        // Assert
        Assert.That(vector.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "X component should be 2.0");
        Assert.That(vector.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Y component should be 3.0");
        Assert.That(vector.Z.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Z component should be 4.0");
    }

    [Test]
    public void BasisVectors_ShouldBeOrthonormal()
    {
        // Arrange
        var e1 = LinFloat64Vector3D.E1;
        var e2 = LinFloat64Vector3D.E2;
        var e3 = LinFloat64Vector3D.E3;

        // Act & Assert - Check unit length
        Assert.That(e1.VectorENorm().ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "E1 should be unit length");
        Assert.That(e2.VectorENorm().ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "E2 should be unit length");
        Assert.That(e3.VectorENorm().ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "E3 should be unit length");

        // Check orthogonality
        Assert.That(e1.VectorESp(e2).ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "E1 and E2 are orthogonal");
        Assert.That(e1.VectorESp(e3).ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "E1 and E3 are orthogonal");
        Assert.That(e2.VectorESp(e3).ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "E2 and E3 are orthogonal");
    }

    #endregion

    #region Basic Operations (3 tests)

    [Test]
    public void VectorAddition_ShouldAddComponents()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var v2 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);

        // Act
        var result = v1 + v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X: 1 + 4 = 5");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(7.0).Within(Tolerance), "Y: 2 + 5 = 7");
        Assert.That(result.Z.ScalarValue, Is.EqualTo(9.0).Within(Tolerance), "Z: 3 + 6 = 9");
    }

    [Test]
    public void VectorSubtraction_ShouldSubtractComponents()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(10.0, 8.0, 6.0);
        var v2 = LinFloat64Vector3D.Create(3.0, 2.0, 1.0);

        // Act
        var result = v1 - v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(7.0).Within(Tolerance), "X: 10 - 3 = 7");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "Y: 8 - 2 = 6");
        Assert.That(result.Z.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Z: 6 - 1 = 5");
    }

    [Test]
    public void VectorScalarMultiplication_ShouldScaleComponents()
    {
        // Arrange
        var vector = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var scalar = 3.0;

        // Act
        var result = vector * scalar;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "X: 1 * 3 = 3");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "Y: 2 * 3 = 6");
        Assert.That(result.Z.ScalarValue, Is.EqualTo(9.0).Within(Tolerance), "Z: 3 * 3 = 9");
    }

    #endregion

    #region Norm and Dot Product (2 tests)

    [Test]
    public void VectorNorm_ShouldCalculateEuclideanNorm()
    {
        // Arrange
        var vector = LinFloat64Vector3D.Create(2.0, 3.0, 6.0);

        // Act
        var norm = vector.VectorENorm();

        // Assert
        // ||v|| = sqrt(2^2 + 3^2 + 6^2) = sqrt(4 + 9 + 36) = sqrt(49) = 7
        Assert.That(norm.ScalarValue, Is.EqualTo(7.0).Within(Tolerance), "Norm of (2,3,6) should be 7");
    }

    [Test]
    public void DotProduct_ShouldCalculateScalarProduct()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var v2 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);

        // Act
        var dotProduct = v1.VectorESp(v2);

        // Assert
        // v1 · v2 = 1*4 + 2*5 + 3*6 = 4 + 10 + 18 = 32
        Assert.That(dotProduct.ScalarValue, Is.EqualTo(32.0).Within(Tolerance), "Dot product should be 32");
    }

    #endregion

    #region Cross Product (2 tests)

    [Test]
    public void CrossProduct_ShouldCalculateVectorProduct()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(1.0, 0.0, 0.0);  // X-axis
        var v2 = LinFloat64Vector3D.Create(0.0, 1.0, 0.0);  // Y-axis

        // Act
        var crossProduct = v1.VectorCross(v2);

        // Assert
        // X × Y = Z
        Assert.That(crossProduct.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X component should be 0");
        Assert.That(crossProduct.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y component should be 0");
        Assert.That(crossProduct.Z.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Z component should be 1 (right-hand rule)");
    }

    [Test]
    public void CrossProduct_ShouldBeOrthogonalToBothVectors()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var v2 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);

        // Act
        var crossProduct = v1.VectorCross(v2);

        // Assert
        // Cross product should be orthogonal to both input vectors
        var dot1 = crossProduct.VectorESp(v1);
        var dot2 = crossProduct.VectorESp(v2);

        Assert.That(dot1.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Cross product should be orthogonal to v1");
        Assert.That(dot2.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "Cross product should be orthogonal to v2");
    }

    #endregion

    #region Normalization (1 test)

    [Test]
    public void VectorNormalization_ShouldProduceUnitVector()
    {
        // Arrange
        var vector = LinFloat64Vector3D.Create(3.0, 4.0, 0.0);

        // Act
        var unitVector = vector.ToUnitLinVector3D();
        var norm = unitVector.VectorENorm();

        // Assert
        Assert.That(norm.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Unit vector should have norm 1");
        Assert.That(unitVector.IsNearUnit(), Is.True, "Vector should be near unit length");

        // Direction should be preserved: (3/5, 4/5, 0)
        Assert.That(unitVector.X.ScalarValue, Is.EqualTo(0.6).Within(Tolerance), "X component: 3/5 = 0.6");
        Assert.That(unitVector.Y.ScalarValue, Is.EqualTo(0.8).Within(Tolerance), "Y component: 4/5 = 0.8");
        Assert.That(unitVector.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z component: 0/5 = 0");
    }

    #endregion
}
