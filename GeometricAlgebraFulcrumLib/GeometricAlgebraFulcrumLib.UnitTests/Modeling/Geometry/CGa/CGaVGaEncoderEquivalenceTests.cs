using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Unit tests for CGA VGa Encoder equivalence - Milestone 1.2 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; encoders produce identical CGA vectors.
///
/// Note: Generic encoders have MORE methods (Hybrid API with overloads for T, IScalar&lt;T&gt;, double)
/// while Float64 is minimalistic. Tests verify functional equivalence, not API parity.
/// </summary>
[TestFixture]
public class CGaVGaEncoderEquivalenceTests
{
    private CGaFloat64GeometricSpace4D _float64Space4D = null!;
    private CGaGeometricSpace<double> _genericSpace4D = null!;
    private CGaFloat64GeometricSpace5D _float64Space5D = null!;
    private CGaGeometricSpace<double> _genericSpace5D = null!;

    [SetUp]
    public void Setup()
    {
        _float64Space4D = CGaFloat64GeometricSpace4D.Instance;
        _genericSpace4D = CGaGeometricSpace4D<double>.Create(
            ScalarProcessorOfFloat64.Instance
        );

        _float64Space5D = CGaFloat64GeometricSpace5D.Instance;
        _genericSpace5D = CGaGeometricSpace5D<double>.Create(
            ScalarProcessorOfFloat64.Instance
        );
    }

    #region VectorAsXGaVector Tests (4D CGA)

