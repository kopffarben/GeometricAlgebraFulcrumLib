using System;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Angles;

/// <summary>
/// Time-varying polar angle for Generic&lt;T&gt; trajectories.
/// Minimal implementation to match Float64PolarAngleTimeSignal use cases.
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class LinPolarAngleTimeSignal<T> :
    Trajectory<T, LinPolarAngle<T>>
{
    private readonly Func<Scalar<T>, LinPolarAngle<T>> _getAngleFunc;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinPolarAngleTimeSignal<T> Create(
        ScalarRange<T> timeRange,
        bool isPeriodic,
        Func<Scalar<T>, LinPolarAngle<T>> getAngleFunc)
    {
        return new LinPolarAngleTimeSignal<T>(timeRange, isPeriodic, getAngleFunc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinPolarAngleTimeSignal<T> CreateConstant(
        ScalarRange<T> timeRange,
        bool isPeriodic,
        LinPolarAngle<T> angle)
    {
        return new LinPolarAngleTimeSignal<T>(
            timeRange,
            isPeriodic,
            _ => angle
        );
    }

    private LinPolarAngleTimeSignal(
        ScalarRange<T> timeRange,
        bool isPeriodic,
        Func<Scalar<T>, LinPolarAngle<T>> getAngleFunc)
        : base(timeRange, isPeriodic)
    {
        _getAngleFunc = getAngleFunc;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        // Delegate-based implementation, assume validity comes from source signal
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ITrajectory<T> ToFinite()
    {
        return IsFinite
            ? this
            : new LinPolarAngleTimeSignal<T>(TimeRange, false, _getAngleFunc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ITrajectory<T> ToPeriodic()
    {
        return IsPeriodic
            ? this
            : new LinPolarAngleTimeSignal<T>(TimeRange, true, _getAngleFunc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinPolarAngle<T> GetValue(Scalar<T> t)
    {
        return _getAngleFunc(TimeRange.Clamp(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinPolarAngle<T> GetAngle(Scalar<T> t)
    {
        return GetValue(t);
    }
}
