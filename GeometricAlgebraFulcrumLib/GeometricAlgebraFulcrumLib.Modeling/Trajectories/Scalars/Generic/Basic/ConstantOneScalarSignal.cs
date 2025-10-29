using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;

/// <summary>
/// Constant scalar signal that always returns 1.0
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ConstantOneScalarSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantOneScalarSignal<T> Finite(IScalarProcessor<T> scalarProcessor)
    {
        return new ConstantOneScalarSignal<T>(scalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantOneScalarSignal<T> Periodic(IScalarProcessor<T> scalarProcessor)
    {
        return new ConstantOneScalarSignal<T>(scalarProcessor, true);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ConstantOneScalarSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
        : base(ScalarRange<T>.SymmetricOne(scalarProcessor), isPeriodic)
    {
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
            : new ConstantOneScalarSignal<T>(ScalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return IsPeriodic
            ? this
            : new ConstantOneScalarSignal<T>(ScalarProcessor, true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        return ScalarProcessor.One.ToScalar();
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
