using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Rotors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

[TestFixture]
public class IsValidDebugTest
{
    [Test]
    public void Debug_DeterministicRotorIsValid()
    {
        var processor = XGaFloat64Processor.Euclidean;

        Console.WriteLine("=== Debug: Deterministic Rotor IsValid() ==");
        Console.WriteLine();

        // Create simple deterministic rotor: e1 → e2
        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);

        var rotor = e1.CreatePureRotor(e2);

        Console.WriteLine($"Rotor: {rotor.Multivector}");
        Console.WriteLine($"Rotor reverse: {rotor.MultivectorReverse}");
        Console.WriteLine();

        // Check IsValid step by step
        var reverseCheck = rotor.Multivector.Reverse() - rotor.MultivectorReverse;
        Console.WriteLine($"1. Reverse check: {reverseCheck.IsNearZero()}");
        Console.WriteLine($"   Difference: {reverseCheck}");
        Console.WriteLine();

        var isEven = rotor.Multivector.IsEven(2);
        Console.WriteLine($"2. IsEven(2): {isEven}");
        Console.WriteLine($"   Grades present: {string.Join(", ", rotor.Multivector.KVectorGrades)}");
        Console.WriteLine();

        var gp = rotor.Multivector.Gp(rotor.MultivectorReverse);
        Console.WriteLine($"3. R * R̃: {gp}");
        Console.WriteLine($"   Is scalar: {gp.IsScalar()}");
        var scalarValue = gp.Scalar();
        Console.WriteLine($"   Scalar value: {scalarValue}");
        var diff = scalarValue - 1;
        Console.WriteLine($"   Difference from 1: {diff}");
        Console.WriteLine();

        Console.WriteLine($"Final IsValid(): {rotor.IsValid()}");
    }

    [Test]
    public void Debug_RandomRotorIsValid()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var random = processor.CreateXGaRandomComposer(5, 42);

        Console.WriteLine("=== Debug: Random Rotor IsValid() ==");
        Console.WriteLine();

        for (int attempt = 0; attempt < 10; attempt++)
        {
            var u = random.GetVector().DivideByENorm();
            var v = random.GetVector().DivideByENorm();

            var cosAngle = u.ESp(v);

            Console.WriteLine($"Attempt {attempt + 1}:");
            Console.WriteLine($"  cos(angle) = {cosAngle}");
            Console.WriteLine($"  |cos(angle)| = {Math.Abs(cosAngle)}");

            if (Math.Abs(cosAngle) > 0.99)
            {
                Console.WriteLine("  SKIPPED: Nearly parallel/antiparallel");
                Console.WriteLine();
                continue;
            }

            try
            {
                var rotor = u.CreatePureRotor(v);
                Console.WriteLine($"  Rotor created successfully");
                Console.WriteLine($"  IsValid: {rotor.IsValid()}");

                if (!rotor.IsValid())
                {
                    // Debug why it's invalid
                    var gp = rotor.Multivector.Gp(rotor.MultivectorReverse);
                    Console.WriteLine($"  R * R̃ scalar: {gp.Scalar()}");
                    Console.WriteLine($"  Diff from 1: {gp.Scalar() - 1}");
                    Console.WriteLine($"  IsEven(2): {rotor.Multivector.IsEven(2)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  EXCEPTION: {ex.GetType().Name}");
            }

            Console.WriteLine();
        }
    }

    [Test]
    public void Debug_CompositionIsValid()
    {
        var processor = XGaFloat64Processor.Euclidean;

        Console.WriteLine("=== Debug: Composition IsValid() ==");
        Console.WriteLine();

        // Use deterministic rotors
        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);
        var e3 = processor.VectorTerm(2);

        var rotor1 = e1.CreatePureRotor(e2);
        var rotor2 = e2.CreatePureRotor(e3);

        Console.WriteLine($"Rotor1 IsValid: {rotor1.IsValid()}");
        Console.WriteLine($"Rotor2 IsValid: {rotor2.IsValid()}");
        Console.WriteLine();

        // Compose
        var combinedMv = rotor2.Multivector.Gp(rotor1.Multivector);
        Console.WriteLine($"Combined multivector: {combinedMv}");
        Console.WriteLine($"  Norm: {combinedMv.Norm().ScalarValue}");
        Console.WriteLine();

        // Try without normalization
        var combined1 = XGaFloat64PureRotor.Create(combinedMv);
        Console.WriteLine($"Without normalization:");
        Console.WriteLine($"  IsValid: {combined1.IsValid()}");

        var gp1 = combined1.Multivector.Gp(combined1.MultivectorReverse);
        Console.WriteLine($"  R * R̃ scalar: {gp1.Scalar()}");
        Console.WriteLine($"  Diff from 1: {gp1.Scalar() - 1}");
        Console.WriteLine();

        // Try with normalization
        var norm = combinedMv.Norm().ScalarValue;
        var normalizedMv = combinedMv / norm;
        var combined2 = XGaFloat64PureRotor.Create(normalizedMv);

        Console.WriteLine($"With normalization:");
        Console.WriteLine($"  IsValid: {combined2.IsValid()}");

        var gp2 = combined2.Multivector.Gp(combined2.MultivectorReverse);
        Console.WriteLine($"  R * R̃ scalar: {gp2.Scalar()}");
        Console.WriteLine($"  Diff from 1: {gp2.Scalar() - 1}");
    }
}
