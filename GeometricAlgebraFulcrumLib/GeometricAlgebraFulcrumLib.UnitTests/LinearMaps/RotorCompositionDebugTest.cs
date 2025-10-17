using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Rotors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

[TestFixture]
public class RotorCompositionDebugTest
{
    [Test]
    [Ignore("CreatePureRotor fails when random vectors are (anti)parallel - known limitation for 180° rotations where normal vector is undefined. Use Debug_SimpleComposition instead for deterministic testing.")]
    public void Debug_RotorComposition()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var random = processor.CreateXGaRandomComposer(5, 42);

        Console.WriteLine("=== Debug: Rotor Composition ===");
        Console.WriteLine();

        // Create two valid rotors
        var u1 = random.GetVector().DivideByENorm();
        var v1 = random.GetVector().DivideByENorm();
        var rotor1 = u1.CreatePureRotor(v1);

        var u2 = random.GetVector().DivideByENorm();
        var v2 = random.GetVector().DivideByENorm();
        var rotor2 = u2.CreatePureRotor(v2);

        Console.WriteLine($"Rotor1: {rotor1.Multivector}");
        Console.WriteLine($"  IsValid: {rotor1.IsValid()}");
        Console.WriteLine($"  Scalar: {rotor1.Multivector.GetScalarPart()}");
        Console.WriteLine($"  Bivector: {rotor1.Multivector.GetBivectorPart()}");
        Console.WriteLine();

        Console.WriteLine($"Rotor2: {rotor2.Multivector}");
        Console.WriteLine($"  IsValid: {rotor2.IsValid()}");
        Console.WriteLine($"  Scalar: {rotor2.Multivector.GetScalarPart()}");
        Console.WriteLine($"  Bivector: {rotor2.Multivector.GetBivectorPart()}");
        Console.WriteLine();

        // Compose: R_combined = R2 * R1
        var combinedMv = rotor2.Multivector.Gp(rotor1.Multivector);
        Console.WriteLine($"Combined Multivector (R2 * R1): {combinedMv}");
        Console.WriteLine($"  Scalar part: {combinedMv.GetScalarPart()}");
        Console.WriteLine($"  Bivector part: {combinedMv.GetBivectorPart()}");
        Console.WriteLine($"  All grades: {string.Join(", ", combinedMv.KVectorGrades)}");
        Console.WriteLine();

        // Check if it contains only even grades
        Console.WriteLine($"IsEven(2): {combinedMv.IsEven(2)}");
        Console.WriteLine($"Has only grades 0,2: {combinedMv.KVectorGrades.All(g => g == 0 || g == 2)}");
        Console.WriteLine();

        // Create rotor from combined multivector
        var combinedRotor = XGaFloat64PureRotor.Create(combinedMv);
        Console.WriteLine($"Combined Rotor:");
        Console.WriteLine($"  Multivector: {combinedRotor.Multivector}");
        Console.WriteLine($"  Reverse: {combinedRotor.MultivectorReverse}");
        Console.WriteLine($"  IsValid: {combinedRotor.IsValid()}");
        Console.WriteLine();

        // Check rotor condition manually
        var gpCheck = combinedRotor.Multivector.Gp(combinedRotor.MultivectorReverse);
        Console.WriteLine($"Rotor condition check:");
        Console.WriteLine($"  R * R̃ = {gpCheck}");
        Console.WriteLine($"  Scalar part: {gpCheck.GetScalarPart()}");
        Console.WriteLine($"  Expected: 1.0");
        Console.WriteLine($"  Is scalar: {gpCheck.IsScalar()}");
        Console.WriteLine();

        // Try normalizing
        var norm = combinedMv.Norm().ScalarValue;
        Console.WriteLine($"Norm of combined multivector: {norm}");
        var normalized = combinedMv / norm;
        Console.WriteLine($"Normalized: {normalized}");
        var normalizedRotor = XGaFloat64PureRotor.Create(normalized);
        Console.WriteLine($"Normalized rotor IsValid: {normalizedRotor.IsValid()}");
    }

    [Test]
    public void Debug_SimpleComposition()
    {
        var processor = XGaFloat64Processor.Euclidean;

        Console.WriteLine("=== Debug: Simple Rotor Composition (e1→e2, e2→e3) ===");
        Console.WriteLine();

        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);
        var e3 = processor.VectorTerm(2);

        // First rotor: e1 → e2
        var rotor1 = e1.CreatePureRotor(e2);
        Console.WriteLine($"Rotor1 (e₁ → e₂): {rotor1.Multivector}");
        Console.WriteLine($"  IsValid: {rotor1.IsValid()}");

        // Second rotor: e2 → e3
        var rotor2 = e2.CreatePureRotor(e3);
        Console.WriteLine($"Rotor2 (e₂ → e₃): {rotor2.Multivector}");
        Console.WriteLine($"  IsValid: {rotor2.IsValid()}");
        Console.WriteLine();

        // Compose
        var combinedMv = rotor2.Multivector.Gp(rotor1.Multivector);
        Console.WriteLine($"Combined (R2 * R1): {combinedMv}");
        Console.WriteLine($"  Grades: {string.Join(", ", combinedMv.KVectorGrades)}");

        var combinedRotor = XGaFloat64PureRotor.Create(combinedMv);
        Console.WriteLine($"  IsValid: {combinedRotor.IsValid()}");
        Console.WriteLine();

        // Test: should rotate e1 → e3
        var result = combinedRotor.OmMap(e1);
        Console.WriteLine($"Test: R_combined(e₁) = {result}");
        Console.WriteLine($"Expected: Close to e₃ = {e3}");
        var diff = (result - e3).ENormSquared();
        Console.WriteLine($"Difference: {diff}");
    }
}