    [Test]
    public void VectorAsXGaVector_2D_FromDoubles_ShouldProduceIdenticalVectors()
    {
        // Arrange
        double x = 1.5, y = 2.5;

        // Act
        var float64Vector = _float64Space4D.Encode.VGa.VectorAsXGaVector(x, y);
        var genericVector = _genericSpace4D.Encode.VGa.VectorAsXGaVector(x, y);

        // Assert - Both should encode as (0, 0, x, y) in 4D CGA
        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count), "Same number of terms");

        for (int i = 0; i < 4; i++)
        {
            Assert.That(genericVector.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Vector.GetTermScalarByIndex(i)).Within(1e-14),
                $"Term at index {i} should match");
        }

        // Verify encoding structure: (0, 0, x, y)
        Assert.That(float64Vector.GetTermScalarByIndex(0), Is.EqualTo(0.0), "Index 0 should be 0");
        Assert.That(float64Vector.GetTermScalarByIndex(1), Is.EqualTo(0.0), "Index 1 should be 0");
        Assert.That(float64Vector.GetTermScalarByIndex(2), Is.EqualTo(x).Within(1e-14));
        Assert.That(float64Vector.GetTermScalarByIndex(3), Is.EqualTo(y).Within(1e-14));
    }

    #endregion

    #region VectorAsXGaVector Tests (5D CGA)

    [Test]
    public void VectorAsXGaVector_3D_FromDoubles_ShouldProduceIdenticalVectors()
    {
        // Arrange
        double x = 1.5, y = 2.5, z = 3.5;

        // Act
        var float64Vector = _float64Space5D.Encode.VGa.VectorAsXGaVector(x, y, z);
        var genericVector = _genericSpace5D.Encode.VGa.VectorAsXGaVector(x, y, z);

        // Assert - Both should encode as (0, 0, x, y, z) in 5D CGA
        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));

        for (int i = 0; i < 5; i++)
        {
            Assert.That(genericVector.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Vector.GetTermScalarByIndex(i)).Within(1e-14),
                $"Term at index {i} should match");
        }

        // Verify encoding structure: (0, 0, x, y, z)
        Assert.That(float64Vector.GetTermScalarByIndex(0), Is.EqualTo(0.0));
        Assert.That(float64Vector.GetTermScalarByIndex(1), Is.EqualTo(0.0));
        Assert.That(float64Vector.GetTermScalarByIndex(2), Is.EqualTo(x).Within(1e-14));
        Assert.That(float64Vector.GetTermScalarByIndex(3), Is.EqualTo(y).Within(1e-14));
        Assert.That(float64Vector.GetTermScalarByIndex(4), Is.EqualTo(z).Within(1e-14));
    }

    #endregion

    #region Vector (CGaFloat64Blade) Tests

    [Test]
    public void Vector_3D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 1.5, y = 2.5, z = 3.5;

        // Act
        var float64Blade = _float64Space5D.Encode.VGa.Vector(x, y, z);
        var genericBlade = _genericSpace5D.Encode.VGa.Vector(x, y, z);

        // Assert - Compare the underlying XGaVector
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));

        for (int i = 0; i < 5; i++)
        {
            Assert.That(genericVector.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Vector.GetTermScalarByIndex(i)).Within(1e-14),
                $"Blade internal vector term at index {i} should match");
        }
    }

    [Test]
    public void Vector_2D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 1.5, y = 2.5;

        // Act
        var float64Blade = _float64Space4D.Encode.VGa.Vector(x, y);
        var genericBlade = _genericSpace4D.Encode.VGa.Vector(x, y);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));

        for (int i = 0; i < 4; i++)
        {
            Assert.That(genericVector.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Vector.GetTermScalarByIndex(i)).Within(1e-14),
                $"Blade term at index {i} should match");
        }
    }

    #endregion

    #region Bivector Tests

    [Test]
    public void Bivector_3D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange - Bivector in 3D space (e.g., e_1 ∧ e_2)
        double xy = 1.5, xz = 2.5, yz = 3.5;

        // Act
        var float64Blade = _float64Space5D.Encode.VGa.Bivector(xy, xz, yz);

        // Generic version needs IScalar<T> parameters
        var scalarProcessor = _genericSpace5D.ScalarProcessor;
        var genericBlade = _genericSpace5D.Encode.VGa.Bivector(
            scalarProcessor.ScalarFromValue(xy),
            scalarProcessor.ScalarFromValue(xz),
            scalarProcessor.ScalarFromValue(yz)
        );

        // Assert
        var float64Bivector = float64Blade.InternalBivector;
        var genericBivector = genericBlade.InternalBivector;

        Assert.That(genericBivector.Count, Is.EqualTo(float64Bivector.Count));

        foreach (var pair in float64Bivector.IdScalarPairs)
        {
            Assert.That(genericBivector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Bivector term at {pair.Key} should match");
        }
    }

    #endregion

    #region Trivector Tests (3D only)

    [Test]
    public void Trivector_3D_FromDouble_ShouldProduceIdenticalBlades()
    {
        // Arrange - Trivector in 3D space (e_1 ∧ e_2 ∧ e_3)
        double xyz = 2.5;

        // Act
        var float64Blade = _float64Space5D.Encode.VGa.Trivector(xyz);

        // Generic version needs IScalar<T> parameter
        var scalarProcessor = _genericSpace5D.ScalarProcessor;
        var genericBlade = _genericSpace5D.Encode.VGa.Trivector(
            scalarProcessor.ScalarFromValue(xyz)
        );

        // Assert
        var float64Trivector = float64Blade.InternalKVector;
        var genericTrivector = genericBlade.InternalKVector;

        Assert.That(genericTrivector.Count, Is.EqualTo(float64Trivector.Count));
        Assert.That(genericTrivector.Grade, Is.EqualTo(3));
        Assert.That(float64Trivector.Grade, Is.EqualTo(3));

        foreach (var pair in float64Trivector.IdScalarPairs)
        {
            Assert.That(genericTrivector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Trivector term at {pair.Key} should match");
        }
    }

    #endregion

    #region Edge Cases

    [Test]
    public void VectorAsXGaVector_ZeroVector_ShouldProduceIdenticalResults()
    {
        // Arrange
        double x = 0.0, y = 0.0, z = 0.0;

        // Act
        var float64Vector = _float64Space5D.Encode.VGa.VectorAsXGaVector(x, y, z);
        var genericVector = _genericSpace5D.Encode.VGa.VectorAsXGaVector(x, y, z);

        // Assert - Zero vector should still be encoded with proper structure
        for (int i = 0; i < 5; i++)
        {
            Assert.That(genericVector.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Vector.GetTermScalarByIndex(i)).Within(1e-14));
        }
    }

    [Test]
    public void Vector_NegativeCoordinates_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = -1.5, y = -2.5, z = -3.5;

        // Act
        var float64Blade = _float64Space5D.Encode.VGa.Vector(x, y, z);
        var genericBlade = _genericSpace5D.Encode.VGa.Vector(x, y, z);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        for (int i = 0; i < 5; i++)
        {
            Assert.That(genericVector.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Vector.GetTermScalarByIndex(i)).Within(1e-14),
                $"Negative coordinate at index {i} should match");
        }
    }

    #endregion
}
