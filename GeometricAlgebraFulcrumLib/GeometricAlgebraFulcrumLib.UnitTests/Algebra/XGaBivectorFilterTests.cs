using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for XGaBivector filter methods - Phase 1.1.2 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; have identical functionality.
///
/// Note: XGaBivector already has complete parity between Float64 and Generic.
/// Both use GetBivectorPart for index-based filters and GetPart for scalar-based filters.
/// </summary>
[TestFixture]
public class XGaBivectorFilterTests
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
    public void Float64_GetBivectorPart_FilterByIndices_ShouldReturnFilteredTerms()
    {
        // Arrange - Create bivector with terms: e_0∧e_1, e_0∧e_2, e_1∧e_2, e_2∧e_3
        var bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.0)
            .SetBivectorTerm(0, 2, 3.0)
            .SetBivectorTerm(1, 2, 5.0)
            .SetBivectorTerm(2, 3, 7.0)
            .GetBivector();

        // Act - Filter for terms where first index is 0
        Func<int, int, bool> filterFunc = (i, j) => i == 0;
        var filtered = bivector.GetBivectorPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2), "Should have 2 terms with first index 0");
        Assert.That(filtered.GetBasisBladeScalar(IndexSet.Create(0, 1)), Is.EqualTo(2.0));
        Assert.That(filtered.GetBasisBladeScalar(IndexSet.Create(0, 2)), Is.EqualTo(3.0));
    }

    [Test]
    public void Float64_GetPart_FilterByScalar_ShouldReturnFilteredTerms()
    {
        // Arrange
        var bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.0)
            .SetBivectorTerm(0, 2, 6.0)
            .SetBivectorTerm(1, 2, 3.0)
            .SetBivectorTerm(2, 3, 9.0)
            .GetBivector();

        // Act - Filter for scalars > 5.0
        Func<double, bool> filterFunc = scalar => scalar > 5.0;
        var filtered = bivector.GetPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2), "Should have 2 terms with values > 5");
        Assert.That(filtered.GetBasisBladeScalar(IndexSet.Create(0, 2)), Is.EqualTo(6.0));
        Assert.That(filtered.GetBasisBladeScalar(IndexSet.Create(2, 3)), Is.EqualTo(9.0));
    }

    #endregion

    #region Generic<double> Tests

    [Test]
    public void Generic_GetBivectorPart_FilterByIndices_ShouldReturnFilteredTerms()
    {
        // Arrange
        var bivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.0)
            .SetBivectorTerm(0, 2, 3.0)
            .SetBivectorTerm(1, 2, 5.0)
            .SetBivectorTerm(2, 3, 7.0)
            .GetBivector();

        // Act - Filter for terms where first index is 0
        Func<int, int, bool> filterFunc = (i, j) => i == 0;
        var filtered = bivector.GetBivectorPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2), "Should have 2 terms");
        Assert.That(filtered.GetBasisBladeScalar(IndexSet.Create(0, 1)).ScalarValue, Is.EqualTo(2.0));
        Assert.That(filtered.GetBasisBladeScalar(IndexSet.Create(0, 2)).ScalarValue, Is.EqualTo(3.0));
    }

    [Test]
    public void Generic_GetPart_FilterByScalar_ShouldReturnFilteredTerms()
    {
        // Arrange
        var bivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.0)
            .SetBivectorTerm(0, 2, 6.0)
            .SetBivectorTerm(1, 2, 3.0)
            .SetBivectorTerm(2, 3, 9.0)
            .GetBivector();

        // Act - Filter for scalars > 5.0
        Func<double, bool> filterFunc = scalar => scalar > 5.0;
        var filtered = bivector.GetPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2), "Should have 2 terms");
        Assert.That(filtered.GetBasisBladeScalar(IndexSet.Create(0, 2)).ScalarValue, Is.EqualTo(6.0));
        Assert.That(filtered.GetBasisBladeScalar(IndexSet.Create(2, 3)).ScalarValue, Is.EqualTo(9.0));
    }

    #endregion

    #region Equivalence Tests (Float64 vs Generic<double>)

    [Test]
    public void Equivalence_FilterByIndices_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.5)
            .SetBivectorTerm(0, 2, 2.5)
            .SetBivectorTerm(1, 3, 3.5)
            .SetBivectorTerm(2, 3, 4.5)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.5)
            .SetBivectorTerm(0, 2, 2.5)
            .SetBivectorTerm(1, 3, 3.5)
            .SetBivectorTerm(2, 3, 4.5)
            .GetBivector();

        // Act - Filter for second index > 1
        Func<int, int, bool> filterFunc = (i, j) => j > 1;
        var float64Filtered = float64Bivector.GetBivectorPart(filterFunc);
        var genericFiltered = genericBivector.GetBivectorPart(filterFunc);

        // Assert
        Assert.That(genericFiltered.Count, Is.EqualTo(float64Filtered.Count), "Same number of terms");

        Assert.That(genericFiltered.GetBasisBladeScalar(IndexSet.Create(0, 2)).ScalarValue,
            Is.EqualTo(float64Filtered.GetBasisBladeScalar(IndexSet.Create(0, 2))).Within(1e-14));
        Assert.That(genericFiltered.GetBasisBladeScalar(IndexSet.Create(1, 3)).ScalarValue,
            Is.EqualTo(float64Filtered.GetBasisBladeScalar(IndexSet.Create(1, 3))).Within(1e-14));
        Assert.That(genericFiltered.GetBasisBladeScalar(IndexSet.Create(2, 3)).ScalarValue,
            Is.EqualTo(float64Filtered.GetBasisBladeScalar(IndexSet.Create(2, 3))).Within(1e-14));
    }

    [Test]
    public void Equivalence_FilterByScalar_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.0)
            .SetBivectorTerm(0, 2, 10.0)
            .SetBivectorTerm(1, 2, 3.0)
            .SetBivectorTerm(2, 3, 15.0)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.0)
            .SetBivectorTerm(0, 2, 10.0)
            .SetBivectorTerm(1, 2, 3.0)
            .SetBivectorTerm(2, 3, 15.0)
            .GetBivector();

        // Act
        Func<double, bool> filterFunc = scalar => scalar >= 10.0;
        var float64Filtered = float64Bivector.GetPart(filterFunc);
        var genericFiltered = genericBivector.GetPart(filterFunc);

        // Assert
        Assert.That(genericFiltered.Count, Is.EqualTo(float64Filtered.Count));

        Assert.That(genericFiltered.GetBasisBladeScalar(IndexSet.Create(0, 2)).ScalarValue,
            Is.EqualTo(float64Filtered.GetBasisBladeScalar(IndexSet.Create(0, 2))).Within(1e-14));
        Assert.That(genericFiltered.GetBasisBladeScalar(IndexSet.Create(2, 3)).ScalarValue,
            Is.EqualTo(float64Filtered.GetBasisBladeScalar(IndexSet.Create(2, 3))).Within(1e-14));
    }

    [Test]
    public void Equivalence_ComplexIndexFilter_ShouldProduceIdenticalResults()
    {
        // Arrange
        var float64Bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.0)
            .SetBivectorTerm(0, 2, 3.0)
            .SetBivectorTerm(1, 2, 5.0)
            .SetBivectorTerm(1, 3, 7.0)
            .SetBivectorTerm(2, 3, 11.0)
            .GetBivector();

        var genericBivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 2.0)
            .SetBivectorTerm(0, 2, 3.0)
            .SetBivectorTerm(1, 2, 5.0)
            .SetBivectorTerm(1, 3, 7.0)
            .SetBivectorTerm(2, 3, 11.0)
            .GetBivector();

        // Act - Filter for terms where sum of indices is prime
        var isPrime = new Func<int, bool>(n =>
        {
            if (n < 2) return false;
            for (int i = 2; i <= Math.Sqrt(n); i++)
                if (n % i == 0) return false;
            return true;
        });

        Func<int, int, bool> combinedFilter = (i, j) => isPrime(i + j);
        var float64Filtered = float64Bivector.GetBivectorPart(combinedFilter);
        var genericFiltered = genericBivector.GetBivectorPart(combinedFilter);

        // Assert
        Assert.That(genericFiltered.Count, Is.EqualTo(float64Filtered.Count),
            "Should have same number of filtered terms");

        // i+j=3: e_1∧e_2 (prime)
        // i+j=4: e_1∧e_3 (not prime)
        // i+j=5: e_2∧e_3 (prime)
        Assert.That(genericFiltered.GetBasisBladeScalar(IndexSet.Create(1, 2)).ScalarValue, Is.EqualTo(5.0));
        Assert.That(genericFiltered.GetBasisBladeScalar(IndexSet.Create(2, 3)).ScalarValue, Is.EqualTo(11.0));
    }

    #endregion

    #region Edge Cases

    [Test]
    public void Generic_FilterByIndices_WithAllFalse_ShouldReturnZeroBivector()
    {
        // Arrange
        var bivector = _genericProcessor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.0)
            .SetBivectorTerm(1, 2, 2.0)
            .GetBivector();

        // Act - Filter that rejects everything
        Func<int, int, bool> filterFunc = (i, j) => false;
        var filtered = bivector.GetBivectorPart(filterFunc);

        // Assert
        Assert.That(filtered.IsZero, Is.True);
        Assert.That(filtered.Count, Is.EqualTo(0));
    }

    [Test]
    public void Float64_FilterByScalar_WithAllTrue_ShouldReturnOriginalBivector()
    {
        // Arrange
        var bivector = _float64Processor.CreateBivectorComposer()
            .SetBivectorTerm(0, 1, 1.0)
            .SetBivectorTerm(0, 2, 2.0)
            .SetBivectorTerm(1, 2, 3.0)
            .GetBivector();

        // Act - Filter that accepts everything
        Func<double, bool> filterFunc = scalar => true;
        var filtered = bivector.GetPart(filterFunc);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(bivector.Count));

        foreach (var pair in bivector.IdScalarPairs)
        {
            Assert.That(filtered.GetBasisBladeScalar(pair.Key),
                Is.EqualTo(bivector.GetBasisBladeScalar(pair.Key)));
        }
    }

    #endregion
}
