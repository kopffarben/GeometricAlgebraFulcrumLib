using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

/// <summary>
/// A computed 3D path that uses arbitrary functions to compute position and derivatives
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ComputedPath3D<T> :
    ParametricPath3D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Finite(IScalarProcessor<T> scalarProcessor, Func<Scalar<T>, LinVector3D<T>> getPointFunc)
    {
        var timeRange = ScalarRange<T>.SymmetricOne(scalarProcessor);
        return new ComputedPath3D<T>(timeRange, false, getPointFunc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Finite(IScalarProcessor<T> scalarProcessor, ScalarRange<T> timeRange, Func<Scalar<T>, T> xFunc, Func<Scalar<T>, T> yFunc, Func<Scalar<T>, T> zFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            false,
            t =>
                LinVector3D<T>.Create(
                    scalarProcessor.ScalarFromValue(xFunc(t)),
                    scalarProcessor.ScalarFromValue(yFunc(t)),
                    scalarProcessor.ScalarFromValue(zFunc(t))
                )
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Finite(ScalarRange<T> timeRange, Func<Scalar<T>, LinVector3D<T>> getValueFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            false,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Finite(ScalarRange<T> timeRange, Func<Scalar<T>, LinVector3D<T>> getValueFunc, Func<Scalar<T>, LinVector3D<T>> getDerivative1ValueFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            false,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Finite(ScalarRange<T> timeRange, Func<Scalar<T>, LinVector3D<T>> getValueFunc, Func<Scalar<T>, LinVector3D<T>> getDerivative1ValueFunc, Func<Scalar<T>, LinVector3D<T>> getDerivative2ValueFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            false,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Finite(IScalarProcessor<T> scalarProcessor, Scalar<T> timeMin, Scalar<T> timeMax, Func<Scalar<T>, LinVector3D<T>> getValueFunc)
    {
        var timeRange = ScalarRange<T>.Create(timeMin, timeMax);

        return new ComputedPath3D<T>(
            timeRange,
            false,
            getValueFunc
        );
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Periodic(IScalarProcessor<T> scalarProcessor, Func<Scalar<T>, LinVector3D<T>> getPointFunc)
    {
        var timeRange = ScalarRange<T>.SymmetricOne(scalarProcessor);
        return new ComputedPath3D<T>(timeRange, true, getPointFunc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Periodic(IScalarProcessor<T> scalarProcessor, ScalarRange<T> timeRange, Func<Scalar<T>, T> xFunc, Func<Scalar<T>, T> yFunc, Func<Scalar<T>, T> zFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            true,
            t =>
                LinVector3D<T>.Create(
                    scalarProcessor.ScalarFromValue(xFunc(t)),
                    scalarProcessor.ScalarFromValue(yFunc(t)),
                    scalarProcessor.ScalarFromValue(zFunc(t))
                )
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Periodic(ScalarRange<T> timeRange, Func<Scalar<T>, LinVector3D<T>> getValueFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            true,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Periodic(ScalarRange<T> timeRange, Func<Scalar<T>, LinVector3D<T>> getValueFunc, Func<Scalar<T>, LinVector3D<T>> getDerivative1ValueFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            true,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Periodic(ScalarRange<T> timeRange, Func<Scalar<T>, LinVector3D<T>> getValueFunc, Func<Scalar<T>, LinVector3D<T>> getDerivative1ValueFunc, Func<Scalar<T>, LinVector3D<T>> getDerivative2ValueFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            true,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Periodic(IScalarProcessor<T> scalarProcessor, Scalar<T> timeMin, Scalar<T> timeMax, Func<Scalar<T>, LinVector3D<T>> getValueFunc)
    {
        var timeRange = ScalarRange<T>.Create(timeMin, timeMax);

        return new ComputedPath3D<T>(
            timeRange,
            true,
            getValueFunc
        );
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Create(IScalarProcessor<T> scalarProcessor, bool isPeriodic, Func<Scalar<T>, LinVector3D<T>> getPointFunc)
    {
        var timeRange = ScalarRange<T>.SymmetricOne(scalarProcessor);
        return new ComputedPath3D<T>(timeRange, isPeriodic, getPointFunc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, Func<Scalar<T>, T> xFunc, Func<Scalar<T>, T> yFunc, Func<Scalar<T>, T> zFunc)
    {
        var scalarProcessor = timeRange.ScalarProcessor;

        return new ComputedPath3D<T>(
            timeRange,
            isPeriodic,
            t =>
                LinVector3D<T>.Create(
                    scalarProcessor.ScalarFromValue(xFunc(t)),
                    scalarProcessor.ScalarFromValue(yFunc(t)),
                    scalarProcessor.ScalarFromValue(zFunc(t))
                )
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, Func<Scalar<T>, LinVector3D<T>> getValueFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            isPeriodic,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, Func<Scalar<T>, LinVector3D<T>> getValueFunc, Func<Scalar<T>, LinVector3D<T>> getDerivative1ValueFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            isPeriodic,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, Func<Scalar<T>, LinVector3D<T>> getValueFunc, Func<Scalar<T>, LinVector3D<T>> getDerivative1ValueFunc, Func<Scalar<T>, LinVector3D<T>> getDerivative2ValueFunc)
    {
        return new ComputedPath3D<T>(
            timeRange,
            isPeriodic,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> Create(Scalar<T> timeMin, Scalar<T> timeMax, bool isPeriodic, Func<Scalar<T>, LinVector3D<T>> getValueFunc)
    {
        var timeRange = ScalarRange<T>.Create(timeMin, timeMax);

        return new ComputedPath3D<T>(
            timeRange,
            isPeriodic,
            getValueFunc
        );
    }


    private Func<Scalar<T>, LinVector3D<T>> GetValueFunc { get; }

    private Func<Scalar<T>, LinVector3D<T>>? GetDerivative1ValueFunc { get; }

    private Func<Scalar<T>, LinVector3D<T>>? GetDerivative2ValueFunc { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ComputedPath3D(ScalarRange<T> timeRange, bool isPeriodic, Func<Scalar<T>, LinVector3D<T>> getValueFunc, Func<Scalar<T>, LinVector3D<T>>? getDerivative1ValueFunc = null, Func<Scalar<T>, LinVector3D<T>>? getDerivative2ValueFunc = null)
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
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new ComputedPath3D<T>(
                TimeRange,
                false,
                GetValueFunc,
                GetDerivative1ValueFunc,
                GetDerivative2ValueFunc
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new ComputedPath3D<T>(
                TimeRange,
                true,
                GetValueFunc,
                GetDerivative1ValueFunc,
                GetDerivative2ValueFunc
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Scalar<T> ClampTime(Scalar<T> t)
    {
        // Clamp time to valid range based on periodicity
        var processor = t.ScalarProcessor;

        if (IsPeriodic)
        {
            // For periodic paths, wrap t into [MinTime, MaxTime] range
            var range = TimeRange.MaxValue - TimeRange.MinValue;
            var tNorm = (t - TimeRange.MinValue) / range;

            // Get fractional part (equivalent to modulo for Generic<T>)
            var tNormValue = tNorm.ScalarValue;
            if (processor is IScalarProcessor<double> doubleProc)
            {
                var tNormDouble = Convert.ToDouble(tNormValue);
                var tWrapped = tNormDouble - Math.Floor(tNormDouble);
                return processor.ScalarFromValue((T)(object)tWrapped) * range + TimeRange.MinValue;
            }

            // Fallback: just return t (periodic wrapping requires double conversion)
            return t;
        }
        else
        {
            // For finite paths, clamp to [MinTime, MaxTime]
            // Use Scalar<T> comparison operators
            if (t < TimeRange.MinValue)
                return TimeRange.MinValue;

            if (t > TimeRange.MaxValue)
                return TimeRange.MaxValue;

            return t;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        return GetValueFunc(ClampTime(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        if (GetDerivative1ValueFunc is not null)
            return GetDerivative1ValueFunc(ClampTime(t));

        // Numerical differentiation not available for Generic<T>
        // MathNet.Numerics.Differentiate is hardcoded to double
        throw new NotImplementedException(
            "Numerical differentiation is not available for Generic<T>. " +
            "Please provide an explicit derivative function when creating ComputedPath3D<T>."
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        if (GetDerivative2ValueFunc is not null)
            return GetDerivative2ValueFunc(ClampTime(t));

        // Numerical differentiation not available for Generic<T>
        throw new NotImplementedException(
            "Numerical differentiation is not available for Generic<T>. " +
            "Please provide an explicit second derivative function when creating ComputedPath3D<T>."
        );
    }
}
