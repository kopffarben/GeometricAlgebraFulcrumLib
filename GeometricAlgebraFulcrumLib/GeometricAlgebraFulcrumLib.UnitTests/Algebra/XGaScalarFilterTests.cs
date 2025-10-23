using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for XGaScalar filter methods - Phase 1.1.3 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;double&gt; have identical functionality.
///
/// Note: XGaScalar filter methods always return VectorZero since scalars have grade 0.
/// These methods exist for API consistency across all multivector types.
/// </summary>
[TestFixture]
public class XGaScalarFilterTests
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
    public void Float64_GetVectorPart_NoFilter_ShouldAlwaysReturnZero()
    {
        // Arrange
        var scalar = _float64Processor.Scalar(5.0);

        // Act
        var vector = scalar.GetVectorPart();

        // Assert
        Assert.That(vector.IsZero, Is.True, "Scalar has no vector part");
    }

    [Test]
    public void Float64_GetVectorPart_FilterByIndex_ShouldAlwaysReturnZero()
    {
        // Arrange
        var scalar = _float64Processor.Scalar(10.0);

        // Act
        Func<int, bool> filterFunc = index => true;  // Accept all
        var vector = scalar.GetVectorPart(filterFunc);

        // Assert
        Assert.That(vector.IsZero, Is.True, "Scalar cannot have vector terms");
    }

    [Test]
    public void Float64_GetVectorPart_FilterByScalar_ShouldAlwaysReturnZero()
    {
        // Arrange
        var scalar = _float64Processor.Scalar(15.0);

        // Act
        Func<double, bool> filterFunc = s => s > 10.0;
        var vector = scalar.GetVectorPart(filterFunc);

        // Assert
        Assert.That(vector.IsZero, Is.True);
    }

    [Test]
    public void Float64_GetVectorPart_FilterByCombined_ShouldAlwaysReturnZero()
    {
        // Arrange
        var scalar = _float64Processor.Scalar(20.0);

        // Act
        Func<int, double, bool> filterFunc = (i, s) => i == 0 && s > 10.0;
        var vector = scalar.GetVectorPart(filterFunc);

        // Assert
        Assert.That(vector.IsZero, Is.True);
    }

    #endregion

    #region Generic<double> Tests

    [Test]
    public void Generic_GetVectorPart_NoFilter_ShouldAlwaysReturnZero()
    {
        // Arrange
        var scalar = _genericProcessor.Scalar(5.0);

        // Act
        var vector = scalar.GetVectorPart();

        // Assert
        Assert.That(vector.IsZero, Is.True, "Scalar has no vector part");
    }

    [Test]
    public void Generic_GetVectorPart_FilterByIndex_ShouldAlwaysReturnZero()
    {
        // Arrange
        var scalar = _genericProcessor.Scalar(10.0);

        // Act
        Func<int, bool> filterFunc = index => true;
        var vector = scalar.GetVectorPart(filterFunc);

        // Assert
        Assert.That(vector.IsZero, Is.True);
    }

    [Test]
    public void Generic_GetVectorPart_FilterByScalar_ShouldAlwaysReturnZero()
    {
        // Arrange
        var scalar = _genericProcessor.Scalar(15.0);

        // Act
        Func<double, bool> filterFunc = s => s > 10.0;
        var vector = scalar.GetVectorPart(filterFunc);

        // Assert
        Assert.That(vector.IsZero, Is.True);
    }

    [Test]
    public void Generic_GetVectorPart_FilterByCombined_ShouldAlwaysReturnZero()
    {
        // Arrange
        var scalar = _genericProcessor.Scalar(20.0);

        // Act
        Func<int, double, bool> filterFunc = (i, s) => i == 0 && s > 10.0;
        var vector = scalar.GetVectorPart(filterFunc);

        // Assert
        Assert.That(vector.IsZero, Is.True);
    }

    #endregion

    #region Equivalence Tests (Float64 vs Generic<double>)

    [Test]
    public void Equivalence_AllFilterMethods_ShouldReturnZero()
    {
        // Arrange
        var float64Scalar = _float64Processor.Scalar(42.0);
        var genericScalar = _genericProcessor.Scalar(42.0);

        // Act & Assert - No filter
        Assert.That(genericScalar.GetVectorPart().IsZero, Is.EqualTo(float64Scalar.GetVectorPart().IsZero));

        // Act & Assert - Index filter
        Func<int, bool> indexFilter = i => i % 2 == 0;
        Assert.That(genericScalar.GetVectorPart(indexFilter).IsZero,
            Is.EqualTo(float64Scalar.GetVectorPart(indexFilter).IsZero));

        // Act & Assert - Scalar filter
        Func<double, bool> scalarFilter = s => s > 20.0;
        Assert.That(genericScalar.GetVectorPart(scalarFilter).IsZero,
            Is.EqualTo(float64Scalar.GetVectorPart(scalarFilter).IsZero));

        // Act & Assert - Combined filter
        Func<int, double, bool> combinedFilter = (i, s) => i == 0 && s > 20.0;
        Assert.That(genericScalar.GetVectorPart(combinedFilter).IsZero,
            Is.EqualTo(float64Scalar.GetVectorPart(combinedFilter).IsZero));
    }

    [Test]
    public void Equivalence_ZeroScalar_AllMethodsShouldReturnZero()
    {
        // Arrange
        var float64Zero = _float64Processor.ScalarZero;
        var genericZero = _genericProcessor.ScalarZero;

        // Act & Assert
        Assert.That(genericZero.GetVectorPart().IsZero, Is.True);
        Assert.That(float64Zero.GetVectorPart().IsZero, Is.True);

        Func<int, bool> filter = i => true;
        Assert.That(genericZero.GetVectorPart(filter).IsZero, Is.True);
        Assert.That(float64Zero.GetVectorPart(filter).IsZero, Is.True);
    }

    #endregion

    #region Edge Cases

    [Test]
    public void Float64_NegativeScalar_FiltersShouldStillReturnZero()
    {
        // Arrange
        var scalar = _float64Processor.Scalar(-100.0);

        // Act & Assert
        Assert.That(scalar.GetVectorPart().IsZero, Is.True);

        Func<double, bool> filter = s => s < 0;
        Assert.That(scalar.GetVectorPart(filter).IsZero, Is.True);
    }

    [Test]
    public void Generic_LargeScalar_FiltersShouldStillReturnZero()
    {
        // Arrange
        var scalar = _genericProcessor.Scalar(1e100);

        // Act & Assert
        Assert.That(scalar.GetVectorPart().IsZero, Is.True);

        Func<double, bool> filter = s => s > 1e50;
        Assert.That(scalar.GetVectorPart(filter).IsZero, Is.True);
    }

    #endregion
}
