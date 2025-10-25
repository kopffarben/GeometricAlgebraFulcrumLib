using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space4D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space4D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.Euclidean;

/// <summary>
/// Tests for Euclidean Vector Operations
/// EXTENDED: Tests both Float64 AND Generic&lt;double&gt; implementations
/// Phase 3B - Core Modeling: Euclidean Geometry Vector Operations
/// Tests vector construction, operations, and properties in 2D, 3D, and 4D
/// </summary>
[TestFixture]
public class LinVectorTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Vector2D Tests (5 tests × 2 = 10 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector2D_Construction_ShouldWork(bool useGeneric)
    {
        // Arrange & Act
        var v = useGeneric
            ? (object)LinVector2D<double>.Create(
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4))
            : LinFloat64Vector2D.Create(3, 4);

        // Assert
        Assert.That(v, Is.Not.Null, "Vector should be created");

        var x = v switch
        {
            LinFloat64Vector2D f64 => f64.X.ScalarValue,
            LinVector2D<double> gen => gen.X.ScalarValue,
            _ => throw new ArgumentException()
        };
        var y = v switch
        {
            LinFloat64Vector2D f64 => f64.Y.ScalarValue,
            LinVector2D<double> gen => gen.Y.ScalarValue,
            _ => throw new ArgumentException()
        };

        Assert.That(x, Is.EqualTo(3), "X component should match");
        Assert.That(y, Is.EqualTo(4), "Y component should match");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector2D_Addition_ShouldWork(bool useGeneric)
    {
        // Arrange
        if (useGeneric)
        {
            var v1 = LinVector2D<double>.Create(_scalarProcessor.ScalarFromNumber(1), _scalarProcessor.ScalarFromNumber(2));
            var v2 = LinVector2D<double>.Create(_scalarProcessor.ScalarFromNumber(3), _scalarProcessor.ScalarFromNumber(4));

            // Act
            var result = v1 + v2;

            // Assert
            Assert.That(result.X.ScalarValue, Is.EqualTo(4).Within(Tolerance), "X should be 1+3=4");
            Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance), "Y should be 2+4=6");
        }
        else
        {
            var v1 = LinFloat64Vector2D.Create(1, 2);
            var v2 = LinFloat64Vector2D.Create(3, 4);

            // Act
            var result = v1 + v2;

            // Assert
            Assert.That(result.X.ScalarValue, Is.EqualTo(4).Within(Tolerance), "X should be 1+3=4");
            Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance), "Y should be 2+4=6");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector2D_Subtraction_ShouldWork(bool useGeneric)
    {
        // Arrange & Act & Assert
        if (useGeneric)
        {
            var v1 = LinVector2D<double>.Create(_scalarProcessor.ScalarFromNumber(5), _scalarProcessor.ScalarFromNumber(7));
            var v2 = LinVector2D<double>.Create(_scalarProcessor.ScalarFromNumber(2), _scalarProcessor.ScalarFromNumber(3));
            var result = v1 - v2;

            Assert.That(result.X.ScalarValue, Is.EqualTo(3).Within(Tolerance), "X should be 5-2=3");
            Assert.That(result.Y.ScalarValue, Is.EqualTo(4).Within(Tolerance), "Y should be 7-3=4");
        }
        else
        {
            var v1 = LinFloat64Vector2D.Create(5, 7);
            var v2 = LinFloat64Vector2D.Create(2, 3);
            var result = v1 - v2;

            Assert.That(result.X.ScalarValue, Is.EqualTo(3).Within(Tolerance), "X should be 5-2=3");
            Assert.That(result.Y.ScalarValue, Is.EqualTo(4).Within(Tolerance), "Y should be 7-3=4");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector2D_ScalarMultiplication_ShouldWork(bool useGeneric)
    {
        // Arrange & Act & Assert
        if (useGeneric)
        {
            var v = LinVector2D<double>.Create(_scalarProcessor.ScalarFromNumber(2), _scalarProcessor.ScalarFromNumber(3));
            var result = v * 2.5;

            Assert.That(result.X.ScalarValue, Is.EqualTo(5).Within(Tolerance), "X should be 2*2.5=5");
            Assert.That(result.Y.ScalarValue, Is.EqualTo(7.5).Within(Tolerance), "Y should be 3*2.5=7.5");
        }
        else
        {
            var v = LinFloat64Vector2D.Create(2, 3);
            var result = v * 2.5;

            Assert.That(result.X.ScalarValue, Is.EqualTo(5).Within(Tolerance), "X should be 2*2.5=5");
            Assert.That(result.Y.ScalarValue, Is.EqualTo(7.5).Within(Tolerance), "Y should be 3*2.5=7.5");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector2D_Negation_ShouldWork(bool useGeneric)
    {
        // Arrange & Act & Assert
        if (useGeneric)
        {
            var v = LinVector2D<double>.Create(_scalarProcessor.ScalarFromNumber(3), _scalarProcessor.ScalarFromNumber(-4));
            var result = -v;

            Assert.That(result.X.ScalarValue, Is.EqualTo(-3).Within(Tolerance), "X should be negated");
            Assert.That(result.Y.ScalarValue, Is.EqualTo(4).Within(Tolerance), "Y should be negated");
        }
        else
        {
            var v = LinFloat64Vector2D.Create(3, -4);
            var result = -v;

            Assert.That(result.X.ScalarValue, Is.EqualTo(-3).Within(Tolerance), "X should be negated");
            Assert.That(result.Y.ScalarValue, Is.EqualTo(4).Within(Tolerance), "Y should be negated");
        }
    }

    #endregion

    #region Vector3D Tests (8 tests × 2 = 16 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector3D_Construction_ShouldWork(bool useGeneric)
    {
        // Arrange & Act
        var v = useGeneric
            ? (object)LinVector3D<double>.Create(
                _scalarProcessor.ScalarFromNumber(1),
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3))
            : LinFloat64Vector3D.Create(1, 2, 3);

        // Assert
        Assert.That(v, Is.Not.Null, "Vector should be created");

        var (x, y, z) = v switch
        {
            LinFloat64Vector3D f64 => (f64.X.ScalarValue, f64.Y.ScalarValue, f64.Z.ScalarValue),
            LinVector3D<double> gen => (gen.X.ScalarValue, gen.Y.ScalarValue, gen.Z.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(x, Is.EqualTo(1), "X component should match");
        Assert.That(y, Is.EqualTo(2), "Y component should match");
        Assert.That(z, Is.EqualTo(3), "Z component should match");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector3D_BasisVectors_ShouldBeCorrect(bool useGeneric)
    {
        // Arrange & Act
        object e1, e2, e3;
        if (useGeneric)
        {
            e1 = LinVector3D<double>.E1(_scalarProcessor);
            e2 = LinVector3D<double>.E2(_scalarProcessor);
            e3 = LinVector3D<double>.E3(_scalarProcessor);
        }
        else
        {
            e1 = LinFloat64Vector3D.E1;
            e2 = LinFloat64Vector3D.E2;
            e3 = LinFloat64Vector3D.E3;
        }

        // Assert
        var (e1x, e1y, e1z) = e1 switch
        {
            LinFloat64Vector3D f => (f.X.ScalarValue, f.Y.ScalarValue, f.Z.ScalarValue),
            LinVector3D<double> g => (g.X.ScalarValue, g.Y.ScalarValue, g.Z.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(e1x, Is.EqualTo(1), "E1.X should be 1");
        Assert.That(e1y, Is.EqualTo(0), "E1.Y should be 0");
        Assert.That(e1z, Is.EqualTo(0), "E1.Z should be 0");

        var (e2x, e2y, e2z) = e2 switch
        {
            LinFloat64Vector3D f => (f.X.ScalarValue, f.Y.ScalarValue, f.Z.ScalarValue),
            LinVector3D<double> g => (g.X.ScalarValue, g.Y.ScalarValue, g.Z.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(e2x, Is.EqualTo(0), "E2.X should be 0");
        Assert.That(e2y, Is.EqualTo(1), "E2.Y should be 1");
        Assert.That(e2z, Is.EqualTo(0), "E2.Z should be 0");

        var (e3x, e3y, e3z) = e3 switch
        {
            LinFloat64Vector3D f => (f.X.ScalarValue, f.Y.ScalarValue, f.Z.ScalarValue),
            LinVector3D<double> g => (g.X.ScalarValue, g.Y.ScalarValue, g.Z.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(e3x, Is.EqualTo(0), "E3.X should be 0");
        Assert.That(e3y, Is.EqualTo(0), "E3.Y should be 0");
        Assert.That(e3z, Is.EqualTo(1), "E3.Z should be 1");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector3D_Addition_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var v1 = LinVector3D<double>.Create(_scalarProcessor.ScalarFromNumber(1), _scalarProcessor.ScalarFromNumber(2), _scalarProcessor.ScalarFromNumber(3));
            var v2 = LinVector3D<double>.Create(_scalarProcessor.ScalarFromNumber(4), _scalarProcessor.ScalarFromNumber(5), _scalarProcessor.ScalarFromNumber(6));
            var result = v1 + v2;

            Assert.That(result.X.ScalarValue, Is.EqualTo(5).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(7).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        }
        else
        {
            var v1 = LinFloat64Vector3D.Create(1, 2, 3);
            var v2 = LinFloat64Vector3D.Create(4, 5, 6);
            var result = v1 + v2;

            Assert.That(result.X.ScalarValue, Is.EqualTo(5).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(7).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector3D_Subtraction_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var v1 = LinVector3D<double>.Create(_scalarProcessor.ScalarFromNumber(10), _scalarProcessor.ScalarFromNumber(8), _scalarProcessor.ScalarFromNumber(6));
            var v2 = LinVector3D<double>.Create(_scalarProcessor.ScalarFromNumber(1), _scalarProcessor.ScalarFromNumber(2), _scalarProcessor.ScalarFromNumber(3));
            var result = v1 - v2;

            Assert.That(result.X.ScalarValue, Is.EqualTo(9).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(3).Within(Tolerance));
        }
        else
        {
            var v1 = LinFloat64Vector3D.Create(10, 8, 6);
            var v2 = LinFloat64Vector3D.Create(1, 2, 3);
            var result = v1 - v2;

            Assert.That(result.X.ScalarValue, Is.EqualTo(9).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(3).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector3D_ScalarMultiplication_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var v = LinVector3D<double>.Create(_scalarProcessor.ScalarFromNumber(1), _scalarProcessor.ScalarFromNumber(2), _scalarProcessor.ScalarFromNumber(3));
            var result = v * 3.0;

            Assert.That(result.X.ScalarValue, Is.EqualTo(3).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        }
        else
        {
            var v = LinFloat64Vector3D.Create(1, 2, 3);
            var result = v * 3;

            Assert.That(result.X.ScalarValue, Is.EqualTo(3).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector3D_ScalarDivision_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var v = LinVector3D<double>.Create(_scalarProcessor.ScalarFromNumber(6), _scalarProcessor.ScalarFromNumber(9), _scalarProcessor.ScalarFromNumber(12));
            var result = v / 3.0;

            Assert.That(result.X.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(3).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(4).Within(Tolerance));
        }
        else
        {
            var v = LinFloat64Vector3D.Create(6, 9, 12);
            var result = v / 3;

            Assert.That(result.X.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(3).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(4).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector3D_Negation_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var v = LinVector3D<double>.Create(_scalarProcessor.ScalarFromNumber(1), _scalarProcessor.ScalarFromNumber(-2), _scalarProcessor.ScalarFromNumber(3));
            var result = -v;

            Assert.That(result.X.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
        }
        else
        {
            var v = LinFloat64Vector3D.Create(1, -2, 3);
            var result = -v;

            Assert.That(result.X.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector3D_Zero_ShouldBeZeroVector(bool useGeneric)
    {
        // Arrange & Act
        var zero = useGeneric
            ? (object)LinVector3D<double>.Zero(_scalarProcessor)
            : LinFloat64Vector3D.Zero;

        // Assert
        var (x, y, z) = zero switch
        {
            LinFloat64Vector3D f => (f.X.ScalarValue, f.Y.ScalarValue, f.Z.ScalarValue),
            LinVector3D<double> g => (g.X.ScalarValue, g.Y.ScalarValue, g.Z.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(x, Is.EqualTo(0), "Zero.X should be 0");
        Assert.That(y, Is.EqualTo(0), "Zero.Y should be 0");
        Assert.That(z, Is.EqualTo(0), "Zero.Z should be 0");
    }

    #endregion

    #region Vector4D Tests (4 tests × 2 = 8 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector4D_Construction_ShouldWork(bool useGeneric)
    {
        // Arrange & Act
        var v = useGeneric
            ? (object)LinVector4D<double>.Create(
                _scalarProcessor.ScalarFromNumber(1),
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4))
            : LinFloat64Vector4D.Create(1, 2, 3, 4);

        // Assert
        Assert.That(v, Is.Not.Null, "Vector should be created");

        var (x, y, z, w) = v switch
        {
            LinFloat64Vector4D f64 => (f64.X.ScalarValue, f64.Y.ScalarValue, f64.Z.ScalarValue, f64.W.ScalarValue),
            LinVector4D<double> gen => (gen.X.ScalarValue, gen.Y.ScalarValue, gen.Z.ScalarValue, gen.W.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(x, Is.EqualTo(1), "X component should match");
        Assert.That(y, Is.EqualTo(2), "Y component should match");
        Assert.That(z, Is.EqualTo(3), "Z component should match");
        Assert.That(w, Is.EqualTo(4), "W component should match");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector4D_Addition_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var v1 = LinVector4D<double>.Create(
                _scalarProcessor.ScalarFromNumber(1),
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4));
            var v2 = LinVector4D<double>.Create(
                _scalarProcessor.ScalarFromNumber(5),
                _scalarProcessor.ScalarFromNumber(6),
                _scalarProcessor.ScalarFromNumber(7),
                _scalarProcessor.ScalarFromNumber(8));
            var result = v1 + v2;

            Assert.That(result.X.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(8).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(10).Within(Tolerance));
            Assert.That(result.W.ScalarValue, Is.EqualTo(12).Within(Tolerance));
        }
        else
        {
            var v1 = LinFloat64Vector4D.Create(1, 2, 3, 4);
            var v2 = LinFloat64Vector4D.Create(5, 6, 7, 8);
            var result = v1 + v2;

            Assert.That(result.X.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(8).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(10).Within(Tolerance));
            Assert.That(result.W.ScalarValue, Is.EqualTo(12).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector4D_ScalarMultiplication_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var v = LinVector4D<double>.Create(
                _scalarProcessor.ScalarFromNumber(1),
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(4));
            var result = v * 2.0; // Note: Vector4D only supports * double, not * Scalar<T>

            Assert.That(result.X.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(4).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.W.ScalarValue, Is.EqualTo(8).Within(Tolerance));
        }
        else
        {
            var v = LinFloat64Vector4D.Create(1, 2, 3, 4);
            var result = v * 2;

            Assert.That(result.X.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(4).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.W.ScalarValue, Is.EqualTo(8).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Vector4D_Negation_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var v = LinVector4D<double>.Create(
                _scalarProcessor.ScalarFromNumber(1),
                _scalarProcessor.ScalarFromNumber(-2),
                _scalarProcessor.ScalarFromNumber(3),
                _scalarProcessor.ScalarFromNumber(-4));
            var result = -v;

            Assert.That(result.X.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
            Assert.That(result.W.ScalarValue, Is.EqualTo(4).Within(Tolerance));
        }
        else
        {
            var v = LinFloat64Vector4D.Create(1, -2, 3, -4);
            var result = -v;

            Assert.That(result.X.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
            Assert.That(result.Y.ScalarValue, Is.EqualTo(2).Within(Tolerance));
            Assert.That(result.Z.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
            Assert.That(result.W.ScalarValue, Is.EqualTo(4).Within(Tolerance));
        }
    }

    #endregion
}
