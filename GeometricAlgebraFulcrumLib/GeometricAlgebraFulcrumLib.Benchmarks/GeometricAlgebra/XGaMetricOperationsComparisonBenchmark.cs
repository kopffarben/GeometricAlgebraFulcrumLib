using BenchmarkDotNet.Attributes;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Floating;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Benchmarks.GeometricAlgebra;

/// <summary>
/// Metric Operations Performance Comparison: Float64 vs Generic
///
/// Goal: Compare performance of metric-dependent operations across implementations
/// Tests different metric signatures:
///   - Euclidean (p, 0, 0) - Standard positive-definite metric
///   - Projective (p+1, 0, 0) - Projective geometry
///   - Conformal (p+1, 1, 0) - Conformal geometry (most complex)
///
/// Implementations tested:
///   1. Float64 Specialized - Original hand-coded Float64 implementation
///   2. Generic&lt;double&gt; - Generic implementation with double scalars
///   3. Generic&lt;float&gt; - Generic implementation with float scalars
///
/// Operations benchmarked:
///   - Gp (Geometric Product with metric)
///   - Sp (Scalar Product with metric)
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class XGaMetricOperationsComparisonBenchmark
{
    private const int Iterations = 100;

    // ========================================
    // Float64 Specialized - Euclidean
    // ========================================

    private XGaFloat64Processor _float64Euclidean = null!;
    private XGaFloat64Vector _float64EucV1 = null!;
    private XGaFloat64Vector _float64EucV2 = null!;

    // ========================================
    // Float64 Specialized - Conformal (3,1,0)
    // ========================================

    private XGaFloat64Processor _float64Conformal = null!;
    private XGaFloat64Vector _float64ConfV1 = null!;
    private XGaFloat64Vector _float64ConfV2 = null!;

    // ========================================
    // Generic<double> - Euclidean
    // ========================================

    private XGaProcessor<double> _genericDoubleEuclidean = null!;
    private XGaVector<double> _genericDoubleEucV1 = null!;
    private XGaVector<double> _genericDoubleEucV2 = null!;

    // ========================================
    // Generic<double> - Conformal (3,1,0)
    // ========================================

    private XGaProcessor<double> _genericDoubleConformal = null!;
    private XGaVector<double> _genericDoubleConfV1 = null!;
    private XGaVector<double> _genericDoubleConfV2 = null!;

    // ========================================
    // Generic<float> - Euclidean
    // ========================================

    private XGaProcessor<float> _genericFloatEuclidean = null!;
    private XGaVector<float> _genericFloatEucV1 = null!;
    private XGaVector<float> _genericFloatEucV2 = null!;

    // ========================================
    // Generic<float> - Conformal (3,1,0)
    // ========================================

    private XGaProcessor<float> _genericFloatConformal = null!;
    private XGaVector<float> _genericFloatConfV1 = null!;
    private XGaVector<float> _genericFloatConfV2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        // ========================================
        // Float64 Specialized - Euclidean Setup
        // ========================================

        _float64Euclidean = XGaFloat64Processor.Euclidean;

        _float64EucV1 = _float64Euclidean.Vector(1.0, 2.0, 3.0);
        _float64EucV2 = _float64Euclidean.Vector(4.0, 5.0, 6.0);

        // ========================================
        // Float64 Specialized - Conformal (4,1,0) Setup
        // ========================================

        _float64Conformal = XGaFloat64Processor.Conformal;

        _float64ConfV1 = _float64Conformal.Vector(1.0, 2.0, 3.0, 0.5, 0.5);
        _float64ConfV2 = _float64Conformal.Vector(4.0, 5.0, 6.0, 0.7, 0.3);

        // ========================================
        // Generic<double> - Euclidean Setup
        // ========================================

        var scalarProcessorDouble = ScalarProcessorOfFloat64.Instance;
        _genericDoubleEuclidean = XGaProcessor<double>.CreateEuclidean(scalarProcessorDouble);

        _genericDoubleEucV1 = _genericDoubleEuclidean.Vector(1.0, 2.0, 3.0);
        _genericDoubleEucV2 = _genericDoubleEuclidean.Vector(4.0, 5.0, 6.0);

        // ========================================
        // Generic<double> - Conformal (4,1,0) Setup
        // ========================================

        _genericDoubleConformal = XGaProcessor<double>.CreateConformal(scalarProcessorDouble);

        _genericDoubleConfV1 = _genericDoubleConformal.Vector(1.0, 2.0, 3.0, 0.5, 0.5);
        _genericDoubleConfV2 = _genericDoubleConformal.Vector(4.0, 5.0, 6.0, 0.7, 0.3);

        // ========================================
        // Generic<float> - Euclidean Setup
        // ========================================

        var scalarProcessorFloat = ScalarProcessorOfFloating<float>.Instance;
        _genericFloatEuclidean = XGaProcessor<float>.CreateEuclidean(scalarProcessorFloat);

        _genericFloatEucV1 = _genericFloatEuclidean.Vector(1.0f, 2.0f, 3.0f);
        _genericFloatEucV2 = _genericFloatEuclidean.Vector(4.0f, 5.0f, 6.0f);

        // ========================================
        // Generic<float> - Conformal (4,1,0) Setup
        // ========================================

        _genericFloatConformal = XGaProcessor<float>.CreateConformal(scalarProcessorFloat);

        _genericFloatConfV1 = _genericFloatConformal.Vector(1.0f, 2.0f, 3.0f, 0.5f, 0.5f);
        _genericFloatConfV2 = _genericFloatConformal.Vector(4.0f, 5.0f, 6.0f, 0.7f, 0.3f);
    }

    // ========================================
    // Euclidean - Geometric Product (Gp) Benchmarks
    // ========================================

    [Benchmark(Baseline = true, Description = "Float64 - Euclidean Gp")]
    public void EuclideanGp_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64EucV1.Gp(_float64EucV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Euclidean Gp")]
    public void EuclideanGp_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleEucV1.Gp(_genericDoubleEucV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Euclidean Gp")]
    public void EuclideanGp_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatEucV1.Gp(_genericFloatEucV2);
        }
    }

    // ========================================
    // Conformal - Geometric Product (Gp) Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 - Conformal Gp")]
    public void ConformalGp_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64ConfV1.Gp(_float64ConfV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Conformal Gp")]
    public void ConformalGp_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleConfV1.Gp(_genericDoubleConfV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Conformal Gp")]
    public void ConformalGp_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatConfV1.Gp(_genericFloatConfV2);
        }
    }

    // ========================================
    // Euclidean - Scalar Product (Sp) Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 - Euclidean Sp")]
    public void EuclideanSp_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64EucV1.Sp(_float64EucV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Euclidean Sp")]
    public void EuclideanSp_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleEucV1.Sp(_genericDoubleEucV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Euclidean Sp")]
    public void EuclideanSp_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatEucV1.Sp(_genericFloatEucV2);
        }
    }

    // ========================================
    // Conformal - Scalar Product (Sp) Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 - Conformal Sp")]
    public void ConformalSp_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64ConfV1.Sp(_float64ConfV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Conformal Sp")]
    public void ConformalSp_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleConfV1.Sp(_genericDoubleConfV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Conformal Sp")]
    public void ConformalSp_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatConfV1.Sp(_genericFloatConfV2);
        }
    }
}
