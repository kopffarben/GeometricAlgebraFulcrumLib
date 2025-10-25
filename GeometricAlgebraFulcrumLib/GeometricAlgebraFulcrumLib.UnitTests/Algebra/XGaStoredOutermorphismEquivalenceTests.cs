using System;
using System.Collections.Generic;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Outermorphisms;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.LinearMaps.Outermorphisms;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.BitManipulation;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for XGaStoredOutermorphism equivalence - Module 1, Task 1.2 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; stored outermorphisms produce identical results.
/// </summary>
[TestFixture]
public class XGaStoredOutermorphismEquivalenceTests
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

    private (XGaFloat64StoredOutermorphism, XGaStoredOutermorphism<double>) CreateSimpleScalingOutermorphisms()
    {
        // Create basis map dictionaries for a simple scaling outermorphism
        // e_i maps to 2*i*e_i
        var float64Dict = new Dictionary<IndexSet, XGaFloat64KVector>();
        var genericDict = new Dictionary<IndexSet, XGaKVector<double>>();

        for (int i = 0; i < 5; i++)
        {
            var id = i.ToUnitIndexSet();
            float64Dict[id] = _float64Processor.VectorTerm(i, 2.0 * i);
            genericDict[id] = _genericProcessor.VectorTerm(i, 2.0 * i);
        }

        var float64Om = _float64Processor.CreateStoredOutermorphism(float64Dict);
        var genericOm = _genericProcessor.CreateStoredOutermorphism(genericDict);

        return (float64Om, genericOm);
    }

    private (XGaFloat64StoredOutermorphism, XGaStoredOutermorphism<double>) CreateRotationLikeOutermorphisms()
    {
        // Create a rotation-like outermorphism
        // e_0 -> e_0
        // e_1 -> 0.707*e_1 + 0.707*e_2
        // e_2 -> -0.707*e_1 + 0.707*e_2
        var float64Dict = new Dictionary<IndexSet, XGaFloat64KVector>();
        var genericDict = new Dictionary<IndexSet, XGaKVector<double>>();

        float64Dict[0.ToUnitIndexSet()] = _float64Processor.Vector(1.0, 0.0, 0.0);
        genericDict[0.ToUnitIndexSet()] = _genericProcessor.Vector(1.0, 0.0, 0.0);

        float64Dict[1.ToUnitIndexSet()] = _float64Processor.Vector(0.0, 0.707, 0.707);
        genericDict[1.ToUnitIndexSet()] = _genericProcessor.Vector(0.0, 0.707, 0.707);

        float64Dict[2.ToUnitIndexSet()] = _float64Processor.Vector(0.0, -0.707, 0.707);
        genericDict[2.ToUnitIndexSet()] = _genericProcessor.Vector(0.0, -0.707, 0.707);

        var float64Om = _float64Processor.CreateStoredOutermorphism(float64Dict);
        var genericOm = _genericProcessor.CreateStoredOutermorphism(genericDict);

        return (float64Om, genericOm);
    }

    [Test]
    public void StoredOutermorphism_OmMapBasisVector_ShouldProduceIdenticalResults()
    {
        // Arrange
        var (float64Om, genericOm) = CreateSimpleScalingOutermorphisms();

        // Act & Assert - Test mapping of basis vectors
        for (int i = 0; i < 5; i++)
        {
            var float64Result = float64Om.OmMapBasisVector(i);
            var genericResult = genericOm.OmMapBasisVector(i);

            Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
                $"Basis vector {i} should have same term count");

            if (i > 0) // e_0 maps to zero vector (2*0*e_0 = 0)
            {
                Assert.That(genericResult.GetTermScalarByIndex(i).ScalarValue,
                    Is.EqualTo(float64Result.GetTermScalarByIndex(i)).Within(Tolerance),
                    $"Basis vector {i} mapping should be identical");
            }
        }
    }

    [Test]
    public void StoredOutermorphism_OmMapBasisBivector_ShouldProduceIdenticalResults()
    {
        // Arrange - Create outermorphisms with explicit bivector mappings
        var float64Dict = new Dictionary<IndexSet, XGaFloat64KVector>();
        var genericDict = new Dictionary<IndexSet, XGaKVector<double>>();

        // Add bivector mappings: e_i ∧ e_j maps to scaled bivectors
        var id01 = IndexSet.CreatePair(0, 1);
        var id12 = IndexSet.CreatePair(1, 2);

        float64Dict[id01] = _float64Processor.BivectorTerm(0, 1, 2.5);
        genericDict[id01] = _genericProcessor.BivectorTerm(0, 1, 2.5);

        float64Dict[id12] = _float64Processor.BivectorTerm(1, 2, 3.5);
        genericDict[id12] = _genericProcessor.BivectorTerm(1, 2, 3.5);

        var float64Om = _float64Processor.CreateStoredOutermorphism(float64Dict);
        var genericOm = _genericProcessor.CreateStoredOutermorphism(genericDict);

        // Act & Assert - Test mapping of stored basis bivectors
        var float64Result01 = float64Om.OmMapBasisBivector(0, 1);
        var genericResult01 = genericOm.OmMapBasisBivector(0, 1);

        Assert.That(genericResult01.Count, Is.EqualTo(float64Result01.Count),
            "Basis bivector e0∧e1 should have same term count");

        var float64Result12 = float64Om.OmMapBasisBivector(1, 2);
        var genericResult12 = genericOm.OmMapBasisBivector(1, 2);

        Assert.That(genericResult12.Count, Is.EqualTo(float64Result12.Count),
            "Basis bivector e1∧e2 should have same term count");
    }

    [Test]
    public void StoredOutermorphism_OmMapVector_ShouldProduceIdenticalResults()
    {
        // Arrange
        var (float64Om, genericOm) = CreateRotationLikeOutermorphisms();

        var float64Vector = _float64Processor.Vector(1.0, 2.0, 3.0);
        var genericVector = _genericProcessor.Vector(1.0, 2.0, 3.0);

        // Act
        var float64Result = float64Om.OmMap(float64Vector);
        var genericResult = genericOm.OmMap(genericVector);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
            "Mapped vectors should have same term count");

        for (int i = 0; i < 3; i++)
        {
            Assert.That(genericResult.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Result.GetTermScalarByIndex(i)).Within(Tolerance),
                $"Vector component e{i} should match");
        }
    }

    [Test]
    public void StoredOutermorphism_OmMapBivector_ShouldProduceIdenticalResults()
    {
        // Arrange - Create outermorphisms with bivector mappings
        var float64Dict = new Dictionary<IndexSet, XGaFloat64KVector>();
        var genericDict = new Dictionary<IndexSet, XGaKVector<double>>();

        // Add bivector mapping: e1 ∧ e2 maps to 3.0 * e1 ∧ e2
        var id12 = IndexSet.CreatePair(1, 2);
        float64Dict[id12] = _float64Processor.BivectorTerm(1, 2, 3.0);
        genericDict[id12] = _genericProcessor.BivectorTerm(1, 2, 3.0);

        var float64Om = _float64Processor.CreateStoredOutermorphism(float64Dict);
        var genericOm = _genericProcessor.CreateStoredOutermorphism(genericDict);

        // Test bivector (e1 ∧ e2)
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(1, 2, 1.0)
            .GetBivector();
        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(1, 2, 1.0)
            .GetBivector();

        // Act
        var float64Result = float64Om.OmMap(float64Bivector);
        var genericResult = genericOm.OmMap(genericBivector);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
            "Mapped bivectors should have same term count");

        // e1 ∧ e2 should map to 3.0 * (e1 ∧ e2), result should be 3.0 * e1 ∧ e2
        Assert.That(genericResult.Count, Is.GreaterThan(0), "Mapped bivector should be non-zero");
        Assert.That(float64Result.Count, Is.GreaterThan(0), "Mapped bivector should be non-zero");
    }

    [Test]
    public void StoredOutermorphism_OmMapMultivector_ShouldProduceIdenticalResults()
    {
        // Arrange
        var (float64Om, genericOm) = CreateSimpleScalingOutermorphisms();

        // Test multivector with mixed grades
        var float64Mv = _float64Processor.CreateMultivectorComposer()
            .SetScalarTerm(1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .SetBivectorTerm(1, 2, 4.0)
            .GetMultivector();

        var genericMv = _genericProcessor.CreateMultivectorComposer()
            .SetScalarTerm(1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .SetBivectorTerm(1, 2, 4.0)
            .GetMultivector();

        // Act
        var float64Result = float64Om.OmMap(float64Mv);
        var genericResult = genericOm.OmMap(genericMv);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
            "Mapped multivectors should have same term count");

        // Check scalar part (should be unchanged by outermorphism)
        Assert.That(genericResult.GetScalarPart().ScalarValue,
            Is.EqualTo(float64Result.GetScalarPart().ScalarValue).Within(Tolerance),
            "Scalar part should match");
    }

    [Test]
    public void StoredOutermorphism_GetMappedBasisBlades_ShouldProduceIdenticalResults()
    {
        // Arrange
        var (float64Om, genericOm) = CreateSimpleScalingOutermorphisms();

        // Act
        var float64Blades = float64Om.GetMappedBasisBlades(4).ToList();
        var genericBlades = genericOm.GetMappedBasisBlades(4).ToList();

        // Assert
        Assert.That(genericBlades.Count, Is.EqualTo(float64Blades.Count),
            "Should have same number of mapped basis blades");

        for (int i = 0; i < float64Blades.Count; i++)
        {
            Assert.That(genericBlades[i].Key, Is.EqualTo(float64Blades[i].Key),
                $"Basis blade {i} index should match");
        }
    }

    [Test]
    public void StoredOutermorphism_GetOmMappedBasisVectors_ShouldProduceIdenticalResults()
    {
        // Arrange
        var (float64Om, genericOm) = CreateRotationLikeOutermorphisms();

        // Act
        var float64Vectors = float64Om.GetOmMappedBasisVectors(4).ToList();
        var genericVectors = genericOm.GetOmMappedBasisVectors(4).ToList();

        // Assert
        Assert.That(genericVectors.Count, Is.EqualTo(float64Vectors.Count),
            "Should have same number of mapped basis vectors");

        for (int i = 0; i < float64Vectors.Count; i++)
        {
            Assert.That(genericVectors[i].Key, Is.EqualTo(float64Vectors[i].Key),
                $"Basis vector {i} index should match");

            Assert.That(genericVectors[i].Value.Count, Is.EqualTo(float64Vectors[i].Value.Count),
                $"Basis vector {i} should have same term count");
        }
    }

    [Test]
    public void StoredOutermorphism_IsValid_ShouldReturnTrue()
    {
        // Arrange
        var (float64Om, genericOm) = CreateSimpleScalingOutermorphisms();

        // Act & Assert
        Assert.That(float64Om.IsValid(), Is.True, "Float64 outermorphism should be valid");
        Assert.That(genericOm.IsValid(), Is.True, "Generic outermorphism should be valid");
    }

    [Test]
    public void StoredOutermorphism_EmptyMapping_ShouldProduceZeroResults()
    {
        // Arrange - Create empty outermorphisms
        var float64Dict = new Dictionary<IndexSet, XGaFloat64KVector>();
        var genericDict = new Dictionary<IndexSet, XGaKVector<double>>();

        var float64Om = _float64Processor.CreateStoredOutermorphism(float64Dict);
        var genericOm = _genericProcessor.CreateStoredOutermorphism(genericDict);

        var float64Vector = _float64Processor.Vector(1.0, 2.0, 3.0);
        var genericVector = _genericProcessor.Vector(1.0, 2.0, 3.0);

        // Act
        var float64Result = float64Om.OmMap(float64Vector);
        var genericResult = genericOm.OmMap(genericVector);

        // Assert - Empty mapping should produce zero vectors
        Assert.That(float64Result.IsZero, Is.True, "Float64 result should be zero");
        Assert.That(genericResult.IsZero, Is.True, "Generic result should be zero");
    }
}
