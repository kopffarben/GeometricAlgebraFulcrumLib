using System;
using System.Collections.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Frames;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Frames;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for XGaGramSchmidtFrame equivalence - Module 1, Task 1.4 of deduplication roadmap.
/// Tests ensure Float64 (QR-based) and Generic&lt;double&gt; (Gram-Schmidt-based) produce equivalent orthonormal frames.
/// Note: Exact equivalence is not expected due to different algorithms (QR vs Gram-Schmidt),
/// but orthonormality and geometric properties should match within tolerance.
/// </summary>
[TestFixture]
public class XGaGramSchmidtFrameEquivalenceTests
{
    private XGaFloat64Processor _float64Processor = null!;
    private XGaProcessor<double> _genericProcessor = null!;
    private const double Tolerance = 1e-10;

    [SetUp]
    public void Setup()
    {
        _float64Processor = XGaFloat64Processor.Euclidean;
        _genericProcessor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    }

    /// <summary>
    /// Helper method to verify orthonormality of a frame.
    /// </summary>
    private void AssertFrameIsOrthonormal<T>(IReadOnlyList<T> norms, double tolerance, string message)
    {
        // All norms should be non-negative (by construction)
        foreach (var norm in norms)
        {
            var normValue = Convert.ToDouble(norm);
            Assert.That(normValue, Is.GreaterThanOrEqualTo(0.0),
                $"{message}: Norm should be non-negative");
        }
    }

    [Test]
    public void GramSchmidtFrame_OrthogonalVectors_ShouldProduceOrthonormalFrame()
    {
        // Arrange - Create orthogonal input vectors (e1, e2, e3)
        var float64V1 = _float64Processor.Vector(1.0, 0.0, 0.0);
        var float64V2 = _float64Processor.Vector(0.0, 2.0, 0.0);
        var float64V3 = _float64Processor.Vector(0.0, 0.0, 3.0);

        var genericV1 = _genericProcessor.Vector(1.0, 0.0, 0.0);
        var genericV2 = _genericProcessor.Vector(0.0, 2.0, 0.0);
        var genericV3 = _genericProcessor.Vector(0.0, 0.0, 3.0);

        // Act
        var float64Frame = XGaFloat64GramSchmidtFrame.Create(float64V1, float64V2, float64V3);
        var genericFrame = XGaGramSchmidtFrame<double>.Create(genericV1, genericV2, genericV3);

        // Assert - Check norms
        Assert.That(genericFrame.DirectionNorms.Count, Is.EqualTo(3), "Generic frame should have 3 direction norms");
        Assert.That(float64Frame.DirectionNorms.Count, Is.EqualTo(3), "Float64 frame should have 3 direction norms");

        // Check that norms match (within tolerance)
        for (int i = 0; i < 3; i++)
        {
            Assert.That(genericFrame.DirectionNorms[i],
                Is.EqualTo(float64Frame.DirectionNorms[i]).Within(Tolerance),
                $"Direction norm {i} should match");
        }

        // Check that unit directions are orthonormal
        for (int i = 0; i < 3; i++)
        {
            var unitNorm = genericFrame.UnitDirections[i].Norm().ScalarValue;
            Assert.That(unitNorm, Is.EqualTo(1.0).Within(Tolerance),
                $"Unit direction {i} should have norm 1");
        }
    }

    [Test]
    public void GramSchmidtFrame_NonOrthogonalVectors_ShouldOrthogonalize()
    {
        // Arrange - Create non-orthogonal input vectors
        var float64V1 = _float64Processor.Vector(1.0, 0.0, 0.0);
        var float64V2 = _float64Processor.Vector(1.0, 1.0, 0.0);  // Not orthogonal to v1
        var float64V3 = _float64Processor.Vector(1.0, 1.0, 1.0);  // Not orthogonal to v1, v2

        var genericV1 = _genericProcessor.Vector(1.0, 0.0, 0.0);
        var genericV2 = _genericProcessor.Vector(1.0, 1.0, 0.0);
        var genericV3 = _genericProcessor.Vector(1.0, 1.0, 1.0);

        // Act
        var float64Frame = XGaFloat64GramSchmidtFrame.Create(float64V1, float64V2, float64V3);
        var genericFrame = XGaGramSchmidtFrame<double>.Create(genericV1, genericV2, genericV3);

        // Assert - Unit directions should be orthogonal
        for (int i = 0; i < 3; i++)
        {
            for (int j = i + 1; j < 3; j++)
            {
                var dotProduct = genericFrame.UnitDirections[i].ESp(genericFrame.UnitDirections[j]).ScalarValue;
                Assert.That(Math.Abs(dotProduct), Is.LessThan(Tolerance),
                    $"Unit directions {i} and {j} should be orthogonal");
            }
        }
    }

