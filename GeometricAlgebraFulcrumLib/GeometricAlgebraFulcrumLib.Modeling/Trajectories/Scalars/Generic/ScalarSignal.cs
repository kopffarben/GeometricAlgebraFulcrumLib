using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;

/// <summary>
/// Abstract base class for scalar-valued signals with generic scalar type T
/// </summary>
/// <typeparam name="T">Scalar type for both time and value</typeparam>
/// <remarks>
/// SIMPLIFIED VERSION: This implementation does NOT include:
/// - All factory methods from Float64ScalarSignal (will be added incrementally)
/// - Operator overloading (will be added after basic functionality works)
/// - Numerical differentiation (requires careful handling for Generic T)
/// - Value range caching (will be added after basic implementations work)
/// These features will be added incrementally after the core functionality is validated.
/// </remarks>
public abstract class ScalarSignal<T>(ScalarRange<T> timeRange, bool isPeriodic) :
    Trajectory<T, Scalar<T>>(timeRange, isPeriodic)
{
    /// <summary>
    /// Gets the scalar processor for this signal
    /// </summary>
    public IScalarProcessor<T> ScalarProcessor
        => TimeRange.MinValue.ScalarProcessor;

    public abstract ScalarSignal<T> ToFiniteSignal();

    public abstract ScalarSignal<T> ToPeriodicSignal();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ITrajectory<T> ToFinite()
    {
        return ToFiniteSignal();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ITrajectory<T> ToPeriodic()
    {
        return ToPeriodicSignal();
    }

    /// <summary>
    /// Get the first derivative at time t.
    /// Subclasses should override this with analytical derivatives for best accuracy.
    /// Default implementation throws NotSupportedException - numerical differentiation
    /// for Generic T requires special handling.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        throw new NotSupportedException(
            $"First derivative not implemented for {GetType().Name}. " +
            "Numerical differentiation for Generic<T> requires Math operations " +
            "which may not be available for all scalar types.");
    }

    /// <summary>
    /// Get the second derivative at time t.
    /// Subclasses should override this with analytical derivatives for best accuracy.
    /// Default implementation throws NotSupportedException - numerical differentiation
    /// for Generic T requires special handling.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        throw new NotSupportedException(
            $"Second derivative not implemented for {GetType().Name}. " +
            "Numerical differentiation for Generic<T> requires Math operations " +
            "which may not be available for all scalar types.");
    }
}
