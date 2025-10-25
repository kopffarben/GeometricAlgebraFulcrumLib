using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for 2D and 3D Euclidean Bivector Operations
/// EXTENDED: Tests both Float64 AND Generic&lt;double&gt; implementations
/// Phase 2 - Validates API parity between Float64 and Generic&lt;double&gt;
/// Bivectors represent oriented areas (2D) or oriented planes (3D)
/// </summary>
[TestFixture]
public class LinBivectorTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Helper Methods - 2D Bivectors

    private object CreateBivector2D(double scalar12, bool useGeneric)
    {
        if (useGeneric)
        {
            return LinBivector2D<double>.Create(
                _scalarProcessor.ScalarFromNumber(scalar12)
            );
        }
        else
        {
            return LinFloat64Bivector2D.Create(scalar12);
        }
    }

    private object GetBivector2DE12(bool useGeneric)
    {
        return useGeneric
            ? LinBivector2D<double>.E12(_scalarProcessor)
            : LinFloat64Bivector2D.E12;
    }

    private object GetBivector2DE21(bool useGeneric)
    {
        return useGeneric
            ? LinBivector2D<double>.E21(_scalarProcessor)
            : LinFloat64Bivector2D.E21;
    }

    private double GetScalar12_2D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector2D f64 => f64.Scalar12.ScalarValue,
            LinBivector2D<double> gen => gen.Scalar12.ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double GetXy_2D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector2D f64 => f64.Xy.ScalarValue,
            LinBivector2D<double> gen => gen.Xy.ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private object Dual2D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector2D f64 => f64.Dual2D(),
            LinBivector2D<double> gen => gen.Dual2D(),
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private object UnDual2D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector2D f64 => f64.UnDual2D(),
            LinBivector2D<double> gen => gen.UnDual2D(),
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double GetScalarFromScalar2D(object scalar)
    {
        return scalar switch
        {
            LinFloat64Scalar2D f64 => f64.Scalar.ScalarValue,
            LinScalar2D<double> gen => gen.Scalar.ScalarValue,
            _ => throw new ArgumentException($"Unexpected scalar type: {scalar.GetType()}")
        };
    }

    private double Sp2D(object b1, object b2)
    {
        return (b1, b2) switch
        {
            (LinFloat64Bivector2D f1, LinFloat64Bivector2D f2) => f1.Sp(f2).ScalarValue,
            (LinBivector2D<double> g1, LinBivector2D<double> g2) => g1.Sp(g2).ScalarValue,
            _ => throw new ArgumentException("Mixed bivector types")
        };
    }

    #endregion

    #region Helper Methods - 3D Bivectors

    private object CreateBivector3D(double scalar12, double scalar13, double scalar23, bool useGeneric)
    {
        if (useGeneric)
        {
            return LinBivector3D<double>.Create(
                _scalarProcessor.ScalarFromNumber(scalar12),
                _scalarProcessor.ScalarFromNumber(scalar13),
                _scalarProcessor.ScalarFromNumber(scalar23)
            );
        }
        else
        {
            return LinFloat64Bivector3D.Create(scalar12, scalar13, scalar23);
        }
    }

    private object GetBivector3DE12(bool useGeneric)
    {
        return useGeneric
            ? LinBivector3D<double>.E12(_scalarProcessor)
            : LinFloat64Bivector3D.E12;
    }

    private object GetBivector3DE13(bool useGeneric)
    {
        return useGeneric
            ? LinBivector3D<double>.E13(_scalarProcessor)
            : LinFloat64Bivector3D.E13;
    }

    private object GetBivector3DE23(bool useGeneric)
    {
        return useGeneric
            ? LinBivector3D<double>.E23(_scalarProcessor)
            : LinFloat64Bivector3D.E23;
    }

    private double GetScalar12_3D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector3D f64 => f64.Scalar12.ScalarValue,
            LinBivector3D<double> gen => gen.Scalar12.ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double GetScalar13_3D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector3D f64 => f64.Scalar13.ScalarValue,
            LinBivector3D<double> gen => gen.Scalar13.ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double GetScalar23_3D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector3D f64 => f64.Scalar23.ScalarValue,
            LinBivector3D<double> gen => gen.Scalar23.ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double GetXy_3D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector3D f64 => f64.Xy.ScalarValue,
            LinBivector3D<double> gen => gen.Xy.ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double GetXz_3D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector3D f64 => f64.Xz.ScalarValue,
            LinBivector3D<double> gen => gen.Xz.ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double GetYz_3D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector3D f64 => f64.Yz.ScalarValue,
            LinBivector3D<double> gen => gen.Yz.ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double Norm3D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector3D f64 => f64.Norm().ScalarValue,
            LinBivector3D<double> gen => gen.Norm().ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double NormSquared3D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector3D f64 => f64.NormSquared().ScalarValue,
            LinBivector3D<double> gen => gen.NormSquared().ScalarValue,
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
    }

    private double Sp3D(object b1, object b2)
    {
        return (b1, b2) switch
        {
            (LinFloat64Bivector3D f1, LinFloat64Bivector3D f2) => f1.Sp(f2).ScalarValue,
            (LinBivector3D<double> g1, LinBivector3D<double> g2) => g1.Sp(g2).ScalarValue,
            _ => throw new ArgumentException("Mixed bivector types")
        };
    }

    private object Dual3D(object bivector)
    {
        return bivector switch
        {
            LinFloat64Bivector3D f64 => f64.Dual3D(),
            LinBivector3D<double> gen => gen.Dual3D(),
            _ => throw new ArgumentException($"Unexpected bivector type: {bivector.GetType()}")
        };
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

    #endregion

    #region 2D Bivector Tests (4 tests × 2 implementations = 8 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector2D_Construction_ShouldHaveCorrectComponent(bool useGeneric)
    {
        // Arrange & Act
        var bivector = CreateBivector2D(5.0, useGeneric);

        // Assert
        Assert.That(GetScalar12_2D(bivector), Is.EqualTo(5.0).Within(Tolerance),
            "Bivector2D should have correct Scalar12 component");
        Assert.That(GetXy_2D(bivector), Is.EqualTo(5.0).Within(Tolerance),
            "Xy property should match Scalar12");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector2D_BasisBivectors_ShouldBeCorrect(bool useGeneric)
    {
        // Arrange
        var e12 = GetBivector2DE12(useGeneric);
        var e21 = GetBivector2DE21(useGeneric);

        // Assert
        Assert.That(GetScalar12_2D(e12), Is.EqualTo(1.0).Within(Tolerance), "E12 should be +1");
        Assert.That(GetScalar12_2D(e21), Is.EqualTo(-1.0).Within(Tolerance), "E21 should be -1");

        // E12 and E21 are anti-commutative: e21 = -e12
        Assert.That(GetScalar12_2D(e21), Is.EqualTo(-GetScalar12_2D(e12)).Within(Tolerance),
            "E21 should be negative of E12");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector2D_Dual_ShouldConvertToScalar(bool useGeneric)
    {
        // Arrange
        var bivector = CreateBivector2D(3.0, useGeneric);

        // Act
        var dual = Dual2D(bivector);

        // Assert
        // In 2D, the dual of a bivector is a scalar (pseudoscalar)
        Assert.That(GetScalarFromScalar2D(dual), Is.EqualTo(3.0).Within(Tolerance),
            "Dual of bivector e12 should be scalar");

        // Test UnDual (should negate)
        var unDual = UnDual2D(bivector);
        Assert.That(GetScalarFromScalar2D(unDual), Is.EqualTo(-3.0).Within(Tolerance),
            "UnDual should negate the scalar");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector2D_ScalarProduct_ShouldCalculateCorrectly(bool useGeneric)
    {
        // Arrange
        var b1 = CreateBivector2D(2.0, useGeneric);
        var b2 = CreateBivector2D(3.0, useGeneric);

        // Act
        var scalarProduct = Sp2D(b1, b2);

        // Assert
        // Scalar product of bivectors: b1 · b2 = -(Scalar12_1 * Scalar12_2)
        // For b1 = 2*e12 and b2 = 3*e12: Sp = -(2*3) = -6
        Assert.That(scalarProduct, Is.EqualTo(-6.0).Within(Tolerance),
            "Scalar product of parallel bivectors should be negative of product");
    }

    #endregion

    #region 3D Bivector Tests (4 tests × 2 implementations = 8 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_Construction_ShouldHaveCorrectComponents(bool useGeneric)
    {
        // Arrange & Act
        var bivector = CreateBivector3D(1.0, 2.0, 3.0, useGeneric);

        // Assert
        Assert.That(GetScalar12_3D(bivector), Is.EqualTo(1.0).Within(Tolerance), "Scalar12 should be 1");
        Assert.That(GetScalar13_3D(bivector), Is.EqualTo(2.0).Within(Tolerance), "Scalar13 should be 2");
        Assert.That(GetScalar23_3D(bivector), Is.EqualTo(3.0).Within(Tolerance), "Scalar23 should be 3");

        // Test alternative property names
        Assert.That(GetXy_3D(bivector), Is.EqualTo(1.0).Within(Tolerance), "Xy = Scalar12");
        Assert.That(GetXz_3D(bivector), Is.EqualTo(2.0).Within(Tolerance), "Xz = Scalar13");
        Assert.That(GetYz_3D(bivector), Is.EqualTo(3.0).Within(Tolerance), "Yz = Scalar23");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_BasisBivectors_ShouldBeOrthogonal(bool useGeneric)
    {
        // Arrange
        var e12 = GetBivector3DE12(useGeneric);
        var e13 = GetBivector3DE13(useGeneric);
        var e23 = GetBivector3DE23(useGeneric);

        // Assert - Each basis bivector should be unit
        Assert.That(Norm3D(e12), Is.EqualTo(1.0).Within(Tolerance), "E12 should be unit");
        Assert.That(Norm3D(e13), Is.EqualTo(1.0).Within(Tolerance), "E13 should be unit");
        Assert.That(Norm3D(e23), Is.EqualTo(1.0).Within(Tolerance), "E23 should be unit");

        // Assert - Basis bivectors should be orthogonal (scalar product = 0)
        Assert.That(Sp3D(e12, e13), Is.EqualTo(0.0).Within(Tolerance),
            "E12 and E13 are orthogonal");
        Assert.That(Sp3D(e12, e23), Is.EqualTo(0.0).Within(Tolerance),
            "E12 and E23 are orthogonal");
        Assert.That(Sp3D(e13, e23), Is.EqualTo(0.0).Within(Tolerance),
            "E13 and E23 are orthogonal");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_Dual_ShouldConvertToVector(bool useGeneric)
    {
        // Arrange
        var bivector = CreateBivector3D(1.0, 2.0, 3.0, useGeneric);

        // Act
        var dual = Dual3D(bivector);

        // Assert
        // In 3D, the dual of a bivector is a vector (Hodge dual)
        // Dual(e12) = e3, Dual(e13) = -e2, Dual(e23) = e1
        Assert.That(GetX(dual), Is.EqualTo(3.0).Within(Tolerance),
            "X component = Scalar23");
        Assert.That(GetY(dual), Is.EqualTo(-2.0).Within(Tolerance),
            "Y component = -Scalar13");
        Assert.That(GetZ(dual), Is.EqualTo(1.0).Within(Tolerance),
            "Z component = Scalar12");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_Norm_ShouldCalculateCorrectly(bool useGeneric)
    {
        // Arrange
        var bivector = CreateBivector3D(2.0, 3.0, 6.0, useGeneric);

        // Act
        var norm = Norm3D(bivector);
        var normSquared = NormSquared3D(bivector);

        // Assert
        // ||B|| = sqrt(Scalar12² + Scalar13² + Scalar23²)
        // ||B|| = sqrt(2² + 3² + 6²) = sqrt(4 + 9 + 36) = sqrt(49) = 7
        Assert.That(normSquared, Is.EqualTo(49.0).Within(Tolerance),
            "Norm squared should be 49");
        Assert.That(norm, Is.EqualTo(7.0).Within(Tolerance),
            "Norm should be 7");
    }

    #endregion
}
