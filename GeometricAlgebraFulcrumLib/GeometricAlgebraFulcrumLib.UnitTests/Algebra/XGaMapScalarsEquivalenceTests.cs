using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for MapScalars equivalence - Phase 1.1 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; MapScalars methods produce identical results.
///
/// **Status:** Verifying that existing MapScalars implementations are functionally equivalent.
/// The deduplication task document claimed these methods were missing, but they already exist.
/// These tests validate that the existing implementations produce identical results.
/// </summary>
[TestFixture]
public class XGaMapScalarsEquivalenceTests
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

    #region Scalar MapScalar Tests

    [Test]
    public void Scalar_MapScalar_SimpleFunction_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Scalar = _float64Processor.Scalar(5.0);
        var genericScalar = _genericProcessor.Scalar(5.0);

        // Act - Double each scalar
        var float64Result = float64Scalar.MapScalar(x => x * 2.0);
        var genericResult = genericScalar.MapScalar(x => x * 2.0);

        // Assert
        Assert.That(genericResult.ScalarValue, Is.EqualTo(float64Result.ScalarValue).Within(Tolerance));
        Assert.That(genericResult.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
    }

    [Test]
    public void Scalar_MapScalar_WithIndexSet_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Scalar = _float64Processor.Scalar(3.0);
        var genericScalar = _genericProcessor.Scalar(3.0);

        // Act - Map using IndexSet (though scalar has empty index set)
        var float64Result = float64Scalar.MapScalar((id, x) => x * 3.0);
        var genericResult = genericScalar.MapScalar((id, x) => x * 3.0);

        // Assert
        Assert.That(genericResult.ScalarValue, Is.EqualTo(float64Result.ScalarValue).Within(Tolerance));
        Assert.That(genericResult.ScalarValue, Is.EqualTo(9.0).Within(Tolerance));
    }

    [Test]
    public void Scalar_MapScalar_ZeroScalar_ShouldReturnSameZero()
    {
        // Arrange
        var float64Scalar = _float64Processor.ScalarZero;
        var genericScalar = _genericProcessor.ScalarZero;

        // Act - Mapping zero should short-circuit
        var float64Result = float64Scalar.MapScalar(x => x * 100.0);
        var genericResult = genericScalar.MapScalar(x => x * 100.0);

        // Assert
        Assert.That(genericResult.IsZero, Is.True);
        Assert.That(float64Result.IsZero, Is.True);
        Assert.That(ReferenceEquals(float64Result, float64Scalar), Is.True, "Should return same zero instance");
        Assert.That(ReferenceEquals(genericResult, genericScalar), Is.True, "Should return same zero instance");
    }

    #endregion

    #region Vector MapScalars Tests

    [Test]
    public void Vector_MapScalars_SimpleFunction_ShouldProduceIdenticalResults()
    {
        // Arrange - Create vectors with same components
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 4.0)
            .SetVectorTerm(3, 6.0)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 4.0)
            .SetVectorTerm(3, 6.0)
            .GetVector();

        // Act - Map: multiply each scalar by 1.5
        var float64Result = float64Vector.MapScalars(x => x * 1.5);
        var genericResult = genericVector.MapScalars(x => x * 1.5);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        for (int i = 0; i < 5; i++)
        {
            Assert.That(genericResult.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Result.GetTermScalarByIndex(i)).Within(Tolerance),
                $"Term at index {i} should match");
        }

        // Verify specific values
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(6.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(3).ScalarValue, Is.EqualTo(9.0).Within(Tolerance));
    }

    [Test]
    public void Vector_MapScalars_WithIndexSet_ShouldProduceIdenticalResults()
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

        // Act - Map using IndexSet: add index count to scalar
        var float64Result = float64Vector.MapScalars((id, x) => x + id.FirstIndex);
        var genericResult = genericVector.MapScalars((id, x) => x + id.FirstIndex);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        // Result should be: [1+0, 2+1, 3+2] = [1, 3, 5]
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(1), Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(2), Is.EqualTo(5.0).Within(Tolerance));
    }

    [Test]
    public void Vector_MapScalars_WithIntIndex_ShouldProduceIdenticalResults()
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

        // Act - Map using int index: multiply by (index + 1)
        var float64Result = float64Vector.MapScalars((index, x) => x * (index + 1));
        var genericResult = genericVector.MapScalars((index, x) => x * (index + 1));

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        // Result should be: [10*1, 20*2, 30*3] = [10, 40, 90]
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(40.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(1), Is.EqualTo(40.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(90.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(2), Is.EqualTo(90.0).Within(Tolerance));
    }

    [Test]
    public void Vector_MapScalars_ZeroVector_ShouldReturnSameZero()
    {
        // Arrange
        var float64Vector = _float64Processor.VectorZero;
        var genericVector = _genericProcessor.VectorZero;

        // Act
        var float64Result = float64Vector.MapScalars(x => x * 100.0);
        var genericResult = genericVector.MapScalars(x => x * 100.0);

        // Assert
        Assert.That(genericResult.IsZero, Is.True);
        Assert.That(float64Result.IsZero, Is.True);
        Assert.That(ReferenceEquals(float64Result, float64Vector), Is.True);
        Assert.That(ReferenceEquals(genericResult, genericVector), Is.True);
    }

    #endregion

    #region Bivector MapScalars Tests

    [Test]
    public void Bivector_MapScalars_SimpleFunction_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.0)
            .SetBivectorTerm(0, 2, 4.0)
            .SetBivectorTerm(1, 2, 6.0)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.0)
            .SetBivectorTerm(0, 2, 4.0)
            .SetBivectorTerm(1, 2, 6.0)
            .GetBivector();

        // Act - Map: square each scalar
        var float64Result = float64Bivector.MapScalars(x => x * x);
        var genericResult = genericBivector.MapScalars(x => x * x);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        var id_01 = IndexSet.Create(0, 1);
        var id_02 = IndexSet.Create(0, 2);
        var id_12 = IndexSet.Create(1, 2);

        Assert.That(genericResult.GetBasisBladeScalar(id_01).ScalarValue,
            Is.EqualTo(float64Result.GetBasisBladeScalar(id_01)).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_01).ScalarValue, Is.EqualTo(4.0).Within(Tolerance));

        Assert.That(genericResult.GetBasisBladeScalar(id_02).ScalarValue,
            Is.EqualTo(float64Result.GetBasisBladeScalar(id_02)).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_02).ScalarValue, Is.EqualTo(16.0).Within(Tolerance));

        Assert.That(genericResult.GetBasisBladeScalar(id_12).ScalarValue,
            Is.EqualTo(float64Result.GetBasisBladeScalar(id_12)).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_12).ScalarValue, Is.EqualTo(36.0).Within(Tolerance));
    }

    [Test]
    public void Bivector_MapScalars_WithIndexSet_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 10.0)
            .SetBivectorTerm(1, 2, 20.0)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 10.0)
            .SetBivectorTerm(1, 2, 20.0)
            .GetBivector();

        // Act - Map using IndexSet: divide by index count
        var float64Result = float64Bivector.MapScalars((id, x) => x / id.Count);
        var genericResult = genericBivector.MapScalars((id, x) => x / id.Count);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        var id_01 = IndexSet.Create(0, 1);
        var id_12 = IndexSet.Create(1, 2);

        // Both should be divided by 2 (count of indices in bivector)
        Assert.That(genericResult.GetBasisBladeScalar(id_01).ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_01), Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_12).ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_12), Is.EqualTo(10.0).Within(Tolerance));
    }

    [Test]
    public void Bivector_MapScalars_WithIntIndices_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 100.0)
            .SetBivectorTerm(1, 2, 200.0)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 100.0)
            .SetBivectorTerm(1, 2, 200.0)
            .GetBivector();

        // Act - Map using int indices: add (i1 + i2) to scalar
        var float64Result = float64Bivector.MapScalars((i1, i2, x) => x + i1 + i2);
        var genericResult = genericBivector.MapScalars((i1, i2, x) => x + i1 + i2);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        var id_01 = IndexSet.Create(0, 1);
        var id_12 = IndexSet.Create(1, 2);

        // [0,1]: 100 + 0 + 1 = 101
        // [1,2]: 200 + 1 + 2 = 203
        Assert.That(genericResult.GetBasisBladeScalar(id_01).ScalarValue, Is.EqualTo(101.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_01), Is.EqualTo(101.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_12).ScalarValue, Is.EqualTo(203.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_12), Is.EqualTo(203.0).Within(Tolerance));
    }

    #endregion

    #region HigherKVector MapScalars Tests

    [Test]
    public void HigherKVector_MapScalars_Trivector_ShouldProduceIdenticalResults()
    {
        // Arrange - Create grade-3 k-vectors (trivectors)
        var id_012 = IndexSet.Create(0, 1, 2);
        var id_013 = IndexSet.Create(0, 1, 3);

        var float64KVector = _float64Processor.CreateKVectorComposer(3)
            .SetTerm(id_012, 5.0)
            .SetTerm(id_013, 10.0)
            .GetHigherKVector();

        var genericKVector = _genericProcessor.CreateKVectorComposer(3)
            .SetTrivectorTerm(0, 1, 2, 5.0)
            .SetTrivectorTerm(0, 1, 3, 10.0)
            .GetHigherKVector();

        // Act - Map: negate all scalars
        var float64Result = float64KVector.MapScalars(x => -x);
        var genericResult = genericKVector.MapScalars(x => -x);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));
        Assert.That(genericResult.Grade, Is.EqualTo(3));
        Assert.That(float64Result.Grade, Is.EqualTo(3));

        Assert.That(genericResult.GetBasisBladeScalar(id_012).ScalarValue, Is.EqualTo(-5.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_012), Is.EqualTo(-5.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_013).ScalarValue, Is.EqualTo(-10.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_013), Is.EqualTo(-10.0).Within(Tolerance));
    }

    [Test]
    public void HigherKVector_MapScalars_WithIndexSet_ShouldProduceIdenticalResults()
    {
        // Arrange
        var id_012 = IndexSet.Create(0, 1, 2);
        var id_123 = IndexSet.Create(1, 2, 3);

        var float64KVector = _float64Processor.CreateKVectorComposer(3)
            .SetTerm(id_012, 3.0)
            .SetTerm(id_123, 6.0)
            .GetHigherKVector();

        var genericKVector = _genericProcessor.CreateKVectorComposer(3)
            .SetTrivectorTerm(0, 1, 2, 3.0)
            .SetTrivectorTerm(1, 2, 3, 6.0)
            .GetHigherKVector();

        // Act - Map: multiply by grade
        var float64Result = float64KVector.MapScalars((id, x) => x * id.Count);
        var genericResult = genericKVector.MapScalars((id, x) => x * id.Count);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        // Both should be multiplied by 3 (grade)
        Assert.That(genericResult.GetBasisBladeScalar(id_012).ScalarValue, Is.EqualTo(9.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_012), Is.EqualTo(9.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_123).ScalarValue, Is.EqualTo(18.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_123), Is.EqualTo(18.0).Within(Tolerance));
    }

    #endregion

    #region KVector MapScalars Tests

    [Test]
    public void KVector_MapScalars_Grade2_ShouldProduceIdenticalResults()
    {
        // Arrange
        var id_01 = IndexSet.Create(0, 1);
        var id_12 = IndexSet.Create(1, 2);

        var float64KVector = _float64Processor.CreateKVectorComposer(2)
            .SetBivectorTerm(0, 1, 7.0)
            .SetBivectorTerm(1, 2, 14.0)
            .GetKVector();

        var genericKVector = _genericProcessor.CreateKVectorComposer(2)
            .SetBivectorTerm(0, 1, 7.0)
            .SetBivectorTerm(1, 2, 14.0)
            .GetKVector();

        // Act - Map: divide by 7
        var float64Result = float64KVector.MapScalars(x => x / 7.0);
        var genericResult = genericKVector.MapScalars(x => x / 7.0);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));
        Assert.That(genericResult.Grade, Is.EqualTo(2));

        Assert.That(genericResult.GetBasisBladeScalar(id_01).ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_01), Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_12).ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_12), Is.EqualTo(2.0).Within(Tolerance));
    }

    [Test]
    public void KVector_MapScalars_WithIndexSet_ShouldProduceIdenticalResults()
    {
        // Arrange
        var id_0 = IndexSet.Create(0);
        var id_1 = IndexSet.Create(1);

        var float64KVector = _float64Processor.CreateKVectorComposer(1)
            .SetVectorTerm(0, 100.0)
            .SetVectorTerm(1, 200.0)
            .GetKVector();

        var genericKVector = _genericProcessor.CreateKVectorComposer(1)
            .SetVectorTerm(0, 100.0)
            .SetVectorTerm(1, 200.0)
            .GetKVector();

        // Act - Map using IndexSet
        var float64Result = float64KVector.MapScalars((id, x) => x + id.FirstIndex * 10);
        var genericResult = genericKVector.MapScalars((id, x) => x + id.FirstIndex * 10);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        // [0]: 100 + 0*10 = 100
        // [1]: 200 + 1*10 = 210
        Assert.That(genericResult.GetBasisBladeScalar(id_0).ScalarValue, Is.EqualTo(100.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_0), Is.EqualTo(100.0).Within(Tolerance));
        Assert.That(genericResult.GetBasisBladeScalar(id_1).ScalarValue, Is.EqualTo(210.0).Within(Tolerance));
        Assert.That(float64Result.GetBasisBladeScalar(id_1), Is.EqualTo(210.0).Within(Tolerance));
    }

    #endregion

    #region Multivector MapScalars Tests

    [Test]
    public void Multivector_MapScalars_MixedGrades_ShouldProduceIdenticalResults()
    {
        // Arrange - Create multivector with scalar, vector, and bivector parts
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

        // Act - Map: add 10 to each scalar
        var float64Result = float64Multivector.MapScalars(x => x + 10.0);
        var genericResult = genericMultivector.MapScalars(x => x + 10.0);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));
        Assert.That(genericResult.Count, Is.EqualTo(4));

        // Check all terms
        Assert.That(genericResult.Scalar().ScalarValue, Is.EqualTo(11.0).Within(Tolerance));
        Assert.That(float64Result.Scalar(), Is.EqualTo(11.0).Within(Tolerance));

        Assert.That(genericResult.GetVectorPart().GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(12.0).Within(Tolerance));
        Assert.That(float64Result.GetVectorPart().GetTermScalarByIndex(0), Is.EqualTo(12.0).Within(Tolerance));

        Assert.That(genericResult.GetVectorPart().GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(13.0).Within(Tolerance));
        Assert.That(float64Result.GetVectorPart().GetTermScalarByIndex(1), Is.EqualTo(13.0).Within(Tolerance));

        var id_01 = IndexSet.Create(0, 1);
        Assert.That(genericResult.GetBivectorPart().GetBasisBladeScalar(id_01).ScalarValue, Is.EqualTo(14.0).Within(Tolerance));
        Assert.That(float64Result.GetBivectorPart().GetBasisBladeScalar(id_01), Is.EqualTo(14.0).Within(Tolerance));
    }

    [Test]
    public void Multivector_MapScalars_WithIndexSet_ShouldProduceIdenticalResults()
    {
        // Arrange
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

        // Act - Map: multiply by (grade + 1) where grade = id.Count
        var float64Result = float64Multivector.MapScalars((id, x) => x * (id.Count + 1));
        var genericResult = genericMultivector.MapScalars((id, x) => x * (id.Count + 1));

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count));

        // Scalar (grade 0): 10 * (0+1) = 10
        Assert.That(genericResult.Scalar().ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(float64Result.Scalar(), Is.EqualTo(10.0).Within(Tolerance));

        // Vector (grade 1): 20 * (1+1) = 40
        Assert.That(genericResult.GetVectorPart().GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(40.0).Within(Tolerance));
        Assert.That(float64Result.GetVectorPart().GetTermScalarByIndex(0), Is.EqualTo(40.0).Within(Tolerance));

        // Bivector (grade 2): 30 * (2+1) = 90
        var id_01 = IndexSet.Create(0, 1);
        Assert.That(genericResult.GetBivectorPart().GetBasisBladeScalar(id_01).ScalarValue, Is.EqualTo(90.0).Within(Tolerance));
        Assert.That(float64Result.GetBivectorPart().GetBasisBladeScalar(id_01), Is.EqualTo(90.0).Within(Tolerance));
    }

    #endregion

    #region Edge Cases

    [Test]
    public void MapScalars_ComplexFunction_ShouldProduceIdenticalResults()
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

        // Act - Complex mapping: x^2 + 2x + 1
        var float64Result = float64Vector.MapScalars(x => x * x + 2 * x + 1);
        var genericResult = genericVector.MapScalars(x => x * x + 2 * x + 1);

        // Assert
        // [0]: 1^2 + 2*1 + 1 = 4
        // [1]: 2^2 + 2*2 + 1 = 9
        // [2]: 3^2 + 2*3 + 1 = 16
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(4.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(4.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(9.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(1), Is.EqualTo(9.0).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(16.0).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(2), Is.EqualTo(16.0).Within(Tolerance));
    }

    [Test]
    public void MapScalars_IdentityFunction_ShouldProduceIdenticalCopy()
    {
        // Arrange
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 5.5)
            .SetVectorTerm(2, 7.7)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 5.5)
            .SetVectorTerm(2, 7.7)
            .GetVector();

        // Act - Identity mapping
        var float64Result = float64Vector.MapScalars(x => x);
        var genericResult = genericVector.MapScalars(x => x);

        // Assert - Should be identical to original
        Assert.That(genericResult.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(5.5).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(0), Is.EqualTo(5.5).Within(Tolerance));
        Assert.That(genericResult.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(7.7).Within(Tolerance));
        Assert.That(float64Result.GetTermScalarByIndex(2), Is.EqualTo(7.7).Within(Tolerance));
    }

    #endregion
}
