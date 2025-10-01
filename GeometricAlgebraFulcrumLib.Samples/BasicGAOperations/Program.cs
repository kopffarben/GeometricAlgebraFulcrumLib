using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;

namespace GAExamples
{
    class BasicGAOperations
    {
        static void Main(string[] args)
        {
            // 1. Create scalar processor for double precision
            var scalarProcessor = ScalarProcessorOfFloat64.Instance;

            // 2. Create 3D Euclidean GA processor
            var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

            // 3. Create vectors
            var v1 = processor.Vector(1, 2, 3);
            var v2 = processor.Vector(4, 5, 6);

            Console.WriteLine("=== Basic Geometric Algebra Operations ===");
            Console.WriteLine($"v1 = {v1}");
            Console.WriteLine($"v2 = {v2}");
            Console.WriteLine();

            // 4. Perform GA operations
            var outerProduct = v1.Op(v2);        // Outer product → bivector
            var geometricProduct = v1.Gp(v2);    // Geometric product → scalar + bivector
            var scalarProduct = v1.Sp(v2);       // Scalar product (inner product) → 32.0

            Console.WriteLine($"v1 ∧ v2 (outer product) = {outerProduct}");
            Console.WriteLine($"v1 * v2 (geometric product) = {geometricProduct}");
            Console.WriteLine($"v1 · v2 (scalar product) = {scalarProduct:F1}");
            Console.WriteLine();

            // 5. Additional operations
            var v1Magnitude = v1.Norm();
            var v2Magnitude = v2.Norm();
            var dotProduct = v1.Sp(v2).ScalarValue;
            var angle = Math.Acos(dotProduct / (v1Magnitude.ScalarValue * v2Magnitude.ScalarValue));

            Console.WriteLine($"|v1| = {v1Magnitude.ScalarValue:F3}");
            Console.WriteLine($"|v2| = {v2Magnitude.ScalarValue:F3}");
            Console.WriteLine($"Angle between v1 and v2 = {angle * 180 / Math.PI:F1}°");

            // 6. Test orthogonal vectors
            var e1 = processor.Vector(1, 0, 0);
            var e2 = processor.Vector(0, 1, 0);
            var e3 = processor.Vector(0, 0, 1);

            Console.WriteLine("\n=== Orthogonal Basis Vectors ===");
            Console.WriteLine($"e1 ∧ e2 = {e1.Op(e2)}");
            Console.WriteLine($"e2 ∧ e3 = {e2.Op(e3)}");
            Console.WriteLine($"e3 ∧ e1 = {e3.Op(e1)}");

            // 7. Volume calculation using trivector
            var volume = e1.Op(e2).Op(e3);
            Console.WriteLine($"e1 ∧ e2 ∧ e3 (unit volume) = {volume}");
        }
    }
}
