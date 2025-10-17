using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for 2D and 3D Euclidean Bivector Operations
/// Phase 3B - Core Modeling: Euclidean Geometry (8 tests)
/// Bivectors represent oriented areas (2D) or oriented planes (3D)
/// </summary>
[TestFixture]
public class LinFloat64BivectorTests
{
    private const double Tolerance = 1e-10;

    #region 2D Bivector Tests (4 tests)

    [Test]
    public void Bivector2D_Construction_ShouldHaveCorrectComponent()
    {
        // Arrange & Act
        var bivector = LinFloat64Bivector2D.Create(5.0);

        // Assert
        Assert.That(bivector.Scalar12.ScalarValue, Is.EqualTo(5.0).Within(Tolerance),
            "Bivector2D should have correct Scalar12 component");
        Assert.That(bivector.Xy.ScalarValue, Is.EqualTo(5.0).Within(Tolerance),
            "Xy property should match Scalar12");
    }

    [Test]
    public void Bivector2D_BasisBivectors_ShouldBeCorrect()
    {
        // Arrange
        var e12 = LinFloat64Bivector2D.E12;
        var e21 = LinFloat64Bivector2D.E21;

        // Assert
        Assert.That(e12.Scalar12.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "E12 should be +1");
        Assert.That(e21.Scalar12.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "E21 should be -1");

        // E12 and E21 are anti-commutative: e21 = -e12
        Assert.That(e21.Scalar12.ScalarValue, Is.EqualTo(-e12.Scalar12.ScalarValue).Within(Tolerance),
            "E21 should be negative of E12");
    }

    [Test]
    public void Bivector2D_Dual_ShouldConvertToScalar()
    {
        // Arrange
        var bivector = LinFloat64Bivector2D.Create(3.0);

        // Act
        var dual = bivector.Dual2D();

        // Assert
        // In 2D, the dual of a bivector is a scalar (pseudoscalar)
        Assert.That(dual.Scalar.ScalarValue, Is.EqualTo(3.0).Within(Tolerance),
            "Dual of bivector e12 should be scalar");

        // Test UnDual (should negate)
        var unDual = bivector.UnDual2D();
        Assert.That(unDual.Scalar.ScalarValue, Is.EqualTo(-3.0).Within(Tolerance),
            "UnDual should negate the scalar");
    }

    [Test]
    public void Bivector2D_ScalarProduct_ShouldCalculateCorrectly()
    {
        // Arrange
        var b1 = LinFloat64Bivector2D.Create(2.0);
        var b2 = LinFloat64Bivector2D.Create(3.0);

        // Act
        var scalarProduct = b1.Sp(b2);

        // Assert
        // Scalar product of bivectors: b1 · b2 = -(Scalar12_1 * Scalar12_2)
        // For b1 = 2*e12 and b2 = 3*e12: Sp = -(2*3) = -6
        Assert.That(scalarProduct.ScalarValue, Is.EqualTo(-6.0).Within(Tolerance),
            "Scalar product of parallel bivectors should be negative of product");
    }

    #endregion

    #region 3D Bivector Tests (4 tests)

    [Test]
    public void Bivector3D_Construction_ShouldHaveCorrectComponents()
    {
        // Arrange & Act
        var bivector = LinFloat64Bivector3D.Create(1.0, 2.0, 3.0);

        // Assert
        Assert.That(bivector.Scalar12.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Scalar12 should be 1");
        Assert.That(bivector.Scalar13.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Scalar13 should be 2");
        Assert.That(bivector.Scalar23.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Scalar23 should be 3");

        // Test alternative property names
        Assert.That(bivector.Xy.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Xy = Scalar12");
        Assert.That(bivector.Xz.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Xz = Scalar13");
        Assert.That(bivector.Yz.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Yz = Scalar23");
    }

    [Test]
    public void Bivector3D_BasisBivectors_ShouldBeOrthogonal()
    {
        // Arrange
        var e12 = LinFloat64Bivector3D.E12;
        var e13 = LinFloat64Bivector3D.E13;
        var e23 = LinFloat64Bivector3D.E23;

        // Assert - Each basis bivector should be unit
        Assert.That(e12.Norm().ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "E12 should be unit");
        Assert.That(e13.Norm().ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "E13 should be unit");
        Assert.That(e23.Norm().ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "E23 should be unit");

        // Assert - Basis bivectors should be orthogonal (scalar product = 0)
        Assert.That(e12.Sp(e13).ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "E12 and E13 are orthogonal");
        Assert.That(e12.Sp(e23).ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "E12 and E23 are orthogonal");
        Assert.That(e13.Sp(e23).ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "E13 and E23 are orthogonal");
    }

    [Test]
    public void Bivector3D_Dual_ShouldConvertToVector()
    {
        // Arrange
        var bivector = LinFloat64Bivector3D.Create(1.0, 2.0, 3.0);

        // Act
        var dual = bivector.Dual3D();

        // Assert
        // In 3D, the dual of a bivector is a vector (Hodge dual)
        // Dual(e12) = e3, Dual(e13) = -e2, Dual(e23) = e1
        Assert.That(dual.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance),
            "X component = Scalar23");
        Assert.That(dual.Y.ScalarValue, Is.EqualTo(-2.0).Within(Tolerance),
            "Y component = -Scalar13");
        Assert.That(dual.Z.ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Z component = Scalar12");
    }

    [Test]
    public void Bivector3D_Norm_ShouldCalculateCorrectly()
    {
        // Arrange
        var bivector = LinFloat64Bivector3D.Create(2.0, 3.0, 6.0);

        // Act
        var norm = bivector.Norm();
        var normSquared = bivector.NormSquared();

        // Assert
        // ||B|| = sqrt(Scalar12² + Scalar13² + Scalar23²)
        // ||B|| = sqrt(2² + 3² + 6²) = sqrt(4 + 9 + 36) = sqrt(49) = 7
        Assert.That(normSquared.ScalarValue, Is.EqualTo(49.0).Within(Tolerance),
            "Norm squared should be 49");
        Assert.That(norm.ScalarValue, Is.EqualTo(7.0).Within(Tolerance),
            "Norm should be 7");
    }

    #endregion
}
