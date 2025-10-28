using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

/// <summary>
/// Arc-length parameterized 3D path with generic scalar type T.
/// Provides methods for converting between time parameter and arc length.
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public abstract class ArcLengthPath3D<T>(ScalarRange<T> timeRange, bool isPeriodic) :
    ParametricPath3D<T>(timeRange, isPeriodic)
{
    /// <summary>
    /// Convert this path to a finite (non-periodic) arc-length parameterized path
    /// </summary>
    public abstract ArcLengthPath3D<T> ToFiniteArcLengthPath();

    /// <summary>
    /// Convert this path to a periodic arc-length parameterized path
    /// </summary>
    public abstract ArcLengthPath3D<T> ToPeriodicArcLengthPath();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return ToFiniteArcLengthPath();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return ToPeriodicArcLengthPath();
    }


    /// <summary>
    /// Get the total arc length of this path
    /// </summary>
    public abstract Scalar<T> GetLength();

    /// <summary>
    /// Convert time parameter value to arc length
    /// </summary>
    public abstract Scalar<T> TimeToLength(Scalar<T> t);

    /// <summary>
    /// Convert arc length to time parameter value
    /// </summary>
    public abstract Scalar<T> LengthToTime(Scalar<T> length);
}