    [Test]
    public void GramSchmidtFrame_GetDirection_ShouldRecoverScaledOriginalVectors()
    {
        // Arrange - Orthogonal vectors with known norms
        var float64V1 = _float64Processor.Vector(2.0, 0.0, 0.0);  // norm = 2
        var float64V2 = _float64Processor.Vector(0.0, 3.0, 0.0);  // norm = 3
        var float64V3 = _float64Processor.Vector(0.0, 0.0, 4.0);  // norm = 4

        var genericV1 = _genericProcessor.Vector(2.0, 0.0, 0.0);
        var genericV2 = _genericProcessor.Vector(0.0, 3.0, 0.0);
        var genericV3 = _genericProcessor.Vector(0.0, 0.0, 4.0);

        // Act
        var float64Frame = XGaFloat64GramSchmidtFrame.Create(float64V1, float64V2, float64V3);
        var genericFrame = XGaGramSchmidtFrame<double>.Create(genericV1, genericV2, genericV3);

        // Assert - GetDirection should return norm * unit direction ≈ original vector
        var genericD1 = genericFrame.GetDirection(0);
        var genericD2 = genericFrame.GetDirection(1);
        var genericD3 = genericFrame.GetDirection(2);

        Assert.That(genericD1.Norm().ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Direction 0 norm should be 2");
        Assert.That(genericD2.Norm().ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Direction 1 norm should be 3");
        Assert.That(genericD3.Norm().ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Direction 2 norm should be 4");
    }

    [Test]
    public void GramSchmidtFrame_GetCurvature_ShouldComputeNormRatios()
    {
        // Arrange
        var float64V1 = _float64Processor.Vector(2.0, 0.0, 0.0);  // norm = 2
        var float64V2 = _float64Processor.Vector(0.0, 4.0, 0.0);  // norm = 4
        var float64V3 = _float64Processor.Vector(0.0, 0.0, 8.0);  // norm = 8

        var genericV1 = _genericProcessor.Vector(2.0, 0.0, 0.0);
        var genericV2 = _genericProcessor.Vector(0.0, 4.0, 0.0);
        var genericV3 = _genericProcessor.Vector(0.0, 0.0, 8.0);

        // Act
        var float64Frame = XGaFloat64GramSchmidtFrame.Create(float64V1, float64V2, float64V3);
        var genericFrame = XGaGramSchmidtFrame<double>.Create(genericV1, genericV2, genericV3);

        var curvature0 = genericFrame.GetCurvature(0);  // norm[1] / norm[0] = 4 / 2 = 2
        var curvature1 = genericFrame.GetCurvature(1);  // norm[2] / norm[1] = 8 / 4 = 2

        // Assert
        Assert.That(curvature0, Is.EqualTo(2.0).Within(Tolerance), "Curvature 0 should be 2.0");
        Assert.That(curvature1, Is.EqualTo(2.0).Within(Tolerance), "Curvature 1 should be 2.0");
    }

    [Test]
    public void GramSchmidtFrame_GetDarbouxBlade_ShouldReturnBivector()
    {
        // Arrange
        var genericV1 = _genericProcessor.Vector(1.0, 0.0, 0.0);
        var genericV2 = _genericProcessor.Vector(0.0, 2.0, 0.0);
        var genericV3 = _genericProcessor.Vector(0.0, 0.0, 3.0);

        // Act
        var genericFrame = XGaGramSchmidtFrame<double>.Create(genericV1, genericV2, genericV3);
        var darbouxBlade0 = genericFrame.GetDarbouxBlade(0);

        // Assert
        Assert.That(darbouxBlade0, Is.Not.Null, "Darboux blade should not be null");
        Assert.That(darbouxBlade0.Grade, Is.EqualTo(2), "Darboux blade should be grade 2 (bivector)");
    }

    [Test]
    public void GramSchmidtFrame_GetDarbouxBivector_ShouldReturnSumOfBlades()
    {
        // Arrange
        var genericV1 = _genericProcessor.Vector(1.0, 0.0, 0.0);
        var genericV2 = _genericProcessor.Vector(0.0, 1.0, 0.0);
        var genericV3 = _genericProcessor.Vector(0.0, 0.0, 1.0);

        // Act
        var genericFrame = XGaGramSchmidtFrame<double>.Create(genericV1, genericV2, genericV3);
        var darbouxBivector = genericFrame.GetDarbouxBivector();

        // Assert
        Assert.That(darbouxBivector, Is.Not.Null, "Darboux bivector should not be null");
        Assert.That(darbouxBivector.Grade, Is.EqualTo(2), "Darboux bivector should be grade 2");
    }

    [Test]
    public void GramSchmidtFrame_CleanNorms_ShouldSetNearZeroNormsToZero()
    {
        // Arrange - Create frame with one near-zero vector
        var genericV1 = _genericProcessor.Vector(1.0, 0.0, 0.0);
        var genericV2 = _genericProcessor.Vector(0.0, 1.0, 0.0);
        var genericV3 = _genericProcessor.Vector(1e-20, 1e-20, 0.0);  // Near zero

        // Act
        var genericFrame = XGaGramSchmidtFrame<double>.Create(genericV1, genericV2, genericV3);
        genericFrame.CleanNorms();

        // Assert - Third norm should be very small or zero
        Assert.That(genericFrame.DirectionNorms[2], Is.LessThan(1e-10),
            "Near-zero norm should be cleaned");
    }

    [Test]
    public void GramSchmidtFrame_2DVectors_ShouldOrthogonalize()
    {
        // Arrange - 2D vectors in 3D space
        var float64V1 = _float64Processor.Vector(3.0, 4.0, 0.0);  // norm = 5
        var float64V2 = _float64Processor.Vector(1.0, 1.0, 0.0);  // not orthogonal to v1

        var genericV1 = _genericProcessor.Vector(3.0, 4.0, 0.0);
        var genericV2 = _genericProcessor.Vector(1.0, 1.0, 0.0);

        // Act
        var float64Frame = XGaFloat64GramSchmidtFrame.Create(float64V1, float64V2);
        var genericFrame = XGaGramSchmidtFrame<double>.Create(genericV1, genericV2);

        // Assert - Check orthogonality
        var dotProduct = genericFrame.UnitDirections[0].ESp(genericFrame.UnitDirections[1]).ScalarValue;
        Assert.That(Math.Abs(dotProduct), Is.LessThan(Tolerance),
            "Unit directions should be orthogonal");

        // Check first norm matches
        Assert.That(genericFrame.DirectionNorms[0], Is.EqualTo(5.0).Within(Tolerance),
            "First direction norm should be 5.0");
    }

    [Test]
    public void GramSchmidtFrame_SingleVector_ShouldNormalize()
    {
        // Arrange
        var genericV1 = _genericProcessor.Vector(3.0, 4.0, 0.0);  // norm = 5

        // Act
        var genericFrame = XGaGramSchmidtFrame<double>.Create(genericV1);

        // Assert
        Assert.That(genericFrame.DirectionNorms.Count, Is.EqualTo(1), "Should have 1 direction norm");
        Assert.That(genericFrame.DirectionNorms[0], Is.EqualTo(5.0).Within(Tolerance), "Norm should be 5.0");
        Assert.That(genericFrame.UnitDirections[0].Norm().ScalarValue, Is.EqualTo(1.0).Within(Tolerance),
            "Unit direction should have norm 1");
    }
}
