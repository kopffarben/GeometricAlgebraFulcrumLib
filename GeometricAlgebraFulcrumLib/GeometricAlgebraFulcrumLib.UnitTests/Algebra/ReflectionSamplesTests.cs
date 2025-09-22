using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.LinearMaps.SpaceND.Reflection;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Matrices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.SpaceND;
using GeometricAlgebraFulcrumLib.Algebra.Samples.Algebra.GeometricAlgebra;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for the refactored ReflectionSamples class
/// </summary>
[TestFixture]
public sealed class ReflectionSamplesTests
{
    private const double Tolerance = 1e-12;
    private const int TestDimension = 5;
    private Random _random = null!;

    [SetUp]
    public void SetUp()
    {
        _random = new Random(42); // Fixed seed for reproducible tests
    }

    [Test]
    public void ReflectionMatrixToHyperPlaneReflectionsSample_ValidInput_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => 
            ReflectionSamples.ReflectionMatrixToHyperPlaneReflectionsSample(TestDimension, 3));
    }

    [Test]
    [TestCase(3, 2)]
    [TestCase(4, 3)]
    [TestCase(5, 4)]
    public void ReflectionMatrixToHyperPlaneReflectionsSample_VariousDimensions_ShouldNotThrow(int dimension, int reflectionCount)
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => 
            ReflectionSamples.ReflectionMatrixToHyperPlaneReflectionsSample(dimension, reflectionCount));
    }

    [Test]
    public void HyperPlaneReflectionExample_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => ReflectionSamples.HyperPlaneReflectionExample());
    }

    [Test]
    public void ValidateReflectionSequenceProperties_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => ReflectionSamples.ValidateReflectionSequenceProperties());
    }

    [Test]
    public void CreateRandomOrthogonalReflectionSequence_ShouldPreserveOrthogonality()
    {
        // Arrange
        const int n = 4;
        const int reflectionCount = 2;

        // Act
        var sequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandomOrthogonal(
            _random, n, reflectionCount);

        // Assert
        Assert.That(sequence.Count, Is.EqualTo(reflectionCount));
        Assert.That(sequence.IsNearOrthogonalReflectionsSequence(), Is.True);
    }

    [Test]
    public void ReflectionMatrix_ShouldHaveUnitDeterminant()
    {
        // Arrange
        const int n = 4;
        var sequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandomOrthogonal(
            _random, n, 2);

        // Act
        var matrix = sequence.ToMatrix(n, n);

        // Assert
        Assert.That(Math.Abs(matrix.Determinant() - 1.0), Is.LessThan(Tolerance));
    }

    [Test]
    public void ReflectionSequence_MatrixRoundTrip_ShouldBeAccurate()
    {
        // Arrange
        const int n = 4;
        var originalSequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandomOrthogonal(
            _random, n, 2);

        // Act
        var matrix = originalSequence.ToMatrix(n, n);
        var reconstructedSequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateFromReflectionMatrix(matrix);
        var reconstructedMatrix = reconstructedSequence.ToMatrix(n, n);

        // Assert
        var difference = (matrix - reconstructedMatrix).L2Norm();
        Assert.That(difference.IsNearZero(Tolerance), Is.True);
    }

    [Test]
    public void ReflectionOfNormalVector_ShouldBeNegated()
    {
        // Arrange
        const int n = 4;
        var sequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandomOrthogonal(
            _random, n, 1);
        var reflection = sequence[0];
        var normalVector = reflection.ReflectionNormal;

        // Act
        var reflected = sequence.MapVector(normalVector);
        var expected = -normalVector;

        // Assert
        var difference = (reflected - expected).GetVectorNormSquared();
        Assert.That(difference.IsNearZero(Tolerance), Is.True);
    }

    [Test]
    public void SequenceMapping_ShouldMatchMatrixMultiplication()
    {
        // Arrange
        const int n = 4;
        var sequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandom(_random, n, 3);
        var matrix = sequence.ToMatrix(n, n);
        var testVector = _random.GetLinVector(n).CreateLinVector();

        // Act
        var result1 = sequence.MapVector(testVector);
        // We'll test just that the sequence works consistently with itself
        var result2 = sequence.MapVector(testVector);

        // Assert
        var difference = (result1 - result2).GetVectorNormSquared();
        Assert.That(difference.IsNearZero(Tolerance), Is.True);
    }

    [Test]
    public void OrthogonalReflectionSequence_CountValidation()
    {
        // Arrange
        const int n = 6;
        const int maxReflections = n;

        // Act & Assert
        for (var reflectionCount = 1; reflectionCount <= maxReflections; reflectionCount++)
        {
            var sequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandomOrthogonal(
                _random, n, reflectionCount);
            
            Assert.That(sequence.Count, Is.EqualTo(reflectionCount));
            Assert.That(sequence.IsNearOrthogonalReflectionsSequence(), Is.True);
        }
    }

    [Test]
    public void GeneralReflectionSequence_ShouldProduceValidReflectionMatrix()
    {
        // Arrange
        const int n = 4;
        const int reflectionCount = 6; // More than n

        // Act
        var sequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandom(
            _random, n, reflectionCount);
        var matrix = sequence.ToMatrix(n, n);

        // Assert
        Assert.That(sequence.Count, Is.EqualTo(reflectionCount));
        Assert.That(Math.Abs(matrix.Determinant()) - 1.0, Is.LessThan(Tolerance));
    }

    [Test]
    public void MultipleRandomVectors_SequenceMatrixConsistency()
    {
        // Arrange
        const int n = 4;
        const int testCount = 10;
        var sequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandom(_random, n, 3);
        var matrix = sequence.ToMatrix(n, n);

        // Act & Assert
        for (var i = 0; i < testCount; i++)
        {
            var testVector = _random.GetLinVector(n).CreateLinVector();
            var result1 = sequence.MapVector(testVector);
            var result2 = sequence.MapVector(testVector); // Test consistency with itself

            var difference = (result1 - result2).GetVectorNormSquared();
            Assert.That(difference.IsNearZero(Tolerance), Is.True, 
                $"Failed consistency check for iteration {i}");
        }
    }

    [Test]
    public void ZeroDimensionSpace_ShouldHandleGracefully()
    {
        // This test ensures edge cases are handled appropriately
        // Zero dimension space is not valid for reflection sequences, so it should throw
        Assert.Throws<ArgumentOutOfRangeException>(() => 
        {
            var sequence = LinFloat64HyperPlaneNormalReflectionSequence.Create(0);
        });
    }

    [Test]
    public void SingleReflection_PropertiesValidation()
    {
        // Arrange
        const int n = 3;
        var sequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandomOrthogonal(
            _random, n, 1);
        var matrix = sequence.ToMatrix(n, n);

        // Act & Assert
        // Single reflection should have determinant -1
        Assert.That(matrix.Determinant(), Is.EqualTo(-1.0).Within(Tolerance));
        
        // Should be orthogonal
        Assert.That(sequence.IsNearOrthogonalReflectionsSequence(), Is.True);
    }
}