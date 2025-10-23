using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.PGa.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.PGa.Float32;

/// <summary>
/// Thin wrapper for PGaGeometricSpace&lt;float&gt; providing convenient
/// Float64-compatible API without code duplication.
/// </summary>
public static class PGaFloat32GeometricSpace
{
    private static readonly ScalarProcessorOfFloat32 ScalarProcessor =
        ScalarProcessorOfFloat32.Instance;

    /// <summary>
    /// 4D Projective Geometric Algebra for 3D Euclidean Space
    /// </summary>
    public static PGaGeometricSpace3D<float> Space4D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => PGaGeometricSpace<float>.Create4D(ScalarProcessor);
    }

    /// <summary>
    /// 5D Projective Geometric Algebra for 4D Euclidean Space
    /// </summary>
    public static PGaGeometricSpace4D<float> Space5D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => PGaGeometricSpace<float>.Create5D(ScalarProcessor);
    }

    /// <summary>
    /// Create a PGA space with custom dimensions
    /// </summary>
    /// <param name="vSpaceDimensions">Vector space dimensions (must be >= 3)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PGaGeometricSpace<float> Create(int vSpaceDimensions)
    {
        return PGaGeometricSpace<float>.Create(ScalarProcessor, vSpaceDimensions);
    }
}
