using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Optimization.GradientDescent;

namespace GeometricAlgebraFulcrumLib.Optimization.Samples
{
    public static class GradientDescentSamples
    {
        /// <summary>
        /// Demonstrates the performance comparison between CPU SGD and GPU SGD implementations
        /// on a large synthetic dataset with 100,000 samples and 1,000 features.
        /// </summary>
        public static void PerformanceComparisonExample()
        {
            const int numRows = 100000; // number of samples
            const int numCols = 1000; // number of features
            // Generate random training data
            var x = new double[numRows, numCols];
            var y = new double[numRows];
            var trueWeights = new double[numCols];

            var rand = new Random(42); // Fixed seed for reproducibility

            // Fill feature matrix with random values
            for (var i = 0; i < numRows; i++)
            {
                for (var j = 0; j < numCols; j++)
                {
                    x[i, j] = rand.NextDouble();
                }
            }

            // Generate true weights
            for (var i = 0; i < numCols; i++)
            {
                trueWeights[i] = rand.NextDouble() * 10;
            }

            // Generate target values using true weights
            for (var i = 0; i < numRows; i++)
            {
                y[i] = 0;
                for (var j = 0; j < numCols; j++)
                {
                    y[i] += x[i, j] * trueWeights[j];
                }
            }

            var stopwatch = new Stopwatch();
            var sgd = new GpuSgd(x, y);

            Console.WriteLine("Performance Comparison: CPU SGD vs GPU SGD");
            Console.WriteLine("==========================================");
            
            // Test CPU SGD
            stopwatch.Start();
            var cpuError = sgd.CompareWeights(trueWeights, sgd.StochasticGradientDescentAdj(x, y, 0.01, 10000, 1000));
            stopwatch.Stop();
            
            Console.WriteLine($"CPU SGD Error: {cpuError:F6}");
            Console.WriteLine($"CPU SGD Time: {stopwatch.ElapsedMilliseconds} ms");
            
            stopwatch.Reset();

            // Test GPU SGD
            stopwatch.Start();
            var gpuError = sgd.CompareWeights(trueWeights, sgd.SgDgpu(x, y, 0.01, 10000, 1000));
            stopwatch.Stop();
            
            Console.WriteLine($"GPU SGD Error: {gpuError:F6}");
            Console.WriteLine($"GPU SGD Time: {stopwatch.ElapsedMilliseconds} ms");

        }
    }
}
