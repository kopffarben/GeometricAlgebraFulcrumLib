using System;
using System.Numerics;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for VGA (Vector Geometric Algebra) equivalence - Module 3 of deduplication roadmap.
/// Tests ensure Generic&lt;double&gt; VGA produces equivalent results to Float64 VGA.
/// </summary>
[TestFixture]
public class VGaEquivalenceTests
{
    private IScalarProcessor<double> _scalarProcessor = null!;
    private const double Tolerance = 1e-12;

    [SetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    /// <summary>
    /// Helper method to assert two vectors are equivalent within tolerance.
    /// </summary>
    private void AssertVectorEquivalent(double expectedX, double expectedY, double actualX, double actualY, string message)
    {
        Assert.That(actualX, Is.EqualTo(expectedX).Within(Tolerance), $"{message}: X component should match");
        Assert.That(actualY, Is.EqualTo(expectedY).Within(Tolerance), $"{message}: Y component should match");
    }

    /// <summary>
    /// Helper method to assert two 3D vectors are equivalent within tolerance.
    /// </summary>
    private void AssertVector3DEquivalent(double expectedX, double expectedY, double expectedZ,
                                          double actualX, double actualY, double actualZ, string message)
    {
        Assert.That(actualX, Is.EqualTo(expectedX).Within(Tolerance), $"{message}: X component should match");
        Assert.That(actualY, Is.EqualTo(expectedY).Within(Tolerance), $"{message}: Y component should match");
        Assert.That(actualZ, Is.EqualTo(expectedZ).Within(Tolerance), $"{message}: Z component should match");
    }

    [Test]
    public void VGa2D_EncodeVector_ShouldProduceEquivalentResults()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace2D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace2D<double>(_scalarProcessor);

        // Act
        var vectorF64 = vgaFloat64.EncodeVector(3.0, 4.0);
        var vectorGen = vgaGeneric.EncodeVector(3.0, 4.0);

        // Assert
        AssertVectorEquivalent(
            vectorF64[0], vectorF64[1],
            vectorGen[0].ScalarValue, vectorGen[1].ScalarValue,
            "2D Vector encoding"
        );
    }

    [Test]
    public void VGa2D_EncodeBivector_ShouldProduceEquivalentResults()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace2D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace2D<double>(_scalarProcessor);

        // Act
        var bivectorF64 = vgaFloat64.EncodeBivector(2.5);
        var bivectorGen = vgaGeneric.EncodeBivector(2.5);

