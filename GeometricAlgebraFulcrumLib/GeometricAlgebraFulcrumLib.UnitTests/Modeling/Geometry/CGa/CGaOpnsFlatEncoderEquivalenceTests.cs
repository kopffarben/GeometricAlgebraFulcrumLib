using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Unit tests for CGA OpnsFlat Encoder equivalence - Milestone 1.2 continuation.
/// Tests ensure Float64 and Generic&lt;double&gt; encoders produce identical CGA blades.
///
/// OPNS Flat encoding represents flat geometric objects (points, lines, planes, hyperplanes)
/// using Outer Product Null Space representation in conformal geometric algebra.
///
/// Note: Generic encoders have MORE methods (Hybrid API) while Float64 is minimalistic.
/// Tests verify functional equivalence, not API parity.
/// </summary>
[TestFixture]
public class CGaOpnsFlatEncoderEquivalenceTests
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
        var float64Blade = _float64Space4D.Encode.OpnsFlat.Point(x, y);
        var genericBlade = _genericSpace4D.Encode.OpnsFlat.Point(x, y);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"OpnsFlat point term at {pair.Key} should match");
        }
    }

    [Test]
    public void Point_3D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 1.5, y = 2.5, z = 3.5;

        // Act
        var float64Blade = _float64Space5D.Encode.OpnsFlat.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.OpnsFlat.Point(x, y, z);

        // Assert
        var float64Vector = float64Blade.InternalVector;
        var genericVector = genericBlade.InternalVector;

        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Assert.That(genericVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"OpnsFlat point 3D term at {pair.Key} should match");
        }
    }

    #endregion

    #region Line Tests

    [Test]
    public void Line_2D_FromPoints_ShouldProduceIdenticalBlades()
    {
        // Arrange - Line defined by two points
        double x1 = 1.0, y1 = 1.0;
        double x2 = 3.0, y2 = 2.0;

        var scalarProcessor = ScalarProcessorOfFloat64.Instance;

        // Act
        var float64Blade = _float64Space4D.Encode.OpnsFlat.LineFromPoints(
            LinFloat64Vector2D.Create(x1, y1),
            LinFloat64Vector2D.Create(x2, y2)
        );
        var genericBlade = _genericSpace4D.Encode.OpnsFlat.LineFromPoints(
            LinVector2D<double>.Create(
                scalarProcessor.ScalarFromNumber(x1),
                scalarProcessor.ScalarFromNumber(y1)
            ),
            LinVector2D<double>.Create(
                scalarProcessor.ScalarFromNumber(x2),
                scalarProcessor.ScalarFromNumber(y2)
            )
        );

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
    public void Plane_3D_FromPoints_ShouldProduceIdenticalBlades()
    {
        // Arrange - Plane defined by three points
        double x1 = 1.0, y1 = 0.0, z1 = 0.0;
        double x2 = 0.0, y2 = 1.0, z2 = 0.0;
        double x3 = 0.0, y3 = 0.0, z3 = 1.0;

        var scalarProcessor = ScalarProcessorOfFloat64.Instance;

        // Act
        var float64Blade = _float64Space5D.Encode.OpnsFlat.PlaneFromPoints(
            LinFloat64Vector3D.Create(x1, y1, z1),
            LinFloat64Vector3D.Create(x2, y2, z2),
            LinFloat64Vector3D.Create(x3, y3, z3)
        );
        var genericBlade = _genericSpace5D.Encode.OpnsFlat.PlaneFromPoints(
            LinVector3D<double>.Create(
                scalarProcessor.ScalarFromNumber(x1),
                scalarProcessor.ScalarFromNumber(y1),
                scalarProcessor.ScalarFromNumber(z1)
            ),
            LinVector3D<double>.Create(
                scalarProcessor.ScalarFromNumber(x2),
                scalarProcessor.ScalarFromNumber(y2),
                scalarProcessor.ScalarFromNumber(z2)
            ),
            LinVector3D<double>.Create(
                scalarProcessor.ScalarFromNumber(x3),
                scalarProcessor.ScalarFromNumber(y3),
                scalarProcessor.ScalarFromNumber(z3)
            )
        );

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
        var float64Blade = _float64Space5D.Encode.OpnsFlat.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.OpnsFlat.Point(x, y, z);

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
    public void Line_ThroughOrigin_ShouldProduceIdenticalBlades()
    {
        // Arrange - Line through origin defined by two points
        double x1 = 0.0, y1 = 0.0;
        double x2 = 1.0, y2 = 1.0;

        var scalarProcessor = ScalarProcessorOfFloat64.Instance;

        // Act
        var float64Blade = _float64Space4D.Encode.OpnsFlat.LineFromPoints(
            LinFloat64Vector2D.Create(x1, y1),
            LinFloat64Vector2D.Create(x2, y2)
        );
        var genericBlade = _genericSpace4D.Encode.OpnsFlat.LineFromPoints(
            LinVector2D<double>.Create(
                scalarProcessor.ScalarFromNumber(x1),
                scalarProcessor.ScalarFromNumber(y1)
            ),
            LinVector2D<double>.Create(
                scalarProcessor.ScalarFromNumber(x2),
                scalarProcessor.ScalarFromNumber(y2)
            )
        );

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Line through origin term at {pair.Key} should match");
        }
    }

    #endregion
}
