using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;

/// <summary>
/// A 2D parametric path computed from user-provided functions
/// Allows flexible path definition via Func delegates
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ComputedPath2D<T> :
    ParametricPath2D<T>
{
    #region Static Factory Methods - Finite

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Finite(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc)
    {
        return new ComputedPath2D<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Finite(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc)
    {
        return new ComputedPath2D<T>(
            timeRange,
            false,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Finite(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative1ValueFunc)
    {
        return new ComputedPath2D<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Finite(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative1ValueFunc)
    {
        return new ComputedPath2D<T>(
            timeRange,
            false,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Finite(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative1ValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative2ValueFunc)
    {
        return new ComputedPath2D<T>(
            ScalarRange<T>.SymmetricOne(processor),
            false,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Finite(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative1ValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative2ValueFunc)
    {
        return new ComputedPath2D<T>(
            timeRange,
            false,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }

    #endregion

    #region Static Factory Methods - Periodic

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Periodic(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc)
    {
        return new ComputedPath2D<T>(
            ScalarRange<T>.SymmetricOne(processor),
            true,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Periodic(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc)
    {
        return new ComputedPath2D<T>(
            timeRange,
            true,
            getValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Periodic(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative1ValueFunc)
    {
        return new ComputedPath2D<T>(
            ScalarRange<T>.SymmetricOne(processor),
            true,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Periodic(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative1ValueFunc)
    {
        return new ComputedPath2D<T>(
            timeRange,
            true,
            getValueFunc,
            getDerivative1ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Periodic(
        IScalarProcessor<T> processor,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative1ValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative2ValueFunc)
    {
        return new ComputedPath2D<T>(
            ScalarRange<T>.SymmetricOne(processor),
            true,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> Periodic(
        ScalarRange<T> timeRange,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative1ValueFunc,
        Func<Scalar<T>, LinVector2D<T>> getDerivative2ValueFunc)
    {
        return new ComputedPath2D<T>(
            timeRange,
            true,
            getValueFunc,
            getDerivative1ValueFunc,
            getDerivative2ValueFunc
        );
    }

    #endregion


    private Func<Scalar<T>, LinVector2D<T>> GetValueFunc { get; }

    private Func<Scalar<T>, LinVector2D<T>>? GetDerivative1ValueFunc { get; }

    private Func<Scalar<T>, LinVector2D<T>>? GetDerivative2ValueFunc { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ComputedPath2D(
        ScalarRange<T> timeRange,
        bool isPeriodic,
        Func<Scalar<T>, LinVector2D<T>> getValueFunc,
        Func<Scalar<T>, LinVector2D<T>>? getDerivative1ValueFunc = null,
        Func<Scalar<T>, LinVector2D<T>>? getDerivative2ValueFunc = null)
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
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new ComputedPath2D<T>(
                TimeRange,
                false,
                GetValueFunc,
                GetDerivative1ValueFunc,
                GetDerivative2ValueFunc
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new ComputedPath2D<T>(
                TimeRange,
                true,
                GetValueFunc,
                GetDerivative1ValueFunc,
                GetDerivative2ValueFunc
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        return GetValueFunc(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        if (GetDerivative1ValueFunc is not null)
            return GetDerivative1ValueFunc(t);

        // Fall back to numerical differentiation if available in base class
        // For Generic<T>, numerical differentiation is complex, so we throw
        throw new NotSupportedException(
            "First derivative not provided for ComputedPath2D. " +
            "Please provide derivative functions in constructor for Generic<T> paths."
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        if (GetDerivative2ValueFunc is not null)
            return GetDerivative2ValueFunc(t);

        // Fall back to numerical differentiation if available in base class
        // For Generic<T>, numerical differentiation is complex, so we throw
        throw new NotSupportedException(
            "Second derivative not provided for ComputedPath2D. " +
            "Please provide derivative functions in constructor for Generic<T> paths."
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        // Get tangent vector from first derivative
        var tangent = GetDerivative1Value(t);
        var tangentNorm = tangent.Norm();

        var normalizedTangent = tangent.ScalarProcessor.IsZero(tangentNorm.ScalarValue)
            ? LinVector2D<T>.UnitSymmetric(tangent.ScalarProcessor)
            : tangent / tangentNorm;

        return ParametricPath2DLocalFrame<T>.Create(
            t,
            GetValue(t),
            normalizedTangent
        );
    }
}
