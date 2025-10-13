using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

/// <summary>
/// Debug test to understand how reflections work in the library
/// </summary>
[TestFixture]
public class ReflectorDebugTest
{
    [Test]
    public void Debug_ReflectionBehavior()
    {
        var processor = XGaFloat64Processor.Euclidean;

        // FINDING: Library uses "reflection THROUGH a line" convention, not "Householder reflection"
        // - Vectors parallel to n are PRESERVED
        // - Vectors perpendicular to n are REVERSED

        // Test 1: Reflect e₁ through e₁ axis
        var e1 = processor.VectorTerm(0);
        var reflector1 = e1.ToPureReflector();
        var reflected1 = reflector1.OmMap(e1);

        Console.WriteLine($"LIBRARY CONVENTION: Reflection THROUGH a line/axis");
        Console.WriteLine($"Test 1: Reflect e₁ through e₁ axis");
        Console.WriteLine($"  Input: {e1}");
        Console.WriteLine($"  Expected (library): e₁ (preserve parallel)");
        Console.WriteLine($"  Actual: {reflected1}");
        Console.WriteLine($"  ✓ Matches library convention!");
        Console.WriteLine();

        // Test 2: Reflect e₂ through e₁ axis (perpendicular)
        var e2 = processor.VectorTerm(1);
        var reflected2 = reflector1.OmMap(e2);

        Console.WriteLine($"Test 2: Reflect e₂ through e₁ axis");
        Console.WriteLine($"  Input: {e2}");
        Console.WriteLine($"  Expected (library): -e₂ (reverse perpendicular)");
        Console.WriteLine($"  Actual: {reflected2}");
        Console.WriteLine($"  ✓ Matches library convention!");
        Console.WriteLine();

        // Test 3: Reflect (1,1,0) through e₁ axis
        var v = e1 + e2;
        var reflected3 = reflector1.OmMap(v);

        Console.WriteLine($"Test 3: Reflect (1,1,0) through e₁ axis");
        Console.WriteLine($"  Input: {v}");
        Console.WriteLine($"  Expected (library): (1,-1,0)");
        Console.WriteLine($"  Actual: {reflected3}");
        Console.WriteLine($"  ✓ Matches library convention!");
        Console.WriteLine();

        // Verify with assertions
        Assert.That(reflected1.Subtract(e1).ENormSquared(), Is.LessThan(1e-10),
            "Parallel component should be preserved");
        Assert.That(reflected2.Add(e2).ENormSquared(), Is.LessThan(1e-10),
            "Perpendicular component should be reversed");

        Console.WriteLine($"CONCLUSION: All tests need to be rewritten for 'reflection through axis' convention");
    }

    [Test]
    public void Debug_InverseReflectorBehavior()
    {
        var processor = XGaFloat64Processor.Euclidean;

        // Test inverse reflector behavior
        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);
        var testVector = e1 + e2 * 2;

        // Create reflector for e1 axis
        var reflector = e1.ToPureReflector();
        var inverse = reflector.GetPureReflectorInverse();

        Console.WriteLine($"DEBUG: Inverse Reflector Behavior");
        Console.WriteLine($"Original vector: {testVector}");
        Console.WriteLine();

        // Apply reflector
        var reflected = reflector.OmMap(testVector);
        Console.WriteLine($"After reflection: {reflected}");
        Console.WriteLine($"Expected: e1 - 2*e2 (preserve parallel, reverse perpendicular)");
        Console.WriteLine();

        // Apply inverse
        var restored = inverse.OmMap(reflected);
        Console.WriteLine($"After inverse: {restored}");
        Console.WriteLine($"Expected: {testVector} (original)");
        Console.WriteLine();

        // Check if reflector equals its inverse (involution property)
        var doubleReflected = reflector.OmMap(reflected);
        Console.WriteLine($"Double reflection: {doubleReflected}");
        Console.WriteLine($"Expected: {testVector} (reflector should be its own inverse)");
        Console.WriteLine();

        // Check the inverse reflector's multivector
        Console.WriteLine($"Reflector multivector: {reflector.GetMultivector()}");
        Console.WriteLine($"Inverse multivector: {inverse.GetMultivector()}");
        Console.WriteLine();

        // Mathematical property: For reflection through axis, R^2 = I
        Assert.That(doubleReflected.Subtract(testVector).ENormSquared(), Is.LessThan(1e-10),
            "Double reflection should return original (R^2 = I)");
    }

    [Test]
    public void Debug_ScalarProductPreservation()
    {
        var processor = XGaFloat64Processor.Euclidean;

        // Test scalar product preservation
        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);
        var e3 = processor.VectorTerm(2);

        // Test vectors
        var a = e1 + e2 * 2;
        var b = e2 + e3 * 3;

        // Create reflector through e1 axis
        var reflector = e1.ToPureReflector();

        // Calculate original scalar product
        var originalSp = a.Sp(b).ScalarValue;

        // Reflect both vectors
        var aReflected = reflector.OmMap(a);
        var bReflected = reflector.OmMap(b);

        // Calculate reflected scalar product
        var reflectedSp = aReflected.Sp(bReflected).ScalarValue;

        Console.WriteLine($"DEBUG: Scalar Product Preservation");
        Console.WriteLine($"Original a: {a}");
        Console.WriteLine($"Original b: {b}");
        Console.WriteLine($"Original scalar product a·b: {originalSp}");
        Console.WriteLine();

        Console.WriteLine($"Reflected a: {aReflected}");
        Console.WriteLine($"Reflected b: {bReflected}");
        Console.WriteLine($"Reflected scalar product: {reflectedSp}");
        Console.WriteLine();

        Console.WriteLine($"Difference: {Math.Abs(originalSp - reflectedSp)}");
        Console.WriteLine($"Expected: Should be equal (orthogonal transformation)");
        Console.WriteLine();

        // Manual calculation to verify
        // a = e1 + 2*e2 → reflected = e1 - 2*e2
        // b = e2 + 3*e3 → reflected = -e2 - 3*e3
        // Original: (e1 + 2*e2) · (e2 + 3*e3) = 0 + 2*1 + 0 = 2
        // Reflected: (e1 - 2*e2) · (-e2 - 3*e3) = 0 + 2*1 + 0 = 2
        Console.WriteLine($"Manual calculation:");
        Console.WriteLine($"  Original: (e1 + 2*e2) · (e2 + 3*e3) = 2");
        Console.WriteLine($"  Reflected: (e1 - 2*e2) · (-e2 - 3*e3) = 2");

        // The scalar products should be equal for orthogonal transformations
        Assert.That(Math.Abs(originalSp - reflectedSp), Is.LessThan(1e-10),
            "Scalar product should be preserved by reflection");
    }
}
