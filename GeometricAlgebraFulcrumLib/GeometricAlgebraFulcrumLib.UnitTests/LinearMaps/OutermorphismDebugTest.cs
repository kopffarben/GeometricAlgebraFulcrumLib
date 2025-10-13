using System;
using System.Collections.Generic;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Outermorphisms;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.SpaceND;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.LinearMaps.SpaceND;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

/// <summary>
/// Debug test to understand how diagonal outermorphisms work
/// </summary>
[TestFixture]
public class OutermorphismDebugTest
{
    [Test]
    public void Debug_DiagonalOutermorphismBehavior()
    {
        var processor = XGaFloat64Processor.Euclidean;

        // Try creating a diagonal outermorphism that scales e₁ by 2 and e₂ by 3
        var scalars = new[] { 2.0, 3.0, 1.0 };

        // Method 1: Using our helper
        var dict = new Dictionary<int, LinFloat64Vector>();
        for (int i = 0; i < scalars.Length; i++)
        {
            dict[i] = LinFloat64Vector.Create(i, scalars[i]);
        }
        var linearMap = dict.ToLinUnilinearMap();
        var outermorphism = processor.ToOutermorphism(linearMap);

        // Test mapping of e₁
        var e1 = processor.VectorTerm(0);
        var e1Mapped = outermorphism.OmMap(e1);

        Console.WriteLine($"Test 1: Map e₁ with scalar 2.0");
        Console.WriteLine($"  Input: {e1}");
        Console.WriteLine($"  Expected: 2*e₁");
        Console.WriteLine($"  Actual: {e1Mapped}");
        Console.WriteLine();

        // Test mapping of e₂
        var e2 = processor.VectorTerm(1);
        var e2Mapped = outermorphism.OmMap(e2);

        Console.WriteLine($"Test 2: Map e₂ with scalar 3.0");
        Console.WriteLine($"  Input: {e2}");
        Console.WriteLine($"  Expected: 3*e₂");
        Console.WriteLine($"  Actual: {e2Mapped}");
        Console.WriteLine();

        // Test linearity
        var v = e1 + e2;
        var vMapped = outermorphism.OmMap(v);
        var expected = e1 * 2 + e2 * 3;

        Console.WriteLine($"Test 3: Linearity");
        Console.WriteLine($"  Input: e₁ + e₂");
        Console.WriteLine($"  Expected: 2*e₁ + 3*e₂");
        Console.WriteLine($"  Actual: {vMapped}");
        Console.WriteLine();

        // Test outer product preservation
        var bivector = e1.Op(e2).GetBivectorPart();
        var bivectorMapped = outermorphism.OmMap(bivector);
        var expectedBivector = e1Mapped.Op(e2Mapped);

        Console.WriteLine($"Test 4: Outer product preservation");
        Console.WriteLine($"  f(e₁ ∧ e₂) = {bivectorMapped}");
        Console.WriteLine($"  f(e₁) ∧ f(e₂) = {expectedBivector}");
        Console.WriteLine();

        // Verify the linear map itself
        Console.WriteLine($"Test 5: Linear map structure");
        Console.WriteLine($"  LinearMap has {linearMap.Count} mappings");
        foreach (var kvp in linearMap)
        {
            Console.WriteLine($"    [{kvp.Key}] → {kvp.Value}");
        }
    }

    [Test]
    public void Debug_GradePreservation()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var random = processor.CreateXGaRandomComposer(5, 42);

        // Create diagonal outermorphism
        var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
        var diagonalVector = processor.VectorZero;
        for (int i = 0; i < scalars.Length; i++)
        {
            diagonalVector += processor.VectorTerm(i, scalars[i]);
        }
        var outermorphism = diagonalVector.ToDiagonalAutomorphism();

        // Test grade preservation with deterministic k-vector
        const int grade = 3;

        // Create a specific trivector: e0 ∧ e1 ∧ e2
        var e0 = processor.VectorTerm(0);
        var e1 = processor.VectorTerm(1);
        var e2 = processor.VectorTerm(2);
        var trivector = e0.Op(e1).Op(e2).GetKVectorPart(grade);

        Console.WriteLine($"DEBUG: Grade Preservation");
        Console.WriteLine($"Input trivector: {trivector}");
        Console.WriteLine($"Input grade: {grade}");
        Console.WriteLine();

        var mapped = outermorphism.OmMap(trivector);
        Console.WriteLine($"Mapped result: {mapped}");
        Console.WriteLine($"Mapped grades: {string.Join(", ", mapped.KVectorGrades)}");
        Console.WriteLine();

        // Expected: (2*e0) ∧ (3*e1) ∧ (4*e2) = 24*(e0∧e1∧e2)
        var expected = trivector * (scalars[0] * scalars[1] * scalars[2]);
        Console.WriteLine($"Expected: {expected}");
        Console.WriteLine($"Expected grade: {grade}");
        Console.WriteLine();

        // Test with random k-vector
        var randomKVector = random.GetKVector(grade);
        Console.WriteLine($"Random k-vector: {randomKVector}");
        Console.WriteLine($"Random grades: {string.Join(", ", randomKVector.KVectorGrades)}");

        var mappedRandom = outermorphism.OmMap(randomKVector);
        Console.WriteLine($"Mapped random: {mappedRandom}");
        Console.WriteLine($"Mapped random grades: {string.Join(", ", mappedRandom.KVectorGrades)}");

        // Check if grade is preserved
        var gradePreserved = mappedRandom.KVectorGrades.Count() == 1 &&
                            mappedRandom.KVectorGrades.First() == grade;
        Console.WriteLine($"Grade preserved for random: {gradePreserved}");
    }
}
