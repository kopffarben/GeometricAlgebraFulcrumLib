using BenchmarkDotNet.Attributes;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Floating;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;

namespace GeometricAlgebraFulcrumLib.Benchmarks.Scalars;

/// <summary>
/// Phase 0b: Float32 PoC & Performance Validation Benchmarks
///
/// Goal: Measure Float32 CGa performance vs Float64 baseline
/// Success Criteria: Float32 ≥60% of Float64 performance (realistic workloads)
/// GO/NO-GO Decision: If <60% → Abort Float32 workflow
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class CgaFloat32PerformanceBenchmarks
{
    // ========================================
    // Float32 Setup
    // ========================================

    private IScalarProcessor<float> _float32Processor = null!;
    private CGaGeometricSpace5D<float> _float32Space = null!;

    // Float32 Test Data
    private CGaBlade<float> _float32Circle1 = null!;
    private CGaBlade<float> _float32Circle2 = null!;
    private CGaBlade<float> _float32Sphere1 = null!;
    private CGaBlade<float> _float32Sphere2 = null!;
    private CGaBlade<float> _float32Point1 = null!;
    private CGaBlade<float> _float32Point2 = null!;

    // ========================================
    // Float64 Setup (Baseline)
    // ========================================

    private IScalarProcessor<double> _float64Processor = null!;
    private CGaGeometricSpace5D<double> _float64Space = null!;

    // Float64 Test Data (same geometric values as Float32 for fair comparison)
    private CGaBlade<double> _float64Circle1 = null!;
    private CGaBlade<double> _float64Circle2 = null!;
    private CGaBlade<double> _float64Sphere1 = null!;
    private CGaBlade<double> _float64Sphere2 = null!;
    private CGaBlade<double> _float64Point1 = null!;
    private CGaBlade<double> _float64Point2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        // ========================================
        // Float32 Initialization
        // ========================================

        _float32Processor = ScalarProcessorOfFloating<float>.Instance;
        _float32Space = CGaGeometricSpace5D<float>.Create(_float32Processor);

        // Create Float32 geometric objects
        // Circle: radius=5, center=(1, 2, 0)
        _float32Circle1 = _float32Space.Encode.IpnsRound.Circle(5f, 1f, 2f);
        _float32Circle2 = _float32Space.Encode.IpnsRound.Circle(3f, 4f, 5f);

        // Sphere: radius=10, center=(0, 0, 0)
        _float32Sphere1 = _float32Space.Encode.IpnsRound.Sphere(10f, 0f, 0f, 0f);
        _float32Sphere2 = _float32Space.Encode.IpnsRound.Sphere(7f, 2f, 3f, 4f);

        // Points
        _float32Point1 = _float32Space.Encode.IpnsRound.Point(1f, 2f, 3f);
        _float32Point2 = _float32Space.Encode.IpnsRound.Point(4f, 5f, 6f);

        // ========================================
        // Float64 Initialization (Baseline)
        // ========================================

        _float64Processor = ScalarProcessorOfFloat64.Instance;
        _float64Space = CGaGeometricSpace5D<double>.Create(_float64Processor);

        // Create Float64 geometric objects (same values)
        _float64Circle1 = _float64Space.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
        _float64Circle2 = _float64Space.Encode.IpnsRound.Circle(3.0, 4.0, 5.0);

        _float64Sphere1 = _float64Space.Encode.IpnsRound.Sphere(10.0, 0.0, 0.0, 0.0);
        _float64Sphere2 = _float64Space.Encode.IpnsRound.Sphere(7.0, 2.0, 3.0, 4.0);

        _float64Point1 = _float64Space.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
        _float64Point2 = _float64Space.Encode.IpnsRound.Point(4.0, 5.0, 6.0);
    }

    // ========================================
    // BENCHMARK 1: Circle Encoding
    // ========================================

    [Benchmark(Baseline = true, Description = "Float64 - Circle Encoding")]
    public CGaBlade<double> Float64_CircleEncoding()
    {
        return _float64Space.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
    }

    [Benchmark(Description = "Float32 - Circle Encoding")]
    public CGaBlade<float> Float32_CircleEncoding()
    {
        return _float32Space.Encode.IpnsRound.Circle(5f, 1f, 2f);
    }

    // ========================================
    // BENCHMARK 2: Sphere Encoding
    // ========================================

    [Benchmark(Description = "Float64 - Sphere Encoding")]
    public CGaBlade<double> Float64_SphereEncoding()
    {
        return _float64Space.Encode.IpnsRound.Sphere(10.0, 0.0, 0.0, 0.0);
    }

    [Benchmark(Description = "Float32 - Sphere Encoding")]
    public CGaBlade<float> Float32_SphereEncoding()
    {
        return _float32Space.Encode.IpnsRound.Sphere(10f, 0f, 0f, 0f);
    }

    // ========================================
    // BENCHMARK 3: Point Encoding
    // ========================================

    [Benchmark(Description = "Float64 - Point Encoding")]
    public CGaBlade<double> Float64_PointEncoding()
    {
        return _float64Space.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
    }

    [Benchmark(Description = "Float32 - Point Encoding")]
    public CGaBlade<float> Float32_PointEncoding()
    {
        return _float32Space.Encode.IpnsRound.Point(1f, 2f, 3f);
    }

    // ========================================
    // BENCHMARK 4: Geometric Product (Circle ∧ Sphere)
    // ========================================

    [Benchmark(Description = "Float64 - Outer Product (Circle ∧ Sphere)")]
    public CGaBlade<double> Float64_OuterProduct_CircleSphere()
    {
        return _float64Circle1.Op(_float64Sphere1);
    }

    [Benchmark(Description = "Float32 - Outer Product (Circle ∧ Sphere)")]
    public CGaBlade<float> Float32_OuterProduct_CircleSphere()
    {
        return _float32Circle1.Op(_float32Sphere1);
    }

    // ========================================
    // BENCHMARK 5: Dual Operation
    // ========================================

    [Benchmark(Description = "Float64 - Dual (Circle)")]
    public CGaBlade<double> Float64_Dual_Circle()
    {
        return _float64Circle1.CGaDual();
    }

    [Benchmark(Description = "Float32 - Dual (Circle)")]
    public CGaBlade<float> Float32_Dual_Circle()
    {
        return _float32Circle1.CGaDual();
    }

    // ========================================
    // BENCHMARK 6: Norm Calculation
    // ========================================

    [Benchmark(Description = "Float64 - Norm (Sphere)")]
    public double Float64_Norm_Sphere()
    {
        return _float64Sphere1.Norm().ScalarValue;
    }

    [Benchmark(Description = "Float32 - Norm (Sphere)")]
    public float Float32_Norm_Sphere()
    {
        return _float32Sphere1.Norm().ScalarValue;
    }

    // ========================================
    // BENCHMARK 7: Reverse Operation
    // ========================================

    [Benchmark(Description = "Float64 - Reverse (Circle)")]
    public CGaBlade<double> Float64_Reverse_Circle()
    {
        return _float64Circle1.Reverse();
    }

    [Benchmark(Description = "Float32 - Reverse (Circle)")]
    public CGaBlade<float> Float32_Reverse_Circle()
    {
        return _float32Circle1.Reverse();
    }

    // ========================================
    // BENCHMARK 8: Conjugate Operation
    // ========================================

    [Benchmark(Description = "Float64 - Conjugate (Sphere)")]
    public CGaBlade<double> Float64_Conjugate_Sphere()
    {
        return _float64Sphere1.CliffordConjugate();
    }

    [Benchmark(Description = "Float32 - Conjugate (Sphere)")]
    public CGaBlade<float> Float32_Conjugate_Sphere()
    {
        return _float32Sphere1.CliffordConjugate();
    }

    // ========================================
    // BENCHMARK 9: Inner Product (Point • Sphere)
    // ========================================

    [Benchmark(Description = "Float64 - Inner Product (Point • Sphere)")]
    public CGaBlade<double> Float64_InnerProduct_PointSphere()
    {
        return _float64Point1.Lcp(_float64Sphere1);
    }

    [Benchmark(Description = "Float32 - Inner Product (Point • Sphere)")]
    public CGaBlade<float> Float32_InnerProduct_PointSphere()
    {
        return _float32Point1.Lcp(_float32Sphere1);
    }

    // ========================================
    // BENCHMARK 10: Geometric Product (Complex Workflow)
    // ========================================

    [Benchmark(Description = "Float64 - Complex Workflow (Encode → Op → Dual → Norm)")]
    public double Float64_ComplexWorkflow()
    {
        var circle = _float64Space.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
        var sphere = _float64Space.Encode.IpnsRound.Sphere(10.0, 0.0, 0.0, 0.0);
        var result = circle.Op(sphere);
        var dual = result.CGaDual();
        return dual.Norm().ScalarValue;
    }

    [Benchmark(Description = "Float32 - Complex Workflow (Encode → Op → Dual → Norm)")]
    public float Float32_ComplexWorkflow()
    {
        var circle = _float32Space.Encode.IpnsRound.Circle(5f, 1f, 2f);
        var sphere = _float32Space.Encode.IpnsRound.Sphere(10f, 0f, 0f, 0f);
        var result = circle.Op(sphere);
        var dual = result.CGaDual();
        return dual.Norm().ScalarValue;
    }
}
