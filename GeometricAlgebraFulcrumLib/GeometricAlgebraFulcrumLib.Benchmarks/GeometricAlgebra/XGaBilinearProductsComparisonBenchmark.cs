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
/// Bilinear Products Performance Comparison: Float64 vs Generic
///
/// Goal: Compare performance of critical GA bilinear products across implementations
/// Implementations tested:
///   1. Float64 Specialized - Original hand-coded Float64 implementation
///   2. Generic&lt;double&gt; - Generic implementation with double scalars
///   3. Generic&lt;float&gt; - Generic implementation with float scalars
///
/// Operations benchmarked:
///   - Gp (Geometric Product) - Most fundamental GA operation
///   - Op (Outer Product) - Wedge product
///   - Sp (Scalar Product) - Inner product with metric
///   - Lcp (Left Contraction) - Left-sided inner product
///   - Rcp (Right Contraction) - Right-sided inner product
///   - Cp (Commutator Product) - [A,B] = (AB - BA)/2
///   - Acp (Anti-Commutator Product) - {A,B} = (AB + BA)/2
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class XGaBilinearProductsComparisonBenchmark
{
    private const int VSpaceDimensions = 5; // 5D space (32 basis blades)
    private const int Iterations = 100;

    // ========================================
    // Float64 Specialized Setup
    // ========================================

    private XGaFloat64Processor _float64Processor = null!;
    private XGaFloat64Multivector _float64MV1 = null!;
    private XGaFloat64Multivector _float64MV2 = null!;

    // ========================================
    // Generic<double> Setup
    // ========================================

    private XGaProcessor<double> _genericDoubleProcessor = null!;
    private XGaMultivector<double> _genericDoubleMV1 = null!;
    private XGaMultivector<double> _genericDoubleMV2 = null!;

    // ========================================
    // Generic<float> Setup
    // ========================================

    private XGaProcessor<float> _genericFloatProcessor = null!;
    private XGaMultivector<float> _genericFloatMV1 = null!;
    private XGaMultivector<float> _genericFloatMV2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        // ========================================
        // Float64 Specialized Initialization
        // ========================================

        _float64Processor = XGaFloat64Processor.Euclidean;

        // Create multivectors with mixed grades (scalar + vector + bivector)
        _float64MV1 = _float64Processor
            .CreateMultivectorComposer()
            .SetScalarTerm(2.5)
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .SetBivectorTerm(0, 1, 0.5)  // e_1 ∧ e_2
            .SetBivectorTerm(1, 2, 0.7)  // e_2 ∧ e_3
            .GetMultivector();

        _float64MV2 = _float64Processor
            .CreateMultivectorComposer()
            .SetScalarTerm(1.5)
            .SetVectorTerm(0, 4.0)
            .SetVectorTerm(1, 5.0)
            .SetVectorTerm(2, 6.0)
            .SetBivectorTerm(0, 2, 0.3)  // e_1 ∧ e_3
            .SetBivectorTerm(1, 2, 0.9)  // e_2 ∧ e_3
            .GetMultivector();

        // ========================================
        // Generic<double> Initialization
        // ========================================

        var scalarProcessorDouble = ScalarProcessorOfFloat64.Instance;
        _genericDoubleProcessor = XGaProcessor<double>.CreateEuclidean(scalarProcessorDouble);

        _genericDoubleMV1 = _genericDoubleProcessor
            .CreateMultivectorComposer()
            .SetScalarTerm(2.5)
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .SetBivectorTerm(0, 1, 0.5)
            .SetBivectorTerm(1, 2, 0.7)
            .GetMultivector();

        _genericDoubleMV2 = _genericDoubleProcessor
            .CreateMultivectorComposer()
            .SetScalarTerm(1.5)
            .SetVectorTerm(0, 4.0)
            .SetVectorTerm(1, 5.0)
            .SetVectorTerm(2, 6.0)
            .SetBivectorTerm(0, 2, 0.3)
            .SetBivectorTerm(1, 2, 0.9)
            .GetMultivector();

        // ========================================
        // Generic<float> Initialization
        // ========================================

        var scalarProcessorFloat = ScalarProcessorOfFloating<float>.Instance;
        _genericFloatProcessor = XGaProcessor<float>.CreateEuclidean(scalarProcessorFloat);

        _genericFloatMV1 = _genericFloatProcessor
            .CreateMultivectorComposer()
            .SetScalarTerm(2.5f)
            .SetVectorTerm(0, 1.0f)
            .SetVectorTerm(1, 2.0f)
            .SetVectorTerm(2, 3.0f)
            .SetBivectorTerm(0, 1, 0.5f)
            .SetBivectorTerm(1, 2, 0.7f)
            .GetMultivector();

        _genericFloatMV2 = _genericFloatProcessor
            .CreateMultivectorComposer()
            .SetScalarTerm(1.5f)
            .SetVectorTerm(0, 4.0f)
            .SetVectorTerm(1, 5.0f)
            .SetVectorTerm(2, 6.0f)
            .SetBivectorTerm(0, 2, 0.3f)
            .SetBivectorTerm(1, 2, 0.9f)
            .GetMultivector();
    }

    // ========================================
    // Geometric Product (Gp) Benchmarks
    // ========================================

    [Benchmark(Baseline = true, Description = "Float64 Specialized - Gp")]
    public void GeometricProduct_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV1.Gp(_float64MV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Gp")]
    public void GeometricProduct_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV1.Gp(_genericDoubleMV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Gp")]
    public void GeometricProduct_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV1.Gp(_genericFloatMV2);
        }
    }

    // ========================================
    // Outer Product (Op) Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Op")]
    public void OuterProduct_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV1.Op(_float64MV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Op")]
    public void OuterProduct_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV1.Op(_genericDoubleMV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Op")]
    public void OuterProduct_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV1.Op(_genericFloatMV2);
        }
    }

    // ========================================
    // Scalar Product (Sp) Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Sp")]
    public void ScalarProduct_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV1.Sp(_float64MV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Sp")]
    public void ScalarProduct_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV1.Sp(_genericDoubleMV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Sp")]
    public void ScalarProduct_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV1.Sp(_genericFloatMV2);
        }
    }

    // ========================================
    // Left Contraction (Lcp) Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Lcp")]
    public void LeftContraction_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV1.Lcp(_float64MV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Lcp")]
    public void LeftContraction_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV1.Lcp(_genericDoubleMV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Lcp")]
    public void LeftContraction_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV1.Lcp(_genericFloatMV2);
        }
    }

    // ========================================
    // Right Contraction (Rcp) Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Rcp")]
    public void RightContraction_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV1.Rcp(_float64MV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Rcp")]
    public void RightContraction_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV1.Rcp(_genericDoubleMV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Rcp")]
    public void RightContraction_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV1.Rcp(_genericFloatMV2);
        }
    }

    // ========================================
    // Commutator Product (Cp) Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Cp")]
    public void CommutatorProduct_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV1.Cp(_float64MV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Cp")]
    public void CommutatorProduct_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV1.Cp(_genericDoubleMV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Cp")]
    public void CommutatorProduct_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV1.Cp(_genericFloatMV2);
        }
    }

    // ========================================
    // Anti-Commutator Product (Acp) Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Acp")]
    public void AntiCommutatorProduct_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV1.Acp(_float64MV2);
        }
    }

    [Benchmark(Description = "Generic<double> - Acp")]
    public void AntiCommutatorProduct_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV1.Acp(_genericDoubleMV2);
        }
    }

    [Benchmark(Description = "Generic<float> - Acp")]
    public void AntiCommutatorProduct_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV1.Acp(_genericFloatMV2);
        }
    }
}
