using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

[TestFixture]
public class ReflectorFailureDebugTest
{
    private const double Tolerance = 1e-10;

    [Test]
    public void Debug_InverseReflectorFailure()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var random = processor.CreateXGaRandomComposer(5, 42);

        Console.WriteLine("=== Debug: PureReflector_InverseReflector ===");
        Console.WriteLine();

        // Replicate exact test conditions
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"Attempt {i + 1}:");

            var normal = random.GetVector().DivideByENorm();
            var reflector = normal.ToPureReflector();

            var v = random.GetVector();

            // Apply reflector twice
            var reflected = reflector.OmMap(v);
            var doubleReflected = reflector.OmMap(reflected);

            // Check difference
            var diff = (v - doubleReflected).ENormSquared();
            Console.WriteLine($"  Normal: {normal}");
            Console.WriteLine($"  Vector: {v}");
            Console.WriteLine($"  Reflected: {reflected}");
            Console.WriteLine($"  Double reflected: {doubleReflected}");
            Console.WriteLine($"  Difference norm squared: {diff}");
            Console.WriteLine($"  Tolerance: {Tolerance * 100}");
            Console.WriteLine($"  Pass: {diff < Tolerance * 100}");
            Console.WriteLine();

            if (diff >= Tolerance * 100)
            {
                Console.WriteLine("FAILURE DETECTED!");

                // Additional debugging
                Console.WriteLine($"  Normal norm: {normal.ENorm()}");
                Console.WriteLine($"  Normal norm squared: {normal.ENormSquared()}");
                Console.WriteLine($"  Reflector IsValid: {reflector.IsValid()}");
                Console.WriteLine($"  Vector inverse: {reflector.VectorInverse}");
                Console.WriteLine($"  v * v^-1: {normal.Gp(reflector.VectorInverse)}");
                break;
            }
        }
    }

    [Test]
    public void Debug_PreservesScalarProductFailure()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var random = processor.CreateXGaRandomComposer(5, 42);

        Console.WriteLine("=== Debug: PureReflector_PreservesScalarProduct ===");
        Console.WriteLine();

        // Replicate exact test conditions
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"Attempt {i + 1}:");

            var normal = random.GetVector().DivideByENorm();
            var reflector = normal.ToPureReflector();

            var a = random.GetVector();
            var b = random.GetVector();

            var originalSp = a.Sp(b).ScalarValue;

            var aReflected = reflector.OmMap(a);
            var bReflected = reflector.OmMap(b);
            var reflectedSp = aReflected.Sp(bReflected).ScalarValue;

            var diff = Math.Abs(originalSp - reflectedSp);

            Console.WriteLine($"  Normal: {normal}");
            Console.WriteLine($"  a: {a}");
            Console.WriteLine($"  b: {b}");
            Console.WriteLine($"  Original SP: {originalSp}");
            Console.WriteLine($"  Reflected SP: {reflectedSp}");
            Console.WriteLine($"  Difference: {diff}");
            Console.WriteLine($"  Tolerance: {Tolerance * 1000}");
            Console.WriteLine($"  Pass: {diff < Tolerance * 1000}");
            Console.WriteLine();

            if (diff >= Tolerance * 1000)
            {
                Console.WriteLine("FAILURE DETECTED!");

                // Additional debugging
                Console.WriteLine($"  Normal norm: {normal.ENorm()}");
                Console.WriteLine($"  a reflected: {aReflected}");
                Console.WriteLine($"  b reflected: {bReflected}");
                Console.WriteLine($"  a norm: {a.ENorm()}");
                Console.WriteLine($"  a reflected norm: {aReflected.ENorm()}");
                Console.WriteLine($"  Reflector IsValid: {reflector.IsValid()}");
                break;
            }
        }
    }

    [Test]
    public void Debug_ManualReflectionFormula()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var random = processor.CreateXGaRandomComposer(5, 42);

        Console.WriteLine("=== Debug: Manual Reflection Formula ===");
        Console.WriteLine();

        var n = random.GetVector().DivideByENorm();
        var reflector = n.ToPureReflector();
        var v = random.GetVector();

        Console.WriteLine($"Normal (n): {n}");
        Console.WriteLine($"Vector (v): {v}");
        Console.WriteLine();

        // Library method: n · v · n^-1
        var libraryResult = reflector.OmMap(v);
        Console.WriteLine($"Library result (n·v·n^-1): {libraryResult}");
        Console.WriteLine();

        // Manual formula 1: 2(v·n)n - v (reflection THROUGH axis)
        var vDotN = v.Sp(n).ScalarValue;
        var manual1 = n * (2 * vDotN) - v;
        Console.WriteLine($"Manual formula 1 (2(v·n)n - v): {manual1}");
        Console.WriteLine($"Difference from library: {(libraryResult - manual1).ENormSquared()}");
        Console.WriteLine();

        // Manual formula 2: v - 2(v·n)n (reflection ACROSS hyperplane perpendicular to n)
        var manual2 = v - n * (2 * vDotN);
        Console.WriteLine($"Manual formula 2 (v - 2(v·n)n): {manual2}");
        Console.WriteLine($"Difference from library: {(libraryResult - manual2).ENormSquared()}");
        Console.WriteLine();

        // Check which convention the library uses
        var diff1 = (libraryResult - manual1).ENormSquared();
        var diff2 = (libraryResult - manual2).ENormSquared();

        if (diff1 < Tolerance)
            Console.WriteLine("✓ Library uses: reflection THROUGH axis (preserves parallel)");
        else if (diff2 < Tolerance)
            Console.WriteLine("✓ Library uses: reflection ACROSS hyperplane (reverses parallel)");
        else
            Console.WriteLine("✗ Library doesn't match either convention!");
    }
}
