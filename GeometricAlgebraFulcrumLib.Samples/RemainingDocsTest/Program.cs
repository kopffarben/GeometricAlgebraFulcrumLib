using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;

namespace RemainingDocsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Remaining Documentation Files API Verification ===\n");

            var scalarProcessor = ScalarProcessorOfFloat64.Instance;
            var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

            // Test 1: Vector method (from integration.md, contributing.md)
            Console.WriteLine("Test 1: Vector method (integration.md, contributing.md fixes)");
            var v1 = processor.Vector(1, 2, 3);
            var v2 = processor.Vector(4, 5, 6);
            Console.WriteLine($"✓ processor.Vector(1, 2, 3) works: {v1}");
            Console.WriteLine($"✓ processor.Vector(4, 5, 6) works: {v2}");

            // Test 2: Geometric product from integration.md
            Console.WriteLine("\nTest 2: Geometric product (integration.md)");
            var result = v1.Gp(v2);
            Console.WriteLine($"✓ v1.Gp(v2) works: {result}");

            // Test 3: Test from contributing.md example
            Console.WriteLine("\nTest 3: Test examples (contributing.md)");
            var e1 = processor.Vector(1, 0, 0);
            var e2 = processor.Vector(0, 1, 0);
            var bivector = e1.Gp(e2);
            Console.WriteLine($"✓ e1 = {e1}");
            Console.WriteLine($"✓ e2 = {e2}");
            Console.WriteLine($"✓ e1.Gp(e2) = {bivector}");

            // Test 4: Verify CreateEuclidean without dimension parameter (layer4-metaprogramming.md)
            Console.WriteLine("\nTest 4: CreateEuclidean signature (layer4-metaprogramming.md)");
            var testProcessor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
            Console.WriteLine($"✓ CreateEuclidean(scalarProcessor) works correctly");

            Console.WriteLine("\n=== All Remaining Documentation API tests passed! ===");
        }
    }
}
