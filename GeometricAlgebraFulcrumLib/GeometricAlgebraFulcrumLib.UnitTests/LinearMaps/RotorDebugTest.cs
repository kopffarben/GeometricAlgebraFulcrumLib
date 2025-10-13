using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Rotors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

/// <summary>
/// Debug test to understand why Rotor tests are failing
/// </summary>
[TestFixture]
public class RotorDebugTest
{
    [Test]
    public void Debug_CreatePureRotorBehavior()
    {
        var processor = XGaFloat64Processor.Euclidean;

        Console.WriteLine("=== Debug: CreatePureRotor Behavior ===");
        Console.WriteLine();

        // Test 1: Simple basis vectors (should work)
        Console.WriteLine("Test 1: Rotate e₁ to e₂ (90° rotation)");
        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);

        try
        {
            var rotor1 = e1.CreatePureRotor(e2);
            var result = rotor1.OmMap(e1);
            Console.WriteLine($"  Source: {e1}");
            Console.WriteLine($"  Target: {e2}");
            Console.WriteLine($"  Result: {result}");
            Console.WriteLine($"  IsValid: {rotor1.IsValid()}");
            Console.WriteLine($"  ✓ Success!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ FAILED: {ex.GetType().Name}: {ex.Message}");
        }
        Console.WriteLine();

        // Test 2: Parallel vectors (should work - identity rotor)
        Console.WriteLine("Test 2: Rotate e₁ to e₁ (0° rotation, parallel)");
        try
        {
            var rotor2 = e1.CreatePureRotor(e1);
            var result = rotor2.OmMap(e1);
            Console.WriteLine($"  Source: {e1}");
            Console.WriteLine($"  Target: {e1}");
            Console.WriteLine($"  Result: {result}");
            Console.WriteLine($"  IsValid: {rotor2.IsValid()}");
            Console.WriteLine($"  ✓ Success!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ FAILED: {ex.GetType().Name}: {ex.Message}");
        }
        Console.WriteLine();

        // Test 3: Antiparallel vectors (known bug case)
        Console.WriteLine("Test 3: Rotate e₁ to -e₁ (180° rotation, ANTIPARALLEL)");
        var minusE1 = -e1;
        try
        {
            var rotor3 = e1.CreatePureRotor(minusE1);
            var result = rotor3.OmMap(e1);
            Console.WriteLine($"  Source: {e1}");
            Console.WriteLine($"  Target: {minusE1}");
            Console.WriteLine($"  Result: {result}");
            Console.WriteLine($"  IsValid: {rotor3.IsValid()}");
            Console.WriteLine($"  ✓ Success!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ FAILED: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"  THIS IS THE KNOWN BUG: CreatePureRotor fails with antiparallel vectors!");
        }
        Console.WriteLine();

        // Test 4: Near-antiparallel vectors
        Console.WriteLine("Test 4: Near-antiparallel vectors (cos(θ) ≈ -1)");
        var v1 = e1;
        var v2 = -e1 + e2 * 0.01; // Almost antiparallel
        v2 = v2.DivideByENorm();

        var cosAngle = v1.ESp(v2);
        Console.WriteLine($"  cos(angle) = {cosAngle} (antiparallel when ≈ -1)");

        try
        {
            var rotor4 = v1.CreatePureRotor(v2);
            var result = rotor4.OmMap(v1);
            Console.WriteLine($"  IsValid: {rotor4.IsValid()}");
            Console.WriteLine($"  ✓ Success!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ FAILED: {ex.GetType().Name}: {ex.Message}");
        }
        Console.WriteLine();

        // Test 5: Random vectors - demonstrate the problem
        Console.WriteLine("Test 5: Random vectors (demonstrate failure rate)");
        var random = processor.CreateXGaRandomComposer(5, 42);
        int attempts = 100;
        int successes = 0;
        int antiparallelCases = 0;
        int otherFailures = 0;

        for (int i = 0; i < attempts; i++)
        {
            var u = random.GetVector().DivideByENorm();
            var v = random.GetVector().DivideByENorm();
            var cos = u.ESp(v);

            if (Math.Abs(cos + 1.0) < 1e-9)
            {
                antiparallelCases++;
                continue;
            }

            try
            {
                var rotor = u.CreatePureRotor(v);
                if (rotor.IsValid())
                    successes++;
                else
                    otherFailures++;
            }
            catch
            {
                otherFailures++;
            }
        }

        Console.WriteLine($"  Total attempts: {attempts}");
        Console.WriteLine($"  Successes: {successes}");
        Console.WriteLine($"  Antiparallel cases skipped: {antiparallelCases}");
        Console.WriteLine($"  Other failures: {otherFailures}");
        Console.WriteLine($"  Success rate: {(double)successes / attempts * 100:F1}%");
        Console.WriteLine();
    }

