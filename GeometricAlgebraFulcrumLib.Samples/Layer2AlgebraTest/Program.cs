using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;

namespace Layer2AlgebraTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Layer 2 Algebra API Verification ===\n");

            // Test 1: CreateEuclidean without dimension parameter (correct API)
            Console.WriteLine("Test 1: CreateEuclidean method");
            var scalarProcessor = ScalarProcessorOfFloat64.Instance;
            var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
            Console.WriteLine($"✓ CreateEuclidean(scalarProcessor) works");

            // Test 2: VectorTerm method
            Console.WriteLine("\nTest 2: VectorTerm method");
            var e1 = processor.VectorTerm(0, 1.0);
            var e2 = processor.VectorTerm(1, 1.0);
            var e3 = processor.VectorTerm(2, 1.0);
            Console.WriteLine($"✓ VectorTerm(index, scalar) works: e1={e1}");

            // Test 3: BivectorTerm method
            Console.WriteLine("\nTest 3: BivectorTerm method");
            var bivector = processor.BivectorTerm(0, 1, 1.0);
            Console.WriteLine($"✓ BivectorTerm(index1, index2, scalar) works: {bivector}");

            // Test 4: Vector method with params
            Console.WriteLine("\nTest 4: Vector method");
            var v1 = processor.Vector(1.5, 2.7);
            Console.WriteLine($"✓ Vector(params double[]) works: {v1}");

            // Test 5: CreateConformal without dimension parameter
            Console.WriteLine("\nTest 5: CreateConformal method");
            var conformalProcessor = XGaProcessor<double>.CreateConformal(scalarProcessor);
            Console.WriteLine($"✓ CreateConformal(scalarProcessor) works");

            Console.WriteLine("\n=== All Layer 2 Algebra API tests passed! ===");
        }
    }
}
