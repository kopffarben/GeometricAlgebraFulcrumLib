using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Äquivalenztests für LinBivector2D und LinBivector3D - LinearAlgebra Phase.
/// Testet ob Float64 und Generic&lt;double&gt; Implementierungen identische Ergebnisse liefern.
///
/// Bivektoren repräsentieren orientierte Flächen (2D) oder orientierte Ebenen (3D).
/// Folgt dem gleichen Äquivalenztest-Pattern wie Vector2D/3D.
/// </summary>
[TestFixture]
public class LinBivectorEquivalenceTests
{
    private const double Tolerance = 1e-14; // Striktere Toleranz für Äquivalenztests
    private IScalarProcessor<double> _scalarProcessor = null!;

    [SetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region 2D Bivector Tests (4 tests)

    [Test]
    public void Bivector2D_Construction_ShouldHaveIdenticalComponents()
    {
        // Arrange
        double scalar12 = 5.0;

        // Act
        var float64Bivector = LinFloat64Bivector2D.Create(scalar12);
        var genericBivector = LinBivector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(scalar12)
        );

        // Assert
        Assert.That(genericBivector.Scalar12.ScalarValue, Is.EqualTo(float64Bivector.Scalar12.ScalarValue).Within(Tolerance),
            "Scalar12 Komponenten sollten übereinstimmen");
        Assert.That(genericBivector.Xy.ScalarValue, Is.EqualTo(float64Bivector.Xy.ScalarValue).Within(Tolerance),
            "Xy Eigenschaft sollte mit Scalar12 übereinstimmen");
    }

    [Test]
    public void Bivector2D_BasisBivectors_ShouldBeIdentical()
    {
        // Arrange
        var float64E12 = LinFloat64Bivector2D.E12;
        var float64E21 = LinFloat64Bivector2D.E21;

        var genericE12 = LinBivector2D<double>.E12(_scalarProcessor);
        var genericE21 = LinBivector2D<double>.E21(_scalarProcessor);

        // Assert
        Assert.That(genericE12.Scalar12.ScalarValue, Is.EqualTo(float64E12.Scalar12.ScalarValue).Within(Tolerance),
            "E12 sollte +1 sein");
        Assert.That(genericE21.Scalar12.ScalarValue, Is.EqualTo(float64E21.Scalar12.ScalarValue).Within(Tolerance),
            "E21 sollte -1 sein");

        // E12 und E21 sind anti-kommutativ: e21 = -e12
        Assert.That(genericE21.Scalar12.ScalarValue, Is.EqualTo(-genericE12.Scalar12.ScalarValue).Within(Tolerance),
            "E21 sollte negativ von E12 sein");
    }

    [Test]
    public void Bivector2D_Dual_ShouldProduceIdenticalScalars()
    {
        // Arrange
        double scalar12 = 3.0;
        var float64Bivector = LinFloat64Bivector2D.Create(scalar12);
        var genericBivector = LinBivector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(scalar12)
        );

        // Act
        var float64Dual = float64Bivector.Dual2D();
        var genericDual = genericBivector.Dual2D();

        // Assert
        // In 2D ist das Dual eines Bivektors ein Skalar (Pseudoskalar)
        Assert.That(genericDual.Scalar.ScalarValue, Is.EqualTo(float64Dual.Scalar.ScalarValue).Within(Tolerance),
            "Dual von Bivektor e12 sollte Skalar sein");