    [Test]
    public void Debug_ManualRotorConstruction()
    {
        var processor = XGaFloat64Processor.Euclidean;

        Console.WriteLine("=== Debug: Manual Rotor Construction vs CreatePureRotor ===");
        Console.WriteLine();

        // Test manual construction for 180° rotation
        Console.WriteLine("Test: 180° rotation around Z-axis (antiparallel case)");
        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);

        // Method 1: CreatePureRotor (will fail)
        Console.WriteLine("Method 1: Using CreatePureRotor (e1 → -e1)");
        try
        {
            var rotor1 = e1.CreatePureRotor(-e1);
            Console.WriteLine($"  ✓ Success (unexpected!)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ FAILED: {ex.GetType().Name}");
        }
        Console.WriteLine();

        // Method 2: Manual construction using angle-bivector formula
        Console.WriteLine("Method 2: Manual construction R = cos(θ/2) + sin(θ/2) * B");
        var angle = Math.PI; // 180 degrees
        var bivector = e1.Op(e2).GetBivectorPart(); // Bivector in XY plane

        var scalarPart = Math.Cos(angle / 2); // cos(π/2) = 0
        var bivectorPart = bivector * Math.Sin(angle / 2); // sin(π/2) = 1

        Console.WriteLine($"  Angle: {angle} radians (180°)");
        Console.WriteLine($"  Scalar part: {scalarPart}");
        Console.WriteLine($"  Bivector part: {bivectorPart}");

        var rotor2 = XGaFloat64PureRotor.Create(scalarPart, bivectorPart);
        Console.WriteLine($"  IsValid: {rotor2.IsValid()}");

        // Test rotation
        var rotated = rotor2.OmMap(e1);
        Console.WriteLine($"  e₁ rotated: {rotated}");
        Console.WriteLine($"  Expected: -e₁ (rotation in XY plane)");
        Console.WriteLine();

        // Method 3: Using geometric product formula
        Console.WriteLine("Method 3: Using geometric product R = (v + u) / |v + u|");
        var sum = -e1 + e1; // This will be zero for antiparallel vectors!
        Console.WriteLine($"  v + u = {sum}");
        Console.WriteLine($"  ✗ Cannot normalize zero vector - this is why CreatePureRotor fails!");
        Console.WriteLine();
    }

    [Test]
    public void Debug_RotorConstructionFormula()
    {
        var processor = XGaFloat64Processor.Euclidean;

        Console.WriteLine("=== Debug: Understanding Rotor Construction Formula ===");
        Console.WriteLine();

        Console.WriteLine("Formula: R = (v*u + |v||u|) / |v*u + |v||u||");
        Console.WriteLine("where v*u is geometric product (scalar + bivector)");
        Console.WriteLine();

        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);

        // Case 1: 90° rotation
        Console.WriteLine("Case 1: e₁ → e₂ (90° rotation)");
        var gp1 = e1.Gp(e2);
        var norm1 = e1.ENorm() * e2.ENorm();
        Console.WriteLine($"  e₁ * e₂ = {gp1}");
        Console.WriteLine($"  |e₁||e₂| = {norm1}");
        Console.WriteLine($"  Numerator: (geometric product) + (scalar norm)");
        Console.WriteLine($"  Can normalize: Yes - non-zero result");
        Console.WriteLine();

        // Case 2: 180° rotation (antiparallel)
        Console.WriteLine("Case 2: e₁ → -e₁ (180° rotation, ANTIPARALLEL)");
        var gp2 = e1.Gp(-e1);
        var norm2 = e1.ENorm() * (-e1).ENorm();
        var gpScalarPart = gp2.GetScalarPart();
        Console.WriteLine($"  e₁ * (-e₁) = {gp2} (scalar part: {gpScalarPart})");
        Console.WriteLine($"  |e₁||-e₁| = {norm2}");
        Console.WriteLine($"  Sum: {gpScalarPart} + {norm2} = {gpScalarPart.ScalarValue + norm2}");
        Console.WriteLine($"  Can normalize: NO - numerator is zero!");
        Console.WriteLine();

        Console.WriteLine("CONCLUSION:");
        Console.WriteLine("  When vectors are antiparallel (u = -v):");
        Console.WriteLine("  - Geometric product: u*v = -|u||v| (scalar)");
        Console.WriteLine("  - Formula gives: -|u||v| + |u||v| = 0");
        Console.WriteLine("  - Cannot normalize zero → CreatePureRotor fails!");
        Console.WriteLine("  - This is a FUNDAMENTAL MATHEMATICAL LIMITATION");
        Console.WriteLine();
        Console.WriteLine("  Solution: 180° rotations need a specific bivector (rotation plane)");
        Console.WriteLine("  - The formula doesn't specify which plane to rotate in");
        Console.WriteLine("  - Multiple valid rotors exist for antiparallel vectors");
    }
}
