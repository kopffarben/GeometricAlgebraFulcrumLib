using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GAExamples
{
    class ScalarOperations
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Scalar Processor Examples ===\n");

            // Demonstrate different scalar processor types
            var float64Processor = ScalarProcessorOfFloat64.Instance;
            var rationalProcessor = ScalarProcessorOfERational.Instance;

            // Float64 operations - use operator overloading
            var a = float64Processor.ScalarFromNumber(3.14159);
            var b = float64Processor.ScalarFromNumber(2.71828);
            var result1 = (a + b) * 2;

            Console.WriteLine($"Float64: (π + e) * 2 = {result1.ScalarValue:F5}");

            // Rational arithmetic (exact)
            var rational1 = rationalProcessor.ScalarFromRational(1, 3);  // 1/3
            var rational2 = rationalProcessor.ScalarFromRational(2, 5);  // 2/5
            var rationalSum = rational1 + rational2;

            Console.WriteLine($"Rational: 1/3 + 2/5 = {rationalSum.ScalarValue}");

            // More float64 operations
            var c = float64Processor.ScalarFromNumber(5.0);
            var d = float64Processor.ScalarFromNumber(3.0);
            Console.WriteLine($"\nFloat64: 5 + 3 = {(c + d).ScalarValue}");
            Console.WriteLine($"Float64: 5 - 3 = {(c - d).ScalarValue}");
            Console.WriteLine($"Float64: 5 * 3 = {(c * d).ScalarValue}");
            Console.WriteLine($"Float64: 5 / 3 = {(c / d).ScalarValue:F5}");
        }
    }
}
