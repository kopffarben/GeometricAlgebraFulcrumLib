using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Äquivalenztests für LinQuaternion - LinearAlgebra Phase.
/// Testet ob Float64 und Generic&lt;double&gt; Implementierungen identische Ergebnisse liefern.
///
/// Quaternionen repräsentieren 3D-Rotationen: q = w + xi + yj + zk
/// Folgt dem gleichen Äquivalenztest-Pattern wie Vector2D/3D und Bivector.
/// </summary>
[TestFixture]
public class LinQuaternionEquivalenceTests
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
    public void Quaternion_Construction_ShouldHaveIdenticalComponents()
    {
        // Arrange
        double w = 1.0, x = 2.0, y = 3.0, z = 4.0;

        // Act - Both APIs now use (x, y, z, w) parameter order
        var float64Quaternion = LinFloat64Quaternion.Create(x, y, z, w);
        var genericQuaternion = LinQuaternion<double>.Create(
            _scalarProcessor.ScalarFromNumber(x),
            _scalarProcessor.ScalarFromNumber(y),
            _scalarProcessor.ScalarFromNumber(z),
            _scalarProcessor.ScalarFromNumber(w)
        );

        // Assert
        Assert.That(genericQuaternion.Scalar.ScalarValue, Is.EqualTo(float64Quaternion.Scalar.ScalarValue).Within(Tolerance),
            "Scalar (w) Komponenten sollten übereinstimmen");
        Assert.That(genericQuaternion.ScalarI.ScalarValue, Is.EqualTo(float64Quaternion.ScalarI.ScalarValue).Within(Tolerance),
            "ScalarI (x) Komponenten sollten übereinstimmen");
        Assert.That(genericQuaternion.ScalarJ.ScalarValue, Is.EqualTo(float64Quaternion.ScalarJ.ScalarValue).Within(Tolerance),
            "ScalarJ (y) Komponenten sollten übereinstimmen");
        Assert.That(genericQuaternion.ScalarK.ScalarValue, Is.EqualTo(float64Quaternion.ScalarK.ScalarValue).Within(Tolerance),
            "ScalarK (z) Komponenten sollten übereinstimmen");
    }

    [Test]
    public void Quaternion_Identity_ShouldBeIdentical()
    {
        // Arrange & Act
        var float64Identity = LinFloat64Quaternion.Identity;
        var genericIdentity = LinQuaternion<double>.Identity(_scalarProcessor);

        // Assert
        Assert.That(genericIdentity.Scalar.ScalarValue, Is.EqualTo(float64Identity.Scalar.ScalarValue).Within(Tolerance),
            "Identity Scalar sollte 1 sein");
        Assert.That(genericIdentity.ScalarI.ScalarValue, Is.EqualTo(float64Identity.ScalarI.ScalarValue).Within(Tolerance),
            "Identity i Komponente sollte 0 sein");
        Assert.That(genericIdentity.ScalarJ.ScalarValue, Is.EqualTo(float64Identity.ScalarJ.ScalarValue).Within(Tolerance),
            "Identity j Komponente sollte 0 sein");
        Assert.That(genericIdentity.ScalarK.ScalarValue, Is.EqualTo(float64Identity.ScalarK.ScalarValue).Within(Tolerance),
            "Identity k Komponente sollte 0 sein");

        // Both should recognize as identity
        Assert.That(float64Identity.IsIdentity(), Is.True, "Float64 sollte als Identity erkannt werden");
        Assert.That(genericIdentity.IsIdentity(), Is.True, "Generic sollte als Identity erkannt werden");
    }

    #endregion

    #region Norm and Normalization (2 tests)

    [Test]
    public void Quaternion_Norm_ShouldProduceIdenticalValues()
    {
        // Arrange
        var float64Quaternion = LinFloat64Quaternion.Create(2.0, 3.0, 4.0, 1.0);
        var genericQuaternion = LinQuaternion<double>.Create(
            _scalarProcessor.ScalarFromNumber(2.0),  // i
            _scalarProcessor.ScalarFromNumber(3.0),  // j
            _scalarProcessor.ScalarFromNumber(4.0),  // k
            _scalarProcessor.ScalarFromNumber(1.0)   // scalar
        );

        // Act
        var float64Norm = float64Quaternion.Norm();
        var float64NormSquared = float64Quaternion.NormSquared();
        var genericNorm = genericQuaternion.Norm();
        var genericNormSquared = genericQuaternion.NormSquared();

        // Assert
        // ||q|| = sqrt(1² + 2² + 3² + 4²) = sqrt(30)
        Assert.That(genericNormSquared.ScalarValue, Is.EqualTo(float64NormSquared.ScalarValue).Within(Tolerance),
            "Norm quadriert sollte übereinstimmen (sollte 30 sein)");
        Assert.That(genericNorm.ScalarValue, Is.EqualTo(float64Norm.ScalarValue).Within(Tolerance),
            "Norm sollte übereinstimmen (sollte sqrt(30) sein)");
    }

    [Test]
    public void Quaternion_Normalize_ShouldProduceIdenticalUnitQuaternions()
    {
        // Arrange
        var float64Quaternion = LinFloat64Quaternion.Create(2.0, 3.0, 4.0, 1.0);
        var genericQuaternion = LinQuaternion<double>.Create(
            _scalarProcessor.ScalarFromNumber(2.0),  // i
            _scalarProcessor.ScalarFromNumber(3.0),  // j
            _scalarProcessor.ScalarFromNumber(4.0),  // k
            _scalarProcessor.ScalarFromNumber(1.0)   // scalar
        );

        // Act
        var float64UnitQ = float64Quaternion.Normalize();
        var genericUnitQ = genericQuaternion.Normalize();

        var float64Norm = float64UnitQ.Norm();
        var genericNorm = genericUnitQ.Norm();

        // Assert
        Assert.That(genericNorm.ScalarValue, Is.EqualTo(float64Norm.ScalarValue).Within(Tolerance),
            "Normalisierte Quaternion Normen sollten beide 1 sein");

        // All components should match
        Assert.That(genericUnitQ.Scalar.ScalarValue, Is.EqualTo(float64UnitQ.Scalar.ScalarValue).Within(Tolerance),
            "Normalisierte Scalar Komponente sollte übereinstimmen");
        Assert.That(genericUnitQ.ScalarI.ScalarValue, Is.EqualTo(float64UnitQ.ScalarI.ScalarValue).Within(Tolerance),
            "Normalisierte i Komponente sollte übereinstimmen");
        Assert.That(genericUnitQ.ScalarJ.ScalarValue, Is.EqualTo(float64UnitQ.ScalarJ.ScalarValue).Within(Tolerance),
            "Normalisierte j Komponente sollte übereinstimmen");
        Assert.That(genericUnitQ.ScalarK.ScalarValue, Is.EqualTo(float64UnitQ.ScalarK.ScalarValue).Within(Tolerance),
            "Normalisierte k Komponente sollte übereinstimmen");

        // Check normalized status
        Assert.That(float64UnitQ.IsNearNormalized(Tolerance), Is.True, "Float64 sollte als normalisiert erkannt werden");
        Assert.That(genericUnitQ.IsNearNormalized(), Is.True, "Generic sollte als normalisiert erkannt werden");
    }

    #endregion

    #region Conjugate and Inverse (2 tests)

    [Test]
    public void Quaternion_Conjugate_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Q = LinFloat64Quaternion.Create(2.0, 3.0, 4.0, 1.0);
        var genericQ = LinQuaternion<double>.Create(
            _scalarProcessor.ScalarFromNumber(2.0),  // i
            _scalarProcessor.ScalarFromNumber(3.0),  // j
            _scalarProcessor.ScalarFromNumber(4.0),  // k
            _scalarProcessor.ScalarFromNumber(1.0)   // scalar
        );

        // Act
        var float64Conjugate = float64Q.Conjugate();
        var genericConjugate = genericQ.Conjugate();

        // Assert
        // Conjugate: q* = w - xi - yj - zk
        Assert.That(genericConjugate.Scalar.ScalarValue, Is.EqualTo(float64Conjugate.Scalar.ScalarValue).Within(Tolerance),
            "Konjugat behält Scalar Teil");
        Assert.That(genericConjugate.ScalarI.ScalarValue, Is.EqualTo(float64Conjugate.ScalarI.ScalarValue).Within(Tolerance),
            "Konjugat negiert i Komponente");
        Assert.That(genericConjugate.ScalarJ.ScalarValue, Is.EqualTo(float64Conjugate.ScalarJ.ScalarValue).Within(Tolerance),
            "Konjugat negiert j Komponente");
        Assert.That(genericConjugate.ScalarK.ScalarValue, Is.EqualTo(float64Conjugate.ScalarK.ScalarValue).Within(Tolerance),
            "Konjugat negiert k Komponente");

        // q * q* should give norm squared (scalar only)
        var float64Product = float64Q * float64Conjugate;
        var genericProduct = genericQ * genericConjugate;

        Assert.That(genericProduct.Scalar.ScalarValue, Is.EqualTo(float64Product.Scalar.ScalarValue).Within(Tolerance),
            "q * q* sollte Norm-Quadrat als Scalar ergeben");
        Assert.That(genericProduct.ScalarI.ScalarValue, Is.EqualTo(float64Product.ScalarI.ScalarValue).Within(Tolerance),
            "q * q* sollte Null-Vektor-Teil haben (i=0)");
    }

    [Test]
    public void Quaternion_Inverse_ShouldSatisfyIdenticalIdentity()
    {
        // Arrange
        var float64Q = LinFloat64Quaternion.Create(2.0, 3.0, 4.0, 1.0);
        var genericQ = LinQuaternion<double>.Create(
            _scalarProcessor.ScalarFromNumber(2.0),  // i
            _scalarProcessor.ScalarFromNumber(3.0),  // j
            _scalarProcessor.ScalarFromNumber(4.0),  // k
            _scalarProcessor.ScalarFromNumber(1.0)   // scalar
        );

        // Act
        var float64Inverse = float64Q.Inverse();
        var genericInverse = genericQ.Inverse();

        var float64Product1 = float64Q * float64Inverse;
        var genericProduct1 = genericQ * genericInverse;

        var float64Product2 = float64Inverse * float64Q;
        var genericProduct2 = genericInverse * genericQ;

        // Assert
        // q * q^-1 = q^-1 * q = identity
        Assert.That(float64Product1.IsNearIdentity(Tolerance), Is.True,
            "Float64: q * q^-1 sollte Identity sein");
        Assert.That(genericProduct1.IsNearIdentity(), Is.True,
            "Generic: q * q^-1 sollte Identity sein");

        Assert.That(float64Product2.IsNearIdentity(Tolerance), Is.True,
            "Float64: q^-1 * q sollte Identity sein");
        Assert.That(genericProduct2.IsNearIdentity(), Is.True,
            "Generic: q^-1 * q sollte Identity sein");

        // Inverse components should match
        Assert.That(genericInverse.Scalar.ScalarValue, Is.EqualTo(float64Inverse.Scalar.ScalarValue).Within(Tolerance),
            "Inverse Scalar Komponente sollte übereinstimmen");
    }

    #endregion

    #region Quaternion Multiplication (2 tests)

    [Test]
    public void Quaternion_Multiplication_ShouldBeIdenticalAndNonCommutative()
    {
        // Arrange
        var float64Q1 = LinFloat64Quaternion.Create(1.0, 0.0, 0.0, 1.0);
        var float64Q2 = LinFloat64Quaternion.Create(0.0, 1.0, 0.0, 1.0);

        var genericQ1 = LinQuaternion<double>.Create(
            _scalarProcessor.ScalarFromNumber(1.0),  // i
            _scalarProcessor.ScalarFromNumber(0.0),  // j
            _scalarProcessor.ScalarFromNumber(0.0),  // k
            _scalarProcessor.ScalarFromNumber(1.0)   // scalar
        );
        var genericQ2 = LinQuaternion<double>.Create(
            _scalarProcessor.ScalarFromNumber(0.0),  // i
            _scalarProcessor.ScalarFromNumber(1.0),  // j
            _scalarProcessor.ScalarFromNumber(0.0),  // k
            _scalarProcessor.ScalarFromNumber(1.0)   // scalar
        );

        // Act
        var float64Product12 = float64Q1 * float64Q2;
        var genericProduct12 = genericQ1 * genericQ2;

        var float64Product21 = float64Q2 * float64Q1;
        var genericProduct21 = genericQ2 * genericQ1;

        // Assert
        // Products should match between implementations
        Assert.That(genericProduct12.Scalar.ScalarValue, Is.EqualTo(float64Product12.Scalar.ScalarValue).Within(Tolerance),
            "q1 * q2 Scalar sollte übereinstimmen");
        Assert.That(genericProduct12.ScalarI.ScalarValue, Is.EqualTo(float64Product12.ScalarI.ScalarValue).Within(Tolerance),
            "q1 * q2 i-Komponente sollte übereinstimmen");
        Assert.That(genericProduct12.ScalarJ.ScalarValue, Is.EqualTo(float64Product12.ScalarJ.ScalarValue).Within(Tolerance),
            "q1 * q2 j-Komponente sollte übereinstimmen");
        Assert.That(genericProduct12.ScalarK.ScalarValue, Is.EqualTo(float64Product12.ScalarK.ScalarValue).Within(Tolerance),
            "q1 * q2 k-Komponente sollte übereinstimmen");

        // Verify non-commutativity in both implementations
        Assert.That(genericProduct21.Scalar.ScalarValue, Is.EqualTo(float64Product21.Scalar.ScalarValue).Within(Tolerance),
            "q2 * q1 sollte ebenfalls übereinstimmen");
    }

    [Test]
    public void Quaternion_IdentityMultiplication_ShouldPreserveQuaternion()
    {
        // Arrange
        var float64Q = LinFloat64Quaternion.Create(3.0, 4.0, 5.0, 2.0);
        var genericQ = LinQuaternion<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0),  // i
            _scalarProcessor.ScalarFromNumber(4.0),  // j
            _scalarProcessor.ScalarFromNumber(5.0),  // k
            _scalarProcessor.ScalarFromNumber(2.0)   // scalar
        );

        var float64Identity = LinFloat64Quaternion.Identity;
        var genericIdentity = LinQuaternion<double>.Identity(_scalarProcessor);

        // Act
        var float64Product1 = float64Q * float64Identity;
        var genericProduct1 = genericQ * genericIdentity;

        var float64Product2 = float64Identity * float64Q;
        var genericProduct2 = genericIdentity * genericQ;

        // Assert
        // q * I = q
        Assert.That(genericProduct1.Scalar.ScalarValue, Is.EqualTo(float64Product1.Scalar.ScalarValue).Within(Tolerance),
            "q * I Scalar sollte q entsprechen");
        Assert.That(genericProduct1.ScalarI.ScalarValue, Is.EqualTo(float64Product1.ScalarI.ScalarValue).Within(Tolerance),
            "q * I i sollte q entsprechen");
        Assert.That(genericProduct1.ScalarJ.ScalarValue, Is.EqualTo(float64Product1.ScalarJ.ScalarValue).Within(Tolerance),
            "q * I j sollte q entsprechen");
        Assert.That(genericProduct1.ScalarK.ScalarValue, Is.EqualTo(float64Product1.ScalarK.ScalarValue).Within(Tolerance),
            "q * I k sollte q entsprechen");

        // I * q = q
        Assert.That(genericProduct2.Scalar.ScalarValue, Is.EqualTo(float64Product2.Scalar.ScalarValue).Within(Tolerance),
            "I * q sollte ebenfalls q entsprechen");
    }

    #endregion

    #region Rotation Tests (2 tests)

    [Test]
    public void Quaternion_FromAxisAngle_ShouldCreateIdenticalRotations()
    {
        // Arrange
        var float64Angle = LinFloat64PolarAngle.CreateFromDegrees(90);
        var genericAngle = LinPolarAngle<double>.CreateFromDegrees(_scalarProcessor, 90);

        var float64Axis = LinFloat64Vector3D.E3; // Z-axis
        var genericAxis = LinVector3D<double>.E3(_scalarProcessor);

        // Act
        var float64Q = LinFloat64Quaternion.CreateFromAxisAngle(float64Axis, float64Angle);
        var genericQ = LinQuaternion<double>.CreateFromNormalAndAngle(genericAxis, genericAngle);

        // Assert
        // For 90° rotation around Z: q = cos(45°) + sin(45°) * k
        Assert.That(genericQ.Scalar.ScalarValue, Is.EqualTo(float64Q.Scalar.ScalarValue).Within(Tolerance),
            "Rotation Quaternion Scalar sollte übereinstimmen");
        Assert.That(genericQ.ScalarI.ScalarValue, Is.EqualTo(float64Q.ScalarI.ScalarValue).Within(Tolerance),
            "i Komponente sollte 0 sein für Z-Achsen-Rotation");
        Assert.That(genericQ.ScalarJ.ScalarValue, Is.EqualTo(float64Q.ScalarJ.ScalarValue).Within(Tolerance),
            "j Komponente sollte 0 sein für Z-Achsen-Rotation");
        Assert.That(genericQ.ScalarK.ScalarValue, Is.EqualTo(float64Q.ScalarK.ScalarValue).Within(Tolerance),
            "k Komponente sollte sin(45°) sein");

        // Both should be normalized
        Assert.That(float64Q.IsNearNormalized(Tolerance), Is.True, "Float64 Rotations-Quaternion sollte normalisiert sein");
        Assert.That(genericQ.IsNearNormalized(), Is.True, "Generic Rotations-Quaternion sollte normalisiert sein");
    }

    [Test]
    public void Quaternion_RotateVector_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Angle = LinFloat64PolarAngle.CreateFromDegrees(90);
        var genericAngle = LinPolarAngle<double>.CreateFromDegrees(_scalarProcessor, 90);

        var float64Axis = LinFloat64Vector3D.E3; // Z-axis
        var genericAxis = LinVector3D<double>.E3(_scalarProcessor);

        var float64Q = LinFloat64Quaternion.CreateFromAxisAngle(float64Axis, float64Angle);
        var genericQ = LinQuaternion<double>.CreateFromNormalAndAngle(genericAxis, genericAngle);

        var float64Vector = LinFloat64Vector3D.E1; // X-axis
        var genericVector = LinVector3D<double>.E1(_scalarProcessor);

        // Act
        var float64Rotated = float64Q.RotateVector(float64Vector);
        var genericRotated = genericQ.RotateVector(genericVector);

        // Assert
        // 90° rotation around Z should map X-axis to Y-axis
        Assert.That(genericRotated.X.ScalarValue, Is.EqualTo(float64Rotated.X.ScalarValue).Within(Tolerance),
            "Rotierter X Komponente sollte übereinstimmen (sollte ~0 sein)");
        Assert.That(genericRotated.Y.ScalarValue, Is.EqualTo(float64Rotated.Y.ScalarValue).Within(Tolerance),
            "Rotierter Y Komponente sollte übereinstimmen (sollte ~1 sein)");
        Assert.That(genericRotated.Z.ScalarValue, Is.EqualTo(float64Rotated.Z.ScalarValue).Within(Tolerance),
            "Rotierter Z Komponente sollte übereinstimmen (sollte ~0 sein)");

        // Both should preserve length
        var float64Norm = float64Rotated.VectorENorm();
        var genericNorm = genericRotated.VectorENorm();

        Assert.That(genericNorm.ScalarValue, Is.EqualTo(float64Norm.ScalarValue).Within(Tolerance),
            "Rotation sollte Vektor-Länge erhalten");
    }

    #endregion

    #region Static Rotation Properties (6 tests)

    [Test]
    public void Quaternion_XyToXz_ShouldBeIdentical()
    {
        // Arrange & Act
        var float64Q = LinFloat64Quaternion.XyToXz;
        var genericQ = LinQuaternion<double>.XyToXz(_scalarProcessor);

        // Assert
        Assert.That(genericQ.ScalarI.ScalarValue, Is.EqualTo(float64Q.ScalarI.ScalarValue).Within(Tolerance),
            "XyToXz ScalarI sollte übereinstimmen");
        Assert.That(genericQ.ScalarJ.ScalarValue, Is.EqualTo(float64Q.ScalarJ.ScalarValue).Within(Tolerance),
            "XyToXz ScalarJ sollte übereinstimmen");
        Assert.That(genericQ.ScalarK.ScalarValue, Is.EqualTo(float64Q.ScalarK.ScalarValue).Within(Tolerance),
            "XyToXz ScalarK sollte übereinstimmen");
        Assert.That(genericQ.Scalar.ScalarValue, Is.EqualTo(float64Q.Scalar.ScalarValue).Within(Tolerance),
            "XyToXz Scalar sollte übereinstimmen");

        // Both should be normalized rotation quaternions
        Assert.That(float64Q.IsNearNormalized(Tolerance), Is.True, "Float64 XyToXz sollte normalisiert sein");
        Assert.That(genericQ.IsNearNormalized(), Is.True, "Generic XyToXz sollte normalisiert sein");
    }

    [Test]
    public void Quaternion_XyToYx_ShouldBeIdentical()
    {
        // Arrange & Act
        var float64Q = LinFloat64Quaternion.XyToYx;
        var genericQ = LinQuaternion<double>.XyToYx(_scalarProcessor);

        // Assert
        Assert.That(genericQ.ScalarI.ScalarValue, Is.EqualTo(float64Q.ScalarI.ScalarValue).Within(Tolerance),
            "XyToYx ScalarI sollte übereinstimmen");
        Assert.That(genericQ.ScalarJ.ScalarValue, Is.EqualTo(float64Q.ScalarJ.ScalarValue).Within(Tolerance),
            "XyToYx ScalarJ sollte übereinstimmen");
        Assert.That(genericQ.ScalarK.ScalarValue, Is.EqualTo(float64Q.ScalarK.ScalarValue).Within(Tolerance),
            "XyToYx ScalarK sollte übereinstimmen");
        Assert.That(genericQ.Scalar.ScalarValue, Is.EqualTo(float64Q.Scalar.ScalarValue).Within(Tolerance),
            "XyToYx Scalar sollte übereinstimmen");

        Assert.That(float64Q.IsNearNormalized(Tolerance), Is.True, "Float64 XyToYx sollte normalisiert sein");
        Assert.That(genericQ.IsNearNormalized(), Is.True, "Generic XyToYx sollte normalisiert sein");
    }

    [Test]
    public void Quaternion_XyToYz_ShouldBeIdentical()
    {
        // Arrange & Act
        var float64Q = LinFloat64Quaternion.XyToYz;
        var genericQ = LinQuaternion<double>.XyToYz(_scalarProcessor);

        // Assert
        Assert.That(genericQ.ScalarI.ScalarValue, Is.EqualTo(float64Q.ScalarI.ScalarValue).Within(Tolerance),
            "XyToYz ScalarI sollte übereinstimmen");
        Assert.That(genericQ.ScalarJ.ScalarValue, Is.EqualTo(float64Q.ScalarJ.ScalarValue).Within(Tolerance),
            "XyToYz ScalarJ sollte übereinstimmen");
        Assert.That(genericQ.ScalarK.ScalarValue, Is.EqualTo(float64Q.ScalarK.ScalarValue).Within(Tolerance),
            "XyToYz ScalarK sollte übereinstimmen");
        Assert.That(genericQ.Scalar.ScalarValue, Is.EqualTo(float64Q.Scalar.ScalarValue).Within(Tolerance),
            "XyToYz Scalar sollte übereinstimmen");

        Assert.That(float64Q.IsNearNormalized(Tolerance), Is.True, "Float64 XyToYz sollte normalisiert sein");
        Assert.That(genericQ.IsNearNormalized(), Is.True, "Generic XyToYz sollte normalisiert sein");
    }

    [Test]
    public void Quaternion_XyToZx_ShouldBeIdentical()
    {
        // Arrange & Act
        var float64Q = LinFloat64Quaternion.XyToZx;
        var genericQ = LinQuaternion<double>.XyToZx(_scalarProcessor);

        // Assert
        Assert.That(genericQ.ScalarI.ScalarValue, Is.EqualTo(float64Q.ScalarI.ScalarValue).Within(Tolerance),
            "XyToZx ScalarI sollte übereinstimmen");
        Assert.That(genericQ.ScalarJ.ScalarValue, Is.EqualTo(float64Q.ScalarJ.ScalarValue).Within(Tolerance),
            "XyToZx ScalarJ sollte übereinstimmen");
        Assert.That(genericQ.ScalarK.ScalarValue, Is.EqualTo(float64Q.ScalarK.ScalarValue).Within(Tolerance),
            "XyToZx ScalarK sollte übereinstimmen");
        Assert.That(genericQ.Scalar.ScalarValue, Is.EqualTo(float64Q.Scalar.ScalarValue).Within(Tolerance),
            "XyToZx Scalar sollte übereinstimmen");

        Assert.That(float64Q.IsNearNormalized(Tolerance), Is.True, "Float64 XyToZx sollte normalisiert sein");
        Assert.That(genericQ.IsNearNormalized(), Is.True, "Generic XyToZx sollte normalisiert sein");
    }

    [Test]
    public void Quaternion_XyToZy_ShouldBeIdentical()
    {
        // Arrange & Act
        var float64Q = LinFloat64Quaternion.XyToZy;
        var genericQ = LinQuaternion<double>.XyToZy(_scalarProcessor);

        // Assert
        Assert.That(genericQ.ScalarI.ScalarValue, Is.EqualTo(float64Q.ScalarI.ScalarValue).Within(Tolerance),
            "XyToZy ScalarI sollte übereinstimmen");
        Assert.That(genericQ.ScalarJ.ScalarValue, Is.EqualTo(float64Q.ScalarJ.ScalarValue).Within(Tolerance),
            "XyToZy ScalarJ sollte übereinstimmen");
        Assert.That(genericQ.ScalarK.ScalarValue, Is.EqualTo(float64Q.ScalarK.ScalarValue).Within(Tolerance),
            "XyToZy ScalarK sollte übereinstimmen");
        Assert.That(genericQ.Scalar.ScalarValue, Is.EqualTo(float64Q.Scalar.ScalarValue).Within(Tolerance),
            "XyToZy Scalar sollte übereinstimmen");

        Assert.That(float64Q.IsNearNormalized(Tolerance), Is.True, "Float64 XyToZy sollte normalisiert sein");
        Assert.That(genericQ.IsNearNormalized(), Is.True, "Generic XyToZy sollte normalisiert sein");
    }

    [Test]
    public void Quaternion_ZxToXy_ShouldBeIdentical()
    {
        // Arrange & Act
        var float64Q = LinFloat64Quaternion.ZxToXy;
        var genericQ = LinQuaternion<double>.ZxToXy(_scalarProcessor);

        // Assert
        Assert.That(genericQ.ScalarI.ScalarValue, Is.EqualTo(float64Q.ScalarI.ScalarValue).Within(Tolerance),
            "ZxToXy ScalarI sollte übereinstimmen");
        Assert.That(genericQ.ScalarJ.ScalarValue, Is.EqualTo(float64Q.ScalarJ.ScalarValue).Within(Tolerance),
            "ZxToXy ScalarJ sollte übereinstimmen");
        Assert.That(genericQ.ScalarK.ScalarValue, Is.EqualTo(float64Q.ScalarK.ScalarValue).Within(Tolerance),
            "ZxToXy ScalarK sollte übereinstimmen");
        Assert.That(genericQ.Scalar.ScalarValue, Is.EqualTo(float64Q.Scalar.ScalarValue).Within(Tolerance),
            "ZxToXy Scalar sollte übereinstimmen");

        Assert.That(float64Q.IsNearNormalized(Tolerance), Is.True, "Float64 ZxToXy sollte normalisiert sein");
        Assert.That(genericQ.IsNearNormalized(), Is.True, "Generic ZxToXy sollte normalisiert sein");
    }

    #endregion
}
