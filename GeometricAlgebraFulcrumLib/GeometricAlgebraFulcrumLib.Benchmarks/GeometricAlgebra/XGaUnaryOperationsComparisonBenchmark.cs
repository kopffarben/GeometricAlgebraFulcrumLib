using BenchmarkDotNet.Attributes;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Floating;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;

namespace GeometricAlgebraFulcrumLib.Benchmarks.GeometricAlgebra;

/// <summary>
/// Unary Operations Performance Comparison: Float64 vs Generic
///
/// Goal: Compare performance of critical GA unary operations across implementations
/// Implementations tested:
///   1. Float64 Specialized - Original hand-coded Float64 implementation
///   2. Generic&lt;double&gt; - Generic implementation with double scalars
///   3. Generic&lt;float&gt; - Generic implementation with float scalars
///
/// Operations benchmarked:
///   - Reverse - Reversion (reverse order of wedge products)
///   - GradeInvolution - Negate odd-grade terms
///   - CliffordConjugate - Combination of Reverse and GradeInvolution
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class XGaUnaryOperationsComparisonBenchmark
{
    private const int Iterations = 1000;

    // ========================================
    // Float64 Specialized Setup
    // ========================================

    private XGaFloat64Processor _float64Processor = null!;
    private XGaFloat64Multivector _float64MV = null!;

    // ========================================
    // Generic<double> Setup
    // ========================================

    private XGaProcessor<double> _genericDoubleProcessor = null!;
    private XGaMultivector<double> _genericDoubleMV = null!;

    // ========================================
    // Generic<float> Setup
    // ========================================

    private XGaProcessor<float> _genericFloatProcessor = null!;
    private XGaMultivector<float> _genericFloatMV = null!;

    [GlobalSetup]
    public void Setup()
    {
        // ========================================
        // Float64 Specialized Initialization
        // ========================================

        _float64Processor = XGaFloat64Processor.Euclidean;

        // Create multivector with all grades (scalar + vector + bivector + trivector)
        _float64MV = _float64Processor
            .CreateMultivectorComposer()
            .SetScalarTerm(2.5)
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .SetBivectorTerm(0, 1, 0.5)  // e_1 ∧ e_2
            .SetBivectorTerm(1, 2, 0.7)  // e_2 ∧ e_3
            .SetBivectorTerm(0, 2, 0.3)  // e_1 ∧ e_3
            .SetTerm(new[] { 0, 1, 2 }.ToIndexSet(true), 0.9)  // e_1 ∧ e_2 ∧ e_3
            .GetMultivector();

        // ========================================
        // Generic<double> Initialization
        // ========================================

        var scalarProcessorDouble = ScalarProcessorOfFloat64.Instance;
        _genericDoubleProcessor = XGaProcessor<double>.CreateEuclidean(scalarProcessorDouble);

        _genericDoubleMV = _genericDoubleProcessor
            .CreateMultivectorComposer()
            .SetScalarTerm(2.5)
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .SetBivectorTerm(0, 1, 0.5)
            .SetBivectorTerm(1, 2, 0.7)
            .SetBivectorTerm(0, 2, 0.3)
            .SetTerm(new[] { 0, 1, 2 }.ToIndexSet(true), 0.9)
            .GetMultivector();

        // ========================================
        // Generic<float> Initialization
        // ========================================

        var scalarProcessorFloat = ScalarProcessorOfFloating<float>.Instance;
        _genericFloatProcessor = XGaProcessor<float>.CreateEuclidean(scalarProcessorFloat);

        _genericFloatMV = _genericFloatProcessor
            .CreateMultivectorComposer()
            .SetScalarTerm(2.5f)
            .SetVectorTerm(0, 1.0f)
            .SetVectorTerm(1, 2.0f)
            .SetVectorTerm(2, 3.0f)
            .SetBivectorTerm(0, 1, 0.5f)
            .SetBivectorTerm(1, 2, 0.7f)
            .SetBivectorTerm(0, 2, 0.3f)
            .SetTerm(new[] { 0, 1, 2 }.ToIndexSet(true), 0.9f)
            .GetMultivector();
    }

    // ========================================
    // Reverse Benchmarks
    // ========================================

    [Benchmark(Baseline = true, Description = "Float64 Specialized - Reverse")]
    public void Reverse_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV.Reverse();
        }
    }

    [Benchmark(Description = "Generic<double> - Reverse")]
    public void Reverse_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV.Reverse();
        }
    }

    [Benchmark(Description = "Generic<float> - Reverse")]
    public void Reverse_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV.Reverse();
        }
    }

    // ========================================
    // Grade Involution Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 Specialized - GradeInvolution")]
    public void GradeInvolution_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV.GradeInvolution();
        }
    }

    [Benchmark(Description = "Generic<double> - GradeInvolution")]
    public void GradeInvolution_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV.GradeInvolution();
        }
    }

    [Benchmark(Description = "Generic<float> - GradeInvolution")]
    public void GradeInvolution_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV.GradeInvolution();
        }
    }

    // ========================================
    // Clifford Conjugate Benchmarks
    // ========================================

    [Benchmark(Description = "Float64 Specialized - CliffordConjugate")]
    public void CliffordConjugate_Float64()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _float64MV.CliffordConjugate();
        }
    }

    [Benchmark(Description = "Generic<double> - CliffordConjugate")]
    public void CliffordConjugate_GenericDouble()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericDoubleMV.CliffordConjugate();
        }
    }

    [Benchmark(Description = "Generic<float> - CliffordConjugate")]
    public void CliffordConjugate_GenericFloat()
    {
        for (var i = 0; i < Iterations; i++)
        {
            var result = _genericFloatMV.CliffordConjugate();
        }
    }
}
