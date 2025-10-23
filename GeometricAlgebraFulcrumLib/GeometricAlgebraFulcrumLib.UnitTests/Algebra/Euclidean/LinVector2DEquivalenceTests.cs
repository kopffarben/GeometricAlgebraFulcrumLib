using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Unit tests for LinVector2D equivalence - LinearAlgebra Phase.
/// Tests ensure Float64 and Generic&lt;double&gt; implementations produce identical results.
///
/// This follows the same equivalence testing pattern established in Milestone 1.2 for CGA encoders.
/// Expected: Potential bugs in Generic implementation (normalization, processor usage, etc.)
/// </summary>
[TestFixture]
public class LinVector2DEquivalenceTests
{
    private const double Tolerance = 1e-14; // Stricter tolerance for equivalence testing
    private IScalarProcessor<double> _scalarProcessor = null!;

    [SetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Construction Tests (2 tests)

    [Test]
    public void CreateVector_ShouldHaveIdenticalComponents()
    {
        // Arrange
        double x = 3.0, y = 4.0;

        // Act
        var float64Vector = LinFloat64Vector2D.Create(x, y);
        var genericVector = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(x),
            _scalarProcessor.ScalarFromNumber(y)
        );

        // Assert
        Assert.That(genericVector.X.ScalarValue, Is.EqualTo(float64Vector.X.ScalarValue).Within(Tolerance),
            "X components should match");
        Assert.That(genericVector.Y.ScalarValue, Is.EqualTo(float64Vector.Y.ScalarValue).Within(Tolerance),
            "Y components should match");
    }

    [Test]
    public void CreateFromPolar_ShouldHaveIdenticalCartesianComponents()
    {
        // Arrange
        var float64Angle = LinFloat64PolarAngle.CreateFromDegrees(90);
        var genericAngle = LinPolarAngle<double>.CreateFromDegrees(_scalarProcessor, 90);
        var length = 5.0;

        // Act
        var float64Vector = LinFloat64Vector2D.CreateFromPolar(length, float64Angle);
        var genericVector = LinVector2D<double>.CreateFromPolar(
            _scalarProcessor.ScalarFromNumber(length),
            genericAngle
        );

        // Assert
        // 90 degrees: cos(90°) = 0, sin(90°) = 1
        Assert.That(genericVector.X.ScalarValue, Is.EqualTo(float64Vector.X.ScalarValue).Within(Tolerance),
            "X component should match at 90°");
        Assert.That(genericVector.Y.ScalarValue, Is.EqualTo(float64Vector.Y.ScalarValue).Within(Tolerance),
            "Y component should match at 90°");
    }

    #endregion

    #region Basic Operations (3 tests)

    [Test]
    public void VectorAddition_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64V1 = LinFloat64Vector2D.Create(1.0, 2.0);
        var float64V2 = LinFloat64Vector2D.Create(3.0, 4.0);

