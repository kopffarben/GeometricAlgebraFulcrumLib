using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Composers;

public static class Path3DComposerUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> CreateAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Create(curve)
            .GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> CreateAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, ScalarRange<T> timeRange, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Create(timeRange, curve)
            .GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> CreateAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, bool isPeriodic, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Create(isPeriodic, curve)
            .GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> CreateAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, ScalarRange<T> timeRange, bool isPeriodic, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Create(timeRange, isPeriodic, curve)
            .GenerateTree(options);
    }
}
