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
            var complexProcessor = ScalarProcessorOfComplex.Instance;
            var rationalProcessor = ScalarProcessorOfERational.Instance;

            // Float64 operations
            var a = float64Processor.ScalarFromNumber(3.14159);
            var b = float64Processor.ScalarFromNumber(2.71828);
            var result1 = a.Add(b).Multiply(float64Processor.ScalarFromNumber(2));

            Console.WriteLine($"Float64: (π + e) * 2 = {result1.ScalarValue:F5}");

            // Complex operations
            var complex1 = complexProcessor.ScalarFromNumbers(3, 4);  // 3 + 4i
            var complex2 = complexProcessor.ScalarFromNumbers(1, -2); // 1 - 2i
            var complexResult = complex1.Multiply(complex2);

            Console.WriteLine($"Complex: (3+4i) * (1-2i) = {complexResult}");

            // Rational arithmetic (exact)
            var rational1 = rationalProcessor.ScalarFromFraction(1, 3);  // 1/3
            var rational2 = rationalProcessor.ScalarFromFraction(2, 5);  // 2/5
            var rationalSum = rational1.Add(rational2);

            Console.WriteLine($"Rational: 1/3 + 2/5 = {rationalSum}");
        }
    }
}
