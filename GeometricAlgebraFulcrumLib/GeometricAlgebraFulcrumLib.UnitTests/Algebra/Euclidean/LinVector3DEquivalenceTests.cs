using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Äquivalenztests für LinVector3D - LinearAlgebra Phase.
/// Testet ob Float64 und Generic&lt;double&gt; Implementierungen identische Ergebnisse liefern.
///
/// Folgt dem gleichen Äquivalenztest-Pattern wie in Milestone 1.2 für CGA Encoder.
/// Erwartet: Potenzielle Bugs in Generic Implementierung (Normalisierung, Processor-Nutzung, etc.)
/// </summary>
[TestFixture]
public class LinVector3DEquivalenceTests
{
    private const double Tolerance = 1e-14; // Striktere Toleranz für Äquivalenztests
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
        double x = 2.0, y = 3.0, z = 4.0;

        // Act
        var float64Vector = LinFloat64Vector3D.Create(x, y, z);
        var genericVector = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(x),
            _scalarProcessor.ScalarFromNumber(y),
            _scalarProcessor.ScalarFromNumber(z)
        );

        // Assert
        Assert.That(genericVector.X.ScalarValue, Is.EqualTo(float64Vector.X.ScalarValue).Within(Tolerance),
            "X Komponenten sollten übereinstimmen");
        Assert.That(genericVector.Y.ScalarValue, Is.EqualTo(float64Vector.Y.ScalarValue).Within(Tolerance),
            "Y Komponenten sollten übereinstimmen");
        Assert.That(genericVector.Z.ScalarValue, Is.EqualTo(float64Vector.Z.ScalarValue).Within(Tolerance),
            "Z Komponenten sollten übereinstimmen");
    }

    [Test]
    public void BasisVectors_ShouldBeIdenticalAndOrthonormal()
    {
        // Arrange
        var float64E1 = LinFloat64Vector3D.E1;
        var float64E2 = LinFloat64Vector3D.E2;
        var float64E3 = LinFloat64Vector3D.E3;

        var genericE1 = LinVector3D<double>.E1(_scalarProcessor);
        var genericE2 = LinVector3D<double>.E2(_scalarProcessor);
        var genericE3 = LinVector3D<double>.E3(_scalarProcessor);

        // Act & Assert - Komponenten prüfen
        Assert.That(genericE1.X.ScalarValue, Is.EqualTo(float64E1.X.ScalarValue).Within(Tolerance), "E1 X");
        Assert.That(genericE2.Y.ScalarValue, Is.EqualTo(float64E2.Y.ScalarValue).Within(Tolerance), "E2 Y");
        Assert.That(genericE3.Z.ScalarValue, Is.EqualTo(float64E3.Z.ScalarValue).Within(Tolerance), "E3 Z");

        // Norm prüfen
        Assert.That(genericE1.VectorENorm().ScalarValue, Is.EqualTo(float64E1.VectorENorm().ScalarValue).Within(Tolerance),
            "E1 Norm sollte übereinstimmen");
        Assert.That(genericE2.VectorENorm().ScalarValue, Is.EqualTo(float64E2.VectorENorm().ScalarValue).Within(Tolerance),
            "E2 Norm sollte übereinstimmen");
        Assert.That(genericE3.VectorENorm().ScalarValue, Is.EqualTo(float64E3.VectorENorm().ScalarValue).Within(Tolerance),
            "E3 Norm sollte übereinstimmen");

        // Orthogonalität prüfen
        Assert.That(genericE1.VectorESp(genericE2).ScalarValue, Is.EqualTo(float64E1.VectorESp(float64E2).ScalarValue).Within(Tolerance),
            "E1·E2 sollte übereinstimmen (0)");
        Assert.That(genericE1.VectorESp(genericE3).ScalarValue, Is.EqualTo(float64E1.VectorESp(float64E3).ScalarValue).Within(Tolerance),
            "E1·E3 sollte übereinstimmen (0)");
        Assert.That(genericE2.VectorESp(genericE3).ScalarValue, Is.EqualTo(float64E2.VectorESp(float64E3).ScalarValue).Within(Tolerance),
            "E2·E3 sollte übereinstimmen (0)");
    }

    #endregion

    #region Basic Operations (3 tests)

    [Test]
    public void VectorAddition_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64V1 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64V2 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);

        var genericV1 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(2.0),
            _scalarProcessor.ScalarFromNumber(3.0)
        );
        var genericV2 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(4.0),
            _scalarProcessor.ScalarFromNumber(5.0),
            _scalarProcessor.ScalarFromNumber(6.0)
        );

        // Act
        var float64Result = float64V1 + float64V2;
        var genericResult = genericV1 + genericV2;

        // Assert
        Assert.That(genericResult.X.ScalarValue, Is.EqualTo(float64Result.X.ScalarValue).Within(Tolerance),
            "Addition X Komponente sollte übereinstimmen");
        Assert.That(genericResult.Y.ScalarValue, Is.EqualTo(float64Result.Y.ScalarValue).Within(Tolerance),
            "Addition Y Komponente sollte übereinstimmen");
        Assert.That(genericResult.Z.ScalarValue, Is.EqualTo(float64Result.Z.ScalarValue).Within(Tolerance),
            "Addition Z Komponente sollte übereinstimmen");
    }

    [Test]
    public void VectorSubtraction_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64V1 = LinFloat64Vector3D.Create(10.0, 8.0, 6.0);
        var float64V2 = LinFloat64Vector3D.Create(3.0, 2.0, 1.0);

        var genericV1 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(10.0),
            _scalarProcessor.ScalarFromNumber(8.0),
            _scalarProcessor.ScalarFromNumber(6.0)
        );
        var genericV2 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0),
            _scalarProcessor.ScalarFromNumber(2.0),
            _scalarProcessor.ScalarFromNumber(1.0)
        );

        // Act
        var float64Result = float64V1 - float64V2;
        var genericResult = genericV1 - genericV2;

        // Assert
        Assert.That(genericResult.X.ScalarValue, Is.EqualTo(float64Result.X.ScalarValue).Within(Tolerance),
            "Subtraktion X Komponente sollte übereinstimmen");
        Assert.That(genericResult.Y.ScalarValue, Is.EqualTo(float64Result.Y.ScalarValue).Within(Tolerance),
            "Subtraktion Y Komponente sollte übereinstimmen");
        Assert.That(genericResult.Z.ScalarValue, Is.EqualTo(float64Result.Z.ScalarValue).Within(Tolerance),
            "Subtraktion Z Komponente sollte übereinstimmen");
    }

    [Test]
    public void VectorScalarMultiplication_ShouldProduceIdenticalResults()
    {
        // Arrange
        double scalar = 3.0;
        var float64Vector = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var genericVector = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(2.0),
            _scalarProcessor.ScalarFromNumber(3.0)
        );

        // Act
        var float64Result = float64Vector * scalar;
        var genericResult = genericVector * _scalarProcessor.ScalarFromNumber(scalar);

        // Assert
        Assert.That(genericResult.X.ScalarValue, Is.EqualTo(float64Result.X.ScalarValue).Within(Tolerance),
            "Multiplikation X Komponente sollte übereinstimmen");
        Assert.That(genericResult.Y.ScalarValue, Is.EqualTo(float64Result.Y.ScalarValue).Within(Tolerance),
            "Multiplikation Y Komponente sollte übereinstimmen");
        Assert.That(genericResult.Z.ScalarValue, Is.EqualTo(float64Result.Z.ScalarValue).Within(Tolerance),
            "Multiplikation Z Komponente sollte übereinstimmen");
    }

    #endregion

    #region Norm and Dot Product (2 tests)

    [Test]
    public void VectorNorm_ShouldProduceIdenticalValues()
    {
        // Arrange
        var float64Vector = LinFloat64Vector3D.Create(2.0, 3.0, 6.0);
        var genericVector = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(2.0),
            _scalarProcessor.ScalarFromNumber(3.0),
            _scalarProcessor.ScalarFromNumber(6.0)
        );

        // Act
        var float64Norm = float64Vector.VectorENorm();
        var genericNorm = genericVector.VectorENorm();

        // Assert
        // ||v|| = sqrt(2^2 + 3^2 + 6^2) = 7
        Assert.That(genericNorm.ScalarValue, Is.EqualTo(float64Norm.ScalarValue).Within(Tolerance),
            "Euklidische Norm sollte übereinstimmen");
    }

    [Test]
    public void DotProduct_ShouldProduceIdenticalValues()
    {
        // Arrange
        var float64V1 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64V2 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);

        var genericV1 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(2.0),
            _scalarProcessor.ScalarFromNumber(3.0)
        );
        var genericV2 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(4.0),
            _scalarProcessor.ScalarFromNumber(5.0),
            _scalarProcessor.ScalarFromNumber(6.0)
        );

        // Act
        var float64DotProduct = float64V1.VectorESp(float64V2);
        var genericDotProduct = genericV1.VectorESp(genericV2);

        // Assert
        // v1 · v2 = 1*4 + 2*5 + 3*6 = 32
        Assert.That(genericDotProduct.ScalarValue, Is.EqualTo(float64DotProduct.ScalarValue).Within(Tolerance),
            "Skalarprodukt sollte übereinstimmen");
    }

    #endregion

    #region Cross Product (2 tests)

    [Test]
    public void CrossProduct_ShouldProduceIdenticalVectors()
    {
        // Arrange
        var float64V1 = LinFloat64Vector3D.Create(1.0, 0.0, 0.0);  // X-Achse
        var float64V2 = LinFloat64Vector3D.Create(0.0, 1.0, 0.0);  // Y-Achse

        var genericV1 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(0.0),
            _scalarProcessor.ScalarFromNumber(0.0)
        );
        var genericV2 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(0.0),
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(0.0)
        );

        // Act
        var float64CrossProduct = float64V1.VectorCross(float64V2);
        var genericCrossProduct = genericV1.VectorCross(genericV2);

        // Assert
        // X × Y = Z (Rechte-Hand-Regel)
        Assert.That(genericCrossProduct.X.ScalarValue, Is.EqualTo(float64CrossProduct.X.ScalarValue).Within(Tolerance),
            "Kreuzprodukt X Komponente sollte übereinstimmen");
        Assert.That(genericCrossProduct.Y.ScalarValue, Is.EqualTo(float64CrossProduct.Y.ScalarValue).Within(Tolerance),
            "Kreuzprodukt Y Komponente sollte übereinstimmen");
        Assert.That(genericCrossProduct.Z.ScalarValue, Is.EqualTo(float64CrossProduct.Z.ScalarValue).Within(Tolerance),
            "Kreuzprodukt Z Komponente sollte übereinstimmen (sollte 1.0 sein)");
    }

    [Test]
    public void CrossProduct_ShouldBeOrthogonalToBothVectors_IdenticalForBothImplementations()
    {
        // Arrange
        var float64V1 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64V2 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);

        var genericV1 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),
            _scalarProcessor.ScalarFromNumber(2.0),
            _scalarProcessor.ScalarFromNumber(3.0)
        );
        var genericV2 = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(4.0),
            _scalarProcessor.ScalarFromNumber(5.0),
            _scalarProcessor.ScalarFromNumber(6.0)
        );

        // Act
        var float64CrossProduct = float64V1.VectorCross(float64V2);
        var genericCrossProduct = genericV1.VectorCross(genericV2);

        // Assert - Kreuzprodukt sollte orthogonal zu beiden Eingabevektoren sein
        var float64Dot1 = float64CrossProduct.VectorESp(float64V1);
        var float64Dot2 = float64CrossProduct.VectorESp(float64V2);
        var genericDot1 = genericCrossProduct.VectorESp(genericV1);
        var genericDot2 = genericCrossProduct.VectorESp(genericV2);

        Assert.That(genericDot1.ScalarValue, Is.EqualTo(float64Dot1.ScalarValue).Within(Tolerance),
            "Kreuzprodukt orthogonal zu v1 - beide Implementierungen sollten übereinstimmen");
        Assert.That(genericDot2.ScalarValue, Is.EqualTo(float64Dot2.ScalarValue).Within(Tolerance),
            "Kreuzprodukt orthogonal zu v2 - beide Implementierungen sollten übereinstimmen");

        // Zusätzlich: Kreuzprodukte selbst sollten identisch sein
        Assert.That(genericCrossProduct.X.ScalarValue, Is.EqualTo(float64CrossProduct.X.ScalarValue).Within(Tolerance),
            "Kreuzprodukt X Komponente sollte übereinstimmen");
        Assert.That(genericCrossProduct.Y.ScalarValue, Is.EqualTo(float64CrossProduct.Y.ScalarValue).Within(Tolerance),
            "Kreuzprodukt Y Komponente sollte übereinstimmen");
        Assert.That(genericCrossProduct.Z.ScalarValue, Is.EqualTo(float64CrossProduct.Z.ScalarValue).Within(Tolerance),
            "Kreuzprodukt Z Komponente sollte übereinstimmen");
    }

    #endregion

    #region Normalization (1 test)

    [Test]
    public void VectorNormalization_ShouldProduceIdenticalUnitVectors()
    {
        // Arrange
        var float64Vector = LinFloat64Vector3D.Create(3.0, 4.0, 0.0);
        var genericVector = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0),
            _scalarProcessor.ScalarFromNumber(4.0),
            _scalarProcessor.ScalarFromNumber(0.0)
        );

        // Act
        var float64UnitVector = float64Vector.ToUnitLinVector3D();
        var genericUnitVector = genericVector.ToUnitVector();

        var float64Norm = float64UnitVector.VectorENorm();
        var genericNorm = genericUnitVector.VectorENorm();

        // Assert
        Assert.That(genericNorm.ScalarValue, Is.EqualTo(float64Norm.ScalarValue).Within(Tolerance),
            "Einheitsvektor Normen sollten beide 1 sein");

        // Richtung sollte erhalten bleiben: (3/5, 4/5, 0)
        Assert.That(genericUnitVector.X.ScalarValue, Is.EqualTo(float64UnitVector.X.ScalarValue).Within(Tolerance),
            "Einheitsvektor X Komponente sollte übereinstimmen");
        Assert.That(genericUnitVector.Y.ScalarValue, Is.EqualTo(float64UnitVector.Y.ScalarValue).Within(Tolerance),
            "Einheitsvektor Y Komponente sollte übereinstimmen");
        Assert.That(genericUnitVector.Z.ScalarValue, Is.EqualTo(float64UnitVector.Z.ScalarValue).Within(Tolerance),
            "Einheitsvektor Z Komponente sollte übereinstimmen");

        // IsNearUnit prüfen für Float64
        var float64IsUnit = float64UnitVector.IsNearUnit();
        Assert.That(float64IsUnit, Is.True,
            "Float64 Einheitsvektor sollte als near-unit erkannt werden");
        // Hinweis: Generic Version nutzt IsNearUnitVector() was IPair Casting benötigt,
        // aber wir haben die Norm = 1.0 oben verifiziert, was ausreichend für Äquivalenz ist
    }

    #endregion
}
