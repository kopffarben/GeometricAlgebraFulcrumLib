using System;
using System.Collections.Generic;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for MapTerms equivalence - Phase 1.1 Task 1.1.3 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; MapTerms methods produce identical results.
///
/// **Status:** Verifying that existing MapTerms implementations are functionally equivalent.
/// The deduplication task document claimed these methods were missing, but they already exist.
/// These tests validate that the existing implementations produce identical results.
/// </summary>
[TestFixture]
public class XGaMapTermsEquivalenceTests
{
    private XGaFloat64Processor _float64Processor = null!;
    private XGaProcessor<double> _genericProcessor = null!;
    private const double Tolerance = 1e-14;

    [SetUp]
    public void Setup()
    {
        _float64Processor = XGaFloat64Processor.Euclidean;
        _genericProcessor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    }

    #region Vector MapTerms Tests

    [Test]
    public void Vector_MapTerms_SimpleRemapping_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 10.0)
            .SetVectorTerm(1, 20.0)
            .SetVectorTerm(2, 30.0)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 10.0)
            .SetVectorTerm(1, 20.0)
            .SetVectorTerm(2, 30.0)
            .GetVector();

        // Act - Remap: swap indices and double scalars
        var float64Result = float64Vector.MapTerms((index, scalar) =>
            new KeyValuePair<int, double>(index + 1, scalar * 2.0));
        var genericResult = genericVector.MapTerms((index, scalar) =>
            new KeyValuePair<int, double>(index + 1, scalar * 2.0));

        // Assert - [e0=10, e1=20, e2=30] becomes [e1=20, e2=40, e3=60]
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "e0 should be empty");
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(20.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(1), Is.EqualTo(20.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(40.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(2), Is.EqualTo(40.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(3).ScalarValue, Is.EqualTo(60.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(3), Is.EqualTo(60.0).Within(Tolerance));
    }

    [Test]
    public void Vector_MapTerms_ConditionalMapping_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 5.0)
            .SetVectorTerm(1, 15.0)
            .SetVectorTerm(2, 25.0)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 5.0)
            .SetVectorTerm(1, 15.0)
            .SetVectorTerm(2, 25.0)
            .GetVector();

        // Act - If scalar > 10, move to index+3 and negate
        var float64Result = float64Vector.MapTerms((index, scalar) =>
            scalar > 10.0
                ? new KeyValuePair<int, double>(index + 3, -scalar)
                : new KeyValuePair<int, double>(index, scalar));
        var genericResult = genericVector.MapTerms((index, scalar) =>
            scalar > 10.0
                ? new KeyValuePair<int, double>(index + 3, -scalar)
                : new KeyValuePair<int, double>(index, scalar));

        // Assert - e0=5 stays, e1=15 becomes e4=-15, e2=25 becomes e5=-25
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(4).ScalarValue, Is.EqualTo(-15.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(4), Is.EqualTo(-15.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(5).ScalarValue, Is.EqualTo(-25.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(5), Is.EqualTo(-25.0).Within(Tolerance));
    }

    [Test]
    public void Vector_MapTerms_ZeroVector_ShouldReturnSameZero()
    {
        // Arrange
        var float64Vector = _float64Processor.VectorZero;
        var genericVector = _genericProcessor.VectorZero;

        // Act
        var float64Result = float64Vector.MapTerms((index, scalar) =>
            new KeyValuePair<int, double>(index + 10, scalar * 5.0));
        var genericResult = genericVector.MapTerms((index, scalar) =>
            new KeyValuePair<int, double>(index + 10, scalar * 5.0));

        // Assert
        Assert.That(genericResult.IsZero, Is.True);
        Assert.That(float64Result.IsZero, Is.True);
        Assert.That(ReferenceEquals(float64Result, float64Vector), Is.True, "Should return same zero instance");
        Assert.That(ReferenceEquals(genericResult, genericVector), Is.True, "Should return same zero instance");
    }

    #endregion

    #region Bivector MapTerms Tests

    [Test]
    public void Bivector_MapTerms_SwapAndScale_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 100.0)  // e01
            .SetBivectorTerm(1, 2, 200.0)  // e12
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 100.0)
            .SetBivectorTerm(1, 2, 200.0)
            .GetBivector();

        // Act - Shift indices and halve scalars: e01 → e12, e12 → e23
        var float64Result = float64Bivector.MapTerms((i1, i2, scalar) =>
            new KeyValuePair<IPair<int>, double>(
                new Int32Pair(i1 + 1, i2 + 1),
                scalar / 2.0));
        var genericResult = genericBivector.MapTerms((i1, i2, scalar) =>
            new KeyValuePair<IPair<int>, double>(
                new Int32Pair(i1 + 1, i2 + 1),
                scalar / 2.0));

        // Assert - e01=100 becomes e12=50, e12=200 becomes e23=100
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        var id_12 = IndexSet.Create(1, 2);
        var id_23 = IndexSet.Create(2, 3);

        Assert.That(genericResult.GetBasisBladeScalar(id_12).ScalarValue, Is.EqualTo(50.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_12), Is.EqualTo(50.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_23).ScalarValue, Is.EqualTo(100.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_23), Is.EqualTo(100.0).Within(Tolerance));
    }

    [Test]
    public void Bivector_MapTerms_ConditionalRemapping_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 50.0)
            .SetBivectorTerm(1, 2, 150.0)
            .SetBivectorTerm(2, 3, 250.0)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 50.0)
            .SetBivectorTerm(1, 2, 150.0)
            .SetBivectorTerm(2, 3, 250.0)
            .GetBivector();

        // Act - If scalar > 100, shift indices by +2 and square the scalar
        var float64Result = float64Bivector.MapTerms((i1, i2, scalar) =>
            scalar > 100.0
                ? new KeyValuePair<IPair<int>, double>(
                    new Int32Pair(i1 + 2, i2 + 2),
                    scalar * scalar / 1000.0)  // Scale down to keep reasonable
                : new KeyValuePair<IPair<int>, double>(
                    new Int32Pair(i1, i2),
                    scalar));
        var genericResult = genericBivector.MapTerms((i1, i2, scalar) =>
            scalar > 100.0
                ? new KeyValuePair<IPair<int>, double>(
                    new Int32Pair(i1 + 2, i2 + 2),
                    scalar * scalar / 1000.0)
                : new KeyValuePair<IPair<int>, double>(
                    new Int32Pair(i1, i2),
                    scalar));

        // Assert
        var id_01 = IndexSet.Create(0, 1);
        var id_34 = IndexSet.Create(3, 4);
        var id_45 = IndexSet.Create(4, 5);

        Assert.That(genericResult.GetBasisBladeScalar(id_01).ScalarValue, Is.EqualTo(50.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_01), Is.EqualTo(50.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_34).ScalarValue, Is.EqualTo(22.5).Within(Tolerance)); // 150^2/1000 = 22.5
        Assert.That(float64Result.GetBasisBladeScalar(id_34), Is.EqualTo(22.5).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_45).ScalarValue, Is.EqualTo(62.5).Within(Tolerance)); // 250^2/1000 = 62.5
        Assert.That(float64Result.GetBasisBladeScalar(id_45), Is.EqualTo(62.5).Within(Tolerance));
    }

    #endregion

    #region Multivector MapTerms Tests

    [Test]
    public void Multivector_MapTerms_MixedGrades_ShouldProduceIdenticalResults()
    {
        // Arrange - Multivector with scalar, vector, and bivector
        var float64Multivector = _float64Processor.CreateMultivectorComposer()
            .SetScalarTerm(10.0)
            .SetVectorTerm(0, 20.0)
            .SetBivectorTerm(0, 1, 30.0)
            .GetMultivector();

        var genericMultivector = _genericProcessor.CreateMultivectorComposer()
            .SetScalarTerm(10.0)
            .SetVectorTerm(0, 20.0)
            .SetBivectorTerm(0, 1, 30.0)
            .GetMultivector();

        // Act - Add 1 to all indices in IndexSet and multiply scalar by 3
        var float64Result = float64Multivector.MapTerms((id, scalar) =>
        {
            if (id.IsEmptySet)
                return new KeyValuePair<IndexSet, double>(id, scalar * 3.0); // Scalar term

            var newIndices = id.Select(index => index + 1).ToArray();
            return new KeyValuePair<IndexSet, double>(
                IndexSet.Create(newIndices),
                scalar * 3.0);
        });

        var genericResult = genericMultivector.MapTerms((id, scalar) =>
        {
            if (id.IsEmptySet)
                return new KeyValuePair<IndexSet, double>(id, scalar * 3.0);

            var newIndices = id.Select(index => index + 1).ToArray();
            return new KeyValuePair<IndexSet, double>(
                IndexSet.Create(newIndices),
                scalar * 3.0);
        });

        // Assert - Scalar=10 becomes Scalar=30, e0=20 becomes e1=60, e01=30 becomes e12=90
        Assert.That(genericResult.Scalar().ScalarValue, Is.EqualTo(30.0).Within(Tolerance));
        Assert.That(float64Result.Scalar(), Is.EqualTo(30.0).Within(Tolerance));
        Assert.That(genericResult.GetVectorPart().GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(60.0).Within(Tolerance));
        Assert.That(float64Result.GetVectorPart().GetTermScalarByIndex(1), Is.EqualTo(60.0).Within(Tolerance));

        var id_12 = IndexSet.Create(1, 2);
        Assert.That(genericResult.GetBivectorPart().GetBasisBladeScalar(id_12).ScalarValue, Is.EqualTo(90.0).Within(Tolerance));
        Assert.That(float64Result.GetBivectorPart().GetBasisBladeScalar(id_12), Is.EqualTo(90.0).Within(Tolerance));
    }

    [Test]
    public void Multivector_MapTerms_ConditionalByGrade_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Multivector = _float64Processor.CreateMultivectorComposer()
            .SetScalarTerm(1.0)
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 3.0)
            .SetBivectorTerm(0, 1, 4.0)
            .GetMultivector();

        var genericMultivector = _genericProcessor.CreateMultivectorComposer()
            .SetScalarTerm(1.0)
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 3.0)
            .SetBivectorTerm(0, 1, 4.0)
            .GetMultivector();

        // Act - Double grade-0, triple grade-1, quadruple grade-2
        var float64Result = float64Multivector.MapTerms((id, scalar) =>
        {
            var multiplier = id.Count switch
            {
                0 => 2.0,
                1 => 3.0,
                2 => 4.0,
                _ => 1.0
            };
            return new KeyValuePair<IndexSet, double>(id, scalar * multiplier);
        });

        var genericResult = genericMultivector.MapTerms((id, scalar) =>
        {
            var multiplier = id.Count switch
            {
                0 => 2.0,
                1 => 3.0,
                2 => 4.0,
                _ => 1.0
            };
            return new KeyValuePair<IndexSet, double>(id, scalar * multiplier);
        });

        // Assert
        Assert.That(genericResult.Scalar().ScalarValue, Is.EqualTo(2.0).Within(Tolerance)); // 1*2
        Assert.That(float64Result.Scalar(), Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(genericResult.GetVectorPart().GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(6.0).Within(Tolerance)); // 2*3
        Assert.That(float64Result.GetVectorPart().GetTermScalarByIndex(0), Is.EqualTo(6.0).Within(Tolerance));
        Assert.That(genericResult.GetVectorPart().GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(9.0).Within(Tolerance)); // 3*3
        Assert.That(float64Result.GetVectorPart().GetTermScalarByIndex(1), Is.EqualTo(9.0).Within(Tolerance));

        var id_01 = IndexSet.Create(0, 1);
        Assert.That(genericResult.GetBivectorPart().GetBasisBladeScalar(id_01).ScalarValue, Is.EqualTo(16.0).Within(Tolerance)); // 4*4
        Assert.That(float64Result.GetBivectorPart().GetBasisBladeScalar(id_01), Is.EqualTo(16.0).Within(Tolerance));
    }

    [Test]
    public void Multivector_MapTerms_IdentityMapping_ShouldProduceIdenticalCopy()
    {
        // Arrange
        var float64Multivector = _float64Processor.CreateMultivectorComposer()
            .SetScalarTerm(1.0)
            .SetVectorTerm(0, 2.0)
            .SetBivectorTerm(1, 2, 3.0)
            .GetMultivector();

        var genericMultivector = _genericProcessor.CreateMultivectorComposer()
            .SetScalarTerm(1.0)
            .SetVectorTerm(0, 2.0)
            .SetBivectorTerm(1, 2, 3.0)
            .GetMultivector();

        // Act - Identity mapping
        var float64Result = float64Multivector.MapTerms((id, scalar) =>
            new KeyValuePair<IndexSet, double>(id, scalar));
        var genericResult = genericMultivector.MapTerms((id, scalar) =>
            new KeyValuePair<IndexSet, double>(id, scalar));

        // Assert - Should be identical
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        foreach (var pair in float64Result.IdScalarPairs)
        {
            Assert.That(genericResult.GetBasisBladeScalar(pair.Key).ScalarValue,
                Is.EqualTo(pair.Value).Within(Tolerance),
                $"Term at {pair.Key} should match");
        }
    }

    #endregion
}
