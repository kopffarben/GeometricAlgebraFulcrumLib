using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Unit tests for CGA IpnsRound Encoder equivalence - Milestone 1.2 continuation.
/// Tests ensure Float64 and Generic&lt;double&gt; encoders produce identical CGA blades.
///
/// IPNS (Inner Product Null Space) encoding represents geometric objects (points, spheres, circles)
/// as null vectors or blades in conformal geometric algebra.
///
/// Note: Generic encoders have MORE methods (Hybrid API) while Float64 is minimalistic.
/// Tests verify functional equivalence, not API parity.
/// </summary>
[TestFixture]
public class CGaIpnsRoundEncoderEquivalenceTests
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

    #region Point Tests (2D)

    [Test]
    public void Point_2D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 1.5, y = 2.5;

        // Act
        var float64Blade = _float64Space4D.Encode.IpnsRound.Point(x, y);
        var genericBlade = _genericSpace4D.Encode.IpnsRound.Point(x, y);

        // Assert - Compare the internal vectors
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count),
            "Point vectors should have same number of terms");

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Point term at {pair.Key} should match");
        }
    }

    [Test]
    public void Point_2D_WithNegativeCoordinates_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = -1.5, y = -2.5;

        // Act
        var float64Blade = _float64Space4D.Encode.IpnsRound.Point(x, y);
        var genericBlade = _genericSpace4D.Encode.IpnsRound.Point(x, y);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Point with negative coordinates term at {pair.Key} should match");
        }
    }

    #endregion

    #region Point Tests (3D)

    [Test]
    public void Point_3D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 1.5, y = 2.5, z = 3.5;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsRound.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.IpnsRound.Point(x, y, z);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"3D Point term at {pair.Key} should match");
        }
    }

    [Test]
    public void Point_3D_LargeCoordinates_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 100.5, y = 200.5, z = 300.5;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsRound.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.IpnsRound.Point(x, y, z);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Point with large coordinates term at {pair.Key} should match");
        }
    }

    #endregion

    #region Sphere Tests

    [Test]
    public void Sphere_3D_FromCenterAndRadius_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double cx = 1.0, cy = 2.0, cz = 3.0, radius = 2.5;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsRound.Sphere(cx, cy, cz, radius);

        // Generic version needs IScalar<T> parameters
        var scalarProcessor = _genericSpace5D.ScalarProcessor;
        var genericBlade = _genericSpace5D.Encode.IpnsRound.Sphere(
            scalarProcessor.ScalarFromValue(cx),
            scalarProcessor.ScalarFromValue(cy),
            scalarProcessor.ScalarFromValue(cz),
            scalarProcessor.ScalarFromValue(radius)
        );

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Sphere term at {pair.Key} should match");
        }
    }

    [Test]
    public void RealSphere_3D_FromCenterAndRadius_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double cx = 1.0, cy = 2.0, cz = 3.0, radius = 2.5;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsRound.RealSphere(cx, cy, cz, radius);

        // Generic version
        var scalarProcessor = _genericSpace5D.ScalarProcessor;
        var genericBlade = _genericSpace5D.Encode.IpnsRound.RealSphere(
            scalarProcessor.ScalarFromValue(cx),
            scalarProcessor.ScalarFromValue(cy),
            scalarProcessor.ScalarFromValue(cz),
            scalarProcessor.ScalarFromValue(radius)
        );

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"RealSphere term at {pair.Key} should match");
        }
    }

    #endregion

    #region ImaginarySphere Tests

    [Test]
    public void ImaginarySphere_3D_FromCenterAndRadius_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double cx = 1.0, cy = 2.0, cz = 3.0, radius = 2.5;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsRound.ImaginarySphere(cx, cy, cz, radius);

        // Generic version
        var scalarProcessor = _genericSpace5D.ScalarProcessor;
        var genericBlade = _genericSpace5D.Encode.IpnsRound.ImaginarySphere(
            scalarProcessor.ScalarFromValue(cx),
            scalarProcessor.ScalarFromValue(cy),
            scalarProcessor.ScalarFromValue(cz),
            scalarProcessor.ScalarFromValue(radius)
        );

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"ImaginarySphere term at {pair.Key} should match");
        }
    }

    #endregion

    #region Edge Cases

    [Test]
    public void Point_AtOrigin_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 0.0, y = 0.0, z = 0.0;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsRound.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.IpnsRound.Point(x, y, z);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Origin point term at {pair.Key} should match");
        }
    }

    [Test]
    public void Sphere_UnitRadius_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double cx = 0.0, cy = 0.0, cz = 0.0, radius = 1.0;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsRound.Sphere(cx, cy, cz, radius);

        var scalarProcessor = _genericSpace5D.ScalarProcessor;
        var genericBlade = _genericSpace5D.Encode.IpnsRound.Sphere(
            scalarProcessor.ScalarFromValue(cx),
            scalarProcessor.ScalarFromValue(cy),
            scalarProcessor.ScalarFromValue(cz),
            scalarProcessor.ScalarFromValue(radius)
        );

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Unit sphere term at {pair.Key} should match");
        }
    }

    #endregion
}
