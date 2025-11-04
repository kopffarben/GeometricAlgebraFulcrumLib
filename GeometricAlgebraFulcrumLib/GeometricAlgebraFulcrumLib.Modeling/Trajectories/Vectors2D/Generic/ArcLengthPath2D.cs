using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic;

/// <summary>
/// Base class for 2D curves that support arc-length parameterization
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public abstract class ArcLengthPath2D<T>(ScalarRange<T> timeRange, bool isPeriodic) :
    ParametricPath2D<T>(timeRange, isPeriodic)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToFinitePath()
    {
        return ToFiniteArcLengthPath();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return ToPeriodicArcLengthPath();
    }

    public abstract ArcLengthPath2D<T> ToFiniteArcLengthPath();

    public abstract ArcLengthPath2D<T> ToPeriodicArcLengthPath();


    /// <summary>
    /// Get the total arc length of the curve
    /// </summary>
    public abstract Scalar<T> GetLength();

    /// <summary>
    /// Convert time parameter to arc length
    /// </summary>
    public abstract Scalar<T> TimeToLength(Scalar<T> t);

    /// <summary>
    /// Convert arc length to time parameter
    /// </summary>
    public abstract Scalar<T> LengthToTime(Scalar<T> length);
}
