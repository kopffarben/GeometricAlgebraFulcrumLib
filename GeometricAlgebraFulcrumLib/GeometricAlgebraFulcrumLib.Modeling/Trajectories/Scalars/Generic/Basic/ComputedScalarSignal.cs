using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;

/// <summary>
/// A scalar signal computed from user-provided functions
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ComputedScalarSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Finite(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, Scalar<T>> getValueFunc)
    {
        return new ComputedScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Finite(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, Scalar<T>> getValueFunc)
    {
        return new ComputedScalarSignal<T>(
            timeRange,
            false,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Finite(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, Scalar<T>> getValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative1ValueFunc)
    {
        return new ComputedScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Finite(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, Scalar<T>> getValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative1ValueFunc)
    {
        return new ComputedScalarSignal<T>(
            timeRange,
            false,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Finite(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, Scalar<T>> getValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative1ValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative2ValueFunc)
    {
        return new ComputedScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Finite(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, Scalar<T>> getValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative1ValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative2ValueFunc)
    {
        return new ComputedScalarSignal<T>(
            timeRange,
            false,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Periodic(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, Scalar<T>> getValueFunc)
    {
        return new ComputedScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            true,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Periodic(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, Scalar<T>> getValueFunc)
    {
        return new ComputedScalarSignal<T>(
            timeRange,
            true,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Periodic(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, Scalar<T>> getValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative1ValueFunc)
    {
        return new ComputedScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            true,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Periodic(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, Scalar<T>> getValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative1ValueFunc)
    {
        return new ComputedScalarSignal<T>(
            timeRange,
            true,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Periodic(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, Scalar<T>> getValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative1ValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative2ValueFunc)
    {
        return new ComputedScalarSignal<T>(
            ScalarRange<T>.SymmetricOne(processor),
            true,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> Periodic(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, Scalar<T>> getValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative1ValueFunc,
        Func<Scalar<T>, Scalar<T>> getDerivative2ValueFunc)
    {
        return new ComputedScalarSignal<T>(
            timeRange,
            true,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }


    private Func<Scalar<T>, Scalar<T>> GetValueFunc { get; }

    private Func<Scalar<T>, Scalar<T>>? GetDerivative1ValueFunc { get; }

    private Func<Scalar<T>, Scalar<T>>? GetDerivative2ValueFunc { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ComputedScalarSignal(
        ScalarRange<T> timeRange,
        bool isPeriodic,
        Func<Scalar<T>, Scalar<T>> getValueFunc,
        Func<Scalar<T>, Scalar<T>>? getDerivative1ValueFunc = null,
        Func<Scalar<T>, Scalar<T>>? getDerivative2ValueFunc = null)
        : base(timeRange, isPeriodic)
    {
        GetValueFunc = getValueFunc;
        GetDerivative1ValueFunc = getDerivative1ValueFunc;
        GetDerivative2ValueFunc = getDerivative2ValueFunc;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new ComputedScalarSignal<T>(
                TimeRange,
                false,
                GetValueFunc,
                GetDerivative1ValueFunc,
                GetDerivative2ValueFunc
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ComputedScalarSignal<T>(
                TimeRange,
                true,
                GetValueFunc,
                GetDerivative1ValueFunc,
                GetDerivative2ValueFunc
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        return GetValueFunc(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        if (GetDerivative1ValueFunc is not null)
            return GetDerivative1ValueFunc(t);

        // Fall back to base class (throws NotSupportedException)
        return base.GetDerivative1Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        if (GetDerivative2ValueFunc is not null)
            return GetDerivative2ValueFunc(t);

        // Fall back to base class (throws NotSupportedException)
        return base.GetDerivative2Value(t);
    }
}
