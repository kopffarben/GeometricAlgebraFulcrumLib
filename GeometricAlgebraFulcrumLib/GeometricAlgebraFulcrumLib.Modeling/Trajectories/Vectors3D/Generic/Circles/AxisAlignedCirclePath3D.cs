using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Circles;

/// <summary>
/// Base class for circle paths aligned to coordinate planes (XY, YZ, ZX)
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public abstract class AxisAlignedCirclePath3D<T>(ScalarRange<T> timeRange, bool isPeriodic) :
    ArcLengthPath3D<T>(timeRange, isPeriodic)
{
    /// <summary>
    /// Radius of the circle
    /// </summary>
    public abstract Scalar<T> Radius { get; }

    /// <summary>
    /// Center point of the circle
    /// </summary>
    public abstract LinVector3D<T> Center { get; }

    /// <summary>
    /// Unit normal vector perpendicular to the circle plane
    /// </summary>
    public abstract LinVector3D<T> UnitNormal { get; }

    /// <summary>
    /// Number of complete rotations around the circle (positive = counterclockwise, negative = clockwise)
    /// </summary>
    public abstract int RotationCount { get; }
}
