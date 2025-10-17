using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space4D;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.Euclidean;

/// <summary>
/// Tests for Euclidean Vector Operations
/// Phase 3B - Core Modeling: Euclidean Geometry Vector Operations (20 tests)
/// Tests vector construction, operations, and properties in 2D, 3D, and 4D
/// </summary>
[TestFixture]
public class LinFloat64VectorTests
{
    private const double Tolerance = 1e-10;

    #region Vector2D Tests (5 tests)

    [Test]
    public void Vector2D_Construction_ShouldWork()
    {
        // Arrange & Act
        var v = LinFloat64Vector2D.Create(3, 4);

        // Assert
        Assert.That(v, Is.Not.Null, "Vector should be created");
        Assert.That(v.X.ScalarValue, Is.EqualTo(3), "X component should match");
        Assert.That(v.Y.ScalarValue, Is.EqualTo(4), "Y component should match");
    }

    [Test]
    public void Vector2D_Addition_ShouldWork()
    {
        // Arrange
        var v1 = LinFloat64Vector2D.Create(1, 2);
        var v2 = LinFloat64Vector2D.Create(3, 4);

        // Act
        var result = v1 + v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(4).Within(Tolerance), "X should be 1+3=4");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance), "Y should be 2+4=6");
    }

    [Test]
    public void Vector2D_Subtraction_ShouldWork()
    {
        // Arrange
        var v1 = LinFloat64Vector2D.Create(5, 7);
        var v2 = LinFloat64Vector2D.Create(2, 3);

        // Act
        var result = v1 - v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(3).Within(Tolerance), "X should be 5-2=3");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(4).Within(Tolerance), "Y should be 7-3=4");
    }

    [Test]
    public void Vector2D_ScalarMultiplication_ShouldWork()
    {
        // Arrange
        var v = LinFloat64Vector2D.Create(2, 3);

        // Act
        var result = v * 2.5;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(5).Within(Tolerance), "X should be 2*2.5=5");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(7.5).Within(Tolerance), "Y should be 3*2.5=7.5");
    }

    [Test]
    public void Vector2D_Negation_ShouldWork()
    {
        // Arrange
        var v = LinFloat64Vector2D.Create(3, -4);

        // Act
        var result = -v;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(-3).Within(Tolerance), "X should be negated");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(4).Within(Tolerance), "Y should be negated");
    }

    #endregion

    #region Vector3D Tests (8 tests)

    [Test]
    public void Vector3D_Construction_ShouldWork()
    {
        // Arrange & Act
        var v = LinFloat64Vector3D.Create(1, 2, 3);

        // Assert
        Assert.That(v, Is.Not.Null, "Vector should be created");
        Assert.That(v.X.ScalarValue, Is.EqualTo(1), "X component should match");
        Assert.That(v.Y.ScalarValue, Is.EqualTo(2), "Y component should match");
        Assert.That(v.Z.ScalarValue, Is.EqualTo(3), "Z component should match");
    }

    [Test]
    public void Vector3D_BasisVectors_ShouldBeCorrect()
    {
        // Arrange & Act
        var e1 = LinFloat64Vector3D.E1;
        var e2 = LinFloat64Vector3D.E2;
        var e3 = LinFloat64Vector3D.E3;

        // Assert
        Assert.That(e1.X.ScalarValue, Is.EqualTo(1), "E1.X should be 1");
        Assert.That(e1.Y.ScalarValue, Is.EqualTo(0), "E1.Y should be 0");
        Assert.That(e1.Z.ScalarValue, Is.EqualTo(0), "E1.Z should be 0");

        Assert.That(e2.X.ScalarValue, Is.EqualTo(0), "E2.X should be 0");
        Assert.That(e2.Y.ScalarValue, Is.EqualTo(1), "E2.Y should be 1");
        Assert.That(e2.Z.ScalarValue, Is.EqualTo(0), "E2.Z should be 0");

        Assert.That(e3.X.ScalarValue, Is.EqualTo(0), "E3.X should be 0");
        Assert.That(e3.Y.ScalarValue, Is.EqualTo(0), "E3.Y should be 0");
        Assert.That(e3.Z.ScalarValue, Is.EqualTo(1), "E3.Z should be 1");
    }

    [Test]
    public void Vector3D_Addition_ShouldWork()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(1, 2, 3);
        var v2 = LinFloat64Vector3D.Create(4, 5, 6);

        // Act
        var result = v1 + v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(5).Within(Tolerance));
        Assert.That(result.Y.ScalarValue, Is.EqualTo(7).Within(Tolerance));
        Assert.That(result.Z.ScalarValue, Is.EqualTo(9).Within(Tolerance));
    }

    [Test]
    public void Vector3D_Subtraction_ShouldWork()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(10, 8, 6);
        var v2 = LinFloat64Vector3D.Create(1, 2, 3);

        // Act
        var result = v1 - v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.Z.ScalarValue, Is.EqualTo(3).Within(Tolerance));
    }

    [Test]
    public void Vector3D_ScalarMultiplication_ShouldWork()
    {
        // Arrange
        var v = LinFloat64Vector3D.Create(1, 2, 3);

        // Act
        var result = v * 3;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(3).Within(Tolerance));
        Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.Z.ScalarValue, Is.EqualTo(9).Within(Tolerance));
    }

    [Test]
    public void Vector3D_ScalarDivision_ShouldWork()
    {
        // Arrange
        var v = LinFloat64Vector3D.Create(6, 9, 12);

        // Act
        var result = v / 3;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(2).Within(Tolerance));
        Assert.That(result.Y.ScalarValue, Is.EqualTo(3).Within(Tolerance));
        Assert.That(result.Z.ScalarValue, Is.EqualTo(4).Within(Tolerance));
    }

    [Test]
    public void Vector3D_Negation_ShouldWork()
    {
        // Arrange
        var v = LinFloat64Vector3D.Create(1, -2, 3);

        // Act
        var result = -v;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
        Assert.That(result.Y.ScalarValue, Is.EqualTo(2).Within(Tolerance));
        Assert.That(result.Z.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
    }

    [Test]
    public void Vector3D_Zero_ShouldBeZeroVector()
    {
        // Arrange & Act
        var zero = LinFloat64Vector3D.Zero;

        // Assert
        Assert.That(zero.X.ScalarValue, Is.EqualTo(0), "Zero.X should be 0");
        Assert.That(zero.Y.ScalarValue, Is.EqualTo(0), "Zero.Y should be 0");
        Assert.That(zero.Z.ScalarValue, Is.EqualTo(0), "Zero.Z should be 0");
    }

    #endregion

    #region Vector4D Tests (4 tests)

    [Test]
    public void Vector4D_Construction_ShouldWork()
    {
        // Arrange & Act
        var v = LinFloat64Vector4D.Create(1, 2, 3, 4);

        // Assert
        Assert.That(v, Is.Not.Null, "Vector should be created");
        Assert.That(v.X.ScalarValue, Is.EqualTo(1), "X component should match");
        Assert.That(v.Y.ScalarValue, Is.EqualTo(2), "Y component should match");
        Assert.That(v.Z.ScalarValue, Is.EqualTo(3), "Z component should match");
        Assert.That(v.W.ScalarValue, Is.EqualTo(4), "W component should match");
    }

    [Test]
    public void Vector4D_Addition_ShouldWork()
    {
        // Arrange
        var v1 = LinFloat64Vector4D.Create(1, 2, 3, 4);
        var v2 = LinFloat64Vector4D.Create(5, 6, 7, 8);

        // Act
        var result = v1 + v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.Y.ScalarValue, Is.EqualTo(8).Within(Tolerance));
        Assert.That(result.Z.ScalarValue, Is.EqualTo(10).Within(Tolerance));
        Assert.That(result.W.ScalarValue, Is.EqualTo(12).Within(Tolerance));
    }

    [Test]
    public void Vector4D_ScalarMultiplication_ShouldWork()
    {
        // Arrange
        var v = LinFloat64Vector4D.Create(1, 2, 3, 4);

        // Act
        var result = v * 2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(2).Within(Tolerance));
        Assert.That(result.Y.ScalarValue, Is.EqualTo(4).Within(Tolerance));
        Assert.That(result.Z.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.W.ScalarValue, Is.EqualTo(8).Within(Tolerance));
    }

    [Test]
    public void Vector4D_Negation_ShouldWork()
    {
        // Arrange
        var v = LinFloat64Vector4D.Create(1, -2, 3, -4);

        // Act
        var result = -v;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
        Assert.That(result.Y.ScalarValue, Is.EqualTo(2).Within(Tolerance));
        Assert.That(result.Z.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
        Assert.That(result.W.ScalarValue, Is.EqualTo(4).Within(Tolerance));
    }

    #endregion

    #region Mixed Dimension Tests (3 tests)

    [Test]
    public void Vector_AllDimensions_ShouldSupportBasicOperations()
    {
        // Arrange
        var v2d = LinFloat64Vector2D.Create(1, 2);
        var v3d = LinFloat64Vector3D.Create(1, 2, 3);
        var v4d = LinFloat64Vector4D.Create(1, 2, 3, 4);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var _ = v2d + v2d;
            var __ = v3d + v3d;
            var ___ = v4d + v4d;
        }, "All dimensions should support addition");

        Assert.DoesNotThrow(() =>
        {
            var _ = v2d * 2;
            var __ = v3d * 2;
            var ___ = v4d * 2;
        }, "All dimensions should support scalar multiplication");
    }

    [Test]
    public void Vector_ChainedOperations_ShouldWork()
    {
        // Arrange
        var v1 = LinFloat64Vector3D.Create(1, 2, 3);
        var v2 = LinFloat64Vector3D.Create(4, 5, 6);
        var v3 = LinFloat64Vector3D.Create(7, 8, 9);

        // Act
        var result = (v1 + v2) * 2 - v3;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(3).Within(Tolerance), "Chained operations should work");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.Z.ScalarValue, Is.EqualTo(9).Within(Tolerance));
    }

    [Test]
    public void Vector_OperatorConsistency_ShouldBeValid()
    {
        // Arrange
        var v = LinFloat64Vector3D.Create(2, 4, 6);

        // Act
        var scaled1 = v * 0.5;
        var scaled2 = v / 2;

        // Assert - multiplication by 0.5 should equal division by 2
        Assert.That(scaled1.X.ScalarValue, Is.EqualTo(scaled2.X.ScalarValue).Within(Tolerance));
        Assert.That(scaled1.Y.ScalarValue, Is.EqualTo(scaled2.Y.ScalarValue).Within(Tolerance));
        Assert.That(scaled1.Z.ScalarValue, Is.EqualTo(scaled2.Z.ScalarValue).Within(Tolerance));
    }

    #endregion
}
