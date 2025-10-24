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
/// Debug tests to isolate the issue with XGaComputedOutermorphism
/// </summary>
[TestFixture]
public class XGaComputedOutermorphismDebugTests
{
    [Test]
    public void Debug_SimpleGenericMapping()
    {
        // Arrange
        var processor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);

        Func<int, XGaVector<double>> mapFunc = index =>
        {
            var result = processor.VectorTerm(index, 2.0);
            Console.WriteLine($"Mapping basis vector {index} to: {result}");
            Console.WriteLine($"  IsValid: {result.IsValid()}");
            Console.WriteLine($"  Count: {result.Count}");
            return result;
        };

        // Act
        Console.WriteLine("Creating outermorphism...");
        var om = processor.CreateComputedOutermorphism(mapFunc);

        Console.WriteLine("Calling OmMapBasisVector(0)...");
        var result = om.OmMapBasisVector(0);

        // Assert
        Console.WriteLine($"Result: {result}");
        Console.WriteLine($"Result IsValid: {result.IsValid()}");
        Assert.That(result.IsValid(), Is.True);
    }

    [Test]
    public void Debug_Float64Mapping()
    {
        // Arrange
        var processor = XGaFloat64Processor.Euclidean;

        Func<int, XGaFloat64Vector> mapFunc = index =>
        {
            var result = processor.VectorTerm(index, 2.0);
            Console.WriteLine($"Mapping basis vector {index} to: {result}");
            Console.WriteLine($"  IsValid: {result.IsValid()}");
            Console.WriteLine($"  Count: {result.Count}");
            return result;
        };

        // Act
        Console.WriteLine("Creating outermorphism...");
        var om = processor.CreateComputedOutermorphism(mapFunc);

        Console.WriteLine("Calling OmMapBasisVector(0)...");
        var result = om.OmMapBasisVector(0);

        // Assert
        Console.WriteLine($"Result: {result}");
        Console.WriteLine($"Result IsValid: {result.IsValid()}");
        Assert.That(result.IsValid(), Is.True);
    }

    [Test]
    public void Debug_RotationMapping_Generic()
    {
        // Arrange
        var processor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);

        Func<int, XGaVector<double>> mapFunc = index =>
        {
            Console.WriteLine($"Called with index: {index}");
            var result = index switch
            {
                0 => processor.Vector(1.0, 0.0, 0.0),
                1 => processor.Vector(0.0, 0.707, 0.707),
                2 => processor.Vector(0.0, -0.707, 0.707),
                _ => processor.VectorTerm(index, 1.0)
            };
            Console.WriteLine($"  Returning: {result}");
            Console.WriteLine($"  IsValid: {result.IsValid()}");
            return result;
        };

        // Act
        Console.WriteLine("Creating outermorphism...");
        var om = processor.CreateComputedOutermorphism(mapFunc);

        Console.WriteLine("\nMapping a vector (1, 2, 3)...");
        var vector = processor.Vector(1.0, 2.0, 3.0);
        Console.WriteLine($"Input vector: {vector}");

        var result = om.OmMap(vector);

        // Assert
        Console.WriteLine($"Result: {result}");
        Console.WriteLine($"Result IsValid: {result.IsValid()}");
        Assert.That(result.IsValid(), Is.True);
    }

    [Test]
    public void Debug_ExactOriginalTest_BasisVector()
    {
        // Arrange - EXACTLY like the original failing test
        var float64Processor = XGaFloat64Processor.Euclidean;
        var genericProcessor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);

        Func<int, XGaFloat64Vector> float64MapFunc = index =>
        {
            Console.WriteLine($"Float64: Called with index {index}");
            var result = float64Processor.VectorTerm(index, 2.0 * (index + 1));
            Console.WriteLine($"  Returning: {result}, IsValid: {result.IsValid()}");
            return result;
        };

        Func<int, XGaVector<double>> genericMapFunc = index =>
        {
            Console.WriteLine($"Generic: Called with index {index}");
            var result = genericProcessor.VectorTerm(index, 2.0 * (index + 1));
            Console.WriteLine($"  Returning: {result}, IsValid: {result.IsValid()}");
            return result;
        };

        var float64Om = float64Processor.CreateComputedOutermorphism(float64MapFunc);
        var genericOm = genericProcessor.CreateComputedOutermorphism(genericMapFunc);

        // Act & Assert - Test mapping of basis vectors
        Console.WriteLine("\nTesting Float64 mapping...");
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine($"\n--- Testing index {i} ---");
            var float64Result = float64Om.OmMapBasisVector(i);
            Console.WriteLine($"Float64 result: {float64Result}, IsValid: {float64Result.IsValid()}");

            var genericResult = genericOm.OmMapBasisVector(i);
            Console.WriteLine($"Generic result: {genericResult}, IsValid: {genericResult.IsValid()}");

            // NOW ADD ASSERTIONS like in original test
            Console.WriteLine($"Asserting count...");
            Assert.That(genericResult.Count, Is.EqualTo(float64Result.Count),
                $"Basis vector {i} should have same term count");

            Console.WriteLine($"Asserting scalar value...");
            Assert.That(genericResult.GetTermScalarByIndex(i).ScalarValue,
                Is.EqualTo(float64Result.GetTermScalarByIndex(i)).Within(1e-12),
                $"Basis vector {i} mapping should be identical");
        }
    }

    [Test]
    public void Debug_GetMappedBasisBlades()
    {
        // Arrange
        var float64Processor = XGaFloat64Processor.Euclidean;
        var genericProcessor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);

        Func<int, XGaFloat64Vector> float64MapFunc = index =>
        {
            Console.WriteLine($"Float64: Mapping index {index}");
            return float64Processor.VectorTerm(index, 3.0);
        };

        Func<int, XGaVector<double>> genericMapFunc = index =>
        {
            Console.WriteLine($"Generic: Mapping index {index}");
            return genericProcessor.VectorTerm(index, 3.0);
        };

        var float64Om = float64Processor.CreateComputedOutermorphism(float64MapFunc);
        var genericOm = genericProcessor.CreateComputedOutermorphism(genericMapFunc);

        // Act
        Console.WriteLine("\n=== Getting Float64 mapped blades ===");
        try
        {
            var float64Blades = float64Om.GetMappedBasisBlades(3).ToList();
            Console.WriteLine($"Float64: Got {float64Blades.Count} blades");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Float64 FAILED: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine("\n=== Getting Generic mapped blades ===");
        try
        {
            var genericBlades = genericOm.GetMappedBasisBlades(3).ToList();
            Console.WriteLine($"Generic: Got {genericBlades.Count} blades");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Generic FAILED: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
