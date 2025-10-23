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
/// Unit tests for CGA OpnsRound Encoder equivalence - Milestone 1.2 continuation.
/// Tests ensure Float64 and Generic&lt;double&gt; encoders produce identical CGA blades.
///
/// OPNS Round encoding represents round geometric objects (points, point-pairs, circles, spheres)
/// using Outer Product Null Space representation in conformal geometric algebra.
///
/// Note: OpnsRound is implemented as a wrapper around IpnsRound with CGaUnDual() transformation.
/// Since IpnsRound tests all pass (9/9), OpnsRound should also work correctly.
///
/// API DESIGN:
/// - Points: Defined by coordinates Point(x, y, z)
/// - Circles: Defined by 3 points Circle(p1, p2, p3)
/// - Spheres: Defined by 4 points Sphere(p1, p2, p3, p4)
/// This is the natural OPNS representation - objects defined by points on them.
/// </summary>
[TestFixture]
public class CGaOpnsRoundEncoderEquivalenceTests
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
        var float64Blade = _float64Space4D.Encode.OpnsRound.Point(x, y);
        var genericBlade = _genericSpace4D.Encode.OpnsRound.Point(x, y);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        Assert.That(genericKVector.Count, Is.EqualTo(float64KVector.Count));
        Assert.That(genericKVector.Grade, Is.EqualTo(float64KVector.Grade));

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"OpnsRound point term at {pair.Key} should match");
        }
    }

    [Test]
    public void Point_3D_FromDoubles_ShouldProduceIdenticalBlades()
    {
        // Arrange
        double x = 1.5, y = 2.5, z = 3.5;

        // Act
        var float64Blade = _float64Space5D.Encode.OpnsRound.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.OpnsRound.Point(x, y, z);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"OpnsRound point 3D term at {pair.Key} should match");
        }
    }

    #endregion

    #region Circle Tests

    [Test]
    public void Circle_2D_FromThreePoints_ShouldProduceIdenticalBlades()
    {
        // Arrange - Circle defined by three points
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;

        var p1Float = LinFloat64Vector2D.Create(1.0, 0.0);
        var p2Float = LinFloat64Vector2D.Create(0.0, 1.0);
        var p3Float = LinFloat64Vector2D.Create(-1.0, 0.0);

        var p1Generic = LinVector2D<double>.Create(
            scalarProcessor.ScalarFromNumber(1.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p2Generic = LinVector2D<double>.Create(
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(1.0)
        );
        var p3Generic = LinVector2D<double>.Create(
            scalarProcessor.ScalarFromNumber(-1.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );

        // Act
        var float64Blade = _float64Space4D.Encode.OpnsRound.Circle(p1Float, p2Float, p3Float);
        var genericBlade = _genericSpace4D.Encode.OpnsRound.Circle(p1Generic, p2Generic, p3Generic);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        Assert.That(genericKVector.Count, Is.EqualTo(float64KVector.Count));
        Assert.That(genericKVector.Grade, Is.EqualTo(float64KVector.Grade));

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Circle term at {pair.Key} should match");
        }
    }

    [Test]
    public void Circle_3D_FromThreePoints_ShouldProduceIdenticalBlades()
    {
        // Arrange - Circle defined by three points in 3D
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;

        var p1Float = LinFloat64Vector3D.Create(1.0, 0.0, 0.0);
        var p2Float = LinFloat64Vector3D.Create(0.0, 1.0, 0.0);
        var p3Float = LinFloat64Vector3D.Create(-1.0, 0.0, 0.0);

        var p1Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(1.0),
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p2Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(1.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p3Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(-1.0),
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );

        // Act
        var float64Blade = _float64Space5D.Encode.OpnsRound.Circle(p1Float, p2Float, p3Float);
        var genericBlade = _genericSpace5D.Encode.OpnsRound.Circle(p1Generic, p2Generic, p3Generic);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        Assert.That(genericKVector.Grade, Is.EqualTo(float64KVector.Grade));

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Circle 3D term at {pair.Key} should match");
        }
    }

    #endregion

    #region Sphere Tests

    [Test]
    public void Sphere_3D_FromFourPoints_ShouldProduceIdenticalBlades()
    {
        // Arrange - Sphere defined by four points
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;

        var p1Float = LinFloat64Vector3D.Create(1.0, 0.0, 0.0);
        var p2Float = LinFloat64Vector3D.Create(0.0, 1.0, 0.0);
        var p3Float = LinFloat64Vector3D.Create(0.0, 0.0, 1.0);
        var p4Float = LinFloat64Vector3D.Create(-1.0, 0.0, 0.0);

        var p1Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(1.0),
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p2Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(1.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p3Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(1.0)
        );
        var p4Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(-1.0),
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );

        // Act
        var float64Blade = _float64Space5D.Encode.OpnsRound.Sphere(p1Float, p2Float, p3Float, p4Float);
        var genericBlade = _genericSpace5D.Encode.OpnsRound.Sphere(p1Generic, p2Generic, p3Generic, p4Generic);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        Assert.That(genericKVector.Grade, Is.EqualTo(float64KVector.Grade));

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Sphere term at {pair.Key} should match");
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
        var float64Blade = _float64Space5D.Encode.OpnsRound.Point(x, y, z);
        var genericBlade = _genericSpace5D.Encode.OpnsRound.Point(x, y, z);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Point at origin term at {pair.Key} should match");
        }
    }

    [Test]
    public void Circle_ThroughOrigin_ShouldProduceIdenticalBlades()
    {
        // Arrange - Circle passing through origin defined by three points
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;

        var p1Float = LinFloat64Vector2D.Create(0.0, 0.0);  // Origin
        var p2Float = LinFloat64Vector2D.Create(1.0, 0.0);
        var p3Float = LinFloat64Vector2D.Create(0.0, 1.0);

        var p1Generic = LinVector2D<double>.Create(
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p2Generic = LinVector2D<double>.Create(
            scalarProcessor.ScalarFromNumber(1.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p3Generic = LinVector2D<double>.Create(
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(1.0)
        );

        // Act
        var float64Blade = _float64Space4D.Encode.OpnsRound.Circle(p1Float, p2Float, p3Float);
        var genericBlade = _genericSpace4D.Encode.OpnsRound.Circle(p1Generic, p2Generic, p3Generic);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Circle through origin term at {pair.Key} should match");
        }
    }

    [Test]
    public void Sphere_ThroughOrigin_ShouldProduceIdenticalBlades()
    {
        // Arrange - Sphere passing through origin defined by four points
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;

        var p1Float = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);  // Origin
        var p2Float = LinFloat64Vector3D.Create(1.0, 0.0, 0.0);
        var p3Float = LinFloat64Vector3D.Create(0.0, 1.0, 0.0);
        var p4Float = LinFloat64Vector3D.Create(0.0, 0.0, 1.0);

        var p1Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p2Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(1.0),
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p3Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(1.0),
            scalarProcessor.ScalarFromNumber(0.0)
        );
        var p4Generic = LinVector3D<double>.Create(
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(0.0),
            scalarProcessor.ScalarFromNumber(1.0)
        );

        // Act
        var float64Blade = _float64Space5D.Encode.OpnsRound.Sphere(p1Float, p2Float, p3Float, p4Float);
        var genericBlade = _genericSpace5D.Encode.OpnsRound.Sphere(p1Generic, p2Generic, p3Generic, p4Generic);

        // Assert
        var float64KVector = float64Blade.InternalKVector;
        var genericKVector = genericBlade.InternalKVector;

        foreach (var pair in float64KVector.IdScalarPairs)
        {
            Assert.That(genericKVector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Sphere through origin term at {pair.Key} should match");
        }
    }

    #endregion
}