        // Assert
        Assert.That(bivectorGen[0, 1].ScalarValue, Is.EqualTo(bivectorF64[0, 1]).Within(Tolerance),
            "2D Bivector encoding should match");
    }

    [Test]
    public void VGa2D_EncodeDecodeComplex_ShouldPreserveValues()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace2D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace2D<double>(_scalarProcessor);
        var originalComplex = new Complex(3.0, 4.0);

        // Act
        var mvF64 = vgaFloat64.EncodeComplex(originalComplex.Real, originalComplex.Imaginary);
        var mvGen = vgaGeneric.EncodeComplex(originalComplex.Real, originalComplex.Imaginary);

        var decodedF64 = vgaFloat64.DecodeComplex(mvF64);
        var decodedGen = vgaGeneric.DecodeComplex(mvGen);

        // Assert
        Assert.That(decodedGen.RealValue, Is.EqualTo(decodedF64.Real).Within(Tolerance),
            "Complex real part should match");
        Assert.That(decodedGen.ImaginaryValue, Is.EqualTo(decodedF64.Imaginary).Within(Tolerance),
            "Complex imaginary part should match");
    }

    [Test]
    public void VGa2D_BasisVectors_ShouldMatch()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace2D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace2D<double>(_scalarProcessor);

        // Assert E1
        AssertVectorEquivalent(
            vgaFloat64.E1[0], vgaFloat64.E1[1],
            vgaGeneric.E1[0].ScalarValue, vgaGeneric.E1[1].ScalarValue,
            "E1 basis vector"
        );

        // Assert E2
        AssertVectorEquivalent(
            vgaFloat64.E2[0], vgaFloat64.E2[1],
            vgaGeneric.E2[0].ScalarValue, vgaGeneric.E2[1].ScalarValue,
            "E2 basis vector"
        );

        // Assert E12
        Assert.That(vgaGeneric.E12[0, 1].ScalarValue, Is.EqualTo(vgaFloat64.E12[0, 1]).Within(Tolerance),
            "E12 basis bivector should match");
    }

    [Test]
    public void VGa3D_EncodeVector_ShouldProduceEquivalentResults()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace3D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace3D<double>(_scalarProcessor);

        // Act
        var vectorF64 = vgaFloat64.EncodeVector(3.0, 4.0, 5.0);
        var vectorGen = vgaGeneric.EncodeVector(3.0, 4.0, 5.0);

        // Assert
        AssertVector3DEquivalent(
            vectorF64[0], vectorF64[1], vectorF64[2],
            vectorGen[0].ScalarValue, vectorGen[1].ScalarValue, vectorGen[2].ScalarValue,
            "3D Vector encoding"
        );
    }

    [Test]
    public void VGa3D_EncodeBivector_ShouldProduceEquivalentResults()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace3D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace3D<double>(_scalarProcessor);

        // Act
        var bivectorF64 = vgaFloat64.EncodeBivector(1.0, 2.0, 3.0);
        var bivectorGen = vgaGeneric.EncodeBivector(1.0, 2.0, 3.0);

        // Assert
        Assert.That(bivectorGen[0, 1].ScalarValue, Is.EqualTo(bivectorF64[0, 1]).Within(Tolerance),
            "XY bivector component should match");
        Assert.That(bivectorGen[0, 2].ScalarValue, Is.EqualTo(bivectorF64[0, 2]).Within(Tolerance),
            "XZ bivector component should match");
        Assert.That(bivectorGen[1, 2].ScalarValue, Is.EqualTo(bivectorF64[1, 2]).Within(Tolerance),
            "YZ bivector component should match");
    }

    [Test]
    public void VGa3D_EncodeDecodeQuaternion_ShouldPreserveValues()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace3D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace3D<double>(_scalarProcessor);
        double scalar = 1.0, i = 0.5, j = 0.3, k = 0.2;

        // Act
        var mvF64 = vgaFloat64.EncodeQuaternion(scalar, i, j, k);
        var mvGen = vgaGeneric.EncodeQuaternion(scalar, i, j, k);

        var decodedF64 = vgaFloat64.DecodeQuaternion(mvF64);
        var decodedGen = vgaGeneric.DecodeQuaternion(mvGen);

        // Assert
        Assert.That(decodedGen.Scalar.ScalarValue, Is.EqualTo(decodedF64.Scalar.ScalarValue).Within(Tolerance),
            "Quaternion scalar part should match");
        Assert.That(decodedGen.ScalarI.ScalarValue, Is.EqualTo(decodedF64.ScalarI.ScalarValue).Within(Tolerance),
            "Quaternion i component should match");
        Assert.That(decodedGen.ScalarJ.ScalarValue, Is.EqualTo(decodedF64.ScalarJ.ScalarValue).Within(Tolerance),
            "Quaternion j component should match");
        Assert.That(decodedGen.ScalarK.ScalarValue, Is.EqualTo(decodedF64.ScalarK.ScalarValue).Within(Tolerance),
            "Quaternion k component should match");
    }

    [Test]
    public void VGa3D_BasisVectors_ShouldMatch()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace3D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace3D<double>(_scalarProcessor);

        // Assert E1
        AssertVector3DEquivalent(
            vgaFloat64.E1[0], vgaFloat64.E1[1], vgaFloat64.E1[2],
            vgaGeneric.E1[0].ScalarValue, vgaGeneric.E1[1].ScalarValue, vgaGeneric.E1[2].ScalarValue,
            "E1 basis vector"
        );

        // Assert E2
        AssertVector3DEquivalent(
            vgaFloat64.E2[0], vgaFloat64.E2[1], vgaFloat64.E2[2],
            vgaGeneric.E2[0].ScalarValue, vgaGeneric.E2[1].ScalarValue, vgaGeneric.E2[2].ScalarValue,
            "E2 basis vector"
        );

        // Assert E3
        AssertVector3DEquivalent(
            vgaFloat64.E3[0], vgaFloat64.E3[1], vgaFloat64.E3[2],
            vgaGeneric.E3[0].ScalarValue, vgaGeneric.E3[1].ScalarValue, vgaGeneric.E3[2].ScalarValue,
            "E3 basis vector"
        );
    }

    [Test]
    public void VGa3D_BasisBivectors_ShouldMatch()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace3D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace3D<double>(_scalarProcessor);

        // Assert E12
        Assert.That(vgaGeneric.E12[0, 1].ScalarValue, Is.EqualTo(vgaFloat64.E12[0, 1]).Within(Tolerance),
            "E12 basis bivector should match");

        // Assert E13
        Assert.That(vgaGeneric.E13[0, 2].ScalarValue, Is.EqualTo(vgaFloat64.E13[0, 2]).Within(Tolerance),
            "E13 basis bivector should match");

        // Assert E23
        Assert.That(vgaGeneric.E23[1, 2].ScalarValue, Is.EqualTo(vgaFloat64.E23[1, 2]).Within(Tolerance),
            "E23 basis bivector should match");
    }

    [Test]
    public void VGa2D_Pseudoscalar_ShouldMatch()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace2D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace2D<double>(_scalarProcessor);

        // Assert I (pseudoscalar)
        Assert.That(vgaGeneric.I[0, 1].ScalarValue, Is.EqualTo(vgaFloat64.I[0, 1]).Within(Tolerance),
            "2D Pseudoscalar I should match");

        // Assert Iinv
        Assert.That(vgaGeneric.Iinv[0, 1].ScalarValue, Is.EqualTo(vgaFloat64.Iinv[0, 1]).Within(Tolerance),
            "2D Inverse pseudoscalar Iinv should match");

        // Assert Irev
        Assert.That(vgaGeneric.Irev[0, 1].ScalarValue, Is.EqualTo(vgaFloat64.Irev[0, 1]).Within(Tolerance),
            "2D Reverse pseudoscalar Irev should match");
    }

    [Test]
    public void VGa3D_Pseudoscalar_ShouldMatch()
    {
        // Arrange
        var vgaFloat64 = XGaEuclideanGeometrySpace3D.Instance;
        var vgaGeneric = new XGaEuclideanGeometrySpace3D<double>(_scalarProcessor);

        // Assert I (pseudoscalar)
        Assert.That(vgaGeneric.I[0, 1, 2].ScalarValue, Is.EqualTo(vgaFloat64.I[0, 1, 2]).Within(Tolerance),
            "3D Pseudoscalar I should match");

        // Assert Iinv
        Assert.That(vgaGeneric.Iinv[0, 1, 2].ScalarValue, Is.EqualTo(vgaFloat64.Iinv[0, 1, 2]).Within(Tolerance),
            "3D Inverse pseudoscalar Iinv should match");

        // Assert Irev
        Assert.That(vgaGeneric.Irev[0, 1, 2].ScalarValue, Is.EqualTo(vgaFloat64.Irev[0, 1, 2]).Within(Tolerance),
            "3D Reverse pseudoscalar Irev should match");
    }
}
