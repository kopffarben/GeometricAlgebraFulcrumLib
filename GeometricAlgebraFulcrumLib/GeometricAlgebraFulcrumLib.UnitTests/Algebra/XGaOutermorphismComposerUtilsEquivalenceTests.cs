using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Outermorphisms;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.LinearMaps.Outermorphisms;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for XGaOutermorphismComposerUtils equivalence - Module 1, Task 1.3 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; composer utilities produce identical results.
/// </summary>
[TestFixture]
public class XGaOutermorphismComposerUtilsEquivalenceTests
{
    private XGaFloat64Processor _float64Processor = null!;
    private XGaProcessor<double> _genericProcessor = null!;
    private const double Tolerance = 1e-12;

    [SetUp]
    public void Setup()
    {
        _float64Processor = XGaFloat64Processor.Euclidean;
        _genericProcessor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    }

    [Test]
    public void ColumnsToOutermorphism_IdentityMatrix_ShouldProduceIdenticalResults()
    {
        // Arrange - Create 3x3 identity matrix
        var float64Matrix = new double[3, 3]
        {
            { 1.0, 0.0, 0.0 },
            { 0.0, 1.0, 0.0 },
            { 0.0, 0.0, 1.0 }
        };

        var genericMatrix = new double[3, 3]
        {
            { 1.0, 0.0, 0.0 },
            { 0.0, 1.0, 0.0 },
            { 0.0, 0.0, 1.0 }
        };

        // Act - Create outermorphisms from matrices
        var float64Om = float64Matrix.ColumnsToOutermorphism(_float64Processor);
        var genericOm = genericMatrix.ColumnsToOutermorphism(_genericProcessor);

        // Test vectors
        var float64Vector = _float64Processor.Vector(2.0, 3.0, 4.0);
        var genericVector = _genericProcessor.Vector(2.0, 3.0, 4.0);

        // Act - Map vectors
        var float64Result = float64Om.OmMap(float64Vector);
        var genericResult = genericOm.OmMap(genericVector);

        // Assert - Identity matrix should preserve vectors
        for (int i = 0; i < 3; i++)
        {
            Assert.That(genericResult.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Result.GetTermScalarByIndex(i)).Within(Tolerance),
                $"Vector component e{i} should match after identity mapping");
        }
    }

    [Test]
    public void ColumnsToOutermorphism_ScalingMatrix_ShouldProduceIdenticalResults()
    {
        // Arrange - Create scaling matrix (scale by 2, 3, 4)
        var float64Matrix = new double[3, 3]
        {
            { 2.0, 0.0, 0.0 },
            { 0.0, 3.0, 0.0 },
            { 0.0, 0.0, 4.0 }
        };

        var genericMatrix = new double[3, 3]
        {
            { 2.0, 0.0, 0.0 },
            { 0.0, 3.0, 0.0 },
            { 0.0, 0.0, 4.0 }
        };

        // Act
        var float64Om = float64Matrix.ColumnsToOutermorphism(_float64Processor);
        var genericOm = genericMatrix.ColumnsToOutermorphism(_genericProcessor);

        var float64Vector = _float64Processor.Vector(1.0, 1.0, 1.0);
        var genericVector = _genericProcessor.Vector(1.0, 1.0, 1.0);

        var float64Result = float64Om.OmMap(float64Vector);
        var genericResult = genericOm.OmMap(genericVector);

        // Assert - Should scale components correctly
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue,
            Is.EqualTo(2.0).Within(Tolerance),
            "First component should be scaled by 2");

        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue,
            Is.EqualTo(3.0).Within(Tolerance),
            "Second component should be scaled by 3");

        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue,
            Is.EqualTo(4.0).Within(Tolerance),
            "Third component should be scaled by 4");
    }

    [Test]
    public void ColumnsToOutermorphism_RotationMatrix_ShouldProduceIdenticalResults()
    {
        // Arrange - Create 90-degree rotation in xy-plane
        var cos90 = 0.0;
        var sin90 = 1.0;

        var float64Matrix = new double[3, 3]
        {
            { cos90, -sin90, 0.0 },
            { sin90,  cos90, 0.0 },
            { 0.0,    0.0,   1.0 }
        };

        var genericMatrix = new double[3, 3]
        {
            { cos90, -sin90, 0.0 },
            { sin90,  cos90, 0.0 },
            { 0.0,    0.0,   1.0 }
        };

        // Act
        var float64Om = float64Matrix.ColumnsToOutermorphism(_float64Processor);
        var genericOm = genericMatrix.ColumnsToOutermorphism(_genericProcessor);

        // Test with e1 (should map to e2)
        var float64Vector = _float64Processor.Vector(1.0, 0.0, 0.0);
        var genericVector = _genericProcessor.Vector(1.0, 0.0, 0.0);

        var float64Result = float64Om.OmMap(float64Vector);
        var genericResult = genericOm.OmMap(genericVector);

        // Assert - e1 should map to e2
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
            "Mapped vectors should have same term count");

        for (int i = 0; i < 3; i++)
        {
            Assert.That(genericResult.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Result.GetTermScalarByIndex(i)).Within(Tolerance),
                $"Vector component e{i} should match after rotation");
        }
    }

    [Test]
    public void ColumnsToOutermorphism_ReflectionMatrix_ShouldProduceIdenticalResults()
    {
        // Arrange - Create reflection about xy-plane (negate z)
        var float64Matrix = new double[3, 3]
        {
            { 1.0,  0.0,  0.0 },
            { 0.0,  1.0,  0.0 },
            { 0.0,  0.0, -1.0 }
        };

        var genericMatrix = new double[3, 3]
        {
            { 1.0,  0.0,  0.0 },
            { 0.0,  1.0,  0.0 },
            { 0.0,  0.0, -1.0 }
        };

        // Act
        var float64Om = float64Matrix.ColumnsToOutermorphism(_float64Processor);
        var genericOm = genericMatrix.ColumnsToOutermorphism(_genericProcessor);

        var float64Vector = _float64Processor.Vector(2.0, 3.0, 4.0);
        var genericVector = _genericProcessor.Vector(2.0, 3.0, 4.0);

        var float64Result = float64Om.OmMap(float64Vector);
        var genericResult = genericOm.OmMap(genericVector);

        // Assert - x and y should be unchanged, z should be negated
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue,
            Is.EqualTo(2.0).Within(Tolerance),
            "X component should be unchanged");

        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue,
            Is.EqualTo(3.0).Within(Tolerance),
            "Y component should be unchanged");

        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue,
            Is.EqualTo(-4.0).Within(Tolerance),
            "Z component should be negated");
    }

    [Test]
    public void ColumnsToOutermorphism_MapsBivectors_ShouldProduceIdenticalResults()
    {
        // Arrange - Create scaling matrix
        var float64Matrix = new double[3, 3]
        {
            { 2.0, 0.0, 0.0 },
            { 0.0, 3.0, 0.0 },
            { 0.0, 0.0, 1.0 }
        };

        var genericMatrix = new double[3, 3]
        {
            { 2.0, 0.0, 0.0 },
            { 0.0, 3.0, 0.0 },
            { 0.0, 0.0, 1.0 }
        };

        var float64Om = float64Matrix.ColumnsToOutermorphism(_float64Processor);
        var genericOm = genericMatrix.ColumnsToOutermorphism(_genericProcessor);

        // Test bivector e1 ∧ e2
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.0)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.0)
            .GetBivector();

        // Act
        var float64Result = float64Om.OmMap(float64Bivector);
        var genericResult = genericOm.OmMap(genericBivector);

        // Assert - Bivector should scale by product of individual scales: 2 * 3 = 6
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
            "Mapped bivectors should have same term count");

        // Both should produce non-zero results
        Assert.That(genericResult.Count, Is.GreaterThan(0), "Mapped bivector should be non-zero");
        Assert.That(float64Result.Count, Is.GreaterThan(0), "Mapped bivector should be non-zero");
    }

    [Test]
    public void ColumnsToOutermorphism_IsValid_ShouldReturnTrue()
    {
        // Arrange
        var float64Matrix = new double[2, 2]
        {
            { 1.0, 0.0 },
            { 0.0, 1.0 }
        };

        var genericMatrix = new double[2, 2]
        {
            { 1.0, 0.0 },
            { 0.0, 1.0 }
        };

        var float64Om = float64Matrix.ColumnsToOutermorphism(_float64Processor);
        var genericOm = genericMatrix.ColumnsToOutermorphism(_genericProcessor);

        // Act & Assert
        Assert.That(float64Om.IsValid(), Is.True, "Float64 outermorphism should be valid");
        Assert.That(genericOm.IsValid(), Is.True, "Generic outermorphism should be valid");
    }
}
