using GeometricAlgebraFulcrumLib.Algebra;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories;

/// <summary>
/// Generic trajectory interface with scalar type T for time parameterization
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public interface ITrajectory<T> :
    IAlgebraicElement
{
    bool IsPeriodic { get; }

    ScalarRange<T> TimeRange { get; }

    Scalar<T> MinTime { get; }

    Scalar<T> MaxTime { get; }

    Scalar<T> MidTime { get; }

    Scalar<T> TimeRangeLength { get; }

    ITrajectory<T> ToFinite();

    ITrajectory<T> ToPeriodic();
}

/// <summary>
/// Generic trajectory interface with scalar type T for time and value type TValue
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
/// <typeparam name="TValue">Value type returned by trajectory</typeparam>
public interface ITrajectory<T, out TValue> :
    ITrajectory<T>
{
    TValue ValueAtMinTime { get; }

    TValue ValueAtMidTime { get; }

    TValue ValueAtMaxTime { get; }

    TValue GetValue(Scalar<T> t);
}
