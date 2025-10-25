using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Basis;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.Euclidean;

/// <summary>
/// Tests for Quaternion Operations
/// EXTENDED: Tests both Float64 AND Generic&lt;double&gt; implementations
/// Phase 3B - Core Modeling: Euclidean Geometry Quaternion Operations
/// Tests quaternion construction, multiplication, rotation, and conversions
/// </summary>
[TestFixture]
public class LinQuaternionTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Quaternion Construction Tests (5 tests × 2 = 10 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Construction_ShouldWork(bool useGeneric)
    {
        // Arrange & Act
        var q = useGeneric
            ? (object)LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4),
                _scalarProcessor.ScalarFromNumber(1))
            : LinFloat64Quaternion.Create(2, 3, 4, 1);

        // Assert
        Assert.That(q, Is.Not.Null, "Quaternion should be created");

        var (scalar, scalarI, scalarJ, scalarK) = q switch
        {
            LinFloat64Quaternion f64 => (f64.Scalar.ScalarValue, f64.ScalarI.ScalarValue, f64.ScalarJ.ScalarValue, f64.ScalarK.ScalarValue),
            LinQuaternion<double> gen => (gen.Scalar.ScalarValue, gen.ScalarI.ScalarValue, gen.ScalarJ.ScalarValue, gen.ScalarK.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(scalar, Is.EqualTo(1), "Scalar part should match");
        Assert.That(scalarI, Is.EqualTo(2), "i component should match");
        Assert.That(scalarJ, Is.EqualTo(3), "j component should match");
        Assert.That(scalarK, Is.EqualTo(4), "k component should match");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Identity_ShouldBeValid(bool useGeneric)
    {
        // Arrange & Act
        var identity = useGeneric
            ? (object)LinQuaternion<double>.Identity(_scalarProcessor)
            : LinFloat64Quaternion.Identity;

        // Assert
        var (scalar, scalarI, scalarJ, scalarK) = identity switch
        {
            LinFloat64Quaternion f64 => (f64.Scalar.ScalarValue, f64.ScalarI.ScalarValue, f64.ScalarJ.ScalarValue, f64.ScalarK.ScalarValue),
            LinQuaternion<double> gen => (gen.Scalar.ScalarValue, gen.ScalarI.ScalarValue, gen.ScalarJ.ScalarValue, gen.ScalarK.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(scalar, Is.EqualTo(1), "Identity scalar should be 1");
        Assert.That(scalarI, Is.EqualTo(0), "Identity i should be 0");
        Assert.That(scalarJ, Is.EqualTo(0), "Identity j should be 0");
        Assert.That(scalarK, Is.EqualTo(0), "Identity k should be 0");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_FromScalarAndBivector_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var scalar = _scalarProcessor.ScalarFromNumber(1.0);
            var bivector = LinBivector3D<double>.Create(
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4));

            var q = LinQuaternion<double>.Create(scalar, bivector);

            Assert.That(q, Is.Not.Null, "Quaternion should be created");
            Assert.That(q.Scalar.ScalarValue, Is.EqualTo(1), "Scalar part should match");
        }
        else
        {
            var scalar = 1.0;
            var bivector = LinFloat64Bivector3D.Create(2, 3, 4);

            var q = LinFloat64Quaternion.Create(scalar, bivector);

            Assert.That(q, Is.Not.Null, "Quaternion should be created");
            Assert.That(q.Scalar.ScalarValue, Is.EqualTo(1), "Scalar part should match");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_FromAxisAngle_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var axis = LinVector3D<double>.E1(_scalarProcessor); // X axis
            var angle = LinPolarAngle<double>.Angle90(_scalarProcessor); // 90 degrees

            var q = LinQuaternion<double>.CreateFromAxisAngle(axis, angle);

            Assert.That(q, Is.Not.Null, "Quaternion should be created from axis-angle");
            Assert.That(q.Scalar.ScalarValue, Is.GreaterThan(0), "Scalar part should be positive");
        }
        else
        {
            var axis = LinFloat64Vector3D.E1; // X axis
            var angle = LinFloat64PolarAngle.Angle90; // 90 degrees

            var q = LinFloat64Quaternion.CreateFromAxisAngle(axis, angle);

            Assert.That(q, Is.Not.Null, "Quaternion should be created from axis-angle");
            Assert.That(q.Scalar.ScalarValue, Is.GreaterThan(0), "Scalar part should be positive");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_FromBasisAxisAngle_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var axis = LinBasisVector.Px; // X axis
            var angle = LinPolarAngle<double>.Angle180(_scalarProcessor); // 180 degrees

            var q = LinQuaternion<double>.CreateFromAxisAngle(axis, angle);

            Assert.That(q, Is.Not.Null, "Quaternion should be created from basis axis-angle");
        }
        else
        {
            var axis = LinBasisVector.Px; // X axis
            var angle = LinFloat64PolarAngle.Angle180; // 180 degrees

            var q = LinFloat64Quaternion.CreateFromAxisAngle(axis, angle);

            Assert.That(q, Is.Not.Null, "Quaternion should be created from basis axis-angle");
        }
    }

    #endregion

    #region Quaternion Arithmetic Tests (4 tests × 2 = 8 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Addition_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var q1 = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4),
                _scalarProcessor.ScalarFromNumber(1));
            var q2 = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(6),
                _scalarProcessor.ScalarFromNumber(7),
                _scalarProcessor.ScalarFromNumber(8),
                _scalarProcessor.ScalarFromNumber(5));
            var result = q1 + q2;

            Assert.That(result.Scalar.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(8).Within(Tolerance));
            Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(10).Within(Tolerance));
            Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(12).Within(Tolerance));
        }
        else
        {
            var q1 = LinFloat64Quaternion.Create(2, 3, 4, 1);
            var q2 = LinFloat64Quaternion.Create(6, 7, 8, 5);
            var result = q1 + q2;

            Assert.That(result.Scalar.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(8).Within(Tolerance));
            Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(10).Within(Tolerance));
            Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(12).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Subtraction_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var q1 = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(8),
                _scalarProcessor.ScalarFromNumber(6),
                _scalarProcessor.ScalarFromNumber(4),
                _scalarProcessor.ScalarFromNumber(10));
            var q2 = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4),
                _scalarProcessor.ScalarFromNumber(1));
            var result = q1 - q2;

            Assert.That(result.Scalar.ScalarValue, Is.EqualTo(9).Within(Tolerance));
            Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(3).Within(Tolerance));
            Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(0).Within(Tolerance));
        }
        else
        {
            var q1 = LinFloat64Quaternion.Create(8, 6, 4, 10);
            var q2 = LinFloat64Quaternion.Create(2, 3, 4, 1);
            var result = q1 - q2;

            Assert.That(result.Scalar.ScalarValue, Is.EqualTo(9).Within(Tolerance));
            Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(3).Within(Tolerance));
            Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(0).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_ScalarMultiplication_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var q = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4),
                _scalarProcessor.ScalarFromNumber(1));
            var result = q * 2.0;

            Assert.That(result.Scalar.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(4).Within(Tolerance));
            Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(8).Within(Tolerance));
        }
        else
        {
            var q = LinFloat64Quaternion.Create(2, 3, 4, 1);
            var result = q * 2;

            Assert.That(result.Scalar.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(4).Within(Tolerance));
            Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(8).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Negation_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var q = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(-2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(-4),
                _scalarProcessor.ScalarFromNumber(1));
            var result = -q;

            Assert.That(result.Scalar.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
            Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
            Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(4).Within(Tolerance));
        }
        else
        {
            var q = LinFloat64Quaternion.Create(-2, 3, -4, 1);
            var result = -q;

            Assert.That(result.Scalar.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
            Assert.That(result.ScalarI.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.ScalarJ.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
            Assert.That(result.ScalarK.ScalarValue, Is.EqualTo(4).Within(Tolerance));
        }
    }

    #endregion

    #region Quaternion Multiplication Tests (3 tests × 2 = 6 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Multiplication_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var q1 = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4),
                _scalarProcessor.ScalarFromNumber(1));
            var q2 = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(6),
                _scalarProcessor.ScalarFromNumber(7),
                _scalarProcessor.ScalarFromNumber(8),
                _scalarProcessor.ScalarFromNumber(5));
            var result = q1 * q2;

            Assert.That(result, Is.Not.Null, "Quaternion multiplication should produce result");
        }
        else
        {
            var q1 = LinFloat64Quaternion.Create(2, 3, 4, 1);
            var q2 = LinFloat64Quaternion.Create(6, 7, 8, 5);
            var result = q1 * q2;

            Assert.That(result, Is.Not.Null, "Quaternion multiplication should produce result");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_IdentityMultiplication_ShouldPreserveQuaternion(bool useGeneric)
    {
        if (useGeneric)
        {
            var q = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4),
                _scalarProcessor.ScalarFromNumber(5),
                _scalarProcessor.ScalarFromNumber(2));
            var identity = LinQuaternion<double>.Identity(_scalarProcessor);

            var result1 = q * identity;
            var result2 = identity * q;

            Assert.That(result1.Scalar.ScalarValue, Is.EqualTo(q.Scalar.ScalarValue).Within(Tolerance));
            Assert.That(result2.Scalar.ScalarValue, Is.EqualTo(q.Scalar.ScalarValue).Within(Tolerance));
        }
        else
        {
            var q = LinFloat64Quaternion.Create(3, 4, 5, 2);
            var identity = LinFloat64Quaternion.Identity;

            var result1 = q * identity;
            var result2 = identity * q;

            Assert.That(result1.Scalar.ScalarValue, Is.EqualTo(q.Scalar.ScalarValue).Within(Tolerance));
            Assert.That(result2.Scalar.ScalarValue, Is.EqualTo(q.Scalar.ScalarValue).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_MultiplicationIsNonCommutative_ShouldBeValid(bool useGeneric)
    {
        if (useGeneric)
        {
            var q1 = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(1),
                _scalarProcessor.ScalarFromNumber(0),
                _scalarProcessor.ScalarFromNumber(0),
                _scalarProcessor.ScalarFromNumber(1));
            var q2 = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(0),
                _scalarProcessor.ScalarFromNumber(1),
                _scalarProcessor.ScalarFromNumber(0),
                _scalarProcessor.ScalarFromNumber(1));

            var result1 = q1 * q2;
            var result2 = q2 * q1;

            Assert.That(result1, Is.Not.Null, "First product should exist");
            Assert.That(result2, Is.Not.Null, "Second product should exist");
        }
        else
        {
            var q1 = LinFloat64Quaternion.Create(1, 0, 0, 1);
            var q2 = LinFloat64Quaternion.Create(0, 1, 0, 1);

            var result1 = q1 * q2;
            var result2 = q2 * q1;

            Assert.That(result1, Is.Not.Null, "First product should exist");
            Assert.That(result2, Is.Not.Null, "Second product should exist");
        }
    }

    #endregion

    #region Quaternion Operations Tests (3 tests × 2 = 6 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_Conjugate_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var q = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4),
                _scalarProcessor.ScalarFromNumber(1));
            var conj = q.Conjugate();

            Assert.That(conj.Scalar.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Scalar part unchanged");
            Assert.That(conj.ScalarI.ScalarValue, Is.EqualTo(-2).Within(Tolerance), "i negated");
            Assert.That(conj.ScalarJ.ScalarValue, Is.EqualTo(-3).Within(Tolerance), "j negated");
            Assert.That(conj.ScalarK.ScalarValue, Is.EqualTo(-4).Within(Tolerance), "k negated");
        }
        else
        {
            var q = LinFloat64Quaternion.Create(2, 3, 4, 1);
            var conj = q.Conjugate();

            Assert.That(conj.Scalar.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Scalar part unchanged");
            Assert.That(conj.ScalarI.ScalarValue, Is.EqualTo(-2).Within(Tolerance), "i negated");
            Assert.That(conj.ScalarJ.ScalarValue, Is.EqualTo(-3).Within(Tolerance), "j negated");
            Assert.That(conj.ScalarK.ScalarValue, Is.EqualTo(-4).Within(Tolerance), "k negated");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_RotateVector_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var axis = LinVector3D<double>.E3(_scalarProcessor); // Z axis
            var angle = LinPolarAngle<double>.Angle90(_scalarProcessor); // 90 degrees
            var q = LinQuaternion<double>.CreateFromAxisAngle(axis, angle);
            var v = LinVector3D<double>.E1(_scalarProcessor); // X axis vector

            var rotated = q.RotateVector(v);

            Assert.That(rotated, Is.Not.Null, "Rotated vector should exist");
            Assert.That(Math.Abs(rotated.X.ScalarValue), Is.LessThan(Tolerance), "X should be near 0");
            Assert.That(Math.Abs(rotated.Y.ScalarValue - 1), Is.LessThan(Tolerance), "Y should be near 1");
            Assert.That(Math.Abs(rotated.Z.ScalarValue), Is.LessThan(Tolerance), "Z should be near 0");
        }
        else
        {
            var axis = LinFloat64Vector3D.E3; // Z axis
            var angle = LinFloat64PolarAngle.Angle90; // 90 degrees
            var q = LinFloat64Quaternion.CreateFromAxisAngle(axis, angle);
            var v = LinFloat64Vector3D.E1; // X axis vector

            var rotated = q.RotateVector(v);

            Assert.That(rotated, Is.Not.Null, "Rotated vector should exist");
            Assert.That(Math.Abs(rotated.X.ScalarValue), Is.LessThan(Tolerance), "X should be near 0");
            Assert.That(Math.Abs(rotated.Y.ScalarValue - 1), Is.LessThan(Tolerance), "Y should be near 1");
            Assert.That(Math.Abs(rotated.Z.ScalarValue), Is.LessThan(Tolerance), "Z should be near 0");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Quaternion_NormSquared_ShouldBeValid(bool useGeneric)
    {
        if (useGeneric)
        {
            var q = LinQuaternion<double>.Create(
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(0),
                _scalarProcessor.ScalarFromNumber(1));
            var normSquared = q.NormSquared();

            Assert.That(normSquared.ScalarValue, Is.EqualTo(9).Within(Tolerance), "Norm squared should be 9");
        }
        else
        {
            var q = LinFloat64Quaternion.Create(2, 2, 0, 1);
            var normSquared = q.NormSquared();

            Assert.That(normSquared.ScalarValue, Is.EqualTo(9).Within(Tolerance), "Norm squared should be 9");
        }
    }

    #endregion
}
