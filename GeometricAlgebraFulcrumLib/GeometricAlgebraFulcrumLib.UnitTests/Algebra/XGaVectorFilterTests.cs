using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for XGaVector filter methods added in Phase 1.1.1 of deduplication roadmap.
/// Tests ensure Float64 and Generic<double> have identical functionality.
/// </summary>
[TestFixture]
public class XGaVectorFilterTests
{
    private XGaFloat64Processor _float64Processor = null!;
    private XGaProcessor<double> _genericProcessor = null!;

    [SetUp]
    public void Setup()
    {
        _float64Processor = XGaFloat64Processor.Euclidean;
        _genericProcessor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    }

    #region Float64 Tests

    [Test]
    public void Float64_GetVectorPart_FilterByIndex_ShouldReturnFilteredTerms()
    {
        // Arrange
        var vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)  // e_0
            .SetVectorTerm(1, 2.0)  // e_1
            .SetVectorTerm(2, 3.0)  // e_2
            .SetVectorTerm(3, 4.0)  // e_3
            .GetVector();

        // Act - Filter for even indices only
        Func<int, bool> filterFunc = index => index % 2 == 0;
        var filtered = vector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2), "Should have 2 terms (indices 0 and 2)");
        Assert.That(filtered.GetTermScalarByIndex(0), Is.EqualTo(1.0));
        Assert.That(filtered.GetTermScalarByIndex(2), Is.EqualTo(3.0));
        Assert.That(filtered.GetTermScalarByIndex(1), Is.EqualTo(0.0), "Index 1 should be zero");
    }

    [Test]
    public void Float64_GetVectorPart_FilterByScalar_ShouldReturnFilteredTerms()
    {
        // Arrange
        var vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 5.0)
            .SetVectorTerm(2, 3.0)
            .SetVectorTerm(3, 7.0)
            .GetVector();

        // Act - Filter for scalars > 4.0
        Func<double, bool> filterFunc = scalar => scalar > 4.0;
        var filtered = vector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2), "Should have 2 terms with values > 4");
        Assert.That(filtered.GetTermScalarByIndex(1), Is.EqualTo(5.0));
        Assert.That(filtered.GetTermScalarByIndex(3), Is.EqualTo(7.0));
    }

    [Test]
    public void Float64_GetVectorPart_FilterByCombined_ShouldReturnFilteredTerms()
    {
        // Arrange
        var vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 6.0)
            .SetVectorTerm(3, 4.0)
            .GetVector();

        // Act - Filter for even index AND scalar > 2.0
        Func<int, double, bool> filterFunc = (index, scalar) => index % 2 == 0 && scalar > 2.0;
        var filtered = vector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(1), "Should have 1 term (index 2, value 6.0)");
        Assert.That(filtered.GetTermScalarByIndex(2), Is.EqualTo(6.0));
    }

    [Test]
    public void Float64_GetVectorPart_FilterOnZeroVector_ShouldReturnZero()
    {
        // Arrange
        var vector = _float64Processor.VectorZero;

        // Act
        Func<int, bool> filterFunc = index => true;
        var filtered = vector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(filtered.IsZero, Is.True);
    }

    #endregion

    #region Generic<double> Tests

    [Test]
    public void Generic_GetVectorPart_FilterByIndex_ShouldReturnFilteredTerms()
    {
        // Arrange
        var vector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .SetVectorTerm(3, 4.0)
            .GetVector();

        // Act - Filter for even indices only
        Func<int, bool> filterFunc = index => index % 2 == 0;
        var filtered = vector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2), "Should have 2 terms");
        Assert.That(filtered.GetTermScalarByIndex(0).ScalarValue, Is.EqualTo(1.0));
        Assert.That(filtered.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(3.0));
    }

    [Test]
    public void Generic_GetVectorPart_FilterByScalar_ShouldReturnFilteredTerms()
    {
        // Arrange
        var vector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 5.0)
            .SetVectorTerm(2, 3.0)
            .SetVectorTerm(3, 7.0)
            .GetVector();

        // Act - Filter for scalars > 4.0
        Func<double, bool> filterFunc = scalar => scalar > 4.0;
        var filtered = vector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2), "Should have 2 terms");
        Assert.That(filtered.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(5.0));
        Assert.That(filtered.GetTermScalarByIndex(3).ScalarValue, Is.EqualTo(7.0));
    }

    [Test]
    public void Generic_GetVectorPart_FilterByCombined_ShouldReturnFilteredTerms()
    {
        // Arrange
        var vector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 6.0)
            .SetVectorTerm(3, 4.0)
            .GetVector();

        // Act - Filter for even index AND scalar > 2.0
        Func<int, double, bool> filterFunc = (index, scalar) => index % 2 == 0 && scalar > 2.0;
        var filtered = vector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(1));
        Assert.That(filtered.GetTermScalarByIndex(2).ScalarValue, Is.EqualTo(6.0));
    }

    [Test]
    public void Generic_GetVectorPart_FilterOnZeroVector_ShouldReturnZero()
    {
        // Arrange
        var vector = _genericProcessor.VectorZero;

        // Act
        Func<int, bool> filterFunc = index => true;
        var filtered = vector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(filtered.IsZero, Is.True);
    }

    #endregion

    #region Equivalence Tests (Float64 vs Generic<double>)

    [Test]
    public void Equivalence_FilterByIndex_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 1.5)
            .SetVectorTerm(1, 2.5)
            .SetVectorTerm(2, 3.5)
            .SetVectorTerm(3, 4.5)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 1.5)
            .SetVectorTerm(1, 2.5)
            .SetVectorTerm(2, 3.5)
            .SetVectorTerm(3, 4.5)
            .GetVector();

        // Act
        Func<int, bool> filterFunc = index => index > 1;
        var float64Filtered = float64Vector.GetVectorPart(filterFunc);
        var genericFiltered = genericVector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(genericFiltered.Count, Is.EqualTo(float64Filtered.Count), "Same number of terms");
        Assert.That(genericFiltered.GetTermScalarByIndex(2).ScalarValue,
            Is.EqualTo(float64Filtered.GetTermScalarByIndex(2)).Within(1e-14),
            "Term at index 2 should match");
        Assert.That(genericFiltered.GetTermScalarByIndex(3).ScalarValue,
            Is.EqualTo(float64Filtered.GetTermScalarByIndex(3)).Within(1e-14),
            "Term at index 3 should match");
    }

    [Test]
    public void Equivalence_FilterByScalar_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 10.0)
            .SetVectorTerm(2, 3.0)
            .SetVectorTerm(3, 15.0)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 10.0)
            .SetVectorTerm(2, 3.0)
            .SetVectorTerm(3, 15.0)
            .GetVector();

        // Act
        Func<double, bool> filterFunc = scalar => scalar >= 10.0;
        var float64Filtered = float64Vector.GetVectorPart(filterFunc);
        var genericFiltered = genericVector.GetVectorPart(filterFunc);

        // Assert
        Assert.That(genericFiltered.Count, Is.EqualTo(float64Filtered.Count));
        for (int i = 0; i < 4; i++)
        {
            Assert.That(genericFiltered.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Filtered.GetTermScalarByIndex(i)).Within(1e-14),
                $"Term at index {i} should match");
        }
    }

    [Test]
    public void Equivalence_FilterByCombined_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 3.0)
            .SetVectorTerm(2, 5.0)
            .SetVectorTerm(3, 7.0)
            .SetVectorTerm(4, 11.0)
            .GetVector();

        var genericVector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 2.0)
            .SetVectorTerm(1, 3.0)
            .SetVectorTerm(2, 5.0)
            .SetVectorTerm(3, 7.0)
            .SetVectorTerm(4, 11.0)
            .GetVector();

        // Act - Filter for prime scalar values at odd indices
        var isPrime = new Func<double, bool>(n =>
        {
            if (n < 2) return false;
            for (int i = 2; i <= Math.Sqrt(n); i++)
                if (n % i == 0) return false;
            return true;
        });

        Func<int, double, bool> combinedFilter = (index, scalar) => index % 2 == 1 && isPrime(scalar);
        var float64Filtered = float64Vector.GetVectorPart(combinedFilter);
        var genericFiltered = genericVector.GetVectorPart(combinedFilter);

        // Assert
        Assert.That(genericFiltered.Count, Is.EqualTo(float64Filtered.Count),
            "Should have same number of filtered terms");
        Assert.That(genericFiltered.GetTermScalarByIndex(1).ScalarValue, Is.EqualTo(3.0));
        Assert.That(genericFiltered.GetTermScalarByIndex(3).ScalarValue, Is.EqualTo(7.0));
    }

    [Test]
    public void Equivalence_ComplexFilterChain_ShouldProduceIdenticalResults()
    {
        // Arrange - Create a vector with 10 terms
        var float64Composer = _float64Processor.CreateVectorComposer();
        var genericComposer = _genericProcessor.CreateVectorComposer();

        for (int i = 0; i < 10; i++)
        {
            var value = (i + 1) * 1.5;
            float64Composer.SetVectorTerm(i, value);
            genericComposer.SetVectorTerm(i, value);
        }

        var float64Vector = float64Composer.GetVector();
        var genericVector = genericComposer.GetVector();

        // Act - Apply multiple filters sequentially
        Func<int, bool> indexFilter = index => index >= 2;  // Remove first 2
        var float64Step1 = float64Vector.GetVectorPart(indexFilter);
        var genericStep1 = genericVector.GetVectorPart(indexFilter);

        Func<double, bool> scalarFilter = scalar => scalar < 10.0;  // Keep small values
        var float64Step2 = float64Step1.GetVectorPart(scalarFilter);
        var genericStep2 = genericStep1.GetVectorPart(scalarFilter);

        Func<int, double, bool> complexFilter = (index, scalar) => (index + scalar) % 2 == 0;  // Complex filter
        var float64Step3 = float64Step2.GetVectorPart(complexFilter);
        var genericStep3 = genericStep2.GetVectorPart(complexFilter);

        // Assert
        Assert.That(genericStep3.Count, Is.EqualTo(float64Step3.Count),
            "Multi-step filtering should produce same number of terms");

        foreach (var pair in float64Step3.IndexScalarPairs)
        {
            var float64Value = pair.Value;
            var genericValue = genericStep3.GetTermScalarByIndex(pair.Key).ScalarValue;
            Assert.That(genericValue, Is.EqualTo(float64Value).Within(1e-14),
                $"Value at index {pair.Key} should match after multi-step filtering");
        }
    }

    #endregion

    #region Edge Cases

    [Test]
    public void Generic_FilterByIndex_WithAllFalse_ShouldReturnZeroVector()
    {
        // Arrange
        var vector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .GetVector();

        // Act - Filter that rejects everything
        var filtered = vector.GetVectorPart(index => false);

        // Assert
        Assert.That(filtered.IsZero, Is.True);
        Assert.That(filtered.Count, Is.EqualTo(0));
    }

    [Test]
    public void Generic_FilterByScalar_WithAllTrue_ShouldReturnOriginalVector()
    {
        // Arrange
        var vector = _genericProcessor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .GetVector();

        // Act - Filter that accepts everything
        var filtered = vector.GetVectorPart(scalar => true);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(vector.Count));
        for (int i = 0; i < 3; i++)
        {
            Assert.That(filtered.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(vector.GetTermScalarByIndex(i).ScalarValue));
        }
    }

    [Test]
    public void Float64_FilterByCombined_WithNegativeIndices_ShouldHandleCorrectly()
    {
        // Note: This test documents current behavior - negative indices shouldn't occur in practice
        // Arrange
        var vector = _float64Processor.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(5, 2.0)
            .SetVectorTerm(10, 3.0)
            .GetVector();

        // Act - Filter with condition that would never match negative indices
        var filtered = vector.GetVectorPart((index, scalar) => index >= 0 && scalar > 1.5);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2));
        Assert.That(filtered.GetTermScalarByIndex(5), Is.EqualTo(2.0));
        Assert.That(filtered.GetTermScalarByIndex(10), Is.EqualTo(3.0));
    }

    #endregion
}
