using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float32;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float32;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.PGa.Float32;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Scalars;

/// <summary>
/// Smoke tests for Float32 wrapper classes to verify basic API functionality.
/// Full coverage is provided by generic tests and performance benchmarks.
/// </summary>
[TestFixture]
public class Float32SmokeTests
{
    [Test]
    public void XGaFloat32Processor_Euclidean_ShouldNotBeNull()
    {
        // Arrange & Act
        var processor = XGaFloat32Processor.Euclidean;

        // Assert
        Assert.That(processor, Is.Not.Null);
        Assert.That(processor.ScalarProcessor, Is.Not.Null);
    }

    [Test]
    public void XGaFloat32Processor_Conformal_ShouldNotBeNull()
    {
        // Arrange & Act
        var processor = XGaFloat32Processor.Conformal;

        // Assert
        Assert.That(processor, Is.Not.Null);
        Assert.That(processor.ScalarProcessor, Is.Not.Null);
    }

    [Test]
    public void XGaFloat32Processor_Projective_ShouldNotBeNull()
    {
        // Arrange & Act
        var processor = XGaFloat32Processor.Projective;

        // Assert
        Assert.That(processor, Is.Not.Null);
        Assert.That(processor.ScalarProcessor, Is.Not.Null);
    }

    [Test]
    public void XGaFloat32Processor_Create_ShouldWorkWithCustomMetric()
    {
        // Arrange & Act
        var processor = XGaFloat32Processor.Create(negativeCount: 1, zeroCount: 0);

        // Assert
        Assert.That(processor, Is.Not.Null);
        Assert.That(processor.NegativeSignatureBasisCount, Is.EqualTo(1));
        Assert.That(processor.ZeroSignatureBasisCount, Is.EqualTo(0));
    }

    [Test]
    public void CGaFloat32GeometricSpace_Space4D_ShouldNotBeNull()
    {
        // Arrange & Act
        var space = CGaFloat32GeometricSpace.Space4D;

        // Assert
        Assert.That(space, Is.Not.Null);
        Assert.That(space.Is4D, Is.True);
        Assert.That(space.ScalarProcessor, Is.Not.Null);
    }

    [Test]
    public void CGaFloat32GeometricSpace_Space5D_ShouldNotBeNull()
    {
        // Arrange & Act
        var space = CGaFloat32GeometricSpace.Space5D;

        // Assert
        Assert.That(space, Is.Not.Null);
        Assert.That(space.Is5D, Is.True);
        Assert.That(space.ScalarProcessor, Is.Not.Null);
    }

    [Test]
    public void CGaFloat32GeometricSpace_Create_ShouldWorkWithCustomDimensions()
    {
        // Arrange & Act
        var space = CGaFloat32GeometricSpace.Create(vSpaceDimensions: 6);

        // Assert
        Assert.That(space, Is.Not.Null);
        Assert.That(space.VSpaceDimensions, Is.EqualTo(6));
    }

    [Test]
    public void XGaFloat32Processor_BasicVectorOperation_ShouldWork()
    {
        // Arrange
        var processor = XGaFloat32Processor.Euclidean;

        // Act
        var v1 = processor.CreateVectorComposer()
            .SetVectorTerm(0, 1f)
            .SetVectorTerm(1, 2f)
            .SetVectorTerm(2, 3f)
            .GetVector();

        var v2 = processor.CreateVectorComposer()
            .SetVectorTerm(0, 4f)
            .SetVectorTerm(1, 5f)
            .SetVectorTerm(2, 6f)
            .GetVector();

        var result = v1.Op(v2); // Outer product

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Grade, Is.EqualTo(2)); // Bivector
    }

    [Test]
    public void CGaFloat32GeometricSpace_EncodePoint_ShouldWork()
    {
        // Arrange
        var cga = CGaFloat32GeometricSpace.Space5D;

        // Act
        var point = cga.EncodeIpnsRound.Point(1f, 2f, 3f);

        // Assert
        Assert.That(point, Is.Not.Null);
        Assert.That(point.GeometricSpace, Is.EqualTo(cga));
    }

    [Test]
    public void CGaFloat32GeometricSpace_HybridAPI_T_Overload_ShouldWork()
    {
        // Arrange
        var cga = CGaFloat32GeometricSpace.Space4D;

        // Act
        var vector = cga.EncodeVGa.Vector(2f, 3f);

        // Assert
        Assert.That(vector, Is.Not.Null);
    }

    [Test]
    public void CGaFloat32GeometricSpace_HybridAPI_Double_Overload_ShouldWork()
    {
        // Arrange
        var cga = CGaFloat32GeometricSpace.Space4D;

        // Act
        var vector = cga.EncodeVGa.Vector(2.0, 3.0);

        // Assert
        Assert.That(vector, Is.Not.Null);
    }

    [Test]
    public void PGaFloat32GeometricSpace_Space4D_ShouldNotBeNull()
    {
        // Arrange & Act
        var space = PGaFloat32GeometricSpace.Space4D;

        // Assert
        Assert.That(space, Is.Not.Null);
        Assert.That(space.Is3D, Is.True);
        Assert.That(space.ScalarProcessor, Is.Not.Null);
    }

    [Test]
    public void PGaFloat32GeometricSpace_Space5D_ShouldNotBeNull()
    {
        // Arrange & Act
        var space = PGaFloat32GeometricSpace.Space5D;

        // Assert
        Assert.That(space, Is.Not.Null);
        Assert.That(space.Is4D, Is.True);
        Assert.That(space.ScalarProcessor, Is.Not.Null);
    }

    [Test]
    public void PGaFloat32GeometricSpace_Create_ShouldWorkWithCustomDimensions()
    {
        // Arrange & Act
        var space = PGaFloat32GeometricSpace.Create(vSpaceDimensions: 5);

        // Assert
        Assert.That(space, Is.Not.Null);
        Assert.That(space.VSpaceDimensions, Is.EqualTo(5));
    }
}
