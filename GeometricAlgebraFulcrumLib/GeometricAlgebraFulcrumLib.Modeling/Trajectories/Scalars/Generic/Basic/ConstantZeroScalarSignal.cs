using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;

/// <summary>
/// Constant scalar signal that always returns 0.0
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ConstantZeroScalarSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantZeroScalarSignal<T> Finite(IScalarProcessor<T> scalarProcessor)
    {
        return new ConstantZeroScalarSignal<T>(scalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantZeroScalarSignal<T> Periodic(IScalarProcessor<T> scalarProcessor)
    {
        return new ConstantZeroScalarSignal<T>(scalarProcessor, true);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ConstantZeroScalarSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
        : base(ScalarRange<T>.SymmetricOne(scalarProcessor), isPeriodic)
    {
        Debug.Assert(IsValid());
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
            : new ConstantZeroScalarSignal<T>(ScalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ConstantZeroScalarSignal<T>(ScalarProcessor, true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        return ScalarProcessor.Zero.ToScalar();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        return ScalarProcessor.Zero.ToScalar();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        return ScalarProcessor.Zero.ToScalar();
    }
}
