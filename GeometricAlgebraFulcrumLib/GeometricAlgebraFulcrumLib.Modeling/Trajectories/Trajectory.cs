using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories;

/// <summary>
/// Generic trajectory base class with scalar type T for time parameterization
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public abstract class Trajectory<T> :
    ITrajectory<T>
{
    public bool IsPeriodic { get; }

    public bool IsFinite
        => !IsPeriodic;

    public ScalarRange<T> TimeRange { get; }

    public Scalar<T> MinTime
        => TimeRange.MinValue;

    public Scalar<T> MidTime
        => TimeRange.MidValue;

    public Scalar<T> MaxTime
        => TimeRange.MaxValue;

    public Scalar<T> TimeRangeLength
        => TimeRange.Length;


    protected Trajectory(ScalarRange<T> timeRange, bool isPeriodic)
    {
        if (!timeRange.IsValid() || !timeRange.IsFinite)
            throw new ArgumentException(nameof(timeRange));

        TimeRange = timeRange;
        IsPeriodic = isPeriodic;
    }


    public abstract bool IsValid();

    public abstract ITrajectory<T> ToFinite();

    public abstract ITrajectory<T> ToPeriodic();
}

/// <summary>
/// Generic trajectory base class with scalar type T for time and value type TValue
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
/// <typeparam name="TValue">Value type returned by trajectory</typeparam>
public abstract class Trajectory<T, TValue>(ScalarRange<T> timeRange, bool isPeriodic) :
    Trajectory<T>(timeRange, isPeriodic),
    ITrajectory<T, TValue>
{
    public TValue ValueAtMinTime
        => GetValue(MinTime);

    public TValue ValueAtMidTime
        => GetValue(MidTime);

    public TValue ValueAtMaxTime
        => GetValue(MaxTime);


    public abstract TValue GetValue(Scalar<T> t);
}
