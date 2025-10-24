using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for MapBasisVectors/MapBasisBlades equivalence - Phase 1.1 Task 1.1.2 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; basis mapping methods produce identical results.
///
/// **Status:** Verifying that existing MapBasis* implementations are functionally equivalent.
/// The deduplication task document claimed these methods were missing, but they already exist.
/// These tests validate that the existing implementations produce identical results.
/// </summary>
[TestFixture]
public class XGaMapBasisEquivalenceTests
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

    #region Vector MapBasisVectors Tests

    [Test]
    public void Vector_MapBasisVectors_SimpleRemapping_ShouldProduceIdenticalResults()
    {
        // Arrange - Create vectors
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

        // Act - Remap: swap indices 0 and 2
        var float64Result = float64Vector.MapBasisVectors(index =>
            index == 0 ? 2 : index == 2 ? 0 : index);
        var genericResult = genericVector.MapBasisVectors(index =>
            index == 0 ? 2 : index == 2 ? 0 : index);

        // Assert - After swap: e0↔e2, so [10, 20, 30] becomes [30, 20, 10]
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(30.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(30.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(20.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(1), Is.EqualTo(20.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(2), Is.EqualTo(10.0).Within(Tolerance));
    }

    [Test]
    public void Vector_MapBasisVectors_ShiftIndices_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .GetVector();

        // Act - Shift all indices up by 1
        var float64Result = float64Vector.MapBasisVectors(index => index + 1);
        var genericResult = genericVector.MapBasisVectors(index => index + 1);

        // Assert - [e0=1, e1=2, e2=3] becomes [e1=1, e2=2, e3=3]
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "e0 should be empty");
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(1), Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(2), Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(3).ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(3), Is.EqualTo(3.0).Within(Tolerance));
    }

    [Test]
    public void Vector_MapBasisVectors_WithScalar_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 5.0)
            .SetVectorTerm(1, 10.0)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 5.0)
            .SetVectorTerm(1, 10.0)
            .GetVector();

        // Act - Remap with scalar dependency: if scalar > 7, shift index by 2
        var float64Result = float64Vector.MapBasisVectors((index, scalar) =>
            scalar > 7.0 ? index + 2 : index);
        var genericResult = genericVector.MapBasisVectors((index, scalar) =>
            scalar > 7.0 ? index + 2 : index);

        // Assert - e0=5 (not > 7) stays at e0, e1=10 (> 7) moves to e3
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "e1 should be empty");
        Assert.That(float64Result.GetTermScalarByIndex(1), Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(3).ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(3), Is.EqualTo(10.0).Within(Tolerance));
    }

    [Test]
    public void Vector_MapBasisVectors_IdentityMapping_ShouldProduceIdenticalCopy()
    {
        // Arrange
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 7.5)
            .SetVectorTerm(2, 9.5)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 7.5)
            .SetVectorTerm(2, 9.5)
            .GetVector();

        // Act - Identity mapping (no change)
        var float64Result = float64Vector.MapBasisVectors(index => index);
        var genericResult = genericVector.MapBasisVectors(index => index);

        // Assert - Should be identical to original
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(7.5).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(7.5).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(9.5).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(2), Is.EqualTo(9.5).Within(Tolerance));
    }

    [Test]
    public void Vector_MapBasisVectors_ZeroVector_ShouldReturnSameZero()
    {
        // Arrange
        var float64Vector = _float64Processor.VectorZero;
        var genericVector = _genericProcessor.VectorZero;

        // Act
        var float64Result = float64Vector.MapBasisVectors(index => index + 10);
        var genericResult = genericVector.MapBasisVectors(index => index + 10);

        // Assert
        Assert.That(genericResult.IsZero, Is.True);
        Assert.That(float64Result.IsZero, Is.True);
        Assert.That(ReferenceEquals(float64Result, float64Vector), Is.True, "Should return same zero instance");
        Assert.That(ReferenceEquals(genericResult, genericVector), Is.True, "Should return same zero instance");
    }

    #endregion

    #region Bivector MapBasisBivectors Tests

    [Test]
    public void Bivector_MapBasisBivectors_SwapIndices_ShouldProduceIdenticalResults()
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

        // Act - Remap: e01 → e12, e12 → e23
        var float64Result = float64Bivector.MapBasisBivectors((i1, i2) =>
        {
            if (i1 == 0 && i2 == 1) return new Int32Pair(1, 2);
            if (i1 == 1 && i2 == 2) return new Int32Pair(2, 3);
            return new Int32Pair(i1, i2);
        });

        var genericResult = genericBivector.MapBasisBivectors((i1, i2) =>
        {
            if (i1 == 0 && i2 == 1) return new Int32Pair(1, 2);
            if (i1 == 1 && i2 == 2) return new Int32Pair(2, 3);
            return new Int32Pair(i1, i2);
        });

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        var id_12 = IndexSet.Create(1, 2);
        var id_23 = IndexSet.Create(2, 3);

        Assert.That(genericResult.GetBasisBladeScalar(id_12).ScalarValue, Is.EqualTo(100.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_12), Is.EqualTo(100.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_23).ScalarValue, Is.EqualTo(200.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_23), Is.EqualTo(200.0).Within(Tolerance));
    }

    [Test]
    public void Bivector_MapBasisBivectors_WithScalar_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 50.0)
            .SetBivectorTerm(1, 2, 150.0)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 50.0)
            .SetBivectorTerm(1, 2, 150.0)
            .GetBivector();

        // Act - If scalar > 100, shift both indices by 1
        var float64Result = float64Bivector.MapBasisBivectors((i1, i2, scalar) =>
        {
            if (scalar > 100.0)
                return new Int32Pair(i1 + 1, i2 + 1);
            return new Int32Pair(i1, i2);
        });

        var genericResult = genericBivector.MapBasisBivectors((i1, i2, scalar) =>
        {
            if (scalar > 100.0)
                return new Int32Pair(i1 + 1, i2 + 1);
            return new Int32Pair(i1, i2);
        });

        // Assert - e01=50 stays, e12=150 becomes e23
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        var id_01 = IndexSet.Create(0, 1);
        var id_23 = IndexSet.Create(2, 3);

        Assert.That(genericResult.GetBasisBladeScalar(id_01).ScalarValue, Is.EqualTo(50.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_01), Is.EqualTo(50.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_23).ScalarValue, Is.EqualTo(150.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_23), Is.EqualTo(150.0).Within(Tolerance));
    }

    #endregion

    #region Multivector MapBasisBlades Tests

    [Test]
    public void Multivector_MapBasisBlades_MixedGrades_ShouldProduceIdenticalResults()
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

        // Act - Remap: shift all basis blade indices by adding 1 to each index
        var float64Result = float64Multivector.MapBasisBlades(id =>
        {
            if (id.IsEmptySet) return id; // Scalar unchanged
            var newIndices = id.Select(index => index + 1).ToArray();
            return IndexSet.Create(newIndices);
        });

        var genericResult = genericMultivector.MapBasisBlades(id =>
        {
            if (id.IsEmptySet) return id; // Scalar unchanged
            var newIndices = id.Select(index => index + 1).ToArray();
            return IndexSet.Create(newIndices);
        });

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        // Scalar unchanged
        Assert.That(genericResult.Scalar().ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(float64Result.Scalar(), Is.EqualTo(10.0).Within(Tolerance));

        // Vector e0 → e1
        Assert.That(genericResult.GetVectorPart().GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(20.0).Within(Tolerance));
        Assert.That(float64Result.GetVectorPart().GetTermScalarByIndex(1), Is.EqualTo(20.0).Within(Tolerance));

        // Bivector e01 → e12
        var id_12 = IndexSet.Create(1, 2);
        Assert.That(genericResult.GetBivectorPart().GetBasisBladeScalar(id_12).ScalarValue, Is.EqualTo(30.0).Within(Tolerance));
        Assert.That(float64Result.GetBivectorPart().GetBasisBladeScalar(id_12), Is.EqualTo(30.0).Within(Tolerance));
    }

    [Test]
    public void Multivector_MapBasisBlades_WithScalar_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Multivector = _float64Processor.CreateMultivectorComposer()
            .SetVectorTerm(0, 5.0)
            .SetVectorTerm(1, 15.0)
            .GetMultivector();

        var genericMultivector = _genericProcessor.CreateMultivectorComposer()
            .SetVectorTerm(0, 5.0)
            .SetVectorTerm(1, 15.0)
            .GetMultivector();

        // Act - If scalar > 10, double all indices
        var float64Result = float64Multivector.MapBasisBlades((id, scalar) =>
        {
            if (scalar > 10.0)
            {
                var newIndices = id.Select(index => index * 2).ToArray();
                return IndexSet.Create(newIndices);
            }
            return id;
        });

        var genericResult = genericMultivector.MapBasisBlades((id, scalar) =>
        {
            if (scalar > 10.0)
            {
                var newIndices = id.Select(index => index * 2).ToArray();
                return IndexSet.Create(newIndices);
            }
            return id;
        });

        // Assert - e0=5 unchanged, e1=15 becomes e2
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));
        Assert.That(genericResult.GetVectorPart().GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(float64Result.GetVectorPart().GetTermScalarByIndex(0), Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(genericResult.GetVectorPart().GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(15.0).Within(Tolerance));
        Assert.That(float64Result.GetVectorPart().GetTermScalarByIndex(2), Is.EqualTo(15.0).Within(Tolerance));
    }

    #endregion

    #region Edge Cases

    [Test]
    public void MapBasisBlades_IdentityMapping_ShouldProduceIdenticalCopy()
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
        var float64Result = float64Multivector.MapBasisBlades(id => id);
        var genericResult = genericMultivector.MapBasisBlades(id => id);

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
