using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float32;

/// <summary>
/// Thin wrapper for CGaGeometricSpace&lt;float&gt; providing convenient
/// Float64-compatible API without code duplication.
/// </summary>
public static class CGaFloat32GeometricSpace
{
    private static readonly ScalarProcessorOfFloat32 ScalarProcessor =
        ScalarProcessorOfFloat32.Instance;

    /// <summary>
    /// 4D Conformal Geometric Algebra for 2D Euclidean Space
    /// </summary>
    public static CGaGeometricSpace4D<float> Space4D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => CGaGeometricSpace4D<float>.Create(ScalarProcessor);
    }

    /// <summary>
    /// 5D Conformal Geometric Algebra for 3D Euclidean Space
    /// </summary>
    public static CGaGeometricSpace5D<float> Space5D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => CGaGeometricSpace5D<float>.Create(ScalarProcessor);
    }

    /// <summary>
    /// Create a CGA space with custom dimensions
    /// </summary>
    /// <param name="vSpaceDimensions">Vector space dimensions (must be >= 4)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CGaGeometricSpace<float> Create(int vSpaceDimensions)
    {
        return CGaGeometricSpace<float>.Create(ScalarProcessor, vSpaceDimensions);
    }
}
