using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.Euclidean;

/// <summary>
/// Tests for Euclidean Bivector Operations
/// EXTENDED: Tests both Float64 AND Generic&lt;double&gt; implementations
/// Phase 3B - Core Modeling: Euclidean Geometry Bivector Operations
/// Tests bivector construction, operations, and properties
/// </summary>
[TestFixture]
public class LinBivectorTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Bivector2D Tests (4 tests × 2 = 8 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector2D_Construction_ShouldWork(bool useGeneric)
    {
        // Arrange & Act
        var b = useGeneric
            ? (object)LinBivector2D<double>.Create(_scalarProcessor.ScalarFromNumber(5))
            : LinFloat64Bivector2D.Create(5);

        // Assert
        Assert.That(b, Is.Not.Null, "Bivector should be created");

        var xy = b switch
        {
            LinFloat64Bivector2D f64 => f64.Xy.ScalarValue,
            LinBivector2D<double> gen => gen.Xy.ScalarValue,
            _ => throw new ArgumentException()
        };

        Assert.That(xy, Is.EqualTo(5), "Xy component should match");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector2D_Addition_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var b1 = LinBivector2D<double>.Create(_scalarProcessor.ScalarFromNumber(2));
            var b2 = LinBivector2D<double>.Create(_scalarProcessor.ScalarFromNumber(3));
            var result = b1 + b2;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(5).Within(Tolerance), "Xy should be 2+3=5");
        }
        else
        {
            var b1 = LinFloat64Bivector2D.Create(2);
            var b2 = LinFloat64Bivector2D.Create(3);
            var result = b1 + b2;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(5).Within(Tolerance), "Xy should be 2+3=5");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector2D_ScalarMultiplication_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var b = LinBivector2D<double>.Create(_scalarProcessor.ScalarFromNumber(4));
            var result = b * 2.5;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(10).Within(Tolerance), "Xy should be 4*2.5=10");
        }
        else
        {
            var b = LinFloat64Bivector2D.Create(4);
            var result = b * 2.5;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(10).Within(Tolerance), "Xy should be 4*2.5=10");
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector2D_Negation_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var b = LinBivector2D<double>.Create(_scalarProcessor.ScalarFromNumber(3));
            var result = -b;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(-3).Within(Tolerance), "Negation should flip sign");
        }
        else
        {
            var b = LinFloat64Bivector2D.Create(3);
            var result = -b;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(-3).Within(Tolerance), "Negation should flip sign");
        }
    }

    #endregion

    #region Bivector3D Tests (6 tests × 2 = 12 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_Construction_ShouldWork(bool useGeneric)
    {
        // Arrange & Act
        var b = useGeneric
            ? (object)LinBivector3D<double>.Create(
                _scalarProcessor.ScalarFromNumber(1),
                _scalarProcessor.ScalarFromNumber(2),
                _scalarProcessor.ScalarFromNumber(3))
            : LinFloat64Bivector3D.Create(1, 2, 3);

        // Assert
        Assert.That(b, Is.Not.Null, "Bivector should be created");

        var (xy, xz, yz) = b switch
        {
            LinFloat64Bivector3D f64 => (f64.Xy.ScalarValue, f64.Xz.ScalarValue, f64.Yz.ScalarValue),
            LinBivector3D<double> gen => (gen.Xy.ScalarValue, gen.Xz.ScalarValue, gen.Yz.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(xy, Is.EqualTo(1), "Xy component should match");
        Assert.That(xz, Is.EqualTo(2), "Xz component should match");
        Assert.That(yz, Is.EqualTo(3), "Yz component should match");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_BasisBivectors_ShouldBeCorrect(bool useGeneric)
    {
        // Arrange & Act
        object e12, e13, e23;
        if (useGeneric)
        {
            e12 = LinBivector3D<double>.E12(_scalarProcessor);
            e13 = LinBivector3D<double>.E13(_scalarProcessor);
            e23 = LinBivector3D<double>.E23(_scalarProcessor);
        }
        else
        {
            e12 = LinFloat64Bivector3D.E12;
            e13 = LinFloat64Bivector3D.E13;
            e23 = LinFloat64Bivector3D.E23;
        }

        // Assert E12
        var (e12xy, e12xz, e12yz) = e12 switch
        {
            LinFloat64Bivector3D f => (f.Xy.ScalarValue, f.Xz.ScalarValue, f.Yz.ScalarValue),
            LinBivector3D<double> g => (g.Xy.ScalarValue, g.Xz.ScalarValue, g.Yz.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(e12xy, Is.EqualTo(1), "E12.Xy should be 1");
        Assert.That(e12xz, Is.EqualTo(0), "E12.Xz should be 0");
        Assert.That(e12yz, Is.EqualTo(0), "E12.Yz should be 0");

        // Assert E13
        var (e13xy, e13xz, e13yz) = e13 switch
        {
            LinFloat64Bivector3D f => (f.Xy.ScalarValue, f.Xz.ScalarValue, f.Yz.ScalarValue),
            LinBivector3D<double> g => (g.Xy.ScalarValue, g.Xz.ScalarValue, g.Yz.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(e13xy, Is.EqualTo(0), "E13.Xy should be 0");
        Assert.That(e13xz, Is.EqualTo(1), "E13.Xz should be 1");
        Assert.That(e13yz, Is.EqualTo(0), "E13.Yz should be 0");

        // Assert E23
        var (e23xy, e23xz, e23yz) = e23 switch
        {
            LinFloat64Bivector3D f => (f.Xy.ScalarValue, f.Xz.ScalarValue, f.Yz.ScalarValue),
            LinBivector3D<double> g => (g.Xy.ScalarValue, g.Xz.ScalarValue, g.Yz.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(e23xy, Is.EqualTo(0), "E23.Xy should be 0");
        Assert.That(e23xz, Is.EqualTo(0), "E23.Xz should be 0");
        Assert.That(e23yz, Is.EqualTo(1), "E23.Yz should be 1");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_Addition_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var b1 = LinBivector3D<double>.Create(_scalarProcessor.ScalarFromNumber(1), _scalarProcessor.ScalarFromNumber(2), _scalarProcessor.ScalarFromNumber(3));
            var b2 = LinBivector3D<double>.Create(_scalarProcessor.ScalarFromNumber(4), _scalarProcessor.ScalarFromNumber(5), _scalarProcessor.ScalarFromNumber(6));
            var result = b1 + b2;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(5).Within(Tolerance));
            Assert.That(result.Xz.ScalarValue, Is.EqualTo(7).Within(Tolerance));
            Assert.That(result.Yz.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        }
        else
        {
            var b1 = LinFloat64Bivector3D.Create(1, 2, 3);
            var b2 = LinFloat64Bivector3D.Create(4, 5, 6);
            var result = b1 + b2;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(5).Within(Tolerance));
            Assert.That(result.Xz.ScalarValue, Is.EqualTo(7).Within(Tolerance));
            Assert.That(result.Yz.ScalarValue, Is.EqualTo(9).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_Subtraction_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var b1 = LinBivector3D<double>.Create(_scalarProcessor.ScalarFromNumber(10), _scalarProcessor.ScalarFromNumber(8), _scalarProcessor.ScalarFromNumber(6));
            var b2 = LinBivector3D<double>.Create(_scalarProcessor.ScalarFromNumber(1), _scalarProcessor.ScalarFromNumber(2), _scalarProcessor.ScalarFromNumber(3));
            var result = b1 - b2;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(9).Within(Tolerance));
            Assert.That(result.Xz.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Yz.ScalarValue, Is.EqualTo(3).Within(Tolerance));
        }
        else
        {
            var b1 = LinFloat64Bivector3D.Create(10, 8, 6);
            var b2 = LinFloat64Bivector3D.Create(1, 2, 3);
            var result = b1 - b2;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(9).Within(Tolerance));
            Assert.That(result.Xz.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Yz.ScalarValue, Is.EqualTo(3).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_ScalarMultiplication_ShouldWork(bool useGeneric)
    {
        if (useGeneric)
        {
            var b = LinBivector3D<double>.Create(_scalarProcessor.ScalarFromNumber(2), _scalarProcessor.ScalarFromNumber(3), _scalarProcessor.ScalarFromNumber(4));
            var result = b * 3.0;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Xz.ScalarValue, Is.EqualTo(9).Within(Tolerance));
            Assert.That(result.Yz.ScalarValue, Is.EqualTo(12).Within(Tolerance));
        }
        else
        {
            var b = LinFloat64Bivector3D.Create(2, 3, 4);
            var result = b * 3;

            Assert.That(result.Xy.ScalarValue, Is.EqualTo(6).Within(Tolerance));
            Assert.That(result.Xz.ScalarValue, Is.EqualTo(9).Within(Tolerance));
            Assert.That(result.Yz.ScalarValue, Is.EqualTo(12).Within(Tolerance));
        }
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void Bivector3D_Zero_ShouldBeZeroBivector(bool useGeneric)
    {
        // Arrange & Act
        var zero = useGeneric
            ? (object)LinBivector3D<double>.Zero(_scalarProcessor)
            : LinFloat64Bivector3D.Zero;

        // Assert
        var (xy, xz, yz) = zero switch
        {
            LinFloat64Bivector3D f => (f.Xy.ScalarValue, f.Xz.ScalarValue, f.Yz.ScalarValue),
            LinBivector3D<double> g => (g.Xy.ScalarValue, g.Xz.ScalarValue, g.Yz.ScalarValue),
            _ => throw new ArgumentException()
        };

        Assert.That(xy, Is.EqualTo(0), "Zero.Xy should be 0");
        Assert.That(xz, Is.EqualTo(0), "Zero.Xz should be 0");
        Assert.That(yz, Is.EqualTo(0), "Zero.Yz should be 0");
    }

    #endregion
}
