using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

/// <summary>
/// A scalar signal that represents the derivative of another scalar signal
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ScalarDerivativeSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarDerivativeSignal<T> Create(ScalarSignal<T> baseSignal)
    {
        return new ScalarDerivativeSignal<T>(baseSignal);
    }


    public ScalarSignal<T> BaseSignal { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarDerivativeSignal(ScalarSignal<T> baseSignal)
        : base(baseSignal.TimeRange, baseSignal.IsPeriodic)
    {
        BaseSignal = baseSignal;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseSignal.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return IsFinite
            ? this
            : new ScalarDerivativeSignal<T>(
                BaseSignal.ToFiniteSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ScalarDerivativeSignal<T>(
                BaseSignal.ToPeriodicSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        // The value of the derivative signal is the first derivative of the base signal
        return BaseSignal.GetDerivative1Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        // The first derivative of the derivative signal is the second derivative of the base signal
        return BaseSignal.GetDerivative2Value(t);
    }

    // Note: GetDerivative2Value is not implemented for generic types
    // It would require numerical differentiation which is only available for double
    // The base class implementation will throw NotSupportedException
}
