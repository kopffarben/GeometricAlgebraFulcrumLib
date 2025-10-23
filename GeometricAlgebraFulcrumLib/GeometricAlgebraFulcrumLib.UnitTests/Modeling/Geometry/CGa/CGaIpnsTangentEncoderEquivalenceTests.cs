using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Unit tests for CGA IpnsTangent Encoder equivalence - Milestone 1.2 continuation.
/// Tests ensure Float64 and Generic&lt;double&gt; encoders produce identical CGA blades.
///
/// IPNS Tangent encoding represents tangent spaces (points, lines, planes) in conformal geometric algebra.
/// Tangent elements represent infinitesimally small surfaces tangent to geometries.
///
/// Note: IpnsTangent encoder is expected to have the same HyperPlane bug as IpnsFlat had.
/// This test suite will help identify and fix those issues.
/// </summary>
[TestFixture]
public class CGaIpnsTangentEncoderEquivalenceTests
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
        var float64Blade = _float64Space4D.Encode.IpnsTangent.Point(x, y);
        var genericBlade = _genericSpace4D.Encode.IpnsTangent.Point(x, y);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        Assert.That(genericKVector.Count, Is.EqualTo(float64KVector.Count));

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Tangent point term at {pair.Key} should match");
        }
    }

    [Test]
    public void Point_3D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 1.5, y = 2.5, z = 3.5;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsTangent.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.IpnsTangent.Point(x, y, z);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Tangent point 3D term at {pair.Key} should match");
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
        var float64Blade = _float64Space4D.Encode.IpnsTangent.Line(distance, normalX, normalY);
        var genericBlade = _genericSpace4D.Encode.IpnsTangent.Line(distance, normalX, normalY);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        Assert.That(genericKVector.Count, Is.EqualTo(float64KVector.Count));
        Assert.That(genericKVector.Grade, Is.EqualTo(float64KVector.Grade));

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Tangent line term at {pair.Key} should match");
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
        var float64Blade = _float64Space5D.Encode.IpnsTangent.Plane(distance, nx, ny, nz);
        var genericBlade = _genericSpace5D.Encode.IpnsTangent.Plane(distance, nx, ny, nz);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        Assert.That(genericKVector.Count, Is.EqualTo(float64KVector.Count));
        Assert.That(genericKVector.Grade, Is.EqualTo(float64KVector.Grade));

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Tangent plane term at {pair.Key} should match");
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
        var float64Blade = _float64Space5D.Encode.IpnsTangent.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.IpnsTangent.Point(x, y, z);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Tangent point at origin term at {pair.Key} should match");
        }
    }

    [Test]
    public void Plane_ThroughOrigin_ShouldProduceIdenticalBlades()
    {
        // Arrange - Plane through origin (distance = 0)
        double distance = 0.0;
        double nx = 1.0, ny = 1.0, nz = 1.0;

        // Act
        var float64Blade = _float64Space5D.Encode.IpnsTangent.Plane(distance, nx, ny, nz);
        var genericBlade = _genericSpace5D.Encode.IpnsTangent.Plane(distance, nx, ny, nz);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Tangent plane through origin term at {pair.Key} should match");
        }
    }

    #endregion
}
