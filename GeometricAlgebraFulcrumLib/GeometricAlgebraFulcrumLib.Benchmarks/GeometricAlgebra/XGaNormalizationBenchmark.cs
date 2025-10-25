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
/// Benchmark for Norm and Normalization operations
/// Tests: ENorm, ENormSquared, DivideByENorm (normalization)
///
/// Goal: Validate performance for frequent operations in graphics (unit vectors, quaternions)
/// Expected: Generic<float> ~15-20% faster than Float64 Specialized
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class XGaNormalizationBenchmark
{
    private XGaFloat64Processor _procFloat64Spec = null!;
    private XGaProcessor<float> _procFloat32 = null!;
    private XGaProcessor<double> _procDouble = null!;

    // Test vectors for normalization
    private XGaFloat64Vector _vector3D_Float64 = null!;
    private XGaFloat64Vector _vector4D_Float64 = null!;
    private XGaFloat64Multivector _multivector_Float64 = null!;

    private XGaVector<float> _vector3D_Float32 = null!;
    private XGaVector<float> _vector4D_Float32 = null!;
    private XGaMultivector<float> _multivector_Float32 = null!;

    private XGaVector<double> _vector3D_Double = null!;
    private XGaVector<double> _vector4D_Double = null!;
    private XGaMultivector<double> _multivector_Double = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Float64 Specialized
        _procFloat64Spec = XGaFloat64Processor.Euclidean;

        // Generic<float>
        var float32Processor = ScalarProcessorOfFloating<float>.Instance;
        _procFloat32 = XGaProcessor<float>.CreateEuclidean(float32Processor);

        // Generic<double>
        var doubleProcessor = ScalarProcessorOfFloat64.Instance;
        _procDouble = XGaProcessor<double>.CreateEuclidean(doubleProcessor);

        // Create test data - 3D vectors (most common in graphics)
        _vector3D_Float64 = _procFloat64Spec.CreateVectorComposer()
            .SetVectorTerm(0, 3.0)
            .SetVectorTerm(1, 4.0)
            .SetVectorTerm(2, 5.0)
            .GetVector();

        _vector3D_Float32 = _procFloat32.CreateVectorComposer()
            .SetVectorTerm(0, 3f)
            .SetVectorTerm(1, 4f)
            .SetVectorTerm(2, 5f)
            .GetVector();

        _vector3D_Double = _procDouble.CreateVectorComposer()
            .SetVectorTerm(0, 3.0)
            .SetVectorTerm(1, 4.0)
            .SetVectorTerm(2, 5.0)
            .GetVector();

        // 4D vectors (quaternions)
        _vector4D_Float64 = _procFloat64Spec.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .SetVectorTerm(3, 4.0)
            .GetVector();

        _vector4D_Float32 = _procFloat32.CreateVectorComposer()
            .SetVectorTerm(0, 1.0f)
            .SetVectorTerm(1, 2.0f)
            .SetVectorTerm(2, 3.0f)
            .SetVectorTerm(3, 4.0f)
            .GetVector();

        _vector4D_Double = _procDouble.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .SetVectorTerm(3, 4.0)
            .GetVector();

        // Create multivector for general norm tests
        // Vectors are already multivectors, just cast them
        _multivector_Float64 = (XGaFloat64Multivector)_vector4D_Float64;
        _multivector_Float32 = (XGaMultivector<float>)_vector4D_Float32;
        _multivector_Double = (XGaMultivector<double>)_vector4D_Double;
    }

    // ========================================
    // Vector Norm (3D) Benchmarks
    // ========================================

    [Benchmark(Baseline = true, Description = "Vector Norm (3D) - Float64 Specialized")]
    public double VectorNorm3D_Float64Specialized()
    {
        return _vector3D_Float64.ENorm().ScalarValue;
    }

    [Benchmark(Description = "Vector Norm (3D) - Generic<float>")]
    public float VectorNorm3D_Float32()
    {
        return _vector3D_Float32.ENorm().ScalarValue;
    }

    [Benchmark(Description = "Vector Norm (3D) - Generic<double>")]
    public double VectorNorm3D_Double()
    {
        return _vector3D_Double.ENorm().ScalarValue;
    }

    // ========================================
    // Vector Norm Squared (3D) Benchmarks
    // Faster than Norm (no sqrt)
    // ========================================

    [Benchmark(Description = "Vector Norm² (3D) - Float64 Specialized")]
    public double VectorNormSquared3D_Float64Specialized()
    {
        return _vector3D_Float64.ENormSquared().ScalarValue;
    }

    [Benchmark(Description = "Vector Norm² (3D) - Generic<float>")]
    public float VectorNormSquared3D_Float32()
    {
        return _vector3D_Float32.ENormSquared().ScalarValue;
    }

    [Benchmark(Description = "Vector Norm² (3D) - Generic<double>")]
    public double VectorNormSquared3D_Double()
    {
        return _vector3D_Double.ENormSquared().ScalarValue;
    }

    // ========================================
    // Vector Normalization (3D) Benchmarks
    // Most common operation in graphics
    // ========================================

    [Benchmark(Description = "Vector Normalization (3D) - Float64 Specialized")]
    public XGaFloat64Vector VectorNormalize3D_Float64Specialized()
    {
        return _vector3D_Float64.DivideByENorm();
    }

    [Benchmark(Description = "Vector Normalization (3D) - Generic<float>")]
    public XGaVector<float> VectorNormalize3D_Float32()
    {
        return _vector3D_Float32.DivideByENorm();
    }

    [Benchmark(Description = "Vector Normalization (3D) - Generic<double>")]
    public XGaVector<double> VectorNormalize3D_Double()
    {
        return _vector3D_Double.DivideByENorm();
    }

    // ========================================
    // Vector Norm (4D/Quaternion) Benchmarks
    // ========================================

    [Benchmark(Description = "Vector Norm (4D) - Float64 Specialized")]
    public double VectorNorm4D_Float64Specialized()
    {
        return _vector4D_Float64.ENorm().ScalarValue;
    }

    [Benchmark(Description = "Vector Norm (4D) - Generic<float>")]
    public float VectorNorm4D_Float32()
    {
        return _vector4D_Float32.ENorm().ScalarValue;
    }

    [Benchmark(Description = "Vector Norm (4D) - Generic<double>")]
    public double VectorNorm4D_Double()
    {
        return _vector4D_Double.ENorm().ScalarValue;
    }

    // ========================================
    // Multivector Norm Benchmarks
    // ========================================

    [Benchmark(Description = "Multivector Norm - Float64 Specialized")]
    public double MultivectorNorm_Float64Specialized()
    {
        return _multivector_Float64.Norm().ScalarValue;
    }

    [Benchmark(Description = "Multivector Norm - Generic<float>")]
    public float MultivectorNorm_Float32()
    {
        return _multivector_Float32.Norm().ScalarValue;
    }

    [Benchmark(Description = "Multivector Norm - Generic<double>")]
    public double MultivectorNorm_Double()
    {
        return _multivector_Double.Norm().ScalarValue;
    }

    // ========================================
    // Batch Normalization (1000 vectors)
    // Realistic graphics workload
    // ========================================

    [Benchmark(Description = "Batch Normalize (1000 vectors) - Float64 Specialized")]
    public void BatchNormalize_Float64Specialized()
    {
        for (int i = 1; i <= 1000; i++)
        {
            var v = _procFloat64Spec.CreateVectorComposer()
                .SetVectorTerm(0, i)
                .SetVectorTerm(1, i + 1)
                .SetVectorTerm(2, i + 2)
                .GetVector();
            var normalized = v.DivideByENorm();
        }
    }

    [Benchmark(Description = "Batch Normalize (1000 vectors) - Generic<float>")]
    public void BatchNormalize_Float32()
    {
        for (int i = 1; i <= 1000; i++)
        {
            var v = _procFloat32.CreateVectorComposer()
                .SetVectorTerm(0, (float)i)
                .SetVectorTerm(1, (float)(i + 1))
                .SetVectorTerm(2, (float)(i + 2))
                .GetVector();
            var normalized = v.DivideByENorm();
        }
    }

    [Benchmark(Description = "Batch Normalize (1000 vectors) - Generic<double>")]
    public void BatchNormalize_Double()
    {
        for (int i = 1; i <= 1000; i++)
        {
            var v = _procDouble.CreateVectorComposer()
                .SetVectorTerm(0, (double)i)
                .SetVectorTerm(1, (double)(i + 1))
                .SetVectorTerm(2, (double)(i + 2))
                .GetVector();
            var normalized = v.DivideByENorm();
        }
    }

    // ========================================
    // Normalize + Dot Product
    // Common pattern: normalize then compute angle
    // ========================================

    [Benchmark(Description = "Normalize + Dot Product - Float64 Specialized")]
    public double NormalizeDotProduct_Float64Specialized()
    {
        var v1 = _procFloat64Spec.CreateVectorComposer()
            .SetVectorTerm(0, 3.0)
            .SetVectorTerm(1, 4.0)
            .SetVectorTerm(2, 5.0)
            .GetVector()
            .DivideByENorm();

        var v2 = _procFloat64Spec.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .GetVector()
            .DivideByENorm();

        return v1.ESp(v2).ScalarValue;
    }

    [Benchmark(Description = "Normalize + Dot Product - Generic<float>")]
    public float NormalizeDotProduct_Float32()
    {
        var v1 = _procFloat32.CreateVectorComposer()
            .SetVectorTerm(0, 3f)
            .SetVectorTerm(1, 4f)
            .SetVectorTerm(2, 5f)
            .GetVector()
            .DivideByENorm();

        var v2 = _procFloat32.CreateVectorComposer()
            .SetVectorTerm(0, 1f)
            .SetVectorTerm(1, 2f)
            .SetVectorTerm(2, 3f)
            .GetVector()
            .DivideByENorm();

        return v1.ESp(v2).ScalarValue;
    }

    [Benchmark(Description = "Normalize + Dot Product - Generic<double>")]
    public double NormalizeDotProduct_Double()
    {
        var v1 = _procDouble.CreateVectorComposer()
            .SetVectorTerm(0, 3.0)
            .SetVectorTerm(1, 4.0)
            .SetVectorTerm(2, 5.0)
            .GetVector()
            .DivideByENorm();

        var v2 = _procDouble.CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .GetVector()
            .DivideByENorm();

        return v1.ESp(v2).ScalarValue;
    }
}
