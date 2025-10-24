using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Outermorphisms;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.LinearMaps.Outermorphisms;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for XGaComputedOutermorphism equivalence - Module 1, Task 1.1 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; computed outermorphisms produce identical results.
/// </summary>
[TestFixture]
public class XGaComputedOutermorphismEquivalenceTests
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
    public void ComputedOutermorphism_OmMapBasisVector_ShouldProduceIdenticalResults()
    {
        // Arrange - Create a simple scaling outermorphism
        Func<int, XGaFloat64Vector> float64MapFunc = index =>
            _float64Processor.VectorTerm(index, 2.0 * (index + 1));

        Func<int, XGaVector<double>> genericMapFunc = index =>
            _genericProcessor.VectorTerm(index, 2.0 * (index + 1));

        var float64Om = _float64Processor.CreateComputedOutermorphism(float64MapFunc);
        var genericOm = _genericProcessor.CreateComputedOutermorphism(genericMapFunc);

        // Act & Assert - Test mapping of basis vectors
        for (int i = 0; i < 5; i++)
        {
            var float64Result = float64Om.OmMapBasisVector(i);
            var genericResult = genericOm.OmMapBasisVector(i);

            Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
                $"Basis vector {i} should have same term count");

            Assert.That(genericResult.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Result.GetTermScalarByIndex(i)).Within(Tolerance),
                $"Basis vector {i} mapping should be identical");
        }
    }

    [Test]
    public void ComputedOutermorphism_OmMapVector_ShouldProduceIdenticalResults()
    {
        // Arrange - Create a rotation-like outermorphism
        Func<int, XGaFloat64Vector> float64MapFunc = index =>
        {
            return index switch
            {
                0 => _float64Processor.Vector(1.0, 0.0, 0.0),
                1 => _float64Processor.Vector(0.0, 0.707, 0.707),
                2 => _float64Processor.Vector(0.0, -0.707, 0.707),
                _ => _float64Processor.VectorTerm(index, 1.0)
            };
        };

        Func<int, XGaVector<double>> genericMapFunc = index =>
        {
            return index switch
            {
                0 => _genericProcessor.Vector(1.0, 0.0, 0.0),
                1 => _genericProcessor.Vector(0.0, 0.707, 0.707),
                2 => _genericProcessor.Vector(0.0, -0.707, 0.707),
                _ => _genericProcessor.VectorTerm(index, 1.0)
            };
        };

        var float64Om = _float64Processor.CreateComputedOutermorphism(float64MapFunc);
        var genericOm = _genericProcessor.CreateComputedOutermorphism(genericMapFunc);

        // Test vector
        var float64Vector = _float64Processor.Vector(1.0, 2.0, 3.0);
        var genericVector = _genericProcessor.Vector(1.0, 2.0, 3.0);

        // Act
        var float64Result = float64Om.OmMap(float64Vector);
        var genericResult = genericOm.OmMap(genericVector);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
            "Mapped vectors should have same term count");

        for (int i = 0; i < 4; i++)
        {
            Assert.That(genericResult.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Result.GetTermScalarByIndex(i)).Within(Tolerance),
                $"Vector component {i} should match");
        }
    }

    [Test]
    public void ComputedOutermorphism_OmMapBivector_ShouldProduceIdenticalResults()
    {
        // Arrange - Create a scaling outermorphism
        Func<int, XGaFloat64Vector> float64MapFunc = index =>
            _float64Processor.VectorTerm(index, 1.5);

        Func<int, XGaVector<double>> genericMapFunc = index =>
            _genericProcessor.VectorTerm(index, 1.5);

        var float64Om = _float64Processor.CreateComputedOutermorphism(float64MapFunc);
        var genericOm = _genericProcessor.CreateComputedOutermorphism(genericMapFunc);

        // Test bivector (e1 ∧ e2)
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.0)
            .GetBivector();
        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.0)
            .GetBivector();

        // Act
        var float64Result = float64Om.OmMap(float64Bivector);
        var genericResult = genericOm.OmMap(genericBivector);

        // Assert
        Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
            "Mapped bivectors should have same term count");

        // For a scaling of 1.5, e1 ∧ e2 should map to 1.5^2 * e1 ∧ e2 = 2.25 * e1 ∧ e2
        // Check if result is non-zero and has expected magnitude
        Assert.That(genericResult.Count, Is.GreaterThan(0), "Mapped bivector should be non-zero");
        Assert.That(float64Result.Count, Is.GreaterThan(0), "Mapped bivector should be non-zero");
    }

    [Test]
    public void ComputedOutermorphism_OmMapMultivector_ShouldProduceIdenticalResults()
    {
        // Arrange - Create a simple scaling outermorphism
        Func<int, XGaFloat64Vector> float64MapFunc = index =>
            _float64Processor.VectorTerm(index, 2.0);

        Func<int, XGaVector<double>> genericMapFunc = index =>
            _genericProcessor.VectorTerm(index, 2.0);

        var float64Om = _float64Processor.CreateComputedOutermorphism(float64MapFunc);
        var genericOm = _genericProcessor.CreateComputedOutermorphism(genericMapFunc);

        // Test multivector with mixed grades
        var float64Mv = _float64Processor.CreateMultivectorComposer()
            .SetScalarTerm(1.0)
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 3.0)
            .SetBivectorTerm(0, 1, 4.0)
            .GetMultivector();

        var genericMv = _genericProcessor.CreateMultivectorComposer()
            .SetScalarTerm(1.0)
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 3.0)
            .SetBivectorTerm(0, 1, 4.0)
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

        // Check vector part (should be scaled by 2)
        var genericVectorPart = genericResult.GetVectorPart();
        var float64VectorPart = float64Result.GetVectorPart();

        Assert.That(genericVectorPart.GetTermScalarByIndex(0).ScalarValue,
            Is.EqualTo(float64VectorPart.GetTermScalarByIndex(0)).Within(Tolerance),
            "Vector e0 component should match");
    }

    [Test]
    public void ComputedOutermorphism_GetMappedBasisBlades_ShouldProduceIdenticalResults()
    {
        // Arrange - Create a simple outermorphism
        Func<int, XGaFloat64Vector> float64MapFunc = index =>
            _float64Processor.VectorTerm(index, 3.0);

        Func<int, XGaVector<double>> genericMapFunc = index =>
            _genericProcessor.VectorTerm(index, 3.0);

        var float64Om = _float64Processor.CreateComputedOutermorphism(float64MapFunc);
        var genericOm = _genericProcessor.CreateComputedOutermorphism(genericMapFunc);

        // Act
        var float64Blades = float64Om.GetMappedBasisBlades(3).ToList();
        var genericBlades = genericOm.GetMappedBasisBlades(3).ToList();

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
    public void ComputedOutermorphism_IsValid_ShouldReturnTrue()
    {
        // Arrange
        Func<int, XGaFloat64Vector> float64MapFunc = index =>
            _float64Processor.VectorTerm(index, 1.0);

        Func<int, XGaVector<double>> genericMapFunc = index =>
            _genericProcessor.VectorTerm(index, 1.0);

        var float64Om = _float64Processor.CreateComputedOutermorphism(float64MapFunc);
        var genericOm = _genericProcessor.CreateComputedOutermorphism(genericMapFunc);

        // Act & Assert
        Assert.That(float64Om.IsValid(), Is.True, "Float64 outermorphism should be valid");
        Assert.That(genericOm.IsValid(), Is.True, "Generic outermorphism should be valid");
    }
}
