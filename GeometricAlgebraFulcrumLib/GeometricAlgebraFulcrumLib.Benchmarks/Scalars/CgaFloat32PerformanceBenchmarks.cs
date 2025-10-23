using BenchmarkDotNet.Attributes;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Floating;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;

namespace GeometricAlgebraFulcrumLib.Benchmarks.Scalars;

/// <summary>
/// Generic vs Specialized Performance Benchmarks
///
/// Goal: Compare performance of Generic<T> vs specialized Float64 implementations
/// Implementations tested:
///   1. Float32 (Generic<float>) - New generic implementation with float scalars
///   2. Float64 Specialized - Original hand-coded Float64 implementation
///   3. Generic<double> - New generic implementation with double scalars
///
/// Success Criteria:
///   - Generic<double> should be within 95-105% of Float64 Specialized (JIT optimization)
///   - Float32 should be ≥60% of Float64 (acceptable for lower precision use cases)
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class CgaFloat32PerformanceBenchmarks
{
    // ========================================
    // Float32 Setup (Generic<float>)
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
    // Float64 Specialized Setup (BASELINE)
    // ========================================

    private CGaFloat64GeometricSpace5D _float64SpecializedSpace = null!;

    // Float64 Specialized Test Data
    private CGaFloat64Blade _float64SpecCircle1 = null!;
    private CGaFloat64Blade _float64SpecCircle2 = null!;
    private CGaFloat64Blade _float64SpecSphere1 = null!;
    private CGaFloat64Blade _float64SpecSphere2 = null!;
    private CGaFloat64Blade _float64SpecPoint1 = null!;
    private CGaFloat64Blade _float64SpecPoint2 = null!;

    // ========================================
    // Generic<double> Setup
    // ========================================

    private IScalarProcessor<double> _genericDoubleProcessor = null!;
    private CGaGeometricSpace5D<double> _genericDoubleSpace = null!;

    // Generic<double> Test Data
    private CGaBlade<double> _genericDoubleCircle1 = null!;
    private CGaBlade<double> _genericDoubleCircle2 = null!;
    private CGaBlade<double> _genericDoubleSphere1 = null!;
    private CGaBlade<double> _genericDoubleSphere2 = null!;
    private CGaBlade<double> _genericDoublePoint1 = null!;
    private CGaBlade<double> _genericDoublePoint2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        // ========================================
        // Float32 Initialization (Generic<float>)
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
        // Float64 Specialized Initialization (BASELINE)
        // ========================================

        _float64SpecializedSpace = CGaFloat64GeometricSpace5D.Instance;

        // Create Float64 Specialized geometric objects (same values)
        _float64SpecCircle1 = _float64SpecializedSpace.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
        _float64SpecCircle2 = _float64SpecializedSpace.Encode.IpnsRound.Circle(3.0, 4.0, 5.0);

        _float64SpecSphere1 = _float64SpecializedSpace.Encode.IpnsRound.Sphere(10.0, 0.0, 0.0, 0.0);
        _float64SpecSphere2 = _float64SpecializedSpace.Encode.IpnsRound.Sphere(7.0, 2.0, 3.0, 4.0);

        _float64SpecPoint1 = _float64SpecializedSpace.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
        _float64SpecPoint2 = _float64SpecializedSpace.Encode.IpnsRound.Point(4.0, 5.0, 6.0);

        // ========================================
        // Generic<double> Initialization
        // ========================================

        _genericDoubleProcessor = ScalarProcessorOfFloat64.Instance;
        _genericDoubleSpace = CGaGeometricSpace5D<double>.Create(_genericDoubleProcessor);

        // Create Generic<double> geometric objects (same values)
        _genericDoubleCircle1 = _genericDoubleSpace.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
        _genericDoubleCircle2 = _genericDoubleSpace.Encode.IpnsRound.Circle(3.0, 4.0, 5.0);

        _genericDoubleSphere1 = _genericDoubleSpace.Encode.IpnsRound.Sphere(10.0, 0.0, 0.0, 0.0);
        _genericDoubleSphere2 = _genericDoubleSpace.Encode.IpnsRound.Sphere(7.0, 2.0, 3.0, 4.0);

        _genericDoublePoint1 = _genericDoubleSpace.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
        _genericDoublePoint2 = _genericDoubleSpace.Encode.IpnsRound.Point(4.0, 5.0, 6.0);
    }

    // ========================================
    // BENCHMARK 1: Circle Encoding
    // ========================================

    [Benchmark(Baseline = true, Description = "Float64 Specialized - Circle Encoding")]
    public CGaFloat64Blade Float64Specialized_CircleEncoding()
    {
        return _float64SpecializedSpace.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
    }

    [Benchmark(Description = "Generic<double> - Circle Encoding")]
    public CGaBlade<double> GenericDouble_CircleEncoding()
    {
        return _genericDoubleSpace.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
    }

    [Benchmark(Description = "Generic<float> - Circle Encoding")]
    public CGaBlade<float> Float32_CircleEncoding()
    {
        return _float32Space.Encode.IpnsRound.Circle(5f, 1f, 2f);
    }

    // ========================================
    // BENCHMARK 2: Sphere Encoding
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Sphere Encoding")]
    public CGaFloat64Blade Float64Specialized_SphereEncoding()
    {
        return _float64SpecializedSpace.Encode.IpnsRound.Sphere(10.0, 0.0, 0.0, 0.0);
    }

    [Benchmark(Description = "Generic<double> - Sphere Encoding")]
    public CGaBlade<double> GenericDouble_SphereEncoding()
    {
        return _genericDoubleSpace.Encode.IpnsRound.Sphere(10.0, 0.0, 0.0, 0.0);
    }

    [Benchmark(Description = "Generic<float> - Sphere Encoding")]
    public CGaBlade<float> Float32_SphereEncoding()
    {
        return _float32Space.Encode.IpnsRound.Sphere(10f, 0f, 0f, 0f);
    }

    // ========================================
    // BENCHMARK 3: Point Encoding
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Point Encoding")]
    public CGaFloat64Blade Float64Specialized_PointEncoding()
    {
        return _float64SpecializedSpace.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
    }

    [Benchmark(Description = "Generic<double> - Point Encoding")]
    public CGaBlade<double> GenericDouble_PointEncoding()
    {
        return _genericDoubleSpace.Encode.IpnsRound.Point(1.0, 2.0, 3.0);
    }

    [Benchmark(Description = "Generic<float> - Point Encoding")]
    public CGaBlade<float> Float32_PointEncoding()
    {
        return _float32Space.Encode.IpnsRound.Point(1f, 2f, 3f);
    }

    // ========================================
    // BENCHMARK 4: Outer Product (Circle ∧ Sphere)
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Outer Product (Circle ∧ Sphere)")]
    public CGaFloat64Blade Float64Specialized_OuterProduct_CircleSphere()
    {
        return _float64SpecCircle1.Op(_float64SpecSphere1);
    }

    [Benchmark(Description = "Generic<double> - Outer Product (Circle ∧ Sphere)")]
    public CGaBlade<double> GenericDouble_OuterProduct_CircleSphere()
    {
        return _genericDoubleCircle1.Op(_genericDoubleSphere1);
    }

    [Benchmark(Description = "Generic<float> - Outer Product (Circle ∧ Sphere)")]
    public CGaBlade<float> Float32_OuterProduct_CircleSphere()
    {
        return _float32Circle1.Op(_float32Sphere1);
    }

    /*
    // ========================================
    // BENCHMARK 5-9: Additional Operations (Commented out for quick testing)
    // ========================================
    // These can be re-enabled later if needed
    */

    // ========================================
    // BENCHMARK 10: Complex Workflow (MOST IMPORTANT)
    // ========================================

    [Benchmark(Description = "Float64 Specialized - Complex Workflow (Encode → Op → Dual → Norm)")]
    public double Float64Specialized_ComplexWorkflow()
    {
        var circle = _float64SpecializedSpace.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
        var sphere = _float64SpecializedSpace.Encode.IpnsRound.Sphere(10.0, 0.0, 0.0, 0.0);
        var result = circle.Op(sphere);
        var dual = result.CGaDual();
        return dual.Norm();  // Float64Specialized Norm() returns double directly
    }

    [Benchmark(Description = "Generic<double> - Complex Workflow (Encode → Op → Dual → Norm)")]
    public double GenericDouble_ComplexWorkflow()
    {
        var circle = _genericDoubleSpace.Encode.IpnsRound.Circle(5.0, 1.0, 2.0);
        var sphere = _genericDoubleSpace.Encode.IpnsRound.Sphere(10.0, 0.0, 0.0, 0.0);
        var result = circle.Op(sphere);
        var dual = result.CGaDual();
        return dual.Norm().ScalarValue;
    }

    [Benchmark(Description = "Generic<float> - Complex Workflow (Encode → Op → Dual → Norm)")]
    public float Float32_ComplexWorkflow()
    {
        var circle = _float32Space.Encode.IpnsRound.Circle(5f, 1f, 2f);
        var sphere = _float32Space.Encode.IpnsRound.Sphere(10f, 0f, 0f, 0f);
        var result = circle.Op(sphere);
        var dual = result.CGaDual();
        return dual.Norm().ScalarValue;
    }
}
