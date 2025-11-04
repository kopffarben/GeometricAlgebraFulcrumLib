using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic;

/// <summary>
/// A parametric 2D curve with generic scalar type T for time parameterization
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
/// <remarks>
/// SIMPLIFIED VERSION: This implementation does NOT include:
/// - FindValueRange() / GetValueRange() (requires optimization/sampling)
/// - Numerical differentiation methods (MathNet.Numerics is hardcoded to double)
/// These features may be added in future updates.
/// </remarks>
public abstract class ParametricPath2D<T>(ScalarRange<T> timeRange, bool isPeriodic) :
    Trajectory<T, LinVector2D<T>>(timeRange, isPeriodic)
{
    public abstract ParametricPath2D<T> ToFinitePath();

    public abstract ParametricPath2D<T> ToPeriodicPath();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ITrajectory<T> ToFinite()
    {
        return ToFinitePath();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ITrajectory<T> ToPeriodic()
    {
        return ToPeriodicPath();
    }


    /// <summary>
    /// Get the first derivative (velocity) at time t.
    /// Subclasses should override this with analytical derivatives for best accuracy.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract LinVector2D<T> GetDerivative1Value(Scalar<T> t);

    /// <summary>
    /// Get the second derivative (acceleration) at time t.
    /// Subclasses should override this with analytical derivatives for best accuracy.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract LinVector2D<T> GetDerivative2Value(Scalar<T> t);

    /// <summary>
    /// Get the local frame (position, tangent, normal) at time t
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        return ParametricPath2DLocalFrame<T>.Create(
            t,
            GetValue(t),
            GetDerivative1Value(t)
        );
    }

    /// <summary>
    /// Get the X and Y components as separate scalar signals.
    /// This creates computed signals that wrap the path's GetValue/GetDerivative methods.
    /// Override this method in derived classes for more efficient implementations.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual Pair<ScalarSignal<T>> GetScalarComponents()
    {
        if (IsFinite)
            return new Pair<ScalarSignal<T>>(
                ComputedScalarSignal<T>.Finite(
                    TimeRange,
                    t => GetValue(t).Item1,
                    t => GetDerivative1Value(t).Item1,
                    t => GetDerivative2Value(t).Item1
                ),

                ComputedScalarSignal<T>.Finite(
                    TimeRange,
                    t => GetValue(t).Item2,
                    t => GetDerivative1Value(t).Item2,
                    t => GetDerivative2Value(t).Item2
                )
            );

        return new Pair<ScalarSignal<T>>(
            ComputedScalarSignal<T>.Periodic(
                TimeRange,
                t => GetValue(t).Item1,
                t => GetDerivative1Value(t).Item1,
                t => GetDerivative2Value(t).Item1
            ),

            ComputedScalarSignal<T>.Periodic(
                TimeRange,
                t => GetValue(t).Item2,
                t => GetDerivative1Value(t).Item2,
                t => GetDerivative2Value(t).Item2
            )
        );
    }
}
