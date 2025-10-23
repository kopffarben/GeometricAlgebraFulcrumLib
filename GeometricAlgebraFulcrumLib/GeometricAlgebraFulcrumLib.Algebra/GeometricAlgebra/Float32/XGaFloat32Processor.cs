using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float32;

/// <summary>
/// Thin wrapper for XGaProcessor&lt;float&gt; providing Float64-compatible API
/// without code duplication. Uses generic implementation internally.
/// </summary>
public static class XGaFloat32Processor
{
    private static readonly ScalarProcessorOfFloat32 ScalarProcessor =
        ScalarProcessorOfFloat32.Instance;

    /// <summary>
    /// Euclidean metric processor (all positive signatures)
    /// </summary>
    public static XGaProcessor<float> Euclidean
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => XGaProcessor<float>.CreateEuclidean(ScalarProcessor);
    }

    /// <summary>
    /// Projective metric processor (1 zero signature)
    /// </summary>
    public static XGaProcessor<float> Projective
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => XGaProcessor<float>.CreateProjective(ScalarProcessor);
    }

    /// <summary>
    /// Conformal metric processor (1 negative signature)
    /// </summary>
    public static XGaConformalProcessor<float> Conformal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => XGaProcessor<float>.CreateConformal(ScalarProcessor);
    }

    /// <summary>
    /// Create a processor with custom metric signature
    /// </summary>
    /// <param name="negativeCount">Number of negative signature basis vectors</param>
    /// <param name="zeroCount">Number of zero signature basis vectors</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XGaProcessor<float> Create(int negativeCount, int zeroCount)
    {
        return XGaProcessor<float>.Create(ScalarProcessor, negativeCount, zeroCount);
    }

    /// <summary>
    /// Create a processor with custom metric signature (p, q, r notation)
    /// </summary>
    /// <param name="p">Number of positive signature basis vectors</param>
    /// <param name="q">Number of negative signature basis vectors</param>
    /// <param name="r">Number of zero signature basis vectors</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XGaProcessor<float> Create(int p, int q, int r = 0)
    {
        return XGaProcessor<float>.Create(ScalarProcessor, q, r);
    }
}
