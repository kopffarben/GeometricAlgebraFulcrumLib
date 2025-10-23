using BenchmarkDotNet.Running;
using GeometricAlgebraFulcrumLib.Benchmarks.Applications;
using GeometricAlgebraFulcrumLib.Benchmarks.GeometricAlgebra;

namespace GeometricAlgebraFulcrumLib.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        // Use BenchmarkSwitcher to support command-line filters
        // e.g., dotnet run --filter "*CgaFloat32*"
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}