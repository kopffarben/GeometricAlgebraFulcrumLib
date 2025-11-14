using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

public static class ArcLengthPath3DUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RouletteMappedPath3D<T> GetRouletteMappedCurve<T>(
        this ArcLengthPath3D<T> baseCurve,
        RouletteAffineMap3D<T> rouletteMap)
    {
        return new RouletteMappedPath3D<T>(baseCurve, rouletteMap);
    }
}