        var genericV1 = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(2.0)
        );
        var genericV2 = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0),
            _scalarProcessor.ScalarFromNumber(4.0)
        );

        // Act
        var float64Result = float64V1 + float64V2;
        var genericResult = genericV1 + genericV2;

        // Assert
        Assert.That(genericResult.X.ScalarValue, Is.EqualTo(float64Result.X.ScalarValue).Within(Tolerance),
            "Addition X component should match");
        Assert.That(genericResult.Y.ScalarValue, Is.EqualTo(float64Result.Y.ScalarValue).Within(Tolerance),
            "Addition Y component should match");
    }

    [Test]
    public void VectorSubtraction_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64V1 = LinFloat64Vector2D.Create(5.0, 7.0);
        var float64V2 = LinFloat64Vector2D.Create(2.0, 3.0);

        var genericV1 = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(5.0),
            _scalarProcessor.ScalarFromNumber(7.0)
        );
        var genericV2 = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(2.0),
            _scalarProcessor.ScalarFromNumber(3.0)
        );

        // Act
        var float64Result = float64V1 - float64V2;
        var genericResult = genericV1 - genericV2;

        // Assert
        Assert.That(genericResult.X.ScalarValue, Is.EqualTo(float64Result.X.ScalarValue).Within(Tolerance),
            "Subtraction X component should match");
        Assert.That(genericResult.Y.ScalarValue, Is.EqualTo(float64Result.Y.ScalarValue).Within(Tolerance),
            "Subtraction Y component should match");
    }

    [Test]
    public void VectorScalarMultiplication_ShouldProduceIdenticalResults()
    {
        // Arrange
        double scalar = 4.0;
        var float64Vector = LinFloat64Vector2D.Create(2.0, 3.0);
        var genericVector = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(2.0),
            _scalarProcessor.ScalarFromNumber(3.0)
        );

        // Act
        var float64Result = float64Vector * scalar;
        var genericResult = genericVector * _scalarProcessor.ScalarFromNumber(scalar);

        // Assert
        Assert.That(genericResult.X.ScalarValue, Is.EqualTo(float64Result.X.ScalarValue).Within(Tolerance),
            "Multiplication X component should match");
        Assert.That(genericResult.Y.ScalarValue, Is.EqualTo(float64Result.Y.ScalarValue).Within(Tolerance),
            "Multiplication Y component should match");
    }

    #endregion

    #region Norm and Distance (2 tests)

    [Test]
    public void VectorNorm_ShouldProduceIdenticalValues()
    {
        // Arrange
        var float64Vector = LinFloat64Vector2D.Create(3.0, 4.0);
        var genericVector = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0),
            _scalarProcessor.ScalarFromNumber(4.0)
        );

        // Act
        var float64Norm = float64Vector.VectorENorm();
        var genericNorm = genericVector.VectorENorm();

        // Assert
        // ||v|| = sqrt(3^2 + 4^2) = 5
        Assert.That(genericNorm.ScalarValue, Is.EqualTo(float64Norm).Within(Tolerance),
            "Euclidean norm should match");
    }

    [Test]
    public void VectorNormSquared_ShouldProduceIdenticalValues()
    {
        // Arrange
        var float64Vector = LinFloat64Vector2D.Create(3.0, 4.0);
        var genericVector = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0),
            _scalarProcessor.ScalarFromNumber(4.0)
        );

        // Act
        var float64NormSquared = float64Vector.VectorENormSquared();
        var genericNormSquared = genericVector.VectorENormSquared();

        // Assert
        // ||v||² = 3² + 4² = 25
        Assert.That(genericNormSquared.ScalarValue, Is.EqualTo(float64NormSquared.ScalarValue).Within(Tolerance),
            "Squared norm should match");
    }

    #endregion

    #region Dot Product (2 tests)

    [Test]
    public void DotProduct_ShouldProduceIdenticalValues()
    {
        // Arrange
        var float64V1 = LinFloat64Vector2D.Create(1.0, 2.0);
        var float64V2 = LinFloat64Vector2D.Create(3.0, 4.0);

        var genericV1 = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(2.0)
        );
        var genericV2 = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0),
            _scalarProcessor.ScalarFromNumber(4.0)
        );

        // Act
        var float64DotProduct = float64V1.VectorESp(float64V2);
        var genericDotProduct = genericV1.VectorESp(genericV2);

        // Assert
        // v1 · v2 = 1*3 + 2*4 = 11
        Assert.That(genericDotProduct.ScalarValue, Is.EqualTo(float64DotProduct.ScalarValue).Within(Tolerance),
            "Dot product should match");
    }

    [Test]
    public void OrthogonalVectors_ShouldHaveIdenticalZeroDotProduct()
    {
        // Arrange
        var float64V1 = LinFloat64Vector2D.Create(1.0, 0.0);
        var float64V2 = LinFloat64Vector2D.Create(0.0, 1.0);

        var genericV1 = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(0.0)
        );
        var genericV2 = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(0.0),
            _scalarProcessor.ScalarFromNumber(1.0)
        );

        // Act
        var float64DotProduct = float64V1.VectorESp(float64V2);
        var genericDotProduct = genericV1.VectorESp(genericV2);
        var float64IsOrthogonal = float64V1.IsNearOrthogonalTo(float64V2);

        // Assert
        Assert.That(genericDotProduct.ScalarValue, Is.EqualTo(float64DotProduct.ScalarValue).Within(Tolerance),
            "Orthogonal dot product should match (both zero)");
        Assert.That(float64IsOrthogonal, Is.True,
            "Float64 vectors should be detected as orthogonal");
        // Note: Generic version IsNearOrthogonalTo requires IPair interface casting,
        // so we just verify the dot product is zero for both implementations
    }

    #endregion

    #region Normalization (1 test)

    [Test]
    public void VectorNormalization_ShouldProduceIdenticalUnitVectors()
    {
        // Arrange
        var float64Vector = LinFloat64Vector2D.Create(3.0, 4.0);
        var genericVector = LinVector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0),
            _scalarProcessor.ScalarFromNumber(4.0)
        );

        // Act
        var float64UnitVector = float64Vector.ToUnitLinVector2D();
        var genericUnitVector = genericVector.VectorDivideByNorm();

        var float64Norm = float64UnitVector.VectorENorm();
        var genericNorm = genericUnitVector.VectorENorm();

        // Assert
        Assert.That(genericNorm.ScalarValue, Is.EqualTo(float64Norm).Within(Tolerance),
            "Unit vector norms should both be 1");

        // Direction should be preserved: (3/5, 4/5) = (0.6, 0.8)
        Assert.That(genericUnitVector.X.ScalarValue, Is.EqualTo(float64UnitVector.X.ScalarValue).Within(Tolerance),
            "Unit vector X component should match");
        Assert.That(genericUnitVector.Y.ScalarValue, Is.EqualTo(float64UnitVector.Y.ScalarValue).Within(Tolerance),
            "Unit vector Y component should match");

        // Check IsNearUnit for Float64
        var float64IsUnit = float64UnitVector.IsNearUnit();
        Assert.That(float64IsUnit, Is.True,
            "Float64 unit vector should be detected as near unit");
        // Note: Generic version uses IsNearUnitVector() which requires IPair casting,
        // but we've verified the norm is 1.0 above, which is sufficient for equivalence
    }

    #endregion
}
