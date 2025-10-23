using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Unit tests for CGA IpnsFlat Encoder equivalence - Milestone 1.2 continuation.
/// Tests ensure Float64 and Generic&lt;double&gt; encoders produce identical CGA blades.
///
/// IPNS Flat encoding represents flat geometric objects (points, lines, planes, hyperplanes)
/// as blades in conformal geometric algebra.
///
/// Note: Generic encoders have MORE methods (Hybrid API) while Float64 is minimalistic.
/// Tests verify functional equivalence, not API parity.
///
/// BUG FIXED (2025-10-23): Line/Plane encoding was broken in BOTH implementations!
///
/// ROOT CAUSE:
/// - Original code used ToXGaFloat64Vector() with global XGaFloat64Processor.Euclidean
/// - This created vectors with indices {0,1} or {0,1,2}
/// - But HyperPlane() validated with IsValidVGaElement() which expects VGa indices {2,3,4}
/// - Generic implementation also had DivideByNorm() commented out, causing normalization mismatch
///
/// FIX IMPLEMENTED:
/// 1. Use GeometricSpace.EuclideanProcessor to create vectors (not global processor)
/// 2. Fixed Debug.Assert to check egaNormalVector.Processor.IsEuclidean (not IsValidVGaElement)
/// 3. EncodeVGaBlade() automatically shifts Euclidean indices {0,1,2} → VGa indices {2,3,4}
/// 4. Uncommented DivideByNorm() in Generic HyperPlane() for equivalence with Float64
///
/// RESULT: All 6 tests now pass (100% pass rate)
/// </summary>
[TestFixture]
public class CGaIpnsFlatEncoderEquivalenceTests
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

    #region Point Tests

    [Test]
    public void Point_2D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 1.5, y = 2.5;

        // Act
        var float64Blade = _float64Space4D.Encode.IpnsFlat.Point(x, y);
        var genericBlade = _genericSpace4D.Encode.IpnsFlat.Point(x, y);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Flat point term at {pair.Key} should match");
        }
    }

    [Test]
    public void Point_3D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 1.5, y = 2.5, z = 3.5;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsFlat.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.IpnsFlat.Point(x, y, z);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Flat point 3D term at {pair.Key} should match");
        }
    }

    #endregion

    #region Line Tests

    [Test]
    public void Line_2D_FromDistanceAndNormal_ShouldProduceIdenticalBlades()
    {
        // Arrange - Line defined by distance from origin and normal vector
        double distance = 2.0;
        double normalX = 1.0, normalY = 0.0;  // Normal in X direction

        // Act
        var float64Blade = _float64Space4D.Encode.IpnsFlat.Line(distance, normalX, normalY);
        var genericBlade = _genericSpace4D.Encode.IpnsFlat.Line(distance, normalX, normalY);

        // Assert - Use the general KVector accessor
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        Assert.That(genericKVector.Count, Is.EqualTo(float64KVector.Count));
        Assert.That(genericKVector.Grade, Is.EqualTo(float64KVector.Grade));

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Line term at {pair.Key} should match");
        }
    }

    #endregion

    #region Plane Tests

    [Test]
    public void Plane_3D_FromDistanceAndNormal_ShouldProduceIdenticalBlades()
    {
        // Arrange - Plane with distance and normal
        double distance = 2.0;  // Distance from origin
        double nx = 0.0, ny = 0.0, nz = 1.0;  // Normal pointing in Z direction

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsFlat.Plane(distance, nx, ny, nz);
        var genericBlade = _genericSpace5D.Encode.IpnsFlat.Plane(distance, nx, ny, nz);

        // Assert - Use the general KVector accessor
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        Assert.That(genericKVector.Count, Is.EqualTo(float64KVector.Count));
        Assert.That(genericKVector.Grade, Is.EqualTo(float64KVector.Grade));

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Plane term at {pair.Key} should match");
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
        var float64Blade = _float64Space5D.Encode.IpnsFlat.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.IpnsFlat.Point(x, y, z);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Point at origin term at {pair.Key} should match");
        }
    }

    [Test]
    public void Plane_ThroughOrigin_ShouldProduceIdenticalBlades()
    {
        // Arrange - Plane through origin (distance = 0)
        double distance = 0.0;
        double nx = 1.0, ny = 1.0, nz = 1.0;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsFlat.Plane(distance, nx, ny, nz);
        var genericBlade = _genericSpace5D.Encode.IpnsFlat.Plane(distance, nx, ny, nz);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Plane through origin term at {pair.Key} should match");
        }
    }

    #endregion
}