        // Test UnDual (sollte negieren)
        var float64UnDual = float64Bivector.UnDual2D();
        var genericUnDual = genericBivector.UnDual2D();
        Assert.That(genericUnDual.Scalar.ScalarValue, Is.EqualTo(float64UnDual.Scalar.ScalarValue).Within(Tolerance),
            "UnDual sollte Skalar negieren");
    }

    [Test]
    public void Bivector2D_ScalarProduct_ShouldProduceIdenticalValues()
    {
        // Arrange
        var float64B1 = LinFloat64Bivector2D.Create(2.0);
        var float64B2 = LinFloat64Bivector2D.Create(3.0);

        var genericB1 = LinBivector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(2.0)
        );
        var genericB2 = LinBivector2D<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0)
        );

        // Act
        var float64ScalarProduct = float64B1.Sp(float64B2);
        var genericScalarProduct = genericB1.Sp(genericB2);

        // Assert
        // Skalarprodukt von Bivektoren: b1 · b2 = -(Scalar12_1 * Scalar12_2)
        // Für b1 = 2*e12 und b2 = 3*e12: Sp = -(2*3) = -6
        Assert.That(genericScalarProduct.ScalarValue, Is.EqualTo(float64ScalarProduct.ScalarValue).Within(Tolerance),
            "Skalarprodukt paralleler Bivektoren sollte übereinstimmen");
    }

    #endregion

    #region 3D Bivector Tests (4 tests)

    [Test]
    public void Bivector3D_Construction_ShouldHaveIdenticalComponents()
    {
        // Arrange
        double scalar12 = 1.0, scalar13 = 2.0, scalar23 = 3.0;

        // Act
        var float64Bivector = LinFloat64Bivector3D.Create(scalar12, scalar13, scalar23);
        var genericBivector = LinBivector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(scalar12),
            _scalarProcessor.ScalarFromNumber(scalar13),
            _scalarProcessor.ScalarFromNumber(scalar23)
        );

        // Assert
        Assert.That(genericBivector.Scalar12.ScalarValue, Is.EqualTo(float64Bivector.Scalar12.ScalarValue).Within(Tolerance),
            "Scalar12 sollte übereinstimmen");
        Assert.That(genericBivector.Scalar13.ScalarValue, Is.EqualTo(float64Bivector.Scalar13.ScalarValue).Within(Tolerance),
            "Scalar13 sollte übereinstimmen");
        Assert.That(genericBivector.Scalar23.ScalarValue, Is.EqualTo(float64Bivector.Scalar23.ScalarValue).Within(Tolerance),
            "Scalar23 sollte übereinstimmen");

        // Test alternative Eigenschaftsnamen
        Assert.That(genericBivector.Xy.ScalarValue, Is.EqualTo(float64Bivector.Xy.ScalarValue).Within(Tolerance),
            "Xy = Scalar12");
        Assert.That(genericBivector.Xz.ScalarValue, Is.EqualTo(float64Bivector.Xz.ScalarValue).Within(Tolerance),
            "Xz = Scalar13");
        Assert.That(genericBivector.Yz.ScalarValue, Is.EqualTo(float64Bivector.Yz.ScalarValue).Within(Tolerance),
            "Yz = Scalar23");
    }

    [Test]
    public void Bivector3D_BasisBivectors_ShouldBeIdenticalAndOrthogonal()
    {
        // Arrange
        var float64E12 = LinFloat64Bivector3D.E12;
        var float64E13 = LinFloat64Bivector3D.E13;
        var float64E23 = LinFloat64Bivector3D.E23;

        var genericE12 = LinBivector3D<double>.E12(_scalarProcessor);
        var genericE13 = LinBivector3D<double>.E13(_scalarProcessor);
        var genericE23 = LinBivector3D<double>.E23(_scalarProcessor);

        // Assert - Jeder Basis-Bivektor sollte Einheitsnorm haben
        Assert.That(genericE12.Norm().ScalarValue, Is.EqualTo(float64E12.Norm().ScalarValue).Within(Tolerance),
            "E12 Norm sollte übereinstimmen (sollte 1 sein)");
        Assert.That(genericE13.Norm().ScalarValue, Is.EqualTo(float64E13.Norm().ScalarValue).Within(Tolerance),
            "E13 Norm sollte übereinstimmen (sollte 1 sein)");
        Assert.That(genericE23.Norm().ScalarValue, Is.EqualTo(float64E23.Norm().ScalarValue).Within(Tolerance),
            "E23 Norm sollte übereinstimmen (sollte 1 sein)");

        // Assert - Basis-Bivektoren sollten orthogonal sein (Skalarprodukt = 0)
        Assert.That(genericE12.Sp(genericE13).ScalarValue, Is.EqualTo(float64E12.Sp(float64E13).ScalarValue).Within(Tolerance),
            "E12 und E13 Skalarprodukt sollte übereinstimmen (sollte 0 sein)");
        Assert.That(genericE12.Sp(genericE23).ScalarValue, Is.EqualTo(float64E12.Sp(float64E23).ScalarValue).Within(Tolerance),
            "E12 und E23 Skalarprodukt sollte übereinstimmen (sollte 0 sein)");
        Assert.That(genericE13.Sp(genericE23).ScalarValue, Is.EqualTo(float64E13.Sp(float64E23).ScalarValue).Within(Tolerance),
            "E13 und E23 Skalarprodukt sollte übereinstimmen (sollte 0 sein)");
    }

    [Test]
    public void Bivector3D_Dual_ShouldProduceIdenticalVectors()
    {
        // Arrange
        double scalar12 = 1.0, scalar13 = 2.0, scalar23 = 3.0;
        var float64Bivector = LinFloat64Bivector3D.Create(scalar12, scalar13, scalar23);
        var genericBivector = LinBivector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(scalar12),
            _scalarProcessor.ScalarFromNumber(scalar13),
            _scalarProcessor.ScalarFromNumber(scalar23)
        );

        // Act
        var float64Dual = float64Bivector.Dual3D();
        var genericDual = genericBivector.Dual3D();

        // Assert
        // In 3D ist das Dual eines Bivektors ein Vektor (Hodge Dual)
        // Dual(e12) = e3, Dual(e13) = -e2, Dual(e23) = e1
        Assert.That(genericDual.X.ScalarValue, Is.EqualTo(float64Dual.X.ScalarValue).Within(Tolerance),
            "X Komponente sollte übereinstimmen = Scalar23");
        Assert.That(genericDual.Y.ScalarValue, Is.EqualTo(float64Dual.Y.ScalarValue).Within(Tolerance),
            "Y Komponente sollte übereinstimmen = -Scalar13");
        Assert.That(genericDual.Z.ScalarValue, Is.EqualTo(float64Dual.Z.ScalarValue).Within(Tolerance),
            "Z Komponente sollte übereinstimmen = Scalar12");
    }

    [Test]
    public void Bivector3D_Norm_ShouldProduceIdenticalValues()
    {
        // Arrange
        double scalar12 = 2.0, scalar13 = 3.0, scalar23 = 6.0;
        var float64Bivector = LinFloat64Bivector3D.Create(scalar12, scalar13, scalar23);
        var genericBivector = LinBivector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(scalar12),
            _scalarProcessor.ScalarFromNumber(scalar13),
            _scalarProcessor.ScalarFromNumber(scalar23)
        );

        // Act
        var float64Norm = float64Bivector.Norm();
        var float64NormSquared = float64Bivector.NormSquared();
        var genericNorm = genericBivector.Norm();
        var genericNormSquared = genericBivector.NormSquared();

        // Assert
        // ||B|| = sqrt(Scalar12² + Scalar13² + Scalar23²)
        // ||B|| = sqrt(2² + 3² + 6²) = sqrt(4 + 9 + 36) = sqrt(49) = 7
        Assert.That(genericNormSquared.ScalarValue, Is.EqualTo(float64NormSquared.ScalarValue).Within(Tolerance),
            "Norm quadriert sollte übereinstimmen (sollte 49 sein)");
        Assert.That(genericNorm.ScalarValue, Is.EqualTo(float64Norm.ScalarValue).Within(Tolerance),
            "Norm sollte übereinstimmen (sollte 7 sein)");
    }

    #endregion
}
