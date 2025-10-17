using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.Euclidean;

/// <summary>
/// Tests for Euclidean Bivector Operations
/// Phase 3B - Core Modeling: Euclidean Geometry Bivector Operations (10 tests)
/// Tests bivector construction, operations, and properties
/// </summary>
[TestFixture]
public class LinFloat64BivectorTests
{
    private const double Tolerance = 1e-10;

    #region Bivector2D Tests (4 tests)

    [Test]
    public void Bivector2D_Construction_ShouldWork()
    {
        // Arrange & Act
        var b = LinFloat64Bivector2D.Create(5);

        // Assert
        Assert.That(b, Is.Not.Null, "Bivector should be created");
        Assert.That(b.Xy.ScalarValue, Is.EqualTo(5), "Xy component should match");
    }

    [Test]
    public void Bivector2D_Addition_ShouldWork()
    {
        // Arrange
        var b1 = LinFloat64Bivector2D.Create(2);
        var b2 = LinFloat64Bivector2D.Create(3);

        // Act
        var result = b1 + b2;

        // Assert
        Assert.That(result.Xy.ScalarValue, Is.EqualTo(5).Within(Tolerance), "Xy should be 2+3=5");
    }

    [Test]
    public void Bivector2D_ScalarMultiplication_ShouldWork()
    {
        // Arrange
        var b = LinFloat64Bivector2D.Create(4);

        // Act
        var result = b * 2.5;

        // Assert
        Assert.That(result.Xy.ScalarValue, Is.EqualTo(10).Within(Tolerance), "Xy should be 4*2.5=10");
    }

    [Test]
    public void Bivector2D_Negation_ShouldWork()
    {
        // Arrange
        var b = LinFloat64Bivector2D.Create(3);

        // Act
        var result = -b;

        // Assert
        Assert.That(result.Xy.ScalarValue, Is.EqualTo(-3).Within(Tolerance), "Negation should flip sign");
    }

    #endregion

    #region Bivector3D Tests (6 tests)

    [Test]
    public void Bivector3D_Construction_ShouldWork()
    {
        // Arrange & Act
        var b = LinFloat64Bivector3D.Create(1, 2, 3);

        // Assert
        Assert.That(b, Is.Not.Null, "Bivector should be created");
        Assert.That(b.Xy.ScalarValue, Is.EqualTo(1), "Xy component should match");
        Assert.That(b.Xz.ScalarValue, Is.EqualTo(2), "Xz component should match");
        Assert.That(b.Yz.ScalarValue, Is.EqualTo(3), "Yz component should match");
    }

    [Test]
    public void Bivector3D_BasisBivectors_ShouldBeCorrect()
    {
        // Arrange & Act
        var e12 = LinFloat64Bivector3D.E12;
        var e13 = LinFloat64Bivector3D.E13;
        var e23 = LinFloat64Bivector3D.E23;

        // Assert
        Assert.That(e12.Xy.ScalarValue, Is.EqualTo(1), "E12.Xy should be 1");
        Assert.That(e12.Xz.ScalarValue, Is.EqualTo(0), "E12.Xz should be 0");
        Assert.That(e12.Yz.ScalarValue, Is.EqualTo(0), "E12.Yz should be 0");

        Assert.That(e13.Xy.ScalarValue, Is.EqualTo(0), "E13.Xy should be 0");
        Assert.That(e13.Xz.ScalarValue, Is.EqualTo(1), "E13.Xz should be 1");
        Assert.That(e13.Yz.ScalarValue, Is.EqualTo(0), "E13.Yz should be 0");

        Assert.That(e23.Xy.ScalarValue, Is.EqualTo(0), "E23.Xy should be 0");
        Assert.That(e23.Xz.ScalarValue, Is.EqualTo(0), "E23.Xz should be 0");
        Assert.That(e23.Yz.ScalarValue, Is.EqualTo(1), "E23.Yz should be 1");
    }

    [Test]
    public void Bivector3D_Addition_ShouldWork()
    {
        // Arrange
        var b1 = LinFloat64Bivector3D.Create(1, 2, 3);
        var b2 = LinFloat64Bivector3D.Create(4, 5, 6);

        // Act
        var result = b1 + b2;

        // Assert
        Assert.That(result.Xy.ScalarValue, Is.EqualTo(5).Within(Tolerance));
        Assert.That(result.Xz.ScalarValue, Is.EqualTo(7).Within(Tolerance));
        Assert.That(result.Yz.ScalarValue, Is.EqualTo(9).Within(Tolerance));
    }

    [Test]
    public void Bivector3D_Subtraction_ShouldWork()
    {
        // Arrange
        var b1 = LinFloat64Bivector3D.Create(10, 8, 6);
        var b2 = LinFloat64Bivector3D.Create(1, 2, 3);

        // Act
        var result = b1 - b2;

        // Assert
        Assert.That(result.Xy.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        Assert.That(result.Xz.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.Yz.ScalarValue, Is.EqualTo(3).Within(Tolerance));
    }

    [Test]
    public void Bivector3D_ScalarMultiplication_ShouldWork()
    {
        // Arrange
        var b = LinFloat64Bivector3D.Create(2, 3, 4);

        // Act
        var result = b * 3;

        // Assert
        Assert.That(result.Xy.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(result.Xz.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        Assert.That(result.Yz.ScalarValue, Is.EqualTo(12).Within(Tolerance));
    }

    [Test]
    public void Bivector3D_Zero_ShouldBeZeroBivector()
    {
        // Arrange & Act
        var zero = LinFloat64Bivector3D.Zero;

        // Assert
        Assert.That(zero.Xy.ScalarValue, Is.EqualTo(0), "Zero.Xy should be 0");
        Assert.That(zero.Xz.ScalarValue, Is.EqualTo(0), "Zero.Xz should be 0");
        Assert.That(zero.Yz.ScalarValue, Is.EqualTo(0), "Zero.Yz should be 0");
    }

    #endregion
}
