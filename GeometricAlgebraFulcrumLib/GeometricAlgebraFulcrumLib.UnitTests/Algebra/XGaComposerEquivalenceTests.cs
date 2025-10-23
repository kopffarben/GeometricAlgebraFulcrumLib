using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for Composer equivalence - Phase 1.2 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; composers produce identical multivectors.
///
/// Note: Generic composers have MORE methods (Hybrid API with 8 overloads per method)
/// while Float64 is minimalistic. Tests verify functional equivalence, not API parity.
/// </summary>
[TestFixture]
public class XGaComposerEquivalenceTests
{
    private XGaFloat64Processor _float64Processor = null!;
    private XGaProcessor<double> _genericProcessor = null!;

    [SetUp]
    public void Setup()
    {
        _float64Processor = XGaFloat64Processor.Euclidean;
        _genericProcessor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    }

    #region VectorComposer Tests

    [Test]
    public void VectorComposer_SetVectorTerm_ShouldProduceIdenticalVectors()
    {
        // Arrange
        var float64Composer = _float64Processor.CreateVectorComposer();
        var genericComposer = _genericProcessor.CreateVectorComposer();

        // Act - Set same terms
        float64Composer
            .SetVectorTerm(0, 1.5)
            .SetVectorTerm(1, 2.5)
            .SetVectorTerm(3, 4.5);

        genericComposer
            .SetVectorTerm(0, 1.5)
            .SetVectorTerm(1, 2.5)
            .SetVectorTerm(3, 4.5);

        var float64Vector = float64Composer.GetVector();
        var genericVector = genericComposer.GetVector();

        // Assert
        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count), "Same number of terms");

        for (int i = 0; i < 5; i++)
        {
            Assert.That(genericVector.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Vector.GetTermScalarByIndex(i)).Within(1e-14),
                $"Term at index {i} should match");
        }
    }

    [Test]
    public void VectorComposer_AddVectorTerm_ShouldProduceIdenticalVectors()
    {
        // Arrange & Act
        var float64Composer = _float64Processor.CreateVectorComposer();
        float64Composer.AddVectorTerm(0, 1.0);
        float64Composer.AddVectorTerm(0, 2.0);  // Should sum to 3.0
        float64Composer.AddVectorTerm(1, 5.0);
        var float64Vector = float64Composer.GetVector();

        var genericComposer = _genericProcessor.CreateVectorComposer();
        genericComposer.AddVectorTerm(0, 1.0);
        genericComposer.AddVectorTerm(0, 2.0);
        genericComposer.AddVectorTerm(1, 5.0);
        var genericVector = genericComposer.GetVector();

        // Assert
        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));
        Assert.That(genericVector.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(3.0));
        Assert.That(float64Vector.GetTermScalarByIndex(0), Is.EqualTo(3.0));
        Assert.That(genericVector.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(5.0));
    }

    [Test]
    public void VectorComposer_SetVectorTerms_FromArray_ShouldProduceIdenticalVectors()
    {
        // Arrange
        var scalars = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };

        // Act
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerms(0, scalars)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerms(0, scalars)
            .GetVector();

        // Assert
        Assert.That(genericVector.Count, Is.EqualTo(float64Vector.Count));

        for (int i = 0; i < 5; i++)
        {
            Assert.That(genericVector.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(scalars[i]).Within(1e-14));
            Assert.That(float64Vector.GetTermScalarByIndex(i),
                Is.EqualTo(scalars[i]).Within(1e-14));
        }
    }

    [Test]
    public void VectorComposer_SubtractVectorTerm_ShouldProduceIdenticalVectors()
    {
        // Arrange & Act
        var float64Composer = _float64Processor.CreateVectorComposer();
        float64Composer.SetVectorTerm(0, 10.0);
        float64Composer.SubtractVectorTerm(0, 3.0);  // Should be 7.0
        float64Composer.SetVectorTerm(1, 5.0);
        var float64Vector = float64Composer.GetVector();

        var genericComposer = _genericProcessor.CreateVectorComposer();
        genericComposer.SetVectorTerm(0, 10.0);
        genericComposer.SubtractVectorTerm(0, 3.0);
        genericComposer.SetVectorTerm(1, 5.0);
        var genericVector = genericComposer.GetVector();

        // Assert
        Assert.That(genericVector.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(7.0));
        Assert.That(float64Vector.GetTermScalarByIndex(0), Is.EqualTo(7.0));
    }

    #endregion

    #region BivectorComposer Tests

    [Test]
    public void BivectorComposer_SetBivectorTerm_ShouldProduceIdenticalBivectors()
    {
        // Arrange & Act
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.5)
            .SetBivectorTerm(0, 2, 3.5)
            .SetBivectorTerm(1, 2, 5.0)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.5)
            .SetBivectorTerm(0, 2, 3.5)
            .SetBivectorTerm(1, 2, 5.0)
            .GetBivector();

        // Assert
        Assert.That(genericBivector.Count, Is.EqualTo(float64Bivector.Count));

        Assert.That(genericBivector.GetBasisBladeScalar(IndexSet.Create(0, 1)).ScalarValue,
            Is.EqualTo(float64Bivector.GetBasisBladeScalar(IndexSet.Create(0, 1))).Within(1e-14));
        Assert.That(genericBivector.GetBasisBladeScalar(IndexSet.Create(0, 2)).ScalarValue,
            Is.EqualTo(float64Bivector.GetBasisBladeScalar(IndexSet.Create(0, 2))).Within(1e-14));
        Assert.That(genericBivector.GetBasisBladeScalar(IndexSet.Create(1, 2)).ScalarValue,
            Is.EqualTo(float64Bivector.GetBasisBladeScalar(IndexSet.Create(1, 2))).Within(1e-14));
    }

    [Test]
    public void BivectorComposer_AddBivectorTerm_ShouldProduceIdenticalBivectors()
    {
        // Arrange & Act
        var float64Composer = _float64Processor.CreateBivectorComposer();
        float64Composer.AddBivectorTerm(0, 1, 1.0);
        float64Composer.AddBivectorTerm(0, 1, 2.0);  // Should sum to 3.0
        var float64Bivector = float64Composer.GetBivector();

        var genericComposer = _genericProcessor.CreateBivectorComposer();
        genericComposer.AddBivectorTerm(0, 1, 1.0);
        genericComposer.AddBivectorTerm(0, 1, 2.0);
        var genericBivector = genericComposer.GetBivector();

        // Assert
        Assert.That(genericBivector.GetBasisBladeScalar(IndexSet.Create(0, 1)).ScalarValue,
            Is.EqualTo(3.0).Within(1e-14));
        Assert.That(float64Bivector.GetBasisBladeScalar(IndexSet.Create(0, 1)),
            Is.EqualTo(3.0).Within(1e-14));
    }

    #endregion

    #region KVectorComposer Tests

    [Test]
    public void KVectorComposer_MixedOperations_ShouldProduceIdenticalKVectors()
    {
        // Arrange & Act - Create grade-3 k-vector (trivector)
        var id_012 = IndexSet.Create(new[] { 0, 1, 2 });
        var id_013 = IndexSet.Create(new[] { 0, 1, 3 });

        var float64Composer = _float64Processor.CreateKVectorComposer(3);
        float64Composer.SetTerm(id_012, 1.5);
        float64Composer.SetTerm(id_013, 2.5);
        float64Composer.AddTerm(id_012, 0.5);  // Should be 2.0 total
        var float64KVector = float64Composer.GetKVector();

        var genericComposer = _genericProcessor.CreateKVectorComposer(3);
        genericComposer.SetTrivectorTerm(0, 1, 2, 1.5);
        genericComposer.SetTrivectorTerm(0, 1, 3, 2.5);
        genericComposer.AddTrivectorTerm(0, 1, 2, 0.5);
        var genericKVector = genericComposer.GetKVector();

        // Assert
        Assert.That(genericKVector.Count, Is.EqualTo(float64KVector.Count));
        Assert.That(genericKVector.Grade, Is.EqualTo(3));
        Assert.That(float64KVector.Grade, Is.EqualTo(3));

        Assert.That(genericKVector.GetBasisBladeScalar(id_012).ScalarValue,
            Is.EqualTo(2.0).Within(1e-14));
        Assert.That(float64KVector.GetBasisBladeScalar(id_012),
            Is.EqualTo(2.0).Within(1e-14));
    }

    [Test]
    public void KVectorComposer_Clear_ShouldProduceIdenticalEmptyResults()
    {
        // Arrange
        var float64Composer = _float64Processor.CreateKVectorComposer(2)
            .SetBivectorTerm(0, 1, 5.0);

        var genericComposer = _genericProcessor.CreateKVectorComposer(2)
            .SetBivectorTerm(0, 1, 5.0);

        // Act
        float64Composer.Clear();
        genericComposer.Clear();

        var float64KVector = float64Composer.GetKVector();
        var genericKVector = genericComposer.GetKVector();

        // Assert
        Assert.That(genericKVector.IsZero, Is.True);
        Assert.That(float64KVector.IsZero, Is.True);
        Assert.That(genericKVector.Count, Is.EqualTo(0));
        Assert.That(float64KVector.Count, Is.EqualTo(0));
    }

    #endregion

    #region MultivectorComposer Tests

    [Test]
    public void MultivectorComposer_MixedGrades_ShouldProduceIdenticalMultivectors()
    {
        // Arrange & Act - Create multivector with scalar, vector, and bivector parts
        var float64Multivector = _float64Processor.CreateMultivectorComposer()
            .SetScalarTerm(10.0)
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetBivectorTerm(0, 1, 3.0)
            .SetBivectorTerm(1, 2, 4.0)
            .GetMultivector();

        var genericMultivector = _genericProcessor.CreateMultivectorComposer()
            .SetScalarTerm(10.0)
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetBivectorTerm(0, 1, 3.0)
            .SetBivectorTerm(1, 2, 4.0)
            .GetMultivector();

        // Assert
        Assert.That(genericMultivector.Count, Is.EqualTo(float64Multivector.Count));
        Assert.That(genericMultivector.Count, Is.EqualTo(5), "Should have 5 terms total");

        // Check scalar part
        Assert.That(genericMultivector.GetScalarPart().ScalarValue,
            Is.EqualTo(float64Multivector.Scalar()).Within(1e-14));

        // Check vector part
        var float64Vector = float64Multivector.GetVectorPart();
        var genericVector = genericMultivector.GetVectorPart();

        Assert.That(genericVector.GetTermScalarByIndex(0).ScalarValue,
            Is.EqualTo(float64Vector.GetTermScalarByIndex(0)).Within(1e-14));
        Assert.That(genericVector.GetTermScalarByIndex(1).ScalarValue,
            Is.EqualTo(float64Vector.GetTermScalarByIndex(1)).Within(1e-14));

        // Check bivector part
        var float64Bivector = float64Multivector.GetBivectorPart();
        var genericBivector = genericMultivector.GetBivectorPart();

        Assert.That(genericBivector.GetBasisBladeScalar(IndexSet.Create(0, 1)).ScalarValue,
            Is.EqualTo(float64Bivector.GetBasisBladeScalar(IndexSet.Create(0, 1))).Within(1e-14));
    }

    [Test]
    public void MultivectorComposer_SetTerm_ByIndexSet_ShouldProduceIdenticalMultivectors()
    {
        // Arrange & Act
        var id1 = IndexSet.Create(0);        // Vector e_0
        var id2 = IndexSet.Create(0, 1);     // Bivector e_01
        var id3 = IndexSet.Create(0, 1, 2);  // Trivector e_012

        var float64Multivector = _float64Processor.CreateMultivectorComposer()
            .SetTerm(id1, 1.0)
            .SetTerm(id2, 2.0)
            .SetTerm(id3, 3.0)
            .GetMultivector();

        var genericMultivector = _genericProcessor.CreateMultivectorComposer()
            .SetTerm(id1, 1.0)
            .SetTerm(id2, 2.0)
            .SetTerm(id3, 3.0)
            .GetMultivector();

        // Assert
        Assert.That(genericMultivector.Count, Is.EqualTo(3));
        Assert.That(float64Multivector.Count, Is.EqualTo(3));

        foreach (var pair in float64Multivector.IdScalarPairs)
        {
            Assert.That(genericMultivector.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(1e-14),
                $"Term at {pair.Key} should match");
        }
    }

    #endregion

    #region Composer Operations Tests

    [Test]
    public void Composer_Negative_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Composer = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 5.0)
            .SetVectorTerm(1, -3.0);

        var genericComposer = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 5.0)
            .SetVectorTerm(1, -3.0);

        // Act
        var float64Vector = float64Composer.Negative().GetVector();
        var genericVector = genericComposer.Negative().GetVector();

        // Assert
        Assert.That(genericVector.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(-5.0));
        Assert.That(float64Vector.GetTermScalarByIndex(0), Is.EqualTo(-5.0));
        Assert.That(genericVector.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(3.0));
        Assert.That(float64Vector.GetTermScalarByIndex(1), Is.EqualTo(3.0));
    }

    [Test]
    public void Composer_Times_ShouldProduceIdenticalResults()
    {
        // Arrange & Act
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 4.0)
            .Times(2.5)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 4.0)
            .Times(2.5)
            .GetVector();

        // Assert
        Assert.That(genericVector.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(5.0));
        Assert.That(float64Vector.GetTermScalarByIndex(0), Is.EqualTo(5.0));
        Assert.That(genericVector.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(10.0));
        Assert.That(float64Vector.GetTermScalarByIndex(1), Is.EqualTo(10.0));
    }

    [Test]
    public void Composer_Reverse_ShouldProduceIdenticalResults()
    {
        // Arrange & Act - Reverse of bivector negates it (grade 2)
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 3.0)
            .Reverse()
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 3.0)
            .Reverse()
            .GetBivector();

        // Assert - Reverse of grade 2 should negate
        Assert.That(genericBivector.GetBasisBladeScalar(IndexSet.Create(0, 1)).ScalarValue,
            Is.EqualTo(-3.0).Within(1e-14));
        Assert.That(float64Bivector.GetBasisBladeScalar(IndexSet.Create(0, 1)),
            Is.EqualTo(-3.0).Within(1e-14));
    }

    #endregion

    #region Edge Cases

    [Test]
    public void Composer_EmptyComposer_ShouldProduceIdenticalZeroResults()
    {
        // Arrange & Act
        var float64Vector = _float64Processor.CreateVectorComposer().GetVector();
        var genericVector = _genericProcessor.CreateVectorComposer().GetVector();

        // Assert
        Assert.That(genericVector.IsZero, Is.True);
        Assert.That(float64Vector.IsZero, Is.True);
        Assert.That(genericVector.Count, Is.EqualTo(0));
        Assert.That(float64Vector.Count, Is.EqualTo(0));
    }

    [Test]
    public void Composer_RemoveTerm_ShouldProduceIdenticalResults()
    {
        // Arrange & Act
        var float64Composer = _float64Processor.CreateVectorComposer();
        float64Composer.SetVectorTerm(0, 1.0);
        float64Composer.SetVectorTerm(1, 2.0);
        float64Composer.SetVectorTerm(2, 3.0);
        float64Composer.RemoveVectorTerm(1);  // Remove middle term
        var float64Vector = float64Composer.GetVector();

        var genericComposer = _genericProcessor.CreateVectorComposer();
        genericComposer.SetVectorTerm(0, 1.0);
        genericComposer.SetVectorTerm(1, 2.0);
        genericComposer.SetVectorTerm(2, 3.0);
        genericComposer.RemoveVectorTerm(1);
        var genericVector = genericComposer.GetVector();

        // Assert
        Assert.That(genericVector.Count, Is.EqualTo(2));
        Assert.That(float64Vector.Count, Is.EqualTo(2));
        Assert.That(genericVector.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(1.0));
        Assert.That(genericVector.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(0.0), "Index 1 removed");
        Assert.That(genericVector.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(3.0));
    }

    #endregion
}
