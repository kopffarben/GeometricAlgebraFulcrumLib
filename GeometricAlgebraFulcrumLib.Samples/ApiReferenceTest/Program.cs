using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;

namespace ApiReferenceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== API Reference Verification ===\n");

            // Test 1: Vector creation with Vector() method (not CreateVector)
            Console.WriteLine("Test 1: Vector creation");
            var scalarProcessor = ScalarProcessorOfFloat64.Instance;
            var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
            
            var v1 = processor.Vector(1, 2, 3);
            Console.WriteLine($"✓ processor.Vector(1, 2, 3) works: {v1}");
            
            // Test 2: Multivector Add method
            Console.WriteLine("\nTest 2: Multivector operations");
            var v2 = processor.Vector(4, 5, 6);
            var sum = v1.Add(v2);
            Console.WriteLine($"✓ v1.Add(v2) works: {sum}");
            
            // Test 3: Complex scalar processor
            Console.WriteLine("\nTest 3: Complex scalar processor");
            var complexProcessor = ScalarProcessorOfComplex.Instance;
            var c1 = complexProcessor.ScalarFromNumber(3.0);
            Console.WriteLine($"✓ complexProcessor.ScalarFromNumber(3.0) works: {c1.ScalarValue}");
            
            var c2 = complexProcessor.ScalarFromRational(1, 2);
            Console.WriteLine($"✓ complexProcessor.ScalarFromRational(1, 2) works: {c2.ScalarValue}");
            
            // Test 4: Rational scalar processor
            Console.WriteLine("\nTest 4: Rational scalar processor");
            var rationalProcessor = ScalarProcessorOfERational.Instance;
            var r1 = rationalProcessor.ScalarFromRational(1, 3);
            Console.WriteLine($"✓ rationalProcessor.ScalarFromRational(1, 3) works: {r1.ScalarValue}");
            
            Console.WriteLine("\n=== All API Reference tests passed! ===");
        }
    }
}
