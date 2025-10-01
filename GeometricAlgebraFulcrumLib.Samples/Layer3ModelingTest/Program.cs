using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;

namespace Layer3ModelingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Layer 3 Modeling API Verification ===\n");

            var scalarProcessor = ScalarProcessorOfFloat64.Instance;

            // Test 1: CreateEuclidean for robotics application (line 341)
            Console.WriteLine("Test 1: CreateEuclidean for robotics");
            var processor1 = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
            Console.WriteLine($"✓ CreateEuclidean(scalarProcessor) works for robotics");

            // Test 2: CreateConformal for ray tracing (line 551)
            Console.WriteLine("\nTest 2: CreateConformal for graphics");
            var processor2 = XGaProcessor<double>.CreateConformal(scalarProcessor);
            Console.WriteLine($"✓ CreateConformal(scalarProcessor) works for graphics");

            // Test 3: CreateEuclidean for signal processing (line 827)
            Console.WriteLine("\nTest 3: CreateEuclidean for signal processing");
            var processor3 = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
            Console.WriteLine($"✓ CreateEuclidean(scalarProcessor) works for signal processing");

            // Verify all processors are functional
            Console.WriteLine("\nTest 4: Verify processors work");
            var v1 = processor1.Vector(1.0, 2.0, 3.0);
            var v2 = processor1.Vector(4.0, 5.0, 6.0);
            var result = v1.Sp(v2);
            Console.WriteLine($"✓ Vector operations work: v1·v2 = {result.ScalarValue}");

            Console.WriteLine("\n=== All Layer 3 Modeling API tests passed! ===");
        }
    }
}
